using System.Globalization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Schema;

/// <summary>
/// Builds a <see cref="TableSchema"/> from a loaded document. Fields are discovered as the union of
/// all records in first-appearance order; curated column metadata, when present, overrides types and
/// editor hints and contributes declared fields that the document does not currently contain.
/// </summary>
public sealed class TableSchemaBuilder
{
    private const NumberStyles DecimalStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    /// <summary>
    /// Builds the schema of a loaded table document.
    /// </summary>
    /// <param name="document">Loaded table document; sparse records keep their original shape.</param>
    /// <returns>The discovered schema, ordered by the first appearance of each field.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
    public TableSchema Build(TableDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var metadata = document.Metadata;
        var statistics = new Dictionary<string, ColumnStatistics>(StringComparer.Ordinal);
        var order = new List<string>();

        foreach (var record in document.Records)
        {
            var occurrences = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var element in record.Element.Elements())
            {
                var name = element.Name.LocalName;
                if (!statistics.TryGetValue(name, out var columnStats))
                {
                    columnStats = new ColumnStatistics();
                    statistics.Add(name, columnStats);
                    order.Add(name);
                }

                columnStats.PresenceCount++;
                occurrences.TryGetValue(name, out var seen);
                occurrences[name] = seen + 1;
                columnStats.MaxOccurrences = Math.Max(columnStats.MaxOccurrences, seen + 1);
                ObserveValue(columnStats, element.Value);
            }
        }

        if (metadata is not null)
        {
            foreach (var column in metadata.Columns)
            {
                if (statistics.ContainsKey(column.XmlFieldName))
                {
                    continue;
                }

                statistics[column.XmlFieldName] = new ColumnStatistics { DeclaredColumn = column };
                order.Add(column.XmlFieldName);
            }
        }

        var listFields = metadata?.ListFields
            .Select(static field => field.XmlFieldName)
            .ToHashSet(StringComparer.Ordinal);
        var dictPatterns = BuildDictPatterns(metadata);
        var translatableFields = metadata?.TranslatableFieldPaths
            .ToHashSet(StringComparer.Ordinal);
        var keyField = metadata?.RecordKey?.Primary.KeyField;
        if (keyField is null && order.Count > 0)
        {
            keyField = order[0];
        }

        var columns = new List<TableSchemaColumn>(order.Count);
        for (var index = 0; index < order.Count; index++)
        {
            var name = order[index];
            var columnStats = statistics[name];
            var declared = columnStats.DeclaredColumn;
            if (declared is null && metadata is not null && metadata.TryGetColumn(name, out var resolved))
            {
                declared = resolved;
            }

            var dataType = declared?.DataType
                ?? columnStats.InferDataType(string.Equals(name, keyField, StringComparison.Ordinal));
            var isTranslatable = translatableFields?.Contains(name) == true
                || declared?.Flags.HasFlag(ColumnFlags.Translatable) == true;
            var isHidden = declared?.EditorKind == ColumnEditorKind.Hidden
                || declared?.Flags.HasFlag(ColumnFlags.Hidden) == true;
            var flags = declared?.Flags ?? ColumnFlags.None;
            if (isTranslatable)
            {
                flags |= ColumnFlags.Translatable;
            }

            if (isHidden)
            {
                flags |= ColumnFlags.Hidden;
            }

            var isList = columnStats.MaxOccurrences > 1
                || listFields?.Contains(name) == true;
            var dictMask = FindDictMask(dictPatterns, name);

            columns.Add(new TableSchemaColumn(
                name,
                dataType,
                declared?.EditorKind ?? DefaultEditorKind(dataType),
                index,
                columnStats.PresenceCount,
                columnStats.MaxOccurrences,
                string.Equals(name, keyField, StringComparison.Ordinal),
                isList,
                dictMask is not null,
                dictMask,
                isTranslatable,
                isHidden,
                declared?.DisplayWidth,
                declared?.LookupTable,
                declared?.TooltipKey,
                flags));
        }

        return new TableSchema(
            document.Descriptor.TableName,
            document.RecordElementName,
            document.Records.Count,
            metadata is null || metadata.Columns.Count == 0,
            columns);
    }

    private static ColumnEditorKind DefaultEditorKind(ColumnDataType dataType) => dataType switch
    {
        ColumnDataType.Integer => ColumnEditorKind.IntegerBox,
        ColumnDataType.Decimal => ColumnEditorKind.DecimalBox,
        _ => ColumnEditorKind.TextBox,
    };

    private static void ObserveValue(ColumnStatistics statistics, string rawText)
    {
        if (rawText.Length == 0)
        {
            return;
        }

        statistics.HasValue = true;
        if (!int.TryParse(rawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
        {
            statistics.AllInteger = false;
        }

        if (!decimal.TryParse(rawText, DecimalStyles, CultureInfo.InvariantCulture, out _))
        {
            statistics.AllDecimal = false;
        }

        if (rawText is not ("0" or "1"))
        {
            statistics.AllBoolean = false;
        }
    }

    private static List<DictMaskPattern> BuildDictPatterns(TableMetadata? metadata)
    {
        var patterns = new List<DictMaskPattern>();
        if (metadata is null)
        {
            return patterns;
        }

        foreach (var definition in metadata.DictFields)
        {
            var placeholderIndex = definition.XmlFieldMask.IndexOf(
                DictFieldDefinition.IndexPlaceholder,
                StringComparison.Ordinal);
            if (placeholderIndex < 0)
            {
                patterns.Add(new DictMaskPattern(definition.XmlFieldMask, string.Empty, definition.XmlFieldMask, RequiresIndex: false));
                continue;
            }

            patterns.Add(new DictMaskPattern(
                definition.XmlFieldMask[..placeholderIndex],
                definition.XmlFieldMask[(placeholderIndex + DictFieldDefinition.IndexPlaceholder.Length)..],
                definition.XmlFieldMask,
                RequiresIndex: true));
        }

        return patterns;
    }

    private static string? FindDictMask(IReadOnlyList<DictMaskPattern> patterns, string fieldName)
    {
        foreach (var pattern in patterns)
        {
            if (pattern.Matches(fieldName))
            {
                return pattern.Mask;
            }
        }

        return null;
    }

    private sealed class ColumnStatistics
    {
        public int PresenceCount { get; set; }

        public int MaxOccurrences { get; set; } = 1;

        public bool HasValue { get; set; }

        public bool AllInteger { get; set; } = true;

        public bool AllDecimal { get; set; } = true;

        public bool AllBoolean { get; set; } = true;

        public ColumnMetadata? DeclaredColumn { get; set; }

        public ColumnDataType InferDataType(bool isKey)
        {
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

    private sealed record DictMaskPattern(string Prefix, string Suffix, string Mask, bool RequiresIndex)
    {
        public bool Matches(string fieldName)
        {
            if (!RequiresIndex)
            {
                return string.Equals(fieldName, Mask, StringComparison.Ordinal);
            }

            if (!fieldName.StartsWith(Prefix, StringComparison.Ordinal)
                || !fieldName.EndsWith(Suffix, StringComparison.Ordinal)
                || fieldName.Length <= Prefix.Length + Suffix.Length)
            {
                return false;
            }

            var indexText = fieldName.Substring(
                Prefix.Length,
                fieldName.Length - Prefix.Length - Suffix.Length);
            return int.TryParse(indexText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index)
                && index >= 0;
        }
    }
}
