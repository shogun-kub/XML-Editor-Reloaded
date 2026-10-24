using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
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
            return OperationResult<TableLoadResult>.Failure(
                ErrorCodes.IoFileNotFound,
                $"The table file '{path}' does not exist.");
        }
        catch (DirectoryNotFoundException)
        {
            return OperationResult<TableLoadResult>.Failure(
                ErrorCodes.IoFileNotFound,
                $"The directory of table file '{path}' does not exist.");
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult<TableLoadResult>.Failure(
                ErrorCodes.IoAccessDenied,
                $"Access to table file '{path}' was denied.");
        }
        catch (IOException exception)
        {
            return OperationResult<TableLoadResult>.Failure(
                ErrorCodes.IoError,
                $"The table file '{path}' could not be read: {exception.Message}");
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
        if (requireRecords
            && !recordsAreNested
            && !root.Elements().Any(element => string.Equals(element.Name.LocalName, recordElementName, StringComparison.Ordinal)))
        {
            diagnostics.Add(new LoadDiagnostic(
                ErrorCodes.XmlRecordElementMissing,
                $"No record elements named '{recordElementName}' were found under the root."));
            status = TableDocumentStatus.LoadedReadOnly;
        }

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

    private static LoadDiagnostic CreateElementDiagnostic(XElement element, string code, string message)
    {
        if (element is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
        {
            return new LoadDiagnostic(code, message, lineInfo.LineNumber, lineInfo.LinePosition);
        }

        return new LoadDiagnostic(code, message);
    }
}
