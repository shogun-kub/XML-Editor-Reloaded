using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// View model for the main shell: data directory selection, table list, and load diagnostics.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IDataSetSessionFactory _sessionFactory;
    private readonly ITableEditorFactory _editorFactory;
    private readonly ISettingsStore _settingsStore;
    private readonly IDialogService _dialogService;
    private readonly IImageViewerService _imageViewer;
    private AppSettings _settings = AppSettings.Default;
    private IDataSetSession? _dataSet;
    private CancellationTokenSource? _operationCts;
    private int _operationGeneration;

    [ObservableProperty]
    private string _dataDirectory = string.Empty;

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
    private TableEditorViewModel? _selectedEditor;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="sessionFactory">Factory for runtime data-set sessions.</param>
    /// <param name="editorFactory">Factory for table editor view models.</param>
    /// <param name="settingsStore">Settings store.</param>
    /// <param name="dialogService">Dialog service.</param>
    /// <param name="imageViewer">Item image viewer service.</param>
    public MainViewModel(
        IDataSetSessionFactory sessionFactory,
        ITableEditorFactory editorFactory,
        ISettingsStore settingsStore,
        IDialogService dialogService,
        IImageViewerService imageViewer)
    {
        ArgumentNullException.ThrowIfNull(sessionFactory);
        ArgumentNullException.ThrowIfNull(editorFactory);
        ArgumentNullException.ThrowIfNull(settingsStore);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(imageViewer);

        _sessionFactory = sessionFactory;
        _editorFactory = editorFactory;
        _settingsStore = settingsStore;
        _dialogService = dialogService;
        _imageViewer = imageViewer;

        Editors.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasEditors));

        Title = Strings.Get("AppTitle");
        BrowseText = Strings.Get("BrowseText");
        RefreshText = Strings.Get("RefreshText");
        ImagesText = Strings.Get("ImageViewerButton");
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
    /// Gets the open table editors.
    /// </summary>
    public ObservableCollection<TableEditorViewModel> Editors { get; } = new();

    /// <summary>
    /// Gets a value indicating whether at least one table editor is open.
    /// </summary>
    public bool HasEditors => Editors.Count > 0;

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
    /// Gets the discovered tables.
    /// </summary>
    public ObservableCollection<TableListItem> Tables { get; } = new();

    /// <summary>
    /// Gets diagnostics for the selected table.
    /// </summary>
    public ObservableCollection<string> Diagnostics { get; } = new();

    /// <summary>
    /// Loads settings and the last active data directory.
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

        DataDirectory = folder;
        await SaveDataDirectoryAsync(folder).ConfigureAwait(true);
        await RefreshAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private void OpenImageViewer() => _imageViewer.Show(DataDirectory);

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

        var existing = Editors.FirstOrDefault(editor =>
            editor.Descriptor.TableName == SelectedTable.Descriptor.TableName);
        if (existing is not null)
        {
            SelectedEditor = existing;
            return;
        }

        var editor = _editorFactory.Create(
            _dataSet,
            SelectedTable.Descriptor,
            ResolveLanguageCode(SelectedTable.Descriptor));
        Editors.Add(editor);
        SelectedEditor = editor;
        await editor.InitializeAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private void CloseEditor(TableEditorViewModel? editor)
    {
        if (editor is null)
        {
            return;
        }

        if (editor.IsDirty
            && !_dialogService.Confirm(
                Strings.Format("EditorConfirmClose", editor.Title),
                Strings.Get("EditorCloseTitle")))
        {
            return;
        }

        var wasSelected = ReferenceEquals(SelectedEditor, editor);
        Editors.Remove(editor);
        if (wasSelected)
        {
            SelectedEditor = Editors.FirstOrDefault();
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
            await editor.SaveAsync().ConfigureAwait(true);
            if (editor.IsDirty)
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
                if (!CloseAllEditorsWithConfirmation())
                {
                    StatusText = Strings.Get("EditorRefreshCancelled");
                    return;
                }

                _dataSet = null;
                Tables.Clear();
                SelectedTable = null;
                Diagnostics.Clear();
                RecordCountText = string.Empty;
                StatusText = Strings.Get("InvalidDataDirectory");
                return;
            }

            var session = opened.Data!;
            if (_dataSet is not null
                && !string.Equals(
                    _dataSet.TableDataDirectory,
                    session.TableDataDirectory,
                    StringComparison.OrdinalIgnoreCase)
                && !CloseAllEditorsWithConfirmation())
            {
                StatusText = Strings.Get("EditorRefreshCancelled");
                return;
            }

            _dataSet = session;
            var result = await session.Catalog
                .GetTablesAsync(session.TableDataDirectory, source.Token)
                .ConfigureAwait(true);
            if (!result.IsSuccess)
            {
                Tables.Clear();
                SelectedTable = null;
                StatusText = Strings.Format("StatusError", result.ErrorText ?? string.Empty);
                return;
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

            StatusText = Strings.Format("StatusTablesLoaded", Tables.Count);
        }
        catch (OperationCanceledException)
        {
            // A newer refresh or selection replaced this operation.
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

    private bool CloseAllEditorsWithConfirmation()
    {
        var dirty = Editors.Where(editor => editor.IsDirty).ToList();
        if (dirty.Count > 0
            && !_dialogService.Confirm(
                Strings.Format("EditorConfirmCloseAll", dirty.Count),
                Strings.Get("EditorCloseTitle")))
        {
            return false;
        }

        Editors.Clear();
        SelectedEditor = null;
        return true;
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

    private async Task SaveDataDirectoryAsync(string path)
    {
        var directories = _settings.DataDirectories.ToList();
        if (!directories.Any(existing => string.Equals(existing, path, StringComparison.OrdinalIgnoreCase)))
        {
            directories.Add(path);
        }

        var recent = new List<string> { path };
        recent.AddRange(_settings.RecentDataDirectories.Where(existing =>
            !string.Equals(existing, path, StringComparison.OrdinalIgnoreCase)));
        _settings = _settings with
        {
            DataDirectories = directories,
            ActiveDataDirectory = path,
            RecentDataDirectories = recent.Take(10).ToArray(),
        };

        var save = await _settingsStore.SaveAsync(_settings).ConfigureAwait(true);
        if (!save.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", save.ErrorText ?? string.Empty);
        }
    }

    private void CancelOperation()
    {
        _operationCts?.Cancel();
        _operationCts?.Dispose();
        _operationCts = null;
    }
}
