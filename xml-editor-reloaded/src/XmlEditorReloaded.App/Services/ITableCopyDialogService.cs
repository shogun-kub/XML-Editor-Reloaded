using XmlEditorReloaded.Xml.Copy;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Initial state of the cross-data-set copy dialog.
/// </summary>
/// <param name="DataDirectories">
/// Configured data directories, in most-recent-first order. Each entry may point at a
/// <c>TableData</c> directory, a <c>Data-1.13</c> directory, or a game root.
/// </param>
/// <param name="InitialSourceDirectory">Directory that is pre-selected as the read-only source.</param>
/// <param name="InitialTableName">Dotted name of the table that is pre-selected, or <see langword="null"/>.</param>
/// <param name="InitialRecordKeys">
/// Keys that are pre-filled into the record filter, for example the item selected in the active tab.
/// <see langword="null"/> or an empty list means every record of the table.
/// </param>
public sealed record TableCopyDialogRequest(
    IReadOnlyList<string> DataDirectories,
    string InitialSourceDirectory,
    string? InitialTableName = null,
    IReadOnlyList<string>? InitialRecordKeys = null);

/// <summary>
/// Result of a copy dialog that the user confirmed.
/// </summary>
/// <param name="TargetTableDataDirectory">Absolute <c>TableData</c> directory that received the copied rows.</param>
/// <param name="Result">Per-table counts, saved files, and warnings of the committed copy.</param>
public sealed record TableCopyDialogOutcome(
    string TargetTableDataDirectory,
    TableCopyResult Result);

/// <summary>
/// Shows the modal dialog that copies table rows from one data set to another. The dialog always
/// previews the plan before writing and never modifies the chosen source directory.
/// </summary>
public interface ITableCopyDialogService
{
    /// <summary>
    /// Shows the copy dialog and returns the committed result, or <see langword="null"/> when the user
    /// cancelled without copying.
    /// </summary>
    /// <param name="request">Initial dialog state.</param>
    /// <param name="cancellationToken">Token to cancel table discovery before the dialog is shown.</param>
    /// <returns>The committed copy result, or <see langword="null"/> when nothing was copied.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    Task<TableCopyDialogOutcome?> ShowAsync(
        TableCopyDialogRequest request,
        CancellationToken cancellationToken = default);
}
