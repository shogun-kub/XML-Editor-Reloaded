using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Overlays;
using XmlEditorReloaded.Xml.Schema;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Master-detail editor for tables whose records or important fields live in nested structures, for
/// example <c>Map.Cities</c> (<c>COUNTRY</c> and <c>CITYLIST/CITY</c>) and <c>Map.FacilityTypes</c>
/// (<c>ASSIGNMENT</c> blocks).
/// </summary>
public sealed partial class NestedTableWorkspaceViewModel : ObservableObject, IEditorTab
{
    private readonly IDataSetSession _session;
    private readonly TableDescriptor _descriptor;
    private readonly IDialogService _dialogService;
    private readonly List<NestedRecordItemViewModel> _allRecords = new();
    private string? _languageCode;
    private string? _selectedKey;
    private bool _suppressLanguageChange;
    private int _assignmentSegmentIndex = -1;

    [ObservableProperty]
    private TableWorkspace? _workspace;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isDirty;

    [ObservableProperty]
    private bool _isEditable;

    [ObservableProperty]
    private string _statusText;

    [ObservableProperty]
    private string _detailHeader = string.Empty;

    [ObservableProperty]
    private string _recordCountText = string.Empty;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private NestedRecordItemViewModel? _selectedRecord;

    [ObservableProperty]
    private LanguageOptionViewModel? _selectedLanguageOption;

    [ObservableProperty]
    private bool _isTranslationMode;

    [ObservableProperty]
    private bool _isValidationVisible;

    [ObservableProperty]
    private bool _canUndo;

    [ObservableProperty]
    private bool _canRedo;

    [ObservableProperty]
    private string _newAssignmentType = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="NestedTableWorkspaceViewModel"/> class.
    /// </summary>
    /// <param name="session">Data-set session that owns the table.</param>
    /// <param name="descriptor">Table descriptor to open.</param>
    /// <param name="languageCode">Optional language code for overlay editing.</param>
    /// <param name="dialogService">Dialog service used for confirmations and external-change recovery.</param>
    public NestedTableWorkspaceViewModel(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode,
        IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(dialogService);

        _session = session;
        _descriptor = descriptor;
        _dialogService = dialogService;
        _languageCode = languageCode;
        Title = descriptor.TableName.Value;
        _statusText = Strings.Get("EditorLoading");
        SaveText = Strings.Get("EditorSaveText");
        UndoText = Strings.Get("EditorUndoText");
        RedoText = Strings.Get("EditorRedoText");
        ValidateText = Strings.Get("EditorValidateText");
        ReloadText = Strings.Get("EditorReloadText");
        AddRecordText = Strings.Get("EditorAddRowText");
        DeleteRecordText = Strings.Get("EditorDeleteRowText");
        SearchLabel = Strings.Get("EditorSearchLabel");
        LanguageLabel = Strings.Get("EditorLanguageLabel");
        ValidationLabel = Strings.Get("EditorValidationLabel");
        NoSelectionText = Strings.Get("NestedEditorSelectRecord");
        NewAssignmentTypeLabel = Strings.Get("NestedEditorAssignmentTypeLabel");
        AddAssignmentText = Strings.Get("NestedEditorAddAssignment");
    }

    /// <inheritdoc />
    public TableName TableName => _descriptor.TableName;

    /// <inheritdoc />
    public string TableDataDirectory => _session.TableDataDirectory;

    /// <inheritdoc />
    public string DatasetLabel => System.IO.Path.GetFileName(
        System.IO.Path.GetDirectoryName(_session.TableDataDirectory)) ?? _session.TableDataDirectory;

    /// <inheritdoc />
    public XmlSnapshot CreateXmlSnapshot() =>
        Workspace?.BaseDocument.CreateXmlSnapshot() ?? XmlSnapshot.Empty;

    /// <inheritdoc />
    public string? SelectedRecordKey => SelectedRecord?.Key;

    /// <inheritdoc />
    public string Title { get; }

    /// <inheritdoc />
    public string DisplayName => IsDirty ? Title + " *" : Title;

    /// <summary>
    /// Gets a value indicating whether the language selector can be used.
    /// </summary>
    public bool IsLanguageSelectorEnabled => !IsBusy && Workspace is not null;

    /// <summary>
    /// Gets the save button text.
    /// </summary>
    public string SaveText { get; }

    /// <summary>
    /// Gets the undo button text.
    /// </summary>
    public string UndoText { get; }

    /// <summary>
    /// Gets the redo button text.
    /// </summary>
    public string RedoText { get; }

    /// <summary>
    /// Gets the validate button text.
    /// </summary>
    public string ValidateText { get; }

    /// <summary>
    /// Gets the reload button text.
    /// </summary>
    public string ReloadText { get; }

    /// <summary>
    /// Gets the add-record button text.
    /// </summary>
    public string AddRecordText { get; }

    /// <summary>
    /// Gets the delete-record button text.
    /// </summary>
    public string DeleteRecordText { get; }

    /// <summary>
    /// Gets the search label.
    /// </summary>
    public string SearchLabel { get; }

    /// <summary>
    /// Gets the language selector label.
    /// </summary>
    public string LanguageLabel { get; }

    /// <summary>
    /// Gets the validation panel label.
    /// </summary>
    public string ValidationLabel { get; }

    /// <summary>
    /// Gets the text shown when no record is selected.
    /// </summary>
    public string NoSelectionText { get; }

