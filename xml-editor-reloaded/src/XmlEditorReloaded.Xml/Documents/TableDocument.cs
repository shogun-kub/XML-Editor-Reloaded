using System.Globalization;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.IO;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Documents;

/// <summary>
/// An editable XML table document loaded from disk. The document keeps the original element tree,
/// including unknown fields, whitespace, comments, and sparse record shapes.
/// </summary>
public sealed class TableDocument
{
    private readonly XDocument _document;
    private readonly List<TableRecord> _records;

    internal TableDocument(
        TableDescriptor descriptor,
        TableMetadata? metadata,
        XDocument document,
        FileFormatInfo format,
        TableDocumentStatus status,
        IReadOnlyList<LoadDiagnostic> diagnostics,
        string recordElementName,
        long originalLength,
        DateTime originalLastWriteTimeUtc,
        string originalSha256,
        byte[] originalBytes)
    {
        Descriptor = descriptor;
        Metadata = metadata;
        _document = document;
        OriginalXml = new XDocument(document);
        OriginalBytes = originalBytes;
        Format = format;
        Status = status;
        Diagnostics = diagnostics;
        RecordElementName = recordElementName;
        OriginalLength = originalLength;
        OriginalLastWriteTimeUtc = originalLastWriteTimeUtc;
        OriginalSha256 = originalSha256;

        _records = ResolveRecordElements(document)
            .Select(element => new TableRecord(this, element))
            .ToList();
    }

    /// <summary>
    /// Gets the descriptor of the loaded table, including its path and overlays.
    /// </summary>
    public TableDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the metadata used to load the table, or <see langword="null"/> for document-driven tables.
    /// </summary>
    public TableMetadata? Metadata { get; }

    /// <summary>
    /// Gets the on-disk format of the source file.
    /// </summary>
    public FileFormatInfo Format { get; }

    /// <summary>
    /// Gets the lifecycle status of the document.
    /// </summary>
    public TableDocumentStatus Status { get; private set; }

    /// <summary>
    /// Gets diagnostics collected while loading the document.
    /// </summary>
    public IReadOnlyList<LoadDiagnostic> Diagnostics { get; }

    /// <summary>
    /// Gets the element name used for records, for example <c>ITEM</c>.
    /// </summary>
    public string RecordElementName { get; }

    /// <summary>
    /// Gets the records in document order. Records keep their original sparse element sets.
    /// </summary>
    public IReadOnlyList<TableRecord> Records => _records;

    /// <summary>
    /// Gets the journal of mutations applied to this document, for a future undo implementation.
    /// </summary>
    public DocumentChangeTracker ChangeTracker { get; } = new();

    /// <summary>
    /// Gets a value indicating whether the document has unsaved changes.
    /// </summary>
    public bool IsDirty { get; private set; }

    /// <summary>
    /// Gets the length of the source file when it was loaded or last saved.
    /// </summary>
    public long OriginalLength { get; private set; }

    /// <summary>
    /// Gets the last write time of the source file when it was loaded or last saved, in UTC.
    /// </summary>
    public DateTime OriginalLastWriteTimeUtc { get; private set; }

    /// <summary>
    /// Gets the SHA-256 hash of the source file when it was loaded or last saved, as an uppercase hexadecimal string.
    /// </summary>
    public string OriginalSha256 { get; private set; }

    internal XDocument OriginalXml { get; private set; }

    internal byte[] OriginalBytes { get; private set; }

    internal XDocument Xml => _document;

    /// <summary>
    /// Creates a read-only textual snapshot of the current in-memory document, including unsaved edits.
    /// The snapshot is for display only; saving still uses the lossless document patcher.
    /// </summary>
    /// <returns>The current XML text, or an empty string when the document has no root.</returns>
    public string CreateXmlSnapshot()
    {
        if (_document.Root is null)
        {
            return string.Empty;
        }

        var body = _document.ToString();
        return _document.Declaration is { } declaration
            ? $"{declaration}{Environment.NewLine}{body}"
            : body;
    }

    private string? PrimaryKeyField => Metadata?.RecordKeyFields is { Count: 1 } single
        ? single[0]
        : Metadata?.RecordKey?.Primary.KeyField;

    private bool HasCompoundRecordKey => Metadata?.HasCompoundRecordKey == true;

