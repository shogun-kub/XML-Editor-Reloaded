using XmlEditorReloaded.Core.Imaging;

namespace XmlEditorReloaded.Imaging.Tests;

/// <summary>
/// Tests for <see cref="ImageTransparency"/>.
/// </summary>
public class ImageTransparencyTests
{
    /// <summary>
    /// The green key becomes transparent while other colors are untouched.
    /// </summary>
    [Fact]
    public void ApplyGreenKey_OnlyClearsExactGreen()
    {
        var pixels = new byte[]
        {
            0, 255, 0, 255,
            1, 255, 0, 255,
            0, 254, 0, 255,
        };
        var buffer = new RgbaBuffer(3, 1, pixels);

        ImageTransparency.ApplyGreenKey(buffer);

        Assert.Equal(0, buffer.Pixels[3]);
        Assert.Equal(255, buffer.Pixels[7]);
        Assert.Equal(255, buffer.Pixels[11]);
    }
}
