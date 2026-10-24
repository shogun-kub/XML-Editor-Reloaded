using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// View model for the standalone item image viewer. It resolves the item list from
/// <c>Items.Items</c> and shows the big, medium, and small STI previews for the selected graphic.
/// </summary>
public partial class ImageViewerViewModel : ObservableObject
{
    private readonly IItemImageService _images;
    private readonly ITableCatalog _catalog;
    private readonly ITableRepository _repository;
    private CancellationTokenSource? _loadCts;
    private string? _gameDataDirectory;
    private bool _suppressItemLoad;

    [ObservableProperty]
    private string _dataDirectoryText = string.Empty;

    [ObservableProperty]
    private ItemGraphicsOption? _selectedItem;

    [ObservableProperty]
    private int _graphicType;

    [ObservableProperty]
    private int _graphicIndex;

    [ObservableProperty]
    private BitmapSource? _bigImage;

    [ObservableProperty]
    private BitmapSource? _mediumImage;

    [ObservableProperty]
    private BitmapSource? _smallImage;

    [ObservableProperty]
    private string _bigInfoText = string.Empty;

    [ObservableProperty]
    private string _mediumInfoText = string.Empty;

    [ObservableProperty]
    private string _smallInfoText = string.Empty;

    [ObservableProperty]
    private string _statusText;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private double _zoom = 1.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageViewerViewModel"/> class.
    /// </summary>
    /// <param name="images">Item image service.</param>
    /// <param name="catalog">Table catalog used to locate <c>Items.Items</c>.</param>
    /// <param name="repository">Table repository used to load the item list.</param>
    public ImageViewerViewModel(
        IItemImageService images,
        ITableCatalog catalog,
        ITableRepository repository)
    {
        ArgumentNullException.ThrowIfNull(images);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(repository);

        _images = images;
        _catalog = catalog;
        _repository = repository;

        Title = Strings.Get("ImageViewerTitle");
        DataDirectoryLabel = Strings.Get("DataDirectoryLabel");
        ItemLabel = Strings.Get("ImageItemLabel");
        TypeLabel = Strings.Get("ImageTypeLabel");
        IndexLabel = Strings.Get("ImageIndexLabel");
        RefreshText = Strings.Get("ImageRefreshText");
        PreviousText = Strings.Get("ImagePreviousText");
        NextText = Strings.Get("ImageNextText");
        BigLabel = Strings.Get("ImageBigLabel");
        MediumLabel = Strings.Get("ImageMediumLabel");
        SmallLabel = Strings.Get("ImageSmallLabel");
        ZoomLabel = Strings.Get("ImageZoomLabel");
        _statusText = Strings.Get("ImageNoSelection");
    }

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the data directory label.
    /// </summary>
    public string DataDirectoryLabel { get; }

    /// <summary>
    /// Gets the item selector label.
    /// </summary>
    public string ItemLabel { get; }

    /// <summary>
    /// Gets the graphic type label.
    /// </summary>
    public string TypeLabel { get; }

    /// <summary>
    /// Gets the graphic index label.
    /// </summary>
    public string IndexLabel { get; }

    /// <summary>
    /// Gets the refresh button text.
    /// </summary>
    public string RefreshText { get; }

    /// <summary>
    /// Gets the previous item button text.
    /// </summary>
    public string PreviousText { get; }

    /// <summary>
    /// Gets the next item button text.
    /// </summary>
    public string NextText { get; }

    /// <summary>
    /// Gets the big image panel label.
    /// </summary>
    public string BigLabel { get; }

    /// <summary>
    /// Gets the medium image panel label.
    /// </summary>
    public string MediumLabel { get; }

    /// <summary>
    /// Gets the small image panel label.
    /// </summary>
    public string SmallLabel { get; }

    /// <summary>
    /// Gets the zoom slider label.
    /// </summary>
    public string ZoomLabel { get; }

    /// <summary>
    /// Gets the item list projected from <c>Items.Items</c>.
    /// </summary>
    public ObservableCollection<ItemGraphicsOption> Items { get; } = new();

