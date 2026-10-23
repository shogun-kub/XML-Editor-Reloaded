namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Metadata describing one XML table: file mapping, element names, keys, columns, and list/dict fields.
/// </summary>
public sealed class TableMetadata
{
    private readonly Dictionary<string, ColumnMetadata> _columnsByName;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableMetadata"/> class.
    /// </summary>
    /// <param name="tableName">Dotted table name, for example <c>Items.Items</c>.</param>
    /// <param name="xmlRoot">Expected root element name of the table file.</param>
    /// <param name="recordElement">Name of the element that represents one record.</param>
    /// <param name="isLookup">Whether the table is a lookup table of id/name pairs.</param>
    /// <param name="recordKey">Record key specification used for identity and overlay matching.</param>
    /// <param name="columns">Column metadata; the column list may be empty for document-driven display.</param>
    /// <param name="listFields">Repeated child element definitions.</param>
    /// <param name="dictFields">Indexed child element group definitions.</param>
    /// <param name="translatableFieldPaths">Slash-separated field paths that may appear in language overlays.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tableName"/> is <see langword="null"/>.</exception>
    public TableMetadata(
        TableName tableName,
        string xmlRoot,
        string recordElement,
        bool isLookup,
        RecordKeySpec? recordKey = null,
        IReadOnlyList<ColumnMetadata>? columns = null,
        IReadOnlyList<ListFieldDefinition>? listFields = null,
        IReadOnlyList<DictFieldDefinition>? dictFields = null,
        IReadOnlyList<string>? translatableFieldPaths = null)
    {
        TableName = tableName;
        ArgumentException.ThrowIfNullOrWhiteSpace(xmlRoot);
        ArgumentException.ThrowIfNullOrWhiteSpace(recordElement);
        XmlRoot = xmlRoot;
        RecordElement = recordElement;
        IsLookup = isLookup;
        RecordKey = recordKey;
        Columns = columns ?? Array.Empty<ColumnMetadata>();
        ListFields = listFields ?? Array.Empty<ListFieldDefinition>();
        DictFields = dictFields ?? Array.Empty<DictFieldDefinition>();
        TranslatableFieldPaths = translatableFieldPaths ?? Array.Empty<string>();
        _columnsByName = Columns.ToDictionary(static c => c.XmlFieldName, StringComparer.Ordinal);
    }

    /// <summary>
    /// Gets the dotted table name.
    /// </summary>
    public TableName TableName { get; }

    /// <summary>
    /// Gets the expected root element name of the table file.
    /// </summary>
    public string XmlRoot { get; }

    /// <summary>
    /// Gets the name of the element that represents one record.
    /// </summary>
    public string RecordElement { get; }

    /// <summary>
    /// Gets a value indicating whether the table is a lookup table of id/name pairs.
    /// </summary>
    public bool IsLookup { get; }

    /// <summary>
    /// Gets the record key specification, or <see langword="null"/> when the table has no defined key.
    /// </summary>
    public RecordKeySpec? RecordKey { get; }

    /// <summary>
    /// Gets the column metadata list. May be empty; unknown fields still round-trip via the document model.
    /// </summary>
    public IReadOnlyList<ColumnMetadata> Columns { get; }

    /// <summary>
    /// Gets the repeated child element definitions.
    /// </summary>
    public IReadOnlyList<ListFieldDefinition> ListFields { get; }

    /// <summary>
    /// Gets the indexed child element group definitions.
    /// </summary>
    public IReadOnlyList<DictFieldDefinition> DictFields { get; }

    /// <summary>
    /// Gets the slash-separated field paths that may appear in language overlays.
    /// </summary>
    public IReadOnlyList<string> TranslatableFieldPaths { get; }

    /// <summary>
    /// Attempts to find column metadata by the exact XML element name.
    /// </summary>
    /// <param name="xmlFieldName">Case-sensitive XML element name.</param>
    /// <param name="column">The column metadata when found.</param>
    /// <returns><see langword="true"/> when a column with that name exists; otherwise <see langword="false"/>.</returns>
    public bool TryGetColumn(string xmlFieldName, out ColumnMetadata? column)
    {
        ArgumentException.ThrowIfNullOrEmpty(xmlFieldName);
        if (_columnsByName.TryGetValue(xmlFieldName, out var found))
        {
            column = found;
            return true;
        }

        column = null;
        return false;
    }
}