    /// <summary>
    /// Returns the record elements of a document. Tables such as <c>Map.Cities</c> keep their records
    /// inside a container (<c>CITYLIST/CITY</c>) instead of directly under the root, so the declared
    /// primary key path is followed when it includes more than the record name.
    /// </summary>
    private IReadOnlyList<XElement> ResolveRecordElements(XDocument document)
    {
        if (document.Root is null)
        {
            return Array.Empty<XElement>();
        }

        if (Metadata?.RecordKey?.Primary.Path is { Length: > 0 } path)
        {
            var resolved = new OverlayKeyResolver(Metadata).ResolvePrimary(document);
            if (resolved.Count > 0)
            {
                return resolved.Select(static record => record.Element).ToArray();
            }

            // An empty nested table still has a container in a valid document. Fall back to a direct
            // path walk so adding the first record can target the existing container.
            var current = new List<XElement> { document.Root };
            foreach (var part in path.Split('/'))
            {
                current = current
                    .SelectMany(element => element.Elements()
                        .Where(child => string.Equals(child.Name.LocalName, part, StringComparison.Ordinal)))
                    .ToList();
                if (current.Count == 0)
                {
                    break;
                }
            }

            return current;
        }

        return document.Root.Elements()
            .Where(element => string.Equals(element.Name.LocalName, RecordElementName, StringComparison.Ordinal))
            .ToArray();
    }

    /// <summary>
    /// Returns the element that receives new records. For a nested primary path this is the container
    /// (<c>CITYLIST</c>), which is created when the document does not contain it yet.
    /// </summary>
    private XElement? ResolveRecordContainer()
    {
        var root = _document.Root;
        if (root is null)
        {
            return null;
        }

        if (Metadata?.RecordKey?.Primary.Path is not { Length: > 0 } path)
        {
            return root;
        }

        var parts = path.Split('/');
        if (parts.Length <= 1)
        {
            return root;
        }

        var current = root;
        for (var index = 0; index < parts.Length - 1; index++)
        {
            var name = parts[index];
            var child = current.Elements()
                .LastOrDefault(element => string.Equals(element.Name.LocalName, name, StringComparison.Ordinal));
            if (child is null)
            {
                child = new XElement(XName.Get(name));
                current.Add(child);
            }

            current = child;
        }

        return current;
    }

    /// <summary>
    /// Finds a record by its primary key value.
    /// </summary>
    /// <param name="key">Key value as it appears in the XML, for example <c>42</c>.</param>
    /// <returns>The matching record, or <see langword="null"/> when no record has that key.</returns>
    public TableRecord? FindRecord(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return _records.FirstOrDefault(record => string.Equals(record.GetKey(), key, StringComparison.Ordinal));
    }

    /// <summary>
    /// Returns the next integer primary key based on the current records.
    /// </summary>
    /// <returns>
    /// The maximum existing integer key plus one, or the record count when the key field is unknown
    /// or contains no integer values.
    /// </returns>
    public int GetNextPrimaryKey()
    {
        var keyField = Metadata?.RecordKey?.Primary.KeyField;
        if (string.IsNullOrEmpty(keyField))
        {
            return _records.Count;
        }

        var maximum = -1;
        foreach (var record in _records)
        {
            if (record.GetValue(keyField)?.TryGetInt32(out var value) == true)
            {
                maximum = Math.Max(maximum, value);
            }
        }

        return maximum < 0 ? _records.Count : maximum + 1;
    }

    /// <summary>
    /// Rebuilds the record list after the document tree was changed outside the record operations,
    /// for example by an overlay writer that created a translation record.
    /// </summary>
    internal void SynchronizeRecords()
    {
        _records.Clear();
        if (_document.Root is null)
        {
            return;
        }

        foreach (var element in ResolveRecordElements(_document))
        {
            _records.Add(new TableRecord(this, element));
        }
    }

