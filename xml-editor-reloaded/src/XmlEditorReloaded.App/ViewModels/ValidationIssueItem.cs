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

        Severity = issue.Severity;
        Code = issue.Code;
        Message = issue.Message;
        Text = BuildText(issue);
    }

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
