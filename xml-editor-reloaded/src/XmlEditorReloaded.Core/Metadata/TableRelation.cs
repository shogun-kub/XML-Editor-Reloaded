namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// A metadata-driven link between two tables. Relations replace hardcoded table-specific pickers:
/// they describe which source field references which target key, how the target is displayed, and
/// which optional structured filter narrows the offered target rows.
/// </summary>
public sealed class TableRelation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableRelation"/> class.
    /// </summary>
    /// <param name="sourceTable">Table that declares the relation and contains the source field.</param>
    /// <param name="sourceField">Source field name, matched case-sensitively.</param>
    /// <param name="targetTable">Table whose records are referenced or listed.</param>
    /// <param name="kind">Relation kind.</param>
    /// <param name="targetValueField">Target field that stores the referenced key.</param>
    /// <param name="targetDisplayField">
    /// Target field shown in pickers, or <see langword="null"/> to show the target value itself.
    /// </param>
    /// <param name="filter">Optional structured filter that narrows the picker options.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="sourceField"/> or <paramref name="targetValueField"/> is empty.
    /// </exception>
    public TableRelation(
        TableName sourceTable,
        string sourceField,
        TableName targetTable,
        TableRelationKind kind = TableRelationKind.Reference,
        string targetValueField = "uiIndex",
        string? targetDisplayField = null,
        RelationFilter? filter = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceField);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetValueField);

        SourceTable = sourceTable;
        SourceField = sourceField;
        TargetTable = targetTable;
        Kind = kind;
        TargetValueField = targetValueField;
        TargetDisplayField = string.IsNullOrWhiteSpace(targetDisplayField) ? null : targetDisplayField;
        Filter = filter;
    }

    /// <summary>
    /// Gets the table that declares the relation and contains the source field.
    /// </summary>
    public TableName SourceTable { get; }

    /// <summary>
    /// Gets the source field name, matched case-sensitively.
    /// </summary>
    public string SourceField { get; }

    /// <summary>
    /// Gets the table whose records are referenced or listed.
    /// </summary>
    public TableName TargetTable { get; }

    /// <summary>
    /// Gets the relation kind.
    /// </summary>
    public TableRelationKind Kind { get; }

    /// <summary>
    /// Gets the target field that stores the referenced key.
    /// </summary>
    public string TargetValueField { get; }

    /// <summary>
    /// Gets the target field shown in pickers, or <see langword="null"/> to show the target value.
    /// </summary>
    public string? TargetDisplayField { get; }

    /// <summary>
    /// Gets the optional structured filter that narrows the picker options.
    /// </summary>
    public RelationFilter? Filter { get; }

    /// <summary>
    /// Gets the resource key of the relation label, or <see langword="null"/>.
    /// </summary>
    public string? LabelKey { get; init; }

    /// <summary>
    /// Gets a value indicating whether copy operations may remap this relation by display text when
    /// the target identifier differs between data sets.
    /// </summary>
    public bool RemapByText { get; init; }

    /// <summary>
    /// Gets the display order of the relation; smaller values are shown first.
    /// </summary>
    public int Ordinal { get; init; }
}
