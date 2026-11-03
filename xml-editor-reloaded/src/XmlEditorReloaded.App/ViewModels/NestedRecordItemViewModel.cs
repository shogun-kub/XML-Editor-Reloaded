using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One entry of the record list in a nested-record workspace.
/// </summary>
public sealed class NestedRecordItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NestedRecordItemViewModel"/> class.
    /// </summary>
    /// <param name="row">Workspace row of the record.</param>
    /// <param name="displayName">Short user-facing name.</param>
    internal NestedRecordItemViewModel(WorkspaceRow row, string displayName)
    {
        ArgumentNullException.ThrowIfNull(row);
        Row = row;
        DisplayName = displayName;
    }

    /// <summary>
    /// Gets the primary key, or <see langword="null"/> when the record has no key.
    /// </summary>
    public string? Key => Row.Key;

    /// <summary>
    /// Gets the key formatted for messages and tooltips.
    /// </summary>
    public string KeyDisplay => Row.KeyDisplay;

    /// <summary>
    /// Gets the workspace row of the record.
    /// </summary>
    public WorkspaceRow Row { get; }

    /// <summary>
    /// Gets the short user-facing name shown in the list.
    /// </summary>
    public string DisplayName { get; }
}
