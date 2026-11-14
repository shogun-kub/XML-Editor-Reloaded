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
    /// <summary>
    /// Numeric styles accepted for decimal table values: an invariant decimal point, a sign, and
    /// surrounding whitespace, but no culture-specific group or decimal separators.
    /// </summary>
    private const NumberStyles DecimalStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

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
    /// Gets the distinct direct child element names of a nested field, in document order.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name of the nested container.</param>
    /// <returns>
    /// The child names, or an empty list when the field is absent or has no child elements.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldName"/> is empty.</exception>
    public IReadOnlyList<string> GetChildFieldNames(string fieldName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        var container = FindDirectChild(fieldName);
        return ContainerChildNames(container);
    }

    /// <summary>
    /// Gets the distinct child element names at a nested field path.
    /// </summary>
    /// <param name="path">Slash-separated path of the container.</param>
    /// <returns>The child names, or an empty list when the path is absent or has no child elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is null.</exception>
    public IReadOnlyList<string> GetChildFieldNames(FieldPath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return ContainerChildNames(FindPath(path));
    }

    private static string[] ContainerChildNames(XElement? container) =>
        container is null || !container.HasElements
            ? Array.Empty<string>()
            : container.Elements()
                .Select(static element => element.Name.LocalName)
                .Distinct(StringComparer.Ordinal)
                .ToArray();

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
        if (!_element.HasElements
            && string.Equals(fieldName, RecordFields.RecordText, StringComparison.Ordinal))
        {
            // Text-only records expose their text through the virtual field, matching the
            // FieldPath overload and the document-driven schema column.
            return _element.Value;
        }

        return FindDirectChild(fieldName)?.Value;
    }

    /// <summary>
    /// Gets the raw text at a nested field path, matching the first occurrence at every level.
    /// </summary>
    /// <param name="path">Field path relative to the record.</param>
    /// <returns>The raw text, or <see langword="null"/> when the path is absent.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is null.</exception>
    public string? GetRawValue(FieldPath path) => GetRawValue(path, occurrenceIndex: 0);

    /// <summary>
    /// Gets the raw text at a nested field path, selecting an occurrence of the first path segment.
    /// </summary>
    /// <param name="path">Field path relative to the record.</param>
    /// <param name="occurrenceIndex">
    /// Zero-based occurrence of the first path segment among same-named siblings. The remaining
    /// segments match their first occurrence. For example, <c>GEARKIT/mPriceMod</c> with index 2 reads
    /// the price modifier of the third gear kit.
    /// </param>
    /// <returns>
    /// The raw text, or <see langword="null"/> when the path is absent or the occurrence does not exist.
    /// For records without child elements, the virtual <see cref="RecordFields.RecordText"/> field
    /// returns the record text.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public string? GetRawValue(FieldPath path, int occurrenceIndex)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        if (occurrenceIndex == 0
            && path.Depth == 1
            && !_element.HasElements
            && string.Equals(path.FieldName, RecordFields.RecordText, StringComparison.Ordinal))
        {
            return _element.Value;
        }

        return FindPath(path, occurrenceIndex)?.Value;
    }

    /// <summary>
    /// Gets the component values of the record key, in key-field order.
    /// </summary>
    /// <returns>
    /// For compound keys, one entry per declared key field; a missing field is <see langword="null"/>.
    /// For single scalar keys, one entry with the key value; for positional keys, the zero-based ordinal.
    /// Document-driven tables expose the first child value.
    /// </returns>
    public IReadOnlyList<string?> GetKeyComponents()
    {
        var keyFields = _owner.Metadata?.RecordKeyFields;
        if (keyFields is { Count: > 0 })
        {
            return keyFields.Select(GetRawValue).ToArray();
        }

        var primary = _owner.Metadata?.RecordKey?.Primary;
        if (primary is { Kind: RecordKeyKind.Scalar, KeyField: not null })
        {
            return new[] { GetRawValue(primary.KeyField) };
        }

        if (primary is { Kind: RecordKeyKind.Positional })
        {
            return new[] { GetPositionalIndex() };
        }

        return new[] { _element.Elements().FirstOrDefault()?.Value };
    }

    /// <summary>
    /// Gets the record key as defined by metadata, or the first child value for document-driven tables.
    /// </summary>
    /// <returns>
    /// The key text, or <see langword="null"/> when no key can be determined. Compound keys use the
    /// canonical encoding of <see cref="RecordKeyCodec"/>; positional keys use the zero-based ordinal of
    /// the record element among same-named siblings.
    /// </returns>
    public string? GetKey()
    {
        if (_owner.Metadata?.HasCompoundRecordKey == true)
        {
            return RecordKeyCodec.Compose(GetKeyComponents());
        }

        if (_owner.Metadata?.RecordKeyFields is { Count: 1 } single)
        {
            return GetRawValue(single[0]) ?? _element.Elements().FirstOrDefault()?.Value;
        }

        var primary = _owner.Metadata?.RecordKey?.Primary;
        if (primary is { Kind: RecordKeyKind.Scalar, KeyField: not null })
        {
            return GetRawValue(primary.KeyField) ?? _element.Elements().FirstOrDefault()?.Value;
        }

        if (primary is { Kind: RecordKeyKind.Positional })
        {
            return GetPositionalIndex();
        }

        return _element.Elements().FirstOrDefault()?.Value;
    }

    private string GetPositionalIndex() =>
        _element.ElementsBeforeSelf()
            .Count(sibling => sibling.Name == _element.Name)
            .ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Gets the key formatted for user-facing messages. Compound keys show their components separated by
    /// <c> | </c>; other tables keep the plain key text.
    /// </summary>
    public string KeyDisplay => _owner.Metadata?.HasCompoundRecordKey == true
        ? RecordKeyCodec.Display(GetKeyComponents())
        : GetKey() ?? string.Empty;

    /// <summary>
    /// Gets a value indicating whether every component of the record key is present and non-empty. A
    /// numeric zero is a valid component; an absent or empty field is not. A newly added compound record
    /// is therefore incomplete until the user fills every key field.
    /// </summary>
    public bool IsKeyComplete => GetKeyComponents().All(static component => !string.IsNullOrEmpty(component));

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
    /// When <see langword="true"/>, a missing field is appended to the record. For a nested path the
    /// missing intermediate elements are created as well, so an edit of a declared nested field works
    /// for sparse records that omit the container.
    /// </param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetFieldValue(FieldPath path, string rawText, bool addIfMissing = false) =>
        SetFieldValue(path, occurrenceIndex: 0, rawText, addIfMissing);

    /// <summary>
    /// Sets the text at a field path, selecting an occurrence of the first path segment.
    /// </summary>
    /// <param name="path">Slash-separated field path.</param>
    /// <param name="occurrenceIndex">
    /// Zero-based occurrence of the first path segment among same-named siblings, for example the
    /// second <c>GEARKIT</c> of a mercenary record. Remaining segments match their first occurrence.
    /// </param>
    /// <param name="rawText">Raw replacement text, preserved exactly.</param>
    /// <param name="addIfMissing">
    /// When <see langword="true"/>, a missing leaf is appended. A missing occurrence of the first
    /// segment cannot be created and produces <c>key.not_found</c>.
    /// </param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> or <paramref name="rawText"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public OperationResult SetFieldValue(
        FieldPath path,
        int occurrenceIndex,
        string rawText,
        bool addIfMissing = false)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        var validation = ValidateValue(path.FieldName, rawText);
        if (!validation.IsSuccess)
        {
            return validation;
        }

        if (occurrenceIndex == 0
            && path.Depth == 1
            && !_element.HasElements
            && string.Equals(path.FieldName, RecordFields.RecordText, StringComparison.Ordinal))
        {
            // The virtual text field of a text-only record, for example one <SOUND>path.wav</SOUND>,
            // writes the record text itself. A real child element with this name cannot exist because
            // the record has no child elements.
            var oldText = _element.Value;
            if (string.Equals(oldText, rawText, StringComparison.Ordinal))
            {
                return OperationResult.Success();
            }

            _element.Value = rawText;
            _owner.MarkDirty();
            _owner.ChangeTracker.Record(
                new FieldValueChanged(OwnerKey, RecordFields.RecordText, oldText, rawText));
            return OperationResult.Success();
        }

        if (path.Depth == 1)
        {
            var child = occurrenceIndex == 0
                ? FindDirectChild(path.FieldName)
                : _element.Elements()
                    .Where(element => string.Equals(element.Name.LocalName, path.FieldName, StringComparison.Ordinal))
                    .Skip(occurrenceIndex)
                    .FirstOrDefault();
            if (child is null)
            {
                if (!addIfMissing || occurrenceIndex > 0)
                {
                    return OperationResult.Failure(
                        ErrorCodes.KeyNotFound,
                        occurrenceIndex == 0
                            ? $"Field '{path.FieldName}' is not present in the record."
                            : $"Occurrence {occurrenceIndex} of field '{path.FieldName}' is not present in the record.");
                }

                child = new XElement(XName.Get(path.FieldName));
                _element.Add(child);
                child.Value = rawText;
                _owner.MarkDirty();
                _owner.ChangeTracker.Record(new FieldValueChanged(OwnerKey, path.Text, null, rawText));
                return OperationResult.Success();
            }

            return SetElementValue(child, path, rawText, occurrenceIndex);
        }

        var element = FindPath(path, occurrenceIndex);
        if (element is null)
        {
            if (!addIfMissing)
            {
                return OperationResult.Failure(
                    ErrorCodes.KeyNotFound,
                    $"Field path '{path.Text}' is not present in the record.");
            }

            element = CreatePath(path, occurrenceIndex);
            if (element is null)
            {
                return OperationResult.Failure(
                    ErrorCodes.KeyNotFound,
                    $"Occurrence {occurrenceIndex} of '{path.Segments[0]}' is not present in the record.");
            }

            element.Value = rawText;
            _owner.MarkDirty();
            _owner.ChangeTracker.Record(
                new FieldValueChanged(OwnerKey, FormatChangePath(path, occurrenceIndex, element), null, rawText));
            return OperationResult.Success();
        }

        return SetElementValue(element, path, rawText, occurrenceIndex);
    }

    /// <summary>
    /// Sets a direct child field without metadata type validation. Lookup identifiers use this path
    /// because they are unsigned 64-bit values even when the declared column type is a 32-bit integer.
    /// </summary>
    internal OperationResult SetFieldValueUnchecked(string fieldName, string rawText, bool addIfMissing = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        ArgumentNullException.ThrowIfNull(rawText);

        var path = new FieldPath(new[] { fieldName });
        var child = FindDirectChild(fieldName);
        if (child is null)
        {
            if (!addIfMissing)
            {
                return OperationResult.Failure(
                    ErrorCodes.KeyNotFound,
                    $"Field '{fieldName}' is not present in the record.");
            }

            child = new XElement(XName.Get(fieldName));
            _element.Add(child);
            child.Value = rawText;
            _owner.MarkDirty();
            _owner.ChangeTracker.Record(new FieldValueChanged(OwnerKey, fieldName, null, rawText));
            return OperationResult.Success();
        }

        return SetElementValue(child, path, rawText, occurrenceIndex: 0);
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
    /// Gets the number of direct children with the given name.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name.</param>
    /// <returns>The occurrence count; zero when the element is absent.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldName"/> is empty.</exception>
    public int GetOccurrenceCount(string fieldName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        return _element.Elements()
            .Count(element => string.Equals(element.Name.LocalName, fieldName, StringComparison.Ordinal));
    }

    /// <summary>
    /// Gets the distinct child element names at a nested field path, selecting an occurrence of the
    /// first path segment.
    /// </summary>
    /// <param name="path">Slash-separated path of the container.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the first path segment among same-named siblings.</param>
    /// <returns>The child names, or an empty list when the path is absent or has no child elements.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public IReadOnlyList<string> GetChildFieldNames(FieldPath path, int occurrenceIndex)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);
        return ContainerChildNames(FindPath(path, occurrenceIndex));
    }

    /// <summary>
    /// Gets the values of a repeated leaf field at a nested path, selecting an occurrence of the first
    /// path segment. For example, <c>DRUNK/drugitems/drugitem</c> returns the drug item ids of the first
    /// <c>DRUNK</c> block.
    /// </summary>
    /// <param name="itemPath">Path to the repeated leaf, including its container segments.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the first path segment among same-named siblings.</param>
    /// <returns>The values in document order; empty when the path is absent.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="itemPath"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public IReadOnlyList<string> GetRepeatedValues(FieldPath itemPath, int occurrenceIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(itemPath);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        var container = itemPath.Depth == 1
            ? _element
            : FindPath(ContainerPath(itemPath), occurrenceIndex);
        return container is null
            ? Array.Empty<string>()
            : container.Elements()
                .Where(element => string.Equals(
                    element.Name.LocalName,
                    itemPath.FieldName,
                    StringComparison.Ordinal))
                .Select(static element => element.Value)
                .ToArray();
    }

    /// <summary>
    /// Replaces all values of a repeated leaf field at a nested path, selecting an occurrence of the
    /// first path segment. Only same-named siblings inside the resolved container are replaced; other
    /// elements of the container and of the record are preserved.
    /// </summary>
    /// <param name="itemPath">Path to the repeated leaf, including its container segments.</param>
    /// <param name="values">New values in the desired order; an empty sequence removes the occurrences.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the first path segment among same-named siblings.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="itemPath"/> or <paramref name="values"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public OperationResult SetRepeatedValues(
        FieldPath itemPath,
        IReadOnlyList<string> values,
        int occurrenceIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(itemPath);
        ArgumentNullException.ThrowIfNull(values);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        foreach (var value in values)
        {
            ArgumentNullException.ThrowIfNull(value);
            var validation = ValidateValue(itemPath.FieldName, value);
            if (!validation.IsSuccess)
            {
                return validation;
            }
        }

        XElement? container;
        if (itemPath.Depth == 1)
        {
            container = _element;
        }
        else
        {
            var containerPath = ContainerPath(itemPath);
            container = FindPath(containerPath, occurrenceIndex);
            if (container is null)
            {
                if (values.Count == 0)
                {
                    return OperationResult.Success();
                }

                container = CreatePath(containerPath, occurrenceIndex);
                if (container is null)
                {
                    return OperationResult.Failure(
                        ErrorCodes.KeyNotFound,
                        $"Occurrence {occurrenceIndex} of '{containerPath.Segments[0]}' is not present in the record.");
                }
            }
        }

        var existing = container.Elements()
            .Where(element => string.Equals(
                element.Name.LocalName,
                itemPath.FieldName,
                StringComparison.Ordinal))
            .ToList();
        if (existing.Any(static element => element.HasElements))
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                $"Repeated field '{itemPath.Text}' contains nested elements and cannot be replaced as scalar values.");
        }

        var oldValues = existing.Select(static element => element.Value).ToArray();
        var changed = existing.Count != values.Count
            || !oldValues.SequenceEqual(values, StringComparer.Ordinal);

        if (!changed)
        {
            return OperationResult.Success();
        }

        var common = Math.Min(existing.Count, values.Count);
        for (var index = 0; index < common; index++)
        {
            if (!string.Equals(existing[index].Value, values[index], StringComparison.Ordinal))
            {
                existing[index].Value = values[index];
            }
        }

        if (values.Count > existing.Count)
        {
            for (var index = existing.Count; index < values.Count; index++)
            {
                var element = new XElement(XName.Get(itemPath.FieldName), values[index]);
                if (existing.Count > 0)
                {
                    existing[^1].AddAfterSelf(element);
                }
                else
                {
                    container.Add(element);
                }

                existing.Add(element);
            }
        }
        else if (values.Count < existing.Count)
        {
            for (var index = values.Count; index < existing.Count; index++)
            {
                existing[index].Remove();
            }
        }

        _owner.MarkDirty();
        _owner.ChangeTracker.Record(new ListFieldChanged(OwnerKey, itemPath.Text, oldValues, values));
        return OperationResult.Success();
    }

    /// <summary>
    /// Appends one direct child element, optionally with initial leaf values, and inserts it after the
    /// last same-named sibling. Existing elements and their order are preserved.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name of the new child.</param>
    /// <param name="initialValues">
    /// Optional leaf values of the new element in document order. Values are validated against metadata
    /// before the element is attached.
    /// </param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>xml.invalid_number</c> or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldName"/> is empty.</exception>
    public OperationResult AddElement(
        string fieldName,
        IReadOnlyList<KeyValuePair<string, string>>? initialValues = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);

        if (initialValues is not null)
        {
            foreach (var pair in initialValues)
            {
                if (string.IsNullOrEmpty(pair.Key))
                {
                    continue;
                }

                var validation = ValidateValue(pair.Key, pair.Value);
                if (!validation.IsSuccess)
                {
                    return validation;
                }
            }
        }

        var element = new XElement(XName.Get(fieldName));
        if (initialValues is not null)
        {
            foreach (var pair in initialValues)
            {
                if (!string.IsNullOrEmpty(pair.Key))
                {
                    element.Add(new XElement(XName.Get(pair.Key), pair.Value));
                }
            }
        }

        var last = _element.Elements()
            .LastOrDefault(candidate => string.Equals(
                candidate.Name.LocalName,
                fieldName,
                StringComparison.Ordinal));
        if (last is not null)
        {
            last.AddAfterSelf(element);
        }
        else
        {
            _element.Add(element);
        }

        _owner.MarkDirty();
        return OperationResult.Success();
    }

    /// <summary>
    /// Removes one occurrence of a direct child element.
    /// </summary>
    /// <param name="fieldName">Case-sensitive XML element name.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence among same-named siblings.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldName"/> is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public OperationResult RemoveElement(string fieldName, int occurrenceIndex = 0)
    {
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        var element = _element.Elements()
            .Where(candidate => string.Equals(
                candidate.Name.LocalName,
                fieldName,
                StringComparison.Ordinal))
            .Skip(occurrenceIndex)
            .FirstOrDefault();
        if (element is null)
        {
            return OperationResult.Failure(
                ErrorCodes.KeyNotFound,
                $"Occurrence {occurrenceIndex} of element '{fieldName}' is not present in the record.");
        }

        element.Remove();
        _owner.MarkDirty();
        return OperationResult.Success();
    }

    private static FieldPath ContainerPath(FieldPath itemPath) =>
        new(itemPath.Segments.Take(itemPath.Depth - 1).ToArray());

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

    private OperationResult SetElementValue(
        XElement element,
        FieldPath path,
        string rawText,
        int occurrenceIndex = 0)
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
        _owner.ChangeTracker.Record(
            new FieldValueChanged(
                OwnerKey,
                FormatChangePath(path, occurrenceIndex, element),
                oldValue,
                rawText));
        return OperationResult.Success();
    }

    /// <summary>
    /// Formats the field path of a change so edits of different repeated occurrences stay distinct in
    /// the change journal, for example <c>GEARKIT[2]/mPriceMod</c>.
    /// </summary>
    private static string FormatChangePath(FieldPath path, int occurrenceIndex, XElement? element)
    {
        if (occurrenceIndex <= 0 || element is null)
        {
            return path.Text;
        }

        // The occurrence of the first segment is the ancestor at the matching depth, not necessarily
        // the edited leaf itself.
        var occurrence = element;
        for (var level = 1; level < path.Depth && occurrence.Parent is not null; level++)
        {
            occurrence = occurrence.Parent;
        }

        if (occurrence.Parent is null)
        {
            return path.Text;
        }

        var actualIndex = occurrence.Parent
            .Elements()
            .Where(sibling => string.Equals(
                sibling.Name.LocalName,
                occurrence.Name.LocalName,
                StringComparison.Ordinal))
            .TakeWhile(sibling => !ReferenceEquals(sibling, occurrence))
            .Count();
        return $"{path.Segments[0]}[{actualIndex}]" + path.Text[path.Segments[0].Length..];
    }

    private OperationResult ValidateValue(string fieldName, string rawText)
    {
        var metadata = _owner.Metadata;
        ColumnMetadata? column = null;
        ColumnHint? hint = null;
        metadata?.TryGetColumn(fieldName, out column);
        metadata?.TryGetColumnHint(fieldName, out hint);
        var dataType = column?.DataType ?? hint?.DataType;
        if (dataType is null)
        {
            return OperationResult.Success();
        }

        if (dataType == ColumnDataType.Nested)
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                $"Field '{fieldName}' is a nested structure and cannot be set as scalar text.");
        }

        if (dataType is ColumnDataType.String or ColumnDataType.Raw || string.IsNullOrEmpty(rawText))
        {
            return OperationResult.Success();
        }

        var valid = dataType switch
        {
            ColumnDataType.Integer =>
                int.TryParse(rawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Lookup =>
                long.TryParse(rawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)
                || ulong.TryParse(rawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Flags =>
                ulong.TryParse(rawText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Decimal =>
                decimal.TryParse(rawText, DecimalStyles, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Boolean => rawText.Trim() is "0" or "1",
            _ => true,
        };

        return valid
            ? OperationResult.Success()
            : OperationResult.Failure(
                ErrorCodes.XmlInvalidNumber,
                $"'{rawText}' is not a valid {dataType} value for field '{fieldName}'.");
    }

    private XElement? FindDirectChild(string fieldName) =>
        _element.Elements().FirstOrDefault(element => string.Equals(element.Name.LocalName, fieldName, StringComparison.Ordinal));

    private XElement? FindPath(FieldPath path) => FindPath(path, occurrenceIndex: 0);

    private XElement? FindPath(FieldPath path, int occurrenceIndex)
    {
        XElement? current = _element;
        for (var index = 0; index < path.Segments.Count; index++)
        {
            var segment = path.Segments[index];
            current = index == 0
                ? current?.Elements()
                    .Where(element => string.Equals(element.Name.LocalName, segment, StringComparison.Ordinal))
                    .Skip(occurrenceIndex)
                    .FirstOrDefault()
                : current?.Elements()
                    .FirstOrDefault(element => string.Equals(element.Name.LocalName, segment, StringComparison.Ordinal));
            if (current is null)
            {
                return null;
            }
        }

        return current;
    }

    /// <summary>
    /// Creates a missing field path, reusing existing elements and appending new ones in path order.
    /// The first segment is resolved to the requested occurrence; a missing occurrence is not created.
    /// </summary>
    /// <param name="path">Missing path to create.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the first path segment.</param>
    /// <returns>The leaf element of the created path, or <see langword="null"/> when the occurrence is absent.</returns>
    private XElement? CreatePath(FieldPath path, int occurrenceIndex = 0)
    {
        var current = occurrenceIndex == 0
            ? _element
            : _element.Elements()
                .Where(element => string.Equals(element.Name.LocalName, path.Segments[0], StringComparison.Ordinal))
                .Skip(occurrenceIndex)
                .FirstOrDefault();
        if (current is null)
        {
            return null;
        }

        var start = occurrenceIndex == 0 ? 0 : 1;
        for (var index = start; index < path.Segments.Count; index++)
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

        return current;
    }
}
