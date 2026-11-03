using System.Globalization;
using System.Text;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Xml.Comparison;

/// <summary>
/// Renders a <see cref="TableComparisonReport"/> as Markdown or long-format CSV for export.
/// Both formats are deterministic for the same report and use invariant numeric formatting.
/// </summary>
public static class TableComparisonReportFormatter
{
    /// <summary>
    /// Formats the report as a Markdown document with a summary, table list, and per-table details
    /// for entries that contain differences.
    /// </summary>
    /// <param name="report">Report to format.</param>
    /// <returns>Markdown text encoded as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report"/> is null.</exception>
    public static string FormatMarkdown(TableComparisonReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var builder = new StringBuilder();
        builder.AppendLine("# TableData comparison report");
        builder.AppendLine();
        builder.AppendLine($"- Left: `{report.LeftDirectory.Replace("`", "'", StringComparison.Ordinal)}`");
        builder.AppendLine($"- Right: `{report.RightDirectory.Replace("`", "'", StringComparison.Ordinal)}`");
        builder.AppendLine($"- Generated (UTC): {report.GeneratedAtUtc.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}");
        builder.AppendLine();

        AppendSummary(builder, report.Summary);
        AppendWarnings(builder, report.Warnings);
        AppendLookupReferences(builder, report.LookupReferences);
        AppendTableList(builder, report.Tables);
        AppendTableDetails(builder, report.Tables);

        return builder.ToString();
    }

    /// <summary>
    /// Formats the report as long-format CSV: one row per table, one row per compared field, and
    /// one row per lookup reference. Cells are quoted and escaped according to RFC 4180 rules.
    /// </summary>
    /// <param name="report">Report to format.</param>
    /// <returns>CSV text with a header row.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report"/> is null.</exception>
    public static string FormatCsv(TableComparisonReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var builder = new StringBuilder();
        builder.AppendLine("RecordType,Table,Name,Status,Left,Right,LeftDetail,RightDetail");

        foreach (var entry in report.Tables)
        {
            var left = entry.Left?.Snapshot;
            var right = entry.Right?.Snapshot;
            AppendCsvRow(
                builder,
                "Table",
                entry.TableName.Value,
                string.Empty,
                GetEntryStatusText(entry),
                left?.RecordCount.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
                right?.RecordCount.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
                left is null ? string.Empty : $"{left.FieldCount} fields; {left.DistinctRecordShapes} shapes",
                right is null ? string.Empty : $"{right.FieldCount} fields; {right.DistinctRecordShapes} shapes");

            foreach (var field in entry.Fields)
            {
                AppendCsvRow(
                    builder,
                    "Field",
                    entry.TableName.Value,
                    field.Name,
                    field.Status.ToString(),
                    FormatType(field.LeftType),
                    FormatType(field.RightType),
                    FormatPresence(field.LeftPresence, field.LeftMaxOccurrences),
                    FormatPresence(field.RightPresence, field.RightMaxOccurrences));
            }

            AppendValueCsvRows(builder, entry);
            AppendOverlayCsvRows(builder, entry);
        }

        foreach (var reference in report.LookupReferences)
        {
            AppendCsvRow(
                builder,
                "Lookup",
                reference.Table.Value,
                reference.FieldName,
                $"{reference.LeftAvailability}/{reference.RightAvailability}",
                reference.LeftAvailability.ToString(),
                reference.RightAvailability.ToString(),
                reference.LookupTable,
                string.Empty);
        }

        foreach (var warning in report.Warnings)
        {
            AppendCsvRow(builder, "Warning", string.Empty, string.Empty, "Warning", string.Empty, string.Empty, warning, string.Empty);
        }

        return builder.ToString();
    }

    private static void AppendValueCsvRows(StringBuilder builder, TableComparisonEntry entry)
    {
        var value = entry.Value;
        if (!value.HasDifferences)
        {
            return;
        }

        var identity = value.IdentityKind.ToString();
        foreach (var record in value.Records)
        {
            AppendCsvRow(
                builder,
                "Record",
                entry.TableName.Value,
                record.Key,
                record.Kind.ToString(),
                string.Empty,
                string.Empty,
                record.ChangedFieldCount.ToString(CultureInfo.InvariantCulture),
                identity);
            foreach (var field in record.Fields)
            {
                AppendCsvRow(
                    builder,
                    "Value",
                    entry.TableName.Value,
                    string.Concat(record.Key, "/", field.Name),
                    field.Kind.ToString(),
                    field.LeftValue ?? string.Empty,
                    field.RightValue ?? string.Empty,
                    record.Kind.ToString(),
                    identity);
            }
        }

        foreach (var field in value.RootFields)
        {
            AppendCsvRow(
                builder,
                "RootField",
                entry.TableName.Value,
                field.Name,
                field.Kind.ToString(),
                field.LeftValue ?? string.Empty,
                field.RightValue ?? string.Empty,
                identity,
                string.Empty);
        }
    }

    private static void AppendOverlayCsvRows(StringBuilder builder, TableComparisonEntry entry)
    {
        foreach (var overlay in entry.Overlays)
        {
            AppendCsvRow(
                builder,
                "Overlay",
                entry.TableName.Value,
                overlay.LanguageCode,
                overlay.Status.ToString(),
                overlay.Left.CellCount.ToString(CultureInfo.InvariantCulture),
                overlay.Right.CellCount.ToString(CultureInfo.InvariantCulture),
                $"{overlay.Left.Status}/{overlay.Right.Status}",
                $"{overlay.AddedCellCount}/{overlay.RemovedCellCount}/{overlay.ChangedCellCount}");
            foreach (var cell in overlay.Cells)
            {
                AppendCsvRow(
                    builder,
                    "OverlayCell",
                    entry.TableName.Value,
                    string.Concat(overlay.LanguageCode, "/", cell.Key, "/", cell.FieldPath),
                    cell.Kind.ToString(),
                    cell.LeftValue ?? string.Empty,
                    cell.RightValue ?? string.Empty,
                    cell.Key,
                    cell.FieldPath);
            }
        }
    }

    private static void AppendSummary(StringBuilder builder, TableComparisonSummary summary)
    {
        builder.AppendLine("## Summary");
        builder.AppendLine();
        builder.AppendLine("| Metric | Value |");
        builder.AppendLine("| --- | ---: |");
        AppendMetric(builder, "Tables left", summary.LeftTableCount);
        AppendMetric(builder, "Tables right", summary.RightTableCount);
        AppendMetric(builder, "Common tables", summary.CommonTableCount);
        AppendMetric(builder, "Only left", summary.LeftOnlyTableCount);
        AppendMetric(builder, "Only right", summary.RightOnlyTableCount);
        AppendMetric(builder, "Identical tables", summary.IdenticalTableCount);
        AppendMetric(builder, "Changed tables", summary.ChangedTableCount);
        AppendMetric(builder, "Tables with a malformed side", summary.MalformedTableCount);
        AppendMetric(builder, "Tables with an unreadable side", summary.FailedTableCount);
        AppendMetric(builder, "Fields added", summary.AddedFieldCount);
        AppendMetric(builder, "Fields removed", summary.RemovedFieldCount);
        AppendMetric(builder, "Fields with changed type", summary.TypeChangedFieldCount);
        AppendMetric(builder, "Fields with changed cardinality", summary.CardinalityChangedFieldCount);
        AppendMetric(builder, "Records added", summary.AddedRecordCount);
        AppendMetric(builder, "Records removed", summary.RemovedRecordCount);
        AppendMetric(builder, "Records changed", summary.ChangedRecordCount);
        AppendMetric(builder, "Field values changed", summary.ChangedFieldCount);
        AppendMetric(builder, "Root fields changed", summary.ChangedRootFieldCount);
        AppendMetric(builder, "Overlay cells differing", summary.DifferingOverlayCellCount);
        AppendMetric(builder, "Records left", summary.LeftRecordTotal);
        AppendMetric(builder, "Records right", summary.RightRecordTotal);
        builder.AppendLine();
    }

    private static void AppendMetric(StringBuilder builder, string name, long value) =>
        builder.AppendLine($"| {EscapeCell(name)} | {value.ToString(CultureInfo.InvariantCulture)} |");

    private static void AppendWarnings(StringBuilder builder, IReadOnlyList<string> warnings)
    {
        if (warnings.Count == 0)
        {
            return;
        }

        builder.AppendLine("## Warnings");
        builder.AppendLine();
        foreach (var warning in warnings)
        {
            builder.AppendLine($"- {EscapeCell(warning)}");
        }

        builder.AppendLine();
    }

    private static void AppendLookupReferences(
        StringBuilder builder,
        IReadOnlyList<LookupReferenceComparison> references)
    {
        builder.AppendLine("## Lookup references");
        builder.AppendLine();
        if (references.Count == 0)
        {
            builder.AppendLine("_No lookup references were reported._");
            builder.AppendLine();
            return;
        }

        builder.AppendLine("| Table | Field | Lookup table | Left | Right |");
        builder.AppendLine("| --- | --- | --- | --- | --- |");
        foreach (var reference in references)
        {
            builder.AppendLine(
                $"| {EscapeCell(reference.Table.Value)} | {EscapeCell(reference.FieldName)} | "
                + $"{EscapeCell(reference.LookupTable)} | {FormatAvailability(reference.LeftAvailability)} | "
                + $"{FormatAvailability(reference.RightAvailability)} |");
        }

        builder.AppendLine();
    }

    private static void AppendTableList(StringBuilder builder, IReadOnlyList<TableComparisonEntry> entries)
    {
        builder.AppendLine("## Tables");
        builder.AppendLine();
        builder.AppendLine(
            "| Table | Status | Records left | Records right | Fields left | Fields right | Added | Removed | Type changes |");
        builder.AppendLine("| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");
        foreach (var entry in entries)
        {
            var left = entry.Left?.Snapshot;
            var right = entry.Right?.Snapshot;
            builder.AppendLine(
                $"| {EscapeCell(entry.TableName.Value)} | {GetEntryStatusText(entry)} | "
                + $"{FormatCount(left?.RecordCount)} | {FormatCount(right?.RecordCount)} | "
                + $"{FormatCount(left?.FieldCount)} | {FormatCount(right?.FieldCount)} | "
                + $"{entry.AddedFieldCount.ToString(CultureInfo.InvariantCulture)} | "
                + $"{entry.RemovedFieldCount.ToString(CultureInfo.InvariantCulture)} | "
                + $"{entry.TypeChangedFieldCount.ToString(CultureInfo.InvariantCulture)} |");
        }

        builder.AppendLine();
    }

    private static void AppendTableDetails(StringBuilder builder, IReadOnlyList<TableComparisonEntry> entries)
    {
        var changed = entries.Where(static entry => entry.HasDifferences).ToList();
        if (changed.Count == 0)
        {
            return;
        }

        builder.AppendLine("## Table details");
        builder.AppendLine();
        foreach (var entry in changed)
        {
            builder.AppendLine($"### {entry.TableName.Value}");
            builder.AppendLine();
            builder.AppendLine($"- Status: {GetEntryStatusText(entry)}");
            builder.AppendLine($"- Relative path: `{entry.RelativePath.Replace("`", "'", StringComparison.Ordinal)}`");
            AppendSide(builder, "Left", entry.Left);
            AppendSide(builder, "Right", entry.Right);
            builder.AppendLine();

            var changedFields = entry.Fields
                .Where(static field => field.Status != FieldComparisonStatus.Same)
                .ToList();
            if (changedFields.Count == 0)
            {
                builder.AppendLine("_No field-level differences._");
                builder.AppendLine();
            }
            else
            {
                builder.AppendLine("| Field | Status | Left type | Right type | Left presence | Right presence |");
                builder.AppendLine("| --- | --- | --- | --- | --- | --- |");
                foreach (var field in changedFields)
                {
                    builder.AppendLine(
                        $"| {EscapeCell(field.Name)} | {field.Status} | {FormatType(field.LeftType)} | "
                        + $"{FormatType(field.RightType)} | {FormatPresence(field.LeftPresence, field.LeftMaxOccurrences)} | "
                        + $"{FormatPresence(field.RightPresence, field.RightMaxOccurrences)} |");
                }

                builder.AppendLine();
            }

            AppendValueDetails(builder, entry);
            AppendOverlayDetails(builder, entry);
        }
    }

    private static void AppendValueDetails(StringBuilder builder, TableComparisonEntry entry)
    {
        var value = entry.Value;
        if (!value.HasDifferences)
        {
            return;
        }

        builder.AppendLine("#### Value differences");
        builder.AppendLine();
        builder.AppendLine(
            $"- Identity: {(value.IdentityKind == RecordIdentityKind.DeclaredKey ? "declared record key" : "record order (no key declared)")}");
        builder.AppendLine(
            $"- Records: {value.AddedRecordCount} added, {value.RemovedRecordCount} removed, "
            + $"{value.ChangedRecordCount} changed; {value.ChangedFieldCount} field values differ; "
            + $"{value.RootFieldChangeCount} root fields differ.");
        if (value.RecordsTruncated)
        {
            builder.AppendLine("- Detail lists are capped by the report limits; the counts above are exact.");
        }

        builder.AppendLine();
        if (value.Records.Count > 0)
        {
            builder.AppendLine("| Record | Status | Changed fields |");
            builder.AppendLine("| --- | --- | ---: |");
            foreach (var record in value.Records)
            {
                builder.AppendLine(
                    $"| {EscapeCell(record.Key)} | {record.Kind} | "
                    + $"{record.ChangedFieldCount.ToString(CultureInfo.InvariantCulture)} |");
            }

            builder.AppendLine();

            var fieldRows = value.Records
                .SelectMany(static record => record.Fields.Select(field => (Record: record, Field: field)))
                .ToList();
            if (fieldRows.Count > 0)
            {
                builder.AppendLine("| Record | Field | Status | Left | Right |");
                builder.AppendLine("| --- | --- | --- | --- | --- |");
                foreach (var row in fieldRows)
                {
                    builder.AppendLine(
                        $"| {EscapeCell(row.Record.Key)} | {EscapeCell(row.Field.Name)} | {row.Field.Kind} | "
                        + $"{EscapeCell(row.Field.LeftValue ?? string.Empty)} | "
                        + $"{EscapeCell(row.Field.RightValue ?? string.Empty)} |");
                }

                builder.AppendLine();
            }
        }

        if (value.RootFields.Count > 0)
        {
            builder.AppendLine("| Root field | Status | Left | Right |");
            builder.AppendLine("| --- | --- | --- | --- |");
            foreach (var field in value.RootFields)
            {
                builder.AppendLine(
                    $"| {EscapeCell(field.Name)} | {field.Kind} | "
                    + $"{EscapeCell(field.LeftValue ?? string.Empty)} | "
                    + $"{EscapeCell(field.RightValue ?? string.Empty)} |");
            }

            builder.AppendLine();
        }
    }

    private static void AppendOverlayDetails(StringBuilder builder, TableComparisonEntry entry)
    {
        var overlays = entry.Overlays;
        if (overlays.Count == 0)
        {
            return;
        }

        var changed = overlays.Where(static overlay => overlay.HasDifferences).ToList();
        if (changed.Count == 0)
        {
            return;
        }

        builder.AppendLine("#### Overlays");
        builder.AppendLine();
        builder.AppendLine("| Language | Status | Left cells | Right cells | Added | Removed | Changed |");
        builder.AppendLine("| --- | --- | ---: | ---: | ---: | ---: | ---: |");
        var errors = new List<string>();
        foreach (var overlay in changed)
        {
            builder.AppendLine(
                $"| {EscapeCell(overlay.LanguageCode)} | {overlay.Status} | "
                + $"{overlay.Left.CellCount.ToString(CultureInfo.InvariantCulture)} ({overlay.Left.Status}) | "
                + $"{overlay.Right.CellCount.ToString(CultureInfo.InvariantCulture)} ({overlay.Right.Status}) | "
                + $"{overlay.AddedCellCount.ToString(CultureInfo.InvariantCulture)} | "
                + $"{overlay.RemovedCellCount.ToString(CultureInfo.InvariantCulture)} | "
                + $"{overlay.ChangedCellCount.ToString(CultureInfo.InvariantCulture)} |");
            if (!string.IsNullOrEmpty(overlay.Left.ErrorText))
            {
                errors.Add($"{overlay.LanguageCode}: left overlay error: {EscapeCell(overlay.Left.ErrorText)}");
            }

            if (!string.IsNullOrEmpty(overlay.Right.ErrorText))
            {
                errors.Add($"{overlay.LanguageCode}: right overlay error: {EscapeCell(overlay.Right.ErrorText)}");
            }
        }

        builder.AppendLine();
        if (errors.Count > 0)
        {
            foreach (var error in errors)
            {
                builder.AppendLine($"- {error}");
            }

            builder.AppendLine();
        }
        if (changed.Any(static overlay => overlay.CellsTruncated))
        {
            builder.AppendLine("_Overlay cell lists are capped by the report limits; the counts above are exact._");
            builder.AppendLine();
        }

        var cellRows = changed
            .SelectMany(static overlay => overlay.Cells.Select(cell => (Overlay: overlay, Cell: cell)))
            .ToList();
        if (cellRows.Count == 0)
        {
            return;
        }

        builder.AppendLine("| Language | Record | Field | Status | Left | Right |");
        builder.AppendLine("| --- | --- | --- | --- | --- | --- |");
        foreach (var row in cellRows)
        {
            builder.AppendLine(
                $"| {EscapeCell(row.Overlay.LanguageCode)} | {EscapeCell(row.Cell.Key)} | "
                + $"{EscapeCell(row.Cell.FieldPath)} | {row.Cell.Kind} | "
                + $"{EscapeCell(row.Cell.LeftValue ?? string.Empty)} | "
                + $"{EscapeCell(row.Cell.RightValue ?? string.Empty)} |");
        }

        builder.AppendLine();
    }

    private static void AppendSide(StringBuilder builder, string label, TableSideComparison? side)
    {
        if (side is null)
        {
            builder.AppendLine($"- {label}: not present");
            return;
        }

        if (side.Snapshot is null)
        {
            builder.AppendLine($"- {label}: {side.Status.ToString().ToLowerInvariant()}");
            if (!string.IsNullOrEmpty(side.ErrorText))
            {
                builder.AppendLine($"  - Error: {EscapeCell(side.ErrorText)}");
            }

            foreach (var diagnostic in side.Diagnostics)
            {
                builder.AppendLine($"  - {FormatDiagnostic(diagnostic)}");
            }

            return;
        }

        var snapshot = side.Snapshot;
        builder.AppendLine(
            $"- {label}: {snapshot.RecordCount.ToString(CultureInfo.InvariantCulture)} records, "
            + $"{snapshot.FieldCount.ToString(CultureInfo.InvariantCulture)} fields, "
            + $"{snapshot.DistinctRecordShapes.ToString(CultureInfo.InvariantCulture)} distinct shapes, "
            + $"{snapshot.MinFieldsPerRecord.ToString(CultureInfo.InvariantCulture)}-"
            + $"{snapshot.MaxFieldsPerRecord.ToString(CultureInfo.InvariantCulture)} fields/record "
            + $"(avg {snapshot.AverageFieldsPerRecord.ToString("0.0", CultureInfo.InvariantCulture)}), "
            + $"{snapshot.EmptyRecordCount.ToString(CultureInfo.InvariantCulture)} empty records; "
            + $"overlays: {(snapshot.OverlayLanguages.Count == 0 ? "none" : string.Join(", ", snapshot.OverlayLanguages))}");
    }

    private static string FormatDiagnostic(LoadDiagnostic diagnostic)
    {
        if (diagnostic.Line is not int line)
        {
            return EscapeCell($"{diagnostic.Code}: {diagnostic.Message}");
        }

        var position = diagnostic.Position ?? 1;
        return EscapeCell(
            $"{diagnostic.Code}: {diagnostic.Message} "
            + $"(line {line.ToString(CultureInfo.InvariantCulture)}, position {position.ToString(CultureInfo.InvariantCulture)})");
    }

    private static string GetEntryStatusText(TableComparisonEntry entry)
    {
        var status = entry.Status switch
        {
            TableComparisonStatus.Both => entry.HasDifferences ? "Both (changed)" : "Both (identical)",
            TableComparisonStatus.LeftOnly => "Only left",
            _ => "Only right",
        };

        if (entry.HasMalformedSide)
        {
            status += "; malformed";
        }

        if (entry.HasLoadFailure)
        {
            status += "; unreadable";
        }

        return status;
    }

    private static string FormatAvailability(LookupAvailability availability) => availability switch
    {
        LookupAvailability.Missing => "missing",
        LookupAvailability.Unreadable => "unreadable",
        _ => "available",
    };

    private static string FormatType(ColumnDataType? type) => type?.ToString() ?? "-";

    private static string FormatPresence(int presence, int maxOccurrences) =>
        maxOccurrences > 1
            ? $"{presence.ToString(CultureInfo.InvariantCulture)} (x{maxOccurrences.ToString(CultureInfo.InvariantCulture)})"
            : presence.ToString(CultureInfo.InvariantCulture);

    private static string FormatCount(int? count) =>
        count?.ToString(CultureInfo.InvariantCulture) ?? "-";

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
