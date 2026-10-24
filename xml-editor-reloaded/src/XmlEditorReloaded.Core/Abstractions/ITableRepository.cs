using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Abstractions;

/// <summary>
/// Lifecycle status of a loaded table document.
/// </summary>
public enum TableDocumentStatus
{
    /// <summary>The document parsed successfully and can be edited and saved.</summary>
    Loaded,

    /// <summary>The document parsed but must not be saved, for example a read-only diagnostic view.</summary>
    LoadedReadOnly,

    /// <summary>The file is malformed; only diagnostics are available and saving is refused.</summary>
    Malformed,
}

/// <summary>
/// A diagnostic message produced while loading a table file.
/// </summary>
/// <param name="Code">Stable machine-readable diagnostic code, for example <c>xml.parse_error</c>.</param>
/// <param name="Message">Human-readable description.</param>
/// <param name="Line">One-based line number in the source file, when known.</param>
/// <param name="Position">One-based character position in the source line, when known.</param>
public sealed record LoadDiagnostic(string Code, string Message, int? Line = null, int? Position = null);

/// <summary>
/// Result of loading a table file: status, diagnostics, and an opaque document handle.
/// The concrete document type is owned by the backend that produced the load
/// (for the XML backend it is <c>XmlEditorReloaded.Xml.Documents.TableDocument</c>).
/// </summary>
/// <param name="Descriptor">Descriptor of the loaded table.</param>
/// <param name="Status">Lifecycle status of the document.</param>
/// <param name="Document">Opaque document handle; <see langword="null"/> when the file is malformed.</param>
/// <param name="Diagnostics">Diagnostics collected during the load.</param>
public sealed record TableLoadResult(
    TableDescriptor Descriptor,
    TableDocumentStatus Status,
    object? Document,
    IReadOnlyList<LoadDiagnostic> Diagnostics)
{
    /// <summary>
    /// Attempts to cast the opaque document handle to the concrete document type.
    /// </summary>
    /// <typeparam name="TDocument">Expected concrete document type.</typeparam>
    /// <returns>The typed document, or <see langword="null"/> when absent or of another type.</returns>
    public TDocument? GetDocument<TDocument>()
        where TDocument : class => Document as TDocument;
}

/// <summary>
/// Loads and saves table documents for a data directory.
/// </summary>
public interface ITableRepository
{
    /// <summary>
    /// Loads the table document described by <paramref name="descriptor"/>.
    /// </summary>
    /// <param name="descriptor">Table descriptor discovered by <see cref="ITableCatalog"/>.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the load result;
    /// on failure, <c>error_code</c> is <c>io.file_not_found</c> or <c>io.error</c> and <c>error_text</c> describes the problem.
    /// A malformed file is reported as a successful load result with
    /// <see cref="TableDocumentStatus.Malformed"/> and diagnostics, not as an operation failure.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<TableLoadResult>> LoadTableAsync(TableDescriptor descriptor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the document contained in a load result when it has unsaved changes.
    /// </summary>
    /// <param name="loadResult">Result previously returned by <see cref="LoadTableAsync"/>.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>;
    /// on failure, <c>error_code</c> is <c>xml.not_saveable</c>, <c>xml.file_changed_externally</c>, or <c>io.error</c>.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult> SaveTableAsync(TableLoadResult loadResult, CancellationToken cancellationToken = default);
}
