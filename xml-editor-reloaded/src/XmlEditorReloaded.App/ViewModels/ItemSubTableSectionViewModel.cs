using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One 1:1 sub-table section of the item workspace, for example the weapon or armour record of the
/// selected item. The section shows the fields of the related record and can create the row when the
/// item class expects one but the data file does not contain it yet.
/// </summary>
public sealed partial class ItemSubTableSectionViewModel : ObservableObject
{
    private readonly ItemWorkspaceViewModel _host;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemSubTableSectionViewModel"/> class.
    /// </summary>
    /// <param name="host">Item workspace that applies edits and creates rows.</param>
    /// <param name="member">Loaded related table and the relation that links it to the item.</param>
    /// <param name="label">Localized section label.</param>
    /// <param name="row">Related record of the selected item, or <see langword="null"/> when absent.</param>
    /// <param name="sourceValue">Value of the relation source field on the item, or <see langword="null"/>.</param>
    /// <param name="fields">Editable fields of the related record.</param>
    /// <param name="listSections">Repeated fields of the related record, for example barrel configurations.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public ItemSubTableSectionViewModel(
        ItemWorkspaceViewModel host,
        TableSetMember member,
        string label,
        TableRowViewModel? row,
        string? sourceValue,
        IEnumerable<DetailFieldViewModel> fields,
        IEnumerable<ItemListSectionViewModel>? listSections = null)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(member);

        _host = host;
        Member = member;
        Label = label;
        Row = row;
        SourceValue = sourceValue;
        Fields = new ObservableCollection<DetailFieldViewModel>(fields);
        ListSections = new ObservableCollection<ItemListSectionViewModel>(
            listSections ?? Array.Empty<ItemListSectionViewModel>());
    }

    /// <summary>
    /// Gets the loaded related table.
    /// </summary>
    public TableSetMember Member { get; }

    /// <summary>
    /// Gets the localized section label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the dotted name of the related table.
    /// </summary>
    public TableName TableName => Member.Relation.TargetTable;

    /// <summary>
    /// Gets the related record of the selected item, or <see langword="null"/> when it is absent.
    /// </summary>
    public TableRowViewModel? Row { get; }

    /// <summary>
    /// Gets the value of the relation source field on the item, for example <c>ubClassIndex</c>.
    /// </summary>
    public string? SourceValue { get; }

    /// <summary>
    /// Gets the editable fields of the related record.
    /// </summary>
    public ObservableCollection<DetailFieldViewModel> Fields { get; }

    /// <summary>
    /// Gets the repeated fields of the related record, for example <c>BarrelConfiguration</c>.
    /// </summary>
    public ObservableCollection<ItemListSectionViewModel> ListSections { get; }

    /// <summary>
    /// Gets a value indicating whether the related record exists.
    /// </summary>
    public bool HasRow => Row is not null;

    /// <summary>
    /// Gets a value indicating whether the missing row can be created. A row cannot be created when the
    /// item lacks the source value, for example a placeholder item without <c>ubClassIndex</c>.
    /// </summary>
    public bool CanCreate =>
        !HasRow
        && !string.IsNullOrWhiteSpace(SourceValue)
        && Member.Workspace.CanEdit;

    /// <summary>
    /// Gets the explanation shown when the record is absent.
    /// </summary>
    public string StatusText => HasRow
        ? string.Empty
        : string.IsNullOrWhiteSpace(SourceValue)
            ? Strings.Get("ItemNoClassIndex")
            : Strings.Get("ItemNoSubTableRow");

    /// <summary>
    /// Gets the create-row button text.
    /// </summary>
    public string CreateText => Strings.Get("ItemCreateRow");

    /// <summary>
    /// Gets the localized note shown for class-keyed rows that are shared by several items.
    /// </summary>
    public string SharedNote => string.Equals(
        Member.Relation.SourceField,
        "ubClassIndex",
        StringComparison.Ordinal)
        ? Strings.Get("ItemSharedClassRow")
        : string.Empty;

    /// <summary>
    /// Creates the missing related record.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCreate))]
    private void Create() => _host.CreateSubTableRow(this);
}
