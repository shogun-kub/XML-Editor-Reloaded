using System.Globalization;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Comparison;

/// <summary>
/// One leaf value of a record or of a root singleton element.
/// </summary>
/// <param name="Name">Field path relative to the record or singleton element, using slash separators.</param>
/// <param name="Value">Truncated XML text of the leaf.</param>
internal sealed record TableFieldValue(string Name, string Value);

/// <summary>
/// Cells of one record together with its identity and document position.
/// </summary>
/// <param name="Identity">Canonical record identity used for matching; positional identity is <c>#N</c>.</param>
/// <param name="Display">User-facing key text.</param>
/// <param name="Order">Zero-based record position in document order.</param>
/// <param name="Fields">Leaf cells in document order; repeated names are kept as separate entries.</param>
internal sealed record TableRecordCells(
    string Identity,
    string Display,
    int Order,
    IReadOnlyList<TableFieldValue> Fields);

/// <summary>
/// Leaf cells of one document, split into records and root singleton fields.
/// </summary>
/// <param name="IdentityKind">How record identities were produced.</param>
/// <param name="Records">Records in document order.</param>
/// <param name="RootFields">Leaf cells of declared root singletons, with paths such as <c>COUNTRY/countryName</c>.</param>
internal sealed record TableCells(
    RecordIdentityKind IdentityKind,
    IReadOnlyList<TableRecordCells> Records,
    IReadOnlyList<TableFieldValue> RootFields)
{
    /// <summary>
    /// Gets the total number of leaf cells, including records and root fields.
    /// </summary>
    public int CellCount =>
        Records.Sum(static record => record.Fields.Count) + RootFields.Count;
}

/// <summary>
/// Reads leaf cells from a loaded table or overlay document. Values are compared as exact XML text:
/// nested elements are traversed to their leaves, and repeated elements keep their order.
/// </summary>
internal static class TableCellsReader
{
    /// <summary>
    /// Reads the cells of a loaded table or overlay document.
    /// </summary>
    /// <param name="document">Loaded document; its metadata defines record identity and nested segments.</param>
    /// <returns>Record and root singleton cells.</returns>
    public static TableCells Read(TableDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var metadata = document.Metadata;
        var identityKind = metadata?.HasRecordKey == true
            ? RecordIdentityKind.DeclaredKey
            : RecordIdentityKind.Positional;
        var resolver = metadata is null ? null : new OverlayKeyResolver(metadata);
        var records = new List<TableRecordCells>(document.Records.Count);

        for (var index = 0; index < document.Records.Count; index++)
        {
            var record = document.Records[index];
            var identity = identityKind == RecordIdentityKind.DeclaredKey ? record.GetKey() : null;
            var display = identityKind == RecordIdentityKind.DeclaredKey ? record.KeyDisplay : string.Empty;
            if (string.IsNullOrEmpty(identity))
            {
                identity = string.Concat("#", (index + 1).ToString(CultureInfo.InvariantCulture));
                display = identity;
            }
            else if (display.Length == 0)
            {
                display = identity;
            }

            var fields = new List<TableFieldValue>();
            CollectFields(record.Element, string.Empty, 1, metadata, resolver, fields);
            records.Add(new TableRecordCells(identity, display, index, fields));
        }

        return new TableCells(identityKind, records, ReadRootFields(document, metadata, resolver));
    }

    /// <summary>
    /// Reads the cells of an overlay document whose base table has no metadata. Records are matched by
    /// their position because no key is declared.
    /// </summary>
    /// <param name="document">Parsed overlay document.</param>
    /// <param name="recordElementName">Name of the record element inferred from the base table.</param>
    /// <returns>Positional record cells without root singleton fields.</returns>
    public static TableCells Read(XDocument document, string recordElementName)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(recordElementName);

        var root = document.Root;
        if (root is null)
        {
            return new TableCells(
                RecordIdentityKind.Positional,
                Array.Empty<TableRecordCells>(),
                Array.Empty<TableFieldValue>());
        }

        var elements = root.Elements()
            .Where(element => string.Equals(element.Name.LocalName, recordElementName, StringComparison.Ordinal))
            .ToList();
        if (elements.Count == 0)
        {
            elements = root.Elements().ToList();
        }

        var records = new List<TableRecordCells>(elements.Count);
        for (var index = 0; index < elements.Count; index++)
        {
            var identity = string.Concat("#", (index + 1).ToString(CultureInfo.InvariantCulture));
            var fields = new List<TableFieldValue>();
            CollectFields(elements[index], string.Empty, 0, metadata: null, resolver: null, fields);
            records.Add(new TableRecordCells(identity, identity, index, fields));
        }

