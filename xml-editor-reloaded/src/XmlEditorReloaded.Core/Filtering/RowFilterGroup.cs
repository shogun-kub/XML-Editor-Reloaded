namespace XmlEditorReloaded.Core.Filtering;

/// <summary>
/// A flat group of row filter criteria combined with AND or OR. An empty group matches every row.
/// </summary>
public sealed class RowFilterGroup
{
    /// <summary>
    /// Gets the empty group that matches every row.
    /// </summary>
    public static RowFilterGroup Empty { get; } = new(
        RowFilterLogicalOperator.And,
        Array.Empty<RowFilterCriterion>());

    /// <summary>
    /// Initializes a new instance of the <see cref="RowFilterGroup"/> class.
    /// </summary>
    /// <param name="operator">How individual criteria are combined.</param>
    /// <param name="criteria">Criteria in display order; <see langword="null"/> means no criteria.</param>
    public RowFilterGroup(
        RowFilterLogicalOperator @operator,
        IReadOnlyList<RowFilterCriterion>? criteria)
    {
        Operator = @operator;
        Criteria = criteria is null
            ? Array.Empty<RowFilterCriterion>()
            : criteria.Where(static criterion => criterion is not null).ToArray();
    }

    /// <summary>
    /// Gets how individual criteria are combined.
    /// </summary>
    public RowFilterLogicalOperator Operator { get; }

    /// <summary>
    /// Gets the criteria in display order.
    /// </summary>
    public IReadOnlyList<RowFilterCriterion> Criteria { get; }

    /// <summary>
    /// Gets a value indicating whether the group contains no criteria and therefore matches every row.
    /// </summary>
    public bool IsEmpty => Criteria.Count == 0;
}
