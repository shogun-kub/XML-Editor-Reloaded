using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Diagnostics;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.IO;

namespace XmlEditorReloaded.Xml.Loading;

/// <summary>
/// Loads table XML files into <see cref="TableDocument"/> instances without normalizing their content.
/// </summary>
public sealed class TableFileLoader
{
    /// <summary>
    /// Loads a table file and returns a load result even when the XML is malformed.
    /// </summary>
    /// <param name="descriptor">Table descriptor discovered by the catalog.</param>
    /// <param name="metadata">Optional metadata; when absent the document is interpreted from its own structure.</param>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the load result.
    /// A malformed XML file is reported as a successful result with
    /// <see cref="TableDocumentStatus.Malformed"/> and diagnostics, not as an operation failure.
    /// When <paramref name="metadata"/> declares the root and that root contains no child elements, the
    /// table loads with <see cref="TableDocumentStatus.Loaded"/>, zero records, and no diagnostic, so its
    /// first record can be added; a non-nested table whose root has other child elements but no record
    /// element still loads with <see cref="TableDocumentStatus.LoadedReadOnly"/> and
    /// <c>xml.record_element_missing</c>.
    /// On I/O failure, <c>error_code</c> is <c>io.file_not_found</c>, <c>io.access_denied</c>, or <c>io.error</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptor"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public Task<OperationResult<TableLoadResult>> LoadAsync(
        TableDescriptor descriptor,
        TableMetadata? metadata = null,
        CancellationToken cancellationToken = default) =>
        LoadCoreAsync(descriptor, metadata, requireRecords: true, cancellationToken);

    /// <summary>
    /// Loads a language overlay file without requiring the table's direct record element to be present.
    /// </summary>
    /// <param name="descriptor">Descriptor whose file path points at the overlay file.</param>
    /// <param name="metadata">Metadata of the base table, used for root and key information.</param>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the overlay document
    /// with status <see cref="TableDocumentStatus.Loaded"/>.
    /// A malformed overlay is reported as a successful result with <see cref="TableDocumentStatus.Malformed"/>.
    /// On I/O failure, <c>error_code</c> is <c>io.file_not_found</c>, <c>io.access_denied</c>, or <c>io.error</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptor"/> or <paramref name="metadata"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public Task<OperationResult<TableLoadResult>> LoadOverlayAsync(
        TableDescriptor descriptor,
        TableMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        return LoadCoreAsync(descriptor, metadata, requireRecords: false, cancellationToken);
    }

    private async Task<OperationResult<TableLoadResult>> LoadCoreAsync(
        TableDescriptor descriptor,
        TableMetadata? metadata,
        bool requireRecords,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        var path = descriptor.FilePath;
        byte[] bytes;
        try
        {
            bytes = await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
        }
        catch (FileNotFoundException)
        {
            var message = $"The table file '{path}' does not exist.";
            OperationLog.Warning("TableFileLoader", message, ErrorCodes.IoFileNotFound);
            return OperationResult<TableLoadResult>.Failure(ErrorCodes.IoFileNotFound, message);
        }
        catch (DirectoryNotFoundException)
        {
            var message = $"The directory of table file '{path}' does not exist.";
            OperationLog.Warning("TableFileLoader", message, ErrorCodes.IoFileNotFound);
            return OperationResult<TableLoadResult>.Failure(ErrorCodes.IoFileNotFound, message);
        }
        catch (UnauthorizedAccessException exception)
        {
            var message = $"Access to table file '{path}' was denied.";
            OperationLog.Error("TableFileLoader", message, ErrorCodes.IoAccessDenied, exception);
            return OperationResult<TableLoadResult>.Failure(ErrorCodes.IoAccessDenied, message);
        }
        catch (IOException exception)
        {
            var message = $"The table file '{path}' could not be read: {exception.Message}";
            OperationLog.Error("TableFileLoader", message, ErrorCodes.IoError, exception);
            return OperationResult<TableLoadResult>.Failure(ErrorCodes.IoError, message);
        }

        var format = FileFormatInfo.Detect(bytes);
        var diagnostics = new List<LoadDiagnostic>();

        cancellationToken.ThrowIfCancellationRequested();

        XDocument document;
        try
        {
            using var stream = new MemoryStream(bytes, writable: false);
            using var reader = XmlReader.Create(stream, CreateReaderSettings());
            document = XDocument.Load(reader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
        }
        catch (XmlException exception)
        {
            diagnostics.Add(new LoadDiagnostic(
                ErrorCodes.XmlParseError,
                exception.Message,
                exception.LineNumber,
                exception.LinePosition));

            return OperationResult<TableLoadResult>.Success(
                new TableLoadResult(descriptor, TableDocumentStatus.Malformed, null, diagnostics));
        }

        if (document.Root is null)
        {
            diagnostics.Add(new LoadDiagnostic(ErrorCodes.XmlParseError, "The XML document has no root element."));
            return OperationResult<TableLoadResult>.Success(
                new TableLoadResult(descriptor, TableDocumentStatus.Malformed, null, diagnostics));
        }

        var root = document.Root;
        var recordElementName = metadata?.RecordElement ?? InferRecordElement(root);
        var status = TableDocumentStatus.Loaded;

        if (metadata is not null && !string.Equals(root.Name.LocalName, metadata.XmlRoot, StringComparison.Ordinal))
        {
            diagnostics.Add(CreateElementDiagnostic(
                root,
                ErrorCodes.XmlUnexpectedRoot,
                $"Expected root element '{metadata.XmlRoot}' but found '{root.Name.LocalName}'."));
            status = TableDocumentStatus.LoadedReadOnly;
        }

        var primaryKeyPath = metadata?.RecordKey?.Primary.Path;
        var recordsAreNested = !string.IsNullOrEmpty(primaryKeyPath);
        if (requireRecords && !recordsAreNested)
        {
            var childElements = root.Elements();
            var hasRecordElement = childElements.Any(element => string.Equals(
                element.Name.LocalName,
                recordElementName,
                StringComparison.Ordinal));

            // A declared root without any child element is a table whose last row was deleted, not a
            // malformed file: it stays editable so the user can add the first record again.
            var isEmptyDeclaredRoot = metadata is not null
                && !childElements.Any()
                && string.Equals(root.Name.LocalName, metadata.XmlRoot, StringComparison.Ordinal);

            if (!hasRecordElement && !isEmptyDeclaredRoot)
            {
                diagnostics.Add(new LoadDiagnostic(
                    ErrorCodes.XmlRecordElementMissing,
                    $"No record elements named '{recordElementName}' were found under the root."));
                status = TableDocumentStatus.LoadedReadOnly;
            }
        }

        AddLegacyRepeatedKeyDiagnostic(metadata, root, recordElementName, diagnostics);

        var lastWriteTimeUtc = File.GetLastWriteTimeUtc(path);
        var sha256 = Convert.ToHexString(SHA256.HashData(bytes));
        var tableDocument = new TableDocument(
            descriptor,
            metadata,
            document,
            format,
            status,
            diagnostics,
            recordElementName,
            bytes.LongLength,
            lastWriteTimeUtc,
            sha256,
            bytes);

        return OperationResult<TableLoadResult>.Success(
            new TableLoadResult(descriptor, status, tableDocument, diagnostics));
    }

    private static XmlReaderSettings CreateReaderSettings() => new()
    {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = null,
        IgnoreWhitespace = false,
        IgnoreComments = false,
        CloseInput = false,
        ConformanceLevel = ConformanceLevel.Document,
    };

    private static string InferRecordElement(XElement root) =>
        root.Elements().FirstOrDefault()?.Name.LocalName ?? root.Name.LocalName;

    /// <summary>
    /// Reports compound keys that use the legacy positional representation, for example a repeated
    /// <c>usResult</c> element instead of <c>usResult1..usResult10</c>. The document is not modified:
    /// the numbered key fields stay absent, so such records cannot be identified for editing, but the
    /// file is preserved byte for byte until the user changes a value.
    /// </summary>
    private static void AddLegacyRepeatedKeyDiagnostic(
        TableMetadata? metadata,
        XElement root,
        string recordElementName,
        List<LoadDiagnostic> diagnostics)
    {
        if (metadata?.RecordKeyFields is not { Count: > 1 } keyFields)
        {
            return;
        }

        var numberedPrefixes = keyFields
            .Select(static field =>
            {
                var digits = field.Length;
                while (digits > 0 && char.IsAsciiDigit(field[digits - 1]))
                {
                    digits--;
                }

                return digits > 0 && digits < field.Length ? field[..digits] : null;
            })
            .Where(static prefix => prefix is not null)
            .Select(static prefix => prefix!)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        foreach (var prefix in numberedPrefixes)
        {
            var offending = root
                .Elements()
                .Where(element => string.Equals(element.Name.LocalName, recordElementName, StringComparison.Ordinal))
                .FirstOrDefault(element =>
                    element.Elements().Count(child => string.Equals(
                        child.Name.LocalName,
                        prefix,
                        StringComparison.Ordinal)) > 1
                    && !element.Elements().Any(child => string.Equals(
                        child.Name.LocalName,
                        prefix + "1",
                        StringComparison.Ordinal)));
            if (offending is null)
            {
                continue;
            }

            diagnostics.Add(CreateElementDiagnostic(
                offending,
                ErrorCodes.XmlLegacyRepeatedKey,
                $"The record uses repeated '{prefix}' elements for a compound key instead of numbered "
                + $"'{prefix}1..{prefix}N' fields. The file is preserved unchanged, but the record cannot be "
                + "identified or edited until it is converted."));
            return;
        }
    }

    private static LoadDiagnostic CreateElementDiagnostic(XElement element, string code, string message)
    {
        if (element is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
        {
            return new LoadDiagnostic(code, message, lineInfo.LineNumber, lineInfo.LinePosition);
        }

        return new LoadDiagnostic(code, message);
    }
}
