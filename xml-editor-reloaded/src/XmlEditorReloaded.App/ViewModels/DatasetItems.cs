using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Search;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Display projection of one validation issue of a data-set-wide report.
/// </summary>
public sealed class DatasetIssueItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetIssueItem"/> class.
    /// </summary>
    /// <param name="table">Table the issue belongs to.</param>
    /// <param name="issue">Issue to project.</param>
    /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
    public DatasetIssueItem(TableValidationSummary table, ValidationIssue issue)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(issue);

        Table = table.TableName;
        TableName = table.TableName.Value;
        Severity = issue.Severity;
        SeverityLabel = DatasetDisplayText.SeverityLabel(issue.Severity);
        RecordKey = issue.RecordKey ?? string.Empty;
        FieldPath = issue.FieldPath ?? string.Empty;
        Code = issue.Code;
        Message = issue.Message;
    }

    /// <summary>
    /// Gets the affected table.
    /// </summary>
    public TableName Table { get; }

    /// <summary>
    /// Gets the dotted table name for display.
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// Gets the issue severity.
    /// </summary>
    public ValidationSeverity Severity { get; }

    /// <summary>
    /// Gets the localized severity label.
    /// </summary>
    public string SeverityLabel { get; }

    /// <summary>
    /// Gets the affected record key, or an empty string for a table-level issue.
    /// </summary>
    public string RecordKey { get; }

    /// <summary>
    /// Gets the affected field path, or an empty string for a record-level issue.
    /// </summary>
    public string FieldPath { get; }

    /// <summary>
    /// Gets the stable issue code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the human-readable issue message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the record key passed to navigation, or <see langword="null"/> for a table-level issue.
    /// </summary>
    public string? NavigationRecordKey => RecordKey.Length == 0 ? null : RecordKey;

    /// <summary>
    /// Determines whether the issue matches a free-text filter.
    /// </summary>
    /// <param name="term">Filter text; compared ignoring case against table, key, field, code, and message.</param>
    /// <returns><see langword="true"/> when the issue matches or the term is empty.</returns>
    public bool Matches(string term)
    {
        if (term.Length == 0)
        {
            return true;
        }

        return TableName.Contains(term, StringComparison.OrdinalIgnoreCase)
            || RecordKey.Contains(term, StringComparison.OrdinalIgnoreCase)
            || FieldPath.Contains(term, StringComparison.OrdinalIgnoreCase)
            || Code.Contains(term, StringComparison.OrdinalIgnoreCase)
            || Message.Contains(term, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Display projection of one hit of a data-set-wide search.
/// </summary>
public sealed class DatasetSearchHitItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetSearchHitItem"/> class.
    /// </summary>
    /// <param name="hit">Search hit to project.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="hit"/> is null.</exception>
    public DatasetSearchHitItem(DatasetSearchHit hit)
    {
        ArgumentNullException.ThrowIfNull(hit);

        Table = hit.TableName;
        TableName = hit.TableName.Value;
        RecordKey = hit.RecordKey;
        FieldPath = hit.FieldPath;
        Value = hit.Value;
        LanguageLabel = hit.LanguageCode is null
            ? Strings.Get("DatasetBaseLanguage")
            : hit.LanguageCode;
    }

    /// <summary>
    /// Gets the table that contains the value.
    /// </summary>
    public TableName Table { get; }

    /// <summary>
    /// Gets the dotted table name for display.
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// Gets the record key, or an empty string for a root-level value.
    /// </summary>
    public string RecordKey { get; }

    /// <summary>
    /// Gets the field path relative to the record or document root.
    /// </summary>
    public string FieldPath { get; }

    /// <summary>
    /// Gets the displayed value, which may be truncated by the search service.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the overlay language code, or the localized base-table label.
    /// </summary>
    public string LanguageLabel { get; }

    /// <summary>
    /// Gets the record key passed to navigation, or <see langword="null"/> for a root-level value.
    /// </summary>
    public string? NavigationRecordKey => RecordKey.Length == 0 ? null : RecordKey;
}

/// <summary>
/// Localized display helpers for data-set report items.
/// </summary>
internal static class DatasetDisplayText
{
    /// <summary>
    /// Returns the localized singular label of a validation severity.
    /// </summary>
    /// <param name="severity">Severity to label.</param>
    /// <returns>Localized label.</returns>
    public static string SeverityLabel(ValidationSeverity severity) => severity switch
    {
        ValidationSeverity.Error => Strings.Get("DatasetSeverityErrorOne"),
        ValidationSeverity.Warning => Strings.Get("DatasetSeverityWarningOne"),
        _ => Strings.Get("DatasetSeverityInfoOne"),
    };
}
