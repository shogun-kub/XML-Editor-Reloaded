using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Schema;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Editable table view: a virtualized grid over <see cref="TableWorkspace"/> with schema-driven
/// columns, sparse cell editing, row operations, search, sorting, validation, and saving.
/// </summary>
public partial class TableEditorViewModel : ObservableObject, ITableCellEditor
{
    private readonly IDataSetSession _session;
    private readonly IItemImageService _itemImages;
    private string? _languageCode;
    private readonly IDialogService _dialogService;
    private readonly List<TableRowViewModel> _allRows = new();
    private readonly List<TableColumnViewModel> _allColumns = new();
    private readonly List<DetailFieldViewModel> _allDetailFields = new();
    private readonly Dictionary<string, LookupColumnData> _lookupByField = new(StringComparer.Ordinal);
    private readonly Dictionary<string, bool> _groupExpansion = new(StringComparer.Ordinal);
    private CancellationTokenSource? _imageCts;
    private bool _suppressLanguageChange;
    private bool _isChangingLanguage;
    private const int DefaultColumnBudget = 8;
    private const NumberStyles DecimalStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    [ObservableProperty]
    private TableWorkspace? _workspace;

    [ObservableProperty]
    private string _displayName;

    [ObservableProperty]
    private string _statusText;

    [ObservableProperty]
    private string _recordCountText = string.Empty;

    [ObservableProperty]
    private string _languageText = string.Empty;

    [ObservableProperty]
    private string _visibleColumnCountText = string.Empty;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _columnSearchText = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isDirty;

    [ObservableProperty]
    private bool _isEditable;

    [ObservableProperty]
    private bool _isValidationVisible;

    [ObservableProperty]
    private TableRowViewModel? _selectedRow;

    [ObservableProperty]
    private string _sortField = string.Empty;

    [ObservableProperty]
    private ListSortDirection _sortDirection = ListSortDirection.Ascending;

    [ObservableProperty]
    private bool _showDetails = true;

    [ObservableProperty]
    private string _detailFilter = string.Empty;

    [ObservableProperty]
    private string _detailHeader = string.Empty;

    [ObservableProperty]
    private bool _hasGraphicFields;

    [ObservableProperty]
    private LanguageOptionViewModel? _selectedLanguageOption;

    [ObservableProperty]
    private bool _isTranslationMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableEditorViewModel"/> class.
    /// </summary>
    /// <param name="session">Data-set session that opens the table.</param>
    /// <param name="descriptor">Table descriptor to edit.</param>
    /// <param name="languageCode">Optional language code for overlay editing.</param>
    /// <param name="dialogService">Dialog service used for confirmations and bit editing.</param>
    /// <param name="itemImages">Item image service used by the detail panel.</param>
    public TableEditorViewModel(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode,
        IDialogService dialogService,
        IItemImageService itemImages)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(itemImages);

        _session = session;
        Descriptor = descriptor;
        _languageCode = languageCode;
        _dialogService = dialogService;
        _itemImages = itemImages;

        DetailImages = new[]
        {
            new DetailImagePanelViewModel(ImageSize.Big, Strings.Get("ImageBigLabel"), 170),
            new DetailImagePanelViewModel(ImageSize.Medium, Strings.Get("ImageMediumLabel"), 90),
            new DetailImagePanelViewModel(ImageSize.Small, Strings.Get("ImageSmallLabel"), 50),
        };

        Title = descriptor.TableName.Value;
        _displayName = Title;
        _statusText = Strings.Get("EditorLoading");

