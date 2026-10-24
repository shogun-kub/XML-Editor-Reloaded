using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Abstractions;

/// <summary>
/// Resolves relation targets from the active data directory, applying display fields, structured
/// filters, language overlays, and caching.
/// </summary>
public interface ITableRelationResolver
{
    /// <summary>
    /// Resolves the target records of a relation.
    /// </summary>
    /// <param name="relation">Relation to resolve.</param>
    /// <param name="languageCode">
    /// Optional language code whose overlay should supply translated display values;
    /// <see langword="null"/> returns base values only.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries all target
    /// records plus the filtered picker options. On failure, <c>error_code</c> is
    /// <c>io.file_not_found</c>, <c>xml.parse_error</c>, <c>metadata.not_found</c>, or <c>io.error</c>;
    /// a failure never prevents the source table from opening.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="relation"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<RelationTarget>> ResolveAsync(
        TableRelation relation,
        string? languageCode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Drops cached target data for a table, for example after the table was saved.
    /// </summary>
    /// <param name="tableName">Table whose cached target rows should be discarded.</param>
    void Invalidate(TableName tableName);
}
