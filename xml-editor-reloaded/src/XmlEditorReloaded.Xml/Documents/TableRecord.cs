using System.Xml.Linq;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Values;

namespace XmlEditorReloaded.Xml.Documents;

/// <summary>
/// A single record inside a <see cref="TableDocument"/>. The record is a thin facade over its
/// original XML element and never materializes a fixed set of fields.
/// </summary>
public sealed class TableRecord
{
    private readonly TableDocument _owner;
    private readonly XElement _element;

    internal TableRecord(TableDocument owner, XElement element)
    {
        _owner = owner;
        _element = element;
    }

    /// <summary>
    /// Gets the distinct direct child element names of the record, in document order.
    /// Repeated elements appear once.
    /// </summary>
    public IReadOnlyList<string> FieldNames =>
        _element.Elements()
            .Select(static element => element.Name.LocalName)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

    /// <summary>
    /// Determines whether the record contains at least one direct child with the given name.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name.</param>
    /// <returns><see langword="true"/> when the field is present.</returns>
    public bool HasField(string fieldName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        return FindDirectChild(fieldName) is not null;
    }

    /// <summary>
    /// Gets the first value of a direct child field.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name.</param>
    /// <returns>The lexical value, or <see langword="null"/> when the field is absent.</returns>
    public LexicalValue? GetValue(string fieldName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        var child = FindDirectChild(fieldName);
        return child is null ? null : new LexicalValue(child.Value);
    }

    /// <summary>
    /// Gets the value at a nested field path, matching the first occurrence at every level.
    /// </summary>
    /// <param name="path">Slash-separated field path.</param>
    /// <returns>The lexical value, or <see langword="null"/> when the path is absent.</returns>
    public LexicalValue? GetValue(FieldPath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        var element = FindPath(path);
        return element is null ? null : new LexicalValue(element.Value);
    }

    /// <summary>
    /// Gets all values of a repeated direct child field, in document order.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name.</param>
    /// <returns>The values; an empty list when the field is absent.</returns>
    public IReadOnlyList<LexicalValue> GetValues(string fieldName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        return _element.Elements()
            .Where(element => string.Equals(element.Name.LocalName, fieldName, StringComparison.Ordinal))
            .Select(static element => new LexicalValue(element.Value))
            .ToArray();
    }

    /// <summary>
    /// Gets the raw text of the first direct child field.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name.</param>
    /// <returns>The raw text, or <see langword="null"/> when the field is absent.</returns>
    public string? GetRawValue(string fieldName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        return FindDirectChild(fieldName)?.Value;
    }

    /// <summary>
    /// Gets the record key as defined by metadata, or the first child value for document-driven tables.
    /// </summary>
    /// <returns>The key text, or <see langword="null"/> when no key can be determined.</returns>
    public string? GetKey()
    {
        var primary = _owner.Metadata?.RecordKey?.Primary;
        if (primary is { Kind: RecordKeyKind.Scalar, KeyField: not null })
        {
            return GetRawValue(primary.KeyField) ?? _element.Elements().FirstOrDefault()?.Value;
        }

        return _element.Elements().FirstOrDefault()?.Value;
    }

    /// <summary>
    /// Sets the text of a direct child field without changing its position or any other field.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name.</param>
    /// <param name="rawText">Raw replacement text, preserved exactly.</param>
    /// <param name="addIfMissing">
    /// When <see langword="true"/>, adds a new empty child element if the field is absent.
    /// </param>
    /// <returns><see langword="true"/> when the field exists or was added; otherwise <see langword="false"/>.</returns>
    public bool TrySetValue(string fieldName, string rawText, bool addIfMissing = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        ArgumentNullException.ThrowIfNull(rawText);

        var child = FindDirectChild(fieldName);
        if (child is null)
        {
            if (!addIfMissing)
            {
                return false;
            }

            child = new XElement(XName.Get(fieldName));
            _element.Add(child);
        }

        if (child.HasElements)
        {
            return false;
        }

        if (!string.Equals(child.Value, rawText, StringComparison.Ordinal))
        {
            child.Value = rawText;
            _owner.MarkDirty();
        }

        return true;
    }

    /// <summary>
    /// Sets the text at a nested field path, matching the first occurrence at every level.
    /// </summary>
    /// <param name="path">Slash-separated field path.</param>
    /// <param name="rawText">Raw replacement text, preserved exactly.</param>
    /// <param name="addIfMissing">
    /// When <see langword="true"/> and the path has one segment, adds a new child element if absent.
    /// </param>
    /// <returns><see langword="true"/> when the value was found or added; otherwise <see langword="false"/>.</returns>
    public bool TrySetValue(FieldPath path, string rawText, bool addIfMissing = false)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);

        if (path.Depth == 1)
        {
            return TrySetValue(path.FieldName, rawText, addIfMissing);
        }

        var element = FindPath(path);
        if (element is null || element.HasElements)
        {
            return false;
        }

        if (!string.Equals(element.Value, rawText, StringComparison.Ordinal))
        {
            element.Value = rawText;
            _owner.MarkDirty();
        }

        return true;
    }

    private XElement? FindDirectChild(string fieldName) =>
        _element.Elements().FirstOrDefault(element => string.Equals(element.Name.LocalName, fieldName, StringComparison.Ordinal));

    private XElement? FindPath(FieldPath path)
    {
        XElement? current = _element;
        foreach (var segment in path.Segments)
        {
            current = current?.Elements().FirstOrDefault(element => string.Equals(element.Name.LocalName, segment, StringComparison.Ordinal));
            if (current is null)
            {
                return null;
            }
        }

        return current;
    }
}
