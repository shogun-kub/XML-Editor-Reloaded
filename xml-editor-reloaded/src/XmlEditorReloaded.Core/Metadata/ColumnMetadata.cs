namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Metadata describing one editable or displayable column of a table.
/// </summary>
public sealed class ColumnMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnMetadata"/> class.
    /// </summary>
    /// <param name="xmlFieldName">XML element name of the field, case-sensitive.</param>
    /// <param name="dataType">Value type used for parsing and validation.</param>
    /// <param name="editorKind">Preferred editor widget for the field.</param>
    public ColumnMetadata(string xmlFieldName, ColumnDataType dataType, ColumnEditorKind editorKind = ColumnEditorKind.TextBox)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xmlFieldName);
        XmlFieldName = xmlFieldName;
        DataType = dataType;
        EditorKind = editorKind;
    }

    /// <summary>
    /// Gets the XML element name of the field, case-sensitive.
    /// </summary>
    public string XmlFieldName { get; }

    /// <summary>
    /// Gets the value type used for parsing and validation.
    /// </summary>
    public ColumnDataType DataType { get; }

    /// <summary>
    /// Gets the preferred editor widget for the field.
    /// </summary>
    public ColumnEditorKind EditorKind { get; }

    /// <summary>
    /// Gets or sets the suggested display width in characters; <see langword="null"/> means automatic.
    /// </summary>
    public int? DisplayWidth { get; init; }

    /// <summary>
    /// Gets or sets the lookup table name referenced by this column, for example <c>Lookup.ItemClass</c>.
    /// </summary>
    public string? LookupTable { get; init; }

    /// <summary>
    /// Gets or sets the resource key of a tooltip describing the field.
    /// </summary>
    public string? TooltipKey { get; init; }

    /// <summary>
    /// Gets or sets behavior flags of the column.
    /// </summary>
    public ColumnFlags Flags { get; init; }
}

/// <summary>
/// Value type of a table column.
/// </summary>
public enum ColumnDataType
{
    /// <summary>Arbitrary text.</summary>
    String,

    /// <summary>Signed 32-bit integer written without a decimal point.</summary>
    Integer,

    /// <summary>Decimal number; lexical form must be preserved exactly.</summary>
    Decimal,

    /// <summary>Boolean written as <c>0</c> or <c>1</c>.</summary>
    Boolean,

    /// <summary>Integer referring to a lookup table.</summary>
    Lookup,

    /// <summary>Integer used as a bit-flag mask.</summary>
    Flags,

    /// <summary>Nested element structure with its own schema.</summary>
    Nested,

    /// <summary>Opaque value whose type is unknown; always treated as text.</summary>
    Raw,
}

/// <summary>
/// Preferred editor widget for a table column.
/// </summary>
public enum ColumnEditorKind
{
    /// <summary>Plain text box.</summary>
    TextBox,

    /// <summary>Integer input.</summary>
    IntegerBox,

    /// <summary>Decimal input.</summary>
    DecimalBox,

    /// <summary>Combo box bound to a lookup table.</summary>
    LookupCombo,

    /// <summary>Bit-flag checkbox editor.</summary>
    FlagEditor,

    /// <summary>Grid over a nested sub-table.</summary>
    SubTableGrid,

    /// <summary>Not shown in the UI.</summary>
    Hidden,
}

/// <summary>
/// Behavior flags of a table column.
/// </summary>
[Flags]
public enum ColumnFlags
{
    /// <summary>No special behavior.</summary>
    None = 0,

    /// <summary>The field may appear in language overlay files and can be translated.</summary>
    Translatable = 1 << 0,

    /// <summary>The column is hidden by default.</summary>
    Hidden = 1 << 1,

    /// <summary>The column is used as the comparison identity when copying rows between data sets.</summary>
    ComparisonField = 1 << 2,

    /// <summary>Duplicating a row must not carry this column's value semantics blindly; guarded by the UI.</summary>
    NoRowDuplicate = 1 << 3,

    /// <summary>Other tables may reference this value; deletion and copy require referential checks.</summary>
    RefRequired = 1 << 4,
}
