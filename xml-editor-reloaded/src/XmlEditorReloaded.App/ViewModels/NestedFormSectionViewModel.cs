using CommunityToolkit.Mvvm.ComponentModel;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One expandable group of the nested-record form, for example a table group, a nested structure, or
/// one <c>ASSIGNMENT</c> entry.
/// </summary>
public sealed partial class NestedFormSectionViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NestedFormSectionViewModel"/> class.
    /// </summary>
    /// <param name="header">Localized section header.</param>
    /// <param name="automationId">Stable automation identifier of the group box.</param>
    /// <param name="fields">Fields shown in the section.</param>
    /// <param name="isExpanded">Whether the section starts expanded.</param>
    internal NestedFormSectionViewModel(
        string header,
        string automationId,
        IReadOnlyList<NestedFieldViewModel> fields,
        bool isExpanded = true)
    {
        ArgumentNullException.ThrowIfNull(header);
        ArgumentNullException.ThrowIfNull(automationId);
        ArgumentNullException.ThrowIfNull(fields);
        Header = header;
        AutomationId = automationId;
        Fields = fields;
        IsExpanded = isExpanded;
    }

    /// <summary>
    /// Gets the localized section header.
    /// </summary>
    public string Header { get; }

    /// <summary>
    /// Gets the stable automation identifier of the group box.
    /// </summary>
    public string AutomationId { get; }

    /// <summary>
    /// Gets the fields shown in the section.
    /// </summary>
    public IReadOnlyList<NestedFieldViewModel> Fields { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the section is expanded.
    /// </summary>
    [ObservableProperty]
    private bool _isExpanded;
}
