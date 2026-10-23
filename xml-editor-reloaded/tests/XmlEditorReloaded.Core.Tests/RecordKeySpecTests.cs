using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Tests;

/// <summary>
/// Tests for <see cref="RecordKeySpec"/> and <see cref="DictFieldDefinition"/>.
/// </summary>
public class RecordKeySpecTests
{
    /// <summary>
    /// Scalar, positional, and singleton segments can be combined into one specification.
    /// </summary>
    [Fact]
    public void RecordKeySpec_SupportsNestedAndPositionalSegments()
    {
        var spec = new RecordKeySpec(new[]
        {
            new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "mIndex"),
            new RecordKeySegment("GEARKIT", RecordKeyKind.Positional),
        });

        Assert.Equal(2, spec.Segments.Count);
        Assert.Equal("mIndex", spec.Primary.KeyField);
        Assert.Equal(RecordKeyKind.Positional, spec.Segments[1].Kind);
    }

    /// <summary>
    /// Scalar segments without a key field are rejected.
    /// </summary>
    [Fact]
    public void RecordKeySpec_ScalarWithoutKeyField_Throws()
    {
        Assert.Throws<ArgumentException>(() => new RecordKeySegment(string.Empty, RecordKeyKind.Scalar));
    }

    /// <summary>
    /// An empty segment list is rejected.
    /// </summary>
    [Fact]
    public void RecordKeySpec_EmptySegments_Throw()
    {
        Assert.Throws<ArgumentException>(() => new RecordKeySpec(Array.Empty<RecordKeySegment>()));
    }

    /// <summary>
    /// Dict masks expand the index placeholder into a concrete element name.
    /// </summary>
    [Theory]
    [InlineData("ItemCapacityPerSize%Index%", 0, "ItemCapacityPerSize0")]
    [InlineData("ItemCapacityPerSize%Index%", 12, "ItemCapacityPerSize12")]
    [InlineData("item%Index%", 3, "item3")]
    public void DictField_Expand_ProducesElementName(string mask, int index, string expected)
    {
        var field = new DictFieldDefinition(mask);

        Assert.Equal(expected, field.Expand(index));
    }

    /// <summary>
    /// Masks without the index placeholder are rejected.
    /// </summary>
    [Fact]
    public void DictField_MissingPlaceholder_Throws()
    {
        Assert.Throws<ArgumentException>(() => new DictFieldDefinition("ItemCapacityPerSize"));
    }
}