        return new TableCells(
            RecordIdentityKind.Positional,
            records,
            Array.Empty<TableFieldValue>());
    }

    /// <summary>
    /// Collects the leaf cells of a record or singleton, following declared nested segments and
    /// recursing through undeclared nested elements.
    /// </summary>
    /// <param name="parent">Record, singleton, or nested element to read.</param>
    /// <param name="prefix">Path prefix of the parent, ending with a slash when non-empty.</param>
    /// <param name="segmentIndex">Index of the key segment that describes the children of the parent.</param>
    /// <param name="metadata">Table metadata, or <see langword="null"/> for a document-driven read.</param>
    /// <param name="resolver">Key resolver of the metadata, or <see langword="null"/> when metadata is absent.</param>
    /// <param name="fields">Destination list.</param>
    private static void CollectFields(
        XElement parent,
        string prefix,
        int segmentIndex,
        TableMetadata? metadata,
        OverlayKeyResolver? resolver,
        List<TableFieldValue> fields)
    {
        RecordKeySegment? declared = null;
        if (metadata?.RecordKey is { } spec && segmentIndex < spec.Segments.Count)
        {
            var candidate = spec.Segments[segmentIndex];
            if (candidate.Kind != RecordKeyKind.Singleton)
            {
                declared = candidate;
            }
        }

        var containerName = declared is null ? null : FirstPathPart(declared.Path);
        foreach (var child in parent.Elements())
        {
            if (containerName is not null
                && string.Equals(child.Name.LocalName, containerName, StringComparison.Ordinal))
            {
                continue;
            }

            AppendElement(child, prefix, fields);
        }

        if (declared is null || resolver is null)
        {
            return;
        }

        foreach (var nested in resolver.ResolveNested(parent, segmentIndex))
        {
            var key = nested.Key ?? "?";
            var nestedPrefix = string.Concat(prefix, declared.Path, "[", key, "]/");
            CollectFields(nested.Element, nestedPrefix, segmentIndex + 1, metadata, resolver, fields);
        }
    }

    /// <summary>
    /// Appends every leaf of an element, recursing through nested elements that are not declared
    /// collections.
    /// </summary>
    /// <param name="element">Element to read.</param>
    /// <param name="prefix">Path prefix of the element's parent.</param>
    /// <param name="fields">Destination list.</param>
    private static void AppendElement(XElement element, string prefix, List<TableFieldValue> fields)
    {
        var name = string.Concat(prefix, element.Name.LocalName);
        if (!element.HasElements)
        {
            fields.Add(new TableFieldValue(name, ComparisonText.Truncate(element.Value)));
            return;
        }

        var childPrefix = string.Concat(name, "/");
        foreach (var child in element.Elements())
        {
            AppendElement(child, childPrefix, fields);
        }
    }

    /// <summary>
    /// Reads the leaf cells of the declared root singleton segments, for example <c>COUNTRY</c>.
    /// </summary>
    /// <param name="document">Loaded document.</param>
    /// <param name="metadata">Table metadata.</param>
    /// <param name="resolver">Key resolver of the metadata.</param>
    /// <returns>Root field cells in declaration order.</returns>
    private static IReadOnlyList<TableFieldValue> ReadRootFields(
        TableDocument document,
        TableMetadata? metadata,
        OverlayKeyResolver? resolver)
    {
        if (metadata?.RecordKey is not { } spec || resolver is null)
        {
            return Array.Empty<TableFieldValue>();
        }

        var fields = new List<TableFieldValue>();
        for (var index = 1; index < spec.Segments.Count; index++)
        {
            var segment = spec.Segments[index];
            if (segment.Kind != RecordKeyKind.Singleton)
            {
                continue;
            }

            var element = resolver.ResolveSingleton(document.Xml, index)?.Element;
            if (element is null)
            {
                continue;
            }

            CollectFields(
                element,
                string.Concat(segment.Path, "/"),
                index + 1,
                metadata,
                resolver,
                fields);
        }

        return fields;
    }

    private static string FirstPathPart(string path)
    {
        var separator = path.IndexOf('/', StringComparison.Ordinal);
        return separator < 0 ? path : path[..separator];
    }
}

/// <summary>
/// Text helpers shared by the comparison readers and comparers.
/// </summary>
internal static class ComparisonText
{
    /// <summary>
    /// Truncates a value to <see cref="TableComparisonLimits.MaxValueLength"/> characters and marks the
    /// truncation with an ellipsis so a report never silently claims a complete value.
    /// </summary>
    /// <param name="value">Value to truncate.</param>
    /// <returns>The original value when short enough; otherwise a truncated value ending with an ellipsis.</returns>
    public static string Truncate(string value)
    {
        if (value.Length <= TableComparisonLimits.MaxValueLength)
        {
            return value;
        }

        return string.Concat(
            value.AsSpan(0, TableComparisonLimits.MaxValueLength - 1),
            "\u2026");
    }
}
