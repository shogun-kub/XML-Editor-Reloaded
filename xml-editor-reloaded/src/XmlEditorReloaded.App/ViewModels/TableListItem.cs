using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// A table row shown in the shell table list.
/// </summary>
public sealed class TableListItem
{
    /// <summary>
    /// Initializes a new item for a table descriptor.
    /// </summary>
    /// <param name="descriptor">Discovered table descriptor.</param>
    public TableListItem(TableDescriptor descriptor)
    {
        Descriptor = descriptor;
    }

    /// <summary>
    /// Gets the underlying table descriptor.
    /// </summary>
    public TableDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the dotted table name.
    /// </summary>
    public string TableName => Descriptor.TableName.Value;

    /// <summary>
    /// Gets a comma-separated list of overlay language codes, or a dash when none exist.
    /// </summary>
    public string Languages => Descriptor.Overlays.Count == 0
        ? "—"
        : string.Join(", ", Descriptor.Overlays.Select(static overlay => overlay.Language.Code));

    /// <summary>
    /// Gets the localized metadata availability label.
    /// </summary>
    public string MappedText => Descriptor.IsMapped ? Strings.Get("Mapped") : Strings.Get("Unmapped");
}
