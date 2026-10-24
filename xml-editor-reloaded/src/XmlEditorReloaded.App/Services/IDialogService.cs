namespace XmlEditorReloaded.App.Services;

using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Xml.Workspace;

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
    /// Shows a 32-bit bit-flags editor.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="value">Initial value.</param>
    /// <param name="labels">Optional names for individual bits, keyed by bit number.</param>
    /// <param name="valueName">Optional name of the exact value, for example a lookup combination entry.</param>
    /// <returns>The edited value, or <see langword="null"/> when cancelled.</returns>
    uint? EditBits(
        string title,
        uint value,
        IReadOnlyDictionary<int, string>? labels = null,
        string? valueName = null);

    /// <summary>
    /// Shows the read-only translation report for the current overlay.
    /// </summary>
    /// <param name="report">Translation report to display.</param>
    /// <param name="title">Window title, including the table and language names.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report"/> is null.</exception>
    void ShowTranslationReport(TranslationReport report, string title);

    /// <summary>
    /// Shows the bulk edit dialog with a live preview of the requested changes.
    /// </summary>
    /// <param name="context">Columns, target row keys, and the preview callback.</param>
    /// <returns>The request to apply, or <see langword="null"/> when the user cancels the dialog.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
    BulkEditRequest? EditBulk(BulkEditContext context);
}
