using XmlEditorReloaded.Core.Abstractions;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Initial state shown by the settings dialog.
/// </summary>
/// <param name="DataDirectories">Configured data directories.</param>
/// <param name="ActiveDataDirectory">Currently active data directory, or <see langword="null"/>.</param>
/// <param name="UiLanguageCode">Language code of the editor interface, or <see langword="null"/> for the default.</param>
/// <param name="RecentDataDirectories">Recently used data directories, most recent first.</param>
/// <param name="ThemeMode">Visual theme currently selected in settings.</param>
public sealed record SettingsRequest(
    IReadOnlyList<string> DataDirectories,
    string? ActiveDataDirectory,
    string? UiLanguageCode,
    IReadOnlyList<string> RecentDataDirectories,
    EditorThemeMode ThemeMode = EditorThemeMode.System);

/// <summary>
/// Settings confirmed by the user.
/// </summary>
/// <param name="DataDirectories">Configured data directories in display order.</param>
/// <param name="ActiveDataDirectory">Data directory to activate, or <see langword="null"/> when none is configured.</param>
/// <param name="UiLanguageCode">Selected interface language code; it takes effect after a restart.</param>
/// <param name="ResetLayouts">
/// When <see langword="true"/>, every stored grid layout must be removed after the dialog closes.
/// </param>
/// <param name="ThemeMode">Selected visual theme; it takes effect immediately.</param>
public sealed record SettingsResult(
    IReadOnlyList<string> DataDirectories,
    string? ActiveDataDirectory,
    string UiLanguageCode,
    bool ResetLayouts,
    EditorThemeMode ThemeMode = EditorThemeMode.System);

/// <summary>
/// Shows the application settings dialog to the user.
/// </summary>
public interface ISettingsDialogService
{
    /// <summary>
    /// Opens the modal settings dialog.
    /// </summary>
    /// <param name="request">Initial settings state.</param>
    /// <returns>The confirmed settings, or <see langword="null"/> when the user cancels.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    SettingsResult? Show(SettingsRequest request);
}
