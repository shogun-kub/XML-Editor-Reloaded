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
}
