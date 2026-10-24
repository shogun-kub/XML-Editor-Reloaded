using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Filtering;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Core.Layout;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Filtering;
using XmlEditorReloaded.Xml.Schema;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Editable table view: a virtualized grid over <see cref="TableWorkspace"/> with schema-driven
/// columns, sparse cell editing, row operations, search, sorting, validation, and saving.
/// </summary>
public partial class TableEditorViewModel : ObservableObject, ITableCellEditor, IEditorTab, IDetailFieldHost
{
    private readonly IDataSetSession _session;
    private readonly IItemImageService _itemImages;
    private readonly ITableLayoutStore? _layoutStore;
    private string? _languageCode;
    private readonly IDialogService _dialogService;
    private readonly List<TableRowViewModel> _allRows = new();
    private readonly List<TableColumnViewModel> _allColumns = new();
    private readonly List<TableRowViewModel> _selectedRows = new();
    private readonly List<DetailFieldViewModel> _allDetailFields = new();
    private readonly Dictionary<string, LookupColumnData> _lookupByField = new(StringComparer.Ordinal);
    private readonly Dictionary<string, RelationColumnData> _relationByField = new(StringComparer.Ordinal);
    private readonly Dictionary<string, bool> _groupExpansion = new(StringComparer.Ordinal);
    private IReadOnlyDictionary<string, IReadOnlySet<string>>? _relationTargetValues;
    private CancellationTokenSource? _imageCts;
    private CancellationTokenSource? _layoutSaveCts;
    private RowFilterGroup _filterGroup = RowFilterGroup.Empty;
    private bool _suppressLanguageChange;
    private bool _isChangingLanguage;
    private bool _isEmbedded;
    private bool _suppressSelectionSync;
    private bool _suppressLayoutPersistence;
    private bool _layoutChangesReady;
    private bool _layoutDirty;
    private const int DefaultColumnBudget = 8;
    private const int LayoutSaveDelayMilliseconds = 500;
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
    private bool _canUndo;

    [ObservableProperty]
    private bool _canRedo;

    [ObservableProperty]
    private bool _isValidationVisible;

    [ObservableProperty]
    private TableRowViewModel? _selectedRow;

    [ObservableProperty]
    private TableFilterViewModel? _filter;

    [ObservableProperty]
    private string? _currentCellField;

