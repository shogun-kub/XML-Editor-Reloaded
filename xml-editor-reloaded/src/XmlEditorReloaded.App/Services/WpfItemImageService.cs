using System.Collections.Concurrent;
using System.Windows.Media.Imaging;
using XmlEditorReloaded.App.Imaging;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Imaging.Catalog;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Resolves item images through <see cref="ItemImageCatalog"/> and converts them to WPF bitmaps.
/// Catalogs are cached per data directory so switching directories does not recreate the decoder cache.
/// </summary>
public sealed class WpfItemImageService : IItemImageService
{
    private readonly ConcurrentDictionary<string, ItemImageCatalog> _catalogs =
        new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public async Task<OperationResult<BitmapSource>> GetImageAsync(
        string gameDataDirectory,
        ImageSize size,
        int graphicType,
        int graphicIndex,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gameDataDirectory);
        cancellationToken.ThrowIfCancellationRequested();

        var catalog = _catalogs.GetOrAdd(
            gameDataDirectory,
            static directory => new ItemImageCatalog(directory));
        var image = await catalog
            .GetImageAsync(size, graphicType, graphicIndex, cancellationToken)
            .ConfigureAwait(false);

        return image.IsSuccess
            ? OperationResult<BitmapSource>.Success(RgbaBufferConverter.ToBitmapSource(image.Data!))
            : OperationResult<BitmapSource>.Failure(image);
    }
}
