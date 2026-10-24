namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Opens the standalone item image viewer for a data directory.
/// </summary>
public interface IImageViewerService
{
    /// <summary>
    /// Shows the item image viewer and loads the item list and previews for the selected path.
    /// </summary>
    /// <param name="dataDirectory">
    /// Path selected in the main window; it may point at <c>TableData</c>, <c>Data-1.13</c>, or a game root.
    /// </param>
    void Show(string? dataDirectory);
}
