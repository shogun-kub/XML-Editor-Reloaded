using System.Globalization;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Xml.Overlays;

/// <summary>
/// A record element together with the key resolved from a <see cref="RecordKeySpec"/>.
/// </summary>
/// <param name="Key">Resolved key text, or <see langword="null"/> when the key could not be resolved.</param>
/// <param name="Element">XML element representing the record or nested entry.</param>
/// <param name="IsKeyResolved">Whether <paramref name="Key"/> was derived successfully.</param>
public sealed record ResolvedRecord(string? Key, XElement Element, bool IsKeyResolved)
{
    /// <summary>
    /// Gets the compound key fields that are missing or empty when the key is incomplete; empty for a
    /// resolved key and for tables without a compound key.
    /// </summary>
    public IReadOnlyList<string> MissingKeyFields { get; init; } = Array.Empty<string>();
}

/// <summary>
/// Resolves primary and nested record keys for a table and its language overlays.
/// </summary>
public sealed class OverlayKeyResolver
{
    private readonly TableMetadata _metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="OverlayKeyResolver"/> class.
    /// </summary>
    /// <param name="metadata">Table metadata containing the key specification.</param>
    public OverlayKeyResolver(TableMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        _metadata = metadata;
    }

    /// <summary>
    /// Resolves the primary records of a document.
    /// </summary>
    /// <param name="document">Base or overlay document.</param>
    /// <returns>Resolved records in document order.</returns>
    public IReadOnlyList<ResolvedRecord> ResolvePrimary(XDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var root = document.Root;
        if (root is null)
        {
            return Array.Empty<ResolvedRecord>();
        }

        if (_metadata.HasCompoundRecordKey)
        {
            return root.Elements()
                .Where(element => string.Equals(element.Name.LocalName, _metadata.RecordElement, StringComparison.Ordinal))
                .Select(CreateCompoundResolved)
                .ToArray();
        }

        if (_metadata.RecordKeyFields is { Count: 1 } single)
        {
            var field = single[0];
            return root.Elements()
                .Where(element => string.Equals(element.Name.LocalName, _metadata.RecordElement, StringComparison.Ordinal))
                .Select(element =>
                {
                    var value = element.Elements()
                        .FirstOrDefault(child => string.Equals(child.Name.LocalName, field, StringComparison.Ordinal))
                        ?.Value;
                    return new ResolvedRecord(
                        string.IsNullOrEmpty(value) ? null : value,
                        element,
                        !string.IsNullOrEmpty(value));
                })
                .ToArray();
        }

        var spec = _metadata.RecordKey;
        if (spec is null)
        {
            return root.Elements()
                .Where(element => string.Equals(element.Name.LocalName, _metadata.RecordElement, StringComparison.Ordinal))
                .Select(static element =>
                {
                    var first = element.Elements().FirstOrDefault()?.Value;
                    return new ResolvedRecord(first, element, !string.IsNullOrEmpty(first));
                })
                .ToArray();
        }

        var segment = spec.Primary;
        return ResolveElements(root, segment.Path, _metadata.RecordElement)
            .Select(element => CreateResolved(element, segment))
            .ToArray();
    }

    /// <summary>
    /// Resolves a nested collection of a record, for example <c>GEARKIT</c> entries or <c>ASSIGNMENT</c> children.
    /// </summary>
    /// <param name="record">Parent record element.</param>
    /// <param name="segmentIndex">Index of the key segment in the specification (primary is 0).</param>
    /// <returns>Resolved nested entries in document order.</returns>
    public IReadOnlyList<ResolvedRecord> ResolveNested(XElement record, int segmentIndex)
    {
        ArgumentNullException.ThrowIfNull(record);
        var spec = _metadata.RecordKey;
        if (spec is null || segmentIndex < 0 || segmentIndex >= spec.Segments.Count)
        {
            return Array.Empty<ResolvedRecord>();
        }

        var segment = spec.Segments[segmentIndex];
        return ResolveElements(record, segment.Path, recordElement: null)
            .Select(element => CreateResolved(element, segment))
            .ToArray();
    }

