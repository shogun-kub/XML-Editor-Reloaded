using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Validation;

/// <summary>
/// Severity of a validation issue.
/// </summary>
public enum ValidationSeverity
{
    /// <summary>Informational notice, for example a field unknown to metadata.</summary>
    Info,

    /// <summary>A problem that may indicate data quality issues but does not block editing.</summary>
    Warning,

    /// <summary>A problem that makes the data unusable for a specific feature.</summary>
    Error,
}

/// <summary>
/// A single validation problem found in a table document.
/// </summary>
/// <param name="Severity">Severity of the issue.</param>
/// <param name="Code">Stable machine-readable issue code.</param>
/// <param name="Message">Human-readable description.</param>
/// <param name="TableName">Dotted name of the affected table, or <see langword="null"/> when not applicable.</param>
/// <param name="RecordKey">Record key the issue refers to, or <see langword="null"/> when table-level.</param>
/// <param name="FieldPath">Slash-separated field path the issue refers to, or <see langword="null"/> when record-level.</param>
public sealed record ValidationIssue(
    ValidationSeverity Severity,
    string Code,
    string Message,
    TableName? TableName = null,
    string? RecordKey = null,
    string? FieldPath = null);

/// <summary>
/// Result of validating one or more documents.
/// </summary>
/// <param name="Issues">Detected issues.</param>
public sealed record ValidationReport(IReadOnlyList<ValidationIssue> Issues)
{
    /// <summary>
    /// Gets an empty report with no issues.
    /// </summary>
    public static ValidationReport Empty { get; } = new(Array.Empty<ValidationIssue>());

    /// <summary>
    /// Gets a value indicating whether the report contains no issues.
    /// </summary>
    public bool IsEmpty => Issues.Count == 0;
}
