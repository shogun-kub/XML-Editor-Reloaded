using System.Globalization;
using System.Resources;

namespace XmlEditorReloaded.App.Resources;

/// <summary>
/// Access to localized UI strings from RESX resources.
/// </summary>
internal static class Strings
{
    private static readonly ResourceManager Manager = new(
        "XmlEditorReloaded.App.Resources.Strings",
        typeof(Strings).Assembly);

    private static CultureInfo? _uiCulture;

    /// <summary>
    /// Selects the interface language explicitly. The language is an application setting, so it must not
    /// depend on the ambient culture of the thread that creates a window: a dialog created from another
    /// thread would otherwise be localized in the operating-system language.
    /// </summary>
    /// <param name="culture">Culture whose resources are shown.</param>
    public static void SetUiCulture(CultureInfo culture) => _uiCulture = culture;

    /// <summary>
    /// Returns a localized string by resource key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <returns>The localized text, or the key itself when no resource exists.</returns>
    public static string Get(string key) =>
        Manager.GetString(key, _uiCulture ?? CultureInfo.CurrentUICulture) ?? key;

    /// <summary>
    /// Returns a formatted localized string.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>The formatted localized text with the arguments applied.</returns>
    public static string Format(string key, params object[] args) =>
        string.Format(CultureInfo.CurrentCulture, Get(key), args);
}
