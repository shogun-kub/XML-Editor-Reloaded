using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Abstractions;

/// <summary>
/// Discovers table files and their language overlays inside a data directory.
/// </summary>
public interface ITableCatalog
{
    /// <summary>
    /// Scans the <c>TableData</c> directory of a data set and lists all base tables with their overlays.
    /// </summary>
    /// <param name="tableDataDirectory">Absolute path of the <c>TableData</c> directory.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the discovered tables;
    /// on failure, <c>error_code</c> is <c>io.file_not_found</c> or <c>io.error</c> and <c>error_text</c> describes the problem.
    /// </returns>
    Task<OperationResult<IReadOnlyList<TableDescriptor>>> GetTablesAsync(string tableDataDirectory, CancellationToken cancellationToken = default);
}
