using System.Globalization;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for relation picker data: option merging, unknown values, filtering, and navigation lookup.
/// </summary>
public class RelationColumnDataTests
{
    /// <summary>
    /// Filtered target rows come first, then values that already exist in the document but fall
    /// outside the filter or are absent from the target.
    /// </summary>
    [Fact]
    public void Create_MergesFilteredTargetAndDocumentValues()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var relation = CreateRelation();
        var target = CreateTarget();
        var data = RelationColumnData.Create(
            relation,
            target,
            new string?[] { "1", "0", "99", "abc", "1", null, string.Empty });

        Assert.True(data.IsAvailable);
        Assert.Equal(new[] { "1", "2", "0", "99", "abc" }, data.Options.Select(option => option.Value));
        Assert.False(Assert.Single(data.Options, option => option.Value == "1").IsUnknown);
        Assert.True(Assert.Single(data.Options, option => option.Value == "99").IsUnknown);
        Assert.Equal("Glock 17", data.Display("1"));
        Assert.Equal("Nothing", data.Display("0"));
        Assert.Equal("99 (unknown)", data.Display("99"));
        Assert.Equal(string.Empty, data.Display(null));
        Assert.Equal("Glock 17", data.GetName("1"));
        Assert.Null(data.GetName("99"));
        Assert.Contains("Glock 17", data.GetTooltip("1"), StringComparison.Ordinal);
        Assert.Contains("Unknown reference", data.GetTooltip("99"), StringComparison.Ordinal);
        Assert.Equal("Items.Items", data.TargetTable.Value);
    }

    /// <summary>
    /// Filtering matches display text and key, and an empty term returns every option.
    /// </summary>
    [Fact]
    public void Filter_MatchesDisplayAndValue()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var data = RelationColumnData.Create(
            CreateRelation(),
            CreateTarget(),
            new string?[] { "1", "99" });

        Assert.Equal(3, data.Filter(null).Count);
        Assert.Equal("2", Assert.Single(data.Filter("gren")).Value);
        Assert.Equal("99", Assert.Single(data.Filter("99")).Value);
        Assert.Empty(data.Filter("missing"));
    }

    /// <summary>
    /// Navigation resolves canonical keys and rejects unknown values.
    /// </summary>
    [Fact]
    public void TryGetTargetValue_ResolvesOnlyKnownKeys()
    {
        var data = RelationColumnData.Create(
            CreateRelation(),
            CreateTarget(),
            Array.Empty<string?>());

        Assert.True(data.TryGetTargetValue(" 1 ", out var value));
        Assert.Equal("1", value);
        Assert.False(data.TryGetTargetValue("99", out _));
        Assert.False(data.TryGetTargetValue(null, out _));
    }

    /// <summary>
    /// A failed target keeps raw values editable and exposes the error.
    /// </summary>
    [Fact]
    public void Failed_KeepsRawTextAndReportsError()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var data = RelationColumnData.Failed(
            CreateRelation(),
            "io.file_not_found",
            "The target table does not exist.");

        Assert.False(data.IsAvailable);
        Assert.Empty(data.Options);
        Assert.Equal("42", data.Display("42"));
        Assert.Contains("does not exist", data.GetTooltip("42"), StringComparison.Ordinal);
        Assert.False(data.TryGetTargetValue("42", out _));
    }

    private static TableRelation CreateRelation() =>
        new(
            TableName.Parse("Items.Items"),
            "DefaultAttachment",
            TableName.Parse("Items.Items"),
            TableRelationKind.Reference,
            "uiIndex",
            "szLongItemName",
            new RelationFilter("Attachment", RelationFilterOperator.Equals, "1"));

    private static RelationTarget CreateTarget()
    {
        var all = new[]
        {
            new RelationTargetEntry("0", "Nothing"),
            new RelationTargetEntry("1", "Glock 17"),
            new RelationTargetEntry("2", "Grenade"),
            new RelationTargetEntry("3", "Knife"),
        };
        return new RelationTarget(
            TableName.Parse("Items.Items"),
            "uiIndex",
            "szLongItemName",
            all,
            new[] { all[1], all[2] });
    }
}
