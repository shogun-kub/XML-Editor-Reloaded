using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Display projection of one <see cref="ValidationIssue"/>.
/// </summary>
public sealed class ValidationIssueItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationIssueItem"/> class.
    /// </summary>
    /// <param name="issue">Validation issue to project.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="issue"/> is null.</exception>
    public ValidationIssueItem(ValidationIssue issue)
    {
        ArgumentNullException.ThrowIfNull(issue);

        Issue = issue;
        Severity = issue.Severity;
        Code = issue.Code;
        Message = issue.Message;
        Text = BuildText(issue);
    }

    /// <summary>
    /// Gets the original validation issue, including its table, record, and field references.
    /// </summary>
    public ValidationIssue Issue { get; }

    /// <summary>
    /// Gets the affected table, or <see langword="null"/> when the issue is not table-specific.
    /// </summary>
    public TableName? TableName => Issue.TableName;

    /// <summary>
    /// Gets the affected record key, or <see langword="null"/> when the issue is table-level.
    /// </summary>
    public string? RecordKey => Issue.RecordKey;

    /// <summary>
    /// Gets the affected field path, or <see langword="null"/> when the issue is record-level.
    /// </summary>
    public string? FieldPath => Issue.FieldPath;

    /// <summary>
    /// Gets the issue severity.
    /// </summary>
    public ValidationSeverity Severity { get; }

    /// <summary>
    /// Gets the issue code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the issue message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the formatted one-line description.
    /// </summary>
    public string Text { get; }

    private static string BuildText(ValidationIssue issue)
    {
        var location = string.IsNullOrEmpty(issue.RecordKey)
            ? issue.FieldPath
            : string.IsNullOrEmpty(issue.FieldPath)
                ? issue.RecordKey
                : $"{issue.RecordKey}/{issue.FieldPath}";
        var table = issue.TableName?.Value;
        var prefix = string.IsNullOrEmpty(table) ? string.Empty : table + ": ";
        return location is null
            ? $"{prefix}{issue.Severity}: {issue.Code} — {issue.Message}"
            : $"{prefix}{issue.Severity}: {issue.Code} [{location}] — {issue.Message}";
    }
}
