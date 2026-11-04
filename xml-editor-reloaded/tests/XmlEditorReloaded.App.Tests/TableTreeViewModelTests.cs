using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests the table tree built from relative table paths and its filter/find operations.
/// </summary>
public sealed class TableTreeViewModelTests
{
    /// <summary>
    /// The tree groups files by their real folders and keeps root files at the root.
    /// </summary>
    [Fact]
    public void Tree_groups_files_by_folder()
    {
        var tree = Build();

        Assert.Equal(3, tree.Roots.Count);
        Assert.Equal("Items", tree.Roots[0].Name);
        Assert.True(tree.Roots[0].IsFolder);
        Assert.Equal(2, tree.Roots[0].Children.Count);
        Assert.Equal("Lookup", tree.Roots[1].Name);
        Assert.Equal("Backgrounds", tree.Roots[2].Name);
        Assert.True(tree.Roots[2].IsLeaf);
    }

    /// <summary>
    /// Filtering hides non-matching leaves and their empty folders.
    /// </summary>
    [Fact]
    public void Filter_keeps_only_matching_nodes()
    {
        var tree = Build();
        tree.FilterText = "Weapons";

        Assert.Equal(1, tree.VisibleLeafCount);
        Assert.Single(tree.VisibleRoots);
        Assert.Equal("Items", tree.VisibleRoots[0].Name);
        Assert.Single(tree.VisibleRoots[0].VisibleChildren);
        Assert.Equal("Weapons", tree.VisibleRoots[0].VisibleChildren[0].Name);
    }

    /// <summary>
    /// Find in tree clears the filter, expands ancestors, and selects the requested leaf.
    /// </summary>
    [Fact]
    public void Find_in_tree_clears_filter_and_expands_ancestors()
    {
        var tree = Build();
        tree.FilterText = "Weapons";
        var table = tree.Roots[0].Children.First(item => item.Name == "Weapons").Table!;

        var leaf = tree.FindInTree(table);

        Assert.NotNull(leaf);
        Assert.True(leaf.IsSelected);
        Assert.Equal(string.Empty, tree.FilterText);
        Assert.True(tree.Roots[0].IsExpanded);
        Assert.Equal(4, tree.VisibleLeafCount);
    }

    /// <summary>
    /// A filter with no matches leaves the tree empty and displays the empty-state message.
    /// </summary>
    [Fact]
    public void Filter_without_matches_empties_the_tree()
    {
        var tree = Build();
        tree.FilterText = "no-such-table";

        Assert.False(tree.HasVisibleTables);
        Assert.Empty(tree.VisibleRoots);
        Assert.Equal(0, tree.VisibleLeafCount);
    }

    /// <summary>
    /// Fold all collapses folders without losing the hierarchy.
    /// </summary>
    [Fact]
    public void Expand_and_collapse_all_visit_every_folder()
    {
        var tree = Build();
        tree.ExpandAll();
        Assert.All(
            tree.Roots.Where(item => item.IsFolder),
            item => Assert.True(item.IsExpanded));

        tree.CollapseAll();
        Assert.All(
            tree.Roots.Where(item => item.IsFolder),
            item => Assert.False(item.IsExpanded));
    }

    private static TableTreeViewModel Build()
    {
        var tree = new TableTreeViewModel();
        tree.Load(new[]
        {
            Table("Items.Weapons", "Items/Weapons.xml"),
            Table("Items.Items", "Items/Items.xml"),
            Table("Lookup.Cursor", "Lookup/Cursor.xml"),
            Table("Backgrounds", "Backgrounds.xml"),
        });
        return tree;
    }

    private static TableListItem Table(string name, string relativePath)
    {
        var descriptor = new TableDescriptor(
            new TableName(name),
            $"C:\\data\\{relativePath.Replace('/', '\\')}",
            relativePath,
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        return new TableListItem(descriptor);
    }
}
