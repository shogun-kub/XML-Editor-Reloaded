namespace XmlEditorReloaded.Imaging.Sti;

/// <summary>
/// Parsed STI container: header, RGB palette, frame directory, and encoded frame data.
/// </summary>
public sealed class StciImage
{
    internal StciImage(
        string filePath,
        StciHeader header,
        byte[] palette,
        IReadOnlyList<SubImageDescriptor> subImages,
        byte[] frameData)
    {
        FilePath = filePath;
        Header = header;
        Palette = palette;
        SubImages = subImages;
        FrameData = frameData;
    }

    /// <summary>
    /// Gets the source file path.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets the STCI header.
    /// </summary>
    public StciHeader Header { get; }

    /// <summary>
    /// Gets the RGB palette, three bytes per entry.
    /// </summary>
    public byte[] Palette { get; }

    /// <summary>
    /// Gets the frame directory in file order.
    /// </summary>
    public IReadOnlyList<SubImageDescriptor> SubImages { get; }

    /// <summary>
    /// Gets the encoded frame data following the directory.
    /// </summary>
    public byte[] FrameData { get; }
}
