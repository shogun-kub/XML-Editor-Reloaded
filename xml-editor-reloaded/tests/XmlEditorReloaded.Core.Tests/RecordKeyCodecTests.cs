using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Tests;

/// <summary>
/// Tests for the canonical compound record key encoding.
/// </summary>
public class RecordKeyCodecTests
{
    /// <summary>
    /// Every component shape survives a compose/decompose round trip.
    /// </summary>
    [Fact]
    public void ComposeDecompose_RoundTripsMissingEmptyAndSpecialCharacters()
    {
        var components = new string?[]
        {
            null,
            string.Empty,
            "12",
            "a:b",
            "left|right",
            "Кириллица",
            "line\nbreak",
            "-",
        };

        var key = RecordKeyCodec.Compose(components);

        Assert.Equal(components, RecordKeyCodec.Decompose(key));
    }

    /// <summary>
    /// The canonical form never depends on separators inside component values.
    /// </summary>
    [Fact]
    public void Compose_SeparatorsInsideValues_AreUnambiguous()
    {
        var first = RecordKeyCodec.Compose(new string?[] { "a|b", "c" });
        var second = RecordKeyCodec.Compose(new string?[] { "a", "b|c" });

        Assert.NotEqual(first, second);
        Assert.Equal(new string?[] { "a|b", "c" }, RecordKeyCodec.Decompose(first));
        Assert.Equal(new string?[] { "a", "b|c" }, RecordKeyCodec.Decompose(second));
    }

    /// <summary>
    /// Missing and empty components are encoded differently.
    /// </summary>
    [Fact]
    public void Compose_MissingAndEmpty_AreDistinct()
    {
        Assert.NotEqual(
            RecordKeyCodec.Compose(new string?[] { null }),
            RecordKeyCodec.Compose(new string?[] { string.Empty }));
        Assert.Equal(new string?[] { null }, RecordKeyCodec.Decompose("-"));
        Assert.Equal(new string?[] { string.Empty }, RecordKeyCodec.Decompose("0:"));
    }

    /// <summary>
    /// Display uses a readable separator while preserving component order.
    /// </summary>
    [Fact]
    public void Display_JoinsComponentsForMessages()
    {
        Assert.Equal("12 | 37", RecordKeyCodec.Display(new string?[] { "12", "37" }));
        Assert.Equal("12 |  | 37", RecordKeyCodec.Display(new string?[] { "12", null, "37" }));
    }

    /// <summary>
    /// Malformed keys are rejected instead of being silently accepted.
    /// </summary>
    /// <param name="key">Invalid canonical key.</param>
    [Theory]
    [InlineData("12")]
    [InlineData("2:12|")]
    [InlineData("x:12")]
    [InlineData("5:12")]
    public void Decompose_InvalidKey_Throws(string key)
    {
        Assert.Throws<FormatException>(() => RecordKeyCodec.Decompose(key));
    }

    /// <summary>
    /// An empty input has no components.
    /// </summary>
    [Fact]
    public void ComposeDecompose_EmptyInput_ReturnsNoComponents()
    {
        Assert.Equal(string.Empty, RecordKeyCodec.Compose(Array.Empty<string?>()));
        Assert.Empty(RecordKeyCodec.Decompose(string.Empty));
    }
}
