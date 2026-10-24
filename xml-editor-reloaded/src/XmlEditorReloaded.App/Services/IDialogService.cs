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
}
