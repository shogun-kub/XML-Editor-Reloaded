using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Xml.Schema;

/// <summary>
/// One field of a table as seen by the editor: a union of the document's sparse records combined
/// with optional curated column metadata.
/// </summary>
public sealed class TableSchemaColumn
{
    internal TableSchemaColumn(
        string xmlFieldName,
        ColumnDataType dataType,
        ColumnEditorKind editorKind,
        int order,
        int presenceCount,
        int maxOccurrences,
        bool isKey,
        bool isList,
        bool isDict,
        string? dictMask,
        bool isTranslatable,
        bool isHidden,
        int? displayWidth,
        string? lookupTable,
        string? tooltipKey,
        ColumnFlags flags,
        bool isNested,
        string groupKey,
        int groupOrder)
    {
        XmlFieldName = xmlFieldName;
        DataType = dataType;
        EditorKind = editorKind;
        Order = order;
        PresenceCount = presenceCount;
        MaxOccurrences = maxOccurrences;
        IsKey = isKey;
        IsList = isList;
        IsDict = isDict;
        DictMask = dictMask;
        IsTranslatable = isTranslatable;
        IsHidden = isHidden;
        DisplayWidth = displayWidth;
        LookupTable = lookupTable;
        TooltipKey = tooltipKey;
        Flags = flags;
        IsNested = isNested;
        GroupKey = groupKey;
        GroupOrder = groupOrder;
    }

    /// <summary>
    /// Gets the XML element name of the field, preserving its original casing.
    /// </summary>
    public string XmlFieldName { get; }

    /// <summary>
    /// Gets the field type, taken from column metadata when present and inferred from values otherwise.
    /// </summary>
    public ColumnDataType DataType { get; }

    /// <summary>
    /// Gets the preferred editor, taken from column metadata or derived from the inferred type.
    /// </summary>
    public ColumnEditorKind EditorKind { get; }

    /// <summary>
    /// Gets the zero-based position in the schema, based on the first appearance of the field.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Gets the number of records that contain at least one element with this name.
    /// </summary>
    public int PresenceCount { get; }

    /// <summary>
    /// Gets the largest number of repetitions of this element found in a single record.
    /// </summary>
    public int MaxOccurrences { get; }

    /// <summary>
    /// Gets a value indicating whether this field identifies a record.
    /// </summary>
    public bool IsKey { get; }

    /// <summary>
    /// Gets a value indicating whether the field repeats inside a record and is therefore a list.
    /// </summary>
    public bool IsList { get; }

    /// <summary>
    /// Gets a value indicating whether the field belongs to an index-masked dictionary.
    /// </summary>
    public bool IsDict { get; }

    /// <summary>
    /// Gets the dictionary mask, for example <c>ItemCapacityPerSize%Index%</c>, or <see langword="null"/>.
    /// </summary>
    public string? DictMask { get; }

    /// <summary>
    /// Gets a value indicating whether the field can be translated through a language overlay.
    /// </summary>
    public bool IsTranslatable { get; }

    /// <summary>
    /// Gets a value indicating whether the field should be hidden from the default grid view.
    /// </summary>
    public bool IsHidden { get; }

    /// <summary>
    /// Gets the preferred display width in pixels, or <see langword="null"/> when unspecified.
    /// </summary>
    public int? DisplayWidth { get; }

    /// <summary>
    /// Gets the lookup table used by the field, or <see langword="null"/> when the field is not a lookup.
    /// </summary>
    public string? LookupTable { get; }

    /// <summary>
    /// Gets the resource key of the field tooltip, or <see langword="null"/>.
    /// </summary>
    public string? TooltipKey { get; }

    /// <summary>
    /// Gets the curated column flags, enriched with <see cref="ColumnFlags.Translatable"/> and
    /// <see cref="ColumnFlags.Hidden"/> when they were derived from the schema.
    /// </summary>
    public ColumnFlags Flags { get; }

    /// <summary>
    /// Gets a value indicating whether the field contains nested elements and must be edited by a
    /// dedicated sub-table editor rather than as scalar text.
    /// </summary>
    public bool IsNested { get; }

    /// <summary>
    /// Gets the stable group key used by the detail form, for example <c>attachments</c>.
    /// </summary>
    public string GroupKey { get; }

    /// <summary>
    /// Gets the group display order; smaller values are shown first.
    /// </summary>
    public int GroupOrder { get; }
}
