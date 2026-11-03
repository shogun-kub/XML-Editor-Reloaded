using System.Globalization;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Schema;

/// <summary>
/// Builds a <see cref="TableSchema"/> from a loaded document. Fields are discovered as the union of
/// all records in first-appearance order; records of a nested key path are resolved through metadata.
/// Curated column metadata overrides types and editor hints, contributes declared fields that the
/// document does not currently contain, and non-declaring column hints refine fields that are present.
/// A mapped table with no record at all falls back to those hints, which then describe the starter
/// fields of the first row to be added.
/// </summary>
public sealed class TableSchemaBuilder
{
    private const NumberStyles DecimalStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    private static bool HasNestedRecordKey(TableMetadata metadata) =>
        metadata.RecordKey is { Segments.Count: > 0 } spec
        && !string.IsNullOrWhiteSpace(spec.Primary.Path);

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
        var recordElements = ResolveRecordElements(document);

        foreach (var recordElement in recordElements)
        {
            var occurrences = new Dictionary<string, int>(StringComparer.Ordinal);
            if (!recordElement.HasElements)
            {
                // Some tables store one text value per record instead of a container of fields, for
                // example <SOUND>SOUNDS\DOOR OPEN 01.wav</SOUND>. Expose that text as a virtual column
                // so such tables stay editable without inventing XML elements.
                if (recordElement.Nodes().Any(static node => node is XText text && !string.IsNullOrWhiteSpace(text.Value)))
                {
                    if (!statistics.TryGetValue(RecordFields.RecordText, out var textStats))
                    {
                        textStats = new ColumnStatistics();
                        statistics.Add(RecordFields.RecordText, textStats);
                        order.Add(RecordFields.RecordText);
                    }

                    textStats.PresenceCount++;
                    textStats.MaxOccurrences = 1;
                    ObserveValue(textStats, recordElement.Value);
                }

                continue;
            }

            foreach (var element in recordElement.Elements())
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
                var isNestedElement = element.HasElements;
                columnStats.HasNested |= isNestedElement;
                if (!isNestedElement)
                {
                    ObserveValue(columnStats, element.Value);
                }
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

        if (statistics.Count == 0 && metadata is not null && !HasNestedRecordKey(metadata))
        {
            // A mapped table whose last row was deleted has no document field to discover. Curated hints
            // stay non-declaring while rows exist, but without any row they are the only description of
            // the table: without this fallback the grid would have no column at all and the first row
            // could not be filled in. Key fields come first so the new row's identity is visible.
            var starterKeyFields = metadata.RecordKeyFields.Count > 0
                ? metadata.RecordKeyFields.ToHashSet(StringComparer.Ordinal)
                : metadata.RecordKey?.Primary.KeyField is { } primaryKey
                    ? new HashSet<string>(StringComparer.Ordinal) { primaryKey }
                    : new HashSet<string>(StringComparer.Ordinal);
            foreach (var hint in metadata.ColumnHints
                         .OrderBy(hint => starterKeyFields.Contains(hint.XmlFieldName) ? 0 : 1)
                         .ThenBy(static hint => hint.XmlFieldName, StringComparer.Ordinal))
            {
                if (statistics.ContainsKey(hint.XmlFieldName))
                {
                    continue;
                }

                statistics[hint.XmlFieldName] = new ColumnStatistics();
                order.Add(hint.XmlFieldName);
            }
        }

        var listFields = metadata?.ListFields
            .Select(static field => field.XmlFieldName)
            .ToHashSet(StringComparer.Ordinal);
        var dictPatterns = BuildDictPatterns(metadata);
        var translatableFields = metadata?.TranslatableFieldPaths
            .ToHashSet(StringComparer.Ordinal);
        var keyFields = metadata?.RecordKeyFields.ToHashSet(StringComparer.Ordinal)
            ?? new HashSet<string>(StringComparer.Ordinal);
        var isCompoundKey = keyFields.Count > 1;
        var keyField = metadata?.RecordKey?.Primary.KeyField;
        if (keyFields.Count == 0 && keyField is null && metadata is null && order.Count > 0)
        {
            // Document-driven tables have no key metadata; the first discovered field identifies rows.
            // A mapped table with a positional or singleton key must not promote its first field.
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

            ColumnHint? hint = null;
            metadata?.TryGetColumnHint(name, out hint);

            var isKey = keyFields.Contains(name)
                || string.Equals(name, keyField, StringComparison.Ordinal);
            var dataType = columnStats.HasNested
                ? ColumnDataType.Nested
                : hint?.DataType
                    ?? declared?.DataType
                    ?? columnStats.InferDataType(isKey);
            var editorKind = dataType == ColumnDataType.Nested
                ? ColumnEditorKind.SubTableGrid
                : hint?.EditorKind ?? declared?.EditorKind ?? DefaultEditorKind(dataType);
            // Translatable metadata paths may be qualified by the record path, for example
            // CITYLIST/CITY/townName; the discovered column carries only the leaf name, so a suffix
            // match marks the same field on the nested record schema.
            var isTranslatable = translatableFields?.Any(path =>
                    string.Equals(path, name, StringComparison.Ordinal)
                    || path.EndsWith("/" + name, StringComparison.Ordinal)) == true
                || hint?.Flags?.HasFlag(ColumnFlags.Translatable) == true
                || declared?.Flags.HasFlag(ColumnFlags.Translatable) == true;
            // A nested element has no cell editor yet: the grid shows it as an empty read-only
            // pseudo-column until the dedicated sub-table editor (M24a) exists. It stays in the schema
            // and the column chooser so the user can inspect or enable it explicitly.
            var isHidden = editorKind == ColumnEditorKind.Hidden
                || hint?.Flags?.HasFlag(ColumnFlags.Hidden) == true
                || declared?.Flags.HasFlag(ColumnFlags.Hidden) == true
                || columnStats.HasNested;
            var flags = (hint?.Flags ?? ColumnFlags.None) | (declared?.Flags ?? ColumnFlags.None);
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
            var isDict = dictMask is not null;
            var groupKey = ResolveGroupKey(hint, isKey, columnStats.HasNested, isList, isDict, dataType);
            var groupOrder = ResolveGroupOrder(hint, groupKey);

            columns.Add(new TableSchemaColumn(
                name,
                dataType,
                editorKind,
                index,
                columnStats.PresenceCount,
                columnStats.MaxOccurrences,
                isKey,
                isKey && isCompoundKey,
                isList,
                isDict,
                dictMask,
                isTranslatable,
                isHidden,
                hint?.DisplayWidth ?? declared?.DisplayWidth,
                hint?.LookupTable ?? declared?.LookupTable,
                hint?.TooltipKey ?? declared?.TooltipKey,
                flags,
                columnStats.HasNested,
                groupKey,
                groupOrder));
        }

        return new TableSchema(
            document.Descriptor.TableName,
            document.RecordElementName,
            recordElements.Count,
            metadata is null || metadata.Columns.Count == 0,
            columns);
    }

