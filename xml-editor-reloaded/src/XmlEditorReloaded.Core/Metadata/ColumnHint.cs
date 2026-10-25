namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Optional curated hints for one XML field that is discovered from the document. Unlike
/// <see cref="ColumnMetadata"/>, a hint never declares a field that is absent from the document; it
/// only refines how a present field is typed, edited, and grouped.
/// </summary>
public sealed class ColumnHint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnHint"/> class.
    /// </summary>
    /// <param name="xmlFieldName">XML element name of the field, case-sensitive.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="xmlFieldName"/> is empty.</exception>
    public ColumnHint(string xmlFieldName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xmlFieldName);
        XmlFieldName = xmlFieldName;
    }

    /// <summary>
    /// Gets the XML element name of the field, case-sensitive.
    /// </summary>
    public string XmlFieldName { get; }

    /// <summary>
    /// Gets the value type override, or <see langword="null"/> to keep the inferred type.
    /// </summary>
    public ColumnDataType? DataType { get; init; }

    /// <summary>
    /// Gets the preferred editor override, or <see langword="null"/> to derive it from the type.
    /// </summary>
    public ColumnEditorKind? EditorKind { get; init; }

    /// <summary>
    /// Gets the suggested display width in pixels, or <see langword="null"/> when unspecified.
    /// </summary>
    public int? DisplayWidth { get; init; }

    /// <summary>
    /// Gets the lookup table referenced by the field, or <see langword="null"/> when it is not a lookup.
    /// </summary>
    public string? LookupTable { get; init; }

    /// <summary>
    /// Gets the resource key of the field tooltip, or <see langword="null"/>.
    /// </summary>
    public string? TooltipKey { get; init; }

    /// <summary>
    /// Gets additional behavior flags, or <see langword="null"/> when none are curated.
    /// </summary>
    public ColumnFlags? Flags { get; init; }

    /// <summary>
    /// Gets the stable group key used by the detail form, for example <c>attachments</c>, or
    /// <see langword="null"/> to use the fallback group derived from the field type.
    /// </summary>
    public string? GroupKey { get; init; }

    /// <summary>
    /// Gets the group display order; smaller values are shown first. Zero means unspecified.
    /// </summary>
    public int GroupOrder { get; init; }

    /// <summary>
    /// Gets the inclusive lower bound of a numeric field, or <see langword="null"/> when unbounded.
    /// The bound is used by validation only and never clamps an edited value.
    /// </summary>
    public long? Minimum { get; init; }

    /// <summary>
    /// Gets the inclusive upper bound of a numeric field, or <see langword="null"/> when unbounded.
    /// The bound is used by validation only and never clamps an edited value.
    /// </summary>
    public long? Maximum { get; init; }
}
