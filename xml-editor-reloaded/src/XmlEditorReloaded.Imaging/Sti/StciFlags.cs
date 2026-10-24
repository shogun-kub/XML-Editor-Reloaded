namespace XmlEditorReloaded.Imaging.Sti;

/// <summary>
/// Flags stored in the STCI header.
/// </summary>
[Flags]
public enum StciFlags : uint
{
    /// <summary>No flags.</summary>
    None = 0,

    /// <summary>The image uses an indexed palette.</summary>
    Indexed = 0x08,

    /// <summary>Frame data is compressed with ETRLE.</summary>
    Etrle = 0x20,

    /// <summary>The supported combination for item images.</summary>
    Supported = Indexed | Etrle,
}
