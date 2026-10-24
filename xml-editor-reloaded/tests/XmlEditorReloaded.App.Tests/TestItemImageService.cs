using System.Windows.Media;
using System.Windows.Media.Imaging;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Imaging;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Configurable item image service for view-model tests.
/// </summary>
internal sealed class TestItemImageService : IItemImageService
{
    /// <summary>
    /// Gets the recorded image requests.
    /// </summary>
    public List<TestImageRequest> Requests { get; } = new();

    /// <summary>
    /// Gets or sets the bitmap returned on success; a default 2×2 bitmap is used when null.
    /// </summary>
    public BitmapSource? Bitmap { get; set; }

    /// <summary>
    /// Gets or sets an error code returned instead of an image.
    /// </summary>
    public string? FailCode { get; set; }

    /// <inheritdoc />
    public Task<OperationResult<BitmapSource>> GetImageAsync(
        string gameDataDirectory,
        ImageSize size,
        int graphicType,
        int graphicIndex,
        CancellationToken cancellationToken = default)
    {
        Requests.Add(new TestImageRequest(gameDataDirectory, size, graphicType, graphicIndex));
        if (FailCode is not null)
        {
            return Task.FromResult(OperationResult<BitmapSource>.Failure(FailCode, "The image is missing."));
        }

        if (Bitmap is null)
        {
            var bitmap = BitmapSource.Create(
                2,
                2,
                96,
                96,
                PixelFormats.Bgra32,
                null,
                new byte[16],
                8);
            bitmap.Freeze();
            Bitmap = bitmap;
        }

        return Task.FromResult(OperationResult<BitmapSource>.Success(Bitmap));
    }
}

/// <summary>
/// Recorded item image request.
/// </summary>
/// <param name="DataDirectory">Requested game data directory.</param>
/// <param name="Size">Requested size.</param>
/// <param name="GraphicType">Requested graphic type.</param>
/// <param name="GraphicIndex">Requested graphic index.</param>
internal sealed record TestImageRequest(
    string DataDirectory,
    ImageSize Size,
    int GraphicType,
    int GraphicIndex);
