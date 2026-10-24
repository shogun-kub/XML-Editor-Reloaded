using System.Globalization;
using XmlEditorReloaded.Core.Filtering;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Filtering;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="TableRowFilterMatcher"/>.
/// </summary>
public class RowFilterMatcherTests
{
    /// <summary>
    /// An empty group matches every row.
    /// </summary>
    [Fact]
    public async Task EmptyGroup_MatchesEveryRow()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        var keys = Filter(table, RowFilterGroup.Empty);

        Assert.Equal(new[] { "0", "1" }, keys);
    }

    /// <summary>
    /// Text equality is case-insensitive and trims surrounding whitespace.
    /// </summary>
    [Fact]
    public async Task Equals_Text_IsCaseInsensitive()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        var keys = Filter(
            table,
            Group(new RowFilterCriterion("szItemName", RowFilterOperator.Equals, "  glock 17 ")));

        Assert.Equal(new[] { "1" }, keys);
    }

    /// <summary>
    /// Text inequality excludes only rows that match the operand.
    /// </summary>
    [Fact]
    public async Task NotEquals_Text_ExcludesMatchingRows()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        var keys = Filter(
            table,
            Group(new RowFilterCriterion("szItemName", RowFilterOperator.NotEquals, "Nothing")));

        Assert.Equal(new[] { "1" }, keys);
    }

    /// <summary>
    /// Contains searches the effective value as a case-insensitive substring.
    /// </summary>
    [Fact]
    public async Task Contains_Text_MatchesSubstring()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        var keys = Filter(
            table,
            Group(new RowFilterCriterion("szItemName", RowFilterOperator.Contains, "LOCK")));

        Assert.Equal(new[] { "1" }, keys);
    }

    /// <summary>
    /// Numeric comparisons use invariant parsing, so <c>1.0</c> and <c>1</c> are equal.
    /// </summary>
    [Fact]
    public async Task NumericComparisons_UseInvariantValues()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        Assert.Equal(
            new[] { "1" },
            Filter(table, Group(new RowFilterCriterion("ubWeight", RowFilterOperator.GreaterThan, "5"))));
        Assert.Equal(
            new[] { "0" },
            Filter(table, Group(new RowFilterCriterion("ubWeight", RowFilterOperator.LessOrEqual, "9"))));
        Assert.Equal(
            new[] { "1" },
            Filter(table, Group(new RowFilterCriterion("ProjectionFactor", RowFilterOperator.Equals, "1"))));
        Assert.Empty(Filter(table, Group(new RowFilterCriterion("ubWeight", RowFilterOperator.LessThan, "0"))));
    }

    /// <summary>
    /// A missing field never matches a positive comparison, but it does match exclusion.
    /// </summary>
    [Fact]
    public async Task MissingField_MatchesOnlyNotEquals()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        Assert.Equal(
            new[] { "0" },
            Filter(table, Group(new RowFilterCriterion("ProjectionFactor", RowFilterOperator.NotEquals, "1"))));
        Assert.Equal(
            new[] { "1" },
            Filter(table, Group(new RowFilterCriterion("ProjectionFactor", RowFilterOperator.Equals, "1"))));
        Assert.Empty(Filter(table, Group(new RowFilterCriterion("ProjectionFactor", RowFilterOperator.LessThan, "0.5"))));
    }

    /// <summary>
    /// AND requires every criterion to match; OR requires at least one.
    /// </summary>
    [Fact]
    public async Task AndOr_CombineCriteria()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);
        var and = new RowFilterGroup(
            RowFilterLogicalOperator.And,
            new[]
            {
                new RowFilterCriterion("ubWeight", RowFilterOperator.GreaterOrEqual, "10"),
                new RowFilterCriterion("szItemName", RowFilterOperator.Contains, "Glock"),
            });
        var or = new RowFilterGroup(
            RowFilterLogicalOperator.Or,
            new[]
            {
                new RowFilterCriterion("szItemName", RowFilterOperator.Contains, "Nothing"),
                new RowFilterCriterion("ubWeight", RowFilterOperator.GreaterThan, "5"),
            });

        Assert.Equal(new[] { "1" }, Filter(table, and));
        Assert.Equal(new[] { "0", "1" }, Filter(table, or));
    }

    /// <summary>
    /// An unknown field never matches and does not throw.
    /// </summary>
    [Fact]
    public async Task UnknownField_NeverMatches()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);
        var unknown = Group(new RowFilterCriterion("NoSuchField", RowFilterOperator.Equals, "1"));
        var or = new RowFilterGroup(
            RowFilterLogicalOperator.Or,
            new[]
            {
                new RowFilterCriterion("NoSuchField", RowFilterOperator.Equals, "1"),
                new RowFilterCriterion("uiIndex", RowFilterOperator.Equals, "0"),
            });

        Assert.Empty(Filter(table, unknown));
        Assert.Equal(new[] { "0" }, Filter(table, or));
    }

    /// <summary>
    /// Relational operators on text columns are not numeric and therefore never match.
    /// </summary>
    [Fact]
    public async Task RelationalOperators_OnTextField_DoNotMatch()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        Assert.Empty(Filter(table, Group(new RowFilterCriterion("szItemName", RowFilterOperator.GreaterThan, "A"))));
    }

    /// <summary>
    /// Numeric parsing stays invariant under a comma-decimal culture.
    /// </summary>
    [Fact]
    public async Task NumericComparisons_UnderCommaCulture_RemainInvariant()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            using var fixture = new TestWorkspace();
            var table = await LoadItemsAsync(fixture);

            var keys = Filter(
                table,
                Group(new RowFilterCriterion("ProjectionFactor", RowFilterOperator.GreaterOrEqual, "1.0")));

            Assert.Equal(new[] { "1" }, keys);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    private static RowFilterGroup Group(params RowFilterCriterion[] criteria) =>
        new(RowFilterLogicalOperator.And, criteria);

    private static string[] Filter(TableWorkspace table, RowFilterGroup group) =>
        table.GetRows()
            .Where(row => TableRowFilterMatcher.Matches(row, table.Schema, group))
            .Select(static row => row.Key!)
            .ToArray();

    private static async Task<TableWorkspace> LoadItemsAsync(TestWorkspace fixture)
    {
        var path = fixture.CopyFixture("items-mini.xml");
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await TableWorkspace.LoadAsync(descriptor, OverlayTestData.ItemsMetadata());
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!;
    }
}
