using XmlEditorReloaded.Core.Api;

namespace XmlEditorReloaded.Xml.Copy;

/// <summary>
/// Copies records between two <c>TableData</c> directories. The source directory is always read-only;
/// the target is modified only by <see cref="ApplyAsync"/> after the caller has reviewed the preview.
/// </summary>
public interface ITableCopyService
{
    /// <summary>
    /// Plans a copy and reports conflicts, key allocation, reference remapping, and related rows
    /// without writing any file.
    /// </summary>
    /// <param name="request">Copy request.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the plan.
    /// On failure, <c>error_code</c> is <c>copy.same_directory</c>, <c>copy.target_table_missing</c>,
    /// <c>io.file_not_found</c>, <c>xml.parse_error</c>, or <c>xml.not_saveable</c>, and
    /// <c>error_text</c> describes the problem.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<TableCopyPreview>> PreviewAsync(
        TableCopyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recomputes the plan against the current files, writes the target records, and reports the saved
    /// files. A failed multi-file write restores the files that were already replaced.
    /// </summary>
    /// <param name="request">Copy request.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the applied tables.
    /// On failure, <c>error_code</c> is <c>copy.no_records</c>, <c>copy.same_directory</c>,
    /// <c>copy.target_table_missing</c>, <c>xml.file_changed_externally</c>, <c>io.access_denied</c>, or
    /// another save error, and <c>error_text</c> describes the problem.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<TableCopyResult>> ApplyAsync(
        TableCopyRequest request,
        CancellationToken cancellationToken = default);
}