    /// <summary>
    /// Appends a new record with the next integer primary key. A table with a compound key appends an
    /// empty record instead, because no single key value identifies the row; the user fills the key
    /// fields in the grid or detail form.
    /// </summary>
    /// <param name="primaryKey">
    /// Optional exact key text for a table with a single declared key field, for example a lookup
    /// identifier above the 32-bit range. The value is written verbatim; the caller validates it. A
    /// compound-key table and a table without a key field ignore the value and use the usual fallback.
    /// </param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the new record.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult<TableRecord> AddRecord(string? primaryKey = null)
    {
        if (Status != TableDocumentStatus.Loaded || _document.Root is null)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.XmlNotSaveable,
                "Records can only be added to a successfully loaded document.");
        }

        var element = new XElement(XName.Get(RecordElementName));
        var keyField = HasCompoundRecordKey ? null : PrimaryKeyField;
        string? key = null;

        if (!string.IsNullOrEmpty(keyField))
        {
            key = string.IsNullOrEmpty(primaryKey)
                ? GetNextPrimaryKey().ToString(CultureInfo.InvariantCulture)
                : primaryKey;
            element.Add(new XElement(XName.Get(keyField), key));
        }

        // A lookup record is an id/name pair on disk: XmlFileLookupRepository rejects the whole file
        // when any record lacks a name element, so a new row starts with an empty name like legacy did.
        if (Metadata?.IsLookup == true
            && !element.Elements().Any(static child =>
                string.Equals(child.Name.LocalName, "name", StringComparison.Ordinal)))
        {
            element.Add(new XElement(XName.Get("name"), string.Empty));
        }

        var container = ResolveRecordContainer();
        if (container is null)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.XmlNotSaveable,
                "The document has no root element.");
        }

        container.Add(element);
        var record = new TableRecord(this, element);
        _records.Add(record);
        MarkDirty();
        ChangeTracker.Record(new RecordAdded(key));

        return OperationResult<TableRecord>.Success(record);
    }

    /// <summary>
    /// Deep-copies a record, assigns it the next integer primary key, and inserts the copy after the source.
    /// Tables with a compound key cannot assign a new identity automatically and return
    /// <c>key.duplicate_not_supported</c>.
    /// </summary>
    /// <param name="key">Key of the record to duplicate.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the copy.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>key.duplicate_not_supported</c>, or
    /// <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult<TableRecord> DuplicateRecord(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (Status != TableDocumentStatus.Loaded)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.XmlNotSaveable,
                "Records can only be duplicated in a successfully loaded document.");
        }

        if (HasCompoundRecordKey)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.KeyDuplicateNotSupported,
                $"Table '{Descriptor.TableName}' uses a compound record key and cannot duplicate a record automatically.");
        }

        var index = _records.FindIndex(record => string.Equals(record.GetKey(), key, StringComparison.Ordinal));
        if (index < 0)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.KeyNotFound,
                $"Record '{key}' was not found.");
        }

        var source = _records[index];
        var clone = new XElement(source.Element);
        var keyField = PrimaryKeyField;
        string? newKey = null;

        if (!string.IsNullOrEmpty(keyField))
        {
            newKey = GetNextPrimaryKey().ToString(CultureInfo.InvariantCulture);
            var keyElement = clone.Elements().FirstOrDefault(element =>
                string.Equals(element.Name.LocalName, keyField, StringComparison.Ordinal));

            if (keyElement is null)
            {
                clone.AddFirst(new XElement(XName.Get(keyField), newKey));
            }
            else
            {
                keyElement.Value = newKey;
            }
        }

        source.Element.AddAfterSelf(clone);
        var record = new TableRecord(this, clone);
        _records.Insert(index + 1, record);
        MarkDirty();
        ChangeTracker.Record(new RecordDuplicated(key, newKey));

        return OperationResult<TableRecord>.Success(record);
    }

    /// <summary>
    /// Deep-copies a record without changing its key fields and inserts the copy directly after the
    /// source. Use this to duplicate dependent rows of a compound-key table whose identity is completed
    /// by a caller-supplied parent link.
    /// </summary>
    /// <param name="source">Record to copy; it must belong to this document.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the copy.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    public OperationResult<TableRecord> DuplicateRecordPreservingKey(TableRecord source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (Status != TableDocumentStatus.Loaded)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.XmlNotSaveable,
                "Records can only be duplicated in a successfully loaded document.");
        }

        var index = _records.IndexOf(source);
        if (index < 0)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.KeyNotFound,
                "The source record does not belong to this document.");
        }

        var key = source.GetKey() ?? string.Empty;
        var clone = new XElement(source.Element);
        source.Element.AddAfterSelf(clone);
        var record = new TableRecord(this, clone);
        _records.Insert(index + 1, record);
        MarkDirty();
        ChangeTracker.Record(new RecordDuplicated(key, key));

        return OperationResult<TableRecord>.Success(record);
    }

    /// <summary>
    /// Removes a record identified by its key. When several records share the key, the first one in
    /// document order is removed; use <see cref="DeleteRecord(TableRecord)"/> to remove an exact row.
    /// </summary>
    /// <param name="key">Key of the record to delete.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult DeleteRecord(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        var index = _records.FindIndex(record => string.Equals(record.GetKey(), key, StringComparison.Ordinal));
        return index < 0
            ? OperationResult.Failure(ErrorCodes.KeyNotFound, $"Record '{key}' was not found.")
            : DeleteRecord(_records[index]);
    }

    /// <summary>
    /// Removes one exact record from the document. Unlike the key overload, this removes the selected
    /// row even when another record has the same key.
    /// </summary>
    /// <param name="record">Record to remove; it must belong to this document.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> is null.</exception>
    public OperationResult DeleteRecord(TableRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (Status != TableDocumentStatus.Loaded)
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                "Records can only be deleted from a successfully loaded document.");
        }

        var index = _records.IndexOf(record);
        if (index < 0)
        {
            return OperationResult.Failure(
                ErrorCodes.KeyNotFound,
                "The record does not belong to this document.");
        }

        var key = record.GetKey() ?? string.Empty;
        var snapshot = record.Element.ToString(SaveOptions.DisableFormatting);
        record.Element.Remove();
        _records.RemoveAt(index);
        MarkDirty();
        ChangeTracker.Record(new RecordDeleted(key, snapshot));

        return OperationResult.Success();
    }

    /// <summary>
    /// Allows a structurally valid table with no records to accept new records. The loader marks such
    /// a table read-only because a missing record element is usually a diagnostic view; a copy target
    /// may legitimately start empty, so the restriction is lifted only when that is the sole problem.
    /// </summary>
    /// <returns><see langword="true"/> when the document is editable.</returns>
    internal bool TryEnableEditing()
    {
        if (Status == TableDocumentStatus.Loaded)
        {
            return true;
        }

        if (Status != TableDocumentStatus.LoadedReadOnly
            || Diagnostics.Any(static diagnostic => !string.Equals(
                diagnostic.Code,
                ErrorCodes.XmlRecordElementMissing,
                StringComparison.Ordinal)))
        {
            return false;
        }

        Status = TableDocumentStatus.Loaded;
        return true;
    }

    /// <summary>
    /// Appends a record cloned from another document, assigning the supplied key when a key field is
    /// known. The clone keeps every element, attribute, and lexical value of the source record.
    /// </summary>
    /// <param name="sourceElement">Record element owned by another document.</param>
    /// <param name="keyField">XML field that stores the record key, or <see langword="null"/>.</param>
    /// <param name="keyValue">Key to write into the clone, or <see langword="null"/> to keep the source key.</param>
    /// <returns>The inserted record facade.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="sourceElement"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the document is not saveable.</exception>
    internal TableRecord InsertRecord(XElement sourceElement, string? keyField, string? keyValue)
    {
        ArgumentNullException.ThrowIfNull(sourceElement);

        if (Status != TableDocumentStatus.Loaded || _document.Root is null)
        {
            throw new InvalidOperationException("Records can only be inserted into a successfully loaded document.");
        }

        var clone = new XElement(sourceElement);
        if (!string.IsNullOrEmpty(keyField) && keyValue is not null)
        {
            var keyElement = clone.Elements().FirstOrDefault(element =>
                string.Equals(element.Name.LocalName, keyField, StringComparison.Ordinal));
            if (keyElement is null)
            {
                clone.AddFirst(new XElement(XName.Get(keyField), keyValue));
            }
            else
            {
                keyElement.Value = keyValue;
            }
        }

        _document.Root.Add(clone);
        var record = new TableRecord(this, clone);
        _records.Add(record);
        MarkDirty();
        ChangeTracker.Record(new RecordAdded(keyValue));
        return record;
    }

    internal void MarkDirty() => IsDirty = true;

    internal void MarkClean() => IsDirty = false;

    internal void RecomputeDirty() => IsDirty = !XNode.DeepEquals(_document, OriginalXml);

    internal void AcceptSaved(byte[] bytes, DateTime lastWriteTimeUtc, string sha256)
    {
        OriginalBytes = bytes;
        OriginalXml = new XDocument(_document);
        OriginalLength = bytes.LongLength;
        OriginalLastWriteTimeUtc = lastWriteTimeUtc;
        OriginalSha256 = sha256;
        IsDirty = false;
    }
}