    /// <summary>
    /// Gets the label of the new-assignment type input.
    /// </summary>
    public string NewAssignmentTypeLabel { get; }

    /// <summary>
    /// Gets the add-assignment button text.
    /// </summary>
    public string AddAssignmentText { get; }

    /// <summary>
    /// Gets a value indicating whether the table has a nested assignment key segment.
    /// </summary>
    public bool HasAssignmentSegment => _assignmentSegmentIndex > 0;

    /// <summary>
    /// Gets a value indicating whether the detail pane offers nested add actions.
    /// </summary>
    public bool HasNestedAddActions => HasAssignmentSegment || BlockAddActions.Count > 0;

    /// <summary>
    /// Gets the actions that append repeated nested blocks such as production lines.
    /// </summary>
    public ObservableCollection<NestedBlockAddViewModel> BlockAddActions { get; } = new();

    /// <summary>
    /// Gets the filtered record list.
    /// </summary>
    public ObservableCollection<NestedRecordItemViewModel> Records { get; } = new();

    /// <summary>
    /// Gets the expandable form sections of the selected record.
    /// </summary>
    public ObservableCollection<NestedFormSectionViewModel> Sections { get; } = new();

    /// <summary>
    /// Gets the language selector options.
    /// </summary>
    public ObservableCollection<LanguageOptionViewModel> LanguageOptions { get; } = new();

    /// <summary>
    /// Gets validation issues from the last validation run.
    /// </summary>
    public ObservableCollection<ValidationIssueItem> ValidationIssues { get; } = new();

    /// <inheritdoc />
    public event EventHandler<string?>? LanguageChanged;

