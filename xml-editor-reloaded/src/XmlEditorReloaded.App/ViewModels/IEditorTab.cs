using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// A document tab shown in the main window. Implementations own their documents, report unsaved
/// changes, save on demand, and accept relation navigation.
/// </summary>
public interface IEditorTab
{
    /// <summary>
    /// Gets the table this tab edits. The value identifies the tab when a table is opened again.
    /// </summary>
    TableName TableName { get; }

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
    /// Saves the tab content when it has unsaved changes.
    /// </summary>
    /// <returns>A task that completes when saving finishes.</returns>
    Task SaveAsync();

    /// <summary>
    /// Selects a record by its primary key, for example after relation navigation.
    /// </summary>
    /// <param name="key">Primary record key.</param>
    void SelectRecord(string key);
}
