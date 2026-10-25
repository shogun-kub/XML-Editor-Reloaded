using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Layout;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// View model for the main shell: data-directory selection, table list, editor tabs across several
/// data sets, and load diagnostics.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private static readonly TableName ItemsTableName = TableName.Parse("Items.Items");
    private static readonly TableName MercGearTableName = TableName.Parse("Inventory.MercStartingGear");

    private readonly IDataSetSessionFactory _sessionFactory;
    private readonly ITableEditorFactory _editorFactory;
    private readonly ISettingsStore _settingsStore;
    private readonly IDialogService _dialogService;
    private readonly IImageViewerService _imageViewer;
    private readonly IVersionCompareService _versionCompare;
    private readonly ITableCopyDialogService _copyDialog;
    private readonly ISettingsDialogService _settingsDialog;
    private readonly ITableLayoutStore _layoutStore;
    private AppSettings _settings = AppSettings.Default;
    private IDataSetSession? _dataSet;
    private CancellationTokenSource? _operationCts;
    private int _operationGeneration;
    private bool _suppressDataDirectorySelection;

    [ObservableProperty]
    private string _dataDirectory = string.Empty;

    [ObservableProperty]
    private DataDirectoryItem? _selectedDataDirectory;

    [ObservableProperty]
    private TableListItem? _selectedTable;

    [ObservableProperty]
    private string _statusText;

    [ObservableProperty]
    private string _detailsText;

    [ObservableProperty]
    private string _recordCountText = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private IEditorTab? _selectedEditor;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="sessionFactory">Factory for runtime data-set sessions.</param>
    /// <param name="editorFactory">Factory for table editor view models.</param>
    /// <param name="settingsStore">Settings store.</param>
    /// <param name="dialogService">Dialog service.</param>
    /// <param name="imageViewer">Item image viewer service.</param>
    /// <param name="versionCompare">Cross-version comparison window service.</param>
    /// <param name="copyDialog">Cross-data-set copy dialog service.</param>
    /// <param name="settingsDialog">Application settings dialog service.</param>
    /// <param name="layoutStore">Stored grid layouts, reset from the settings dialog.</param>
    public MainViewModel(
        IDataSetSessionFactory sessionFactory,
        ITableEditorFactory editorFactory,
        ISettingsStore settingsStore,
        IDialogService dialogService,
        IImageViewerService imageViewer,
        IVersionCompareService versionCompare,
        ITableCopyDialogService copyDialog,
        ISettingsDialogService settingsDialog,
        ITableLayoutStore layoutStore)
    {
        ArgumentNullException.ThrowIfNull(sessionFactory);
        ArgumentNullException.ThrowIfNull(editorFactory);
        ArgumentNullException.ThrowIfNull(settingsStore);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(imageViewer);
        ArgumentNullException.ThrowIfNull(versionCompare);
        ArgumentNullException.ThrowIfNull(copyDialog);
        ArgumentNullException.ThrowIfNull(settingsDialog);
        ArgumentNullException.ThrowIfNull(layoutStore);

        _sessionFactory = sessionFactory;
        _editorFactory = editorFactory;
        _settingsStore = settingsStore;
        _dialogService = dialogService;
        _imageViewer = imageViewer;
        _versionCompare = versionCompare;
        _copyDialog = copyDialog;
        _settingsDialog = settingsDialog;
        _layoutStore = layoutStore;

        Editors.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasEditors));

        Title = Strings.Get("AppTitle");
        BrowseText = Strings.Get("BrowseText");
        RefreshText = Strings.Get("RefreshText");
        ImagesText = Strings.Get("ImageViewerButton");
        EnemyAmmoText = Strings.Get("EnemyAmmoButton");
        CompareText = Strings.Get("CompareButton");
        CopyText = Strings.Get("CopyButton");
        SettingsText = Strings.Get("SettingsButton");
        OpenTableText = Strings.Get("EditorOpenText");
        SaveAllText = Strings.Get("EditorSaveAllText");
        EditorPlaceholder = Strings.Get("EditorPlaceholder");
        DataDirectoryLabel = Strings.Get("DataDirectoryLabel");
        TablesLabel = Strings.Get("TablesLabel");
        DetailsLabel = Strings.Get("DetailsLabel");
        DiagnosticsLabel = Strings.Get("DiagnosticsLabel");
        TableColumn = Strings.Get("TableColumn");
        LanguagesColumn = Strings.Get("LanguagesColumn");
        MappedColumn = Strings.Get("MappedColumn");
        _statusText = Strings.Get("StatusReady");
        _detailsText = Strings.Get("StatusNoSelection");
    }

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the browse button text.
    /// </summary>
    public string BrowseText { get; }

    /// <summary>
    /// Gets the refresh button text.
    /// </summary>
    public string RefreshText { get; }

    /// <summary>
    /// Gets the item image viewer button text.
    /// </summary>
    public string ImagesText { get; }

    /// <summary>
    /// Gets the enemy ammunition editor button text.
    /// </summary>
    public string EnemyAmmoText { get; }

    /// <summary>
    /// Gets the cross-version comparison button text.
    /// </summary>
    public string CompareText { get; }

    /// <summary>
    /// Gets the cross-data-set copy button text.
    /// </summary>
    public string CopyText { get; }

    /// <summary>
    /// Gets the settings button text.
    /// </summary>
    public string SettingsText { get; }

    /// <summary>
    /// Gets the open-table button text.
    /// </summary>
    public string OpenTableText { get; }

    /// <summary>
    /// Gets the save-all button text.
    /// </summary>
    public string SaveAllText { get; }

    /// <summary>
    /// Gets the placeholder shown when no table is open.
    /// </summary>
    public string EditorPlaceholder { get; }

    /// <summary>
    /// Gets the open editor tabs across every data set.
    /// </summary>
    public ObservableCollection<IEditorTab> Editors { get; } = new();

    /// <summary>
    /// Gets a value indicating whether at least one table editor is open.
    /// </summary>
    public bool HasEditors => Editors.Count > 0;

    /// <summary>
    /// Gets the data directories offered by the selector, in most-recent-first order.
    /// </summary>
    public ObservableCollection<DataDirectoryItem> DataDirectories { get; } = new();

    /// <summary>
    /// Gets the data directory label.
    /// </summary>
    public string DataDirectoryLabel { get; }

    /// <summary>
    /// Gets the table list label.
    /// </summary>
    public string TablesLabel { get; }

    /// <summary>
    /// Gets the details label.
    /// </summary>
    public string DetailsLabel { get; }

    /// <summary>
    /// Gets the diagnostics label.
    /// </summary>
    public string DiagnosticsLabel { get; }

    /// <summary>
    /// Gets the table column header.
    /// </summary>
    public string TableColumn { get; }

    /// <summary>
    /// Gets the languages column header.
    /// </summary>
    public string LanguagesColumn { get; }

    /// <summary>
    /// Gets the metadata column header.
    /// </summary>
    public string MappedColumn { get; }

    /// <summary>
    /// Gets the discovered tables of the active data set.
    /// </summary>
    public ObservableCollection<TableListItem> Tables { get; } = new();

    /// <summary>
    /// Gets diagnostics for the selected table.
    /// </summary>
    public ObservableCollection<string> Diagnostics { get; } = new();

    /// <summary>
    /// Loads settings, the configured data directories, and the last active data set.
    /// </summary>
    /// <returns>A task that completes when initialization finishes.</returns>
    public async Task InitializeAsync()
    {
        var load = await _settingsStore.LoadAsync().ConfigureAwait(true);
        if (!load.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", load.ErrorText ?? string.Empty);
            return;
        }

        _settings = load.Data!;
        DataDirectory = _settings.ActiveDataDirectory
            ?? _settings.DataDirectories.FirstOrDefault()
            ?? string.Empty;
        RebuildDataDirectories();

        if (string.IsNullOrWhiteSpace(DataDirectory))
        {
            StatusText = Strings.Get("StatusNoDataDirectory");
            return;
        }

        await RefreshAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private async Task BrowseAsync()
    {
        var folder = _dialogService.PickFolder(
            Strings.Get("ChooseDataDirectory"),
            string.IsNullOrWhiteSpace(DataDirectory) ? null : DataDirectory);
        if (string.IsNullOrWhiteSpace(folder))
        {
            return;
        }

        EnsureDataDirectoryItem(folder);
        DataDirectory = folder;
        await ActivateDataDirectoryAsync(folder).ConfigureAwait(true);
    }

    [RelayCommand]
    private void OpenImageViewer() => _imageViewer.Show(DataDirectory);

    [RelayCommand]
    private async Task OpenEnemyAmmoAsync()
    {
        if (_dataSet is null)
        {
            return;
        }

        IsBusy = true;
        try
        {
            var viewModel = new EnemyAmmoViewModel(_dataSet);
            await viewModel.InitializeAsync().ConfigureAwait(true);
            _dialogService.ShowEnemyAmmo(viewModel);
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

    [RelayCommand]
    private void CompareVersions() => _versionCompare.Show(DataDirectory);

    [RelayCommand]
    private async Task CopyDataAsync()
    {
        if (_dataSet is null)
        {
            StatusText = Strings.Get("InvalidDataDirectory");
            return;
        }

        var sourceDirectory = SelectedEditor?.TableDataDirectory ?? DataDirectory;
        var initialTable = SelectedEditor?.TableName.Value ?? SelectedTable?.TableName;
        var initialKeys = SelectedEditor?.SelectedRecordKey is { Length: > 0 } key
            ? new[] { key }
            : null;
        var directories = DataDirectories
            .Select(static item => item.Path)
            .ToList();
        if (!directories.Any(path => SamePath(path, sourceDirectory)))
        {
            directories.Insert(0, sourceDirectory);
        }

        var outcome = await _copyDialog
            .ShowAsync(new TableCopyDialogRequest(
                directories,
                sourceDirectory,
                initialTable,
                initialKeys))
            .ConfigureAwait(true);
        if (outcome is null)
        {
            return;
        }

        var added = outcome.Result.Tables.Sum(static table => table.Added);
        var updated = outcome.Result.Tables.Sum(static table => table.Updated);
        var skipped = outcome.Result.Tables.Sum(static table => table.Skipped);
        StatusText = Strings.Format("CopyResultSummary", added, updated, skipped);

        var notReloaded = 0;
        foreach (var editor in Editors
                     .Where(editor => SamePath(editor.TableDataDirectory, outcome.TargetTableDataDirectory))
                     .ToArray())
        {
            if (editor.IsDirty)
            {
                notReloaded++;
                continue;
            }

            await editor.ReloadAsync().ConfigureAwait(true);
        }

        if (notReloaded > 0)
        {
            StatusText += " " + Strings.Format("CopyTabsNotReloaded", notReloaded);
        }
    }

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        var request = new SettingsRequest(
            _settings.DataDirectories,
            _settings.ActiveDataDirectory ?? DataDirectory,
            _settings.UiLanguageCode,
            _settings.RecentDataDirectories);
        var result = _settingsDialog.Show(request);
        if (result is null)
        {
            return;
        }

        await ApplySettingsAsync(result).ConfigureAwait(true);
    }

    private async Task ApplySettingsAsync(SettingsResult result)
    {
        string? resetError = null;
        if (result.ResetLayouts)
        {
            foreach (var editor in Editors.OfType<TableEditorViewModel>())
            {
                await editor.ResetLayoutAsync().ConfigureAwait(true);
            }

            var reset = await _layoutStore.ResetAllAsync().ConfigureAwait(true);
            if (!reset.IsSuccess)
            {
                resetError = $"{reset.ErrorCode}: {reset.ErrorText}";
            }
        }

        _settings = _settings with
        {
            DataDirectories = result.DataDirectories.ToArray(),
            ActiveDataDirectory = result.ActiveDataDirectory,
            UiLanguageCode = result.UiLanguageCode,
            RecentDataDirectories = _settings.RecentDataDirectories
                .Where(recent => result.DataDirectories.Any(directory => SamePath(directory, recent)))
                .ToArray(),
        };

        var save = await _settingsStore.SaveAsync(_settings).ConfigureAwait(true);
        if (!save.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", save.ErrorText ?? string.Empty);
            return;
        }

        RebuildDataDirectories();
        if (!string.IsNullOrWhiteSpace(result.ActiveDataDirectory))
        {
            await ActivateDataDirectoryAsync(result.ActiveDataDirectory).ConfigureAwait(true);
        }

        if (resetError is not null)
        {
            StatusText = Strings.Format("StatusError", resetError);
        }
        else if (result.ActiveDataDirectory is null
                 || SamePath(DataDirectory, result.ActiveDataDirectory))
        {
            StatusText = Strings.Get("SettingsApplied");
        }

        // Otherwise activation already reported why the requested directory could not be opened.
    }

    [RelayCommand]
    private async Task OpenTableAsync()
    {
        if (_dataSet is null)
        {
            StatusText = Strings.Get("InvalidDataDirectory");
            return;
        }

        if (SelectedTable is null)
        {
            StatusText = Strings.Get("StatusNoSelection");
            return;
        }

        await OpenEditorByNameAsync(SelectedTable.Descriptor.TableName).ConfigureAwait(true);
    }

    private async Task<IEditorTab?> OpenEditorByNameAsync(
        TableName tableName,
        IDataSetSession? owner = null)
    {
        var session = owner ?? _dataSet;
        if (session is null)
        {
            return null;
        }

        var existing = Editors.FirstOrDefault(editor =>
            editor.TableName == tableName
            && SamePath(editor.TableDataDirectory, session.TableDataDirectory));
        if (existing is not null)
        {
            SelectedEditor = existing;
            return existing;
        }

        var tables = await session.Catalog
            .GetTablesAsync(session.TableDataDirectory)
            .ConfigureAwait(true);
        if (!tables.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", tables.ErrorText ?? string.Empty);
            return null;
        }

        var descriptor = tables.Data!.FirstOrDefault(candidate => candidate.TableName == tableName);
        if (descriptor is null)
        {
            StatusText = Strings.Format("EditorTableNotFound", tableName.Value);
            return null;
        }

        var language = ResolveLanguageCode(descriptor);
        IEditorTab editor = descriptor.IsMapped && tableName == ItemsTableName
            ? _editorFactory.CreateItemWorkspace(session, descriptor, language)
            : descriptor.IsMapped && tableName == MercGearTableName
                ? _editorFactory.CreateMercGearWorkspace(session, descriptor, language)
                : _editorFactory.Create(session, descriptor, language);
        AttachEditor(editor);
        Editors.Add(editor);
        SelectedEditor = editor;
        await editor.InitializeAsync().ConfigureAwait(true);
        return editor;
    }

    [RelayCommand]
    private async Task CloseEditorAsync(IEditorTab? editor)
    {
        if (editor is null)
        {
            return;
        }

        if (!await ConfirmCloseEditorAsync(editor).ConfigureAwait(true))
        {
            return;
        }

        var wasSelected = ReferenceEquals(SelectedEditor, editor);
        DetachEditor(editor);
        Editors.Remove(editor);
        if (editor is TableEditorViewModel tableEditor)
        {
            await tableEditor.FlushLayoutAsync().ConfigureAwait(true);
        }

        if (wasSelected)
        {
            SelectedEditor = Editors.FirstOrDefault();
        }
    }

    /// <summary>
    /// Closes every tab after resolving unsaved changes and flushes pending grid layouts. The window uses
    /// this before it closes; a <see langword="false"/> result means the close must be cancelled.
    /// </summary>
    /// <returns><see langword="true"/> when the application may close; otherwise <see langword="false"/>.</returns>
    public async Task<bool> RequestCloseAsync()
    {
        foreach (var editor in Editors.ToArray())
        {
            if (!await ConfirmCloseEditorAsync(editor).ConfigureAwait(true))
            {
                return false;
            }
        }

        await FlushLayoutsAsync().ConfigureAwait(true);
        return true;
    }

    /// <summary>
    /// Writes pending grid layouts immediately, for example before the application exits.
    /// </summary>
    /// <returns>A task that completes when every layout is stored.</returns>
    public async Task FlushLayoutsAsync()
    {
        foreach (var editor in Editors.OfType<TableEditorViewModel>())
        {
            await editor.FlushLayoutAsync().ConfigureAwait(true);
        }
    }

    [RelayCommand]
    private async Task SaveAllAsync()
    {
        var dirty = Editors.Where(editor => editor.IsDirty).ToList();
        if (dirty.Count == 0)
        {
            StatusText = Strings.Get("EditorNoChanges");
            return;
        }

        var saved = 0;
        var failed = 0;
        foreach (var editor in dirty)
        {
            var clean = await editor.SaveWithRecoveryAsync().ConfigureAwait(true);
            if (!clean || editor.IsDirty)
            {
                failed++;
            }
            else
            {
                saved++;
            }
        }

        StatusText = failed == 0
            ? Strings.Format("EditorSaveAllSummary", saved)
            : Strings.Get("EditorSaveAllErrors");
    }

    private async Task<bool> ConfirmCloseEditorAsync(IEditorTab editor)
    {
        if (!editor.IsDirty)
        {
            return true;
        }

        var choice = _dialogService.ConfirmUnsavedChanges(
            Strings.Format("UnsavedChangesMessageFormat", editor.Title),
            Strings.Get("UnsavedChangesTitle"));
        return choice switch
        {
            UnsavedChangesChoice.Save => await editor.SaveWithRecoveryAsync().ConfigureAwait(true),
            UnsavedChangesChoice.Discard => true,
            _ => false,
        };
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        var (source, generation) = BeginOperation();
        StatusText = Strings.Get("StatusLoading");
        try
        {
            var opened = await _sessionFactory.OpenAsync(DataDirectory, source.Token).ConfigureAwait(true);
            if (!opened.IsSuccess)
            {
                if (generation == _operationGeneration)
                {
                    _dataSet = null;
                    Tables.Clear();
                    SelectedTable = null;
                    Diagnostics.Clear();
                    RecordCountText = string.Empty;
                    DetailsText = Strings.Get("StatusNoSelection");
                    StatusText = Strings.Get("InvalidDataDirectory");
                }

                return;
            }

            var session = opened.Data!;
            if (!await ShowTablesAsync(session, source.Token).ConfigureAwait(true))
            {
                return;
            }

            _dataSet = session;
            if (generation == _operationGeneration)
            {
                StatusText = Strings.Format("StatusTablesLoaded", Tables.Count);
            }
        }
        catch (OperationCanceledException)
        {
            // A newer refresh or data-set switch replaced this operation.
        }
        catch (Exception exception)
        {
            if (generation == _operationGeneration)
            {
                StatusText = Strings.Format("StatusError", exception.Message);
            }
        }
        finally
        {
            CompleteOperation(generation);
        }
    }

    private async Task ActivateDataDirectoryAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        if (_dataSet is not null
            && SamePath(
                DataSetPaths.ResolveTableDataDirectory(path) ?? path,
                _dataSet.TableDataDirectory))
        {
            DataDirectory = path;
            SelectDataDirectorySilently(EnsureDataDirectoryItem(path));
            await PersistActiveDataDirectoryAsync(path).ConfigureAwait(true);
            return;
        }

        var previousDirectory = DataDirectory;
        var previousItem = SelectedDataDirectory;
        var (source, generation) = BeginOperation();
        StatusText = Strings.Get("StatusLoading");
        try
        {
            var opened = await _sessionFactory.OpenAsync(path, source.Token).ConfigureAwait(true);
            if (!opened.IsSuccess)
            {
                if (generation == _operationGeneration)
                {
                    DataDirectory = previousDirectory;
                    SelectDataDirectorySilently(previousItem);
                    StatusText = Strings.Get("InvalidDataDirectory");
                }

                return;
            }

            var session = opened.Data!;
            if (!await ShowTablesAsync(session, source.Token).ConfigureAwait(true))
            {
                if (generation == _operationGeneration)
                {
                    DataDirectory = previousDirectory;
                    SelectDataDirectorySilently(previousItem);
                }

                return;
            }

            _dataSet = session;
            DataDirectory = path;
            SelectDataDirectorySilently(EnsureDataDirectoryItem(path));
            await PersistActiveDataDirectoryAsync(path).ConfigureAwait(true);
            if (generation == _operationGeneration)
            {
                StatusText = Strings.Format("StatusTablesLoaded", Tables.Count);
            }
        }
        catch (OperationCanceledException)
        {
            // A newer switch replaced this activation.
        }
        catch (Exception exception)
        {
            if (generation == _operationGeneration)
            {
                DataDirectory = previousDirectory;
                SelectDataDirectorySilently(previousItem);
                StatusText = Strings.Format("StatusError", exception.Message);
            }
        }
        finally
        {
            CompleteOperation(generation);
        }
    }

    private async Task<bool> ShowTablesAsync(
        IDataSetSession session,
        CancellationToken cancellationToken)
    {
        var result = await session.Catalog
            .GetTablesAsync(session.TableDataDirectory, cancellationToken)
            .ConfigureAwait(true);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", result.ErrorText ?? string.Empty);
            return false;
        }

        Tables.Clear();
        SelectedTable = null;
        Diagnostics.Clear();
        DetailsText = Strings.Get("StatusNoSelection");
        RecordCountText = string.Empty;
        foreach (var descriptor in result.Data!)
        {
            Tables.Add(new TableListItem(descriptor));
        }

        return true;
    }

    partial void OnSelectedDataDirectoryChanged(DataDirectoryItem? value)
    {
        if (_suppressDataDirectorySelection || value is null)
        {
            return;
        }

        if (SamePath(value.Path, DataDirectory))
        {
            return;
        }

        DataDirectory = value.Path;
        _ = ActivateDataDirectoryAsync(value.Path);
    }

    partial void OnSelectedTableChanged(TableListItem? value)
    {
        if (value is null)
        {
            DetailsText = Strings.Get("StatusNoSelection");
            RecordCountText = string.Empty;
            Diagnostics.Clear();
            return;
        }

        _ = LoadSelectedTableAsync(value);
    }

    private async Task LoadSelectedTableAsync(TableListItem item)
    {
        if (_dataSet is null)
        {
            return;
        }

        var (source, generation) = BeginOperation();
        Diagnostics.Clear();
        RecordCountText = string.Empty;
        DetailsText = item.TableName;
        try
        {
            var result = await _dataSet.Repository
                .LoadTableAsync(item.Descriptor, source.Token)
                .ConfigureAwait(true);
            if (!result.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", result.ErrorText ?? string.Empty);
                return;
            }

            var load = result.Data!;
            DetailsText = load.Status switch
            {
                TableDocumentStatus.Loaded => Strings.Get("StatusLoaded"),
                TableDocumentStatus.LoadedReadOnly => Strings.Get("StatusLoadedReadOnly"),
                _ => Strings.Get("StatusMalformed"),
            };

            foreach (var diagnostic in load.Diagnostics)
            {
                Diagnostics.Add($"{diagnostic.Code}: {diagnostic.Message}");
            }

            var document = load.GetDocument<TableDocument>();
            if (document is not null)
            {
                RecordCountText = Strings.Format("RecordCount", document.Records.Count);
            }

            StatusText = Strings.Format("TableSelected", item.TableName);
        }
        catch (OperationCanceledException)
        {
            // A newer selection replaced this load.
        }
        catch (Exception exception)
        {
            if (generation == _operationGeneration)
            {
                StatusText = Strings.Format("StatusError", exception.Message);
            }
        }
        finally
        {
            CompleteOperation(generation);
        }
    }

    private string? ResolveLanguageCode(TableDescriptor descriptor)
    {
        var code = _settings.ActiveLanguageCode;
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return descriptor.Overlays.Any(overlay =>
            string.Equals(overlay.Language.Code, code, StringComparison.OrdinalIgnoreCase))
            ? code
            : null;
    }

    private void AttachEditor(IEditorTab editor)
    {
        editor.LanguageChanged += OnEditorLanguageChanged;
        editor.TableNavigationRequested += OnTableNavigationRequested;
    }

    private void DetachEditor(IEditorTab editor)
    {
        editor.LanguageChanged -= OnEditorLanguageChanged;
        editor.TableNavigationRequested -= OnTableNavigationRequested;
    }

    private async void OnTableNavigationRequested(object? sender, TableNavigationRequest request)
    {
        try
        {
            var opened = await _sessionFactory
                .OpenAsync(request.TableDataDirectory)
                .ConfigureAwait(true);
            if (!opened.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", opened.ErrorText ?? string.Empty);
                return;
            }

            var editor = await OpenEditorByNameAsync(request.TableName, opened.Data!).ConfigureAwait(true);
            editor?.SelectRecord(request.RecordKey);
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
        }
    }

    private void OnEditorLanguageChanged(object? sender, string? languageCode)
    {
        _settings = _settings with { ActiveLanguageCode = languageCode };
        _ = PersistLanguageAsync();
    }

    private async Task PersistLanguageAsync()
    {
        try
        {
            var save = await _settingsStore.SaveAsync(_settings).ConfigureAwait(true);
            if (!save.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", save.ErrorText ?? string.Empty);
            }
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
        }
    }

    private (CancellationTokenSource Source, int Generation) BeginOperation()
    {
        CancelOperation();
        var source = new CancellationTokenSource();
        _operationCts = source;
        var generation = ++_operationGeneration;
        IsBusy = true;
        return (source, generation);
    }

    private void CompleteOperation(int generation)
    {
        if (generation == _operationGeneration)
        {
            IsBusy = false;
        }
    }

    private void RebuildDataDirectories()
    {
        var paths = new List<string>();
        void AddPath(string? path)
        {
            if (!string.IsNullOrWhiteSpace(path)
                && !paths.Any(existing => SamePath(existing, path)))
            {
                paths.Add(path);
            }
        }

        AddPath(DataDirectory);
        AddPath(_settings.ActiveDataDirectory);
        foreach (var recent in _settings.RecentDataDirectories)
        {
            AddPath(recent);
        }

        foreach (var configured in _settings.DataDirectories)
        {
            AddPath(configured);
        }

        _suppressDataDirectorySelection = true;
        try
        {
            DataDirectories.Clear();
            foreach (var path in paths)
            {
                DataDirectories.Add(new DataDirectoryItem(path));
            }

            SelectedDataDirectory = DataDirectories.FirstOrDefault(item =>
                SamePath(item.Path, DataDirectory));
        }
        finally
        {
            _suppressDataDirectorySelection = false;
        }
    }

    private DataDirectoryItem EnsureDataDirectoryItem(string path)
    {
        var existing = DataDirectories.FirstOrDefault(item => SamePath(item.Path, path));
        if (existing is not null)
        {
            return existing;
        }

        var item = new DataDirectoryItem(path);
        DataDirectories.Add(item);
        return item;
    }

    private void SelectDataDirectorySilently(DataDirectoryItem? item)
    {
        _suppressDataDirectorySelection = true;
        try
        {
            SelectedDataDirectory = item;
        }
        finally
        {
            _suppressDataDirectorySelection = false;
        }
    }

    private async Task PersistActiveDataDirectoryAsync(string path)
    {
        var directories = _settings.DataDirectories.ToList();
        if (!directories.Any(existing => SamePath(existing, path)))
        {
            directories.Add(path);
        }

        var recent = new List<string> { path };
        recent.AddRange(_settings.RecentDataDirectories.Where(existing =>
            !SamePath(existing, path)));
        _settings = _settings with
        {
            DataDirectories = directories,
            ActiveDataDirectory = path,
            RecentDataDirectories = recent.Take(10).ToArray(),
        };

        SelectDataDirectorySilently(EnsureDataDirectoryItem(path));
        var save = await _settingsStore.SaveAsync(_settings).ConfigureAwait(true);
        if (!save.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", save.ErrorText ?? string.Empty);
        }
    }

    private static bool SamePath(string? first, string? second)
    {
        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second))
        {
            return false;
        }

        return string.Equals(
            Path.TrimEndingDirectorySeparator(first),
            Path.TrimEndingDirectorySeparator(second),
            StringComparison.OrdinalIgnoreCase);
    }

    private void CancelOperation()
    {
        _operationCts?.Cancel();
        _operationCts?.Dispose();
        _operationCts = null;
    }
}
