using XmlEditorReloaded.Core.Api;

namespace XmlEditorReloaded.Core.Layout;

/// <summary>
/// Loads and stores per-table grid layouts outside the game data directories. A layout is keyed by
/// the data-set directory and the table name, so the same table in two data sets keeps separate
/// presentation state.
/// </summary>
public interface ITableLayoutStore
{
    /// <summary>
    /// Loads the layout of one table.
    /// </summary>
    /// <param name="dataSetDirectory">Absolute <c>TableData</c> directory of the data set.</param>
    /// <param name="tableName">Dotted table name, for example <c>Items.Items</c>.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the stored
    /// layout, or <see cref="TableLayout.Empty"/> when the table has no stored layout.
    /// On failure, <c>error_code</c> is <c>io.error</c> or <c>io.access_denied</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when a path or table name is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult<TableLayout>> LoadAsync(
        string dataSetDirectory,
        string tableName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores the layout of one table, replacing any previous layout for the same key.
    /// </summary>
    /// <param name="dataSetDirectory">Absolute <c>TableData</c> directory of the data set.</param>
    /// <param name="tableName">Dotted table name, for example <c>Items.Items</c>.</param>
    /// <param name="layout">Layout to persist.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>;
    /// on failure, <c>error_code</c> is <c>io.access_denied</c> or <c>io.error</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when a path or table name is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="layout"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult> SaveAsync(
        string dataSetDirectory,
        string tableName,
        TableLayout layout,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the stored layout of one table.
    /// </summary>
    /// <param name="dataSetDirectory">Absolute <c>TableData</c> directory of the data set.</param>
    /// <param name="tableName">Dotted table name, for example <c>Items.Items</c>.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>;
    /// on failure, <c>error_code</c> is <c>io.access_denied</c> or <c>io.error</c>.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when a path or table name is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult> RemoveAsync(
        string dataSetDirectory,
        string tableName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes every stored layout.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>;
    /// on failure, <c>error_code</c> is <c>io.access_denied</c> or <c>io.error</c>.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<OperationResult> ResetAllAsync(CancellationToken cancellationToken = default);
}
