namespace XmlEditorReloaded.Core.Api;

/// <summary>
/// Well-known machine-readable error codes used by operation results across the backend.
/// </summary>
public static class ErrorCodes
{
    /// <summary>The XML file could not be parsed and is considered malformed.</summary>
    public const string XmlParseError = "xml.parse_error";

    /// <summary>The file changed on disk after it was loaded; saving is refused to avoid data loss.</summary>
    public const string XmlFileChangedExternally = "xml.file_changed_externally";

    /// <summary>A numeric value supplied by the caller could not be parsed as a valid number.</summary>
    public const string XmlInvalidNumber = "xml.invalid_number";

    /// <summary>The document is not saveable, for example because it failed to parse.</summary>
    public const string XmlNotSaveable = "xml.not_saveable";

    /// <summary>The requested table is not present in the metadata store.</summary>
    public const string MetadataNotFound = "metadata.not_found";

    /// <summary>The file or directory does not exist.</summary>
    public const string IoFileNotFound = "io.file_not_found";

    /// <summary>Access to the file or directory was denied.</summary>
    public const string IoAccessDenied = "io.access_denied";

    /// <summary>An unexpected I/O failure occurred.</summary>
    public const string IoError = "io.error";

    /// <summary>The stream does not start with the expected <c>STCI</c> magic.</summary>
    public const string StiBadMagic = "sti.bad_magic";

    /// <summary>The STI file ends before a required structure could be read.</summary>
    public const string StiTruncated = "sti.truncated";

    /// <summary>The STI file uses compression or flags that the decoder does not support.</summary>
    public const string StiUnsupportedFlags = "sti.unsupported_flags";

    /// <summary>The requested item image file or frame could not be found.</summary>
    public const string ImageNotFound = "image.not_found";

    /// <summary>The requested record key does not exist in the document.</summary>
    public const string KeyNotFound = "key.not_found";

    /// <summary>The overlay record key could not be resolved from metadata.</summary>
    public const string OverlayUnresolvableKey = "overlay.unresolvable_key";
}
