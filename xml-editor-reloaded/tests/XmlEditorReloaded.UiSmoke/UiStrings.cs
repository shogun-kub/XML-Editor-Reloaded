using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke;

/// <summary>
/// Localized application strings read directly from the application RESX files, so the smoke checks
/// assert the same texts the UI shows in the selected language.
/// </summary>
internal sealed class UiStrings
{
    private readonly Dictionary<string, string> _values;

    private UiStrings(Dictionary<string, string> values, string language)
    {
        _values = values;
        Language = language;
    }

    /// <summary>
    /// Gets the UI language code the strings were loaded for.
    /// </summary>
    public string Language { get; }

    /// <summary>
    /// Loads the neutral strings and overlays the language-specific file, mirroring the RESX fallback
    /// of the application.
    /// </summary>
    /// <param name="resourcesDirectory">Directory that contains <c>Strings.resx</c>.</param>
    /// <param name="language">Two-letter UI language code, for example <c>ru</c>.</param>
    /// <returns>Loaded strings.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the neutral resource file is missing.</exception>
    public static UiStrings Load(string resourcesDirectory, string language)
    {
        var values = ReadResx(Path.Combine(resourcesDirectory, "Strings.resx"));
        var localized = Path.Combine(resourcesDirectory, $"Strings.{language}.resx");
        if (File.Exists(localized))
        {
            foreach (var pair in ReadResx(localized))
            {
                values[pair.Key] = pair.Value;
            }
        }

        return new UiStrings(values, language);
    }

    /// <summary>
    /// Returns a localized string, or the key itself when it is missing.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <returns>Localized text.</returns>
    public string Get(string key) => _values.TryGetValue(key, out var value) ? value : key;

    /// <summary>
    /// Returns a formatted localized string.
    /// </summary>
    /// <param name="key">Resource key.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>Localized text with the arguments applied.</returns>
    public string Format(string key, params object[] args) =>
        string.Format(CultureInfo.CurrentCulture, Get(key), args);

    private static Dictionary<string, string> ReadResx(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("A resource file is missing.", path);
        }

        var document = XDocument.Load(path);
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var element in document.Root?.Elements("data") ?? Enumerable.Empty<XElement>())
        {
            var name = element.Attribute("name")?.Value;
            var value = element.Element("value")?.Value;
            if (!string.IsNullOrEmpty(name) && value is not null)
            {
                values[name] = value;
            }
        }

        return values;
    }
}
