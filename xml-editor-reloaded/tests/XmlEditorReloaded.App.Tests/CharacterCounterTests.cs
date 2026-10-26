using System.Globalization;
using XmlEditorReloaded.App.Resources;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the localized remaining-character counter of the item detail form.
/// </summary>
public class CharacterCounterTests
{
    /// <summary>
    /// Remaining characters are reported with the localized English text.
    /// </summary>
    [Fact]
    public void Format_RemainingCharacters_UsesEnglishText()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");

        Assert.Equal("388 characters left", CharacterCounter.Format(400, 12));
        Assert.Equal("400 characters left", CharacterCounter.Format(400, 0));
    }

    /// <summary>
    /// Remaining characters are reported with the localized Russian text.
    /// </summary>
    [Fact]
    public void Format_RemainingCharacters_UsesRussianText()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("ru");
        try
        {
            Assert.Equal("Осталось символов: 388", CharacterCounter.Format(400, 12));
        }
        finally
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en");
        }
    }

    /// <summary>
    /// A stored value longer than the limit is reported as an excess instead of a negative count.
    /// </summary>
    [Fact]
    public void Format_OverLimit_ReportsExcess()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");

        Assert.Equal("27 characters over the limit", CharacterCounter.Format(63, 90));
    }

    /// <summary>
    /// A field without a limit shows no counter.
    /// </summary>
    [Fact]
    public void Format_NoLimit_ReturnsEmptyString()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");

        Assert.Equal(string.Empty, CharacterCounter.Format(0, 5));
    }
}
