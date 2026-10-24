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

    /// <summary>
    /// Returns a localized string by resource key.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <returns>The localized text, or the key itself when no resource exists.</returns>
    public static string Get(string key) =>
        Manager.GetString(key, CultureInfo.CurrentUICulture) ?? key;

    /// <summary>
    /// Returns a formatted localized string.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>The formatted localized text.</returns>
    public static string Format(string key, params object[] args) =>
        string.Format(CultureInfo.CurrentCulture, Get(key), args);
}
