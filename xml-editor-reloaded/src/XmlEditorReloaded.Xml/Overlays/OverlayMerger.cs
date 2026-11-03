using System.Xml.Linq;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Overlays;

/// <summary>
/// A merged field value showing the base value and the optional translated overlay value.
/// </summary>
/// <param name="FieldPath">Slash-separated path, with bracketed keys for nested collections.</param>
/// <param name="BaseValue">Base table value, or <see langword="null"/> when the field exists only in the overlay.</param>
/// <param name="OverlayValue">Overlay value, or <see langword="null"/> when the field is untranslated.</param>
public sealed record MergedCell(string FieldPath, string? BaseValue, string? OverlayValue)
{
    /// <summary>
    /// Gets the effective display value: the overlay value when present, otherwise the base value.
    /// </summary>
    public string Value => OverlayValue ?? BaseValue ?? string.Empty;

    /// <summary>
    /// Gets a value indicating whether the overlay contains this field.
    /// </summary>
    public bool IsTranslated => OverlayValue is not null;
}

/// <summary>
/// A base record merged with its corresponding overlay record.
/// </summary>
public sealed class MergedRecord
{
    internal MergedRecord(string? key, XElement baseElement, XElement? overlayElement, IReadOnlyList<MergedCell> cells)
    {
        Key = key;
        BaseElement = baseElement;
        OverlayElement = overlayElement;
        Cells = cells;
    }

    /// <summary>
    /// Gets the primary record key, or <see langword="null"/> when it could not be resolved.
    /// </summary>
    public string? Key { get; }

    /// <summary>
    /// Gets the base record element.
    /// </summary>
    public XElement BaseElement { get; }

    /// <summary>
    /// Gets the corresponding overlay record element, or <see langword="null"/> when untranslated.
    /// </summary>
    public XElement? OverlayElement { get; }

    /// <summary>
    /// Gets the merged cells of the record, including nested translated fields.
    /// </summary>
    public IReadOnlyList<MergedCell> Cells { get; }
}

/// <summary>
/// Result of merging a base table with a language overlay.
/// </summary>
public sealed class MergedTable
{
    internal MergedTable(IReadOnlyList<MergedRecord> records, IReadOnlyList<MergedCell> rootCells)
    {
        Records = records;
        RootCells = rootCells;
    }

    /// <summary>
    /// Gets the merged primary records in base-document order.
    /// </summary>
    public IReadOnlyList<MergedRecord> Records { get; }

    /// <summary>
    /// Gets merged root-level fields for singleton segments such as <c>COUNTRY</c>.
    /// </summary>
    public IReadOnlyList<MergedCell> RootCells { get; }
}

/// <summary>
/// Merges a base table with a language overlay by record keys and nested keys.
/// </summary>
public sealed class OverlayMerger
{
    private readonly TableMetadata _metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="OverlayMerger"/> class.
    /// </summary>
    /// <param name="metadata">Base table metadata.</param>
    public OverlayMerger(TableMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        _metadata = metadata;
    }

