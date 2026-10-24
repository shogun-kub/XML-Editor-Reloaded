using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Overlays;

/// <summary>
/// Compares a base table with a language overlay and reports missing, orphaned, duplicate,
/// empty, identical, and structurally mismatched translations.
/// </summary>
public sealed class TranslationAnalyzer
{
    /// <summary>
    /// Analyzes an overlay against its base document.
    /// </summary>
    /// <param name="baseDocument">Loaded base table document with metadata.</param>
    /// <param name="overlayDocument">Loaded overlay document.</param>
    /// <returns>Translation issues in discovery order.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either document is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the base document has no metadata.</exception>
    public TranslationReport Analyze(TableDocument baseDocument, TableDocument overlayDocument)
    {
        ArgumentNullException.ThrowIfNull(baseDocument);
        ArgumentNullException.ThrowIfNull(overlayDocument);

        var metadata = baseDocument.Metadata
            ?? throw new InvalidOperationException("Translation analysis requires base table metadata.");
        var resolver = new OverlayKeyResolver(metadata);
        var baseRecords = resolver.ResolvePrimary(baseDocument.Xml);
        var overlayRecords = resolver.ResolvePrimary(overlayDocument.Xml);

        var issues = new List<TranslationIssue>();
        var baseByKey = baseRecords
            .Where(static record => record.Key is not null)
            .GroupBy(static record => record.Key!, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.Ordinal);
        var overlayGroups = overlayRecords
            .Where(static record => record.Key is not null)
            .GroupBy(static record => record.Key!, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.ToArray(), StringComparer.Ordinal);

        foreach (var record in overlayRecords.Where(static record => !record.IsKeyResolved))
        {
            issues.Add(new TranslationIssue(
                TranslationIssueKind.UnresolvableKey,
                record.Key,
                null,
                "An overlay record key could not be resolved."));
        }

        foreach (var group in overlayGroups.Values.Where(static group => group.Length > 1))
        {
            issues.Add(new TranslationIssue(
                TranslationIssueKind.DuplicateKey,
                group[0].Key,
                null,
                $"Overlay key '{group[0].Key}' occurs {group.Length} times."));
        }

        foreach (var baseRecord in baseRecords)
        {
            if (baseRecord.Key is null)
            {
                issues.Add(new TranslationIssue(
                    TranslationIssueKind.UnresolvableKey,
                    null,
                    null,
                    "A base record key could not be resolved."));
                continue;
            }

            if (!overlayGroups.ContainsKey(baseRecord.Key))
            {
                issues.Add(new TranslationIssue(
                    TranslationIssueKind.MissingTranslation,
                    baseRecord.Key,
                    null,
                    $"Base record '{baseRecord.Key}' has no overlay translation."));
            }
        }

        foreach (var overlayKey in overlayGroups.Keys)
        {
            if (!baseByKey.ContainsKey(overlayKey))
            {
                issues.Add(new TranslationIssue(
                    TranslationIssueKind.OrphanKey,
                    overlayKey,
                    null,
                    $"Overlay key '{overlayKey}' does not exist in the base table."));
            }
        }

        AddNestedCountIssues(metadata, resolver, baseRecords, overlayGroups, issues);

        var merged = new OverlayMerger(metadata).Merge(baseDocument, overlayDocument);
        foreach (var record in merged.Records)
        {
            AddCellIssues(record.Key, record.Cells, issues);
        }

        foreach (var cell in merged.RootCells)
        {
            AddCellIssues(null, new[] { cell }, issues);
        }

        return new TranslationReport(issues);
    }

    private static void AddNestedCountIssues(
        TableMetadata metadata,
        OverlayKeyResolver resolver,
        IReadOnlyList<ResolvedRecord> baseRecords,
        IReadOnlyDictionary<string, ResolvedRecord[]> overlayGroups,
        List<TranslationIssue> issues)
    {
        var segments = metadata.RecordKey?.Segments;
        if (segments is null)
        {
            return;
        }

        foreach (var baseRecord in baseRecords)
        {
            if (baseRecord.Key is null || !overlayGroups.TryGetValue(baseRecord.Key, out var overlayRecords))
            {
                continue;
            }

            var overlayRecord = overlayRecords[0];
            for (var index = 1; index < segments.Count; index++)
            {
                var segment = segments[index];
                if (segment.Kind == RecordKeyKind.Singleton)
                {
                    continue;
                }

                var baseNested = resolver.ResolveNested(baseRecord.Element, index);
                var overlayNested = resolver.ResolveNested(overlayRecord.Element, index);
                if (baseNested.Count == overlayNested.Count)
                {
                    continue;
                }

                issues.Add(new TranslationIssue(
                    TranslationIssueKind.KeyCountMismatch,
                    baseRecord.Key,
                    segment.Path,
                    $"Nested collection '{segment.Path}' has {baseNested.Count} base entries but {overlayNested.Count} overlay entries."));
            }
        }
    }

    private static void AddCellIssues(string? recordKey, IReadOnlyList<MergedCell> cells, List<TranslationIssue> issues)
    {
        foreach (var cell in cells)
        {
            if (cell.OverlayValue is null)
            {
                continue;
            }

            if (cell.OverlayValue.Length == 0)
            {
                issues.Add(new TranslationIssue(
                    TranslationIssueKind.EmptyValue,
                    recordKey,
                    cell.FieldPath,
                    $"Translation for '{cell.FieldPath}' is empty."));
            }
            else if (string.Equals(cell.OverlayValue, cell.BaseValue, StringComparison.Ordinal))
            {
                issues.Add(new TranslationIssue(
                    TranslationIssueKind.IdenticalToBase,
                    recordKey,
                    cell.FieldPath,
                    $"Translation for '{cell.FieldPath}' is identical to the base value."));
            }
        }
    }
}
