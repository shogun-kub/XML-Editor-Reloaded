namespace XmlEditorReloaded.App.Services;

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
}
