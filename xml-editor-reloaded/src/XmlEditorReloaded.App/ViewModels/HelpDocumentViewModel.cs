using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Help;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Read-only view model for a standalone help page such as the glossary or the table index.
/// </summary>
public sealed class HelpDocumentViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpDocumentViewModel"/> class.
    /// </summary>
    /// <param name="document">Help page to display.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
    public HelpDocumentViewModel(HelpDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        Document = document;
        Title = document.Title;
        Text = document.Text;
        CloseText = Strings.Get("HelpClose");
    }

    /// <summary>Gets the displayed help page.</summary>
    public HelpDocument Document { get; }

    /// <summary>Gets the window title.</summary>
    public string Title { get; }

    /// <summary>Gets the page body as Markdown text.</summary>
    public string Text { get; }

    /// <summary>Gets the close button text.</summary>
    public string CloseText { get; }
}