    [ObservableProperty]
    private bool _isFilterVisible;

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
    /// <param name="layoutStore">
    /// Optional per-table layout store; when omitted, the editor keeps its layout in memory only.
    /// </param>
    public TableEditorViewModel(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode,
        IDialogService dialogService,
        IItemImageService itemImages,
        ITableLayoutStore? layoutStore = null)
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
        _layoutStore = layoutStore;

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
        GoToTargetText = Strings.Get("EditorGoToTargetText");
        DetailUntranslatedText = Strings.Get("EditorUntranslated");
        LanguageLabel = Strings.Get("EditorLanguageLabel");
        TranslationReportText = Strings.Get("EditorTranslationReportText");
        TranslatableColumnSuffix = Strings.Get("EditorTranslatableSuffix");
        UndoText = Strings.Get("EditorUndoText");
        RedoText = Strings.Get("EditorRedoText");
        UndoButtonTooltip = Strings.Get("EditorUndoButtonTooltip");
        RedoButtonTooltip = Strings.Get("EditorRedoButtonTooltip");
        BulkEditText = Strings.Get("EditorBulkEditText");
        BulkEditTooltip = Strings.Get("EditorBulkEditTooltip");
        FilterLabel = Strings.Get("EditorFilterLabel");
        FilterAddText = Strings.Get("EditorFilterAdd");
        FilterClearText = Strings.Get("EditorFilterClear");
        FilterByValueText = Strings.Get("EditorFilterByValue");
        FilterExcludeValueText = Strings.Get("EditorFilterExcludeValue");
        ResetLayoutText = Strings.Get("EditorResetLayoutText");
        ResetLayoutTooltip = Strings.Get("EditorResetLayoutTooltip");
    }

    /// <summary>
    /// Occurs after the editor successfully switches to another language or the base table.
    /// </summary>
    public event EventHandler<string?>? LanguageChanged;

    /// <summary>
    /// Occurs when the user asks to open a record referenced by a relation field.
    /// </summary>
    public event EventHandler<TableNavigationRequest>? TableNavigationRequested;

    /// <summary>
    /// Gets the table descriptor. It is replaced when a new language overlay is created.
    /// </summary>
    public TableDescriptor Descriptor { get; private set; }

    /// <inheritdoc />
    public TableName TableName => Descriptor.TableName;

    /// <inheritdoc />
    public string TableDataDirectory => _session.TableDataDirectory;

    /// <inheritdoc />
    public string DatasetLabel => DatasetNames.LabelFor(_session);

    /// <inheritdoc />
    public string? SelectedRecordKey => SelectedRow?.Key;

    /// <summary>
    /// Gets the table title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the editor is embedded in another workspace instead of
    /// owning its own tab. Embedded editors hide the grid chrome and delegate saving to their host.
    /// </summary>
    public bool IsEmbedded
    {
        get => _isEmbedded;
        set
        {
            if (SetProperty(ref _isEmbedded, value))
            {
                OnPropertyChanged(nameof(ShowThumbnailColumn));
                OnPropertyChanged(nameof(IsLanguageSelectorEnabled));
            }
        }
    }

    /// <summary>
    /// Gets or sets an additional row filter applied before search and sorting. The item workspace uses
    /// it to show only the child rows of the selected item.
    /// </summary>
    public Func<TableRowViewModel, bool>? ExternalRowFilter { get; set; }

    /// <summary>
    /// Gets or sets values written to a new row created through <see cref="AddRow"/>, for example the
    /// parent key of a child row.
    /// </summary>
    public IReadOnlyDictionary<string, string>? NewRowDefaults { get; set; }

    /// <summary>
    /// Gets or sets the save action used while the editor is embedded. When set, Ctrl+S and the save
    /// command save the whole host set instead of this table alone.
    /// </summary>
    public Func<Task>? EmbeddedSave { get; set; }

    /// <summary>
    /// Gets a value indicating whether the grid must be read-only.
    /// </summary>
    public bool IsReadOnly => !IsEditable;

    bool IDetailFieldHost.IsEditable => IsEditable;

    bool IDetailFieldHost.IsTranslationMode => IsTranslationMode;

    /// <summary>
    /// Gets a value indicating whether the grid shows an item thumbnail column. The column is available
    /// for tables that define <c>ubGraphicNum</c>, in the standalone editor only.
    /// </summary>
    public bool ShowThumbnailColumn =>
        HasGraphicFields && !IsEmbedded && _session.GameDataDirectory is not null;

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
    /// Gets the tooltip of the button that opens the record referenced by a relation field.
    /// </summary>
    public string GoToTargetText { get; }

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
    /// Gets the undo button text.
    /// </summary>
    public string UndoText { get; }

    /// <summary>
    /// Gets the redo button text.
    /// </summary>
    public string RedoText { get; }

    /// <summary>
    /// Gets the undo button tooltip.
    /// </summary>
    public string UndoButtonTooltip { get; }

    /// <summary>
    /// Gets the redo button tooltip.
    /// </summary>
    public string RedoButtonTooltip { get; }

    /// <summary>
    /// Gets the bulk edit button text.
    /// </summary>
    public string BulkEditText { get; }

    /// <summary>
    /// Gets the bulk edit button tooltip.
    /// </summary>
    public string BulkEditTooltip { get; }

    /// <summary>
    /// Gets the filter panel label.
    /// </summary>
    public string FilterLabel { get; }

    /// <summary>
    /// Gets the add-condition button text.
    /// </summary>
    public string FilterAddText { get; }

    /// <summary>
    /// Gets the clear-filter button text.
    /// </summary>
    public string FilterClearText { get; }

    /// <summary>
    /// Gets the context menu text that filters by the current cell value.
    /// </summary>
    public string FilterByValueText { get; }

    /// <summary>
    /// Gets the context menu text that excludes the current cell value.
    /// </summary>
    public string FilterExcludeValueText { get; }

    /// <summary>
    /// Gets the reset-layout command text.
    /// </summary>
    public string ResetLayoutText { get; }

    /// <summary>
    /// Gets the reset-layout command tooltip.
    /// </summary>
    public string ResetLayoutTooltip { get; }

    /// <summary>
    /// Gets the rows explicitly selected by the user in the grid. The grid auto-selects the first row
    /// for the detail panel, but that selection is not reported here.
    /// </summary>
    public IReadOnlyList<TableRowViewModel> SelectedRows => _selectedRows;

    /// <summary>
    /// Gets the number of explicitly selected rows.
    /// </summary>
    public int SelectedRowCount => _selectedRows.Count;

    /// <summary>
    /// Gets the localized selected-row count, or an empty string when no rows are selected.
    /// </summary>
    public string SelectedRowCountText => _selectedRows.Count == 0
        ? string.Empty
        : Strings.Format("EditorSelectedRowsFormat", _selectedRows.Count);

    /// <summary>
    /// Gets a value indicating whether a structured filter is applied to the grid.
    /// </summary>
    public bool HasActiveFilter => !_filterGroup.IsEmpty;

    /// <summary>
    /// Gets a value indicating whether the language selector can be used.
    /// </summary>
    public bool IsLanguageSelectorEnabled => !IsBusy && !IsEmbedded && Workspace is not null;

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
    public Task InitializeAsync(CancellationToken cancellationToken = default) =>
        InitializeCoreAsync(null, cancellationToken);

    /// <summary>
    /// Builds the grid over an already loaded workspace that belongs to a shared table set. Lookups,
    /// relations, and rows are loaded by this method; the workspace itself is not reloaded and keeps the
    /// shared undo history.
    /// </summary>
    /// <param name="workspace">Workspace to display.</param>
    /// <param name="configuredLanguages">Languages offered by the language selector.</param>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>A task that completes when the table is ready.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="workspace"/> is null.</exception>
    public Task InitializeFromWorkspaceAsync(
        TableWorkspace workspace,
        IReadOnlyList<LanguageDescriptor> configuredLanguages,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(configuredLanguages);
        return InitializeCoreAsync((workspace, configuredLanguages), cancellationToken);
    }

    private async Task InitializeCoreAsync(
        (TableWorkspace Workspace, IReadOnlyList<LanguageDescriptor> Languages)? supplied,
        CancellationToken cancellationToken)
    {
        IsBusy = true;
        StatusText = Strings.Get("EditorLoading");
        HasGraphicFields = false;
        IsTranslationMode = false;
        try
        {
            IReadOnlyList<LanguageDescriptor> configuredLanguages = Array.Empty<LanguageDescriptor>();
            if (supplied is null)
            {
                var languagesResult = await _session.GetLanguagesAsync(cancellationToken).ConfigureAwait(true);
                if (languagesResult.IsSuccess)
                {
                    configuredLanguages = languagesResult.Data!;
                }
            }
            else
            {
                configuredLanguages = supplied.Value.Languages;
            }

            TableWorkspace? loaded = supplied?.Workspace;
            if (loaded is null)
            {
                var result = await _session
                    .OpenTableAsync(Descriptor, _languageCode, cancellationToken)
                    .ConfigureAwait(true);
                if (!result.IsSuccess)
                {
                    StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
                    IsEditable = false;
                    return;
                }

                loaded = result.Data!;
            }

            Workspace = loaded;
            IsTranslationMode = Workspace.Language is not null;
            LanguageText = Workspace.Language is null
                ? Strings.Get("EditorBaseLanguage")
                : Strings.Format("EditorLanguageFormat", Workspace.Language.DisplayName);
            BuildLanguageOptions(configuredLanguages);
            IsEditable = Workspace.CanEdit && Workspace.BaseDocument.Records.Count > 0;
            await LoadFieldSourcesAsync(cancellationToken).ConfigureAwait(true);
            BuildColumns();
            HasGraphicFields = Workspace.Schema.FindColumn("ubGraphicNum") is not null;
            _layoutChangesReady = false;
            var layoutError = await LoadLayoutAsync(cancellationToken).ConfigureAwait(true);
            RebuildRows(null);
            _layoutChangesReady = true;
            StatusText = layoutError ?? (IsEditable
                ? Strings.Get("EditorReady")
                : Workspace.BaseDocument.Records.Count == 0
                    ? Strings.Get("EditorReadOnlyNested")
                    : Strings.Get("EditorReadOnly"));
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
            RefreshHistoryState();
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
        if (IsEmbedded && EmbeddedSave is not null)
        {
            await EmbeddedSave().ConfigureAwait(true);
            return;
        }

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
                // Picker display text in other editors is cached per target table; the saved values
                // must be visible the next time a target is resolved.
                _session.RelationResolver.Invalidate(Descriptor.TableName);
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
    /// Appends a new record with the next primary key and the configured
    /// <see cref="NewRowDefaults"/> values.
    /// </summary>
    [RelayCommand]
    public void AddRow() => AddRowWithDefaults(NewRowDefaults);

    /// <summary>
    /// Appends a new record with the next primary key and the supplied initial values.
    /// </summary>
    /// <param name="defaults">
    /// Direct child values written to the new record, for example the parent key of a child row.
    /// </param>
    public void AddRowWithDefaults(IReadOnlyDictionary<string, string>? defaults)
    {
        if (Workspace is null)
        {
            return;
        }

        var result = Workspace.AddRecord(defaults);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = Workspace.IsDirty;
        var key = result.Data!.GetKey();
        RebuildRows(key);
        RefreshHistoryState();
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
        RefreshHistoryState();
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
        RefreshHistoryState();
        StatusText = Strings.Format("EditorRowDeleted", key);
    }

    /// <summary>
    /// Reverses the most recent cell, row, or bulk edit.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUndo))]
    public void Undo()
    {
        ApplyHistory(undo: true);
    }

    /// <summary>
    /// Restores the most recently undone edit.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRedo))]
    public void Redo()
    {
        ApplyHistory(undo: false);
    }

    /// <summary>
    /// Opens the bulk edit dialog for the rows in the current view and applies the confirmed request
    /// as one undoable transaction.
    /// </summary>
    [RelayCommand]
    public void BulkEdit()
    {
        if (Workspace is null || !IsEditable)
        {
            StatusText = Strings.Get("EditorReadOnly");
            return;
        }

        var targetRows = _selectedRows.Count > 0
            ? _selectedRows.Where(row => Rows.Contains(row)).ToArray()
            : Rows.ToArray();
        var keys = targetRows
            .Where(static row => row.Key is not null)
            .Select(static row => row.Key!)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        if (keys.Length == 0)
        {
            StatusText = Strings.Get("EditorBulkNoRows");
            return;
        }

        var columns = _allColumns
            .Where(static column => !column.IsReadOnly)
            .Select(static column => new BulkEditColumnOption(column.Name, column.TypeText, column.DataType))
            .ToArray();
        if (columns.Length == 0)
        {
            StatusText = Strings.Get("EditorBulkNoColumns");
            return;
        }

        var title = _selectedRows.Count > 0
            ? Strings.Format("BulkEditTitleSelectedFormat", Title, keys.Length)
            : Strings.Format("BulkEditTitleFormat", Title);
        var context = new BulkEditContext(
            title,
            columns,
            keys,
            request => Workspace.PreviewBulk(request),
            isSelectionScope: _selectedRows.Count > 0);
        var request = _dialogService.EditBulk(context);
        if (request is null)
        {
            return;
        }

        var result = Workspace.ApplyBulk(request);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = Workspace.IsDirty;
        RebuildRows(SelectedRow?.Key);
        RefreshHistoryState();
        StatusText = Strings.Format(
            "EditorBulkAppliedFormat",
            result.Data!.ChangedCount,
            result.Data.FieldName,
            result.Data.Operand);
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
                .ValidateAsync(
                    _session.Lookups,
                    Workspace.Language?.Code,
                    cancellationToken: default,
                    relationTargetValues: _relationTargetValues)
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
    /// Clears the structured filter and shows every row that passes search again.
    /// </summary>
    [RelayCommand]
    public void ClearFilter()
    {
        if (_filterGroup.IsEmpty && Filter?.Criteria.Count is not > 0)
        {
            return;
        }

        _filterGroup = RowFilterGroup.Empty;
        Filter?.Load(_filterGroup);
        OnPropertyChanged(nameof(HasActiveFilter));
        ApplyRowView(SelectedRow?.Key);
        ScheduleLayoutSave();
    }

    /// <summary>
    /// Adds an equality filter for the value of the current cell.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanFilterByValue))]
    public void FilterByValue() => ApplyValueFilter(RowFilterOperator.Equals);

    /// <summary>
    /// Adds an inequality filter that hides rows with the current cell value.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanFilterByValue))]
    public void ExcludeValue() => ApplyValueFilter(RowFilterOperator.NotEquals);

    /// <summary>
    /// Removes the stored layout of the table and restores default columns, sorting, and filter.
    /// </summary>
    /// <returns>A task that completes when the layout is reset.</returns>
    [RelayCommand]
    public async Task ResetLayoutAsync()
    {
        _layoutSaveCts?.Cancel();
        _layoutSaveCts?.Dispose();
        _layoutSaveCts = null;
        _layoutDirty = false;

        if (_layoutStore is not null && !IsEmbedded)
        {
            var removed = await _layoutStore
                .RemoveAsync(TableDataDirectory, TableName.Value)
                .ConfigureAwait(true);
            if (!removed.IsSuccess)
            {
                StatusText = Strings.Format(
                    "StatusError",
                    $"{removed.ErrorCode}: {removed.ErrorText}");
                return;
            }
        }

        _suppressLayoutPersistence = true;
        try
        {
            SortField = string.Empty;
            SortDirection = ListSortDirection.Ascending;
            _filterGroup = RowFilterGroup.Empty;
            Filter?.Load(_filterGroup);
            foreach (var column in _allColumns)
            {
                column.SetDisplayWidth(0);
                column.DisplayOrder = column.Column.Order;
            }

            ApplyDefaultColumns();
        }
        finally
        {
            _suppressLayoutPersistence = false;
        }

        OnPropertyChanged(nameof(HasActiveFilter));
        ApplyRowView(SelectedRow?.Key);
        StatusText = Strings.Get("EditorLayoutResetDone");
    }

    /// <summary>
    /// Replaces the explicitly selected rows after the grid selection changed.
    /// </summary>
    /// <param name="rows">Rows selected by the user.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="rows"/> is null.</exception>
    public void UpdateSelectedRows(IEnumerable<TableRowViewModel> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        if (_suppressSelectionSync)
        {
            return;
        }

        var candidates = new HashSet<TableRowViewModel>(rows);
        var selected = Rows.Where(candidates.Contains).ToArray();
        if (selected.SequenceEqual(_selectedRows))
        {
            return;
        }

        _selectedRows.Clear();
        _selectedRows.AddRange(selected);
        OnPropertyChanged(nameof(SelectedRowCount));
        OnPropertyChanged(nameof(SelectedRowCountText));
    }

    /// <summary>
    /// Applies the column order reported by the grid after the user dragged a column.
    /// </summary>
    /// <param name="fieldsInDisplayOrder">Visible field names in their new display order.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fieldsInDisplayOrder"/> is null.</exception>
    public void ApplyColumnOrder(IReadOnlyList<string> fieldsInDisplayOrder)
    {
        ArgumentNullException.ThrowIfNull(fieldsInDisplayOrder);

        var byName = _allColumns.ToDictionary(static column => column.Name, StringComparer.Ordinal);
        var changed = false;
        for (var index = 0; index < fieldsInDisplayOrder.Count; index++)
        {
            if (byName.TryGetValue(fieldsInDisplayOrder[index], out var column)
                && column.DisplayOrder != index)
            {
                column.DisplayOrder = index;
                changed = true;
            }
        }

        if (changed)
        {
            ScheduleLayoutSave();
        }
    }

    /// <summary>
    /// Applies column widths reported by the grid after the user resized a column.
    /// </summary>
    /// <param name="widths">Field names with their new widths in device-independent pixels.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="widths"/> is null.</exception>
    public void ApplyColumnWidths(IReadOnlyList<(string Field, double Width)> widths)
    {
        ArgumentNullException.ThrowIfNull(widths);

        var byName = _allColumns.ToDictionary(static column => column.Name, StringComparer.Ordinal);
        var changed = false;
        foreach (var (field, width) in widths)
        {
            if (!byName.TryGetValue(field, out var column))
            {
                continue;
            }

            var previous = column.PersistedWidth;
            column.SetDisplayWidth(width);
            changed |= Math.Abs(previous - column.PersistedWidth) > 0.5;
        }

        if (changed)
        {
            ScheduleLayoutSave();
        }
    }

    /// <summary>
    /// Writes the pending layout immediately. Called before a tab is closed or the application exits.
    /// </summary>
    /// <returns>A task that completes when the layout is stored.</returns>
    public async Task FlushLayoutAsync()
    {
        if (!_layoutDirty
            || !_layoutChangesReady
            || _layoutStore is null
            || IsEmbedded)
        {
            return;
        }

        _layoutSaveCts?.Cancel();
        _layoutSaveCts?.Dispose();
        _layoutSaveCts = null;
        try
        {
            await SaveLayoutAsync(CancellationToken.None).ConfigureAwait(true);
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
        }
    }

    private bool CanFilterByValue() =>
        SelectedRow is not null && !string.IsNullOrWhiteSpace(CurrentCellField);

    private void ApplyValueFilter(RowFilterOperator @operator)
    {
        var field = CurrentCellField;
        if (SelectedRow is null || string.IsNullOrWhiteSpace(field))
        {
            StatusText = Strings.Get("EditorFilterNoCell");
            return;
        }

        var value = SelectedRow.GetStoredValue(field);
        if (value is null)
        {
            StatusText = Strings.Get("EditorFilterNoValue");
            return;
        }

        var criteria = _filterGroup.Criteria.ToList();
        if (!criteria.Any(criterion =>
                string.Equals(criterion.Field, field, StringComparison.Ordinal)
                && criterion.Operator == @operator
                && string.Equals(criterion.Operand, value, StringComparison.Ordinal)))
        {
            criteria.Add(new RowFilterCriterion(field, @operator, value));
        }

        _filterGroup = new RowFilterGroup(_filterGroup.Operator, criteria);
        Filter?.Load(_filterGroup);
        IsFilterVisible = true;
        OnPropertyChanged(nameof(HasActiveFilter));
        ApplyRowView(SelectedRow.Key);
        ScheduleLayoutSave();
    }

    private void OnFilterChanged()
    {
        if (Filter is null)
        {
            return;
        }

        _filterGroup = Filter.ToGroup();
        OnPropertyChanged(nameof(HasActiveFilter));
        ApplyRowView(SelectedRow?.Key);
        ScheduleLayoutSave();
    }

    private async Task<string?> LoadLayoutAsync(CancellationToken cancellationToken)
    {
        if (_layoutStore is null || IsEmbedded)
        {
            return null;
        }

        _suppressLayoutPersistence = true;
        try
        {
            var result = await _layoutStore
                .LoadAsync(TableDataDirectory, TableName.Value, cancellationToken)
                .ConfigureAwait(true);
            if (!result.IsSuccess)
            {
                return Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            }

            ApplyLayout(result.Data!);
            return null;
        }
        finally
        {
            _suppressLayoutPersistence = false;
        }
    }

    private void ApplyLayout(TableLayout layout)
    {
        SortField = layout.SortField ?? string.Empty;
        SortDirection = layout.SortDirection == TableSortDirection.Descending
            ? ListSortDirection.Descending
            : ListSortDirection.Ascending;
        _filterGroup = layout.Filter;
        Filter?.Load(_filterGroup);
        OnPropertyChanged(nameof(HasActiveFilter));

        if (layout.Columns.Count > 0)
        {
            var byName = _allColumns.ToDictionary(static column => column.Name, StringComparer.Ordinal);
            foreach (var entry in layout.Columns)
            {
                if (!byName.TryGetValue(entry.Name, out var column))
                {
                    continue;
                }

                column.SetDisplayWidth(entry.Width);
                column.DisplayOrder = entry.DisplayOrder >= 0
                    ? entry.DisplayOrder
                    : column.Column.Order;
                column.SetVisibleWithoutNotification(entry.IsVisible);
            }
        }

        RefreshVisibleColumns();
    }

    private TableLayout CaptureLayout() => new(
        _allColumns
            .Select(static column => new ColumnLayout(
                column.Name,
                column.IsVisible,
                column.PersistedWidth,
                column.DisplayOrder))
            .ToArray(),
        SortField.Length > 0 ? SortField : null,
        SortDirection == ListSortDirection.Descending
            ? TableSortDirection.Descending
            : TableSortDirection.Ascending,
        _filterGroup);

    private void ScheduleLayoutSave()
    {
        if (!_layoutChangesReady
            || _suppressLayoutPersistence
            || IsEmbedded
            || _layoutStore is null)
        {
            return;
        }

        _layoutDirty = true;
        _layoutSaveCts?.Cancel();
        _layoutSaveCts?.Dispose();
        _layoutSaveCts = new CancellationTokenSource();
        _ = SaveLayoutAfterDelayAsync(_layoutSaveCts.Token);
    }

    private async Task SaveLayoutAfterDelayAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(LayoutSaveDelayMilliseconds, cancellationToken).ConfigureAwait(true);
            cancellationToken.ThrowIfCancellationRequested();
            await SaveLayoutAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // A newer layout change replaced this write.
        }
    }

    private async Task SaveLayoutAsync(CancellationToken cancellationToken)
    {
        if (_layoutStore is null)
        {
            return;
        }

        var result = await _layoutStore
            .SaveAsync(TableDataDirectory, TableName.Value, CaptureLayout(), cancellationToken)
            .ConfigureAwait(true);
        if (result.IsSuccess)
        {
            _layoutDirty = false;
        }
        else
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
        }
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
        await FlushLayoutAsync().ConfigureAwait(true);
        _layoutChangesReady = false;
        Workspace = null;
        IsDirty = false;
        IsValidationVisible = false;
        _allRows.Clear();
        _allColumns.Clear();
        _selectedRows.Clear();
        _allDetailFields.Clear();
        Rows.Clear();
        Columns.Clear();
        VisibleColumns.Clear();
        DetailGroups.Clear();
        ValidationIssues.Clear();
        OnPropertyChanged(nameof(SelectedRowCount));
        OnPropertyChanged(nameof(SelectedRowCountText));
        await InitializeAsync().ConfigureAwait(true);
    }

    private void ApplyHistory(bool undo)
    {
        if (Workspace is null)
        {
            return;
        }

        var selectedKey = SelectedRow?.Key;
        try
        {
            var result = undo ? Workspace.Undo() : Workspace.Redo();
            if (!result.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
                RefreshHistoryState();
                return;
            }

            IsDirty = Workspace.IsDirty;
            RebuildRows(selectedKey);
            RefreshHistoryState();
            StatusText = Strings.Get(undo ? "EditorUndone" : "EditorRedone");
        }
        catch (InvalidOperationException exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
            RefreshHistoryState();
        }
    }

    private void RefreshHistoryState()
    {
        CanUndo = Workspace?.CanUndo == true;
        CanRedo = Workspace?.CanRedo == true;
    }

    /// <summary>
    /// Restores the default visible column set.
    /// </summary>
    [RelayCommand]
    public void ShowDefaultColumns()
    {
        ApplyDefaultColumns();
        ScheduleLayoutSave();
    }

    /// <summary>
    /// Shows every schema column.
    /// </summary>
    [RelayCommand]
    public void ShowAllColumns()
    {
        foreach (var column in _allColumns)
        {
            column.SetVisibleWithoutNotification(true);
        }

        RefreshVisibleColumns();
        ScheduleLayoutSave();
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

        RefreshVisibleColumns();
        ScheduleLayoutSave();
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
        ScheduleLayoutSave();
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

        var column = Workspace.Schema.FindColumn(fieldName)
            ?? (fieldName.IndexOf('/') >= 0
                ? Workspace.Schema.FindColumn(FieldPath.Parse(fieldName).FieldName)
                : null);
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
            RefreshHistoryState();
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

    partial void OnCanUndoChanged(bool value) => UndoCommand.NotifyCanExecuteChanged();

    partial void OnCanRedoChanged(bool value) => RedoCommand.NotifyCanExecuteChanged();

    partial void OnSearchTextChanged(string value) => ApplyRowView();

    partial void OnColumnSearchTextChanged(string value) => ApplyColumnFilter();

    partial void OnDetailFilterChanged(string value) => ApplyDetailFilter();

    partial void OnSelectedRowChanged(TableRowViewModel? value)
    {
        RefreshDetails();
        FilterByValueCommand.NotifyCanExecuteChanged();
        ExcludeValueCommand.NotifyCanExecuteChanged();
    }

    partial void OnCurrentCellFieldChanged(string? value)
    {
        FilterByValueCommand.NotifyCanExecuteChanged();
        ExcludeValueCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(IsLanguageSelectorEnabled));

    partial void OnHasGraphicFieldsChanged(bool value) => OnPropertyChanged(nameof(ShowThumbnailColumn));

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
        if (_isChangingLanguage || IsEmbedded || Workspace is null)
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

    private async Task LoadFieldSourcesAsync(CancellationToken cancellationToken)
    {
        _lookupByField.Clear();
        _relationByField.Clear();
        _relationTargetValues = null;
        if (Workspace is null)
        {
            return;
        }

        var sources = await FieldSourceSet
            .LoadAsync(_session, Workspace, cancellationToken)
            .ConfigureAwait(true);
        foreach (var pair in sources.Lookups)
        {
            _lookupByField[pair.Key] = pair.Value;
        }

        foreach (var pair in sources.Relations)
        {
            _relationByField[pair.Key] = pair.Value;
        }

        _relationTargetValues = sources.RelationTargetValues;
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
                _relationByField.TryGetValue(column.XmlFieldName, out var relation);
                _allColumns.Add(new TableColumnViewModel(column, OnColumnVisibilityChanged, lookup, relation));
            }
        }

        Filter = new TableFilterViewModel(GetFilterColumns(), OnFilterChanged);
        Filter.Load(_filterGroup);
        ApplyDefaultColumns();
        ApplyColumnFilter();
    }

    private TableColumnViewModel[] GetFilterColumns() => _allColumns
        .Where(static column => !column.IsList && !column.IsDict && !column.Column.IsNested)
        .ToArray();

    private void ApplyDefaultColumns()
    {
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
        }

        RefreshVisibleColumns();
    }

    private void RefreshVisibleColumns()
    {
        VisibleColumns.Clear();
        foreach (var column in _allColumns
                     .Where(static column => column.IsVisible)
                     .OrderBy(static column => column.DisplayOrder)
                     .ThenBy(static column => column.Column.Order))
        {
            VisibleColumns.Add(column);
        }

        UpdateVisibleColumnCount();
    }

    private void OnColumnVisibilityChanged(TableColumnViewModel column, bool visible)
    {
        RefreshVisibleColumns();
        ScheduleLayoutSave();
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
                viewModel.ConfigureThumbnail(HasGraphicFields ? LoadThumbnailAsync : null);
                _allRows.Add(viewModel);
            }
        }

        ApplyRowView(selectKey);
    }

    private void ApplyRowView(string? selectKey = null)
    {
        var selectedKey = selectKey ?? SelectedRow?.Key;
        IEnumerable<TableRowViewModel> rows = _allRows;
        if (ExternalRowFilter is not null)
        {
            rows = rows.Where(ExternalRowFilter);
        }

        var term = SearchText.Trim();
        if (term.Length > 0)
        {
            var fields = VisibleColumns.Select(static column => column.Name).ToArray();
            rows = rows.Where(row => Matches(row, term, fields));
        }

        if (!_filterGroup.IsEmpty && Workspace is not null)
        {
            rows = rows.Where(row => TableRowFilterMatcher.Matches(row.Row, Workspace.Schema, _filterGroup));
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

        _suppressSelectionSync = true;
        try
        {
            _selectedRows.Clear();
            OnPropertyChanged(nameof(SelectedRowCount));
            OnPropertyChanged(nameof(SelectedRowCountText));
            SelectedRow = null;
            SelectedRow = selectedKey is null
                ? Rows.FirstOrDefault()
                : Rows.FirstOrDefault(row => string.Equals(row.Key, selectedKey, StringComparison.Ordinal));
        }
        finally
        {
            _suppressSelectionSync = false;
        }

        UpdateRecordCount();
        if (term.Length > 0 && Rows.Count == 0)
        {
            StatusText = Strings.Get("EditorNoMatches");
        }
    }

    /// <summary>
    /// Applies an edited field value from a detail form.
    /// </summary>
    /// <param name="field">Field that was edited.</param>
    /// <param name="value">New raw value.</param>
    public void SetDetailValue(DetailFieldViewModel field, string value)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(value);

        SetCellValue(field.Row, field.PathText, value);
    }

    /// <summary>
    /// Opens the record referenced by a relation field of the detail form.
    /// </summary>
    /// <param name="field">Relation field whose target should be opened.</param>
    public void NavigateToRelation(DetailFieldViewModel field)
    {
        ArgumentNullException.ThrowIfNull(field);
        if (field.Relation is not { IsAvailable: true } relation
            || !relation.TryGetTargetValue(field.Value, out var value))
        {
            StatusText = Strings.Get("EditorRelationTargetMissing");
            return;
        }

        if (relation.Relation.TargetTable == Descriptor.TableName)
        {
            SelectRecord(value);
            return;
        }

        TableNavigationRequested?.Invoke(
            this,
            new TableNavigationRequest(relation.Relation.TargetTable, value, TableDataDirectory));
    }

    /// <summary>
    /// Selects the record with the supplied key, clearing the search filter when the record is hidden.
    /// </summary>
    /// <param name="key">Primary record key.</param>
    public void SelectRecord(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        var row = _allRows.FirstOrDefault(candidate =>
            string.Equals(candidate.Key, key.Trim(), StringComparison.Ordinal));
        if (row is null)
        {
            StatusText = Strings.Format("EditorRecordNotFound", key);
            return;
        }

        if (!Rows.Contains(row))
        {
            SearchText = string.Empty;
            ApplyRowView(row.Key);
        }
        else
        {
            SelectedRow = row;
        }
    }

    /// <summary>
    /// Rebuilds rows and re-reads detail values after an external edit of a shared table set.
    /// </summary>
    /// <param name="selectKey">Key to select; the current selection is kept when omitted.</param>
    public void RefreshRows(string? selectKey = null)
    {
        RebuildRows(selectKey ?? SelectedRow?.Key);
        RefreshFieldValues();
    }

    /// <summary>
    /// Re-reads the value of every field shown in the detail panel and refreshes undo state.
    /// </summary>
    public void RefreshFieldValues()
    {
        foreach (var detailField in _allDetailFields)
        {
            detailField.Refresh();
        }

        IsDirty = Workspace?.IsDirty == true;
        RefreshHistoryState();
    }

    /// <summary>
    /// Reapplies the external row filter and search without rebuilding the rows.
    /// </summary>
    /// <param name="selectKey">Key to select; the current selection is kept when omitted.</param>
    public void RefreshRowFilter(string? selectKey = null) => ApplyRowView(selectKey);

    /// <summary>
    /// Opens the bit editor for a flag field of the detail form.
    /// </summary>
    /// <param name="field">Flag field to edit.</param>
    public void EditBits(DetailFieldViewModel field)
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
            _relationByField.TryGetValue(column.XmlFieldName, out var relation);
            _allDetailFields.Add(new DetailFieldViewModel(this, SelectedRow, column, lookup, relation));
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
                metadata.Add(field.GroupKey, (field.GroupOrder, field.Order));
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

    private bool TryGetGraphic(out int graphicType, out int graphicIndex) =>
        TryGetGraphic(SelectedRow, out graphicType, out graphicIndex);

    private static bool TryGetGraphic(
        TableRowViewModel? row,
        out int graphicType,
        out int graphicIndex)
    {
        graphicType = 0;
        graphicIndex = -1;
        if (row is null)
        {
            return false;
        }

        var rawIndex = row.GetStoredValue("ubGraphicNum");
        if (!int.TryParse(rawIndex, NumberStyles.Integer, CultureInfo.InvariantCulture, out graphicIndex)
            || graphicIndex < 0)
        {
            return false;
        }

        var rawType = row.GetStoredValue("ubGraphicType");
        if (int.TryParse(rawType, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedType)
            && parsedType >= 0)
        {
            graphicType = parsedType;
        }

        return true;
    }

    private async Task<BitmapSource?> LoadThumbnailAsync(TableRowViewModel row)
    {
        var dataDirectory = _session.GameDataDirectory;
        if (dataDirectory is null || !TryGetGraphic(row, out var graphicType, out var graphicIndex))
        {
            return null;
        }

        try
        {
            var result = await _itemImages
                .GetImageAsync(dataDirectory, ImageSize.Small, graphicType, graphicIndex, CancellationToken.None)
                .ConfigureAwait(true);
            return result.Data;
        }
        catch (Exception)
        {
            return null;
        }
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
