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
}