    /// <summary>
    /// Merges a base document with an optional overlay document.
    /// </summary>
    /// <param name="baseDocument">Base table document.</param>
    /// <param name="overlayDocument">Overlay document, or <see langword="null"/> for base-only values.</param>
    /// <returns>Merged records and root-level singleton cells.</returns>
    public MergedTable Merge(TableDocument baseDocument, TableDocument? overlayDocument)
    {
        ArgumentNullException.ThrowIfNull(baseDocument);

        var resolver = new OverlayKeyResolver(_metadata);
        var baseRecords = resolver.ResolvePrimary(baseDocument.Xml);
        var overlayRecords = overlayDocument is null
            ? Array.Empty<ResolvedRecord>()
            : resolver.ResolvePrimary(overlayDocument.Xml);
        var overlayByKey = overlayRecords
            .Where(static record => record.Key is not null)
            .GroupBy(static record => record.Key!, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.Ordinal);

        var records = new List<MergedRecord>(baseRecords.Count);
        foreach (var baseRecord in baseRecords)
        {
            var overlay = baseRecord.Key is not null && overlayByKey.TryGetValue(baseRecord.Key, out var match)
                ? match
                : null;
            var cells = new List<MergedCell>();
            CollectCells(baseRecord.Element, overlay?.Element, string.Empty, segmentIndex: 1, resolver, cells);
            records.Add(new MergedRecord(baseRecord.Key, baseRecord.Element, overlay?.Element, cells));
        }

        var rootCells = new List<MergedCell>();
        var segments = _metadata.RecordKey?.Segments;
        if (segments is not null)
        {
            for (var index = 1; index < segments.Count; index++)
            {
                if (segments[index].Kind != RecordKeyKind.Singleton)
                {
                    continue;
                }

                var baseSingleton = resolver.ResolveSingleton(baseDocument.Xml, index);
                var overlaySingleton = overlayDocument is null
                    ? null
                    : resolver.ResolveSingleton(overlayDocument.Xml, index);
                if (baseSingleton is null && overlaySingleton is null)
                {
                    continue;
                }

                var prefix = segments[index].Path + "/";
                CollectCells(
                    baseSingleton?.Element,
                    overlaySingleton?.Element,
                    prefix,
                    index + 1,
                    resolver,
                    rootCells);
            }
        }

        return new MergedTable(records, rootCells);
    }

    private void CollectCells(
        XElement? baseElement,
        XElement? overlayElement,
        string prefix,
        int segmentIndex,
        OverlayKeyResolver resolver,
        List<MergedCell> cells)
    {
        var baseLeaves = baseElement?.Elements().Where(static element => !element.HasElements).ToArray()
            ?? Array.Empty<XElement>();
        var overlayLeaves = overlayElement?.Elements().Where(static element => !element.HasElements).ToArray()
            ?? Array.Empty<XElement>();

        foreach (var baseLeaf in baseLeaves)
        {
            var name = baseLeaf.Name.LocalName;
            var overlayLeaf = overlayLeaves.FirstOrDefault(leaf =>
                string.Equals(leaf.Name.LocalName, name, StringComparison.Ordinal));
            cells.Add(new MergedCell(prefix + name, baseLeaf.Value, overlayLeaf?.Value));
        }

        foreach (var overlayLeaf in overlayLeaves)
        {
            if (baseLeaves.Any(leaf => string.Equals(leaf.Name.LocalName, overlayLeaf.Name.LocalName, StringComparison.Ordinal)))
            {
                continue;
            }

            cells.Add(new MergedCell(prefix + overlayLeaf.Name.LocalName, null, overlayLeaf.Value));
        }

        var segments = _metadata.RecordKey?.Segments;
        if (segments is null || segmentIndex >= segments.Count || baseElement is null)
        {
            return;
        }

        var segment = segments[segmentIndex];
        if (segment.Kind == RecordKeyKind.Singleton)
        {
            return;
        }

        var baseNested = resolver.ResolveNested(baseElement, segmentIndex);
        var overlayNested = overlayElement is null
            ? Array.Empty<ResolvedRecord>()
            : resolver.ResolveNested(overlayElement, segmentIndex);
        var overlayByKey = overlayNested
            .Where(static record => record.Key is not null)
            .GroupBy(static record => record.Key!, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.Ordinal);

        foreach (var nested in baseNested)
        {
            var nestedOverlay = nested.Key is not null && overlayByKey.TryGetValue(nested.Key, out var match)
                ? match
                : null;
            var nestedPrefix = prefix + segment.Path + "[" + (nested.Key ?? "?") + "]/";
            CollectCells(nested.Element, nestedOverlay?.Element, nestedPrefix, segmentIndex + 1, resolver, cells);
        }

        foreach (var nested in overlayNested)
        {
            if (nested.Key is not null && baseNested.Any(record => string.Equals(record.Key, nested.Key, StringComparison.Ordinal)))
            {
                continue;
            }

            var nestedPrefix = prefix + segment.Path + "[" + (nested.Key ?? "?") + "]/";
            CollectCells(null, nested.Element, nestedPrefix, segmentIndex + 1, resolver, cells);
        }
    }
}
