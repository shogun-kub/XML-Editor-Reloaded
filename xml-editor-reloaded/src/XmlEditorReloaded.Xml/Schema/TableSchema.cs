using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Xml.Schema;

/// <summary>
/// The editable shape of a table: a union of all record fields in first-appearance order,
/// enriched with optional column metadata. The schema never changes the underlying records.
/// </summary>
public sealed class TableSchema
{
    private readonly Dictionary<string, TableSchemaColumn> _columnsByName;

    internal TableSchema(
        TableName tableName,
        string recordElement,
        int recordCount,
        bool isDocumentDriven,
        IReadOnlyList<TableSchemaColumn> columns)
    {
        TableName = tableName;
        RecordElement = recordElement;
        RecordCount = recordCount;
        IsDocumentDriven = isDocumentDriven;
        Columns = columns;
        _columnsByName = columns.ToDictionary(static column => column.XmlFieldName, StringComparer.Ordinal);
        KeyColumns = columns.Where(static column => column.IsKey).ToArray();
        KeyColumn = KeyColumns.Count > 0 ? KeyColumns[0] : null;
    }

    /// <summary>
    /// Gets the table name.
    /// </summary>
    public TableName TableName { get; }

    /// <summary>
    /// Gets the record element name, for example <c>ITEM</c>.
    /// </summary>
    public string RecordElement { get; }

    /// <summary>
    /// Gets the number of records the schema was built from.
    /// </summary>
    public int RecordCount { get; }

    /// <summary>
    /// Gets a value indicating whether the schema relies on document discovery rather than curated columns.
    /// </summary>
    public bool IsDocumentDriven { get; }

    /// <summary>
    /// Gets the columns in display order.
    /// </summary>
    public IReadOnlyList<TableSchemaColumn> Columns { get; }

    /// <summary>
    /// Gets the column that identifies records, or <see langword="null"/> when none was determined.
    /// </summary>
    public TableSchemaColumn? KeyColumn { get; }

    /// <summary>
    /// Gets every column that participates in record identity, in schema order. A compound-key table
    /// has more than one entry; single-key and document-driven tables have one.
    /// </summary>
    public IReadOnlyList<TableSchemaColumn> KeyColumns { get; }

    /// <summary>
    /// Gets a value indicating whether record identity requires several key columns.
    /// </summary>
    public bool HasCompoundKey => KeyColumns.Count > 1;

    /// <summary>
    /// Finds a column by its exact XML element name.
    /// </summary>
    /// <param name="xmlFieldName">XML element name, matched case-sensitively.</param>
    /// <returns>The column, or <see langword="null"/> when the field is not part of the schema.</returns>
    public TableSchemaColumn? FindColumn(string xmlFieldName) =>
        _columnsByName.TryGetValue(xmlFieldName, out var column) ? column : null;
}
