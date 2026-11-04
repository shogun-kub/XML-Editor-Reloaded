using XmlEditorReloaded.Xml.Lookups;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="LookupIdAllocator"/>.
/// </summary>
public class LookupIdAllocatorTests
{
    /// <summary>
    /// A sequential lookup receives the maximum id plus one, and an empty lookup starts at zero.
    /// </summary>
    [Fact]
    public void SuggestNextId_Sequential_ReturnsMaximumPlusOne()
    {
        Assert.Equal(24UL, LookupIdAllocator.SuggestNextId(new[] { "0", "1", "23" }, isFlagLookup: false));
        Assert.Equal(0UL, LookupIdAllocator.SuggestNextId(Array.Empty<string>(), isFlagLookup: false));
    }

    /// <summary>
    /// A gap in a sequential lookup is not reused, so an old value never silently points at a new name.
    /// </summary>
    [Fact]
    public void SuggestNextId_Sequential_DoesNotReuseGap()
    {
        Assert.Equal(24UL, LookupIdAllocator.SuggestNextId(new[] { "0", "1", "8", "23" }, isFlagLookup: false));
    }

    /// <summary>
    /// A flag lookup receives the lowest unused bit even when combinations already use that bit.
    /// </summary>
    [Fact]
    public void SuggestNextId_Flags_ReturnsLowestFreeBit()
    {
        Assert.Equal(1UL, LookupIdAllocator.SuggestNextId(Array.Empty<string>(), isFlagLookup: true));
        Assert.Equal(8UL, LookupIdAllocator.SuggestNextId(new[] { "0", "1", "2", "3", "4" }, isFlagLookup: true));
        Assert.Equal(
            32UL,
            LookupIdAllocator.SuggestNextId(new[] { "0", "1", "2", "3", "4", "7", "8", "16" }, isFlagLookup: true));
    }

    /// <summary>
    /// A flag lookup that already covers every bit has no free identifier left.
    /// </summary>
    [Fact]
    public void SuggestNextId_Flags_AllBitsUsed_ReturnsNull()
    {
        var allBits = new[] { ulong.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture) };
        Assert.Null(LookupIdAllocator.SuggestNextId(allBits, isFlagLookup: true));
        Assert.Null(
            LookupIdAllocator.SuggestNextId(new[] { ulong.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture) }, isFlagLookup: false));
    }

    /// <summary>
    /// Invalid text is ignored and never becomes a suggestion.
    /// </summary>
    [Fact]
    public void SuggestNextId_IgnoresInvalidText()
    {
        Assert.Equal(2UL, LookupIdAllocator.SuggestNextId(new[] { "1", "", "-3", "1.5", "x" }, isFlagLookup: false));
        Assert.Equal(2UL, LookupIdAllocator.SuggestNextId(new[] { "1", "", "-3", "1.5", "x" }, isFlagLookup: true));
    }

    /// <summary>
    /// Identifier parsing accepts unsigned decimal text only.
    /// </summary>
    [Fact]
    public void TryParseId_AcceptsOnlyUnsignedDecimalText()
    {
        Assert.True(LookupIdAllocator.TryParseId("8589934593", out var id));
        Assert.Equal(8589934593UL, id);
        Assert.True(LookupIdAllocator.TryParseId(" 16 ", out id));
        Assert.Equal(16UL, id);

        Assert.False(LookupIdAllocator.TryParseId(null, out _));
        Assert.False(LookupIdAllocator.TryParseId("", out _));
        Assert.False(LookupIdAllocator.TryParseId("-1", out _));
        Assert.False(LookupIdAllocator.TryParseId("+1", out _));
        Assert.False(LookupIdAllocator.TryParseId("0x10", out _));
        Assert.False(LookupIdAllocator.TryParseId("18446744073709551616", out _));
    }

    /// <summary>
    /// Only a non-zero power of two is a single bit.
    /// </summary>
    [Fact]
    public void IsSingleBit_RecognizesPowersOfTwo()
    {
        Assert.True(LookupIdAllocator.IsSingleBit(1));
        Assert.True(LookupIdAllocator.IsSingleBit(16));
        Assert.True(LookupIdAllocator.IsSingleBit(1UL << 63));

        Assert.False(LookupIdAllocator.IsSingleBit(0));
        Assert.False(LookupIdAllocator.IsSingleBit(3));
        Assert.False(LookupIdAllocator.IsSingleBit(7));
        Assert.False(LookupIdAllocator.IsSingleBit(ulong.MaxValue));
    }
}
