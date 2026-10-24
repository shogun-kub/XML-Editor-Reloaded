namespace XmlEditorReloaded.Core.Filtering;

/// <summary>
/// One structured predicate applied to a grid row. Field names and operands are matched without
/// culture-dependent conversions; the evaluation semantics live in the XML layer.
/// </summary>
public sealed class RowFilterCriterion
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RowFilterCriterion"/> class.
    /// </summary>
    /// <param name="field">XML field name, matched case-sensitively against the table schema.</param>
    /// <param name="operator">Comparison operator.</param>
    /// <param name="operand">Value to compare with; may be empty to match an empty field value.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="field"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="operand"/> is null.</exception>
    public RowFilterCriterion(string field, RowFilterOperator @operator, string operand)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        ArgumentNullException.ThrowIfNull(operand);

        Field = field;
        Operator = @operator;
        Operand = operand;
    }

    /// <summary>
    /// Gets the XML field name, matched case-sensitively against the table schema.
    /// </summary>
    public string Field { get; }

    /// <summary>
    /// Gets the comparison operator.
    /// </summary>
    public RowFilterOperator Operator { get; }

    /// <summary>
    /// Gets the value to compare with; may be empty to match an empty field value.
    /// </summary>
    public string Operand { get; }
}
