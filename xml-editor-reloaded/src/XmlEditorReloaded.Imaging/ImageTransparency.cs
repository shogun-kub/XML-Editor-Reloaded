using XmlEditorReloaded.Core.Imaging;

namespace XmlEditorReloaded.Imaging;

/// <summary>
/// Applies the item-image transparency rules that are not part of the STI format itself.
/// </summary>
public static class ImageTransparency
{
    /// <summary>
    /// Makes the bright-green key color (0, 255, 0) transparent in place.
    /// Used for medium and small item sprites; large item images do not use the green key.
    /// </summary>
    /// <param name="buffer">RGBA buffer to modify.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="buffer"/> is null.</exception>
    public static void ApplyGreenKey(RgbaBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        var pixels = buffer.Pixels;
        for (var offset = 0; offset < pixels.Length; offset += 4)
        {
            if (pixels[offset] == 0 && pixels[offset + 1] == 255 && pixels[offset + 2] == 0)
            {
                pixels[offset + 3] = 0;
            }
        }
    }
}
