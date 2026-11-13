using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Renders the Markdown subset used by the standalone help pages as a read-only <see cref="FlowDocument"/>.
/// Headings, paragraphs, bullet and numbered lists, fenced code, block quotes, horizontal rules, tables,
/// bold/italic text, inline code and links are supported so the glossary and table index are readable in
/// the help window instead of appearing as raw Markdown source.
/// </summary>
public static partial class MarkdownFlowDocumentRenderer
{
    private static readonly Regex HeadingPattern = HeadingRegex();
    private static readonly Regex NumberedItemPattern = NumberedItemRegex();
    private static readonly Regex TableSeparatorPattern = TableSeparatorRegex();
    private static readonly Regex HorizontalRulePattern = HorizontalRuleRegex();

    /// <summary>
    /// Renders a help page as a <see cref="FlowDocument"/>.
    /// </summary>
    /// <param name="markdown">Markdown body of the page.</param>
    /// <param name="title">Page title, when the leading level-one heading duplicates it.</param>
    /// <returns>A read-only document ready for a <see cref="System.Windows.Controls.FlowDocumentScrollViewer"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="markdown"/> is null.</exception>
    public static FlowDocument Render(string markdown, string? title = null)
    {
        ArgumentNullException.ThrowIfNull(markdown);

        var document = new FlowDocument
        {
            FontFamily = new FontFamily("Segoe UI"),
            FontSize = 13,
            Foreground = ThemeBrush("Theme.Text", Brushes.Black),
            PagePadding = new Thickness(16),
            ColumnWidth = double.PositiveInfinity,
        };

        var lines = markdown.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');
        var index = SkipLeadingTitle(lines, title);

        while (index < lines.Length)
        {
            if (string.IsNullOrWhiteSpace(lines[index]))
            {
                index++;
                continue;
            }

            if (IsFence(lines[index]))
            {
                index = AddCodeBlock(document, lines, index);
                continue;
            }

            if (TryHeading(lines[index], out var level, out var heading))
            {
                AddHeading(document, heading, level);
                index++;
                continue;
            }

            if (TryTable(lines, index, out var table))
            {
                AddTable(document, table);
                index = table.NextIndex;
                continue;
            }

            if (HorizontalRulePattern.IsMatch(lines[index]))
            {
                AddHorizontalRule(document);
                index++;
                continue;
            }

            if (TryListItem(lines[index], out var bullet))
            {
                index = AddList(document, lines, index, bullet ? TextMarkerStyle.Disc : TextMarkerStyle.Decimal);
                continue;
            }

            if (lines[index].TrimStart().StartsWith('>'))
            {
                index = AddBlockQuote(document, lines, index);
                continue;
            }

            index = AddParagraph(document, lines, index);
        }

        return document;
    }

    private static int SkipLeadingTitle(IReadOnlyList<string> lines, string? title)
    {
        var index = 0;
        while (index < lines.Count && string.IsNullOrWhiteSpace(lines[index]))
        {
            index++;
        }

        if (index >= lines.Count
            || !TryHeading(lines[index], out var level, out var heading)
            || level != 1
            || string.IsNullOrWhiteSpace(title))
        {
            return 0;
        }

        return string.Equals(StripInlineMarkers(heading), title.Trim(), StringComparison.OrdinalIgnoreCase)
            ? index + 1
            : 0;
    }

