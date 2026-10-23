using XmlEditorReloaded.Core.Localization;

namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Describes one table file discovered in a data directory, together with overlay and metadata availability.
/// </summary>
/// <param name="TableName">Dotted table name derived from the file path.</param>
/// <param name="FilePath">Absolute path of the base table file.</param>
/// <param name="RelativePath">Path of the file relative to the <c>TableData</c> directory, using forward slashes.</param>
/// <param name="IsMapped">Whether metadata exists for this table; unmapped tables still load document-driven.</param>
/// <param name="Overlays">Language overlays found next to the base file.</param>
public sealed record TableDescriptor(
    TableName TableName,
    string FilePath,
    string RelativePath,
    bool IsMapped,
    IReadOnlyList<OverlayDescriptor> Overlays)
{
    /// <summary>
    /// Gets the set of language codes that have an overlay for this table.
    /// </summary>
    public IReadOnlyList<string> OverlayLanguages => Overlays.Select(static o => o.Language.Code).ToArray();
}