    /// <summary>
    /// Returns the elements the schema fields are discovered from. A table with a nested record key
    /// keeps its records inside a container, for example <c>CITYLIST/CITY</c> in <c>Map.Cities</c>, so
    /// <see cref="TableDocument.Records"/> is empty; the declared key path is resolved instead so the
    /// grid shows the real fields of those nested records. An empty nested table still yields no
    /// elements, which keeps the curated hints from declaring columns for it.
    /// </summary>
    private static IReadOnlyList<XElement> ResolveRecordElements(TableDocument document)
    {
        if (document.Metadata is { } metadata
            && metadata.RecordKey?.Primary.Path is { Length: > 0 })
        {
            var resolved = new OverlayKeyResolver(metadata).ResolvePrimary(document.Xml);
            if (resolved.Count > 0)
            {
                return resolved.Select(static record => record.Element).ToArray();
            }
        }

        return document.Records.Select(static record => record.Element).ToArray();
    }

    private static ColumnEditorKind DefaultEditorKind(ColumnDataType dataType) => dataType switch
    {
        ColumnDataType.Integer => ColumnEditorKind.IntegerBox,
        ColumnDataType.Decimal => ColumnEditorKind.DecimalBox,
        ColumnDataType.Lookup => ColumnEditorKind.LookupCombo,
        ColumnDataType.Flags => ColumnEditorKind.FlagEditor,
        ColumnDataType.Nested => ColumnEditorKind.SubTableGrid,
        _ => ColumnEditorKind.TextBox,
    };

    private static string ResolveGroupKey(
        ColumnHint? hint,
        bool isKey,
        bool isNested,
        bool isList,
        bool isDict,
        ColumnDataType dataType)
    {
        if (!string.IsNullOrWhiteSpace(hint?.GroupKey))
        {
            return hint!.GroupKey!;
        }

        if (isKey)
        {
            return "identity";
        }

        if (isNested)
        {
            return "nested";
        }

        if (isList || isDict)
        {
            return "lists";
        }

        return dataType switch
        {
            ColumnDataType.Boolean => "flags",
            ColumnDataType.String or ColumnDataType.Raw => "text",
            _ => "values",
        };
    }

    private static int ResolveGroupOrder(ColumnHint? hint, string groupKey)
    {
        if (hint is { GroupOrder: > 0 })
        {
            return hint.GroupOrder;
        }

        return groupKey switch
        {
            "identity" => 10,
            "text" => 20,
            "values" => 30,
            "flags" => 100,
            "lists" => 200,
            "nested" => 210,
            _ => 250,
        };
    }

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

        public bool HasNested { get; set; }

        public ColumnMetadata? DeclaredColumn { get; set; }

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
