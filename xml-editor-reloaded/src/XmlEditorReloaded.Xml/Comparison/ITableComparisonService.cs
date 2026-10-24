using XmlEditorReloaded.Core.Api;

namespace XmlEditorReloaded.Xml.Comparison;

/// <summary>
/// Produces a read-only cross-version comparison of two <c>TableData</c> directories.
/// </summary>
public interface ITableComparisonService
{
    /// <summary>
    /// Compares two <c>TableData</c> directories without writing to either of them.
    /// </summary>
    /// <param name="leftDirectory">Absolute path of the baseline <c>TableData</c> directory.</param>
    /// <param name="rightDirectory">Absolute path of the candidate <c>TableData</c> directory.</param>
    /// <param name="cancellationToken">Token to cancel the comparison.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the report.
    /// A malformed or unreadable table is reported inside the report instead of failing the operation.
    /// On failure, <c>error_code</c> is <c>io.file_not_found</c>, <c>io.access_denied</c>,
    /// <c>io.error</c>, or a metadata error and <c>error_text</c> describes the problem.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when a directory path is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<TableComparisonReport>> CompareAsync(
        string leftDirectory,
        string rightDirectory,
        CancellationToken cancellationToken = default);
}
