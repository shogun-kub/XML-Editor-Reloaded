using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Folder/file hierarchy of the active table list. The flat list remains the compatibility view; this
/// tree adds navigation by real table paths without changing table discovery.
/// </summary>
public sealed partial class TableTreeViewModel : ObservableObject
{
    [ObservableProperty]
    private string _filterText = string.Empty;

    [ObservableProperty]
    private bool _hasVisibleTables;

    /// <summary>
    /// Gets every root folder and root file, unfiltered.
    /// </summary>
    public ObservableCollection<TableTreeItem> Roots { get; } = new();

    /// <summary>
    /// Gets the roots visible under the active filter.
    /// </summary>
    public ObservableCollection<TableTreeItem> VisibleRoots { get; } = new();

    partial void OnFilterTextChanged(string value) => ApplyFilter(value);

    /// <summary>
    /// Rebuilds the tree from discovered tables and applies the current filter.
    /// </summary>
    /// <param name="tables">Flat table list of the active data set.</param>
    public void Load(IEnumerable<TableListItem> tables)
    {
        Roots.Clear();
        VisibleRoots.Clear();
        var folders = new Dictionary<string, TableTreeItem>(StringComparer.OrdinalIgnoreCase);
        foreach (var table in tables)
        {
            var relative = table.Descriptor.RelativePath.Replace('\\', '/');
            var segments = relative.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
            {
                continue;
            }

            TableTreeItem? parent = null;
            var path = string.Empty;
            for (var index = 0; index < segments.Length - 1; index++)
            {
                path = path.Length == 0 ? segments[index] : $"{path}/{segments[index]}";
                if (!folders.TryGetValue(path, out var folder))
                {
                    folder = TableTreeItem.Folder(segments[index], path);
                    folder.Parent = parent;
                    folders[path] = folder;
                    (parent?.Children ?? Roots).Add(folder);
                }

                parent = folder;
            }

            var leaf = TableTreeItem.Leaf(
                Path.GetFileNameWithoutExtension(segments[^1]),
                relative,
                table);
            leaf.Parent = parent;
            (parent?.Children ?? Roots).Add(leaf);
        }

        Sort(Roots);
        RestoreVisible(Roots);
        ApplyFilter(FilterText);
    }

    /// <summary>
    /// Clears the text filter and restores every tree node.
    /// </summary>
    public void ClearFilter() => FilterText = string.Empty;

    /// <summary>
    /// Expands every folder.
    /// </summary>
    public void ExpandAll() => SetExpanded(Roots, expanded: true);

    /// <summary>
    /// Collapses every folder.
    /// </summary>
    public void CollapseAll() => SetExpanded(Roots, expanded: false);

    /// <summary>
    /// Selects a table leaf in the full tree, expanding its ancestors. The filter is not changed.
    /// </summary>
    /// <param name="table">Table to select, or <see langword="null"/> to clear selection.</param>
    /// <returns>The selected leaf, or <see langword="null"/> when the table is absent.</returns>
    public TableTreeItem? Select(TableListItem? table)
    {
        ClearSelection(Roots);
        if (table is null)
        {
            return null;
        }

        var leaf = FindLeaf(Roots, table);
        if (leaf is null)
        {
            return null;
        }

        leaf.IsSelected = true;
        for (var parent = leaf.Parent; parent is not null; parent = parent.Parent)
        {
            parent.IsExpanded = true;
        }

        return leaf;
    }

    /// <summary>
    /// Clears the filter, selects the table, and expands its ancestors. This is the "Find in tree"
    /// operation.
    /// </summary>
    /// <param name="table">Table to reveal.</param>
    /// <returns>The selected leaf, or <see langword="null"/> when the table is absent.</returns>
    public TableTreeItem? FindInTree(TableListItem? table)
    {
        ClearFilter();
        return Select(table);
    }

    /// <summary>
    /// Returns the number of visible table leaves, used by tests and diagnostics.
    /// </summary>
    public int VisibleLeafCount => CountLeaves(VisibleRoots);

