using System.Globalization;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.IO;

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

        _records = document.Root is null
            ? new List<TableRecord>()
            : document.Root.Elements()
                .Where(element => string.Equals(element.Name.LocalName, recordElementName, StringComparison.Ordinal))
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
    public TableDocumentStatus Status { get; }

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

    private string? PrimaryKeyField => Metadata?.RecordKey?.Primary.KeyField;

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

        foreach (var element in _document.Root.Elements().Where(element =>
                     string.Equals(element.Name.LocalName, RecordElementName, StringComparison.Ordinal)))
        {
            _records.Add(new TableRecord(this, element));
        }
    }

    /// <summary>
    /// Appends a new record with the next integer primary key.
    /// </summary>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the new record.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult<TableRecord> AddRecord()
    {
        if (Status != TableDocumentStatus.Loaded || _document.Root is null)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.XmlNotSaveable,
                "Records can only be added to a successfully loaded document.");
        }

        var element = new XElement(XName.Get(RecordElementName));
        var keyField = PrimaryKeyField;
        string? key = null;

        if (!string.IsNullOrEmpty(keyField))
        {
            key = GetNextPrimaryKey().ToString(CultureInfo.InvariantCulture);
            element.Add(new XElement(XName.Get(keyField), key));
        }

        _document.Root.Add(element);
        var record = new TableRecord(this, element);
        _records.Add(record);
        MarkDirty();
        ChangeTracker.Record(new RecordAdded(key));

        return OperationResult<TableRecord>.Success(record);
    }

    /// <summary>
    /// Deep-copies a record, assigns it the next integer primary key, and inserts the copy after the source.
    /// </summary>
    /// <param name="key">Key of the record to duplicate.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the copy.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
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
    /// Removes a record from the document.
    /// </summary>
    /// <param name="key">Key of the record to delete.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult DeleteRecord(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (Status != TableDocumentStatus.Loaded)
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                "Records can only be deleted from a successfully loaded document.");
        }

        var index = _records.FindIndex(record => string.Equals(record.GetKey(), key, StringComparison.Ordinal));
        if (index < 0)
        {
            return OperationResult.Failure(
                ErrorCodes.KeyNotFound,
                $"Record '{key}' was not found.");
        }

        var record = _records[index];
        var snapshot = record.Element.ToString(SaveOptions.DisableFormatting);
        record.Element.Remove();
        _records.RemoveAt(index);
        MarkDirty();
        ChangeTracker.Record(new RecordDeleted(key, snapshot));

        return OperationResult.Success();
    }

    internal void MarkDirty() => IsDirty = true;

    internal void MarkClean() => IsDirty = false;

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