    /// <summary>
    /// Loads the item list and the initial previews for a data directory.
    /// </summary>
    /// <param name="selectedPath">
    /// Path selected in the main window; it may point at <c>TableData</c>, <c>Data-1.13</c>, or a game root.
    /// </param>
    /// <returns>A task that completes when the item list and initial previews are loaded.</returns>
    public async Task InitializeAsync(string? selectedPath)
    {
        DataDirectoryText = selectedPath ?? string.Empty;
        _gameDataDirectory = DataSetPaths.ResolveGameDataDirectory(selectedPath);
        var tableData = DataSetPaths.ResolveTableDataDirectory(selectedPath);

        _suppressItemLoad = true;
        Items.Clear();
        SelectedItem = null;
        _suppressItemLoad = false;
        ClearImages();
        Zoom = 1.0;

        if (_gameDataDirectory is null)
        {
            StatusText = Strings.Get("ImageNoDataDirectory");
            return;
        }

        var token = BeginLoad();
        IsBusy = true;
        StatusText = Strings.Get("ImageLoadingItems");
        try
        {
            if (tableData is null)
            {
                StatusText = Strings.Get("ImageItemsUnavailable");
                return;
            }

            var tables = await _catalog.GetTablesAsync(tableData, token).ConfigureAwait(true);
            var descriptor = tables.IsSuccess
                ? tables.Data!.FirstOrDefault(static table =>
                    string.Equals(table.TableName.Value, "Items.Items", StringComparison.Ordinal))
                : null;
            if (descriptor is null)
            {
                StatusText = Strings.Get("ImageItemsUnavailable");
                return;
            }

            var load = await _repository.LoadTableAsync(descriptor, token).ConfigureAwait(true);
            var document = load.IsSuccess ? load.Data!.GetDocument<TableDocument>() : null;
            if (document is null)
            {
                StatusText = Strings.Get("ImageItemsUnavailable");
                return;
            }

            var position = 0;
            foreach (var record in document.Records)
            {
                Items.Add(ItemGraphicsOption.FromRecord(record, position));
                position++;
            }

            var initial = Items.FirstOrDefault(static item => item.GraphicNum is not null);
            if (initial is null)
            {
                StatusText = Items.Count == 0
                    ? Strings.Get("ImageItemsUnavailable")
                    : Strings.Get("ImageItemWithoutGraphic");
                return;
            }

            _suppressItemLoad = true;
            SelectedItem = initial;
            _suppressItemLoad = false;
            GraphicType = initial.GraphicType ?? 0;
            GraphicIndex = initial.GraphicNum!.Value;
            await LoadImagesAsync(token).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // A newer load replaced this one.
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

    partial void OnSelectedItemChanged(ItemGraphicsOption? value)
    {
        if (_suppressItemLoad || value is null)
        {
            return;
        }

        if (value.GraphicNum is null)
        {
            ClearImages();
            StatusText = Strings.Get("ImageItemWithoutGraphic");
            return;
        }

        GraphicType = value.GraphicType ?? 0;
        GraphicIndex = value.GraphicNum.Value;
        _ = LoadImagesSafeAsync();
    }

    [RelayCommand]
    private Task RefreshAsync() => LoadImagesSafeAsync();

    [RelayCommand]
    private void PreviousItem() => MoveSelection(-1);

    [RelayCommand]
    private void NextItem() => MoveSelection(1);

    private async Task LoadImagesSafeAsync()
    {
        if (_gameDataDirectory is null)
        {
            StatusText = Strings.Get("ImageNoDataDirectory");
            return;
        }

        var token = BeginLoad();
        IsBusy = true;
        try
        {
            await LoadImagesAsync(token).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // A newer load replaced this one.
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

    private async Task LoadImagesAsync(CancellationToken cancellationToken)
    {
        if (_gameDataDirectory is null)
        {
            StatusText = Strings.Get("ImageNoDataDirectory");
            return;
        }

        ClearImages();
        StatusText = Strings.Get("ImageLoading");

        var bigTask = _images.GetImageAsync(
            _gameDataDirectory, ImageSize.Big, GraphicType, GraphicIndex, cancellationToken);
        var mediumTask = _images.GetImageAsync(
            _gameDataDirectory, ImageSize.Medium, GraphicType, GraphicIndex, cancellationToken);
        var smallTask = _images.GetImageAsync(
            _gameDataDirectory, ImageSize.Small, GraphicType, GraphicIndex, cancellationToken);

        var results = await Task.WhenAll(bigTask, mediumTask, smallTask).ConfigureAwait(true);
        cancellationToken.ThrowIfCancellationRequested();

        Apply(ImageSize.Big, results[0]);
        Apply(ImageSize.Medium, results[1]);
        Apply(ImageSize.Small, results[2]);
        StatusText = Strings.Format("ImageLoaded", GraphicType, GraphicIndex);
    }

    private void Apply(ImageSize size, OperationResult<BitmapSource> result)
    {
        var image = result.Data;
        var info = image is not null
            ? $"{image.PixelWidth}×{image.PixelHeight}"
            : Strings.Format("ImageNotAvailableFormat", result.ErrorCode ?? string.Empty);

        switch (size)
        {
            case ImageSize.Big:
                BigImage = image;
                BigInfoText = info;
                break;
            case ImageSize.Medium:
                MediumImage = image;
                MediumInfoText = info;
                break;
            case ImageSize.Small:
                SmallImage = image;
                SmallInfoText = info;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(size), size, "Unknown image size.");
        }
    }

    private void ClearImages()
    {
        BigImage = null;
        MediumImage = null;
        SmallImage = null;
        BigInfoText = string.Empty;
        MediumInfoText = string.Empty;
        SmallInfoText = string.Empty;
    }

    private void MoveSelection(int delta)
    {
        if (Items.Count == 0)
        {
            return;
        }

        var current = SelectedItem is null ? -1 : Items.IndexOf(SelectedItem);
        var next = Math.Clamp(current + delta, 0, Items.Count - 1);
        if (next >= 0 && !ReferenceEquals(Items[next], SelectedItem))
        {
            SelectedItem = Items[next];
        }
    }

    private CancellationToken BeginLoad()
    {
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = new CancellationTokenSource();
        return _loadCts.Token;
    }
}
