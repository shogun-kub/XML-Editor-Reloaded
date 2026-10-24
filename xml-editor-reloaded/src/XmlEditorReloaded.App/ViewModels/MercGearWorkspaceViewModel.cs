using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Overlays;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Mercenary starting-gear workspace: a mercenary list and one editable form per gear kit. Item slots use
/// relation pickers, kit names are translated through the language overlay, and every change shares one
/// undo history and one save action with the base table.
/// </summary>
public sealed partial class MercGearWorkspaceViewModel : ObservableObject, IEditorTab
{
    private const string GearCollection = "GEARKIT";
    private const string KitNameField = "mGearKitName";

    /// <summary>
    /// Slot layout of a gear kit. Fields that are absent in a sparse kit stay absent until the user
    /// edits them, so opening and saving a kit does not normalize the document.
    /// </summary>
    private static readonly SlotDefinition[] SlotDefinitions = BuildSlotDefinitions();

    private readonly IDataSetSession _session;
    private readonly IDialogService _dialogService;
    private readonly TableDescriptor _descriptor;
    private readonly List<MercGearEntryViewModel> _allMercs = new();
    private IReadOnlyList<LanguageDescriptor> _languages = Array.Empty<LanguageDescriptor>();
    private FieldSourceSet? _fieldSources;
    private TableSetWorkspace? _workspace;
    private string? _languageCode;
    private bool _suppressLanguageChange;
    private bool _suppressSelection;
    private bool _isReloading;
    private string? _pendingSelection;

    [ObservableProperty]
    private string _statusText;

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
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _countText = string.Empty;

    [ObservableProperty]
    private string _languageText = string.Empty;

    [ObservableProperty]
    private string _warningsText = string.Empty;

    [ObservableProperty]
    private string _detailHeader = string.Empty;

    [ObservableProperty]
    private bool _isTranslationMode;

    [ObservableProperty]
    private MercGearEntryViewModel? _selectedMerc;

    [ObservableProperty]
    private LanguageOptionViewModel? _selectedLanguageOption;

    /// <summary>
    /// Initializes a new instance of the <see cref="MercGearWorkspaceViewModel"/> class.
    /// </summary>
    /// <param name="session">Data-set session that opens the gear table.</param>
    /// <param name="descriptor">Descriptor of <c>Inventory.MercStartingGear</c>.</param>
    /// <param name="languageCode">Optional language overlay code.</param>
    /// <param name="dialogService">Dialog service used for confirmations.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required service or descriptor is null.</exception>
    public MercGearWorkspaceViewModel(
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
        _languageCode = languageCode;
        _dialogService = dialogService;

        Title = descriptor.TableName.Value.Split('.').Last();
        _statusText = Strings.Get("EditorLoading");
        _detailHeader = Strings.Get("MercGearNoSelection");

        SaveText = Strings.Get("EditorSaveText");
        UndoText = Strings.Get("EditorUndoText");
        RedoText = Strings.Get("EditorRedoText");
        UndoButtonTooltip = Strings.Get("EditorUndoButtonTooltip");
        RedoButtonTooltip = Strings.Get("EditorRedoButtonTooltip");
        ValidateText = Strings.Get("EditorValidateText");
        ReloadText = Strings.Get("EditorReloadText");
        SearchLabel = Strings.Get("MercGearSearchLabel");
        LanguageLabel = Strings.Get("EditorLanguageLabel");
        ValidationLabel = Strings.Get("EditorValidationLabel");
    }

    /// <inheritdoc />
    public TableName TableName => _descriptor.TableName;

    /// <inheritdoc />
    public string TableDataDirectory => _session.TableDataDirectory;

    /// <inheritdoc />
    public string DatasetLabel => DatasetNames.LabelFor(_session);

    /// <inheritdoc />
    public string? SelectedRecordKey => SelectedMerc?.Key;

    /// <inheritdoc />
    public string Title { get; }

