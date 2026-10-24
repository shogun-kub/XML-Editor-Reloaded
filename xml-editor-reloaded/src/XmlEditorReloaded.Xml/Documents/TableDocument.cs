using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
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
