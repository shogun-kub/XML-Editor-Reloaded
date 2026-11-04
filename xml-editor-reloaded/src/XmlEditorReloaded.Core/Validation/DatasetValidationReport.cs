using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Validation;

/// <summary>
/// Outcome of validating one table of a data set.
/// </summary>
public enum TableValidationState
{
    /// <summary>The table was parsed and validated.</summary>
    Validated,

    /// <summary>The table file is not well-formed XML; details come from load diagnostics.</summary>
    Malformed,

    /// <summary>The table file could not be read at all.</summary>
    Unreadable,
}

/// <summary>
/// Validation result of one table, including exact severity counts and the capped issue list.
/// </summary>
/// <param name="TableName">Dotted name of the table.</param>
/// <param name="RelativePath">Path relative to the <c>TableData</c> directory.</param>
/// <param name="IsMapped">Whether table metadata was available for the load.</param>
/// <param name="State">Whether the table was validated, malformed, or unreadable.</param>
/// <param name="RecordCount">Number of records found in the document, or zero when it was not parsed.</param>
/// <param name="Issues">Issues retained for display; the list is capped by the validator.</param>
/// <param name="InfoCount">Exact number of informational issues, including suppressed ones.</param>
/// <param name="WarningCount">Exact number of warnings, including suppressed ones.</param>
/// <param name="ErrorCount">Exact number of errors, including suppressed ones.</param>
/// <param name="SuppressedIssueCount">Issues counted but not retained in <paramref name="Issues"/>.</param>
public sealed record TableValidationSummary(
    TableName TableName,
    string RelativePath,
    bool IsMapped,
    TableValidationState State,
    int RecordCount,
    IReadOnlyList<ValidationIssue> Issues,
    int InfoCount,
    int WarningCount,
    int ErrorCount,
    int SuppressedIssueCount)
{
    /// <summary>
    /// Gets the exact total number of issues, including suppressed ones.
    /// </summary>
    public int IssueCount => InfoCount + WarningCount + ErrorCount;

    /// <summary>
    /// Gets a value indicating whether the table produced at least one issue.
    /// </summary>
    public bool HasIssues => IssueCount > 0;
}

/// <summary>
/// Result of validating every table of one <c>TableData</c> directory as a single report.
/// Severity counts are exact even when issue details were capped.
/// </summary>
/// <param name="TableDataDirectory">Absolute <c>TableData</c> directory that was validated.</param>
/// <param name="GeneratedAtUtc">UTC time when the report was produced.</param>
/// <param name="Tables">Per-table results in catalog order.</param>
public sealed record DatasetValidationReport(
    string TableDataDirectory,
    DateTime GeneratedAtUtc,
    IReadOnlyList<TableValidationSummary> Tables)
{
    /// <summary>
    /// Gets the exact number of informational issues across the data set.
    /// </summary>
    public int InfoCount => Tables.Sum(static table => table.InfoCount);

    /// <summary>
    /// Gets the exact number of warnings across the data set.
    /// </summary>
    public int WarningCount => Tables.Sum(static table => table.WarningCount);

    /// <summary>
    /// Gets the exact number of errors across the data set.
    /// </summary>
    public int ErrorCount => Tables.Sum(static table => table.ErrorCount);

    /// <summary>
    /// Gets the exact total number of issues, including suppressed ones.
    /// </summary>
    public int IssueCount => Tables.Sum(static table => table.IssueCount);

    /// <summary>
    /// Gets the number of issues that were counted but not retained for display.
    /// </summary>
    public int SuppressedIssueCount => Tables.Sum(static table => table.SuppressedIssueCount);

    /// <summary>
    /// Gets the number of tables that were successfully validated.
    /// </summary>
    public int ValidatedTableCount =>
        Tables.Count(static table => table.State == TableValidationState.Validated);

    /// <summary>
    /// Gets the number of tables whose file is malformed.
    /// </summary>
    public int MalformedTableCount =>
        Tables.Count(static table => table.State == TableValidationState.Malformed);

    /// <summary>
    /// Gets the number of tables whose file could not be read.
    /// </summary>
    public int UnreadableTableCount =>
        Tables.Count(static table => table.State == TableValidationState.Unreadable);

    /// <summary>
    /// Gets the number of tables that used document-driven mode because metadata was absent.
    /// </summary>
    public int UnmappedTableCount => Tables.Count(static table => !table.IsMapped);

    /// <summary>
    /// Gets the total number of records read from every parsed table.
    /// </summary>
    public int RecordCount => Tables.Sum(static table => table.RecordCount);

    /// <summary>
    /// Gets a value indicating whether the report contains no issues at all.
    /// </summary>
    public bool IsEmpty => IssueCount == 0;
}

/// <summary>
/// Progress of a data-set-wide scan such as validation or search.
/// </summary>
/// <param name="Completed">Number of tables already processed.</param>
/// <param name="Total">Total number of tables to process.</param>
/// <param name="TableName">Table that was processed last.</param>
public sealed record DatasetScanProgress(int Completed, int Total, TableName? TableName = null);
