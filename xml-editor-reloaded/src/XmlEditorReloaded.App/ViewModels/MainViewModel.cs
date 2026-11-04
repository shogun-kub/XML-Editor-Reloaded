using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.Themes;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Diagnostics;
using XmlEditorReloaded.Core.Layout;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Search;
using XmlEditorReloaded.Xml.Validation;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// View model for the main shell: data-directory selection, table list, editor tabs across several
/// data sets, and load diagnostics.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private static readonly TableName ItemsTableName = TableName.Parse("Items.Items");
    private static readonly TableName MercGearTableName = TableName.Parse("Inventory.MercStartingGear");
    private static readonly TableName CitiesTableName = TableName.Parse("Map.Cities");
    private static readonly TableName FacilityTypesTableName = TableName.Parse("Map.FacilityTypes");

    private readonly IDataSetSessionFactory _sessionFactory;
    private readonly ITableEditorFactory _editorFactory;
    private readonly ISettingsStore _settingsStore;
    private readonly IDialogService _dialogService;
    private readonly IImageViewerService _imageViewer;
    private readonly IVersionCompareService _versionCompare;
    private readonly ITableCopyDialogService _copyDialog;
    private readonly ISettingsDialogService _settingsDialog;
    private readonly ITableLayoutStore _layoutStore;
    private readonly DatasetValidator _datasetValidator;
    private readonly DatasetSearchService _datasetSearch;
    private AppSettings _settings = AppSettings.Default;
    private bool _settingsLoaded;
    private IDataSetSession? _dataSet;
    private CancellationTokenSource? _operationCts;
    private int _operationGeneration;
    private bool _suppressDataDirectorySelection;

    [ObservableProperty]
    private string _dataDirectory = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveDataDirectoryCommand))]
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
    [NotifyCanExecuteChangedFor(nameof(RemoveDataDirectoryCommand))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BulkEditCommand))]
    [NotifyCanExecuteChangedFor(nameof(ResetLayoutCommand))]
    [NotifyCanExecuteChangedFor(nameof(ShowTranslationReportCommand))]
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
    /// <param name="datasetValidator">Validator used by the data-set window.</param>
    /// <param name="datasetSearch">Search used by the data-set window.</param>
    public MainViewModel(
        IDataSetSessionFactory sessionFactory,
        ITableEditorFactory editorFactory,
        ISettingsStore settingsStore,
        IDialogService dialogService,
        IImageViewerService imageViewer,
        IVersionCompareService versionCompare,
        ITableCopyDialogService copyDialog,
        ISettingsDialogService settingsDialog,
        ITableLayoutStore layoutStore,
        DatasetValidator datasetValidator,
        DatasetSearchService datasetSearch)
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
        ArgumentNullException.ThrowIfNull(datasetValidator);
        ArgumentNullException.ThrowIfNull(datasetSearch);

        _sessionFactory = sessionFactory;
        _editorFactory = editorFactory;
        _settingsStore = settingsStore;
        _dialogService = dialogService;
        _imageViewer = imageViewer;
        _versionCompare = versionCompare;
        _copyDialog = copyDialog;
        _settingsDialog = settingsDialog;
        _layoutStore = layoutStore;
        _datasetValidator = datasetValidator;
        _datasetSearch = datasetSearch;

        Editors.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasEditors));

        Title = Strings.Get("AppTitle");
        BrowseText = Strings.Get("BrowseText");
        RefreshText = Strings.Get("RefreshText");
        RemoveDataDirectoryText = Strings.Get("RemoveDataDirectoryText");
        RemoveDataDirectoryTooltip = Strings.Get("RemoveDataDirectoryTooltip");
        ImagesText = Strings.Get("ImageViewerButton");
        EnemyAmmoText = Strings.Get("EnemyAmmoButton");
        CompareText = Strings.Get("CompareButton");
        CopyText = Strings.Get("CopyButton");
        SettingsText = Strings.Get("SettingsButton");
        HelpMenuText = Strings.Get("MenuHelp");
        AboutMenuText = Strings.Get("MenuAbout");
        FileMenuText = Strings.Get("MenuFile");
        ExitMenuText = Strings.Get("MenuExit");
        ViewMenuText = Strings.Get("MenuView");
        DataMenuText = Strings.Get("MenuData");
        BulkEditText = Strings.Get("EditorBulkEditText");
        ResetLayoutText = Strings.Get("EditorResetLayoutText");
        TranslationReportText = Strings.Get("EditorTranslationReportText");
        DatasetValidationText = Strings.Get("DatasetValidationMenu");
        DatasetSearchText = Strings.Get("DatasetSearchMenu");
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
    /// Gets the text of the button that removes a directory from the list.
    /// </summary>
    public string RemoveDataDirectoryText { get; }

    /// <summary>
    /// Gets the tooltip of the button that removes a directory from the list.
    /// </summary>
    public string RemoveDataDirectoryTooltip { get; }

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
    /// Gets the Help menu header.
    /// </summary>
    public string HelpMenuText { get; }

    /// <summary>
    /// Gets the Help &gt; About menu item text.
    /// </summary>
    public string AboutMenuText { get; }

    /// <summary>
    /// Gets the File menu header.
    /// </summary>
    public string FileMenuText { get; }

    /// <summary>
    /// Gets the File &gt; Exit menu item text.
    /// </summary>
    public string ExitMenuText { get; }

    /// <summary>
    /// Gets the View menu header.
    /// </summary>
    public string ViewMenuText { get; }

    /// <summary>
    /// Gets the Data menu header.
    /// </summary>
    public string DataMenuText { get; }

    /// <summary>
    /// Gets the Data &gt; Bulk edit menu item text.
    /// </summary>
    public string BulkEditText { get; }

    /// <summary>
    /// Gets the View &gt; Reset layout menu item text.
    /// </summary>
    public string ResetLayoutText { get; }

    /// <summary>
    /// Gets the Data &gt; Translations menu item text.
    /// </summary>
    public string TranslationReportText { get; }

    /// <summary>
    /// Gets the Data &gt; Validate data set menu item text.
    /// </summary>
    public string DatasetValidationText { get; }

    /// <summary>
    /// Gets the Data &gt; Search data set menu item text.
    /// </summary>
    public string DatasetSearchText { get; }

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
        _settingsLoaded = true;
        ThemeManager.Apply(_settings.ThemeMode);
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

        // The path is only committed by a successful activation, so a rejected folder leaves the
        // active data set and the selector on the last valid directory.
        await ActivateDataDirectoryAsync(folder).ConfigureAwait(true);
    }

    /// <summary>
    /// Removes a directory from the list of known data directories after confirmation; files on disk
    /// are never deleted. The removal is refused while the directory has open editor tabs. When the
    /// active directory is removed, the closest remaining valid directory becomes active and the
    /// updated list is stored in the settings file.
    /// </summary>
    /// <param name="item">Directory to remove; taken from the selector's selected item.</param>
    /// <returns>A task that completes when the list, the active data set, and the settings are updated.</returns>
    [RelayCommand(CanExecute = nameof(CanRemoveDataDirectory))]
    private async Task RemoveDataDirectoryAsync(DataDirectoryItem? item)
    {
        if (item is null)
        {
            return;
        }

        var path = item.Path;
        if (!_dialogService.Confirm(
                Strings.Format("RemoveDataDirectoryConfirmFormat", path),
                Strings.Get("RemoveDataDirectoryTitle")))
        {
            return;
        }

        var tableDirectory = DataSetPaths.ResolveTableDataDirectory(path) ?? path;
        var openTabs = Editors.Count(editor => SamePath(editor.TableDataDirectory, tableDirectory));
        if (openTabs > 0)
        {
            _dialogService.ShowMessage(
                Strings.Format("RemoveDataDirectoryTabsOpenFormat", path, openTabs),
                Strings.Get("RemoveDataDirectoryTitle"));
            return;
        }

        // Comparing the resolved TableData directory also catches the same data set configured twice
        // under a different path, for example a game root and its Data-1.13 folder.
        var isActive = SamePath(_dataSet?.TableDataDirectory, tableDirectory)
            || SamePath(DataDirectory, path)
            || SamePath(_settings.ActiveDataDirectory, path);
        var replacement = isActive ? FindReplacementDataDirectory(tableDirectory) : null;

        _settings = _settings with
        {
            DataDirectories = RemovePath(_settings.DataDirectories, path, tableDirectory),
            RecentDataDirectories = RemovePath(_settings.RecentDataDirectories, path, tableDirectory),
            // A replacement becomes the active directory only after it opens successfully.
            ActiveDataDirectory = isActive ? null : _settings.ActiveDataDirectory,
        };

        var save = await _settingsStore.SaveAsync(_settings).ConfigureAwait(true);
        var saveError = save.IsSuccess
            ? null
            : $"{save.ErrorCode}: {save.ErrorText}";

        if (!isActive)
        {
            RebuildDataDirectories();
            StatusText = Strings.Format("RemoveDataDirectoryDoneFormat", path);
        }
        else if (replacement is null)
        {
            CancelOperation();
            ClearActiveDataSet();
            RebuildDataDirectories();
            StatusText = Strings.Get("StatusNoDataDirectory");
        }
        else
        {
            DataDirectory = replacement;
            RebuildDataDirectories();
            await ActivateDataDirectoryAsync(replacement).ConfigureAwait(true);
        }

        if (saveError is not null)
        {
            StatusText = Strings.Format("StatusError", saveError);
        }
    }

    private bool CanRemoveDataDirectory(DataDirectoryItem? item) => item is not null && !IsBusy;

    /// <summary>
    /// Finds the closest remaining directory that still contains table data. The entry before the
    /// removed one is preferred, then the entry after it; stale settings entries are skipped.
    /// </summary>
    /// <param name="removedTableDirectory">Resolved <c>TableData</c> directory being removed.</param>
    private string? FindReplacementDataDirectory(string removedTableDirectory)
    {
        var removedIndex = -1;
        for (var index = 0; index < DataDirectories.Count; index++)
        {
            if (SamePath(
                    DataSetPaths.ResolveTableDataDirectory(DataDirectories[index].Path),
                    removedTableDirectory))
            {
                removedIndex = index;
                break;
            }
        }

        for (var distance = 1; distance <= DataDirectories.Count; distance++)
        {
            var before = removedIndex - distance;
            if (before >= 0 && IsUsableDataDirectory(DataDirectories[before], removedTableDirectory))
            {
                return DataDirectories[before].Path;
            }

            var after = removedIndex + distance;
            if (after >= 0
                && after < DataDirectories.Count
                && IsUsableDataDirectory(DataDirectories[after], removedTableDirectory))
            {
                return DataDirectories[after].Path;
            }
        }

        return null;
    }

    private static bool IsUsableDataDirectory(DataDirectoryItem item, string removedTableDirectory)
    {
        var tableDirectory = DataSetPaths.ResolveTableDataDirectory(item.Path);
        return tableDirectory is not null
            && !SamePath(tableDirectory, removedTableDirectory);
    }

    private static string[] RemovePath(
        IReadOnlyList<string> paths,
        string removedPath,
        string removedTableDirectory) =>
        paths.Where(path =>
                !SamePath(path, removedPath)
                && !SamePath(DataSetPaths.ResolveTableDataDirectory(path), removedTableDirectory))
            .ToArray();

    /// <summary>
    /// Forgets the active session and empties the table list after the last configured directory was
    /// removed from the list.
    /// </summary>
    private void ClearActiveDataSet()
    {
        _dataSet = null;
        DataDirectory = string.Empty;
        Tables.Clear();
        SelectedTable = null;
        Diagnostics.Clear();
        RecordCountText = string.Empty;
        DetailsText = Strings.Get("StatusNoSelection");
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

    /// <summary>
    /// Shows the About window with the product, version, and description of the build.
    /// </summary>
    [RelayCommand]
    private void OpenAbout() => _dialogService.ShowAbout();

    /// <summary>
    /// Opens the bulk edit dialog of the active table editor. The command is disabled when the active
    /// tab is not an editable table.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanBulkEdit))]
    private void BulkEdit()
    {
        if (SelectedEditor is TableEditorViewModel editor)
        {
            editor.BulkEditCommand.Execute(null);
        }
    }

    /// <summary>
    /// Resets the grid layout of the active table editor.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanResetLayout))]
    private void ResetLayout()
    {
        if (SelectedEditor is TableEditorViewModel editor)
        {
            editor.ResetLayoutCommand.Execute(null);
        }
    }

    /// <summary>
    /// Shows the translation report of the active table editor; the editor reports the missing
    /// overlay language itself.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanShowTranslationReport))]
    private void ShowTranslationReport()
    {
        if (SelectedEditor is TableEditorViewModel editor)
        {
            editor.ShowTranslationReportCommand.Execute(null);
        }
    }

    private bool CanBulkEdit() => SelectedEditor is TableEditorViewModel { IsEditable: true };

    private bool CanResetLayout() => SelectedEditor is TableEditorViewModel;

    private bool CanShowTranslationReport() => SelectedEditor is TableEditorViewModel;

    /// <summary>
    /// Opens the data-set window on the validation tab. The report validates every table of the active
    /// data set without modifying any file.
    /// </summary>
    [RelayCommand]
    private void ValidateDataset() => ShowDatasetWindow(DatasetWindowTab.Validation);

    /// <summary>
    /// Opens the data-set window on the search tab, which searches every table value and language
    /// overlay of the active data set.
    /// </summary>
    [RelayCommand]
    private void SearchDataset() => ShowDatasetWindow(DatasetWindowTab.Search);

    private void ShowDatasetWindow(DatasetWindowTab initialTab)
    {
        if (_dataSet is null)
        {
            StatusText = Strings.Get("InvalidDataDirectory");
            return;
        }

        var viewModel = new DatasetViewModel(
            _dataSet,
            _datasetValidator,
            _datasetSearch,
            _dialogService,
            initialTab);
        viewModel.NavigationRequested += OnDatasetNavigationRequested;
        _dialogService.ShowDataset(viewModel);
    }

    private async void OnDatasetNavigationRequested(object? sender, DatasetNavigationRequest request)
    {
        try
        {
            var session = (sender as DatasetViewModel)?.Session ?? _dataSet;
            if (session is null)
            {
                return;
            }

            var editor = await OpenEditorByNameAsync(request.TableName, session).ConfigureAwait(true);
            if (editor is not null && !string.IsNullOrWhiteSpace(request.RecordKey))
            {
                editor.SelectRecord(request.RecordKey);
            }
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
        }
    }

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        var request = new SettingsRequest(
            _settings.DataDirectories,
            _settings.ActiveDataDirectory ?? DataDirectory,
            _settings.UiLanguageCode,
            _settings.RecentDataDirectories,
            _settings.ThemeMode);
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
            ThemeMode = result.ThemeMode,
        };

        var save = await _settingsStore.SaveAsync(_settings).ConfigureAwait(true);
        if (!save.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", save.ErrorText ?? string.Empty);
            return;
        }

        ThemeManager.Apply(_settings.ThemeMode);

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

    /// <summary>
    /// Saves the selected editor tab, for the global Ctrl+S shortcut when focus is outside a grid.
    /// </summary>
    [RelayCommand]
    private async Task SaveSelectedEditorAsync()
    {
        if (SelectedEditor is { IsDirty: true } editor)
        {
            await editor.SaveAsync().ConfigureAwait(true);
        }
    }

    /// <summary>
    /// Closes the selected editor tab, for the global Ctrl+W shortcut.
    /// </summary>
    [RelayCommand]
    private async Task CloseSelectedEditorAsync()
    {
        if (SelectedEditor is { } editor)
        {
            await CloseEditorAsync(editor).ConfigureAwait(true);
        }
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
                : descriptor.IsMapped && (tableName == CitiesTableName || tableName == FacilityTypesTableName)
                    ? _editorFactory.CreateNestedWorkspace(session, descriptor, language)
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
    /// Closes every tab after resolving unsaved changes, persists the window placement, and flushes
    /// pending grid layouts. The window uses this before it closes; a <see langword="false"/> result
    /// means the close must be cancelled.
    /// </summary>
    /// <param name="windowPlacement">
    /// Current size, position, and maximized state of the main window, or <see langword="null"/> when
    /// there is nothing to persist.
    /// </param>
    /// <returns><see langword="true"/> when the application may close; otherwise <see langword="false"/>.</returns>
    public async Task<bool> RequestCloseAsync(WindowPlacement? windowPlacement = null)
    {
        foreach (var editor in Editors.ToArray())
        {
            if (!await ConfirmCloseEditorAsync(editor).ConfigureAwait(true))
            {
                return false;
            }
        }

        await FlushLayoutsAsync().ConfigureAwait(true);
        if (windowPlacement is not null)
        {
            await PersistWindowPlacementAsync(windowPlacement).ConfigureAwait(true);
        }

        return true;
    }

    /// <summary>
    /// Stores the window placement when it changed. A settings write failure is reported in the status
    /// line but never blocks closing the application.
    /// </summary>
    /// <param name="placement">Placement captured from the main window.</param>
    /// <returns>A task that completes when the placement is stored or the failure is reported.</returns>
    private async Task PersistWindowPlacementAsync(WindowPlacement placement)
    {
        if (!_settingsLoaded || _settings.WindowPlacement == placement)
        {
            return;
        }

        _settings = _settings with { WindowPlacement = placement };
        var save = await _settingsStore.SaveAsync(_settings).ConfigureAwait(true);
        if (!save.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", save.ErrorText ?? string.Empty);
        }
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
                OperationLog.Warning(
                    "MainViewModel",
                    opened.ErrorText ?? $"The data directory '{DataDirectory}' could not be opened.",
                    opened.ErrorCode);
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
            OperationLog.Error(
                "MainViewModel",
                $"The tables of '{DataDirectory}' could not be refreshed.",
                exception: exception);
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
            StatusText = Strings.Format("StatusTablesLoaded", Tables.Count);
            await PersistActiveDataDirectoryAsync(path).ConfigureAwait(true);
            return;
        }

        // Snapshot the last activated directory from the shell state, not from the selector item the
        // user already moved to, so a failed activation can restore a valid data set.
        var previousDirectory = DataDirectory;
        var previousItem = DataDirectories.FirstOrDefault(item =>
                SamePath(item.Path, previousDirectory))
            ?? SelectedDataDirectory;
        var (source, generation) = BeginOperation();
        StatusText = Strings.Get("StatusLoading");
        try
        {
            var opened = await _sessionFactory.OpenAsync(path, source.Token).ConfigureAwait(true);
            if (!opened.IsSuccess)
            {
                OperationLog.Warning(
                    "MainViewModel",
                    opened.ErrorText ?? $"The data directory '{path}' could not be opened.",
                    opened.ErrorCode);
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
            OperationLog.Error(
                "MainViewModel",
                $"The data directory '{path}' could not be activated.",
                exception: exception);
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
            OperationLog.Error(
                "MainViewModel",
                result.ErrorText ?? "The table catalog could not be read.",
                result.ErrorCode);
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

        // Do not move DataDirectory before the activation succeeds; the selector is restored to the
        // previous data set when the new path cannot be opened.
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
