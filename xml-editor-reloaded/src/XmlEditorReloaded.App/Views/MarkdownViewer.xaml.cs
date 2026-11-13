using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using XmlEditorReloaded.App.Services;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Read-only Markdown viewer used by the help window and its standalone pages.
/// </summary>
public partial class MarkdownViewer : UserControl
{
    /// <summary>Identifies the <see cref="Markdown"/> dependency property.</summary>
    public static readonly DependencyProperty MarkdownProperty = DependencyProperty.Register(
        nameof(Markdown),
        typeof(string),
        typeof(MarkdownViewer),
        new PropertyMetadata(string.Empty, OnMarkdownChanged));

    /// <summary>Identifies the <see cref="Title"/> dependency property.</summary>
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(MarkdownViewer),
        new PropertyMetadata(null, OnMarkdownChanged));

    /// <summary>Identifies the <see cref="ShowScrollBars"/> dependency property.</summary>
    public static readonly DependencyProperty ShowScrollBarsProperty = DependencyProperty.Register(
        nameof(ShowScrollBars),
        typeof(bool),
        typeof(MarkdownViewer),
        new PropertyMetadata(true, OnShowScrollBarsChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownViewer"/> class.
    /// </summary>
    public MarkdownViewer()
    {
        InitializeComponent();
        RenderDocument();
    }

    /// <summary>
    /// Occurs when the user activates a link in the rendered Markdown.
    /// </summary>
    public event EventHandler<string>? LinkRequested;

    /// <summary>Gets or sets the Markdown source to display.</summary>
    public string Markdown
    {
        get => (string)GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    /// <summary>Gets or sets the page title used to suppress a duplicated leading heading.</summary>
    public string? Title
    {
        get => (string?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the inner viewer shows scroll bars.</summary>
    public bool ShowScrollBars
    {
        get => (bool)GetValue(ShowScrollBarsProperty);
        set => SetValue(ShowScrollBarsProperty, value);
    }

    /// <summary>Gets the rendered document, or <see langword="null"/> before initialization.</summary>
    public FlowDocument? Document => Viewer?.Document;

    private static void OnMarkdownChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) =>
        ((MarkdownViewer)dependencyObject).RenderDocument();

    private static void OnShowScrollBarsChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        var viewer = (MarkdownViewer)dependencyObject;
        viewer.Viewer.VerticalScrollBarVisibility =
            viewer.ShowScrollBars ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
        viewer.Viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    }

    private void RenderDocument()
    {
        if (Viewer is null)
        {
            return;
        }

        var document = MarkdownFlowDocumentRenderer.Render(Markdown ?? string.Empty, Title);
        foreach (var link in MarkdownFlowDocumentRenderer.EnumerateLinks(document))
        {
            link.Click += OnLinkClick;
        }

        Viewer.Document = document;
        OnShowScrollBarsChanged(this, default);
    }

    private void OnLinkClick(object sender, RoutedEventArgs e)
    {
        if (sender is Hyperlink { Tag: string target } && !string.IsNullOrWhiteSpace(target))
        {
            if (target.StartsWith('#'))
            {
                NavigateToAnchor(target[1..]);
                return;
            }

            LinkRequested?.Invoke(this, target);
        }
    }

    private void NavigateToAnchor(string anchor)
    {
        if (Viewer.Document is not { } document)
        {
            return;
        }

        foreach (var block in document.Blocks)
        {
            if (FindParagraph(block, anchor) is { } paragraph)
            {
                paragraph.BringIntoView();
                return;
            }
        }
    }

    private static Paragraph? FindParagraph(Block block, string anchor)
    {
        switch (block)
        {
            case Paragraph paragraph when string.Equals(
                AutomationProperties.GetAutomationId(paragraph),
                anchor,
                StringComparison.Ordinal):
                return paragraph;
            case Paragraph:
                return null;
            case Section section:
                foreach (var child in section.Blocks)
                {
                    if (FindParagraph(child, anchor) is { } paragraph)
                    {
                        return paragraph;
                    }
                }

                return null;
            case System.Windows.Documents.List list:
                foreach (var item in list.ListItems)
                {
                    foreach (var child in item.Blocks)
                    {
                        if (FindParagraph(child, anchor) is { } paragraph)
                        {
                            return paragraph;
                        }
                    }
                }

                return null;
            case Table table:
                foreach (var rowGroup in table.RowGroups)
                {
                    foreach (var row in rowGroup.Rows)
                    {
                        foreach (var cell in row.Cells)
                        {
                            foreach (var child in cell.Blocks)
                            {
                                if (FindParagraph(child, anchor) is { } paragraph)
                                {
                                    return paragraph;
                                }
                            }
                        }
                    }
                }

                return null;
            default:
                return null;
        }
    }
}
