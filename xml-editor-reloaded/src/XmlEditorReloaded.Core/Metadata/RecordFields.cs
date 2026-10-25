namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Reserved field names used by the document model for values that are not child elements.
/// </summary>
public static class RecordFields
{
    /// <summary>
    /// Name of the virtual field that exposes the text of a record element which has no child elements,
    /// for example one <c>&lt;SOUND&gt;path.wav&lt;/SOUND&gt;</c> entry. The name cannot collide with a
    /// real field because such a record has no child elements.
    /// </summary>
    public const string RecordText = "Text";

    /// <summary>
    /// Name of the virtual field that resolves to the record key of the target record rather than to a
    /// child element. Metadata uses it for targets whose identity is positional, for example the
    /// <c>Sounds.Sounds</c> and <c>Sounds.BurstSounds</c> lists where the numeric id is the zero-based
    /// record position. The name cannot collide with a real field because <c>#</c> is not valid in an
    /// XML element name.
    /// </summary>
    public const string RecordKey = "#Key";
}