    /// <summary>
    /// Resolves a root-level singleton segment such as <c>COUNTRY</c> in <c>CITY_INFO</c>.
    /// </summary>
    /// <param name="document">Document containing the singleton.</param>
    /// <param name="segmentIndex">Index of the singleton key segment.</param>
    /// <returns>The resolved singleton, or <see langword="null"/> when absent.</returns>
    public ResolvedRecord? ResolveSingleton(XDocument document, int segmentIndex)
    {
        ArgumentNullException.ThrowIfNull(document);
        var root = document.Root;
        var spec = _metadata.RecordKey;
        if (root is null || spec is null || segmentIndex < 0 || segmentIndex >= spec.Segments.Count)
        {
            return null;
        }

        var segment = spec.Segments[segmentIndex];
        var element = ResolveElements(root, segment.Path, recordElement: null).FirstOrDefault();
        return element is null ? null : CreateResolved(element, segment);
    }

    /// <summary>
    /// Finds a primary record by key.
    /// </summary>
    /// <param name="document">Document to search.</param>
    /// <param name="key">Key text to match with ordinal comparison.</param>
    /// <returns>The first matching record, or <see langword="null"/>.</returns>
    public XElement? FindPrimary(XDocument document, string key)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(key);
        return ResolvePrimary(document)
            .FirstOrDefault(record => string.Equals(record.Key, key, StringComparison.Ordinal))
            ?.Element;
    }

    private static IReadOnlyList<XElement> ResolveElements(XElement start, string path, string? recordElement)
    {
        if (string.IsNullOrEmpty(path))
        {
            return recordElement is null
                ? new[] { start }
                : start.Elements()
                    .Where(element => string.Equals(element.Name.LocalName, recordElement, StringComparison.Ordinal))
                    .ToArray();
        }

        var current = new List<XElement> { start };
        foreach (var part in path.Split('/'))
        {
            var next = new List<XElement>();
            foreach (var element in current)
            {
                next.AddRange(element.Elements()
                    .Where(child => string.Equals(child.Name.LocalName, part, StringComparison.Ordinal)));
            }

            current = next;
            if (current.Count == 0)
            {
                break;
            }
        }

        return current;
    }

    private ResolvedRecord CreateCompoundResolved(XElement element)
    {
        var components = new string?[_metadata.RecordKeyFields.Count];
        var missing = new List<string>();
        for (var index = 0; index < _metadata.RecordKeyFields.Count; index++)
        {
            var field = _metadata.RecordKeyFields[index];
            var value = element.Elements()
                .FirstOrDefault(child => string.Equals(child.Name.LocalName, field, StringComparison.Ordinal))
                ?.Value;
            components[index] = value;
            if (string.IsNullOrEmpty(value))
            {
                missing.Add(field);
            }
        }

        return new ResolvedRecord(
            missing.Count == 0 ? RecordKeyCodec.Compose(components) : null,
            element,
            missing.Count == 0)
        {
            MissingKeyFields = missing,
        };
    }

    private static ResolvedRecord CreateResolved(XElement element, RecordKeySegment segment)
    {
        switch (segment.Kind)
        {
            case RecordKeyKind.Scalar:
                var value = element.Elements()
                    .FirstOrDefault(child => string.Equals(child.Name.LocalName, segment.KeyField, StringComparison.Ordinal))
                    ?.Value;
                return new ResolvedRecord(
                    string.IsNullOrEmpty(value) ? null : value,
                    element,
                    !string.IsNullOrEmpty(value));

            case RecordKeyKind.Positional:
                var index = element.ElementsBeforeSelf()
                    .Count(sibling => string.Equals(sibling.Name.LocalName, element.Name.LocalName, StringComparison.Ordinal));
                return new ResolvedRecord(
                    index.ToString(CultureInfo.InvariantCulture),
                    element,
                    true);

            case RecordKeyKind.Singleton:
                return new ResolvedRecord(segment.Path, element, true);

            default:
                return new ResolvedRecord(null, element, false);
        }
    }
}
