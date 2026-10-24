using System.Globalization;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Overlays;

/// <summary>
/// Identifies a record in an overlay: the primary key plus optional nested keys for repeated groups.
/// </summary>
/// <param name="PrimaryKey">Primary record key, or <see langword="null"/> for a root-level path.</param>
/// <param name="NestedKeys">
/// Keys for nested key segments, in segment order after the primary segment.
/// For positional segments these are zero-based indices as text.
/// </param>
public sealed record OverlayRecordKey(string? PrimaryKey, IReadOnlyList<string>? NestedKeys = null)
{
    /// <summary>
    /// Creates a key for a top-level record.
    /// </summary>
    /// <param name="primaryKey">Primary record key text.</param>
    /// <returns>A key with no nested segments.</returns>
    public static OverlayRecordKey ForRecord(string primaryKey) => new(primaryKey);
}

/// <summary>
/// Writes translation edits into a language overlay, never adding untranslated base fields.
/// </summary>
public sealed class OverlayWriter
{
    /// <summary>
    /// Sets a translated field for an existing or newly created overlay record.
    /// </summary>
    /// <param name="overlay">Overlay to edit.</param>
    /// <param name="key">Primary and nested record keys.</param>
    /// <param name="path">Field path relative to the resolved nested record.</param>
    /// <param name="rawText">Translation text to store exactly.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>overlay.unresolvable_key</c> or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetValue(
        OverlayDocument overlay,
        OverlayRecordKey key,
        FieldPath path,
        string rawText)
    {
        ArgumentNullException.ThrowIfNull(overlay);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);

        if (key.PrimaryKey is null)
        {
            return SetRootValue(overlay, path, rawText);
        }

        var metadata = overlay.Metadata;
        var root = overlay.Xml.Root;
        if (root is null)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The overlay document has no root element.");
        }

        var resolver = new OverlayKeyResolver(metadata);
        var record = resolver.FindPrimary(overlay.Xml, key.PrimaryKey)
            ?? CreatePrimaryRecord(root, metadata, key.PrimaryKey);

        var remainingPath = path;
        var segments = metadata.RecordKey?.Segments;
        if (segments is not null)
        {
            for (var index = 1; index < segments.Count; index++)
            {
                var segment = segments[index];
                if (segment.Kind == RecordKeyKind.Singleton)
                {
                    continue;
                }

                var nestedKey = key.NestedKeys is not null && key.NestedKeys.Count >= index
                    ? key.NestedKeys[index - 1]
                    : null;
                if (nestedKey is null)
                {
                    return OperationResult.Failure(
                        ErrorCodes.OverlayUnresolvableKey,
                        $"Nested key for segment '{segment.Path}' is required.");
                }

                var nested = FindOrCreateNested(record, segment, nestedKey);
                if (nested is null)
                {
                    return OperationResult.Failure(
                        ErrorCodes.OverlayUnresolvableKey,
                        $"Nested record '{segment.Path}' with key '{nestedKey}' could not be resolved.");
                }

                record = nested;
                remainingPath = StripSegmentPrefix(remainingPath, segment.Path);
            }
        }

        return SetLeaf(overlay, record, remainingPath, rawText, key.PrimaryKey);
    }

    /// <summary>
    /// Sets a root-level translated field, for example <c>COUNTRY/countryName</c>.
    /// </summary>
    /// <param name="overlay">Overlay to edit.</param>
    /// <param name="path">Slash-separated path from the document root.</param>
    /// <param name="rawText">Translation text to store exactly.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetRootValue(OverlayDocument overlay, FieldPath path, string rawText)
    {
        ArgumentNullException.ThrowIfNull(overlay);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);

        var root = overlay.Xml.Root;
        if (root is null)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The overlay document has no root element.");
        }

        return SetLeaf(overlay, root, path, rawText, recordKey: string.Empty);
    }

    private static OperationResult SetLeaf(
        OverlayDocument overlay,
        XElement root,
        FieldPath path,
        string rawText,
        string recordKey)
    {
        var current = root;
        for (var index = 0; index < path.Depth - 1; index++)
        {
            var segment = path.Segments[index];
            var child = current.Elements()
                .FirstOrDefault(element => string.Equals(element.Name.LocalName, segment, StringComparison.Ordinal));
            if (child is null)
            {
                child = new XElement(XName.Get(segment));
                current.Add(child);
            }

            current = child;
        }

        var fieldName = path.FieldName;
        var leaf = current.Elements()
            .FirstOrDefault(element => string.Equals(element.Name.LocalName, fieldName, StringComparison.Ordinal));
        if (leaf is null)
        {
            leaf = new XElement(XName.Get(fieldName));
            current.Add(leaf);
        }

        if (leaf.HasElements)
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                $"Field '{path.Text}' contains nested elements and cannot be set as translation text.");
        }

        var oldValue = leaf.Value;
        if (string.Equals(oldValue, rawText, StringComparison.Ordinal))
        {
            return OperationResult.Success();
        }

        leaf.Value = rawText;
        overlay.MarkDirty();
        overlay.Document.ChangeTracker.Record(
            new FieldValueChanged(recordKey, path.Text, oldValue, rawText));
        return OperationResult.Success();
    }

    private static XElement CreatePrimaryRecord(XElement root, TableMetadata metadata, string primaryKey)
    {
        var primary = metadata.RecordKey!.Primary;
        XElement record;
        if (string.IsNullOrEmpty(primary.Path))
        {
            record = new XElement(XName.Get(metadata.RecordElement));
            root.Add(record);
        }
        else
        {
            var parts = primary.Path.Split('/');
            var parent = root;
            for (var index = 0; index < parts.Length - 1; index++)
            {
                var child = parent.Elements()
                    .FirstOrDefault(element => string.Equals(element.Name.LocalName, parts[index], StringComparison.Ordinal));
                if (child is null)
                {
                    child = new XElement(XName.Get(parts[index]));
                    parent.Add(child);
                }

                parent = child;
            }

            record = new XElement(XName.Get(parts[^1]));
            parent.Add(record);
        }

        if (!string.IsNullOrEmpty(primary.KeyField))
        {
            record.Add(new XElement(XName.Get(primary.KeyField), primaryKey));
        }

        return record;
    }

    private static XElement? FindOrCreateNested(XElement parent, RecordKeySegment segment, string nestedKey)
    {
        if (string.IsNullOrEmpty(segment.Path))
        {
            return parent;
        }

        var parts = segment.Path.Split('/');
        var container = parent;
        for (var index = 0; index < parts.Length - 1; index++)
        {
            var child = container.Elements()
                .FirstOrDefault(element => string.Equals(element.Name.LocalName, parts[index], StringComparison.Ordinal));
            if (child is null)
            {
                child = new XElement(XName.Get(parts[index]));
                container.Add(child);
            }

            container = child;
        }

        var collectionName = parts[^1];
        return segment.Kind switch
        {
            RecordKeyKind.Positional => FindOrCreatePositional(container, collectionName, nestedKey),
            RecordKeyKind.Scalar => FindOrCreateScalar(container, collectionName, segment.KeyField!, nestedKey),
            RecordKeyKind.Singleton => FindOrCreateSingleton(container, collectionName),
            _ => null,
        };
    }

    private static XElement? FindOrCreatePositional(XElement container, string collectionName, string nestedKey)
    {
        if (!int.TryParse(nestedKey, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index) || index < 0)
        {
            return null;
        }

        var children = container.Elements()
            .Where(element => string.Equals(element.Name.LocalName, collectionName, StringComparison.Ordinal))
            .ToList();
        while (children.Count <= index)
        {
            var child = new XElement(XName.Get(collectionName));
            container.Add(child);
            children.Add(child);
        }

        return children[index];
    }

    private static XElement FindOrCreateScalar(
        XElement container,
        string collectionName,
        string keyField,
        string nestedKey)
    {
        var existing = container.Elements()
            .FirstOrDefault(element =>
                string.Equals(element.Name.LocalName, collectionName, StringComparison.Ordinal)
                && string.Equals(
                    element.Elements().FirstOrDefault(child =>
                        string.Equals(child.Name.LocalName, keyField, StringComparison.Ordinal))?.Value,
                    nestedKey,
                    StringComparison.Ordinal));
        if (existing is not null)
        {
            return existing;
        }

        var created = new XElement(XName.Get(collectionName), new XElement(XName.Get(keyField), nestedKey));
        container.Add(created);
        return created;
    }

    private static XElement FindOrCreateSingleton(XElement container, string collectionName)
    {
        var existing = container.Elements()
            .FirstOrDefault(element => string.Equals(element.Name.LocalName, collectionName, StringComparison.Ordinal));
        if (existing is not null)
        {
            return existing;
        }

        var created = new XElement(XName.Get(collectionName));
        container.Add(created);
        return created;
    }

    private static FieldPath StripSegmentPrefix(FieldPath path, string segmentPath)
    {
        if (string.IsNullOrEmpty(segmentPath))
        {
            return path;
        }

        var prefix = segmentPath + "/";
        return path.Text.StartsWith(prefix, StringComparison.Ordinal)
            ? FieldPath.Parse(path.Text[prefix.Length..])
            : path;
    }
}
