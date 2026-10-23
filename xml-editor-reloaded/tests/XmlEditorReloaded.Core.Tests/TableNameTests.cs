using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Tests;

/// <summary>
/// Tests for <see cref="TableName"/>.
/// </summary>
public class TableNameTests
{
    /// <summary>
    /// Dots become forward slashes and <c>.xml</c> is appended.
    /// </summary>
    [Theory]
    [InlineData("Items.Items", "Items/Items.xml")]
    [InlineData("Lookup.ItemClass", "Lookup/ItemClass.xml")]
    [InlineData("Items.Item_Transformations", "Items/Item_Transformations.xml")]
    public void TableName_ToRelativePath_DotsBecomeSeparators(string dotted, string expected)
    {
        var name = TableName.Parse(dotted);

        Assert.Equal(expected, name.ToRelativePath());
    }

    /// <summary>
    /// A relative path round-trips back to the dotted name.
    /// </summary>
    [Theory]
    [InlineData("Items/Items.xml", "Items.Items")]
    [InlineData("Lookup/SkillCheckType.xml", "Lookup.SkillCheckType")]
    public void TableName_TryFromRelativePath_RoundTrips(string relativePath, string expectedDotted)
    {
        Assert.True(TableName.TryFromRelativePath(relativePath, out var tableName));

        Assert.Equal(expectedDotted, tableName.Value);
        Assert.Equal(relativePath, tableName.ToRelativePath());
    }

    /// <summary>
    /// Paths without an <c>.xml</c> extension are rejected.
    /// </summary>
    [Fact]
    public void TableName_TryFromRelativePath_RejectsNonXml()
    {
        Assert.False(TableName.TryFromRelativePath("Items/readme.txt", out _));
        Assert.False(TableName.TryFromRelativePath("", out _));
        Assert.False(TableName.TryFromRelativePath("Items/Items.xml.bak", out _));
    }
}
