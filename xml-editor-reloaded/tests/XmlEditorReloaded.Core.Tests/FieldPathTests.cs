using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Tests;

/// <summary>
/// Tests for <see cref="FieldPath"/>.
/// </summary>
public class FieldPathTests
{
    /// <summary>
    /// A single-segment path has depth 1 and the field name equals the segment.
    /// </summary>
    [Fact]
    public void FieldPath_SingleSegment_HasDepthOne()
    {
        var path = FieldPath.Parse("szItemName");

        Assert.Equal(1, path.Depth);
        Assert.Equal("szItemName", path.FieldName);
        Assert.Equal("szItemName", path.Text);
    }

    /// <summary>
    /// Nested paths split on slashes and preserve order.
    /// </summary>
    [Fact]
    public void FieldPath_ParsesNestedSegments()
    {
        var path = FieldPath.Parse("GEARKIT/mGearKitName");

        Assert.Equal(2, path.Depth);
        Assert.Equal(new[] { "GEARKIT", "mGearKitName" }, path.Segments);
        Assert.Equal("mGearKitName", path.FieldName);
    }

    /// <summary>
    /// Text form round-trips through <see cref="FieldPath.Parse(string)"/>.
    /// </summary>
    [Fact]
    public void FieldPath_Format_RoundTrips()
    {
        var original = FieldPath.Parse("CITYLIST/CITY/uiIndex");

        var reparsed = FieldPath.Parse(original.Text);

        Assert.Equal(original, reparsed);
        Assert.Equal(original.Text, reparsed.Text);
    }

    /// <summary>
    /// Empty or whitespace segments are rejected.
    /// </summary>
    [Fact]
    public void FieldPath_EmptySegments_Throw()
    {
        Assert.Throws<ArgumentException>(() => FieldPath.Parse(""));
        Assert.Throws<ArgumentException>(() => FieldPath.Parse("a//b"));
    }
}
