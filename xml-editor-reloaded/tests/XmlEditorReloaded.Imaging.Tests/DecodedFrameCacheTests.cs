using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Imaging.Cache;

namespace XmlEditorReloaded.Imaging.Tests;

/// <summary>
/// Tests for <see cref="DecodedFrameCache"/>.
/// </summary>
public class DecodedFrameCacheTests
{
    /// <summary>
    /// Cached frames are returned for the same modification time and evicted after it changes.
    /// </summary>
    [Fact]
    public void Cache_InvalidatesByModificationTime()
    {
        var cache = new DecodedFrameCache(1024);
        var buffer = new RgbaBuffer(1, 1, new byte[4]);
        var modified = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        cache.Set("frame", buffer, modified);

        Assert.True(cache.TryGet("frame", modified, out var cached));
        Assert.Same(buffer, cached);
        Assert.False(cache.TryGet("frame", modified.AddSeconds(1), out _));
        Assert.Equal(0, cache.Count);
    }

    /// <summary>
    /// The least recently used frame is evicted when the capacity is exceeded.
    /// </summary>
    [Fact]
    public void Cache_EvictsLeastRecentlyUsedFrame()
    {
        var cache = new DecodedFrameCache(32);
        var modified = DateTime.UtcNow;
        var first = new RgbaBuffer(2, 2, new byte[16]);
        var second = new RgbaBuffer(2, 2, new byte[16]);
        var third = new RgbaBuffer(2, 2, new byte[16]);

        cache.Set("first", first, modified);
        cache.Set("second", second, modified);
        cache.Set("third", third, modified);

        Assert.False(cache.TryGet("first", modified, out _));
        Assert.True(cache.TryGet("second", modified, out _));
        Assert.True(cache.TryGet("third", modified, out _));
        Assert.Equal(32, cache.TotalBytes);
    }

    /// <summary>
    /// A frame larger than the whole cache is not stored.
    /// </summary>
    [Fact]
    public void Cache_DoesNotStoreFrameLargerThanCapacity()
    {
        var cache = new DecodedFrameCache(8);
        var buffer = new RgbaBuffer(2, 2, new byte[16]);

        cache.Set("large", buffer, DateTime.UtcNow);

        Assert.Equal(0, cache.Count);
        Assert.False(cache.TryGet("large", DateTime.UtcNow, out _));
    }
}
