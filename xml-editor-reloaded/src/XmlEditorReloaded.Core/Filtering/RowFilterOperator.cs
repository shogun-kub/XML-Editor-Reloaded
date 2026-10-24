namespace XmlEditorReloaded.Core.Filtering;

/// <summary>
/// Comparison operators available to a grid row filter. The set is intentionally small: filters
/// never evaluate arbitrary expressions or code.
/// </summary>
public enum RowFilterOperator
{
    /// <summary>Matches when the field value equals the operand.</summary>
    Equals,

    /// <summary>Matches when the field value differs from the operand or the field is absent.</summary>
    NotEquals,

    /// <summary>Matches when the field value contains the operand as a substring.</summary>
    Contains,

    /// <summary>Matches when the numeric field value is greater than the operand.</summary>
    GreaterThan,

    /// <summary>Matches when the numeric field value is greater than or equal to the operand.</summary>
    GreaterOrEqual,

    /// <summary>Matches when the numeric field value is less than the operand.</summary>
    LessThan,

    /// <summary>Matches when the numeric field value is less than or equal to the operand.</summary>
    LessOrEqual,
}

/// <summary>
/// How the criteria of a <see cref="RowFilterGroup"/> are combined.
/// </summary>
public enum RowFilterLogicalOperator
{
    /// <summary>All criteria must match.</summary>
    And,

    /// <summary>At least one criterion must match.</summary>
    Or,
}