    /// <inheritdoc />
    public string DisplayName => IsDirty ? Title + " *" : Title;

    /// <summary>
    /// Gets a value indicating whether the gear set accepts edits.
    /// </summary>
    public bool CanEdit => IsEditable;

    /// <summary>
    /// Gets a value indicating whether the language selector can be used.
    /// </summary>
    public bool IsLanguageSelectorEnabled => !IsBusy && _workspace is not null;

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
    /// Gets the undo button tooltip.
    /// </summary>
    public string UndoButtonTooltip { get; }

    /// <summary>
    /// Gets the redo button tooltip.
    /// </summary>
    public string RedoButtonTooltip { get; }

    /// <summary>
    /// Gets the validate button text.
    /// </summary>
    public string ValidateText { get; }

    /// <summary>
    /// Gets the reload button text.
    /// </summary>
    public string ReloadText { get; }

    /// <summary>
    /// Gets the mercenary search label.
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
    /// Gets the mercenaries shown in the selector, filtered by <see cref="SearchText"/>.
    /// </summary>
    public ObservableCollection<MercGearEntryViewModel> Mercs { get; } = new();

    /// <summary>
    /// Gets the gear kits of the selected mercenary.
    /// </summary>
    public ObservableCollection<MercGearKitViewModel> Kits { get; } = new();

    /// <summary>
    /// Gets the language selector options: the base table and existing overlays.
    /// </summary>
    public ObservableCollection<LanguageOptionViewModel> LanguageOptions { get; } = new();

    /// <summary>
    /// Gets validation issues from the last validation run.
    /// </summary>
    public ObservableCollection<ValidationIssueItem> ValidationIssues { get; } = new();

    /// <summary>
    /// Gets the number of gear kits of the selected mercenary, for tests and diagnostics.
    /// </summary>
    public int KitCount => Kits.Count;

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
        InitializeCoreAsync(cancellationToken);

