using System.Windows.Media;
using System.Windows.Media.Imaging;
using XmlEditorReloaded.Core.Imaging;

namespace XmlEditorReloaded.App.Imaging;

/// <summary>
/// Converts backend RGBA pixel buffers into WPF bitmaps. Pixel conversion and display are WPF-layer
/// responsibilities; the imaging backend never references WPF types.
/// </summary>
public static class RgbaBufferConverter
{
    /// <summary>
    /// Swaps the red and blue channels of a straight RGBA buffer to produce BGRA32 pixels.
    /// </summary>
    /// <param name="buffer">Source RGBA buffer.</param>
    /// <returns>A new byte array with BGRA byte order and the same alpha values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="buffer"/> is null.</exception>
    public static byte[] ToBgra32(RgbaBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        var source = buffer.Pixels;
        var target = new byte[source.Length];
        for (var index = 0; index + 3 < source.Length; index += 4)
        {
            target[index] = source[index + 2];
            target[index + 1] = source[index + 1];
            target[index + 2] = source[index];
            target[index + 3] = source[index + 3];
        }

        return target;
    }

    /// <summary>
    /// Creates a frozen <see cref="BitmapSource"/> with BGRA32 pixels from an RGBA buffer.
    /// </summary>
    /// <param name="buffer">Source RGBA buffer.</param>
    /// <returns>A frozen bitmap that can be used from any thread.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="buffer"/> is null.</exception>
    public static BitmapSource ToBitmapSource(RgbaBuffer buffer)
    {
        var pixels = ToBgra32(buffer);
        var bitmap = BitmapSource.Create(
            buffer.Width,
            buffer.Height,
            96,
            96,
            PixelFormats.Bgra32,
            null,
            pixels,
            buffer.Width * 4);
        bitmap.Freeze();
        return bitmap;
    }
}
