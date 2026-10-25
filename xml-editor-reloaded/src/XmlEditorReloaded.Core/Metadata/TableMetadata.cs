namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Metadata describing one XML table: file mapping, element names, keys, columns, and list/dict fields.
/// </summary>
public sealed class TableMetadata
{
    private readonly Dictionary<string, ColumnMetadata> _columnsByName;
    private readonly Dictionary<string, ColumnHint> _hintsByName;
    private readonly Dictionary<(ClassVisibilitySectionKind Kind, string SectionKey), ClassVisibilityRule>
        _classVisibilityBySection;

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
    /// <param name="columnHints">
    /// Optional curated hints applied only to fields that are present in the document.
    /// </param>
    /// <param name="recordKeyFields">
    /// Ordered direct-child XML fields that together form a compound record key. An empty list means
    /// that <paramref name="recordKey"/> or the document-driven fallback identifies records. A compound
    /// key is a separate concept from <see cref="RecordKeySpec"/> segments, which describe nested
    /// collections and singletons.
    /// </param>
    /// <param name="classVisibilityRules">
    /// Optional rules that limit item field groups and related sections to selected item classes. An
    /// empty list keeps every section visible.
    /// </param>
    /// <param name="symmetricKeyPair">
    /// Optional declaration that records are stored in reversed pairs. The workspace maintains the
    /// counterpart during add, delete, and key edits, and validation warns about asymmetric data.
    /// </param>
    /// <param name="exclusiveFlagGroups">
    /// Optional groups of boolean fields where only one field may be set. Validation reports groups
    /// with several set fields but never clears a field.
    /// </param>
    /// <param name="classValueLimits">
    /// Optional limits of indexed fields that depend on a class value, for example the number of
    /// load-bearing pockets allowed by an LBE class. Validation reports values above the limit only.
    /// </param>
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
        IReadOnlyList<string>? translatableFieldPaths = null,
        IReadOnlyList<ColumnHint>? columnHints = null,
        IReadOnlyList<string>? recordKeyFields = null,
        IReadOnlyList<ClassVisibilityRule>? classVisibilityRules = null,
        SymmetricKeyPair? symmetricKeyPair = null,
        IReadOnlyList<ExclusiveFlagGroup>? exclusiveFlagGroups = null,
        IReadOnlyList<ClassValueLimit>? classValueLimits = null)
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
        ColumnHints = columnHints ?? Array.Empty<ColumnHint>();
        RecordKeyFields = recordKeyFields ?? Array.Empty<string>();
        ClassVisibilityRules = classVisibilityRules ?? Array.Empty<ClassVisibilityRule>();
        SymmetricKeyPair = symmetricKeyPair;
        ExclusiveFlagGroups = exclusiveFlagGroups ?? Array.Empty<ExclusiveFlagGroup>();
        ClassValueLimits = classValueLimits ?? Array.Empty<ClassValueLimit>();
        _columnsByName = Columns.ToDictionary(static c => c.XmlFieldName, StringComparer.Ordinal);
        _hintsByName = ColumnHints.ToDictionary(static h => h.XmlFieldName, StringComparer.Ordinal);
        _classVisibilityBySection = ClassVisibilityRules
            .GroupBy(static rule => (rule.Kind, rule.SectionKey))
            .ToDictionary(static group => group.Key, static group => group.First());
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
    /// Gets the ordered direct-child XML fields that together form a compound record key, or an empty
    /// list when the table uses <see cref="RecordKey"/> or document-driven identity.
    /// </summary>
    public IReadOnlyList<string> RecordKeyFields { get; }

    /// <summary>
    /// Gets a value indicating whether records are identified by several fields and no single key can
    /// be assigned automatically.
    /// </summary>
    public bool HasCompoundRecordKey => RecordKeyFields.Count > 1;

    /// <summary>
    /// Gets a value indicating whether the table declares any record identity: a compound key, a key
    /// specification, or both.
    /// </summary>
    public bool HasRecordKey => RecordKeyFields.Count > 0 || RecordKey is not null;

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
    /// Gets the curated hints for fields discovered in the document. Hints never declare fields.
    /// </summary>
    public IReadOnlyList<ColumnHint> ColumnHints { get; }

    /// <summary>
    /// Gets the rules that limit item field groups and related sections to selected item classes. An
    /// empty list means that no section is class-restricted.
    /// </summary>
    public IReadOnlyList<ClassVisibilityRule> ClassVisibilityRules { get; }

    /// <summary>
    /// Gets the declaration that records are stored in reversed key pairs, or <see langword="null"/>
    /// when the table has no such invariant.
    /// </summary>
    public SymmetricKeyPair? SymmetricKeyPair { get; }

    /// <summary>
    /// Gets the groups of boolean fields where at most one field may be set. An empty list means that
    /// no such invariant is declared.
    /// </summary>
    public IReadOnlyList<ExclusiveFlagGroup> ExclusiveFlagGroups { get; }

    /// <summary>
    /// Gets the limits of indexed fields that depend on a class value. An empty list means that no
    /// class-dependent limit is declared.
    /// </summary>
    public IReadOnlyList<ClassValueLimit> ClassValueLimits { get; }

    /// <summary>
    /// Attempts to find a class visibility rule for one section.
    /// </summary>
    /// <param name="kind">Kind of the section.</param>
    /// <param name="sectionKey">
    /// Group key for a group rule; dotted table name for sub-table and child-table rules.
    /// </param>
    /// <param name="rule">The matching rule when found.</param>
    /// <returns><see langword="true"/> when the section is class-restricted; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="sectionKey"/> is empty.</exception>
    public bool TryGetClassVisibilityRule(
        ClassVisibilitySectionKind kind,
        string sectionKey,
        out ClassVisibilityRule? rule)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionKey);
        if (_classVisibilityBySection.TryGetValue((kind, sectionKey), out var found))
        {
            rule = found;
            return true;
        }

        rule = null;
        return false;
    }

    /// <summary>
    /// Attempts to find a curated hint by the exact XML element name.
    /// </summary>
    /// <param name="xmlFieldName">Case-sensitive XML element name.</param>
    /// <param name="hint">The hint when found.</param>
    /// <returns><see langword="true"/> when a hint with that name exists; otherwise <see langword="false"/>.</returns>
    public bool TryGetColumnHint(string xmlFieldName, out ColumnHint? hint)
    {
        ArgumentException.ThrowIfNullOrEmpty(xmlFieldName);
        if (_hintsByName.TryGetValue(xmlFieldName, out var found))
        {
            hint = found;
            return true;
        }

        hint = null;
        return false;
    }

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
