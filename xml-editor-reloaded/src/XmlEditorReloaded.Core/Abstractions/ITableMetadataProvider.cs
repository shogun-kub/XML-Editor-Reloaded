using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Abstractions;

/// <summary>
/// Provides table metadata and the language catalog from a versioned metadata store.
/// </summary>
public interface ITableMetadataProvider
{
    /// <summary>
    /// Returns metadata for a single table.
    /// </summary>
    /// <param name="tableName">Dotted table name, for example <c>Items.Items</c>.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the metadata;
    /// on failure, <c>error_code</c> is <c>metadata.not_found</c> and <c>error_text</c> describes the problem.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<TableMetadata>> GetTableAsync(TableName tableName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns metadata for every table known to the metadata store.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the metadata list;
    /// on failure, <c>error_code</c> and <c>error_text</c> describe the problem.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<IReadOnlyList<TableMetadata>>> GetAllTablesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the languages known to the editor, ordered by <see cref="LanguageDescriptor.Ordinal"/>.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the language list;
    /// on failure, <c>error_code</c> and <c>error_text</c> describe the problem.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<IReadOnlyList<LanguageDescriptor>>> GetLanguagesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the relations declared by one source table, ordered by <see cref="TableRelation.Ordinal"/>
    /// and then by source field.
    /// </summary>
    /// <param name="sourceTable">Dotted table name that declares the relations.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the relations;
    /// a table without relations yields an empty list. On failure, <c>error_code</c> is
    /// <c>io.error</c> and <c>error_text</c> describes the problem.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<IReadOnlyList<TableRelation>>> GetRelationsAsync(
        TableName sourceTable,
        CancellationToken cancellationToken = default);
}
