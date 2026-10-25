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
    /// Gets or sets an optional asynchronous gate that runs after an image request is recorded. It is
    /// used to test what happens when a thumbnail load is still pending.
    /// </summary>
    public Func<TestImageRequest, Task>? BeforeImageReturn { get; set; }

    /// <summary>
    /// Gets or sets an optional bitmap factory, so a test can distinguish images by their pixels.
    /// </summary>
    public Func<TestImageRequest, BitmapSource?>? BitmapFactory { get; set; }

    /// <summary>
    /// Gets or sets an optional asynchronous gate for <see cref="GetFrameCountAsync"/>.
    /// </summary>
    public Func<int, Task>? BeforeFrameCountReturn { get; set; }

    /// <summary>
    /// Gets or sets the bitmap returned on success; a default 2×2 bitmap is used when null.
    /// </summary>
    public BitmapSource? Bitmap { get; set; }

    /// <summary>
    /// Gets or sets an error code returned instead of an image.
    /// </summary>
    public string? FailCode { get; set; }

    /// <summary>
    /// Gets or sets the sheet frame count returned by <see cref="GetFrameCountAsync"/>. Zero reports
    /// <c>image.not_found</c>. A value in <see cref="FrameCountsByType"/> takes precedence.
    /// </summary>
    public int FrameCount { get; set; }

    /// <summary>
    /// Gets per-graphic-type sheet frame counts. Used when a test needs the chooser to change after
    /// <c>ubGraphicType</c> is edited.
    /// </summary>
    public Dictionary<int, int> FrameCountsByType { get; } = new();

    /// <summary>
    /// Gets the graphic types whose sheet frame count was requested, in request order.
    /// </summary>
    public List<int> FrameCountTypes { get; } = new();

    /// <inheritdoc />
    public async Task<OperationResult<BitmapSource>> GetImageAsync(
        string gameDataDirectory,
        ImageSize size,
        int graphicType,
        int graphicIndex,
        CancellationToken cancellationToken = default)
    {
        var request = new TestImageRequest(gameDataDirectory, size, graphicType, graphicIndex);
        Requests.Add(request);
        if (BeforeImageReturn is not null)
        {
            await BeforeImageReturn(request);
        }

        if (FailCode is not null)
        {
            return OperationResult<BitmapSource>.Failure(FailCode, "The image is missing.");
        }

        var fromFactory = BitmapFactory?.Invoke(request);
        if (fromFactory is not null)
        {
            return OperationResult<BitmapSource>.Success(fromFactory);
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

        return OperationResult<BitmapSource>.Success(Bitmap);
    }

    /// <summary>
    /// Creates a frozen bitmap whose width identifies its graphic index in thumbnail-race tests.
    /// </summary>
    /// <param name="width">Bitmap width and height in pixels.</param>
    /// <returns>A frozen BGRA32 bitmap.</returns>
    internal static BitmapSource CreateBitmap(int width)
    {
        var bitmap = BitmapSource.Create(
            width,
            width,
            96,
            96,
            PixelFormats.Bgra32,
            null,
            new byte[width * width * 4],
            width * 4);
        bitmap.Freeze();
        return bitmap;
    }

    /// <inheritdoc />
    public async Task<OperationResult<int>> GetFrameCountAsync(
        string gameDataDirectory,
        ImageSize size,
        int graphicType,
        CancellationToken cancellationToken = default)
    {
        FrameCountTypes.Add(graphicType);
        if (BeforeFrameCountReturn is not null)
        {
            await BeforeFrameCountReturn(graphicType);
        }

        var count = FrameCountsByType.TryGetValue(graphicType, out var configured)
            ? configured
            : FrameCount;
        if (count <= 0)
        {
            return OperationResult<int>.Failure(
                ErrorCodes.ImageNotFound,
                "The image sheet is missing.");
        }

        return OperationResult<int>.Success(count);
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
