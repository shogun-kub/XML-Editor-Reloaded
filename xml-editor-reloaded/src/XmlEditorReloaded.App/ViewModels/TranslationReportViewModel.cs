using System.Collections.ObjectModel;
using System.Globalization;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Localization;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Read-only projection of a <see cref="TranslationReport"/> for the report window.
/// </summary>
public sealed class TranslationReportViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationReportViewModel"/> class.
    /// </summary>
    /// <param name="report">Translation report to display.</param>
    /// <param name="title">Window title, including the table and language names.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report"/> is null.</exception>
    public TranslationReportViewModel(TranslationReport report, string title)
    {
        ArgumentNullException.ThrowIfNull(report);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Title = title;
        CloseText = Strings.Get("TranslationReportClose");
        SummaryText = Strings.Format("TranslationReportSummary", report.Issues.Count);
        EmptyText = Strings.Get("TranslationReportEmpty");
        KindHeader = Strings.Get("TranslationReportKind");
        RecordHeader = Strings.Get("TranslationReportRecord");
        FieldHeader = Strings.Get("TranslationReportField");
        MessageHeader = Strings.Get("TranslationReportMessage");

        foreach (var group in report.Issues
                     .GroupBy(static issue => issue.Kind)
                     .OrderBy(static group => group.Key))
        {
            Groups.Add(new TranslationReportGroupViewModel(
                Strings.Get($"TranslationIssue.{group.Key}"),
                group.Count()));
        }

        foreach (var issue in report.Issues)
        {
            Issues.Add(new TranslationReportIssueViewModel(issue));
        }
    }

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the close button text.
    /// </summary>
    public string CloseText { get; }

    /// <summary>
    /// Gets the total issue count text.
    /// </summary>
    public string SummaryText { get; }

    /// <summary>
    /// Gets the text shown when the report has no issues.
    /// </summary>
    public string EmptyText { get; }

    /// <summary>
    /// Gets the issue-kind column header.
    /// </summary>
    public string KindHeader { get; }

    /// <summary>
    /// Gets the record-key column header.
    /// </summary>
    public string RecordHeader { get; }

    /// <summary>
    /// Gets the field-path column header.
    /// </summary>
    public string FieldHeader { get; }

    /// <summary>
    /// Gets the message column header.
    /// </summary>
    public string MessageHeader { get; }

    /// <summary>
    /// Gets a value indicating whether the report has no issues.
    /// </summary>
    public bool IsEmpty => Issues.Count == 0;

    /// <summary>
    /// Gets per-kind issue counts.
    /// </summary>
    public ObservableCollection<TranslationReportGroupViewModel> Groups { get; } = new();

    /// <summary>
    /// Gets the individual translation issues.
    /// </summary>
    public ObservableCollection<TranslationReportIssueViewModel> Issues { get; } = new();
}

/// <summary>
/// One translation issue kind and its count.
/// </summary>
/// <param name="Label">Localized issue-kind label.</param>
/// <param name="Count">Number of issues of this kind.</param>
public sealed record TranslationReportGroupViewModel(string Label, int Count)
{
    /// <summary>
    /// Gets the localized count summary, for example <c>Missing translation (12)</c>.
    /// </summary>
    public string DisplayText => $"{Label} ({Count.ToString(CultureInfo.CurrentCulture)})";
}

/// <summary>
/// Display projection of one <see cref="TranslationIssue"/>.
/// </summary>
public sealed class TranslationReportIssueViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationReportIssueViewModel"/> class.
    /// </summary>
    /// <param name="issue">Translation issue to project.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="issue"/> is null.</exception>
    public TranslationReportIssueViewModel(TranslationIssue issue)
    {
        ArgumentNullException.ThrowIfNull(issue);

        KindLabel = Strings.Get($"TranslationIssue.{issue.Kind}");
        RecordKey = issue.RecordKey ?? string.Empty;
        FieldPath = issue.FieldPath ?? string.Empty;
        Message = issue.Message;
    }

    /// <summary>
    /// Gets the localized issue-kind label.
    /// </summary>
    public string KindLabel { get; }

    /// <summary>
    /// Gets the affected record key, or an empty string when not applicable.
    /// </summary>
    public string RecordKey { get; }

    /// <summary>
    /// Gets the affected field path, or an empty string when not applicable.
    /// </summary>
    public string FieldPath { get; }

    /// <summary>
    /// Gets the human-readable issue description.
    /// </summary>
    public string Message { get; }
}