        AddRowText = Strings.Get("EditorAddRowText");
        DuplicateRowText = Strings.Get("EditorDuplicateRowText");
        DeleteRowText = Strings.Get("EditorDeleteRowText");
        SaveText = Strings.Get("EditorSaveText");
        ValidateText = Strings.Get("EditorValidateText");
        ReloadText = Strings.Get("EditorReloadText");
        SearchLabel = Strings.Get("EditorSearchLabel");
        ColumnsLabel = Strings.Get("EditorColumnsLabel");
        ColumnSearchLabel = Strings.Get("EditorColumnSearchLabel");
        DefaultColumnsText = Strings.Get("EditorDefaultColumnsText");
        AllColumnsText = Strings.Get("EditorAllColumnsText");
        NoColumnsText = Strings.Get("EditorNoColumnsText");
        ValidationLabel = Strings.Get("EditorValidationLabel");
        DetailLabel = Strings.Get("DetailLabel");
        DetailFilterLabel = Strings.Get("DetailFilterLabel");
        DetailBitsText = Strings.Get("DetailBitsText");
        DetailUntranslatedText = Strings.Get("EditorUntranslated");
        LanguageLabel = Strings.Get("EditorLanguageLabel");
        TranslationReportText = Strings.Get("EditorTranslationReportText");
        TranslatableColumnSuffix = Strings.Get("EditorTranslatableSuffix");
    }

    /// <summary>
    /// Occurs after the editor successfully switches to another language or the base table.
    /// </summary>
    public event EventHandler<string?>? LanguageChanged;

    /// <summary>
    /// Gets the table descriptor. It is replaced when a new language overlay is created.
    /// </summary>
    public TableDescriptor Descriptor { get; private set; }

    /// <summary>
    /// Gets the table title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets a value indicating whether the grid must be read-only.
    /// </summary>
    public bool IsReadOnly => !IsEditable;

    /// <summary>
    /// Gets the add-row button text.
    /// </summary>
    public string AddRowText { get; }

    /// <summary>
    /// Gets the duplicate-row button text.
    /// </summary>
    public string DuplicateRowText { get; }

    /// <summary>
    /// Gets the delete-row button text.
    /// </summary>
    public string DeleteRowText { get; }

    /// <summary>
    /// Gets the save button text.
    /// </summary>
    public string SaveText { get; }

    /// <summary>
    /// Gets the validate button text.
    /// </summary>
    public string ValidateText { get; }

    /// <summary>
    /// Gets the reload button text.
    /// </summary>
    public string ReloadText { get; }

    /// <summary>
    /// Gets the search label.
    /// </summary>
    public string SearchLabel { get; }

    /// <summary>
    /// Gets the column chooser label.
    /// </summary>
    public string ColumnsLabel { get; }

    /// <summary>
    /// Gets the column search label.
    /// </summary>
    public string ColumnSearchLabel { get; }

    /// <summary>
    /// Gets the default-columns button text.
    /// </summary>
    public string DefaultColumnsText { get; }

    /// <summary>
    /// Gets the show-all-columns button text.
    /// </summary>
    public string AllColumnsText { get; }

    /// <summary>
    /// Gets the empty-column-list text.
    /// </summary>
    public string NoColumnsText { get; }

    /// <summary>
    /// Gets the validation panel label.
    /// </summary>
    public string ValidationLabel { get; }

    /// <summary>
    /// Gets the detail panel label.
    /// </summary>
    public string DetailLabel { get; }

    /// <summary>
    /// Gets the detail field filter label.
    /// </summary>
    public string DetailFilterLabel { get; }

    /// <summary>
    /// Gets the bit editor button text.
    /// </summary>
    public string DetailBitsText { get; }

    /// <summary>
    /// Gets the tooltip shown for a translatable field that still uses its base value.
    /// </summary>
    public string DetailUntranslatedText { get; }

    /// <summary>
    /// Gets the language selector label.
    /// </summary>
    public string LanguageLabel { get; }

    /// <summary>
    /// Gets the translation report button text.
    /// </summary>
    public string TranslationReportText { get; }

    /// <summary>
    /// Gets the suffix appended to translatable grid column headers in translation mode.
    /// </summary>
    public string TranslatableColumnSuffix { get; }

    /// <summary>
    /// Gets a value indicating whether the language selector can be used.
    /// </summary>
    public bool IsLanguageSelectorEnabled => !IsBusy && Workspace is not null;

    /// <summary>
    /// Gets the language selector options: the base table and every configured language.
    /// </summary>
    public ObservableCollection<LanguageOptionViewModel> LanguageOptions { get; } = new();

    /// <summary>
    /// Gets the big, medium, and small image preview panels.
    /// </summary>
    public IReadOnlyList<DetailImagePanelViewModel> DetailImages { get; }

    /// <summary>
    /// Gets the grouped fields of the selected row shown in the detail panel, filtered by
    /// <see cref="DetailFilter"/>.
    /// </summary>
    public ObservableCollection<DetailFieldGroupViewModel> DetailGroups { get; } = new();

    /// <summary>
    /// Gets the filtered rows shown in the grid.
    /// </summary>
    public ObservableCollection<TableRowViewModel> Rows { get; } = new();

    /// <summary>
    /// Gets the columns shown in the column chooser, filtered by <see cref="ColumnSearchText"/>.
    /// </summary>
    public ObservableCollection<TableColumnViewModel> Columns { get; } = new();

    /// <summary>
    /// Gets the visible columns in schema order.
    /// </summary>
    public ObservableCollection<TableColumnViewModel> VisibleColumns { get; } = new();

    /// <summary>
    /// Gets validation issues from the last validation run.
    /// </summary>
    public ObservableCollection<ValidationIssueItem> ValidationIssues { get; } = new();

    /// <summary>
    /// Loads the workspace and builds the schema-driven grid.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>A task that completes when the table is ready.</returns>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        StatusText = Strings.Get("EditorLoading");
        HasGraphicFields = false;
        IsTranslationMode = false;
        try
        {
            IReadOnlyList<LanguageDescriptor> configuredLanguages = Array.Empty<LanguageDescriptor>();
            var languagesResult = await _session.GetLanguagesAsync(cancellationToken).ConfigureAwait(true);
            if (languagesResult.IsSuccess)
            {
                configuredLanguages = languagesResult.Data!;
            }

            var result = await _session
                .OpenTableAsync(Descriptor, _languageCode, cancellationToken)
                .ConfigureAwait(true);
            if (!result.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
                IsEditable = false;
                return;
            }

            Workspace = result.Data!;
            IsTranslationMode = Workspace.Language is not null;
            LanguageText = Workspace.Language is null
                ? Strings.Get("EditorBaseLanguage")
                : Strings.Format("EditorLanguageFormat", Workspace.Language.DisplayName);
            BuildLanguageOptions(configuredLanguages);
            IsEditable = Workspace.CanEdit && Workspace.BaseDocument.Records.Count > 0;
            await LoadLookupsAsync(cancellationToken).ConfigureAwait(true);
            BuildColumns();
            HasGraphicFields = Workspace.Schema.FindColumn("ubGraphicNum") is not null;
            RebuildRows(null);
            StatusText = IsEditable
                ? Strings.Get("EditorReady")
                : Workspace.BaseDocument.Records.Count == 0
                    ? Strings.Get("EditorReadOnlyNested")
                    : Strings.Get("EditorReadOnly");
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("EditorLoadingCancelled");
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
            IsEditable = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Saves the base document and overlay when there are unsaved changes.
    /// </summary>
    /// <returns>A task that completes when saving finishes.</returns>
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (Workspace is null)
        {
            return;
        }

        if (!Workspace.IsDirty)
        {
            IsDirty = false;
            StatusText = Strings.Get("EditorNoChanges");
            return;
        }

        IsBusy = true;
        try
        {
            var result = await Workspace.SaveAsync().ConfigureAwait(true);
            if (result.IsSuccess)
            {
                IsDirty = false;
                StatusText = Strings.Get("EditorSaved");
            }
            else
            {
                StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            }
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("EditorLoadingCancelled");
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Appends a new record with the next primary key.
    /// </summary>
    [RelayCommand]
    public void AddRow()
    {
        if (Workspace is null)
        {
            return;
        }

        var result = Workspace.AddRecord();
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = Workspace.IsDirty;
        var key = result.Data!.GetKey();
        RebuildRows(key);
        StatusText = Strings.Format("EditorRowAdded", key ?? string.Empty);
    }

    /// <summary>
    /// Duplicates the selected record.
    /// </summary>
    [RelayCommand]
    public void DuplicateRow()
    {
        if (Workspace is null)
        {
            return;
        }

        if (SelectedRow?.Key is not { } key)
        {
            StatusText = Strings.Get("EditorSelectRow");
            return;
        }

        var result = Workspace.DuplicateRecord(key);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = Workspace.IsDirty;
        var newKey = result.Data!.GetKey();
        RebuildRows(newKey);
        StatusText = Strings.Format("EditorRowDuplicated", key, newKey ?? string.Empty);
    }

    /// <summary>
    /// Deletes the selected record and its translation after confirmation.
    /// </summary>
    [RelayCommand]
    public void DeleteRow()
    {
        if (Workspace is null)
        {
            return;
        }

        if (SelectedRow?.Key is not { } key)
        {
            StatusText = Strings.Get("EditorSelectRow");
            return;
        }

        if (!_dialogService.Confirm(
                Strings.Format("EditorConfirmDelete", key),
                Strings.Get("EditorDeleteTitle")))
        {
            return;
        }

        var result = Workspace.DeleteRecord(key);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = Workspace.IsDirty;
        RebuildRows(null);
        StatusText = Strings.Format("EditorRowDeleted", key);
    }

    /// <summary>
    /// Runs the read-only validator and lists its issues.
    /// </summary>
    /// <returns>A task that completes when validation finishes.</returns>
    [RelayCommand]
    public async Task ValidateAsync()
    {
        if (Workspace is null)
        {
            return;
        }

        IsBusy = true;
        try
        {
            var report = await Workspace
                .ValidateAsync(_session.Lookups, Workspace.Language?.Code)
                .ConfigureAwait(true);
            ValidationIssues.Clear();
            foreach (var issue in report.Issues)
            {
                ValidationIssues.Add(new ValidationIssueItem(issue));
            }

            IsValidationVisible = true;
            StatusText = Strings.Format("EditorValidationSummary", ValidationIssues.Count);
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Discards unsaved changes after confirmation and reloads the table.
    /// </summary>
    /// <returns>A task that completes when the table is reloaded.</returns>
    [RelayCommand]
    public async Task ReloadAsync()
    {
        if (IsDirty && !_dialogService.Confirm(
                Strings.Format("EditorConfirmReload", Title),
                Strings.Get("EditorReloadTitle")))
        {
            return;
        }

        await ReloadForLanguageAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Shows the translation report for the current language overlay.
    /// </summary>
    [RelayCommand]
    public void ShowTranslationReport()
    {
        if (Workspace is null)
        {
            return;
        }

        var report = Workspace.AnalyzeTranslations();
        if (report is null)
        {
            StatusText = Strings.Get("EditorNoLanguageSelected");
            return;
        }

        _dialogService.ShowTranslationReport(
            report,
            Strings.Format(
                "TranslationReportTitleFormat",
                Title,
                Workspace.Language?.DisplayName ?? string.Empty));
    }

    private async Task ReloadForLanguageAsync()
    {
        Workspace = null;
        IsDirty = false;
        IsValidationVisible = false;
        _allRows.Clear();
        _allColumns.Clear();
        _allDetailFields.Clear();
        Rows.Clear();
        Columns.Clear();
        VisibleColumns.Clear();
        DetailGroups.Clear();
        ValidationIssues.Clear();
        await InitializeAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Restores the default visible column set.
    /// </summary>
    [RelayCommand]
    public void ShowDefaultColumns() => ApplyDefaultColumns();

    /// <summary>
    /// Shows every schema column.
    /// </summary>
    [RelayCommand]
    public void ShowAllColumns()
    {
        VisibleColumns.Clear();
        foreach (var column in _allColumns)
        {
            column.SetVisibleWithoutNotification(true);
            VisibleColumns.Add(column);
        }

        UpdateVisibleColumnCount();
    }

    /// <summary>
    /// Hides every schema column.
    /// </summary>
    [RelayCommand]
    public void HideAllColumns()
    {
        foreach (var column in _allColumns)
        {
            column.SetVisibleWithoutNotification(false);
        }

        VisibleColumns.Clear();
        UpdateVisibleColumnCount();
    }

    /// <summary>
    /// Sorts the rows by a column value.
    /// </summary>
    /// <param name="fieldName">XML field name to sort by.</param>
    /// <param name="direction">Sort direction.</param>
    public void SortBy(string fieldName, ListSortDirection direction)
    {
        SortField = fieldName;
        SortDirection = direction;
        ApplyRowView();
    }

    /// <inheritdoc />
    public OperationResult SetCellValue(TableRowViewModel row, string fieldName, string value)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(value);

        if (Workspace is null || !IsEditable)
        {
            StatusText = Strings.Get("EditorReadOnly");
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        if (row.Key is null)
        {
            StatusText = Strings.Format("StatusError", ErrorCodes.KeyNotFound);
            return OperationResult.Failure(ErrorCodes.KeyNotFound, "The record has no primary key.");
        }

        var wasAbsent = row.GetStoredValue(fieldName) is null;
        if (wasAbsent && value.Length == 0)
        {
            return OperationResult.Success();
        }

        var column = Workspace.Schema.FindColumn(fieldName);
        if (column is not null && !IsValidValue(column, value))
        {
            StatusText = Strings.Format("EditorInvalidValue", value, fieldName);
            return OperationResult.Failure(ErrorCodes.XmlInvalidNumber, StatusText);
        }

        var result = Workspace.SetValue(
            row.Key,
            fieldName,
            value,
            addIfMissing: wasAbsent);
        if (result.IsSuccess)
        {
            var refreshed = Workspace.GetRow(row.Key);
            if (refreshed is not null)
            {
                row.Refresh(refreshed);
            }

            foreach (var detailField in _allDetailFields)
            {
                detailField.Refresh();
            }

            if (fieldName is "ubGraphicNum" or "ubGraphicType")
            {
                _ = LoadDetailImagesAsync();
            }

            IsDirty = Workspace.IsDirty;
            StatusText = Workspace.Language is not null && column?.IsTranslatable != true
                ? Strings.Format("EditorBaseCellUpdated", fieldName, row.Key)
                : Strings.Format("EditorCellUpdated", fieldName, row.Key);
        }
        else
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
        }

        return result;
    }

    partial void OnIsEditableChanged(bool value) => OnPropertyChanged(nameof(IsReadOnly));

    partial void OnIsDirtyChanged(bool value) =>
        DisplayName = value ? Title + " *" : Title;

    partial void OnSearchTextChanged(string value) => ApplyRowView();

    partial void OnColumnSearchTextChanged(string value) => ApplyColumnFilter();

    partial void OnDetailFilterChanged(string value) => ApplyDetailFilter();

    partial void OnSelectedRowChanged(TableRowViewModel? value) => RefreshDetails();

    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(IsLanguageSelectorEnabled));

    partial void OnWorkspaceChanged(TableWorkspace? value) =>
        OnPropertyChanged(nameof(IsLanguageSelectorEnabled));

    partial void OnSelectedLanguageOptionChanged(LanguageOptionViewModel? value)
    {
        if (_suppressLanguageChange || value is null)
        {
            return;
        }

        if (string.Equals(value.Code, _languageCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _ = ChangeLanguageAsync(value);
    }

    private void BuildLanguageOptions(IReadOnlyList<LanguageDescriptor> configuredLanguages)
    {
        _suppressLanguageChange = true;
        try
        {
            LanguageOptions.Clear();
            LanguageOptions.Add(new LanguageOptionViewModel(null, false));
            var canCreateOverlay = Workspace?.Metadata is { TranslatableFieldPaths.Count: > 0 };
            var languages = configuredLanguages
                .Concat(Descriptor.Overlays.Select(static overlay => overlay.Language))
                .GroupBy(static language => language.Code, StringComparer.OrdinalIgnoreCase)
                .Select(static group => group.First())
                .OrderBy(static language => language.Ordinal)
                .ThenBy(static language => language.DisplayName, StringComparer.Ordinal);
            foreach (var language in languages)
            {
                var hasOverlay = Descriptor.Overlays.Any(overlay =>
                    string.Equals(
                        overlay.Language.Code,
                        language.Code,
                        StringComparison.OrdinalIgnoreCase));
                if (hasOverlay || canCreateOverlay)
                {
                    LanguageOptions.Add(new LanguageOptionViewModel(language, hasOverlay));
                }
            }

            SelectedLanguageOption = FindLanguageOption(_languageCode) ?? LanguageOptions[0];
        }
        finally
        {
            _suppressLanguageChange = false;
        }
    }

    private LanguageOptionViewModel? FindLanguageOption(string? code) =>
        string.IsNullOrWhiteSpace(code)
            ? LanguageOptions.FirstOrDefault(static option => option.Language is null)
            : LanguageOptions.FirstOrDefault(option =>
                string.Equals(option.Code, code, StringComparison.OrdinalIgnoreCase));

    private async Task ChangeLanguageAsync(LanguageOptionViewModel option)
    {
        if (_isChangingLanguage || Workspace is null)
        {
            return;
        }

        var previous = FindLanguageOption(_languageCode);
        if (IsDirty && !_dialogService.Confirm(
                Strings.Format("EditorConfirmLanguage", Title),
                Strings.Get("EditorLanguageTitle")))
        {
            RestoreLanguageSelection(previous);
            return;
        }

        var createdOverlay = false;
        if (!option.HasOverlay && option.Language is not null)
        {
            if (!_dialogService.Confirm(
                    Strings.Format(
                        "EditorConfirmCreateOverlay",
                        option.Language.DisplayName,
                        Descriptor.TableName.Value),
                    Strings.Get("EditorLanguageTitle")))
            {
                RestoreLanguageSelection(previous);
                return;
            }

            _isChangingLanguage = true;
            IsBusy = true;
            try
            {
                var ensured = await _session
                    .EnsureOverlayAsync(Descriptor, option.Language)
                    .ConfigureAwait(true);
                if (!ensured.IsSuccess)
                {
                    StatusText = Strings.Format(
                        "StatusError",
                        $"{ensured.ErrorCode}: {ensured.ErrorText}");
                    RestoreLanguageSelection(previous);
                    return;
                }

                Descriptor = ensured.Data!;
                createdOverlay = true;
            }
            catch (OperationCanceledException)
            {
                StatusText = Strings.Get("EditorLoadingCancelled");
                RestoreLanguageSelection(previous);
                return;
            }
            catch (Exception exception)
            {
                StatusText = Strings.Format("StatusError", exception.Message);
                RestoreLanguageSelection(previous);
                return;
            }
            finally
            {
                IsBusy = false;
                _isChangingLanguage = false;
            }
        }

        _isChangingLanguage = true;
        IsBusy = true;
        try
        {
            _languageCode = option.Code;
            await ReloadForLanguageAsync().ConfigureAwait(true);
            StatusText = createdOverlay
                ? Strings.Format("EditorOverlayCreated", option.Language!.DisplayName)
                : option.Code is null
                    ? Strings.Get("EditorLanguageBaseChanged")
                    : Strings.Format("EditorLanguageChanged", option.DisplayText);
            LanguageChanged?.Invoke(this, _languageCode);
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("EditorLoadingCancelled");
            _languageCode = previous?.Code;
            RestoreLanguageSelection(previous);
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
            _languageCode = previous?.Code;
            RestoreLanguageSelection(previous);
        }
        finally
        {
            IsBusy = false;
            _isChangingLanguage = false;
        }
    }

    private void RestoreLanguageSelection(LanguageOptionViewModel? option)
    {
        _suppressLanguageChange = true;
        try
        {
            SelectedLanguageOption = option
                ?? (LanguageOptions.Count > 0 ? LanguageOptions[0] : null);
        }
        finally
        {
            _suppressLanguageChange = false;
        }
    }

    private async Task LoadLookupsAsync(CancellationToken cancellationToken)
    {
        _lookupByField.Clear();
        if (Workspace is null)
        {
            return;
        }

        var columns = Workspace.Schema.Columns
            .Where(static column => !string.IsNullOrWhiteSpace(column.LookupTable))
            .ToArray();
        if (columns.Length == 0)
        {
            return;
        }

        var valuesByField = columns.ToDictionary(
            static column => column.XmlFieldName,
            static _ => new List<string?>(),
            StringComparer.Ordinal);
        foreach (var row in Workspace.GetRows())
        {
            foreach (var column in columns)
            {
                valuesByField[column.XmlFieldName].Add(row.GetValue(column.XmlFieldName));
            }
        }

        var cache = new Dictionary<string, OperationResult<IReadOnlyList<LookupEntry>>>(StringComparer.Ordinal);
        foreach (var column in columns)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var lookupTable = column.LookupTable!;
            var tableName = TableName.Parse(lookupTable);
            if (!cache.TryGetValue(lookupTable, out var result))
            {
                result = await _session.Lookups
                    .GetEntriesAsync(tableName, Workspace.Language?.Code, cancellationToken)
                    .ConfigureAwait(true);
                cache.Add(lookupTable, result);
            }

            _lookupByField[column.XmlFieldName] = result.IsSuccess
                ? LookupColumnData.Create(tableName, result.Data!, valuesByField[column.XmlFieldName])
                : LookupColumnData.Failed(
                    tableName,
                    result.ErrorCode ?? ErrorCodes.XmlInvalidLookupEntry,
                    result.ErrorText ?? $"Lookup table '{lookupTable}' could not be loaded.");
        }
    }

    private void BuildColumns()
    {
        _allColumns.Clear();
        VisibleColumns.Clear();
        if (Workspace is not null)
        {
            foreach (var column in Workspace.Schema.Columns)
            {
                _lookupByField.TryGetValue(column.XmlFieldName, out var lookup);
                _allColumns.Add(new TableColumnViewModel(column, OnColumnVisibilityChanged, lookup));
            }
        }

        ApplyDefaultColumns();
        ApplyColumnFilter();
    }

    private void ApplyDefaultColumns()
    {
        VisibleColumns.Clear();
        var budget = DefaultColumnBudget;
        foreach (var column in _allColumns)
        {
            var visible = column.IsKey;
            if (!visible && budget > 0 && !column.IsHidden && !column.IsList && !column.IsDict)
            {
                visible = true;
                budget--;
            }

            column.SetVisibleWithoutNotification(visible);
            if (visible)
            {
                VisibleColumns.Add(column);
            }
        }

        UpdateVisibleColumnCount();
    }

    private void OnColumnVisibilityChanged(TableColumnViewModel column, bool visible)
    {
        if (visible)
        {
            if (!VisibleColumns.Contains(column))
            {
                var index = 0;
                while (index < VisibleColumns.Count
                       && VisibleColumns[index].Column.Order < column.Column.Order)
                {
                    index++;
                }

                VisibleColumns.Insert(index, column);
            }
        }
        else
        {
            VisibleColumns.Remove(column);
        }

        UpdateVisibleColumnCount();
    }

    private void UpdateVisibleColumnCount() =>
        VisibleColumnCountText = Strings.Format(
            "EditorColumnCount",
            VisibleColumns.Count,
            _allColumns.Count);

    private void ApplyColumnFilter()
    {
        Columns.Clear();
        var term = ColumnSearchText.Trim();
        foreach (var column in _allColumns)
        {
            if (term.Length == 0
                || column.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                || column.TypeText.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                Columns.Add(column);
            }
        }
    }

    private void RebuildRows(string? selectKey)
    {
        _allRows.Clear();
        if (Workspace is not null)
        {
            var translatableFields = Workspace.Schema.Columns
                .Where(static column => column.IsTranslatable)
                .Select(static column => column.XmlFieldName)
                .ToArray();
            foreach (var row in Workspace.GetRows())
            {
                var viewModel = new TableRowViewModel(row, this);
                viewModel.ConfigureTranslation(IsTranslationMode, translatableFields);
                _allRows.Add(viewModel);
            }
        }

        ApplyRowView(selectKey);
    }

    private void ApplyRowView(string? selectKey = null)
    {
        var selectedKey = selectKey ?? SelectedRow?.Key;
        IEnumerable<TableRowViewModel> rows = _allRows;
        var term = SearchText.Trim();
        if (term.Length > 0)
        {
            var fields = VisibleColumns.Select(static column => column.Name).ToArray();
            rows = rows.Where(row => Matches(row, term, fields));
        }

        if (SortField.Length > 0)
        {
            var column = Workspace?.Schema.FindColumn(SortField);
            rows = rows.OrderBy(
                static row => row,
                new RowValueComparer(SortField, column?.DataType, SortDirection));
        }

        Rows.Clear();
        foreach (var row in rows)
        {
            Rows.Add(row);
        }

        SelectedRow = null;
        SelectedRow = selectedKey is null
            ? Rows.FirstOrDefault()
            : Rows.FirstOrDefault(row => string.Equals(row.Key, selectedKey, StringComparison.Ordinal));
        UpdateRecordCount();
        if (term.Length > 0 && Rows.Count == 0)
        {
            StatusText = Strings.Get("EditorNoMatches");
        }
    }

    internal void SetDetailValue(DetailFieldViewModel field, string value)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(value);

        SetCellValue(field.Row, field.Name, value);
    }

    internal void EditBits(DetailFieldViewModel field)
    {
        ArgumentNullException.ThrowIfNull(field);
        if (!field.CanEditBits)
        {
            return;
        }

        var currentText = field.Value;
        var current = uint.TryParse(currentText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0u;
        _lookupByField.TryGetValue(field.Name, out var lookup);
        var edited = _dialogService.EditBits(
            Strings.Format("BitEditorTitleFormat", field.Name),
            current,
            BuildBitLabels(lookup),
            lookup?.GetExactEntryName(currentText));
        if (edited is not { } value)
        {
            return;
        }

        field.Value = value.ToString(CultureInfo.InvariantCulture);
    }

    private static IReadOnlyDictionary<int, string> BuildBitLabels(LookupColumnData? lookup)
    {
        if (lookup is not { IsAvailable: true })
        {
            return new Dictionary<int, string>();
        }

        var labels = new Dictionary<int, string>();
        foreach (var entry in lookup.Entries)
        {
            if (entry.Id <= 0 || entry.Id > uint.MaxValue)
            {
                continue;
            }

            var mask = (uint)entry.Id;
            if ((mask & (mask - 1)) != 0)
            {
                continue;
            }

            var bit = BitOperations.Log2(mask);
            if (bit < 32)
            {
                labels.TryAdd(bit, entry.Name);
            }
        }

        return labels;
    }

    private void RefreshDetails()
    {
        _allDetailFields.Clear();
        DetailGroups.Clear();
        if (SelectedRow is null || Workspace is null)
        {
            DetailHeader = Strings.Get("StatusNoSelection");
            CancelDetailImageLoads();
            ClearDetailImages();
            return;
        }

        DetailHeader = Strings.Format("DetailHeaderFormat", SelectedRow.Key ?? string.Empty);
        foreach (var column in Workspace.Schema.Columns)
        {
            _lookupByField.TryGetValue(column.XmlFieldName, out var lookup);
            _allDetailFields.Add(new DetailFieldViewModel(this, SelectedRow, column, lookup));
        }

        ApplyDetailFilter();
        _ = LoadDetailImagesAsync();
    }

    private void ApplyDetailFilter()
    {
        DetailGroups.Clear();
        var term = DetailFilter.Trim();
        var grouped = new Dictionary<string, List<DetailFieldViewModel>>(StringComparer.Ordinal);
        var metadata = new Dictionary<string, (int GroupOrder, int FirstColumnOrder)>(StringComparer.Ordinal);
        foreach (var field in _allDetailFields)
        {
            if (term.Length > 0
                && !field.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                && !field.TypeText.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!grouped.TryGetValue(field.GroupKey, out var fields))
            {
                fields = new List<DetailFieldViewModel>();
                grouped.Add(field.GroupKey, fields);
                metadata.Add(field.GroupKey, (field.GroupOrder, field.Column.Order));
            }

            fields.Add(field);
        }

        foreach (var entry in grouped
                     .OrderBy(pair => metadata[pair.Key].GroupOrder)
                     .ThenBy(pair => metadata[pair.Key].FirstColumnOrder)
                     .ThenBy(pair => pair.Key, StringComparer.Ordinal))
        {
            var isExpanded = _groupExpansion.TryGetValue(entry.Key, out var remembered)
                ? remembered
                : IsExpandedByDefault(entry.Key);
            var group = new DetailFieldGroupViewModel(
                entry.Key,
                Strings.Get($"DetailGroup.{entry.Key}"),
                metadata[entry.Key].GroupOrder,
                isExpanded,
                entry.Value);
            group.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(DetailFieldGroupViewModel.IsExpanded))
                {
                    _groupExpansion[group.Key] = group.IsExpanded;
                }
            };
            DetailGroups.Add(group);
        }
    }

    private static bool IsExpandedByDefault(string groupKey) =>
        groupKey is "identity" or "classification" or "inventory" or "attachments" or "combat";

    private async Task LoadDetailImagesAsync()
    {
        var token = BeginImageLoad();
        var dataDirectory = _session.GameDataDirectory;
        if (!HasGraphicFields || dataDirectory is null)
        {
            ClearDetailImages();
            return;
        }

        if (!TryGetGraphic(out var graphicType, out var graphicIndex))
        {
            foreach (var panel in DetailImages)
            {
                panel.Image = null;
                panel.InfoText = Strings.Get("DetailNoImage");
                panel.IsLoading = false;
            }

            return;
        }

        await Task.WhenAll(DetailImages
                .Select(panel => LoadDetailImageAsync(panel, dataDirectory, graphicType, graphicIndex, token)))
            .ConfigureAwait(true);
    }

    private async Task LoadDetailImageAsync(
        DetailImagePanelViewModel panel,
        string dataDirectory,
        int graphicType,
        int graphicIndex,
        CancellationToken token)
    {
        panel.Image = null;
        panel.InfoText = Strings.Get("EditorLoading");
        panel.IsLoading = true;
        try
        {
            var result = await _itemImages.GetImageAsync(
                    dataDirectory,
                    panel.Size,
                    graphicType,
                    graphicIndex,
                    token)
                .ConfigureAwait(true);
            token.ThrowIfCancellationRequested();
            panel.Image = result.Data;
            panel.InfoText = result.Data is null
                ? Strings.Format("ImageNotAvailableFormat", result.ErrorCode ?? string.Empty)
                : $"{result.Data.PixelWidth}×{result.Data.PixelHeight}";
        }
        catch (OperationCanceledException)
        {
            // A newer selection replaced this image load.
        }
        catch (Exception exception)
        {
            if (!token.IsCancellationRequested)
            {
                panel.Image = null;
                panel.InfoText = Strings.Format("StatusError", exception.Message);
            }
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                panel.IsLoading = false;
            }
        }
    }

    private void ClearDetailImages()
    {
        foreach (var panel in DetailImages)
        {
            panel.Image = null;
            panel.InfoText = string.Empty;
            panel.IsLoading = false;
        }
    }

    private void CancelDetailImageLoads()
    {
        _imageCts?.Cancel();
        _imageCts?.Dispose();
        _imageCts = null;
    }

    private CancellationToken BeginImageLoad()
    {
        _imageCts?.Cancel();
        _imageCts?.Dispose();
        _imageCts = new CancellationTokenSource();
        return _imageCts.Token;
    }

    private bool TryGetGraphic(out int graphicType, out int graphicIndex)
    {
        graphicType = 0;
        graphicIndex = -1;
        if (SelectedRow is null)
        {
            return false;
        }

        var rawIndex = SelectedRow.GetStoredValue("ubGraphicNum");
        if (!int.TryParse(rawIndex, NumberStyles.Integer, CultureInfo.InvariantCulture, out graphicIndex)
            || graphicIndex < 0)
        {
            return false;
        }

        var rawType = SelectedRow.GetStoredValue("ubGraphicType");
        if (int.TryParse(rawType, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedType)
            && parsedType >= 0)
        {
            graphicType = parsedType;
        }

        return true;
    }

    private static bool IsValidValue(TableSchemaColumn column, string value)
    {
        if (value.Length == 0)
        {
            return true;
        }

        return column.DataType switch
        {
            ColumnDataType.Integer =>
                int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Lookup =>
                long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Flags =>
                uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Decimal =>
                decimal.TryParse(value, DecimalStyles, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Boolean => value.Trim() is "0" or "1",
            _ => true,
        };
    }

    private void UpdateRecordCount() =>
        RecordCountText = Strings.Format("EditorRowCount", Rows.Count, _allRows.Count);

    private static bool Matches(
        TableRowViewModel row,
        string term,
        IReadOnlyList<string> fields)
    {
        if (row.Key?.Contains(term, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        foreach (var field in fields)
        {
            if (row[field].Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private sealed class RowValueComparer : IComparer<TableRowViewModel>
    {
        private readonly string _field;
        private readonly ColumnDataType? _dataType;
        private readonly int _direction;

        public RowValueComparer(string field, ColumnDataType? dataType, ListSortDirection direction)
        {
            _field = field;
            _dataType = dataType;
            _direction = direction == ListSortDirection.Ascending ? 1 : -1;
        }

        public int Compare(TableRowViewModel? left, TableRowViewModel? right)
        {
            if (ReferenceEquals(left, right))
            {
                return 0;
            }

            var leftValue = left?.GetStoredValue(_field);
            var rightValue = right?.GetStoredValue(_field);
            if (leftValue is null && rightValue is null)
            {
                return 0;
            }

            if (leftValue is null)
            {
                return 1;
            }

            if (rightValue is null)
            {
                return -1;
            }

            var comparison = _dataType switch
            {
                ColumnDataType.Integer => CompareSigned(leftValue, rightValue),
                ColumnDataType.Lookup => CompareLookup(leftValue, rightValue),
                ColumnDataType.Flags => CompareFlags(leftValue, rightValue),
                ColumnDataType.Decimal => CompareDecimals(leftValue, rightValue),
                _ => string.Compare(leftValue, rightValue, StringComparison.OrdinalIgnoreCase),
            };

            return comparison * _direction;
        }

        private static int CompareSigned(string left, string right)
        {
            if (int.TryParse(left, NumberStyles.Integer, CultureInfo.InvariantCulture, out var leftValue)
                && int.TryParse(right, NumberStyles.Integer, CultureInfo.InvariantCulture, out var rightValue))
            {
                return leftValue.CompareTo(rightValue);
            }

            return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
        }

        private static int CompareLookup(string left, string right)
        {
            if (long.TryParse(left, NumberStyles.Integer, CultureInfo.InvariantCulture, out var leftValue)
                && long.TryParse(right, NumberStyles.Integer, CultureInfo.InvariantCulture, out var rightValue))
            {
                return leftValue.CompareTo(rightValue);
            }

            return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
        }

        private static int CompareFlags(string left, string right)
        {
            if (uint.TryParse(left, NumberStyles.Integer, CultureInfo.InvariantCulture, out var leftValue)
                && uint.TryParse(right, NumberStyles.Integer, CultureInfo.InvariantCulture, out var rightValue))
            {
                return leftValue.CompareTo(rightValue);
            }

            return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
        }

        private static int CompareDecimals(string left, string right)
        {
            if (decimal.TryParse(left, DecimalStyles, CultureInfo.InvariantCulture, out var leftValue)
                && decimal.TryParse(right, DecimalStyles, CultureInfo.InvariantCulture, out var rightValue))
            {
                return leftValue.CompareTo(rightValue);
            }

            return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
        }
    }
}
