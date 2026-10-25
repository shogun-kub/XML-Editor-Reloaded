using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Imaging.Cache;
using XmlEditorReloaded.Imaging.Catalog;

namespace XmlEditorReloaded.Imaging.Tests;

/// <summary>
/// Tests for <see cref="ItemImageCatalog"/>.
/// </summary>
public class ItemImageCatalogTests
{
    /// <summary>
    /// Big, medium, and small item images resolve to their real fixtures and decode.
    /// </summary>
    [Fact]
    public async Task Catalog_LoadsAllSizeClasses()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.CopyFixtureDirectory("Graphics");
        var cache = new DecodedFrameCache(1024 * 1024);
        var catalog = new ItemImageCatalog(dataDirectory, cache: cache);

        var big = await catalog.GetImageAsync(ImageSize.Big, 2, 35);
        var medium = await catalog.GetImageAsync(ImageSize.Medium, 3, 0);
        var small = await catalog.GetImageAsync(ImageSize.Small, 3, 0);

        Assert.True(big.IsSuccess, big.ErrorText);
        Assert.Equal(39, big.Data!.Width);
        Assert.Equal(42, big.Data.Height);
        Assert.True(medium.IsSuccess, medium.ErrorText);
        Assert.True(small.IsSuccess, small.ErrorText);
        Assert.Equal(3, cache.Count);
    }

    /// <summary>
    /// A missing item image is reported without an exception.
    /// </summary>
    [Fact]
    public async Task Catalog_MissingImage_ReturnsImageNotFound()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.CopyFixtureDirectory("Graphics");
        var catalog = new ItemImageCatalog(dataDirectory);

        var result = await catalog.GetImageAsync(ImageSize.Big, 0, 5);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.ImageNotFound, result.ErrorCode);
    }

    /// <summary>
    /// The cache returns the same decoded buffer on a repeat request.
    /// </summary>
    [Fact]
    public async Task Catalog_SecondRequest_UsesCache()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.CopyFixtureDirectory("Graphics");
        var cache = new DecodedFrameCache(1024 * 1024);
        var catalog = new ItemImageCatalog(dataDirectory, cache: cache);

        var first = await catalog.GetImageAsync(ImageSize.Big, 2, 35);
        var second = await catalog.GetImageAsync(ImageSize.Big, 2, 35);

        Assert.True(first.IsSuccess, first.ErrorText);
        Assert.True(second.IsSuccess, second.ErrorText);
        Assert.Same(first.Data, second.Data);
    }

    /// <summary>
    /// The small sheet frame count is the inclusive maximum graphic index and is cached until the file
    /// changes.
    /// </summary>
    [Fact]
    public async Task Catalog_FrameCount_MatchesSmallSheetAndMaxIndex()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.CopyFixtureDirectory("Graphics");
        var catalog = new ItemImageCatalog(dataDirectory);

        var count = await catalog.GetFrameCountAsync(ImageSize.Small, 3);
        var maximum = await catalog.GetMaxIndexAsync(3);
        var cached = await catalog.GetFrameCountAsync(ImageSize.Small, 3);

        Assert.True(count.IsSuccess, count.ErrorText);
        Assert.Equal(12, count.Data);
        Assert.True(maximum.IsSuccess, maximum.ErrorText);
        Assert.Equal(11, maximum.Data);
        Assert.Equal(12, cached.Data);
    }

    /// <summary>
    /// A missing sheet is reported as <c>image.not_found</c>; big images are separate files and have no
    /// sheet frame count.
    /// </summary>
    [Fact]
    public async Task Catalog_FrameCount_MissingSheetAndBigSizeAreRejected()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.CopyFixtureDirectory("Graphics");
        var catalog = new ItemImageCatalog(dataDirectory);

        var missing = await catalog.GetFrameCountAsync(ImageSize.Small, 2);
        var big = await catalog.GetFrameCountAsync(ImageSize.Big, 2);

        Assert.False(missing.IsSuccess);
        Assert.Equal(ErrorCodes.ImageNotFound, missing.ErrorCode);
        Assert.False(big.IsSuccess);
        Assert.Equal(ErrorCodes.ImageNoSheet, big.ErrorCode);
    }

    /// <summary>
    /// Changing a sheet on disk invalidates the cached frame count and the next read reports the new
    /// (here malformed) content instead of the old count.
    /// </summary>
    [Fact]
    public async Task Catalog_FrameCount_ReReadsWhenSheetChanges()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.CopyFixtureDirectory("Graphics");
        var catalog = new ItemImageCatalog(dataDirectory);
        var first = await catalog.GetFrameCountAsync(ImageSize.Small, 3);
        Assert.True(first.IsSuccess, first.ErrorText);

        var sheetPath = Path.Combine(dataDirectory, "tilesets", "0", "SMP3ITEMS.STI");
        await File.WriteAllBytesAsync(sheetPath, new byte[64]);
        File.SetLastWriteTimeUtc(sheetPath, DateTime.UtcNow.AddMinutes(1));

        var changed = await catalog.GetFrameCountAsync(ImageSize.Small, 3);

        Assert.False(changed.IsSuccess);
        Assert.Equal(ErrorCodes.StiBadMagic, changed.ErrorCode);
    }

    /// <summary>
    /// The medium sheet is counted through the same API, while a negative type and a cancelled read are
    /// rejected before touching the file system.
    /// </summary>
    [Fact]
    public async Task Catalog_FrameCount_MediumSheetAndArgumentGuards()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.CopyFixtureDirectory("Graphics");
        var catalog = new ItemImageCatalog(dataDirectory);

        var medium = await catalog.GetFrameCountAsync(ImageSize.Medium, 3);
        Assert.True(medium.IsSuccess, medium.ErrorText);
        Assert.True(medium.Data > 0);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            catalog.GetFrameCountAsync(ImageSize.Small, -1));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            catalog.GetFrameCountAsync(ImageSize.Small, 3, cancellation.Token));
    }

    /// <summary>
    /// The maximum index reports the same missing-sheet failure as the frame count.
    /// </summary>
    [Fact]
    public async Task Catalog_MaxIndex_MissingSheet_ReturnsImageNotFound()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.CopyFixtureDirectory("Graphics");
        var catalog = new ItemImageCatalog(dataDirectory);

        var maximum = await catalog.GetMaxIndexAsync(2);

        Assert.False(maximum.IsSuccess);
        Assert.Equal(ErrorCodes.ImageNotFound, maximum.ErrorCode);
    }
}
