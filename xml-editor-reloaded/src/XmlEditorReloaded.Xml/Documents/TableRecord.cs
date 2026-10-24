using System.Globalization;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Api;
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

    internal XElement Element => _element;

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
    /// Sets the text of a direct or nested field, validating numeric values against metadata.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name for a direct child.</param>
    /// <param name="rawText">Raw replacement text, preserved exactly.</param>
    /// <param name="addIfMissing">When <see langword="true"/>, appends a missing direct child.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetFieldValue(string fieldName, string rawText, bool addIfMissing = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        return SetFieldValue(new FieldPath(new[] { fieldName }), rawText, addIfMissing);
    }

    /// <summary>
    /// Sets the text at a field path, validating numeric values against metadata.
    /// </summary>
    /// <param name="path">Slash-separated field path.</param>
    /// <param name="rawText">Raw replacement text, preserved exactly.</param>
    /// <param name="addIfMissing">
    /// When <see langword="true"/> and the path has one segment, appends a missing field.
    /// </param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetFieldValue(FieldPath path, string rawText, bool addIfMissing = false)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);

        var validation = ValidateValue(path.FieldName, rawText);
        if (!validation.IsSuccess)
        {
            return validation;
        }

        if (path.Depth == 1)
        {
            var child = FindDirectChild(path.FieldName);
            if (child is null)
            {
                if (!addIfMissing)
                {
                    return OperationResult.Failure(
                        ErrorCodes.KeyNotFound,
                        $"Field '{path.FieldName}' is not present in the record.");
                }

                child = new XElement(XName.Get(path.FieldName));
                _element.Add(child);
                child.Value = rawText;
                _owner.MarkDirty();
                _owner.ChangeTracker.Record(new FieldValueChanged(OwnerKey, path.Text, null, rawText));
                return OperationResult.Success();
            }

            return SetElementValue(child, path, rawText);
        }

        var element = FindPath(path);
        if (element is null)
        {
            return OperationResult.Failure(
                ErrorCodes.KeyNotFound,
                $"Field path '{path.Text}' is not present in the record.");
        }

        return SetElementValue(element, path, rawText);
    }

    /// <summary>
    /// Replaces all values of a repeated direct child field.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name.</param>
    /// <param name="rawValues">New values in the desired order; an empty sequence removes the field occurrences.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>xml.invalid_number</c> or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetListField(string fieldName, IEnumerable<string> rawValues)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        ArgumentNullException.ThrowIfNull(rawValues);

        var values = rawValues.ToArray();
        foreach (var value in values)
        {
            ArgumentNullException.ThrowIfNull(value);
            var validation = ValidateValue(fieldName, value);
            if (!validation.IsSuccess)
            {
                return validation;
            }
        }

        var existing = _element.Elements()
            .Where(element => string.Equals(element.Name.LocalName, fieldName, StringComparison.Ordinal))
            .ToList();

        if (existing.Any(static element => element.HasElements))
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                $"List field '{fieldName}' contains nested elements and cannot be replaced as scalar values.");
        }

        var oldValues = existing.Select(static element => element.Value).ToArray();
        var changed = existing.Count != values.Length
            || !oldValues.AsSpan().SequenceEqual(values);

        if (!changed)
        {
            return OperationResult.Success();
        }

        var common = Math.Min(existing.Count, values.Length);
        for (var index = 0; index < common; index++)
        {
            if (!string.Equals(existing[index].Value, values[index], StringComparison.Ordinal))
            {
                existing[index].Value = values[index];
            }
        }

        if (values.Length > existing.Count)
        {
            for (var index = existing.Count; index < values.Length; index++)
            {
                var element = new XElement(XName.Get(fieldName), values[index]);
                if (existing.Count > 0)
                {
                    existing[^1].AddAfterSelf(element);
                }
                else
                {
                    _element.Add(element);
                }

                existing.Add(element);
            }
        }
        else if (values.Length < existing.Count)
        {
            for (var index = values.Length; index < existing.Count; index++)
            {
                existing[index].Remove();
            }
        }

        _owner.MarkDirty();
        _owner.ChangeTracker.Record(new ListFieldChanged(OwnerKey, fieldName, oldValues, values));
        return OperationResult.Success();
    }

    /// <summary>
    /// Sets one indexed field from a dictionary mask such as <c>ItemCapacityPerSize%Index%</c>.
    /// </summary>
    /// <param name="definition">Dictionary field mask definition.</param>
    /// <param name="index">Zero-based index substituted into the mask.</param>
    /// <param name="rawText">Raw replacement text, preserved exactly.</param>
    /// <param name="addIfMissing">When <see langword="true"/>, appends the field when it is absent.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetDictField(
        DictFieldDefinition definition,
        int index,
        string rawText,
        bool addIfMissing = false)
    {
        ArgumentNullException.ThrowIfNull(definition);
        var expandedName = definition.Expand(index);
        return SetFieldValue(expandedName, rawText, addIfMissing);
    }

    /// <summary>
    /// Sets one indexed field from a raw mask such as <c>ItemCapacityPerSize%Index%</c>.
    /// </summary>
    /// <param name="mask">Element-name mask containing <c>%Index%</c>.</param>
    /// <param name="index">Zero-based index substituted into the mask.</param>
    /// <param name="rawText">Raw replacement text, preserved exactly.</param>
    /// <param name="addIfMissing">When <see langword="true"/>, appends the field when it is absent.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetDictField(string mask, int index, string rawText, bool addIfMissing = false) =>
        SetDictField(new DictFieldDefinition(mask), index, rawText, addIfMissing);

    /// <summary>
    /// Sets a direct field without validation and reports only whether the field was found.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name.</param>
    /// <param name="rawText">Raw replacement text.</param>
    /// <param name="addIfMissing">When <see langword="true"/>, appends a missing direct child.</param>
    /// <returns><see langword="true"/> when the value was set; otherwise <see langword="false"/>.</returns>
    public bool TrySetValue(string fieldName, string rawText, bool addIfMissing = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        return SetFieldValue(fieldName, rawText, addIfMissing).IsSuccess;
    }

    /// <summary>
    /// Sets a field path without exposing validation details.
    /// </summary>
    /// <param name="path">Slash-separated field path.</param>
    /// <param name="rawText">Raw replacement text.</param>
    /// <param name="addIfMissing">When <see langword="true"/> and the path has one segment, appends a missing field.</param>
    /// <returns><see langword="true"/> when the value was set; otherwise <see langword="false"/>.</returns>
    public bool TrySetValue(FieldPath path, string rawText, bool addIfMissing = false) =>
        SetFieldValue(path, rawText, addIfMissing).IsSuccess;

    private string OwnerKey => GetKey() ?? string.Empty;

    private OperationResult SetElementValue(XElement element, FieldPath path, string rawText)
    {
        if (element.HasElements)
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                $"Field '{path.Text}' contains nested elements and cannot be set as scalar text.");
        }

        var oldValue = element.Value;
        if (string.Equals(oldValue, rawText, StringComparison.Ordinal))
        {
            return OperationResult.Success();
        }

        element.Value = rawText;
        _owner.MarkDirty();
        _owner.ChangeTracker.Record(new FieldValueChanged(OwnerKey, path.Text, oldValue, rawText));
        return OperationResult.Success();
    }

    private OperationResult ValidateValue(string fieldName, string rawText)
    {
        if (_owner.Metadata?.TryGetColumn(fieldName, out var column) != true || column is null)
        {
            return OperationResult.Success();
        }

        if (column.DataType == ColumnDataType.Nested)
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                $"Field '{fieldName}' is a nested structure and cannot be set as scalar text.");
        }

        if (column.DataType is ColumnDataType.String or ColumnDataType.Raw || string.IsNullOrEmpty(rawText))
        {
            return OperationResult.Success();
        }

        var valid = column.DataType switch
        {
            ColumnDataType.Integer or ColumnDataType.Lookup or ColumnDataType.Flags =>
                int.TryParse(rawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Decimal =>
                decimal.TryParse(rawText, NumberStyles.Number, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Boolean => rawText is "0" or "1",
            _ => true,
        };

        return valid
            ? OperationResult.Success()
            : OperationResult.Failure(
                ErrorCodes.XmlInvalidNumber,
                $"'{rawText}' is not a valid {column.DataType} value for field '{fieldName}'.");
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
