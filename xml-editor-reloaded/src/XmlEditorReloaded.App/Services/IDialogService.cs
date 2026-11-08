namespace XmlEditorReloaded.App.Services;

using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Xml.Workspace;

/// <summary>
/// User decision for closing a tab or the application with unsaved changes.
/// </summary>
public enum UnsavedChangesChoice
{
    /// <summary>Save the changes and continue closing.</summary>
    Save,

    /// <summary>Discard the changes and continue closing.</summary>
    Discard,

    /// <summary>Keep the tab or application open.</summary>
    Cancel,
}

/// <summary>
/// User decision for a file that changed on disk after it was loaded.
/// </summary>
public enum ExternalChangeChoice
{
    /// <summary>Discard unsaved changes and read the file again.</summary>
    Reload,

    /// <summary>Replace the changed file with the editor contents.</summary>
    Overwrite,

    /// <summary>Keep the editor contents unsaved and leave the file as it is.</summary>
    Cancel,
}

/// <summary>
/// UI dialog abstraction used by view models.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows a folder picker.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="initialDirectory">Optional initial directory.</param>
    /// <returns>The selected folder path, or <see langword="null"/> when cancelled.</returns>
    string? PickFolder(string title, string? initialDirectory = null);

    /// <summary>
    /// Shows a yes/no confirmation dialog.
    /// </summary>
    /// <param name="message">Question shown to the user.</param>
    /// <param name="title">Dialog title.</param>
    /// <returns><see langword="true"/> when the user confirms; otherwise <see langword="false"/>.</returns>
    bool Confirm(string message, string title);

    /// <summary>
    /// Shows an informational message that the user only has to acknowledge.
    /// </summary>
    /// <param name="message">Text shown to the user.</param>
    /// <param name="title">Dialog title.</param>
    void ShowMessage(string message, string title)
    {
    }

    /// <summary>
    /// Asks whether unsaved changes should be saved, discarded, or kept.
    /// </summary>
    /// <param name="message">Text shown to the user.</param>
    /// <param name="title">Dialog title.</param>
    /// <returns>The selected decision; <see cref="UnsavedChangesChoice.Cancel"/> when the dialog is closed.</returns>
    UnsavedChangesChoice ConfirmUnsavedChanges(string message, string title);

    /// <summary>
    /// Asks how to resolve a file that changed on disk after it was loaded.
    /// </summary>
    /// <param name="message">Text shown to the user, usually with the failed file names.</param>
    /// <param name="title">Dialog title.</param>
    /// <returns>The selected decision; <see cref="ExternalChangeChoice.Cancel"/> when the dialog is closed.</returns>
    ExternalChangeChoice ResolveExternalChange(string message, string title);

    /// <summary>
    /// Shows an unsigned 64-bit bit-flags editor.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="value">Initial value.</param>
    /// <param name="labels">Optional names for individual bits, keyed by bit number.</param>
    /// <param name="valueName">Optional name of the exact value, for example a lookup combination entry.</param>
    /// <returns>The edited value, or <see langword="null"/> when cancelled.</returns>
    ulong? EditBits(
        string title,
        ulong value,
        IReadOnlyDictionary<int, string>? labels = null,
        string? valueName = null);

    /// <summary>
    /// Shows the About window with the product, version, and description of the application build.
    /// </summary>
    void ShowAbout()
    {
    }

    /// <summary>
    /// Shows or reuses the modeless context-help window.
    /// </summary>
    /// <param name="viewModel">Help content to display.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    void ShowHelp(HelpViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
    }

    /// <summary>
    /// Shows the read-only data-set window with the validation report and the global search.
    /// </summary>
    /// <param name="viewModel">Data-set view model; it loads the table list and starts validation itself.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    void ShowDataset(DatasetViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
    }

    /// <summary>
    /// Shows the read-only translation report for the current overlay.
    /// </summary>
    /// <param name="report">Translation report to display.</param>
    /// <param name="title">Window title, including the table and language names.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report"/> is null.</exception>
    void ShowTranslationReport(TranslationReport report, string title);

    /// <summary>
    /// Shows the seven-row enemy ammunition editor.
    /// </summary>
    /// <param name="viewModel">Loaded enemy-ammo view model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    void ShowEnemyAmmo(EnemyAmmoViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
    }

    /// <summary>
    /// Shows a save-file dialog.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="filter">File-type filter in the standard dialog format.</param>
    /// <param name="defaultFileName">Suggested file name.</param>
    /// <returns>The selected file path, or <see langword="null"/> when cancelled.</returns>
    string? PickSaveFile(string title, string filter, string defaultFileName);

    /// <summary>
    /// Shows the bulk edit dialog with a live preview of the requested changes.
    /// </summary>
    /// <param name="context">Columns, target row keys, and the preview callback.</param>
    /// <returns>The request to apply, or <see langword="null"/> when the user cancels the dialog.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
    BulkEditRequest? EditBulk(BulkEditContext context);

    /// <summary>
    /// Shows the new-item dialog: identifier, name, and item class.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="suggestedId">Initial identifier, usually the next free key.</param>
    /// <param name="suggestedName">Initial item name.</param>
    /// <param name="classes">Selectable item classes.</param>
    /// <returns>The entered values, or <see langword="null"/> when the user cancels the dialog.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="title"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    ItemCreationRequest? CreateItem(
        string title,
        string suggestedId,
        string suggestedName,
        IReadOnlyList<LookupOption> classes);

    /// <summary>
    /// Shows the free-form lookup identifier dialog. The user may keep the suggested id or type any
    /// unused unsigned 64-bit id; a duplicate or non-numeric value is rejected in the dialog.
    /// </summary>
    /// <param name="request">Lookup identity, current id, suggestion, and existing ids.</param>
    /// <returns>The accepted canonical identifier, or <see langword="null"/> when cancelled.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    string? EditLookupId(LookupIdEditRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return null;
    }

    /// <summary>
    /// Shows the read-only "where used" window for one lookup value.
    /// </summary>
    /// <param name="viewModel">Usage list and its navigation callback.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    void ShowLookupUsages(LookupUsageViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
    }
}