    /// <summary>
    /// Loads the gear table and its item picker sources.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>A task that completes when the workspace is ready.</returns>
    private async Task InitializeCoreAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        StatusText = Strings.Get("EditorLoading");
        var selectedKey = _pendingSelection ?? SelectedMerc?.Key;
        try
        {
            var languagesResult = await _session.GetLanguagesAsync(cancellationToken).ConfigureAwait(true);
            if (languagesResult.IsSuccess)
            {
                _languages = languagesResult.Data!;
            }

            var load = await _session
                .OpenTableSetAsync(
                    _descriptor,
                    _languageCode,
                    includeReverseRelations: false,
                    cancellationToken)
                .ConfigureAwait(true);
            if (!load.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", $"{load.ErrorCode}: {load.ErrorText}");
                IsEditable = false;
                return;
            }

            if (_workspace is not null)
            {
                _workspace.History.Changed -= OnHistoryChanged;
            }

            _workspace = load.Data!;
            _workspace.History.Changed += OnHistoryChanged;
            _languageCode = _workspace.Primary.Language?.Code;
            IsTranslationMode = _workspace.Primary.Language is not null;
            LanguageText = _workspace.Primary.Language is null
                ? Strings.Get("EditorBaseLanguage")
                : Strings.Format("EditorLanguageFormat", _workspace.Primary.Language.DisplayName);
            WarningsText = string.Join(Environment.NewLine, _workspace.Warnings);
            IsEditable = _workspace.CanEdit;

            await LoadFieldSourcesAsync(cancellationToken).ConfigureAwait(true);
            BuildLanguageOptions();
            RebuildMercList(selectedKey);

            StatusText = _workspace.Primary.Metadata is null
                ? Strings.Get("ItemWorkspaceNoMetadata")
                : Strings.Get("EditorReady");
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
            _pendingSelection = null;
            RefreshHistoryState();
            IsBusy = false;
        }
    }

    /// <summary>
    /// Saves the gear table and its overlay when they have unsaved changes.
    /// </summary>
    /// <returns>A task that completes when saving finishes.</returns>
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (_workspace is null)
        {
            return;
        }

        if (!_workspace.IsDirty)
        {
            IsDirty = false;
            StatusText = Strings.Get("EditorNoChanges");
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _workspace.SaveAsync().ConfigureAwait(true);
            if (result.IsSuccess)
            {
                _session.RelationResolver.Invalidate(_descriptor.TableName);
                IsDirty = false;
                StatusText = Strings.Get("EditorSaved");
                RefreshAllKits();
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
    /// Reverses the most recent gear edit.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUndo))]
    public void Undo() => ApplyHistory(undo: true);

    /// <summary>
    /// Restores the most recently undone gear edit.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRedo))]
    public void Redo() => ApplyHistory(undo: false);

    /// <summary>
    /// Validates the gear table and lists the issues.
    /// </summary>
    /// <returns>A task that completes when validation finishes.</returns>
    [RelayCommand]
    public async Task ValidateAsync()
    {
        if (_workspace is null)
        {
            return;
        }

        IsBusy = true;
        try
        {
            ValidationIssues.Clear();
            var workspace = _workspace.Primary;
            var report = await workspace
                .ValidateAsync(_session.Lookups, workspace.Language?.Code)
                .ConfigureAwait(true);
            foreach (var issue in report.Issues)
            {
                ValidationIssues.Add(new ValidationIssueItem(
                    issue.TableName is null
                        ? issue with { TableName = workspace.Descriptor.TableName }
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
    /// Discards unsaved changes after confirmation and reloads the gear table.
    /// </summary>
    /// <returns>A task that completes when the workspace is reloaded.</returns>
    [RelayCommand]
    public async Task ReloadAsync()
    {
        if (IsDirty && !_dialogService.Confirm(
                Strings.Format("EditorConfirmReload", Title),
                Strings.Get("EditorReloadTitle")))
        {
            return;
        }

        await ReloadPreservingSelectionAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Selects the mercenary with the supplied key.
    /// </summary>
    /// <param name="key">Primary key, the <c>mIndex</c> value.</param>
    public void SelectRecord(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        if (_workspace is null || _allMercs.Count == 0)
        {
            _pendingSelection = key.Trim();
            return;
        }

        var entry = _allMercs.FirstOrDefault(candidate =>
            string.Equals(candidate.Key, key.Trim(), StringComparison.Ordinal));
        if (entry is null)
        {
            StatusText = Strings.Format("EditorRecordNotFound", key);
            return;
        }

        if (!Mercs.Contains(entry))
        {
            SearchText = string.Empty;
            ApplyMercFilter(entry.Key);
        }
        else
        {
            SelectedMerc = entry;
        }
    }

    /// <summary>
    /// Applies one gear-kit field to the selected kit occurrence.
    /// </summary>
    /// <param name="kitIndex">Zero-based kit index.</param>
    /// <param name="field">XML field name inside <c>GEARKIT</c>.</param>
    /// <param name="value">New raw value.</param>
    internal void SetKitField(int kitIndex, string field, string value)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(value);

        var set = _workspace;
        var workspace = set?.Primary;
        if (workspace is null || SelectedMerc?.Key is not { } key)
        {
            return;
        }

        if (!IsEditable)
        {
            StatusText = Strings.Get("EditorReadOnly");
            RefreshKit(kitIndex);
            return;
        }

        var path = FieldPath.Parse(GearCollection + "/" + field);
        var wasAbsent = workspace.BaseDocument.FindRecord(key)?.GetRawValue(path, kitIndex) is null;
        if (wasAbsent && value.Length == 0)
        {
            return;
        }

        var result = workspace.SetValue(key, path, kitIndex, value, addIfMissing: wasAbsent);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            RefreshKit(kitIndex);
            return;
        }

        IsDirty = set!.IsDirty;
        RefreshHistoryState();
        RefreshKit(kitIndex);
        StatusText = Strings.Format("EditorCellUpdated", field, key);
    }

    /// <summary>
    /// Applies a kit-name edit. With a language overlay selected the value is written only to the
    /// overlay; otherwise it is written to the base table.
    /// </summary>
    /// <param name="kit">Edited kit.</param>
    /// <param name="value">New kit name.</param>
    internal void SetKitName(MercGearKitViewModel kit, string value)
    {
        ArgumentNullException.ThrowIfNull(kit);
        ArgumentNullException.ThrowIfNull(value);

        var set = _workspace;
        var workspace = set?.Primary;
        if (workspace is null || SelectedMerc?.Key is not { } key)
        {
            return;
        }

        if (!IsEditable)
        {
            StatusText = Strings.Get("EditorReadOnly");
            RefreshKit(kit.Index);
            return;
        }

        var path = FieldPath.Parse(GearCollection + "/" + KitNameField);
        OperationResult result;
        if (workspace.Overlay is not null)
        {
            var nestedKey = new OverlayRecordKey(
                key,
                new[] { kit.Index.ToString(CultureInfo.InvariantCulture) });
            result = workspace.SetNestedValue(nestedKey, path, value);
        }
        else
        {
            var wasAbsent = workspace.BaseDocument.FindRecord(key)?.GetRawValue(path, kit.Index) is null;
            result = workspace.SetValue(key, path, kit.Index, value, addIfMissing: wasAbsent);
        }

        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            RefreshKit(kit.Index);
            return;
        }

        IsDirty = set!.IsDirty;
        RefreshHistoryState();
        RefreshKit(kit.Index);
        StatusText = Strings.Format("EditorCellUpdated", KitNameField, key);
    }

    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(IsLanguageSelectorEnabled));

    partial void OnCanUndoChanged(bool value) => UndoCommand.NotifyCanExecuteChanged();

    partial void OnCanRedoChanged(bool value) => RedoCommand.NotifyCanExecuteChanged();

    partial void OnSearchTextChanged(string value) => ApplyMercFilter(SelectedMerc?.Key);

    partial void OnSelectedMercChanged(MercGearEntryViewModel? value)
    {
        if (_suppressSelection)
        {
            return;
        }

        LoadMercForm();
    }

    partial void OnSelectedLanguageOptionChanged(LanguageOptionViewModel? value)
    {
        if (_suppressLanguageChange || value is null || _isReloading)
        {
            return;
        }

        if (string.Equals(value.Code, _languageCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _ = ChangeLanguageAsync(value);
    }

    private async Task LoadFieldSourcesAsync(CancellationToken cancellationToken)
    {
        if (_workspace is null)
        {
            return;
        }

        _fieldSources = await FieldSourceSet
            .LoadAsync(_session, _workspace.Primary, cancellationToken)
            .ConfigureAwait(true);
    }

    private async Task ChangeLanguageAsync(LanguageOptionViewModel option)
    {
        if (IsDirty && !_dialogService.Confirm(
                Strings.Format("EditorConfirmLanguage", Title),
                Strings.Get("EditorLanguageTitle")))
        {
            RestoreLanguageSelection();
            return;
        }

        if (!option.HasOverlay && option.Language is not null)
        {
            if (!_dialogService.Confirm(
                    Strings.Format(
                        "EditorConfirmCreateOverlay",
                        option.Language.DisplayName,
                        _descriptor.TableName.Value),
                    Strings.Get("EditorLanguageTitle")))
            {
                RestoreLanguageSelection();
                return;
            }

            IsBusy = true;
            try
            {
                var ensured = await _session
                    .EnsureOverlayAsync(_descriptor, option.Language)
                    .ConfigureAwait(true);
                if (!ensured.IsSuccess)
                {
                    StatusText = Strings.Format(
                        "StatusError",
                        $"{ensured.ErrorCode}: {ensured.ErrorText}");
                    RestoreLanguageSelection();
                    return;
                }
            }
            catch (Exception exception)
            {
                StatusText = Strings.Format("StatusError", exception.Message);
                RestoreLanguageSelection();
                return;
            }
            finally
            {
                IsBusy = false;
            }
        }

        _languageCode = option.Code;
        await ReloadPreservingSelectionAsync().ConfigureAwait(true);
        LanguageChanged?.Invoke(this, _languageCode);
    }

    private void RestoreLanguageSelection()
    {
        _suppressLanguageChange = true;
        try
        {
            SelectedLanguageOption = FindLanguageOption(_languageCode);
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

    private async Task ReloadPreservingSelectionAsync()
    {
        if (_isReloading)
        {
            return;
        }

        _isReloading = true;
        try
        {
            var key = SelectedMerc?.Key;
            Kits.Clear();
            Mercs.Clear();
            _allMercs.Clear();
            await InitializeCoreAsync(CancellationToken.None).ConfigureAwait(true);
            if (key is not null)
            {
                SelectRecord(key);
            }
        }
        finally
        {
            _isReloading = false;
        }
    }

    private void RebuildMercList(string? selectKey)
    {
        if (_workspace is null)
        {
            return;
        }

        _allMercs.Clear();
        foreach (var row in _workspace.Primary.GetRows())
        {
            if (row.Key is null)
            {
                continue;
            }

            _allMercs.Add(new MercGearEntryViewModel(row.Key, row.GetValue("mName") ?? string.Empty));
        }

        ApplyMercFilter(selectKey);
    }

    private void ApplyMercFilter(string? selectKey)
    {
        var term = SearchText.Trim();
        var visible = term.Length == 0
            ? _allMercs
            : _allMercs.Where(entry =>
                entry.Key.Contains(term, StringComparison.OrdinalIgnoreCase)
                || entry.Name.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();

        var wanted = selectKey ?? SelectedMerc?.Key;
        _suppressSelection = true;
        try
        {
            Mercs.Clear();
            foreach (var entry in visible)
            {
                Mercs.Add(entry);
            }

            CountText = Strings.Format("MercGearCountFormat", Mercs.Count);
        }
        finally
        {
            _suppressSelection = false;
        }

        var target = wanted is null
            ? Mercs.FirstOrDefault()
            : Mercs.FirstOrDefault(entry => string.Equals(entry.Key, wanted, StringComparison.Ordinal))
                ?? Mercs.FirstOrDefault();
        var same = ReferenceEquals(SelectedMerc, target);
        SelectedMerc = target;
        if (same)
        {
            LoadMercForm();
        }

        if (term.Length > 0 && Mercs.Count == 0)
        {
            StatusText = Strings.Get("EditorNoMatches");
        }
    }

    private void LoadMercForm()
    {
        Kits.Clear();
        if (SelectedMerc is null || _workspace is null)
        {
            DetailHeader = Strings.Get("MercGearNoSelection");
            return;
        }

        DetailHeader = SelectedMerc.DisplayText;
        var record = _workspace.Primary.BaseDocument.FindRecord(SelectedMerc.Key);
        if (record is null)
        {
            DetailHeader = Strings.Get("MercGearNoSelection");
            return;
        }

        var row = _workspace.Primary.GetRows()
            .FirstOrDefault(candidate => string.Equals(candidate.Key, SelectedMerc.Key, StringComparison.Ordinal));
        var kitCount = record.GetValues(GearCollection).Count;
        for (var index = 0; index < kitCount; index++)
        {
            Kits.Add(BuildKit(record, row, index));
        }
    }

    private MercGearKitViewModel BuildKit(TableRecord record, WorkspaceRow? row, int index)
    {
        var state = ReadKitState(record, row, index);
        var slots = new List<MercGearSlotViewModel>(SlotDefinitions.Length);
        for (var slotIndex = 0; slotIndex < SlotDefinitions.Length; slotIndex++)
        {
            var definition = SlotDefinitions[slotIndex];
            slots.Add(new MercGearSlotViewModel(
                this,
                index,
                definition.ItemField,
                definition.StatusField,
                definition.DropField,
                definition.QuantityField,
                definition.ResolveLabel(),
                _fieldSources?.Relations.GetValueOrDefault(definition.ItemField),
                state.Slots[slotIndex]));
        }

        return new MercGearKitViewModel(this, index, state, slots);
    }

    private MercGearKitState ReadKitState(TableRecord record, WorkspaceRow? row, int index)
    {
        var namePath = $"{GearCollection}[{index}]/{KitNameField}";
        var nameCell = row?.Merged?.Cells
            .FirstOrDefault(cell => string.Equals(cell.FieldPath, namePath, StringComparison.Ordinal));
        var baseName = record.GetRawValue(FieldPath.Parse(GearCollection + "/" + KitNameField), index);
        var slots = new List<MercGearSlotState>(SlotDefinitions.Length);
        foreach (var definition in SlotDefinitions)
        {
            var item = GetKitValue(record, definition.ItemField, index);
            var status = GetKitValue(record, definition.StatusField, index);
            var quantity = definition.QuantityField is null
                ? string.Empty
                : GetKitValue(record, definition.QuantityField, index);
            var drop = definition.DropField is not null
                && string.Equals(GetKitValue(record, definition.DropField, index), "1", StringComparison.Ordinal);
            slots.Add(new MercGearSlotState(item, status, quantity, drop));
        }

        return new MercGearKitState(
            nameCell?.Value ?? baseName ?? string.Empty,
            nameCell?.OverlayValue is not null,
            GetKitValue(record, "mPriceMod", index),
            GetKitValue(record, "mAbsolutePrice", index),
            slots);
    }

    private static string GetKitValue(TableRecord record, string field, int index) =>
        record.GetRawValue(FieldPath.Parse(GearCollection + "/" + field), index)?.Trim() ?? string.Empty;

    private void RefreshKit(int kitIndex)
    {
        if (_workspace is null || SelectedMerc?.Key is not { } key)
        {
            return;
        }

        var record = _workspace.Primary.BaseDocument.FindRecord(key);
        var kit = Kits.FirstOrDefault(candidate => candidate.Index == kitIndex);
        if (record is null || kit is null)
        {
            return;
        }

        var row = _workspace.Primary.GetRows()
            .FirstOrDefault(candidate => string.Equals(candidate.Key, key, StringComparison.Ordinal));
        kit.Refresh(ReadKitState(record, row, kitIndex));
    }

    private void RefreshAllKits()
    {
        foreach (var kit in Kits)
        {
            RefreshKit(kit.Index);
        }
    }

    private void ApplyHistory(bool undo)
    {
        if (_workspace is null)
        {
            return;
        }

        try
        {
            var result = undo ? _workspace.Undo() : _workspace.Redo();
            if (!result.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
                RefreshHistoryState();
                return;
            }

            IsDirty = _workspace.IsDirty;
            RefreshAllKits();
            RefreshHistoryState();
            StatusText = Strings.Get(undo ? "EditorUndone" : "EditorRedone");
        }
        catch (InvalidOperationException exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
            RefreshHistoryState();
        }
    }

    private void OnHistoryChanged(object? sender, EventArgs e)
    {
        if (_workspace is null)
        {
            return;
        }

        IsDirty = _workspace.IsDirty;
        RefreshHistoryState();
        RefreshAllKits();
    }

    private void RefreshHistoryState()
    {
        CanUndo = _workspace?.CanUndo == true;
        CanRedo = _workspace?.CanRedo == true;
    }

    private void BuildLanguageOptions()
    {
        _suppressLanguageChange = true;
        try
        {
            LanguageOptions.Clear();
            LanguageOptions.Add(new LanguageOptionViewModel(null, false));
            var overlays = _workspace?.Primary.Descriptor.Overlays ?? Array.Empty<OverlayDescriptor>();
            var canCreateOverlay = _workspace?.Primary.Metadata is { TranslatableFieldPaths.Count: > 0 };
            foreach (var language in _languages
                         .Where(language =>
                             canCreateOverlay
                             || overlays.Any(overlay => string.Equals(
                                 overlay.Language.Code,
                                 language.Code,
                                 StringComparison.OrdinalIgnoreCase)))
                         .OrderBy(static language => language.Ordinal)
                         .ThenBy(static language => language.DisplayName, StringComparer.Ordinal))
            {
                var hasOverlay = overlays.Any(overlay => string.Equals(
                    overlay.Language.Code,
                    language.Code,
                    StringComparison.OrdinalIgnoreCase));
                LanguageOptions.Add(new LanguageOptionViewModel(language, hasOverlay));
            }

            SelectedLanguageOption = FindLanguageOption(_languageCode)
                ?? LanguageOptions.FirstOrDefault();
        }
        finally
        {
            _suppressLanguageChange = false;
        }
    }

    private static SlotDefinition[] BuildSlotDefinitions()
    {
        var definitions = new List<SlotDefinition>
        {
            new("mHelmet", "mHelmetStatus", "mHelmetDrop", null, "MercGearSlot.Helmet", null),
            new("mVest", "mVestStatus", "mVestDrop", null, "MercGearSlot.Vest", null),
            new("mLeg", "mLegStatus", "mLegDrop", null, "MercGearSlot.Leg", null),
            new("mWeapon", "mWeaponStatus", "mWeaponDrop", null, "MercGearSlot.Weapon", null),
            new("lVest", "lVestStatus", null, null, "MercGearSlot.LbeVest", null),
            new("lLeftThigh", "lLeftThighStatus", null, null, "MercGearSlot.LeftThigh", null),
            new("lRightThigh", "lRightThighStatus", null, null, "MercGearSlot.RightThigh", null),
            new("lCPack", "lCPackStatus", null, null, "MercGearSlot.LbePack", null),
            new("lBPack", "lBPackStatus", null, null, "MercGearSlot.Backpack", null),
        };

        for (var index = 0; index < 4; index++)
        {
            definitions.Add(new SlotDefinition(
                $"mBig{index}",
                $"mBig{index}Status",
                $"mBig{index}Drop",
                $"mBig{index}Quantity",
                "MercGearSlot.BigFormat",
                index + 1));
        }

        for (var index = 0; index < 8; index++)
        {
            definitions.Add(new SlotDefinition(
                $"mSmall{index}",
                $"mSmall{index}Status",
                null,
                $"mSmall{index}Quantity",
                "MercGearSlot.SmallFormat",
                index + 1));
        }

        return definitions.ToArray();
    }

    /// <summary>
    /// Layout of one kit slot: which XML fields carry the item, status, drop flag, and quantity.
    /// </summary>
    /// <param name="ItemField">Field that stores the item identifier.</param>
    /// <param name="StatusField">Field that stores the status.</param>
    /// <param name="DropField">Field that stores the drop flag, or <see langword="null"/>.</param>
    /// <param name="QuantityField">Field that stores the quantity, or <see langword="null"/>.</param>
    /// <param name="LabelKey">Resource key of the slot label.</param>
    /// <param name="FormatArgument">Argument of a formatted label, or <see langword="null"/>.</param>
    private sealed record SlotDefinition(
        string ItemField,
        string StatusField,
        string? DropField,
        string? QuantityField,
        string LabelKey,
        int? FormatArgument)
    {
        /// <summary>
        /// Resolves the localized label of the slot.
        /// </summary>
        /// <returns>The localized label.</returns>
        public string ResolveLabel() => FormatArgument is { } argument
            ? Strings.Format(LabelKey, argument)
            : Strings.Get(LabelKey);
    }
}
