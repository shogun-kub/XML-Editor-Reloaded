using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One entry of the item selector in the item workspace: the primary key, the effective name, and the
/// class text of an item record.
/// </summary>
public sealed partial class ItemListEntry : ObservableObject
{
    private readonly Func<TableRowViewModel, string> _classResolver;
    private Func<TableRowViewModel, Task<BitmapSource?>>? _thumbnailLoader;
    private BitmapSource? _thumbnail;
    private bool _thumbnailRequested;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemListEntry"/> class.
    /// </summary>
    /// <param name="row">Item row over the merged base and overlay values.</param>
    /// <param name="classResolver">Formats the item class value for display.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="row"/> or <paramref name="classResolver"/> is null.</exception>
    public ItemListEntry(TableRowViewModel row, Func<TableRowViewModel, string> classResolver)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(classResolver);
        Row = row;
        _classResolver = classResolver;
        row.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == "Item[]")
            {
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(ClassText));
            }
        };
    }

    /// <summary>
    /// Gets the item row.
    /// </summary>
    public TableRowViewModel Row { get; }

    /// <summary>
    /// Gets the item key, or an empty string when the record has no key.
    /// </summary>
    public string Key => Row.Key ?? string.Empty;

    /// <summary>
    /// Gets the item name shown in the selector.
    /// </summary>
    public string Name => Row["szItemName"];

    /// <summary>
    /// Gets the item class text.
    /// </summary>
    public string ClassText => _classResolver(Row);

    /// <summary>
    /// Gets the text shown in the selector, for example <c>311: Combat Knife</c>.
    /// </summary>
    public string DisplayText => Name.Length == 0 ? Key : $"{Key}: {Name}";

    /// <summary>
    /// Gets the lazily loaded small item image, or <see langword="null"/> when no image is available.
    /// </summary>
    public BitmapSource? Thumbnail
    {
        get => _thumbnail;
        private set => SetProperty(ref _thumbnail, value);
    }

    /// <summary>
    /// Starts the thumbnail load at most once. Called when the list container becomes visible.
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

    internal void ConfigureThumbnail(Func<TableRowViewModel, Task<BitmapSource?>>? loader)
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
            Thumbnail = await loader(Row).ConfigureAwait(true);
        }
        catch (Exception)
        {
            // A missing or undecodable image only leaves the thumbnail empty.
        }
    }
}
