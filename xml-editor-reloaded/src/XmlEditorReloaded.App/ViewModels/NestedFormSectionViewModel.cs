using CommunityToolkit.Mvvm.ComponentModel;
using XmlEditorReloaded.App.Resources;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One expandable group of the nested-record form, for example a table group, a nested structure, one
/// <c>ASSIGNMENT</c> entry, or one repeated block such as a <c>PRODUCTIONLINE</c>.
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
    /// <param name="lists">Repeated scalar lists shown below the fields.</param>
    /// <param name="blockFieldName">XML field name of the repeated block, or <see langword="null"/>.</param>
    /// <param name="blockIndex">Zero-based occurrence of the repeated block, or <c>-1</c>.</param>
    /// <param name="nestedSegmentIndex">Key-segment index of a nested record, or <c>-1</c>.</param>
    /// <param name="nestedOccurrenceIndex">Zero-based occurrence of a nested record, or <c>-1</c>.</param>
    /// <param name="nestedKey">Resolved nested key, or <see langword="null"/>.</param>
    internal NestedFormSectionViewModel(
        string header,
        string automationId,
        IReadOnlyList<NestedFieldViewModel> fields,
        bool isExpanded = true,
        IReadOnlyList<NestedListViewModel>? lists = null,
        string? blockFieldName = null,
        int blockIndex = -1,
        int nestedSegmentIndex = -1,
        int nestedOccurrenceIndex = -1,
        string? nestedKey = null)
    {
        ArgumentNullException.ThrowIfNull(header);
        ArgumentNullException.ThrowIfNull(automationId);
        ArgumentNullException.ThrowIfNull(fields);
        Header = header;
        AutomationId = automationId;
        Fields = fields;
        Lists = lists ?? Array.Empty<NestedListViewModel>();
        IsExpanded = isExpanded;
        BlockFieldName = blockFieldName;
        BlockIndex = blockIndex;
        NestedSegmentIndex = nestedSegmentIndex;
        NestedOccurrenceIndex = nestedOccurrenceIndex;
        NestedKey = nestedKey;
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
    /// Gets the repeated scalar lists shown below the fields.
    /// </summary>
    public IReadOnlyList<NestedListViewModel> Lists { get; }

    /// <summary>
    /// Gets the XML field name of the repeated block, or <see langword="null"/> when the section is not
    /// a repeated block.
    /// </summary>
    public string? BlockFieldName { get; }

    /// <summary>
    /// Gets the zero-based occurrence of the repeated block, or <c>-1</c>.
    /// </summary>
    public int BlockIndex { get; }

    /// <summary>
    /// Gets the key-segment index of a nested record, or <c>-1</c> when the section is not a nested record.
    /// </summary>
    public int NestedSegmentIndex { get; }

    /// <summary>
    /// Gets the zero-based occurrence of a nested record, or <c>-1</c>.
    /// </summary>
    public int NestedOccurrenceIndex { get; }

    /// <summary>
    /// Gets the resolved nested key, or <see langword="null"/>.
    /// </summary>
    public string? NestedKey { get; }

    /// <summary>
    /// Gets a value indicating whether the section can be removed as a whole.
    /// </summary>
    public bool CanDelete => BlockFieldName is not null || NestedSegmentIndex > 0;

    /// <summary>
    /// Gets the delete button text.
    /// </summary>
    public string DeleteText => Strings.Get("NestedEditorDeleteSection");

    /// <summary>
    /// Gets the automation identifier of the delete button.
    /// </summary>
    public string DeleteAutomationId => "NestedDelete_" + AutomationId;

    /// <summary>
    /// Gets or sets a value indicating whether the section is expanded.
    /// </summary>
    [ObservableProperty]
    private bool _isExpanded;
}
