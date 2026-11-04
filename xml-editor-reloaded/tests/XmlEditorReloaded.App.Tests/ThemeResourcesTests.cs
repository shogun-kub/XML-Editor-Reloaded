using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the light and dark palette dictionaries.
/// </summary>
public class ThemeResourcesTests
{
    private static readonly XNamespace Xaml = "http://schemas.microsoft.com/winfx/2006/xaml";

    /// <summary>
    /// Both palettes define exactly the same brush keys so switching the dictionary cannot leave a
    /// control with an unresolved color.
    /// </summary>
    [Fact]
    public void LightAndDarkPalettes_DefineTheSameKeys()
    {
        var light = Load("Light");
        var dark = Load("Dark");

        var lightKeys = Keys(light);
        var darkKeys = Keys(dark);

        Assert.Equal(lightKeys, darkKeys);
        Assert.Contains("Theme.Window.Background", lightKeys);
        Assert.Contains("Theme.Text", lightKeys);
        Assert.Contains("Theme.Accent", lightKeys);
    }

    /// <summary>
    /// The primary text color has a readable contrast ratio against the panel background in both
    /// palettes.
    /// </summary>
    [Theory]
    [InlineData("Light")]
    [InlineData("Dark")]
    public void TextOnPanel_HasReadableContrast(string name)
    {
        var palette = Load(name);
        var text = ColorOf(palette, "Theme.Text");
        var panel = ColorOf(palette, "Theme.Panel.Background");

        var ratio = Contrast(text, panel);
        Assert.True(ratio >= 4.5, $"{name}: contrast {ratio:F2} is below 4.5.");
    }

    private static XDocument Load(string name) =>
        XDocument.Load(Path.Combine(AppContext.BaseDirectory, "Themes", $"Theme.{name}.xaml"));

    private static string[] Keys(XDocument palette) => palette
        .Descendants()
        .Where(static element => element.Name.LocalName == "SolidColorBrush")
        .Select(static element => (string?)element.Attribute(Xaml + "Key"))
        .Where(static key => key is not null)
        .Select(static key => key!)
        .Order(StringComparer.Ordinal)
        .ToArray();

    private static Color ColorOf(XDocument palette, string key)
    {
        var value = palette
            .Descendants()
            .First(element => (string?)element.Attribute(Xaml + "Key") == key)
            .Attribute("Color")?.Value
            ?? throw new InvalidOperationException($"'{key}' has no Color attribute.");
        return Color.Parse(value);
    }

    private static double Contrast(Color first, Color second)
    {
        var firstLuminance = Luminance(first);
        var secondLuminance = Luminance(second);
        var lighter = Math.Max(firstLuminance, secondLuminance);
        var darker = Math.Min(firstLuminance, secondLuminance);
        return (lighter + 0.05) / (darker + 0.05);
    }

    private static double Luminance(Color color)
    {
        static double Channel(byte value)
        {
            var normalized = value / 255.0;
            return normalized <= 0.03928
                ? normalized / 12.92
                : Math.Pow((normalized + 0.055) / 1.055, 2.4);
        }

        return (0.2126 * Channel(color.R)) + (0.7152 * Channel(color.G)) + (0.0722 * Channel(color.B));
    }

    private readonly record struct Color(byte R, byte G, byte B)
    {
        public static Color Parse(string text)
        {
            var value = text.TrimStart('#');
            Assert.Equal(6, value.Length);
            return new Color(
                byte.Parse(value[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                byte.Parse(value.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                byte.Parse(value.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
        }
    }
}
