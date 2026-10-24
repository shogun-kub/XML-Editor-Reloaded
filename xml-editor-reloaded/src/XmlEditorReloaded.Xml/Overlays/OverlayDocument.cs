using System.Text;
using System.Xml;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml.Overlays;

/// <summary>
/// A language overlay file paired with the metadata of its base table.
/// </summary>
public sealed class OverlayDocument
{
    private readonly TableDocument _document;

    internal OverlayDocument(TableDocument document, OverlayDescriptor descriptor)
    {
        _document = document;
        Descriptor = descriptor;
    }

    /// <summary>
    /// Gets the overlay descriptor (language, base file, overlay file).
    /// </summary>
    public OverlayDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the underlying loaded overlay document.
    /// </summary>
    public TableDocument Document => _document;

    /// <summary>
    /// Gets the base table metadata used for key resolution and translatable paths.
    /// </summary>
    public TableMetadata Metadata =>
        _document.Metadata ?? throw new InvalidOperationException("Overlay documents require base table metadata.");

    /// <summary>
    /// Gets a value indicating whether the overlay has unsaved changes.
    /// </summary>
    public bool IsDirty => _document.IsDirty;

    internal XDocument Xml => _document.Xml;

    internal void MarkDirty() => _document.MarkDirty();

    /// <summary>
    /// Loads an overlay file with the metadata of its base table.
    /// </summary>
    /// <param name="descriptor">Overlay descriptor containing both file paths.</param>
    /// <param name="metadata">Metadata of the base table.</param>
    /// <param name="loader">Optional table loader.</param>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the overlay.
    /// On failure, <c>error_code</c> is <c>io.file_not_found</c>, <c>xml.parse_error</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptor"/> or <paramref name="metadata"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public static async Task<OperationResult<OverlayDocument>> LoadAsync(
        OverlayDescriptor descriptor,
        TableMetadata metadata,
        TableFileLoader? loader = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(metadata);

        var tableDescriptor = new TableDescriptor(
            metadata.TableName,
            descriptor.OverlayFilePath,
            Path.GetFileName(descriptor.OverlayFilePath),
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await (loader ?? new TableFileLoader())
            .LoadOverlayAsync(tableDescriptor, metadata, cancellationToken)
            .ConfigureAwait(false);

        if (!load.IsSuccess)
        {
            return OperationResult<OverlayDocument>.Failure(load);
        }

        if (load.Data!.Status != TableDocumentStatus.Loaded)
        {
            return OperationResult<OverlayDocument>.Failure(
                ErrorCodes.XmlNotSaveable,
                $"Overlay file '{descriptor.OverlayFilePath}' is not editable.");
        }

        var document = load.Data.GetDocument<TableDocument>();
        return document is null
            ? OperationResult<OverlayDocument>.Failure(
                ErrorCodes.XmlParseError,
                $"Overlay file '{descriptor.OverlayFilePath}' could not be loaded.")
            : OperationResult<OverlayDocument>.Success(new OverlayDocument(document, descriptor));
    }

    /// <summary>
    /// Creates an empty language overlay next to its base table. The file contains only the root
    /// element declared by metadata; records and translated fields are added lazily when the user
    /// edits a translation. The base table is never read or copied.
    /// </summary>
    /// <param name="descriptor">Overlay descriptor with the target language and file paths.</param>
    /// <param name="metadata">Metadata of the base table, used for the root and record element names.</param>
    /// <param name="loader">Optional loader used to read the new file back.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the loaded overlay.
    /// On failure, <c>error_code</c> is <c>overlay.exists</c>, <c>io.file_not_found</c>,
    /// <c>io.access_denied</c>, <c>io.error</c>, <c>xml.parse_error</c>, or <c>xml.unexpected_root</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptor"/> or <paramref name="metadata"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled; no file is left behind.</exception>
    public static async Task<OperationResult<OverlayDocument>> CreateAsync(
        OverlayDescriptor descriptor,
        TableMetadata metadata,
        TableFileLoader? loader = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(metadata);
        cancellationToken.ThrowIfCancellationRequested();

        var path = descriptor.OverlayFilePath;
        if (File.Exists(path))
        {
            return OperationResult<OverlayDocument>.Failure(
                ErrorCodes.OverlayExists,
                $"Overlay file '{path}' already exists and will not be overwritten.");
        }

        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            return OperationResult<OverlayDocument>.Failure(
                ErrorCodes.IoFileNotFound,
                $"The directory of overlay file '{path}' does not exist.");
        }

        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(metadata.XmlRoot));
        byte[] bytes;
        try
        {
            using var stream = new MemoryStream();
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\n",
                NewLineHandling = NewLineHandling.Replace,
                OmitXmlDeclaration = false,
            };
            using (var writer = XmlWriter.Create(stream, settings))
            {
                document.Save(writer);
            }

            bytes = stream.ToArray();
        }
        catch (Exception exception) when (exception is InvalidOperationException or XmlException)
        {
            return OperationResult<OverlayDocument>.Failure(
                ErrorCodes.XmlSerializationError,
                $"The overlay document could not be serialized: {exception.Message}");
        }

        var tempPath = Path.Combine(directory, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
        try
        {
            await File.WriteAllBytesAsync(tempPath, bytes, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            File.Move(tempPath, path, overwrite: false);
        }
        catch (OperationCanceledException)
        {
            TryDelete(tempPath);
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            TryDelete(tempPath);
            return OperationResult<OverlayDocument>.Failure(
                ErrorCodes.IoAccessDenied,
                $"Access to overlay file '{path}' was denied.");
        }
        catch (IOException exception)
        {
            TryDelete(tempPath);
            return OperationResult<OverlayDocument>.Failure(
                ErrorCodes.IoError,
                $"The overlay file '{path}' could not be created: {exception.Message}");
        }

        OperationResult<OverlayDocument> load;
        try
        {
            load = await LoadAsync(descriptor, metadata, loader, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            TryDelete(path);
            throw;
        }

        if (!load.IsSuccess)
        {
            TryDelete(path);
        }

        return load;
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
            // A failed cleanup must not hide the original error.
        }
        catch (UnauthorizedAccessException)
        {
            // A failed cleanup must not hide the original error.
        }
    }

    /// <summary>
    /// Saves the overlay when it has unsaved changes.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the save.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is the same as for <see cref="TableFileSaver.SaveAsync"/>.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled; the file is left unchanged.</exception>
    public Task<OperationResult> SaveAsync(CancellationToken cancellationToken = default) =>
        new TableFileSaver().SaveAsync(_document, cancellationToken);
}
