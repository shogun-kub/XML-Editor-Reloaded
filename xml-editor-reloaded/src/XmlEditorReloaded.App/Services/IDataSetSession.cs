using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// A runtime binding to one data set: the table directory, its lookup repository, translation catalog,
/// and the factory for editable table workspaces.
/// </summary>
public interface IDataSetSession
{
    /// <summary>
    /// Gets the absolute <c>TableData</c> directory of the data set.
    /// </summary>
    string TableDataDirectory { get; }

    /// <summary>
    /// Gets the game data directory that contains <c>TableData</c>, or <see langword="null"/>.
    /// </summary>
    string? GameDataDirectory { get; }

    /// <summary>
    /// Gets the table catalog of the data set.
    /// </summary>
    ITableCatalog Catalog { get; }

    /// <summary>
    /// Gets the table repository of the data set.
    /// </summary>
    ITableRepository Repository { get; }

    /// <summary>
    /// Gets the lookup repository bound to this directory, including language overlays.
    /// </summary>
    ILookupRepository Lookups { get; }

    /// <summary>
    /// Gets the translation catalog bound to this directory.
    /// </summary>
    ITranslationCatalog Translations { get; }

    /// <summary>
    /// Gets the resolver that loads relation target tables for pickers and navigation.
    /// </summary>
    ITableRelationResolver RelationResolver { get; }

    /// <summary>
    /// Returns the relations declared by one source table.
    /// </summary>
    /// <param name="sourceTable">Dotted source table name.</param>
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

    /// <summary>
    /// Opens a table for editing, optionally with a language overlay.
    /// </summary>
    /// <param name="descriptor">Base table descriptor discovered by <see cref="Catalog"/>.</param>
    /// <param name="languageCode">Language code whose overlay should be loaded, or <see langword="null"/> for base-only.</param>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the workspace.
    /// On failure, <c>error_code</c> is <c>io.file_not_found</c>, <c>xml.parse_error</c>,
    /// <c>metadata.not_found</c>, or <c>overlay.not_found</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptor"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<TableWorkspace>> OpenTableAsync(
        TableDescriptor descriptor,
        string? languageCode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens the primary table together with its 1:1 and 1:N related tables as one editable set with a
    /// shared undo history.
    /// </summary>
    /// <param name="descriptor">Descriptor of the primary table.</param>
    /// <param name="languageCode">Language code whose overlays should be loaded, or <see langword="null"/> for base-only.</param>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the set. On failure,
    /// <c>error_code</c> is <c>io.file_not_found</c>, <c>xml.parse_error</c>, or a metadata error for the
    /// primary table. Missing related files are reported through the set warnings.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptor"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<TableSetWorkspace>> OpenTableSetAsync(
        TableDescriptor descriptor,
        string? languageCode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns every language configured in the metadata database, ordered for display.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the configured languages.
    /// On failure, <c>error_code</c> is <c>metadata.not_found</c> or a metadata I/O error.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<IReadOnlyList<LanguageDescriptor>>> GetLanguagesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures that a language overlay exists for the table and returns an updated descriptor that
    /// includes it. An existing overlay is loaded but never overwritten; a missing one is created as
    /// an empty overlay containing only the root element declared by metadata.
    /// </summary>
    /// <param name="descriptor">Base table descriptor discovered by <see cref="Catalog"/>.</param>
    /// <param name="language">Language whose overlay should be created or reused.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the descriptor
    /// with the overlay appended. On failure, <c>error_code</c> is <c>metadata.not_found</c>,
    /// <c>io.file_not_found</c>, <c>io.access_denied</c>, <c>io.error</c>, <c>xml.parse_error</c>, or
    /// <c>xml.unexpected_root</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptor"/> or <paramref name="language"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<TableDescriptor>> EnsureOverlayAsync(
        TableDescriptor descriptor,
        LanguageDescriptor language,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Creates and caches <see cref="IDataSetSession"/> instances for user-selected directories.
/// </summary>
public interface IDataSetSessionFactory
{
    /// <summary>
    /// Opens a session for the selected path, resolving its <c>TableData</c> directory.
    /// </summary>
    /// <param name="selectedPath">
    /// Path selected in the shell; it may point at <c>TableData</c>, <c>Data-1.13</c>, or a game root.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the session.
    /// On failure, <c>error_code</c> is <c>io.file_not_found</c> when no <c>TableData</c> directory exists.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<IDataSetSession>> OpenAsync(
        string? selectedPath,
        CancellationToken cancellationToken = default);
}
