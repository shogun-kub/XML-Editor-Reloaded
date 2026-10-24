using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One repeated field of the selected item, for example <c>DefaultAttachment</c>. Values are edited as a
/// list of item references and written back to the record as repeated elements in list order.
/// </summary>
public sealed partial class ItemListSectionViewModel : ObservableObject
{
    private readonly ItemWorkspaceViewModel _host;
    private readonly TableName _tableName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemListSectionViewModel"/> class.
    /// </summary>
    /// <param name="host">Item workspace that applies list edits.</param>
    /// <param name="tableName">Table that stores the repeated field.</param>
    /// <param name="fieldName">Case-sensitive XML element name of the repeated field.</param>
    /// <param name="label">Localized section label.</param>
    /// <param name="relation">Optional relation used by the value pickers.</param>
    /// <param name="values">Current values in document order.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public ItemListSectionViewModel(
        ItemWorkspaceViewModel host,
        TableName tableName,
        string fieldName,
        string label,
        RelationColumnData? relation,
        IReadOnlyList<string> values)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentNullException.ThrowIfNull(values);

        _host = host;
        _tableName = tableName;
        FieldName = fieldName;
        Label = label;
        Relation = relation;
        Values = new ObservableCollection<ItemListValueViewModel>(
            values.Select(value => new ItemListValueViewModel(this, value)));
    }

    /// <summary>
    /// Gets the case-sensitive XML element name of the repeated field.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Gets the localized section label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the relation used by value pickers, or <see langword="null"/> when the field has no relation.
    /// </summary>
    public RelationColumnData? Relation { get; }

    /// <summary>
    /// Gets the current values in list order.
    /// </summary>
    public ObservableCollection<ItemListValueViewModel> Values { get; }

    /// <summary>
    /// Gets the add-value button text.
    /// </summary>
    public string AddText => Strings.Get("ItemListAdd");

    /// <summary>
    /// Gets the remove-value button text.
    /// </summary>
    public string RemoveText => Strings.Get("ItemListRemove");

    /// <summary>
    /// Gets the note that only items marked as attachments are offered by the picker.
    /// </summary>
    public string CompatibleNote => Strings.Get("ItemCompatibleOnly");

    /// <summary>
    /// Gets a value indicating whether the list accepts edits.
    /// </summary>
    public bool CanEdit => _host.CanEdit;

    /// <summary>
    /// Adds an empty value and writes the list back. An empty key is written as <c>0</c>, the JA2
    /// convention for "no attachment", because the field is numeric.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void Add()
    {
        Values.Add(new ItemListValueViewModel(this, "0"));
        Apply();
    }

    /// <summary>
    /// Removes one value and writes the list back.
    /// </summary>
    /// <param name="value">Value to remove.</param>
    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void Remove(ItemListValueViewModel? value)
    {
        if (value is null || !Values.Remove(value))
        {
            return;
        }

        Apply();
    }

    internal void Apply() =>
        _host.SetListValues(_tableName, FieldName, Values.Select(static value => value.Value).ToArray());
}

/// <summary>
/// One value of a repeated item field.
/// </summary>
public sealed partial class ItemListValueViewModel : ObservableObject
{
    private readonly ItemListSectionViewModel _section;
    private string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemListValueViewModel"/> class.
    /// </summary>
    /// <param name="section">Section that owns the value.</param>
    /// <param name="value">Raw key text.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public ItemListValueViewModel(ItemListSectionViewModel section, string value)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(value);
        _section = section;
        _value = value;
    }

    /// <summary>
    /// Gets the relation data used by the picker, or <see langword="null"/> when the field has no relation.
    /// </summary>
    public RelationColumnData? Relation => _section.Relation;

    /// <summary>
    /// Gets a value indicating whether the value uses a searchable relation picker.
    /// </summary>
    public bool HasRelation => Relation is { IsAvailable: true };

    /// <summary>
    /// Gets a value indicating whether the value is edited as plain numeric text.
    /// </summary>
    public bool IsPlainValue => !HasRelation;

    /// <summary>
    /// Gets or sets the raw key text. Setting the value writes the whole list back to the document.
    /// </summary>
    public string Value
    {
        get => _value;
        set
        {
            if (!SetProperty(ref _value, value))
            {
                return;
            }

            _section.Apply();
        }
    }
}
