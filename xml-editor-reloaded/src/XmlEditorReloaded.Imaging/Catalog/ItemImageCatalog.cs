using System.Collections.Concurrent;
using System.Globalization;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Imaging.Cache;

namespace XmlEditorReloaded.Imaging.Catalog;

/// <summary>
/// Resolves big, medium, and small item images from a JA2 data directory and decodes them.
/// </summary>
public sealed class ItemImageCatalog
{
    private readonly string _dataDirectory;
    private readonly StiDecoder _decoder;
    private readonly DecodedFrameCache _cache;
    private readonly ConcurrentDictionary<string, (DateTime LastWriteTimeUtc, int FrameCount)> _frameCounts =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemImageCatalog"/> class.
    /// </summary>
    /// <param name="dataDirectory">Absolute path of the <c>Data-1.13</c> directory containing <c>BigItems</c>, <c>Interface</c>, and <c>tilesets</c>.</param>
    /// <param name="decoder">Optional STI decoder.</param>
    /// <param name="cache">Optional decoded-frame cache.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="dataDirectory"/> is empty.</exception>
    public ItemImageCatalog(
        string dataDirectory,
        StiDecoder? decoder = null,
        DecodedFrameCache? cache = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);
        _dataDirectory = dataDirectory;
        _decoder = decoder ?? new StiDecoder();
        _cache = cache ?? new DecodedFrameCache();
    }

    /// <summary>
    /// Loads one item image and returns a raw RGBA frame.
    /// </summary>
    /// <param name="size">Image size class.</param>
    /// <param name="graphicType">Item graphic type (<c>ubGraphicType</c>); zero means the gun sheet.</param>
    /// <param name="graphicIndex">Graphic index (<c>ubGraphicNum</c>).</param>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the RGBA frame.
    /// On failure, <c>error_code</c> is <c>image.not_found</c>, <c>sti.bad_magic</c>, <c>sti.truncated</c>,
    /// <c>sti.unsupported_flags</c>, or <c>sti.etrle_overrun</c>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a graphic type or index is negative.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<RgbaBuffer>> GetImageAsync(
        ImageSize size,
        int graphicType,
        int graphicIndex,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(graphicType);
        ArgumentOutOfRangeException.ThrowIfNegative(graphicIndex);
        cancellationToken.ThrowIfCancellationRequested();

        var (directory, fileName, frameIndex) = Resolve(size, graphicType, graphicIndex);
        var path = FindFile(directory, fileName);
        if (path is null)
        {
            return OperationResult<RgbaBuffer>.Failure(
                ErrorCodes.ImageNotFound,
                $"Item image '{fileName}' was not found in '{directory}'.");
        }

        var lastWriteTimeUtc = File.GetLastWriteTimeUtc(path);
        var cacheKey = path + "|" + frameIndex.ToString(CultureInfo.InvariantCulture);
        if (_cache.TryGet(cacheKey, lastWriteTimeUtc, out var cached) && cached is not null)
        {
            return OperationResult<RgbaBuffer>.Success(cached);
        }

        var loaded = await _decoder.LoadFrameAsync(path, frameIndex, cancellationToken).ConfigureAwait(false);
        if (!loaded.IsSuccess)
        {
            return loaded;
        }

        if (size != ImageSize.Big)
        {
            ImageTransparency.ApplyGreenKey(loaded.Data!);
        }

        _cache.Set(cacheKey, loaded.Data!, lastWriteTimeUtc);
        return loaded;
    }

    /// <summary>
    /// Reads the number of frames in a medium or small item sprite sheet.
    /// </summary>
    /// <param name="size">Sheet size class; <see cref="ImageSize.Big"/> is not a sheet and is rejected.</param>
    /// <param name="graphicType">Item graphic type (<c>ubGraphicType</c>); zero means the gun sheet.</param>
    /// <param name="cancellationToken">Token to cancel the read.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> is the frame directory count.
    /// For <see cref="ImageSize.Big"/>, <c>error_code</c> is <c>image.no_sheet</c> because big images are
    /// separate files rather than sheet frames. Other failures are <c>image.not_found</c>,
    /// <c>sti.bad_magic</c>, <c>sti.truncated</c>, <c>sti.unsupported_flags</c>, or
    /// <c>io.file_not_found</c>/<c>io.access_denied</c>/<c>io.error</c>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="graphicType"/> is negative.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<int>> GetFrameCountAsync(
        ImageSize size,
        int graphicType,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(graphicType);
        if (size == ImageSize.Big)
        {
            return OperationResult<int>.Failure(
                ErrorCodes.ImageNoSheet,
                "Big item images are separate files; frame counts apply to medium and small sheets only.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var (directory, fileName, _) = Resolve(size, graphicType, 0);
        var path = FindFile(directory, fileName);
        if (path is null)
        {
            return OperationResult<int>.Failure(
                ErrorCodes.ImageNotFound,
                $"Item image sheet '{fileName}' was not found in '{directory}'.");
        }

        var lastWriteTimeUtc = File.GetLastWriteTimeUtc(path);
        if (_frameCounts.TryGetValue(path, out var cached)
            && cached.LastWriteTimeUtc == lastWriteTimeUtc)
        {
            return OperationResult<int>.Success(cached.FrameCount);
        }

        var parsed = await _decoder.ParseAsync(path, cancellationToken).ConfigureAwait(false);
        if (!parsed.IsSuccess)
        {
            return OperationResult<int>.Failure(parsed);
        }

        var count = parsed.Data!.SubImages.Count;
        _frameCounts[path] = (lastWriteTimeUtc, count);
        return OperationResult<int>.Success(count);
    }

    /// <summary>
    /// Reads the highest valid <c>ubGraphicNum</c> for a graphic type. The legacy editor used the
    /// length of the small sprite sheet, so this is <see cref="GetFrameCountAsync"/> for
    /// <see cref="ImageSize.Small"/> minus one.
    /// </summary>
    /// <param name="graphicType">Item graphic type (<c>ubGraphicType</c>); zero means the gun sheet.</param>
    /// <param name="cancellationToken">Token to cancel the read.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> is the inclusive maximum
    /// graphic index. On failure, the error of <see cref="GetFrameCountAsync"/> is returned.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="graphicType"/> is negative.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<int>> GetMaxIndexAsync(
        int graphicType,
        CancellationToken cancellationToken = default)
    {
        var count = await GetFrameCountAsync(ImageSize.Small, graphicType, cancellationToken)
            .ConfigureAwait(false);
        if (!count.IsSuccess)
        {
            return OperationResult<int>.Failure(count);
        }

        return OperationResult<int>.Success(count.Data - 1);
    }

    private (string Directory, string FileName, int FrameIndex) Resolve(
        ImageSize size,
        int graphicType,
        int graphicIndex)
    {
        var indexText = FormatIndex(graphicIndex);
        return size switch
        {
            ImageSize.Big => (
                Path.Combine(_dataDirectory, "BigItems"),
                graphicType == 0 ? $"gun{indexText}.sti" : $"p{graphicType}item{indexText}.sti",
                0),
            ImageSize.Medium => (
                Path.Combine(_dataDirectory, "Interface"),
                graphicType == 0 ? "mdguns.sti" : $"mdp{graphicType}items.sti",
                graphicIndex),
            ImageSize.Small => (
                Path.Combine(_dataDirectory, "tilesets", "0"),
                graphicType == 0 ? "smguns.sti" : $"smp{graphicType}items.sti",
                graphicIndex),
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, "Unknown image size."),
        };
    }

    private static string FormatIndex(int index) =>
        index < 10
            ? index.ToString("00", CultureInfo.InvariantCulture)
            : index.ToString(CultureInfo.InvariantCulture);

    private static string? FindFile(string directory, string fileName)
    {
        if (!Directory.Exists(directory))
        {
            return null;
        }

        var exact = Path.Combine(directory, fileName);
        if (File.Exists(exact))
        {
            return exact;
        }

        try
        {
            return Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly)
                .FirstOrDefault(path => string.Equals(Path.GetFileName(path), fileName, StringComparison.OrdinalIgnoreCase));
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }
}