    /// <inheritdoc />
    public event EventHandler<TableNavigationRequest>? TableNavigationRequested
    {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public Task InitializeAsync(CancellationToken cancellationToken = default) =>
        LoadWorkspaceAsync(null, cancellationToken);

    /// <inheritdoc />
    [RelayCommand]
    public async Task SaveAsync() => await SaveWithRecoveryAsync().ConfigureAwait(true);

    /// <inheritdoc />
    public async Task<bool> SaveWithRecoveryAsync()
    {
        if (Workspace is null)
        {
            return false;
        }

        if (!Workspace.IsDirty)
        {
            IsDirty = false;
            StatusText = Strings.Get("EditorNoChanges");
            return true;
        }

        IsBusy = true;
        try
        {
            var result = await Workspace.SaveAsync().ConfigureAwait(true);
            if (result.IsSuccess)
            {
                ApplySavedState();
                return true;
            }

            if (string.Equals(
                    result.ErrorCode,
                    ErrorCodes.XmlFileChangedExternally,
                    StringComparison.Ordinal))
            {
                return await ResolveExternalChangeAsync(result).ConfigureAwait(true);
            }

            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return false;
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("EditorLoadingCancelled");
            return false;
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <inheritdoc />
    public async Task ReloadAsync()
    {
        if (IsDirty && !_dialogService.Confirm(
                Strings.Format("EditorConfirmReload", Title),
                Strings.Get("EditorReloadTitle")))
        {
            return;
        }

        await LoadWorkspaceAsync(_selectedKey, CancellationToken.None).ConfigureAwait(true);
    }

    /// <inheritdoc />
    public Task ReloadFromDiskAsync() =>
        LoadWorkspaceAsync(_selectedKey, CancellationToken.None);

    /// <inheritdoc />
    public void SelectRecord(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        var trimmed = key.Trim();
        var match = _allRecords.FirstOrDefault(candidate =>
            string.Equals(candidate.Key, trimmed, StringComparison.Ordinal));
        if (match is null)
        {
            StatusText = Strings.Format("EditorRecordNotFound", key);
            return;
        }

        if (!Records.Contains(match))
        {
            SearchText = string.Empty;
        }

        SelectedRecord = Records.FirstOrDefault(candidate =>
            string.Equals(candidate.Key, trimmed, StringComparison.Ordinal)) ?? match;
    }

    /// <summary>
    /// Selects the nested record referenced by a validation issue. Table-level issues leave the current
    /// selection unchanged.
    /// </summary>
    /// <param name="issue">Validation issue selected in the issue list.</param>
    public void NavigateToValidationIssue(ValidationIssueItem issue)
    {
        ArgumentNullException.ThrowIfNull(issue);
        if (issue.TableName is { } tableName
            && !string.Equals(tableName.Value, _descriptor.TableName.Value, StringComparison.Ordinal))
        {
            StatusText = Strings.Format("EditorValidationIssueOtherTable", tableName.Value);
            return;
        }

        if (issue.RecordKey is not { Length: > 0 } key)
        {
            StatusText = Strings.Get("EditorValidationIssueNoRecord");
            return;
        }

        SelectRecord(key);
    }

    /// <summary>
    /// Reverses the most recent edit.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUndo))]
    public void Undo()
    {
        if (Workspace is null)
        {
            return;
        }

        var result = Workspace.Undo();
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        ApplyHistoryResult(Strings.Get("EditorUndoDone"));
    }

    /// <summary>
    /// Restores the most recently undone edit.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRedo))]
    public void Redo()
    {
        if (Workspace is null)
        {
            return;
        }

        var result = Workspace.Redo();
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        ApplyHistoryResult(Strings.Get("EditorRedoDone"));
    }

    /// <summary>
    /// Validates the table and lists the issues.
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
            ValidationIssues.Clear();
            var report = await Workspace
                .ValidateAsync(_session.Lookups, Workspace.Language?.Code)
                .ConfigureAwait(true);
            foreach (var issue in report.Issues)
            {
                ValidationIssues.Add(new ValidationIssueItem(
                    issue.TableName is null
                        ? issue with { TableName = Workspace.Descriptor.TableName }
                        : issue));
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
    /// Reloads the table after confirmation.
    /// </summary>
    /// <returns>A task that completes when the reload finishes.</returns>
    [RelayCommand]
    public async Task Reload() => await ReloadAsync().ConfigureAwait(true);

    /// <summary>
    /// Appends a record to the table.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddRecord))]
    public void AddRecord()
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

        var key = result.Data!.GetKey();
        IsDirty = Workspace.IsDirty;
        RefreshHistoryState();
        RebuildRecords(key);
        StatusText = Strings.Format("NestedEditorRecordAdded", key ?? string.Empty);
    }

    /// <summary>
    /// Deletes the selected record after confirmation.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanDeleteRecord))]
    public void DeleteRecord()
    {
        if (Workspace is null || SelectedRecord?.Key is not { } key)
        {
            return;
        }

        if (!_dialogService.Confirm(
                Strings.Format("EditorConfirmDelete", key),
                Strings.Get("EditorDeleteRowTitle")))
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
        RefreshHistoryState();
        RebuildRecords(null);
        StatusText = Strings.Format("NestedEditorRecordDeleted", key);
    }

    /// <summary>
    /// Appends a nested assignment with the type entered in the toolbar.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddAssignment))]
    private void AddAssignment()
    {
        if (Workspace is null || _selectedKey is null)
        {
            return;
        }

        var type = NewAssignmentType.Trim();
        if (type.Length == 0)
        {
            StatusText = Strings.Get("NestedEditorAssignmentTypeRequired");
            return;
        }

        var result = Workspace.AddNestedRecord(
            _selectedKey,
            _assignmentSegmentIndex,
            type,
            new[] { new KeyValuePair<string, string>("szTooltipText", string.Empty) });
        if (!result.IsSuccess)
        {
            StatusText = string.Equals(result.ErrorCode, ErrorCodes.KeyConflict, StringComparison.Ordinal)
                ? Strings.Format("NestedEditorAssignmentExists", type)
                : Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        NewAssignmentType = string.Empty;
        IsDirty = Workspace.IsDirty;
        RefreshHistoryState();
        BuildForm();
        StatusText = Strings.Format("NestedEditorAssignmentAdded", type);
    }

    private bool CanAddAssignment() =>
        IsEditable
        && !IsBusy
        && Workspace is not null
        && _selectedKey is not null
        && _assignmentSegmentIndex > 0
        && !string.IsNullOrWhiteSpace(NewAssignmentType);

    /// <summary>
    /// Deletes the assignment or repeated block shown by one section after confirmation.
    /// </summary>
    /// <param name="section">Section to remove.</param>
    [RelayCommand(CanExecute = nameof(CanDeleteSection))]
    private void DeleteSection(NestedFormSectionViewModel? section)
    {
        if (Workspace is null || _selectedKey is null || section is not { CanDelete: true })
        {
            return;
        }

        var isAssignment = section.NestedSegmentIndex > 0;
        var message = isAssignment
            ? Strings.Format("NestedEditorConfirmDeleteAssignment", section.Header)
            : Strings.Format("NestedEditorConfirmDeleteBlock", section.Header);
        if (!_dialogService.Confirm(message, Strings.Get("EditorDeleteRowTitle")))
        {
            return;
        }

        var result = isAssignment
            ? Workspace.DeleteNestedRecord(
                _selectedKey,
                section.NestedSegmentIndex,
                section.NestedOccurrenceIndex)
            : Workspace.DeleteRepeatedBlock(_selectedKey, section.BlockFieldName!, section.BlockIndex);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = Workspace.IsDirty;
        RefreshHistoryState();
        BuildForm();
        StatusText = isAssignment
            ? Strings.Format("NestedEditorAssignmentDeleted", section.Header)
            : Strings.Format("NestedEditorBlockDeleted", section.Header);
    }

    private bool CanDeleteSection(NestedFormSectionViewModel? section) =>
        IsEditable && !IsBusy && Workspace is not null && section?.CanDelete == true;

    private void AddBlock(string fieldName)
    {
        if (Workspace is null || _selectedKey is null)
        {
            return;
        }

        var result = Workspace.AddRepeatedBlock(
            _selectedKey,
            fieldName,
            BuildBlockTemplate(fieldName));
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = Workspace.IsDirty;
        RefreshHistoryState();
        BuildForm();
        StatusText = Strings.Format("NestedEditorBlockAdded", fieldName);
    }

    private IReadOnlyList<KeyValuePair<string, string>> BuildBlockTemplate(string fieldName)
    {
        var values = new List<KeyValuePair<string, string>>();
        if (Workspace is null)
        {
            return values;
        }

        foreach (var row in Workspace.GetRows())
        {
            if (row.Record is not { } record || record.GetOccurrenceCount(fieldName) == 0)
            {
                continue;
            }

            foreach (var name in record.GetChildFieldNames(fieldName))
            {
                var path = fieldName + "/" + name;
                if (record.GetChildFieldNames(FieldPath.Parse(path)).Count > 0)
                {
                    continue;
                }

                values.Add(new KeyValuePair<string, string>(name, string.Empty));
            }

            if (values.Count > 0)
            {
                break;
            }
        }

        return values;
    }

    private bool CanAddRecord() => IsEditable && !IsBusy && Workspace is not null;

    private bool CanDeleteRecord() =>
        IsEditable && !IsBusy && Workspace is not null && SelectedRecord?.Key is not null;

    private async Task<bool> ResolveExternalChangeAsync(OperationResult failedSave)
    {
        var choice = _dialogService.ResolveExternalChange(
            Strings.Format(
                "ExternalChangeMessageFormat",
                Title,
                failedSave.ErrorText ?? string.Empty),
            Strings.Get("ExternalChangeTitle"));
        switch (choice)
        {
            case ExternalChangeChoice.Reload:
                await ReloadFromDiskAsync().ConfigureAwait(true);
                StatusText = Strings.Format("ExternalChangeReloaded", Title);
                return !IsDirty;

            case ExternalChangeChoice.Overwrite:
                if (Workspace is null)
                {
                    return false;
                }

                var forced = await Workspace
                    .SaveAsync(CancellationToken.None, forceOverwrite: true)
                    .ConfigureAwait(true);
                StatusText = forced.IsSuccess
                    ? Strings.Format("ExternalChangeOverwritten", Title)
                    : Strings.Format("StatusError", $"{forced.ErrorCode}: {forced.ErrorText}");
                if (forced.IsSuccess)
                {
                    ApplySavedState();
                    return true;
                }

                return false;

            default:
                StatusText = Strings.Format("ExternalChangeCancelled", Title);
                return false;
        }
    }

    private void ApplySavedState()
    {
        _session.RelationResolver.Invalidate(_descriptor.TableName);
        IsDirty = false;
        StatusText = Strings.Get("EditorSaved");
        RebuildRecords(_selectedKey);
    }

    private void ApplyHistoryResult(string message)
    {
        IsDirty = Workspace?.IsDirty == true;
        RefreshHistoryState();
        RebuildRecords(_selectedKey);
        StatusText = message;
    }

    private async Task LoadWorkspaceAsync(string? preferredKey, CancellationToken cancellationToken)
    {
        IsBusy = true;
        StatusText = Strings.Get("EditorLoading");
        try
        {
            var previousKey = preferredKey ?? _selectedKey;
            var load = await _session
                .OpenTableAsync(_descriptor, _languageCode, cancellationToken)
                .ConfigureAwait(true);
            if (!load.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", $"{load.ErrorCode}: {load.ErrorText}");
                IsEditable = false;
                return;
            }

            if (Workspace is not null)
            {
                Workspace.History.Changed -= OnHistoryChanged;
            }

            Workspace = load.Data!;
            Workspace.History.Changed += OnHistoryChanged;
            _languageCode = Workspace.Language?.Code;
            IsTranslationMode = Workspace.Language is not null;
            IsEditable = Workspace.CanEdit;
            _assignmentSegmentIndex = ResolveAssignmentSegmentIndex();
            RebuildBlockActions();
            OnPropertyChanged(nameof(HasAssignmentSegment));
            OnPropertyChanged(nameof(HasNestedAddActions));
            BuildLanguageOptions();
            RebuildRecords(previousKey);
            RefreshHistoryState();
            IsDirty = Workspace.IsDirty;
            StatusText = IsEditable ? Strings.Get("EditorReady") : Strings.Get("StatusLoadedReadOnly");
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

    private void BuildLanguageOptions()
    {
        _suppressLanguageChange = true;
        try
        {
            LanguageOptions.Clear();
            LanguageOptions.Add(new LanguageOptionViewModel(null, false));
            foreach (var overlay in _descriptor.Overlays)
            {
                LanguageOptions.Add(new LanguageOptionViewModel(overlay.Language, true));
            }

            SelectedLanguageOption = LanguageOptions.FirstOrDefault(option =>
                    string.Equals(option.Code, Workspace?.Language?.Code, StringComparison.OrdinalIgnoreCase))
                ?? LanguageOptions[0];
        }
        finally
        {
            _suppressLanguageChange = false;
        }
    }

    private int ResolveAssignmentSegmentIndex()
    {
        if (Workspace?.Metadata?.RecordKey is not { } spec)
        {
            return -1;
        }

        for (var index = 1; index < spec.Segments.Count; index++)
        {
            var segment = spec.Segments[index];
            if (segment.Kind != RecordKeyKind.Singleton && !string.IsNullOrEmpty(segment.KeyField))
            {
                return index;
            }
        }

        return -1;
    }

    private void RebuildBlockActions()
    {
        BlockAddActions.Clear();
        if (Workspace is not null)
        {
            foreach (var column in Workspace.Schema.Columns)
            {
                if (!IsRepeatedBlock(column))
                {
                    continue;
                }

                var fieldName = column.XmlFieldName;
                BlockAddActions.Add(new NestedBlockAddViewModel(
                    Strings.Format("NestedEditorAddBlock", fieldName),
                    "NestedAddBlock_" + fieldName,
                    new RelayCommand(() => AddBlock(fieldName))));
            }
        }

        OnPropertyChanged(nameof(HasNestedAddActions));
    }

    private bool IsRepeatedBlock(TableSchemaColumn column) =>
        column.IsNested && column.IsList && !IsKeySegment(column.XmlFieldName);

    private bool IsKeySegment(string fieldName)
    {
        if (Workspace?.Metadata?.RecordKey is not { } spec)
        {
            return false;
        }

        for (var index = 1; index < spec.Segments.Count; index++)
        {
            var segment = spec.Segments[index];
            if (segment.Kind != RecordKeyKind.Singleton
                && string.Equals(segment.Path, fieldName, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void RebuildRecords(string? preferredKey)
    {
        _allRecords.Clear();
        if (Workspace is not null)
        {
            foreach (var row in Workspace.GetRows())
            {
                _allRecords.Add(new NestedRecordItemViewModel(row, BuildDisplayName(row)));
            }
        }

        ApplyFilter(preferredKey);
    }

    private void ApplyFilter(string? preferredKey)
    {
        var previousKey = preferredKey ?? SelectedRecord?.Key;
        var term = SearchText.Trim();
        Records.Clear();
        foreach (var record in _allRecords)
        {
            if (term.Length == 0
                || record.KeyDisplay.Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.DisplayName.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                Records.Add(record);
            }
        }

        SelectedRecord = Records.FirstOrDefault(candidate =>
                string.Equals(candidate.Key, previousKey, StringComparison.Ordinal))
            ?? Records.FirstOrDefault();
        RecordCountText = Strings.Format("EditorRowCount", Records.Count, _allRecords.Count);
    }

    private string BuildDisplayName(WorkspaceRow row)
    {
        if (Workspace is not null)
        {
            foreach (var column in Workspace.Schema.Columns)
            {
                if (column.IsKey
                    || column.IsNested
                    || column.IsList
                    || column.IsDict
                    || column.DataType != ColumnDataType.String)
                {
                    continue;
                }

                var value = row.GetValue(column.XmlFieldName);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
        }

        return row.KeyDisplay;
    }

    private void OnHistoryChanged(object? sender, EventArgs e) => RefreshHistoryState();

    private void RefreshHistoryState()
    {
        CanUndo = Workspace?.History.CanUndo == true;
        CanRedo = Workspace?.History.CanRedo == true;
        IsDirty = Workspace?.IsDirty == true;
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
        AddRecordCommand.NotifyCanExecuteChanged();
        DeleteRecordCommand.NotifyCanExecuteChanged();
        AddAssignmentCommand.NotifyCanExecuteChanged();
        DeleteSectionCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedRecordChanged(NestedRecordItemViewModel? value)
    {
        _selectedKey = value?.Key;
        AddAssignmentCommand.NotifyCanExecuteChanged();
        BuildForm();
    }

    partial void OnNewAssignmentTypeChanged(string value) =>
        AddAssignmentCommand.NotifyCanExecuteChanged();

    partial void OnSearchTextChanged(string value) => ApplyFilter(null);

    partial void OnSelectedLanguageOptionChanged(LanguageOptionViewModel? value)
    {
        if (_suppressLanguageChange || value is null)
        {
            return;
        }

        var code = value.Code;
        if (string.Equals(code, Workspace?.Language?.Code, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (IsDirty && !_dialogService.Confirm(
                Strings.Format("EditorConfirmLanguage", Title),
                Strings.Get("EditorLanguageTitle")))
        {
            _suppressLanguageChange = true;
            try
            {
                SelectedLanguageOption = LanguageOptions.FirstOrDefault(option =>
                        string.Equals(option.Code, Workspace?.Language?.Code, StringComparison.OrdinalIgnoreCase))
                    ?? LanguageOptions[0];
            }
            finally
            {
                _suppressLanguageChange = false;
            }

            return;
        }

        _languageCode = code;
        _ = ChangeLanguageAsync();
    }

    private async Task ChangeLanguageAsync()
    {
        await LoadWorkspaceAsync(_selectedKey, CancellationToken.None).ConfigureAwait(true);
        LanguageChanged?.Invoke(this, Workspace?.Language?.Code);
    }

    partial void OnIsDirtyChanged(bool value) => OnPropertyChanged(nameof(DisplayName));

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsLanguageSelectorEnabled));
        AddAssignmentCommand.NotifyCanExecuteChanged();
        DeleteSectionCommand.NotifyCanExecuteChanged();
    }

    partial void OnCanUndoChanged(bool value) => UndoCommand.NotifyCanExecuteChanged();

    partial void OnCanRedoChanged(bool value) => RedoCommand.NotifyCanExecuteChanged();

    partial void OnIsEditableChanged(bool value)
    {
        AddRecordCommand.NotifyCanExecuteChanged();
        DeleteRecordCommand.NotifyCanExecuteChanged();
        AddAssignmentCommand.NotifyCanExecuteChanged();
        DeleteSectionCommand.NotifyCanExecuteChanged();
    }

    private void BuildForm()
    {
        Sections.Clear();
        if (Workspace is null || _selectedKey is null)
        {
            DetailHeader = NoSelectionText;
            return;
        }

        var row = Workspace.GetRow(_selectedKey);
        if (row is null)
        {
            DetailHeader = NoSelectionText;
            return;
        }

        DetailHeader = Strings.Format("DetailHeaderFormat", row.KeyDisplay);
        AddRootSections();
        AddMainSections(row);
        AddMainListSections(row);
        AddNestedLeafSections(row);
        AddRepeatedBlockSections(row);
        AddNestedCollectionSections();
        DeleteSectionCommand.NotifyCanExecuteChanged();
    }

    private void AddRootSections()
    {
        if (Workspace?.Metadata?.RecordKey is not { } spec)
        {
            return;
        }

        var cells = Workspace.GetRootCells();
        for (var index = 1; index < spec.Segments.Count; index++)
        {
            var segment = spec.Segments[index];
            if (segment.Kind != RecordKeyKind.Singleton)
            {
                continue;
            }

            var prefix = segment.Path + "/";
            var segmentCells = cells
                .Where(cell => cell.FieldPath.StartsWith(prefix, StringComparison.Ordinal))
                .ToArray();
            if (segmentCells.Length == 0)
            {
                continue;
            }

            var fields = segmentCells
                .Select(cell => CreateRootField(cell.FieldPath))
                .ToArray();
            Sections.Add(new NestedFormSectionViewModel(
                segment.Path,
                "NestedSectionRoot_" + segment.Path,
                fields));
        }
    }

    private NestedFieldViewModel CreateRootField(string path)
    {
        var name = path[(path.LastIndexOf('/') + 1)..];
        return CreateField(
            name,
            path,
            () => FindRootCell(path)?.Value,
            value => SetRootValue(path, value),
            isReadOnly: !IsEditable,
            isTranslatable: Workspace?.Overlay is not null
                && Workspace.IsTranslatable(FieldPath.Parse(path)),
            isTranslated: () => FindRootCell(path)?.IsTranslated == true);
    }

    private MergedCell? FindRootCell(string path) => Workspace?
        .GetRootCells()
        .FirstOrDefault(cell => string.Equals(cell.FieldPath, path, StringComparison.Ordinal));

    private void SetRootValue(string path, string value)
    {
        if (Workspace is null)
        {
            return;
        }

        var result = Workspace.SetRootValue(FieldPath.Parse(path), value);
        AfterEdit(result, path);
    }

    private void AddMainSections(WorkspaceRow row)
    {
        if (Workspace is null)
        {
            return;
        }

        var groups = new Dictionary<string, (int Order, List<NestedFieldViewModel> Fields)>(StringComparer.Ordinal);
        foreach (var column in Workspace.Schema.Columns)
        {
            if (column.IsList || column.IsDict || column.IsNested || string.IsNullOrEmpty(column.XmlFieldName))
            {
                continue;
            }

            var fieldName = column.XmlFieldName;
            var isKey = column.IsKey;
            var field = CreateField(
                fieldName,
                fieldName,
                () => GetMainValue(fieldName),
                value => SetMainValue(fieldName, value),
                isReadOnly: !IsEditable || isKey || row.Key is null,
                isTranslatable: Workspace.Overlay is not null && column.IsTranslatable,
                isTranslated: () => IsMainTranslated(fieldName));
            var groupKey = string.IsNullOrEmpty(column.GroupKey) ? "other" : column.GroupKey;
            if (!groups.TryGetValue(groupKey, out var group))
            {
                group = (column.GroupOrder, new List<NestedFieldViewModel>());
                groups.Add(groupKey, group);
            }

            group.Fields.Add(field);
        }

        foreach (var group in groups.OrderBy(static pair => pair.Value.Order))
        {
            Sections.Add(new NestedFormSectionViewModel(
                GroupLabel(group.Key),
                "NestedSectionGroup_" + group.Key,
                group.Value.Fields));
        }
    }

    private void AddMainListSections(WorkspaceRow row)
    {
        if (Workspace is null || row.Record is null)
        {
            return;
        }

        var lists = new List<NestedListViewModel>();
        foreach (var column in Workspace.Schema.Columns)
        {
            if (!column.IsList
                || column.IsNested
                || column.MaxOccurrences <= 1
                || IsKeySegment(column.XmlFieldName))
            {
                continue;
            }

            var fieldName = column.XmlFieldName;
            lists.Add(new NestedListViewModel(
                fieldName,
                fieldName,
                IsEditable,
                row.GetValues(fieldName),
                values => SetMainListValues(fieldName, values)));
        }

        if (lists.Count > 0)
        {
            Sections.Add(new NestedFormSectionViewModel(
                "Lists",
                "NestedSectionLists",
                Array.Empty<NestedFieldViewModel>(),
                lists: lists));
        }
    }

    private void SetMainListValues(string fieldName, IReadOnlyList<string> values)
    {
        if (Workspace is null || _selectedKey is null)
        {
            return;
        }

        var result = Workspace.SetListValues(_selectedKey, fieldName, values);
        AfterEdit(result, fieldName);
    }

    private string? GetMainValue(string path) => Workspace?.GetRow(_selectedKey ?? string.Empty)?.GetValue(path);

    private bool IsMainTranslated(string path) =>
        Workspace?.GetRow(_selectedKey ?? string.Empty)?.IsTranslated(path) == true;

    private void SetMainValue(string path, string value)
    {
        if (Workspace is null || _selectedKey is null)
        {
            return;
        }

        var wasAbsent = GetMainValue(path) is null;
        var result = Workspace.SetValue(
            _selectedKey,
            FieldPath.Parse(path),
            occurrenceIndex: 0,
            value,
            addIfMissing: wasAbsent);
        AfterEdit(result, path);
    }

    private void AddNestedLeafSections(WorkspaceRow row)
    {
        if (Workspace is null || row.Record is null)
        {
            return;
        }

        foreach (var column in Workspace.Schema.Columns)
        {
            if (!column.IsNested || IsNestedCollection(column.XmlFieldName) || IsRepeatedBlock(column))
            {
                continue;
            }

            var container = column.XmlFieldName;
            var childNames = row.Record.GetChildFieldNames(container);
            if (childNames.Count == 0)
            {
                continue;
            }

            var fields = new List<NestedFieldViewModel>();
            foreach (var child in childNames)
            {
                var path = container + "/" + child;
                if (row.Record.GetChildFieldNames(FieldPath.Parse(path)).Count > 0)
                {
                    continue;
                }

                fields.Add(CreateField(
                    child,
                    path,
                    () => GetMainValue(path),
                    value => SetMainValue(path, value),
                    isReadOnly: !IsEditable || row.Key is null,
                    isTranslatable: Workspace.Overlay is not null
                        && Workspace.IsTranslatable(FieldPath.Parse(path)),
                    isTranslated: () => IsMainTranslated(path)));
            }

            if (fields.Count > 0)
            {
                Sections.Add(new NestedFormSectionViewModel(
                    container,
                    "NestedSection_" + container,
                    fields));
            }
        }
    }

    private void AddRepeatedBlockSections(WorkspaceRow row)
    {
        if (Workspace is null || row.Record is null)
        {
            return;
        }

        foreach (var column in Workspace.Schema.Columns)
        {
            if (!IsRepeatedBlock(column))
            {
                continue;
            }

            var fieldName = column.XmlFieldName;
            var count = row.Record.GetOccurrenceCount(fieldName);
            for (var index = 0; index < count; index++)
            {
                var fields = new List<NestedFieldViewModel>();
                var lists = new List<NestedListViewModel>();
                CollectBlockFields(
                    row.Record,
                    fieldName,
                    index,
                    fieldName + "[" + (index + 1) + "]",
                    fields,
                    lists);
                Sections.Add(new NestedFormSectionViewModel(
                    fieldName + " #" + (index + 1),
                    "NestedSection_" + fieldName + "_" + index,
                    fields,
                    lists: lists,
                    blockFieldName: fieldName,
                    blockIndex: index));
            }
        }
    }

    private void CollectBlockFields(
        TableRecord record,
        string prefix,
        int blockIndex,
        string displayPrefix,
        List<NestedFieldViewModel> fields,
        List<NestedListViewModel> lists)
    {
        foreach (var childName in record.GetChildFieldNames(FieldPath.Parse(prefix), blockIndex))
        {
            var path = prefix + "/" + childName;
            var childNames = record.GetChildFieldNames(FieldPath.Parse(path), blockIndex);
            var values = record.GetRepeatedValues(FieldPath.Parse(path), blockIndex);
            if (childNames.Count > 0)
            {
                if (values.Count > 1)
                {
                    continue;
                }

                CollectBlockFields(
                    record,
                    path,
                    blockIndex,
                    displayPrefix + "/" + childName,
                    fields,
                    lists);
                continue;
            }

            if (values.Count > 1)
            {
                lists.Add(new NestedListViewModel(
                    childName,
                    displayPrefix + "/" + childName,
                    IsEditable,
                    values,
                    newValues => SetBlockListValues(path, blockIndex, newValues)));
                continue;
            }

            fields.Add(CreateField(
                childName,
                displayPrefix + "/" + childName,
                () => record.GetRawValue(FieldPath.Parse(path), blockIndex),
                value => SetBlockField(path, blockIndex, value),
                isReadOnly: !IsEditable,
                isTranslatable: Workspace?.Overlay is not null
                    && Workspace.IsTranslatable(FieldPath.Parse(path)),
                isTranslated: () => false));
        }
    }

    private void SetBlockField(string path, int blockIndex, string value)
    {
        if (Workspace is null || _selectedKey is null)
        {
            return;
        }

        var record = Workspace.GetRow(_selectedKey)?.Record;
        var wasAbsent = record?.GetRawValue(FieldPath.Parse(path), blockIndex) is null;
        var result = Workspace.SetValue(
            _selectedKey,
            FieldPath.Parse(path),
            blockIndex,
            value,
            addIfMissing: wasAbsent);
        AfterEdit(result, path);
    }

    private void SetBlockListValues(string path, int blockIndex, IReadOnlyList<string> values)
    {
        if (Workspace is null || _selectedKey is null)
        {
            return;
        }

        var result = Workspace.SetRepeatedValues(
            _selectedKey,
            FieldPath.Parse(path),
            values,
            blockIndex);
        AfterEdit(result, path);
    }

    private bool IsNestedCollection(string fieldName)
    {
        var spec = Workspace?.Metadata?.RecordKey;
        if (spec is null)
        {
            return false;
        }

        for (var index = 1; index < spec.Segments.Count; index++)
        {
            var segment = spec.Segments[index];
            if (segment.Kind != RecordKeyKind.Singleton
                && string.Equals(segment.Path, fieldName, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void AddNestedCollectionSections()
    {
        if (Workspace?.Metadata?.RecordKey is not { } spec || _selectedKey is null)
        {
            return;
        }

        for (var index = 1; index < spec.Segments.Count; index++)
        {
            var segment = spec.Segments[index];
            if (segment.Kind == RecordKeyKind.Singleton)
            {
                continue;
            }

            var nestedRecords = Workspace.GetNestedRecords(_selectedKey, index);
            foreach (var nested in nestedRecords)
            {
                var fields = new List<NestedFieldViewModel>();
                var lists = new List<NestedListViewModel>();
                CollectNestedFields(
                    nested,
                    segment,
                    segment.Path + "[" + nested.KeyDisplay + "]",
                    fields,
                    lists);
                Sections.Add(new NestedFormSectionViewModel(
                    segment.Path + ": " + nested.KeyDisplay,
                    "NestedSection_" + segment.Path + "_" + Sanitize(nested.KeyDisplay),
                    fields,
                    lists: lists,
                    nestedSegmentIndex: index,
                    nestedOccurrenceIndex: nested.Index,
                    nestedKey: nested.Key));
            }
        }
    }

    private void CollectNestedFields(
        NestedWorkspaceRecord nested,
        RecordKeySegment segment,
        string displayPrefix,
        List<NestedFieldViewModel> fields,
        List<NestedListViewModel> lists) =>
        CollectNestedChildren(nested, segment, prefix: string.Empty, displayPrefix, fields, lists);

    private void CollectNestedChildren(
        NestedWorkspaceRecord nested,
        RecordKeySegment segment,
        string prefix,
        string displayPrefix,
        List<NestedFieldViewModel> fields,
        List<NestedListViewModel> lists)
    {
        var record = nested.Record;
        var names = prefix.Length == 0
            ? record.FieldNames
            : record.GetChildFieldNames(FieldPath.Parse(prefix));
        foreach (var name in names)
        {
            var path = prefix.Length == 0 ? name : prefix + "/" + name;
            var childNames = record.GetChildFieldNames(FieldPath.Parse(path));
            var values = record.GetRepeatedValues(FieldPath.Parse(path));
            if (childNames.Count > 0)
            {
                if (values.Count > 1)
                {
                    continue;
                }

                CollectNestedChildren(
                    nested,
                    segment,
                    path,
                    displayPrefix + "/" + name,
                    fields,
                    lists);
                continue;
            }

            if (values.Count > 1)
            {
                var listPath = path;
                lists.Add(new NestedListViewModel(
                    name,
                    displayPrefix + "/" + name,
                    IsEditable,
                    values,
                    newValues => SetNestedListValues(nested, listPath, newValues)));
                continue;
            }

            var fieldPath = path;
            var isKeyField = segment.KeyField is not null
                && string.Equals(path, segment.KeyField, StringComparison.Ordinal);
            fields.Add(CreateField(
                name,
                displayPrefix + "/" + name,
                () => GetNestedValue(nested, fieldPath),
                value => SetNestedField(nested, fieldPath, value),
                isReadOnly: !IsEditable || nested.Key is null || isKeyField,
                isTranslatable: Workspace?.Overlay is not null
                    && Workspace.IsTranslatable(FieldPath.Parse(fieldPath)),
                isTranslated: () => Workspace?.IsNestedTranslated(
                    _selectedKey ?? string.Empty,
                    nested,
                    FieldPath.Parse(fieldPath)) == true));
        }
    }

    private void SetNestedListValues(NestedWorkspaceRecord nested, string path, IReadOnlyList<string> values)
    {
        if (Workspace is null || _selectedKey is null)
        {
            return;
        }

        var result = Workspace.SetNestedRepeatedValues(
            _selectedKey,
            nested,
            FieldPath.Parse(path),
            values);
        AfterEdit(result, path);
    }

    private string? GetNestedValue(NestedWorkspaceRecord nested, string path) =>
        Workspace?.GetNestedValue(_selectedKey ?? string.Empty, nested, FieldPath.Parse(path));

    private void SetNestedField(NestedWorkspaceRecord nested, string path, string value)
    {
        if (Workspace is null || _selectedKey is null)
        {
            return;
        }

        var fieldPath = FieldPath.Parse(path);
        var wasAbsent = nested.Record.GetRawValue(fieldPath) is null;
        OperationResult result;
        if (Workspace.Overlay is not null
            && nested.Key is { Length: > 0 }
            && Workspace.IsTranslatable(fieldPath))
        {
            var key = new OverlayRecordKey(_selectedKey, new[] { nested.Key });
            result = Workspace.SetNestedValue(key, fieldPath, value);
        }
        else
        {
            var writePath = FieldPath.Parse(
                Workspace.Metadata!.RecordKey!.Segments[nested.SegmentIndex].Path + "/" + path);
            result = Workspace.SetValue(
                _selectedKey,
                writePath,
                nested.Index,
                value,
                addIfMissing: wasAbsent);
        }

        AfterEdit(result, path);
    }

    private void AfterEdit(OperationResult result, string path)
    {
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = Workspace?.IsDirty == true;
        RefreshHistoryState();
        BuildForm();
        StatusText = Workspace?.Language is not null && Workspace.IsTranslatable(FieldPath.Parse(path))
            ? Strings.Format("EditorCellUpdated", path, _selectedKey ?? string.Empty)
            : Strings.Format("EditorBaseCellUpdated", path, _selectedKey ?? string.Empty);
    }

    private NestedFieldViewModel CreateField(
        string name,
        string pathText,
        Func<string?> read,
        Action<string> write,
        bool isReadOnly,
        bool isTranslatable,
        Func<bool> isTranslated) =>
        new(name, pathText, read, write, isReadOnly, isTranslatable, isTranslated);

    private static string GroupLabel(string key)
    {
        var lookup = "DetailGroup." + key;
        var label = Strings.Get(lookup);
        return string.Equals(label, lookup, StringComparison.Ordinal) ? key : label;
    }

    private static string Sanitize(string value) =>
        new(value.Select(static character => char.IsLetterOrDigit(character) ? character : '_').ToArray());
}
