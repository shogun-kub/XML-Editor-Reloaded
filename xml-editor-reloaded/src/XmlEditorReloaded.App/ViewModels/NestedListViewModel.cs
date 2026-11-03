using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One repeated scalar field of a nested form section, for example <c>DRUNK/drugitems/drugitem</c>.
/// Values are edited as an ordered list and written back as repeated siblings in list order.
/// </summary>
public sealed partial class NestedListViewModel : ObservableObject
{
    private readonly Action<IReadOnlyList<string>> _write;

    /// <summary>
    /// Initializes a new instance of the <see cref="NestedListViewModel"/> class.
    /// </summary>
    /// <param name="label">Label shown above the list.</param>
    /// <param name="pathText">Path text used for display and automation identifiers.</param>
    /// <param name="canEdit">Whether the list accepts edits.</param>
    /// <param name="values">Current values in document order.</param>
    /// <param name="write">Writes the whole list back to the document.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public NestedListViewModel(
        string label,
        string pathText,
        bool canEdit,
        IReadOnlyList<string> values,
        Action<IReadOnlyList<string>> write)
    {
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(pathText);
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(write);

        Label = label;
        PathText = pathText;
        CanEdit = canEdit;
        _write = write;
        Values = new ObservableCollection<NestedListValueViewModel>(
            values.Select(value => new NestedListValueViewModel(this, value)));
    }

    /// <summary>
    /// Gets the label shown above the list.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the path text used for display and automation identifiers.
    /// </summary>
    public string PathText { get; }

    /// <summary>
    /// Gets a value indicating whether the list accepts edits.
    /// </summary>
    public bool CanEdit { get; }

    /// <summary>
    /// Gets the current values in list order.
    /// </summary>
    public ObservableCollection<NestedListValueViewModel> Values { get; }

    /// <summary>
    /// Gets the add-value button text.
    /// </summary>
    public string AddText => Strings.Get("NestedEditorListAdd");

    /// <summary>
    /// Gets the remove-value button text.
    /// </summary>
    public string RemoveText => Strings.Get("NestedEditorListRemove");

    /// <summary>
    /// Gets the automation identifier of the add button.
    /// </summary>
    public string AddAutomationId => "NestedListAdd_" + PathText;

    /// <summary>
    /// Gets the automation identifier shared by the value editors.
    /// </summary>
    public string ValueAutomationId => "NestedListValue_" + PathText;

    /// <summary>
    /// Gets the automation identifier shared by the remove buttons.
    /// </summary>
    public string RemoveAutomationId => "NestedListRemove_" + PathText;

    /// <summary>
    /// Adds one empty value and writes the list back.
    /// </summary>
    [RelayCommand]
    private void Add()
    {
        Values.Add(new NestedListValueViewModel(this, "0"));
        Apply();
    }

    /// <summary>
    /// Removes one value and writes the list back.
    /// </summary>
    /// <param name="value">Value to remove.</param>
    [RelayCommand]
    private void Remove(NestedListValueViewModel? value)
    {
        if (value is null || !Values.Remove(value))
        {
            return;
        }

        Apply();
    }

    internal void Apply() => _write(Values.Select(static value => value.Value).ToArray());
}

/// <summary>
/// One value of a repeated nested field.
/// </summary>
public sealed partial class NestedListValueViewModel : ObservableObject
{
    private readonly NestedListViewModel _list;
    private string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="NestedListValueViewModel"/> class.
    /// </summary>
    /// <param name="list">List that owns the value.</param>
    /// <param name="value">Raw value text.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public NestedListValueViewModel(NestedListViewModel list, string value)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(value);
        _list = list;
        _value = value;
    }

    /// <summary>
    /// Gets the command that removes this value.
    /// </summary>
    public IRelayCommand RemoveCommand => _list.RemoveCommand;

    /// <summary>
    /// Gets the remove button text.
    /// </summary>
    public string RemoveText => _list.RemoveText;

    /// <summary>
    /// Gets the automation identifier of this value editor.
    /// </summary>
    public string ValueAutomationId => _list.ValueAutomationId;

    /// <summary>
    /// Gets the automation identifier of this value's remove button.
    /// </summary>
    public string RemoveAutomationId => _list.RemoveAutomationId;

    /// <summary>
    /// Gets or sets the raw value text. Setting the value writes the whole list back to the document.
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

            _list.Apply();
        }
    }
}
