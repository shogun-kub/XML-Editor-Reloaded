namespace XmlEditorReloaded.Core.Imaging;

/// <summary>
/// Size class of an item image sprite sheet.
/// </summary>
public enum ImageSize
{
    /// <summary>Large per-item sprite from <c>BigItems</c>.</summary>
    Big,

    /// <summary>Medium sprite sheet frame from <c>Interface</c>.</summary>
    Medium,

    /// <summary>Small sprite sheet frame from <c>tilesets/0</c>.</summary>
    Small,
}

/// <summary>
/// Raw straight (non-premultiplied) RGBA pixel buffer produced by the imaging backend.
/// </summary>
public sealed class RgbaBuffer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RgbaBuffer"/> class.
    /// </summary>
    /// <param name="width">Image width in pixels.</param>
    /// <param name="height">Image height in pixels.</param>
    /// <param name="pixels">Pixel data in row-major order, four bytes (R, G, B, A) per pixel.</param>
    /// <exception cref="ArgumentException">Thrown when the buffer length does not match the dimensions.</exception>
    public RgbaBuffer(int width, int height, byte[] pixels)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        ArgumentNullException.ThrowIfNull(pixels);
        if (pixels.Length != checked(width * height * 4))
        {
            throw new ArgumentException(
                $"Pixel buffer length {pixels.Length} does not match {width}x{height} RGBA ({checked(width * height * 4)} bytes).",
                nameof(pixels));
        }

        Width = width;
        Height = height;
        Pixels = pixels;
    }

    /// <summary>
    /// Gets the image width in pixels.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the image height in pixels.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the pixel data in row-major order, four bytes (R, G, B, A) per pixel.
    /// </summary>
    public byte[] Pixels { get; }
}
