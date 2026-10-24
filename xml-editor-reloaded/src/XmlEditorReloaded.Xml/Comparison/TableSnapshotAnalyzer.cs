using System.Globalization;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Comparison;

/// <summary>
/// Builds a read-only structural snapshot of a loaded table. Records are identified by the
/// metadata record element (descendants are used for nested-key tables such as <c>Cities</c>);
/// for document-driven tables the direct children of the root are used.
/// </summary>
internal static class TableSnapshotAnalyzer
{
    private const NumberStyles DecimalStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    /// <summary>
    /// Analyzes a loaded document without mutating it.
    /// </summary>
    /// <param name="document">Loaded table document.</param>
    /// <returns>The structural snapshot and per-field statistics in first-appearance order.</returns>
    public static AnalyzedTable Analyze(TableDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var root = document.Xml.Root;
        var metadata = document.Metadata;
        var recordElement = metadata?.RecordElement ?? document.RecordElementName;
        var records = ResolveRecords(root, recordElement, HasNestedKey(metadata));
        var statistics = new Dictionary<string, FieldStatistics>(StringComparer.Ordinal);
        var order = new List<string>();

        foreach (var record in records)
        {
            var occurrences = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var element in record.Elements())
            {
                var name = element.Name.LocalName;
                if (!statistics.TryGetValue(name, out var field))
                {
                    field = new FieldStatistics();
                    statistics.Add(name, field);
                    order.Add(name);
                }

                field.PresenceCount++;
                occurrences.TryGetValue(name, out var seen);
                occurrences[name] = seen + 1;
                field.MaxOccurrences = Math.Max(field.MaxOccurrences, seen + 1);
                field.HasNested |= element.HasElements;
                if (!element.HasElements)
                {
                    field.ObserveValue(element.Value);
                }
            }
        }

        var keyField = metadata?.RecordKey?.Primary.KeyField
            ?? (order.Count > 0 ? order[0] : null);
        var fields = new List<AnalyzedField>(order.Count);
        foreach (var name in order)
        {
            var field = statistics[name];
            fields.Add(new AnalyzedField(
                name,
                field.InferDataType(string.Equals(name, keyField, StringComparison.Ordinal)),
                field.PresenceCount,
                field.MaxOccurrences));
        }

        var fieldCounts = records.Select(static record => record.Elements().Count()).ToList();
        var shapes = new HashSet<string>(StringComparer.Ordinal);
        foreach (var record in records)
        {
            var names = record.Elements()
                .Select(static element => element.Name.LocalName)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(static name => name, StringComparer.Ordinal);
            shapes.Add(string.Join('/', names));
        }

        var snapshot = new TableComparisonSide(
            root?.Name.LocalName ?? string.Empty,
            recordElement,
            records.Count,
            order.Count,
            shapes.Count,
            fieldCounts.Count == 0 ? 0 : fieldCounts.Min(),
            fieldCounts.Count == 0 ? 0 : fieldCounts.Max(),
            fieldCounts.Count == 0 ? 0 : fieldCounts.Average(),
            fieldCounts.Count(static count => count == 0),
            document.Descriptor.Overlays
                .Select(static overlay => overlay.Language.Code)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(static code => code, StringComparer.Ordinal)
                .ToArray());

        return new AnalyzedTable(snapshot, fields);
    }

    private static List<XElement> ResolveRecords(XElement? root, string recordElement, bool nestedKey)
    {
        if (root is null)
        {
            return new List<XElement>();
        }

        var candidates = nestedKey ? root.Descendants() : root.Elements();
        var records = candidates
            .Where(element => string.Equals(element.Name.LocalName, recordElement, StringComparison.Ordinal))
            .ToList();
        if (records.Count == 0)
        {
            records = root.Elements().ToList();
        }

        return records;
    }

    private static bool HasNestedKey(TableMetadata? metadata) =>
        metadata?.RecordKey is { Segments.Count: > 0 } spec
        && !string.IsNullOrWhiteSpace(spec.Primary.Path);

    private sealed class FieldStatistics
    {
        public int PresenceCount { get; set; }

        public int MaxOccurrences { get; set; } = 1;

        public bool HasValue { get; private set; }

        public bool AllInteger { get; private set; } = true;

        public bool AllDecimal { get; private set; } = true;

        public bool AllBoolean { get; private set; } = true;

        public bool HasNested { get; set; }

        public void ObserveValue(string rawText)
        {
            if (rawText.Length == 0)
            {
                return;
            }

            HasValue = true;
            if (!int.TryParse(rawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            {
                AllInteger = false;
            }

            if (!decimal.TryParse(rawText, DecimalStyles, CultureInfo.InvariantCulture, out _))
            {
                AllDecimal = false;
            }

            if (rawText is not ("0" or "1"))
            {
                AllBoolean = false;
            }
        }

        public ColumnDataType InferDataType(bool isKey)
        {
            if (HasNested)
            {
                return ColumnDataType.Nested;
            }

            if (!HasValue)
            {
                return ColumnDataType.String;
            }

            if (AllBoolean && !isKey)
            {
                return ColumnDataType.Boolean;
            }

            if (AllInteger)
            {
                return ColumnDataType.Integer;
            }

            return AllDecimal ? ColumnDataType.Decimal : ColumnDataType.String;
        }
    }
}

/// <summary>
/// Field statistics produced by <see cref="TableSnapshotAnalyzer"/>.
/// </summary>
/// <param name="Name">Case-sensitive XML element name.</param>
/// <param name="DataType">Inferred field type.</param>
/// <param name="PresenceCount">Number of records containing the field.</param>
/// <param name="MaxOccurrences">Maximum occurrences within one record.</param>
internal sealed record AnalyzedField(
    string Name,
    ColumnDataType DataType,
    int PresenceCount,
    int MaxOccurrences);

/// <summary>
/// Snapshot and field statistics of one analyzed table.
/// </summary>
/// <param name="Snapshot">Structural and sparse profile.</param>
/// <param name="Fields">Fields in first-appearance order.</param>
internal sealed record AnalyzedTable(
    TableComparisonSide Snapshot,
    IReadOnlyList<AnalyzedField> Fields);
