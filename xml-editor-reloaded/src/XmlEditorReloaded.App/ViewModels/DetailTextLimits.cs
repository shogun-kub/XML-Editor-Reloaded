using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Resolves the input length limit of a detail field from the metadata of the workspace that hosts it.
/// </summary>
internal static class DetailTextLimits
{
    /// <summary>
    /// Finds the limit that applies to a field in the language of the workspace overlay.
    /// </summary>
    /// <param name="workspace">Workspace whose metadata and overlay language are used.</param>
    /// <param name="xmlFieldName">Case-sensitive XML element name of the field.</param>
    /// <returns>The applicable limit, or <see langword="null"/> when the field is unlimited.</returns>
    public static ColumnTextLimit? Resolve(TableWorkspace? workspace, string xmlFieldName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xmlFieldName);
        return workspace?.Metadata?.TryGetTextLimit(xmlFieldName, workspace.Language?.Code, out var limit) == true
            ? limit
            : null;
    }
}
