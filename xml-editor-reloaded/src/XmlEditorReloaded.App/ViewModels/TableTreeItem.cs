using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One folder or table leaf in the optional table tree.
/// </summary>
public sealed partial class TableTreeItem : ObservableObject
{
    private TableTreeItem(string name, string relativePath, bool isFolder, TableListItem? table)
    {
        Name = name;
        RelativePath = relativePath;
        IsFolder = isFolder;
        Table = table;
    }

    /// <summary>
    /// Creates a folder node.
    /// </summary>
    /// <param name="name">Folder name shown in the tree.</param>
    /// <param name="relativePath">Path relative to the table data directory.</param>
    /// <returns>A new folder node.</returns>
    public static TableTreeItem Folder(string name, string relativePath) =>
        new(name, relativePath, isFolder: true, table: null);

    /// <summary>
    /// Creates a table leaf.
    /// </summary>
    /// <param name="name">File name without the <c>.xml</c> extension.</param>
    /// <param name="relativePath">Path relative to the table data directory.</param>
    /// <param name="table">Table row shown by the list view.</param>
    /// <returns>A new leaf node.</returns>
    public static TableTreeItem Leaf(string name, string relativePath, TableListItem table) =>
        new(name, relativePath, isFolder: false, table: table);

    /// <summary>
    /// Gets the display name: a folder segment or a file name without the extension.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the path relative to the table data directory.
    /// </summary>
    public string RelativePath { get; }

    /// <summary>
    /// Gets the table row for a leaf, or <see langword="null"/> for a folder.
    /// </summary>
    public TableListItem? Table { get; }

    /// <summary>
    /// Gets a value indicating whether this node is a folder.
    /// </summary>
    public bool IsFolder { get; }

    /// <summary>
    /// Gets the parent folder, or <see langword="null"/> for a root node.
    /// </summary>
    public TableTreeItem? Parent { get; internal set; }

    /// <summary>
    /// Gets every child, unfiltered.
    /// </summary>
    public ObservableCollection<TableTreeItem> Children { get; } = new();

    /// <summary>
    /// Gets the children currently visible under the active filter.
    /// </summary>
    public ObservableCollection<TableTreeItem> VisibleChildren { get; } = new();

    /// <summary>
    /// Gets the overlay language summary for a leaf.
    /// </summary>
    public string Languages => Table?.Languages ?? string.Empty;

    /// <summary>
    /// Gets a value indicating whether the node is a table leaf.
    /// </summary>
    public bool IsLeaf => !IsFolder;

    /// <summary>
    /// Gets or sets whether the folder is expanded.
    /// </summary>
    [ObservableProperty]
    private bool _isExpanded;

    /// <summary>
    /// Gets or sets whether the node is selected.
    /// </summary>
    [ObservableProperty]
    private bool _isSelected;
}
