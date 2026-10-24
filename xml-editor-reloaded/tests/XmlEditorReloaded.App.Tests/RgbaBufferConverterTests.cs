using XmlEditorReloaded.App.Imaging;
using XmlEditorReloaded.Core.Imaging;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for <see cref="RgbaBufferConverter"/>.
/// </summary>
public class RgbaBufferConverterTests
{
    /// <summary>
    /// RGBA input becomes BGRA32 with the alpha channel unchanged.
    /// </summary>
    [Fact]
    public void ToBgra32_SwapsRedAndBlueChannels()
    {
        var buffer = new RgbaBuffer(1, 1, new byte[] { 10, 20, 30, 40 });

        var pixels = RgbaBufferConverter.ToBgra32(buffer);

        Assert.Equal(new byte[] { 30, 20, 10, 40 }, pixels);
    }

    /// <summary>
    /// The produced bitmap keeps the source dimensions and is frozen for cross-thread use.
    /// </summary>
    [Fact]
    public void ToBitmapSource_PreservesDimensionsAndFreezes()
    {
        var buffer = new RgbaBuffer(2, 1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

        var bitmap = RgbaBufferConverter.ToBitmapSource(buffer);

        Assert.Equal(2, bitmap.PixelWidth);
        Assert.Equal(1, bitmap.PixelHeight);
        Assert.True(bitmap.IsFrozen);
    }
}
