namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Opens the read-only cross-version comparison window.
/// </summary>
public interface IVersionCompareService
{
    /// <summary>
    /// Shows the comparison window and suggests the current data directory as the baseline.
    /// </summary>
    /// <param name="initialDirectory">
    /// Directory selected in the main window; it may point at <c>TableData</c>, <c>Data-1.13</c>,
    /// or a game root, and is used only when no baseline has been chosen yet.
    /// </param>
    void Show(string? initialDirectory);
}
