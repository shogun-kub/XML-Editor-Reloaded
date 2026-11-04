using System.Globalization;
using System.Text;
using XmlEditorReloaded.Core.Validation;

namespace XmlEditorReloaded.Xml.Validation;

/// <summary>
/// Renders a <see cref="DatasetValidationReport"/> as one Markdown or CSV document for export.
/// Both formats are deterministic for the same report and use invariant numeric formatting.
/// </summary>
public static class DatasetValidationReportFormatter
{
    /// <summary>
    /// Formats the report as a Markdown document with a summary, per-table counts, and every retained
    /// issue.
    /// </summary>
    /// <param name="report">Report to format.</param>
    /// <returns>Markdown text.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report"/> is null.</exception>
    public static string FormatMarkdown(DatasetValidationReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var builder = new StringBuilder();
        builder.AppendLine("# TableData validation report");
        builder.AppendLine();
        builder.AppendLine($"- Data set: `{report.TableDataDirectory.Replace("`", "'", StringComparison.Ordinal)}`");
        builder.AppendLine(
            $"- Generated (UTC): {report.GeneratedAtUtc.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}");
        builder.AppendLine();

        builder.AppendLine("## Summary");
        builder.AppendLine();
        builder.AppendLine("| Metric | Value |");
        builder.AppendLine("| --- | ---: |");
        AppendMetric(builder, "Tables", report.Tables.Count);
        AppendMetric(builder, "Validated", report.ValidatedTableCount);
        AppendMetric(builder, "Malformed", report.MalformedTableCount);
        AppendMetric(builder, "Unreadable", report.UnreadableTableCount);
        AppendMetric(builder, "Without metadata", report.UnmappedTableCount);
        AppendMetric(builder, "Records", report.RecordCount);
        AppendMetric(builder, "Errors", report.ErrorCount);
        AppendMetric(builder, "Warnings", report.WarningCount);
        AppendMetric(builder, "Information", report.InfoCount);
        AppendMetric(builder, "Issues suppressed", report.SuppressedIssueCount);
        builder.AppendLine();

        builder.AppendLine("## Tables");
        builder.AppendLine();
        builder.AppendLine("| Table | State | Metadata | Records | Errors | Warnings | Info |");
        builder.AppendLine("| --- | --- | --- | ---: | ---: | ---: | ---: |");
        foreach (var table in report.Tables)
        {
            builder.AppendLine(
                $"| {EscapeCell(table.TableName.Value)} | {table.State} | "
                + $"{(table.IsMapped ? "mapped" : "document-driven")} | "
                + $"{table.RecordCount.ToString(CultureInfo.InvariantCulture)} | "
                + $"{table.ErrorCount.ToString(CultureInfo.InvariantCulture)} | "
                + $"{table.WarningCount.ToString(CultureInfo.InvariantCulture)} | "
                + $"{table.InfoCount.ToString(CultureInfo.InvariantCulture)} |");
        }

        builder.AppendLine();

        builder.AppendLine("## Issues");
        builder.AppendLine();
        if (report.IssueCount == 0)
        {
            builder.AppendLine("_No issues were reported._");
            builder.AppendLine();
            return builder.ToString();
        }

        builder.AppendLine("| Table | Severity | Record | Field | Code | Message |");
        builder.AppendLine("| --- | --- | --- | --- | --- | --- |");
        foreach (var table in report.Tables)
        {
            foreach (var issue in table.Issues)
            {
                builder.AppendLine(
                    $"| {EscapeCell(table.TableName.Value)} | {issue.Severity} | "
                    + $"{EscapeCell(issue.RecordKey ?? string.Empty)} | "
                    + $"{EscapeCell(issue.FieldPath ?? string.Empty)} | {EscapeCell(issue.Code)} | "
                    + $"{EscapeCell(issue.Message)} |");
            }
        }

        builder.AppendLine();
        if (report.SuppressedIssueCount > 0)
        {
            builder.AppendLine(
                "_Issue lists are capped by the validator; the counts in the summary are exact._");
            builder.AppendLine();
        }

        return builder.ToString();
    }

    /// <summary>
    /// Formats the report as long-format CSV: one row per table followed by one row per retained issue.
    /// Cells are quoted and escaped according to RFC 4180 rules.
    /// </summary>
    /// <param name="report">Report to format.</param>
    /// <returns>CSV text with a header row.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report"/> is null.</exception>
    public static string FormatCsv(DatasetValidationReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var builder = new StringBuilder();
        builder.AppendLine("Table,State,Mapped,Records,Errors,Warnings,Info,Severity,Record,Field,Code,Message");
        foreach (var table in report.Tables)
        {
            AppendCsvRow(
                builder,
                table.TableName.Value,
                table.State.ToString(),
                table.IsMapped ? "true" : "false",
                table.RecordCount.ToString(CultureInfo.InvariantCulture),
                table.ErrorCount.ToString(CultureInfo.InvariantCulture),
                table.WarningCount.ToString(CultureInfo.InvariantCulture),
                table.InfoCount.ToString(CultureInfo.InvariantCulture),
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);

            foreach (var issue in table.Issues)
            {
                AppendCsvRow(
                    builder,
                    table.TableName.Value,
                    table.State.ToString(),
                    table.IsMapped ? "true" : "false",
                    table.RecordCount.ToString(CultureInfo.InvariantCulture),
                    table.ErrorCount.ToString(CultureInfo.InvariantCulture),
                    table.WarningCount.ToString(CultureInfo.InvariantCulture),
                    table.InfoCount.ToString(CultureInfo.InvariantCulture),
                    issue.Severity.ToString(),
                    issue.RecordKey ?? string.Empty,
                    issue.FieldPath ?? string.Empty,
                    issue.Code,
                    issue.Message);
            }
        }

        return builder.ToString();
    }

    private static void AppendMetric(StringBuilder builder, string name, long value) =>
        builder.AppendLine($"| {EscapeCell(name)} | {value.ToString(CultureInfo.InvariantCulture)} |");

    private static string EscapeCell(string value) =>
        value.Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal)
            .Replace("|", "\\|", StringComparison.Ordinal);

    private static void AppendCsvRow(StringBuilder builder, params string[] cells)
    {
        for (var index = 0; index < cells.Length; index++)
        {
            if (index > 0)
            {
                builder.Append(',');
            }

            builder.Append(EscapeCsv(cells[index]));
        }

        builder.AppendLine();
    }

    private static string EscapeCsv(string value)
    {
        if (!value.Contains(',')
            && !value.Contains('"')
            && !value.Contains('\n')
            && !value.Contains('\r'))
        {
            return value;
        }

        return '"' + value.Replace("\"", "\"\"", StringComparison.Ordinal) + '"';
    }
}
