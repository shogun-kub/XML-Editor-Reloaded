using System.IO;

namespace XmlEditorReloaded.Data.Settings;

/// <summary>
/// Resolves the per-user application-data directory that stores settings, grid layouts, the
/// metadata database, and the operation log.
/// </summary>
/// <remarks>
/// The default is <c>%AppData%\XmlEditorReloaded</c>. The <c>XER_APP_DATA</c> environment variable
/// redirects it to another directory, which lets automated UI runs and portable copies use their own
/// settings, layouts, database, and log without touching the interactive user's profile. The game
/// data directories are never resolved from this directory.
/// </remarks>
public static class AppPaths
{
    /// <summary>
    /// Name of the environment variable that overrides the application-data directory with an
    /// absolute or relative path.
    /// </summary>
    public const string ApplicationDataOverrideVariable = "XER_APP_DATA";

    /// <summary>
    /// Name of the application-data folder under the roaming profile.
    /// </summary>
    public const string ApplicationFolderName = "XmlEditorReloaded";

    /// <summary>
    /// Gets the directory holding settings, layouts, the metadata database, and the operation log.
    /// The value is the absolute value of <c>XER_APP_DATA</c> when that variable is set to a
    /// non-blank value, or <c>%AppData%\XmlEditorReloaded</c> otherwise.
    /// </summary>
    public static string ApplicationDataDirectory
    {
        get
        {
            var overridden = Environment.GetEnvironmentVariable(ApplicationDataOverrideVariable);
            if (!string.IsNullOrWhiteSpace(overridden))
            {
                return Path.GetFullPath(overridden.Trim());
            }

            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ApplicationFolderName);
        }
    }

    /// <summary>
    /// Gets the default settings file path, <c>settings.json</c> in
    /// <see cref="ApplicationDataDirectory"/>.
    /// </summary>
    public static string SettingsFilePath => Path.Combine(ApplicationDataDirectory, "settings.json");

    /// <summary>
    /// Gets the default grid-layout file path, <c>layouts.json</c> in
    /// <see cref="ApplicationDataDirectory"/>.
    /// </summary>
    public static string LayoutsFilePath => Path.Combine(ApplicationDataDirectory, "layouts.json");

    /// <summary>
    /// Gets the default operation-log file path, <c>XmlEditorReloaded.log</c> in
    /// <see cref="ApplicationDataDirectory"/>.
    /// </summary>
    public static string LogFilePath => Path.Combine(ApplicationDataDirectory, "XmlEditorReloaded.log");
}
