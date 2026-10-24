using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Tests;

/// <summary>
/// Tests for structured relation filters and resolved relation targets.
/// </summary>
public class RelationFilterTests
{
    /// <summary>
    /// Equals compares identifiers numerically and text case-insensitively, and never matches an
    /// absent value.
    /// </summary>
    [Fact]
    public void Equals_ComparesNumericAndTextValues()
    {
        var numeric = new RelationFilter("uiIndex", RelationFilterOperator.Equals, "7");
        Assert.True(numeric.Matches("7"));
        Assert.True(numeric.Matches(" 07 "));
        Assert.False(numeric.Matches("8"));
        Assert.False(numeric.Matches(null));
        Assert.False(numeric.Matches(string.Empty));

        var text = new RelationFilter("name", RelationFilterOperator.Equals, "Glock 17");
        Assert.True(text.Matches("glock 17"));
        Assert.False(text.Matches("Glock 18"));
    }

    /// <summary>
    /// NotEquals keeps absent fields, which mirrors the sparse-record meaning of a class exclusion.
    /// </summary>
    [Fact]
    public void NotEquals_KeepsAbsentAndDifferentValues()
    {
        var filter = new RelationFilter("usItemClass", RelationFilterOperator.NotEquals, "1");
        Assert.True(filter.Matches(null));
        Assert.True(filter.Matches(string.Empty));
        Assert.True(filter.Matches("2"));
        Assert.False(filter.Matches("1"));
    }

    /// <summary>
    /// Contains searches text case-insensitively and ignores empty values.
    /// </summary>
    [Fact]
    public void Contains_MatchesSubstring()
    {
        var filter = new RelationFilter("szLongItemName", RelationFilterOperator.Contains, "lock");
        Assert.True(filter.Matches("Glock 17"));
        Assert.False(filter.Matches("Knife"));
        Assert.False(filter.Matches(null));
    }

    /// <summary>
    /// HasFlag matches any set bit of the operand and requires a non-zero mask.
    /// </summary>
    [Fact]
    public void HasFlag_MatchesAnyRequestedBit()
    {
        var filter = new RelationFilter("usItemClass", RelationFilterOperator.HasFlag, "768");
        Assert.True(filter.Matches("256"));
        Assert.True(filter.Matches("512"));
        Assert.True(filter.Matches("768"));
        Assert.False(filter.Matches("1024"));
        Assert.False(filter.Matches("0"));
        Assert.False(filter.Matches(null));

        var emptyMask = new RelationFilter("usItemClass", RelationFilterOperator.HasFlag, "0");
        Assert.False(emptyMask.Matches("256"));
    }

    /// <summary>
    /// Target lookups resolve keys, display text, remapping by name, and search only the filtered
    /// options.
    /// </summary>
    [Fact]
    public void RelationTarget_ResolvesDisplayAndRemapsByName()
    {
        var all = new[]
        {
            new RelationTargetEntry("0", "Nothing"),
            new RelationTargetEntry("1", "Glock 17"),
            new RelationTargetEntry("2", "Grenade"),
        };
        var options = new[] { all[1], all[2] };
        var target = new RelationTarget(
            TableName.Parse("Items.Items"),
            "uiIndex",
            "szLongItemName",
            all,
            options);

        Assert.Equal("Glock 17", target.Find("1")!.Display);
        Assert.Null(target.Find("9"));
        Assert.True(target.Contains("2"));
        Assert.True(target.Contains("0"));
        Assert.DoesNotContain(all[0], target.Options);
        Assert.Equal("1", target.FindValueByDisplay(" glock 17 "));
        Assert.Equal("0", target.FindValueByDisplay("Nothing"));
        Assert.Null(target.FindValueByDisplay("Absent"));
        Assert.Null(target.FindValueByDisplay(null));

        Assert.Equal(2, target.Search(null).Count);
        Assert.Equal("Grenade", Assert.Single(target.Search("gren")).Display);
        Assert.Equal("2", Assert.Single(target.Search("2")).Value);
        Assert.Empty(target.Search("nothing"));
    }

    /// <summary>
    /// Relation metadata keeps kind, fields, filter, and optional ordering flags.
    /// </summary>
    [Fact]
    public void TableRelation_ExposesMetadata()
    {
        var filter = new RelationFilter("Attachment", RelationFilterOperator.Equals, "1");
        var relation = new TableRelation(
            TableName.Parse("Items.Items"),
            "DefaultAttachment",
            TableName.Parse("Items.Items"),
            TableRelationKind.Reference,
            "uiIndex",
            "szLongItemName",
            filter)
        {
            LabelKey = "Items.DefaultAttachment",
            RemapByText = true,
            Ordinal = 10,
        };

        Assert.Equal("DefaultAttachment", relation.SourceField);
        Assert.Equal(TableRelationKind.Reference, relation.Kind);
        Assert.Equal("szLongItemName", relation.TargetDisplayField);
        Assert.Same(filter, relation.Filter);
        Assert.Equal("Items.DefaultAttachment", relation.LabelKey);
        Assert.True(relation.RemapByText);
        Assert.Equal(10, relation.Ordinal);
    }
}