    private static void AddHeading(FlowDocument document, string text, int level)
    {
        var paragraph = new Paragraph
        {
            FontSize = level switch
            {
                1 => 22,
                2 => 17,
                _ => 14,
            },
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, level == 1 ? 0 : 14, 0, 7),
            KeepWithNext = true,
        };
        AddInlines(paragraph.Inlines, text);
        document.Blocks.Add(paragraph);
    }

    private static int AddParagraph(FlowDocument document, IReadOnlyList<string> lines, int start)
    {
        var text = new StringBuilder();
        var index = start;
        while (index < lines.Count
            && !string.IsNullOrWhiteSpace(lines[index])
            && !StartsBlock(lines, index))
        {
            if (text.Length > 0)
            {
                text.Append(' ');
            }

            text.Append(lines[index].Trim());
            index++;
        }

        var paragraph = new Paragraph
        {
            Margin = new Thickness(0, 0, 0, 9),
            TextAlignment = TextAlignment.Left,
        };
        AddInlines(paragraph.Inlines, text.ToString());
        document.Blocks.Add(paragraph);
        return Math.Max(index, start + 1);
    }

    private static bool StartsBlock(IReadOnlyList<string> lines, int index) =>
        IsFence(lines[index])
        || TryHeading(lines[index], out _, out _)
        || HorizontalRulePattern.IsMatch(lines[index])
        || TryListItem(lines[index], out _)
        || lines[index].TrimStart().StartsWith('>')
        || (lines[index].Contains('|') && TryTable(lines, index, out _));

    private static int AddList(FlowDocument document, IReadOnlyList<string> lines, int start, TextMarkerStyle markerStyle)
    {
        var list = new System.Windows.Documents.List
        {
            MarkerStyle = markerStyle,
            Margin = new Thickness(0, 0, 0, 9),
            Padding = new Thickness(20, 0, 0, 0),
        };
        var index = start;
        var bullet = markerStyle == TextMarkerStyle.Disc;
        while (index < lines.Count && TryListItem(lines[index], out var isBullet) && isBullet == bullet)
        {
            var text = new StringBuilder(RemoveListMarker(lines[index]));
            index++;
            while (index < lines.Count
                && !string.IsNullOrWhiteSpace(lines[index])
                && IsIndentedContinuation(lines[index])
                && !TryListItem(lines[index], out _))
            {
                text.Append(' ').Append(lines[index].Trim());
                index++;
            }

            var paragraph = new Paragraph { Margin = new Thickness(0) };
            AddInlines(paragraph.Inlines, text.ToString());
            list.ListItems.Add(new ListItem(paragraph));
        }

        document.Blocks.Add(list);
        return index;
    }

    private static int AddBlockQuote(FlowDocument document, IReadOnlyList<string> lines, int start)
    {
        var text = new StringBuilder();
        var index = start;
        while (index < lines.Count && lines[index].TrimStart().StartsWith('>'))
        {
            var line = lines[index].TrimStart();
            if (text.Length > 0)
            {
                text.Append(' ');
            }

            text.Append(line.Length > 1 ? line[1..].Trim() : string.Empty);
            index++;
        }

        var paragraph = new Paragraph
        {
            FontStyle = FontStyles.Italic,
            Foreground = ThemeBrush("Theme.Text.Muted", Brushes.DimGray),
            Margin = new Thickness(14, 0, 0, 9),
        };
        AddInlines(paragraph.Inlines, text.ToString());
        document.Blocks.Add(paragraph);
        return index;
    }

    private static int AddCodeBlock(FlowDocument document, IReadOnlyList<string> lines, int start)
    {
        var index = start + 1;
        var paragraph = new Paragraph
        {
            FontFamily = new FontFamily("Consolas"),
            FontSize = 12,
            Background = ThemeBrush("Theme.Grid.Header", Brushes.WhiteSmoke),
            BorderBrush = ThemeBrush("Theme.Border", Brushes.LightGray),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(9, 7, 9, 7),
            Margin = new Thickness(0, 0, 0, 10),
        };
        var first = true;
        while (index < lines.Count && !IsFence(lines[index]))
        {
            if (!first)
            {
                paragraph.Inlines.Add(new LineBreak());
            }

            paragraph.Inlines.Add(new Run(lines[index]));
            first = false;
            index++;
        }

        if (index < lines.Count)
        {
            index++;
        }

        document.Blocks.Add(paragraph);
        return index;
    }

    private static void AddHorizontalRule(FlowDocument document) =>
        document.Blocks.Add(new Paragraph
        {
            BorderBrush = ThemeBrush("Theme.Border", Brushes.LightGray),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Margin = new Thickness(0, 4, 0, 12),
        });

    private static bool TryTable(IReadOnlyList<string> lines, int start, out ParsedTable table)
    {
        table = default;
        if (start + 1 >= lines.Count
            || !lines[start].Contains('|')
            || !IsTableSeparator(lines[start + 1]))
        {
            return false;
        }

        var header = SplitTableRow(lines[start]);
        var separator = SplitTableRow(lines[start + 1]);
        if (header.Count == 0 || header.Count != separator.Count)
        {
            return false;
        }

        var rows = new List<IReadOnlyList<string>>();
        var index = start + 2;
        while (index < lines.Count
            && !string.IsNullOrWhiteSpace(lines[index])
            && lines[index].Contains('|'))
        {
            var cells = SplitTableRow(lines[index]);
            if (cells.Count == 0)
            {
                break;
            }

            rows.Add(cells);
            index++;
        }

        table = new ParsedTable(header, rows, index);
        return true;
    }

    private static void AddTable(FlowDocument document, ParsedTable parsed)
    {
        var columnCount = parsed.Header.Count;
        var table = new Table
        {
            CellSpacing = 0,
            BorderBrush = ThemeBrush("Theme.Border", Brushes.LightGray),
            BorderThickness = new Thickness(1),
            Margin = new Thickness(0, 2, 0, 12),
        };

        for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
        {
            table.Columns.Add(new TableColumn { Width = TableColumnWidth(columnCount, columnIndex) });
        }

        var rowGroup = new TableRowGroup();
        table.RowGroups.Add(rowGroup);
        rowGroup.Rows.Add(CreateTableRow(parsed.Header, isHeader: true));
        foreach (var row in parsed.Rows)
        {
            rowGroup.Rows.Add(CreateTableRow(row, isHeader: false));
        }

        document.Blocks.Add(table);
    }

    private static TableRow CreateTableRow(IReadOnlyList<string> cells, bool isHeader)
    {
        var row = new TableRow();
        for (var index = 0; index < cells.Count; index++)
        {
            var paragraph = new Paragraph
            {
                Margin = new Thickness(0),
                FontWeight = isHeader ? FontWeights.SemiBold : FontWeights.Normal,
            };
            AddInlines(paragraph.Inlines, cells[index]);
            row.Cells.Add(new TableCell(paragraph)
            {
                Padding = new Thickness(7, 4, 7, 4),
                BorderBrush = ThemeBrush("Theme.Border", Brushes.LightGray),
                BorderThickness = new Thickness(0, 0, 1, 1),
                Background = isHeader
                    ? ThemeBrush("Theme.Grid.Header", Brushes.WhiteSmoke)
                    : Brushes.Transparent,
            });
        }

        return row;
    }

    private static GridLength TableColumnWidth(int columnCount, int columnIndex) =>
        columnCount switch
        {
            2 => new GridLength(columnIndex == 0 ? 1.2 : 1, GridUnitType.Star),
            3 => new GridLength(columnIndex switch { 0 => 1.1, 1 => 1.1, _ => 1.8 }, GridUnitType.Star),
            _ => new GridLength(columnIndex switch { 0 => 1.1, 1 => 1.2, 2 => 1.8, _ => 0.8 }, GridUnitType.Star),
        };

    private static void AddInlines(InlineCollection target, string text)
    {
        var plain = new StringBuilder();
        var index = 0;

        void FlushPlain()
        {
            if (plain.Length == 0)
            {
                return;
            }

            target.Add(new Run(plain.ToString()));
            plain.Clear();
        }

        while (index < text.Length)
        {
            if (text[index] == '\\' && index + 1 < text.Length)
            {
                plain.Append(text[index + 1]);
                index += 2;
                continue;
            }

            if (index + 1 < text.Length && text[index] == '*' && text[index + 1] == '*')
            {
                var end = text.IndexOf("**", index + 2, StringComparison.Ordinal);
                if (end > index + 2)
                {
                    FlushPlain();
                    var bold = new Bold();
                    AddInlines(bold.Inlines, text[(index + 2)..end]);
                    target.Add(bold);
                    index = end + 2;
                    continue;
                }
            }

            if (text[index] == '`')
            {
                var end = text.IndexOf('`', index + 1);
                if (end > index + 1)
                {
                    FlushPlain();
                    target.Add(new Run(text[(index + 1)..end])
                    {
                        FontFamily = new FontFamily("Consolas"),
                        Background = ThemeBrush("Theme.Grid.Header", Brushes.WhiteSmoke),
                    });
                    index = end + 1;
                    continue;
                }
            }

            if (text[index] == '*' && index + 1 < text.Length)
            {
                var end = text.IndexOf('*', index + 1);
                if (end > index + 1)
                {
                    FlushPlain();
                    var italic = new Italic();
                    AddInlines(italic.Inlines, text[(index + 1)..end]);
                    target.Add(italic);
                    index = end + 1;
                    continue;
                }
            }

            if (text[index] == '[')
            {
                var labelEnd = text.IndexOf(']', index + 1);
                if (labelEnd > index + 1
                    && labelEnd + 1 < text.Length
                    && text[labelEnd + 1] == '(')
                {
                    var targetEnd = text.IndexOf(')', labelEnd + 2);
                    if (targetEnd > labelEnd + 2)
                    {
                        FlushPlain();
                        var link = new Hyperlink
                        {
                            Foreground = ThemeBrush("Theme.Accent", Brushes.SteelBlue),
                            TextDecorations = TextDecorations.Underline,
                            ToolTip = text[(labelEnd + 2)..targetEnd],
                        };
                        AddInlines(link.Inlines, text[(index + 1)..labelEnd]);
                        target.Add(link);
                        index = targetEnd + 1;
                        continue;
                    }
                }
            }

            plain.Append(text[index]);
            index++;
        }

        FlushPlain();
    }

    private static bool TryHeading(string line, out int level, out string text)
    {
        var match = HeadingPattern.Match(line);
        if (!match.Success)
        {
            level = 0;
            text = string.Empty;
            return false;
        }

        level = match.Groups[1].Length;
        text = match.Groups[2].Value.Trim().TrimEnd('#').Trim();
        return true;
    }

    private static bool TryListItem(string line, out bool bullet)
    {
        var trimmed = line.TrimStart();
        if (trimmed.StartsWith("- ", StringComparison.Ordinal)
            || trimmed.StartsWith("* ", StringComparison.Ordinal))
        {
            bullet = true;
            return true;
        }

        if (NumberedItemPattern.IsMatch(trimmed))
        {
            bullet = false;
            return true;
        }

        bullet = false;
        return false;
    }

    private static string RemoveListMarker(string line)
    {
        var trimmed = line.TrimStart();
        if (trimmed.StartsWith("- ", StringComparison.Ordinal)
            || trimmed.StartsWith("* ", StringComparison.Ordinal))
        {
            return trimmed[2..].Trim();
        }

        return NumberedItemPattern.Replace(trimmed, string.Empty, 1).Trim();
    }

    private static bool IsIndentedContinuation(string line) =>
        line.Length > 0 && (line[0] == ' ' || line[0] == '\t');

    private static bool IsFence(string line) =>
        line.TrimStart().StartsWith("```", StringComparison.Ordinal);

    private static bool IsTableSeparator(string line) =>
        line.Contains('|')
        && SplitTableRow(line).All(static cell => TableSeparatorPattern.IsMatch(cell));

    private static IReadOnlyList<string> SplitTableRow(string line)
    {
        var trimmed = line.Trim();
        if (trimmed.StartsWith('|'))
        {
            trimmed = trimmed[1..];
        }

        if (trimmed.EndsWith('|'))
        {
            trimmed = trimmed[..^1];
        }

        var cells = trimmed.Split('|').Select(static cell => cell.Trim()).ToList();
        return cells.All(static cell => cell.Length > 0) ? cells : [];
    }

    private static string StripInlineMarkers(string text) =>
        text.Replace("**", string.Empty, StringComparison.Ordinal)
            .Replace("`", string.Empty, StringComparison.Ordinal)
            .Replace("*", string.Empty, StringComparison.Ordinal)
            .Trim();

    private static Brush ThemeBrush(string key, Brush fallback) =>
        Application.Current?.TryFindResource(key) as Brush ?? fallback;

    [GeneratedRegex(@"^(#{1,6})\s+(.+?)\s*$")]
    private static partial Regex HeadingRegex();

    [GeneratedRegex(@"^\d+\.\s+")]
    private static partial Regex NumberedItemRegex();

    [GeneratedRegex(@"^:?-{3,}:?$")]
    private static partial Regex TableSeparatorRegex();

    [GeneratedRegex(@"^\s*([-*_])\1{2,}\s*$")]
    private static partial Regex HorizontalRuleRegex();

    private readonly record struct ParsedTable(
        IReadOnlyList<string> Header,
        IReadOnlyList<IReadOnlyList<string>> Rows,
        int NextIndex);
}
