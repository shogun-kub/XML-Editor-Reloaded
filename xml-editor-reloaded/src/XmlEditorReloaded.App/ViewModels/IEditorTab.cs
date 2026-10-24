using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// A document tab shown in the main window. Implementations own their documents, report unsaved
/// changes, save on demand, and accept relation navigation.
/// </summary>
public interface IEditorTab
{
    /// <summary>
    /// Gets the table this tab edits. The value identifies the tab within its data set when a table is
    /// opened again.
    /// </summary>
    TableName TableName { get; }

    /// <summary>
    /// Gets the absolute <c>TableData</c> directory of the data set that owns the tab. Together with
    /// <see cref="TableName"/> it identifies the tab when several data sets are open.
    /// </summary>
    string TableDataDirectory { get; }

    /// <summary>
    /// Gets a short label of the owning data set, for example the game directory name.
    /// </summary>
    string DatasetLabel { get; }

    /// <summary>
    /// Gets the primary key of the selected record, or <see langword="null"/> when no record is selected.
    /// </summary>
    string? SelectedRecordKey { get; }

    /// <summary>
    /// Gets the tab title without the dirty marker.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the title shown on the tab, including the dirty marker when needed.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets a value indicating whether the tab has unsaved changes.
    /// </summary>
    bool IsDirty { get; }

    /// <summary>
    /// Gets a value indicating whether a background operation is running.
    /// </summary>
    bool IsBusy { get; }

    /// <summary>
    /// Occurs after the tab successfully switches to another language or the base table.
    /// </summary>
    event EventHandler<string?>? LanguageChanged;

    /// <summary>
    /// Occurs when the user asks to open a record in another table.
    /// </summary>
    event EventHandler<TableNavigationRequest>? TableNavigationRequested;

    /// <summary>
    /// Loads the tab content.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>A task that completes when the tab is ready.</returns>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the tab content when it has unsaved changes. If a file changed on disk, the user is asked
    /// to reload, overwrite, or cancel before the tab is written.
    /// </summary>
    /// <returns>A task that completes when saving finishes.</returns>
    Task SaveAsync();

    /// <summary>
    /// Saves the tab content and recovers from external file changes with an explicit user choice.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> when the tab has no unsaved changes after the operation, either because it
    /// was saved, was already clean, or was reloaded from disk; <see langword="false"/> when the user
    /// cancelled, the overwrite failed, or the reload could not load the tab.
    /// </returns>
    Task<bool> SaveWithRecoveryAsync();

    /// <summary>
    /// Reloads the tab content from disk after an external change, for example a cross-data-set copy
    /// into the tab's table. Unsaved changes are not discarded by this method.
    /// </summary>
    /// <returns>A task that completes when the tab is reloaded.</returns>
    Task ReloadAsync();

    /// <summary>
    /// Discards unsaved changes and reads the tab content from disk without asking for confirmation.
    /// Callers must obtain the user's decision first.
    /// </summary>
    /// <returns>A task that completes when the tab is reloaded.</returns>
    Task ReloadFromDiskAsync();

    /// <summary>
    /// Selects a record by its primary key, for example after relation navigation.
    /// </summary>
    /// <param name="key">Primary record key.</param>
    void SelectRecord(string key);
}
