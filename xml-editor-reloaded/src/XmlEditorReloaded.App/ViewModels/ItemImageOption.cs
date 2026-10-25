using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One selectable frame of an item graphic type in the item workspace image chooser. The thumbnail is
/// loaded lazily when the row becomes visible; selecting the entry writes its index to
/// <c>ubGraphicNum</c>.
/// </summary>
public sealed partial class ItemImageOption : ObservableObject
{
    private Func<ItemImageOption, Task<BitmapSource?>>? _thumbnailLoader;
    private BitmapSource? _thumbnail;
    private bool _thumbnailRequested;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemImageOption"/> class.
    /// </summary>
    /// <param name="graphicType">Item graphic type (<c>ubGraphicType</c>).</param>
    /// <param name="index">Zero-based graphic index (<c>ubGraphicNum</c>).</param>
    /// <param name="displayText">Localized text shown in the image chooser.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an index or type is negative.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="displayText"/> is empty.</exception>
    public ItemImageOption(int graphicType, int index, string displayText)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(graphicType);
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayText);

        GraphicType = graphicType;
        Index = index;
        DisplayText = displayText;
    }

    /// <summary>
    /// Gets the item graphic type (<c>ubGraphicType</c>).
    /// </summary>
    public int GraphicType { get; }

    /// <summary>
    /// Gets the zero-based graphic index (<c>ubGraphicNum</c>).
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the text shown in the image chooser.
    /// </summary>
    public string DisplayText { get; }

    /// <summary>
    /// Gets the lazily loaded image thumbnail, or <see langword="null"/> when no image is available.
    /// </summary>
    public BitmapSource? Thumbnail
    {
        get => _thumbnail;
        private set => SetProperty(ref _thumbnail, value);
    }

    /// <summary>
    /// Starts the thumbnail load at most once. Called when the chooser item becomes visible.
    /// </summary>
    public void RequestThumbnail()
    {
        if (_thumbnailRequested || _thumbnailLoader is null)
        {
            return;
        }

        _thumbnailRequested = true;
        _ = LoadThumbnailAsync();
    }

    internal void ConfigureThumbnail(Func<ItemImageOption, Task<BitmapSource?>>? loader)
    {
        _thumbnailLoader = loader;
    }

    private async Task LoadThumbnailAsync()
    {
        var loader = _thumbnailLoader;
        if (loader is null)
        {
            return;
        }

        try
        {
            Thumbnail = await loader(this).ConfigureAwait(true);
        }
        catch (Exception)
        {
            // A missing or undecodable image only leaves the chooser thumbnail empty.
        }
    }
}
