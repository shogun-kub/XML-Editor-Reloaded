namespace XmlEditorReloaded.App.Services;

using XmlEditorReloaded.Core.Localization;

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
    /// <returns>The edited value, or <see langword="null"/> when cancelled.</returns>
    uint? EditBits(string title, uint value);

    /// <summary>
    /// Shows the read-only translation report for the current overlay.
    /// </summary>
    /// <param name="report">Translation report to display.</param>
    /// <param name="title">Window title, including the table and language names.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report"/> is null.</exception>
    void ShowTranslationReport(TranslationReport report, string title);
}
