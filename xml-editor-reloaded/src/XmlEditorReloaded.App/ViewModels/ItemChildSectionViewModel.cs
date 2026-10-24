using CommunityToolkit.Mvvm.ComponentModel;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One 1:N child-table section of the item workspace. The section hosts a grid over the related table
/// that is filtered to the rows of the selected item, so adding and deleting rows stays a normal table
/// edit with its own pickers, validation, and shared undo history.
/// </summary>
public sealed partial class ItemChildSectionViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemChildSectionViewModel"/> class.
    /// </summary>
    /// <param name="host">Item workspace that owns the section.</param>
    /// <param name="member">Loaded child table and the relation that links it to the item.</param>
    /// <param name="label">Localized section label.</param>
    /// <param name="editor">Embedded grid editor bound to the shared child workspace.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public ItemChildSectionViewModel(
        ItemWorkspaceViewModel host,
        TableSetMember member,
        string label,
        TableEditorViewModel editor)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(member);
        ArgumentNullException.ThrowIfNull(editor);

        Host = host;
        Member = member;
        Label = label;
        Editor = editor;
    }

    /// <summary>
    /// Gets the item workspace that owns the section.
    /// </summary>
    public ItemWorkspaceViewModel Host { get; }

    /// <summary>
    /// Gets the loaded child table.
    /// </summary>
    public TableSetMember Member { get; }

    /// <summary>
    /// Gets the localized section label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the dotted name of the child table.
    /// </summary>
    public TableName TableName => Member.Relation.TargetTable;

    /// <summary>
    /// Gets the embedded grid editor of the child table.
    /// </summary>
    public TableEditorViewModel Editor { get; }

    /// <summary>
    /// Gets the number of rows that belong to the selected item.
    /// </summary>
    public int RowCount => Editor.Rows.Count;

    /// <summary>
    /// Gets the row count text shown in the section header.
    /// </summary>
    public string CountText => Strings.Format("ItemChildRowCountFormat", RowCount);

    /// <summary>
    /// Gets the add-row button text.
    /// </summary>
    public string AddText => Strings.Get("EditorAddRowText");

    /// <summary>
    /// Gets the delete-row button text.
    /// </summary>
    public string DeleteText => Strings.Get("EditorDeleteRowText");

    /// <summary>
    /// Refreshes the row count shown in the section header.
    /// </summary>
    public void RefreshCount() => OnPropertyChanged(nameof(CountText));
}
