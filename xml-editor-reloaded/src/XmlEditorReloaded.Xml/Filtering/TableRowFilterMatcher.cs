using System.Globalization;
using XmlEditorReloaded.Core.Filtering;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Schema;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Xml.Filtering;

/// <summary>
/// Evaluates structured grid row filters against effective row values. The matcher is data-only: it
/// never interprets field names as expressions and never reads or writes the document.
/// </summary>
public static class TableRowFilterMatcher
{
    private const NumberStyles NumericStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    /// <summary>
    /// Tests whether a row satisfies every criterion (AND) or at least one criterion (OR) of a group.
    /// </summary>
    /// <param name="row">Row to evaluate; its effective merged value is used for each field.</param>
    /// <param name="schema">Schema that supplies field types and exact field names.</param>
    /// <param name="group">Filter group; an empty group matches every row.</param>
    /// <returns><see langword="true"/> when the row matches the group; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
    public static bool Matches(WorkspaceRow row, TableSchema schema, RowFilterGroup group)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(group);

        if (group.IsEmpty)
        {
            return true;
        }

        var useAnd = group.Operator == RowFilterLogicalOperator.And;
        foreach (var criterion in group.Criteria)
        {
            var matches = MatchesCriterion(row, schema, criterion);
            if (useAnd && !matches)
            {
                return false;
            }

            if (!useAnd && matches)
            {
                return true;
            }
        }

        return useAnd;
    }

    private static bool MatchesCriterion(
        WorkspaceRow row,
        TableSchema schema,
        RowFilterCriterion criterion)
    {
        var column = schema.FindColumn(criterion.Field);
        if (column is null)
        {
            return false;
        }

        var actual = row.GetValue(column.XmlFieldName);
        if (actual is null)
        {
            // An absent field is not equal to anything, but it does not match an exclusion either.
            return criterion.Operator == RowFilterOperator.NotEquals;
        }

        return criterion.Operator switch
        {
            RowFilterOperator.Equals => ValuesEqual(actual, criterion.Operand, column.DataType),
            RowFilterOperator.NotEquals => !ValuesEqual(actual, criterion.Operand, column.DataType),
            RowFilterOperator.Contains => actual.Contains(criterion.Operand, StringComparison.OrdinalIgnoreCase),
            RowFilterOperator.GreaterThan => CompareNumeric(actual, criterion.Operand, column.DataType, static result => result > 0),
            RowFilterOperator.GreaterOrEqual => CompareNumeric(actual, criterion.Operand, column.DataType, static result => result >= 0),
            RowFilterOperator.LessThan => CompareNumeric(actual, criterion.Operand, column.DataType, static result => result < 0),
            RowFilterOperator.LessOrEqual => CompareNumeric(actual, criterion.Operand, column.DataType, static result => result <= 0),
            _ => false,
        };
    }

    private static bool ValuesEqual(string actual, string operand, ColumnDataType dataType)
    {
        if (IsNumericType(dataType)
            && TryParseNumeric(actual, out var actualNumber)
            && TryParseNumeric(operand, out var operandNumber))
        {
            return actualNumber == operandNumber;
        }

        return string.Equals(actual.Trim(), operand.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool CompareNumeric(
        string actual,
        string operand,
        ColumnDataType dataType,
        Func<int, bool> predicate)
    {
        if (!IsNumericType(dataType)
            || !TryParseNumeric(actual, out var actualNumber)
            || !TryParseNumeric(operand, out var operandNumber))
        {
            return false;
        }

        return predicate(decimal.Compare(actualNumber, operandNumber));
    }

    private static bool IsNumericType(ColumnDataType dataType) =>
        dataType is ColumnDataType.Integer
            or ColumnDataType.Decimal
            or ColumnDataType.Lookup
            or ColumnDataType.Flags;

    private static bool TryParseNumeric(string text, out decimal value) =>
        decimal.TryParse(text, NumericStyles, CultureInfo.InvariantCulture, out value);
}
