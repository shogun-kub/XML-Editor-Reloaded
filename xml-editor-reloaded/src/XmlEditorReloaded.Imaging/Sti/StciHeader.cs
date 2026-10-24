namespace XmlEditorReloaded.Imaging.Sti;

/// <summary>
/// The 64-byte STCI main header.
/// </summary>
/// <param name="OriginalSize">Uncompressed image data size from the header.</param>
/// <param name="StoredSize">Stored data size from the header.</param>
/// <param name="TransparentValue">Palette index or color marked transparent by the authoring tool.</param>
/// <param name="Flags">Image format flags.</param>
/// <param name="Height">Canvas height in pixels.</param>
/// <param name="Width">Canvas width in pixels.</param>
/// <param name="NumberOfColors">Number of palette entries; zero means an RGB image.</param>
/// <param name="NumberOfSubImages">Number of frames in the frame directory.</param>
/// <param name="Depth">Color depth byte from the header.</param>
/// <param name="RedDepth">Red channel depth byte.</param>
/// <param name="GreenDepth">Green channel depth byte.</param>
/// <param name="BlueDepth">Blue channel depth byte.</param>
/// <param name="AppDataSize">Application-data size stored at offset 48.</param>
public sealed record StciHeader(
    int OriginalSize,
    int StoredSize,
    uint TransparentValue,
    StciFlags Flags,
    int Height,
    int Width,
    int NumberOfColors,
    int NumberOfSubImages,
    byte Depth,
    byte RedDepth,
    byte GreenDepth,
    byte BlueDepth,
    int AppDataSize);

/// <summary>
/// One entry of the STI frame directory.
/// </summary>
/// <param name="DataOffset">Offset of the frame data relative to the start of the frame-data area.</param>
/// <param name="DataLength">Encoded frame length in bytes.</param>
/// <param name="OffsetX">Horizontal placement offset.</param>
/// <param name="OffsetY">Vertical placement offset.</param>
/// <param name="Height">Frame height in pixels.</param>
/// <param name="Width">Frame width in pixels.</param>
public sealed record SubImageDescriptor(
    int DataOffset,
    int DataLength,
    short OffsetX,
    short OffsetY,
    int Height,
    int Width);