    private void ApplyFilter(string filter)
    {
        var text = filter.Trim();
        if (text.Length == 0)
        {
            RestoreVisible(Roots);
            SetVisibleRoots(Roots);
            return;
        }

        FilterNodes(Roots, text);
        SetVisibleRoots(Roots.Where(root => root.IsLeaf
            ? Matches(root, text)
            : root.VisibleChildren.Count > 0));
    }

    private void SetVisibleRoots(IEnumerable<TableTreeItem> roots)
    {
        VisibleRoots.Clear();
        foreach (var root in roots)
        {
            VisibleRoots.Add(root);
        }

        HasVisibleTables = VisibleRoots.Count > 0;
    }

    private static bool FilterNodes(IEnumerable<TableTreeItem> nodes, string filter)
    {
        var any = false;
        foreach (var node in nodes)
        {
            if (node.IsLeaf)
            {
                var match = Matches(node, filter);
                node.VisibleChildren.Clear();
                any |= match;
                continue;
            }

            var childMatch = FilterNodes(node.Children, filter);
            var selfMatch = node.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase)
                || node.RelativePath.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
            if (selfMatch)
            {
                RestoreVisible(node.Children);
                childMatch = true;
            }

            node.VisibleChildren.Clear();
            if (selfMatch)
            {
                foreach (var child in node.Children)
                {
                    node.VisibleChildren.Add(child);
                }
            }
            else
            {
                foreach (var child in node.Children.Where(child =>
                             child.IsLeaf
                                 ? Matches(child, filter)
                                 : child.VisibleChildren.Count > 0))
                {
                    node.VisibleChildren.Add(child);
                }
            }

            if (childMatch)
            {
                node.IsExpanded = true;
            }

            any |= childMatch;
        }

        return any;
    }

    private static bool Matches(TableTreeItem node, string filter) =>
        node.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase)
        || Path.GetFileName(node.RelativePath)
            .Contains(filter, StringComparison.CurrentCultureIgnoreCase)
        || node.RelativePath.Contains(filter, StringComparison.CurrentCultureIgnoreCase)
        || (node.Table?.TableName.Contains(filter, StringComparison.CurrentCultureIgnoreCase) ?? false);

    private static void RestoreVisible(IEnumerable<TableTreeItem> nodes)
    {
        foreach (var node in nodes)
        {
            node.VisibleChildren.Clear();
            foreach (var child in node.Children)
            {
                node.VisibleChildren.Add(child);
            }

            RestoreVisible(node.Children);
        }
    }

    private static void Sort(ObservableCollection<TableTreeItem> items)
    {
        var ordered = items
            .OrderByDescending(item => item.IsFolder)
            .ThenBy(item => item.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
        items.Clear();
        foreach (var item in ordered)
        {
            items.Add(item);
            Sort(item.Children);
        }
    }

    private static void SetExpanded(IEnumerable<TableTreeItem> nodes, bool expanded)
    {
        foreach (var node in nodes)
        {
            if (node.IsFolder)
            {
                node.IsExpanded = expanded;
            }

            SetExpanded(node.Children, expanded);
        }
    }

    private static void ClearSelection(IEnumerable<TableTreeItem> nodes)
    {
        foreach (var node in nodes)
        {
            node.IsSelected = false;
            ClearSelection(node.Children);
        }
    }

    private static TableTreeItem? FindLeaf(
        IEnumerable<TableTreeItem> nodes,
        TableListItem table)
    {
        foreach (var node in nodes)
        {
            if (ReferenceEquals(node.Table, table))
            {
                return node;
            }

            var found = FindLeaf(node.Children, table);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private static int CountLeaves(IEnumerable<TableTreeItem> nodes)
    {
        var count = 0;
        foreach (var node in nodes)
        {
            count += node.IsLeaf ? 1 : CountLeaves(node.VisibleChildren);
        }

        return count;
    }
}
