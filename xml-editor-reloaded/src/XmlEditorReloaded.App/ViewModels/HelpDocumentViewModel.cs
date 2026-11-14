using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Help;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Read-only view model for a standalone help page such as the glossary or the table index.
/// </summary>
public sealed class HelpDocumentViewModel
{
    private readonly Action<string>? _openDocument;

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpDocumentViewModel"/> class.
    /// </summary>
    /// <param name="document">Help page to display.</param>
    /// <param name="openDocument">Optional callback that opens a linked help document.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="document"/> is null.</exception>
    public HelpDocumentViewModel(HelpDocument document, Action<string>? openDocument = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        Document = document;
        Title = document.Title;
        Text = document.Text;
        CloseText = Strings.Get("HelpClose");
        _openDocument = openDocument;
    }

    /// <summary>Gets the displayed help page.</summary>
    public HelpDocument Document { get; }

    /// <summary>Gets the window title.</summary>
    public string Title { get; }

    /// <summary>Gets the page body as Markdown text.</summary>
    public string Text { get; }

    /// <summary>Gets the close button text.</summary>
    public string CloseText { get; }

    /// <summary>
    /// Opens a document linked from this page.
    /// </summary>
    /// <param name="target">Document path as written in the page Markdown.</param>
    public void OpenDocument(string target) =>
        _openDocument?.Invoke(HelpPathResolver.Resolve(Document.Path, target));
}
