using XmlEditorReloaded.Xml.Values;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="LexicalValue"/>.
/// </summary>
public class LexicalValueTests
{
    /// <summary>
    /// Decimal forms keep their original text, including trailing zero and sign.
    /// </summary>
    [Theory]
    [InlineData("1.0")]
    [InlineData("0.0")]
    [InlineData("-0.2")]
    [InlineData("17.5")]
    public void LexicalValue_PreservesDecimalText(string text)
    {
        var value = new LexicalValue(text);

        Assert.Equal(text, value.RawText);
        Assert.Equal(text, value.ToString());
        Assert.NotNull(value.AsDecimal);
    }

    /// <summary>
    /// Integer and boolean parsing uses invariant forms only.
    /// </summary>
    [Fact]
    public void LexicalValue_ParsesIntegersAndBooleanFlags()
    {
        Assert.Equal(-1, new LexicalValue("-1").AsInt32);
        Assert.True(new LexicalValue("1").AsBoolean);
        Assert.False(new LexicalValue("0").AsBoolean);
        Assert.Null(new LexicalValue("true").AsBoolean);
    }

    /// <summary>
    /// Empty element text is a present value, not a missing field.
    /// </summary>
    [Fact]
    public void LexicalValue_EmptyIsDistinctFromNull()
    {
        var value = new LexicalValue(string.Empty);

        Assert.True(value.IsEmpty);
        Assert.Equal(string.Empty, value.RawText);
    }
}
