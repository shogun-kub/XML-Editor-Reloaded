using System.Globalization;

namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Operators supported by <see cref="RelationFilter"/>. The set is intentionally small and
/// data-driven: relations never evaluate SQL or arbitrary expressions.
/// </summary>
public enum RelationFilterOperator
{
    /// <summary>Matches when the field value equals the filter value.</summary>
    Equals,

    /// <summary>Matches when the field value differs from the filter value or is absent.</summary>
    NotEquals,

    /// <summary>Matches when the field value contains the filter value as a substring.</summary>
    Contains,

    /// <summary>Matches when the numeric field value has at least one bit of the filter mask set.</summary>
    HasFlag,
}

/// <summary>
/// One structured predicate applied to target records when a relation builds its picker options.
/// Filters only narrow the offered options; they never reject values that already exist in the
/// source document or change what is written back.
/// </summary>
public sealed class RelationFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelationFilter"/> class.
    /// </summary>
    /// <param name="field">Target field name, matched case-sensitively.</param>
    /// <param name="operator">Comparison operator.</param>
    /// <param name="value">Filter operand in invariant lexical form.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="field"/> or <paramref name="value"/> is empty.
    /// </exception>
    public RelationFilter(string field, RelationFilterOperator @operator, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(field);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Field = field;
        Operator = @operator;
        Value = value;
    }

    /// <summary>
    /// Gets the target field name, matched case-sensitively.
    /// </summary>
    public string Field { get; }

    /// <summary>
    /// Gets the comparison operator.
    /// </summary>
    public RelationFilterOperator Operator { get; }

    /// <summary>
    /// Gets the filter operand in invariant lexical form.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Evaluates the predicate against one target record value.
    /// </summary>
    /// <param name="actualValue">Raw field value, or <see langword="null"/> when the field is absent.</param>
    /// <returns><see langword="true"/> when the record matches the predicate; otherwise <see langword="false"/>.</returns>
    public bool Matches(string? actualValue)
    {
        var actual = actualValue?.Trim();
        var isEmpty = string.IsNullOrEmpty(actual);
        switch (Operator)
        {
            case RelationFilterOperator.Equals:
                return !isEmpty && ValuesEqual(actual!, Value);
            case RelationFilterOperator.NotEquals:
                return isEmpty || !ValuesEqual(actual!, Value);
            case RelationFilterOperator.Contains:
                return !isEmpty && actual!.Contains(Value, StringComparison.OrdinalIgnoreCase);
            case RelationFilterOperator.HasFlag:
                return TryParseUnsigned(actual, out var actualMask)
                    && TryParseUnsigned(Value, out var requiredMask)
                    && requiredMask != 0
                    && (actualMask & requiredMask) != 0;
            default:
                return false;
        }
    }

    private static bool ValuesEqual(string left, string right)
    {
        if (TryParseSigned(left, out var leftNumber) && TryParseSigned(right, out var rightNumber))
        {
            return leftNumber == rightNumber;
        }

        if (TryParseUnsigned(left, out var leftMask) && TryParseUnsigned(right, out var rightMask))
        {
            return leftMask == rightMask;
        }

        return string.Equals(left.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryParseSigned(string? text, out long value) =>
        long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

    private static bool TryParseUnsigned(string? text, out ulong value) =>
        ulong.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
}
