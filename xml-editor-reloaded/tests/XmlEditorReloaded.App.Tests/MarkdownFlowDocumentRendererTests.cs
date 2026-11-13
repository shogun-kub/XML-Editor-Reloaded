using System.Windows;
using System.Windows.Documents;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Help;
using XmlEditorReloaded.Data.Help;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for rendering the standalone help pages as a formatted document instead of raw Markdown.
/// </summary>
public class MarkdownFlowDocumentRendererTests
{
    /// <summary>
    /// A level-one heading that duplicates the window title is omitted, while the glossary table and
    /// inline formatting are rendered as document elements.
    /// </summary>
    [Fact]
    public void Render_Glossary_RendersTableAndInlineFormatting()
    {
        const string Markdown = """
            # Glossary

            Canonical **terms** and `code` spans.

            | English | Russian |
            |---|---|
            | field | поле |
            | table | таблица |
            """;

        var document = MarkdownFlowDocumentRenderer.Render(Markdown, "Glossary");

        Assert.DoesNotContain(
            document.Blocks.OfType<Paragraph>(),
            paragraph => paragraph.FontSize == 22);
        var table = Assert.Single(document.Blocks.OfType<Table>());
        Assert.Equal(2, table.Columns.Count);
        Assert.Equal(3, table.RowGroups[0].Rows.Count);
        Assert.Contains("field", CellText(table, row: 1, column: 0), StringComparison.Ordinal);
        Assert.Contains("поле", CellText(table, row: 1, column: 1), StringComparison.Ordinal);
        Assert.Contains(
            document.Blocks.OfType<Paragraph>().SelectMany(static paragraph => paragraph.Inlines.OfType<Bold>()),
            bold => TextOf(bold).Contains("terms", StringComparison.Ordinal));
        Assert.Contains(
            document.Blocks.OfType<Paragraph>().SelectMany(static paragraph => paragraph.Inlines.OfType<Run>()),
            run => run.FontFamily.Source == "Consolas" && run.Text == "code");
    }

    /// <summary>
    /// The generated index is a four-column table; links remain readable labels rather than raw
    /// Markdown syntax.
    /// </summary>
    [Fact]
    public void Render_Index_RendersFourColumnTableAndLinks()
    {
        const string Markdown = """
            # Table help index

            | Table | File | Document | Status |
            |---|---|---|---|
            | `AmmoStrings` | `AmmoStrings.xml` | [tables/AmmoStrings.md](tables/AmmoStrings.md) | documented |
            """;

        var document = MarkdownFlowDocumentRenderer.Render(Markdown, "Table help index");

        var table = Assert.Single(document.Blocks.OfType<Table>());
        Assert.Equal(4, table.Columns.Count);
        Assert.Equal(2, table.RowGroups[0].Rows.Count);
        Assert.Contains("AmmoStrings.md", CellText(table, row: 1, column: 2), StringComparison.Ordinal);
        Assert.DoesNotContain("[tables/", CellText(table, row: 1, column: 2), StringComparison.Ordinal);
        Assert.Contains(
            table.RowGroups[0].Rows[1].Cells[2].Blocks.OfType<Paragraph>()
                .SelectMany(static paragraph => paragraph.Inlines.OfType<Hyperlink>()),
            link => TextOf(link).Contains("AmmoStrings.md", StringComparison.Ordinal));
    }

    /// <summary>
    /// Lists, code blocks, and paragraphs survive the block-level pass.
    /// </summary>
    [Fact]
    public void Render_Blocks_RendersListsAndCode()
    {
        const string Markdown = """
            ## Contents

            - First item
            - Second item with **bold**

            ```text
            one
            two
            ```
            """;

        var document = MarkdownFlowDocumentRenderer.Render(Markdown);

        Assert.Contains(document.Blocks.OfType<Paragraph>(), paragraph => paragraph.FontSize == 17);
        var list = Assert.Single(document.Blocks.OfType<System.Windows.Documents.List>());
        Assert.Equal(2, list.ListItems.Count);
        Assert.Contains(
            document.Blocks.OfType<Paragraph>(),
            paragraph => paragraph.FontFamily.Source == "Consolas"
                && TextOf(paragraph).Contains("one", StringComparison.Ordinal)
                && TextOf(paragraph).Contains("two", StringComparison.Ordinal));
    }

    /// <summary>
    /// The embedded glossary and index, which are the pages opened from the Help menu, become tables
    /// with their real number of rows rather than a single raw-text block.
    /// </summary>
    [Fact]
    public void Render_EmbeddedGlossaryAndIndex_AreTables()
    {
        var provider = new JsonTableHelpProvider();

        foreach (var (path, expectedRows) in new[]
        {
            ("GLOSSARY.md", 58),
            ("INDEX.md", 240),
        })
        {
            var page = Assert.IsType<HelpDocument>(provider.GetDocument(path));
            var document = MarkdownFlowDocumentRenderer.Render(page.Text, page.Title);
            var table = Assert.Single(document.Blocks.OfType<Table>());
            Assert.Equal(expectedRows, table.RowGroups[0].Rows.Count);
            Assert.True(table.Columns.Count >= 2);
        }
    }

    private static string CellText(Table table, int row, int column) =>
        TextOf(table.RowGroups[0].Rows[row].Cells[column]);

    private static string TextOf(TextElement element) =>
        new TextRange(element.ContentStart, element.ContentEnd).Text;
}
