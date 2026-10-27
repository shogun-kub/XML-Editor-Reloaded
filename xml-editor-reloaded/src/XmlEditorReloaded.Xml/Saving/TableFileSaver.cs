using System.Security.Cryptography;
using System.Xml;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Diagnostics;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Saving;

/// <summary>
/// Saves a <see cref="TableDocument"/> with a safety gate: unchanged documents and documents whose
/// serialized bytes are identical to the source are not rewritten.
/// </summary>
public sealed class TableFileSaver
{
    private readonly DocumentSerializer _serializer;
    private readonly LosslessDocumentPatcher _patcher = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TableFileSaver"/> class.
    /// </summary>
    /// <param name="serializer">Serializer to use; a default serializer is created when omitted.</param>
    public TableFileSaver(DocumentSerializer? serializer = null)
    {
        _serializer = serializer ?? new DocumentSerializer();
    }

    /// <summary>
    /// Saves the document when it has unsaved changes and the file has not changed externally.
    /// </summary>
    /// <param name="document">The loaded document to save.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <param name="forceOverwrite">
    /// When <see langword="true"/>, the external-change gate is skipped and the current editor contents
    /// replace the file. The written bytes become the new baseline of the document, so a later regular
    /// save succeeds even though the file changed on disk after loading. The caller must obtain the
    /// user's confirmation before passing this value; the default keeps the gate.
    /// </param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c>, <c>xml.file_changed_externally</c>,
    /// <c>xml.serialization_error</c>, <c>io.file_not_found</c>, <c>io.access_denied</c>, or <c>io.error</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled; the target file is left unchanged.</exception>
    public async Task<OperationResult> SaveAsync(
        TableDocument document,
        CancellationToken cancellationToken = default,
        bool forceOverwrite = false)
    {
        ArgumentNullException.ThrowIfNull(document);
        cancellationToken.ThrowIfCancellationRequested();

        if (document.Status != TableDocumentStatus.Loaded)
        {
            return Fail(
                ErrorCodes.XmlNotSaveable,
                "The document is not saveable because it was not loaded successfully.",
                OperationLogLevel.Warning);
        }

        if (!document.IsDirty)
        {
            return OperationResult.Success();
        }

        var path = document.Descriptor.FilePath;
        if (!forceOverwrite)
        {
            var externalChange = DetectExternalChange(document, path);
            if (externalChange is not null)
            {
                return LogAndReturn(
                    externalChange,
                    OperationLogLevel.Warning);
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        byte[] bytes;
        try
        {
            if (!_patcher.TryPatch(document, out bytes))
            {
                bytes = _serializer.Serialize(document);
            }
        }
        catch (Exception exception) when (exception is InvalidOperationException or XmlException)
        {
            return Fail(
                ErrorCodes.XmlSerializationError,
                $"The document could not be serialized: {exception.Message}",
                OperationLogLevel.Error,
                exception);
        }

        if (!CanReparse(bytes))
        {
            return Fail(
                ErrorCodes.XmlSerializationError,
                "The serialized XML failed verification by re-parsing and was not written.",
                OperationLogLevel.Error);
        }

        var hash = Convert.ToHexString(SHA256.HashData(bytes));
        if (string.Equals(hash, document.OriginalSha256, StringComparison.Ordinal))
        {
            document.MarkClean();
            return OperationResult.Success();
        }

        if (!forceOverwrite)
        {
            var secondCheck = DetectExternalChange(document, path);
            if (secondCheck is not null)
            {
                return LogAndReturn(
                    secondCheck,
                    OperationLogLevel.Warning);
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        var directory = Path.GetDirectoryName(path) ?? ".";
        var tempPath = Path.Combine(directory, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");

        try
        {
            await File.WriteAllBytesAsync(tempPath, bytes, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            File.Move(tempPath, path, overwrite: true);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (UnauthorizedAccessException exception)
        {
            return Fail(
                ErrorCodes.IoAccessDenied,
                $"Access to table file '{path}' was denied.",
                OperationLogLevel.Error,
                exception);
        }
        catch (IOException exception)
        {
            return Fail(
                ErrorCodes.IoError,
                $"The table file '{path}' could not be written: {exception.Message}",
                OperationLogLevel.Error,
                exception);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch (IOException)
                {
                    // A leftover temporary file is harmless; the original file was not affected.
                }
            }
        }

        document.AcceptSaved(bytes, File.GetLastWriteTimeUtc(path), hash);
        return OperationResult.Success();
    }

    private static OperationResult Fail(
        string errorCode,
        string message,
        OperationLogLevel level,
        Exception? exception = null)
    {
        OperationLog.Write(level, "TableFileSaver", message, errorCode, exception);
        return OperationResult.Failure(errorCode, message);
    }

    private static OperationResult LogAndReturn(OperationResult result, OperationLogLevel level)
    {
        OperationLog.Write(
            level,
            "TableFileSaver",
            result.ErrorText ?? "The table could not be saved.",
            result.ErrorCode);
        return result;
    }

    private static OperationResult? DetectExternalChange(TableDocument document, string path)
    {
        var info = new FileInfo(path);
        if (!info.Exists)
        {
            return OperationResult.Failure(
                ErrorCodes.IoFileNotFound,
                $"The table file '{path}' no longer exists.");
        }

        if (info.Length != document.OriginalLength || info.LastWriteTimeUtc != document.OriginalLastWriteTimeUtc)
        {
            return OperationResult.Failure(
                ErrorCodes.XmlFileChangedExternally,
                $"The table file '{path}' changed on disk after it was loaded.");
        }

        return null;
    }

    private static bool CanReparse(byte[] bytes)
    {
        try
        {
            using var stream = new MemoryStream(bytes, writable: false);
            using var reader = XmlReader.Create(stream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                IgnoreWhitespace = false,
                CloseInput = false,
            });
            while (reader.Read())
            {
                // Reading to the end verifies that the whole document is well-formed.
            }

            return true;
        }
        catch (XmlException)
        {
            return false;
        }
    }
}
