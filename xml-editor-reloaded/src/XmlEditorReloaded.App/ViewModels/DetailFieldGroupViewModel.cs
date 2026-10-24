using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One collapsible group of fields in the detail form, ordered by the schema group hints.
/// </summary>
public sealed partial class DetailFieldGroupViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DetailFieldGroupViewModel"/> class.
    /// </summary>
    /// <param name="key">Stable group key, for example <c>attachments</c>.</param>
    /// <param name="label">Localized group label.</param>
    /// <param name="order">Group display order; smaller values are shown first.</param>
    /// <param name="isExpanded">Initial expansion state.</param>
    /// <param name="fields">Fields that belong to the group, in schema order.</param>
    public DetailFieldGroupViewModel(
        string key,
        string label,
        int order,
        bool isExpanded,
        IEnumerable<DetailFieldViewModel> fields)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentNullException.ThrowIfNull(fields);

        Key = key;
        Label = label;
        Order = order;
        _isExpanded = isExpanded;
        Fields = new ObservableCollection<DetailFieldViewModel>(fields);
    }

    /// <summary>
    /// Gets the stable group key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the localized group label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the group display order.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Gets the fields of the group in schema order.
    /// </summary>
    public ObservableCollection<DetailFieldViewModel> Fields { get; }

    /// <summary>
    /// Gets the localized field count shown in the group header.
    /// </summary>
    public string FieldCountText => Resources.Strings.Format("DetailGroupCountFormat", Fields.Count);

    /// <summary>
    /// Gets or sets a value indicating whether the group is expanded.
    /// </summary>
    [ObservableProperty]
    private bool _isExpanded;
}
