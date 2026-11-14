using System.Text.RegularExpressions;
using System.IO;

namespace XmlEditorReloaded.UiSmoke;

/// <summary>
/// Scenario headings extracted from <c>MANUAL-SMOKE-CHECKLIST.md</c>, so the report stays in sync with
/// the manual checklist and every scenario has one row even when it is not automated yet.
/// </summary>
internal sealed class ChecklistCatalog
{
    private static readonly Regex HeadingPattern = new(
        @"\*\*(?<id>S\d{2,3})\.\s+(?<title>.*?)\.\*\*",
        RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.Compiled,
        TimeSpan.FromSeconds(2));

    private static readonly Regex TableRowPattern = new(
        @"^\|\s*(?<id>S\d{2,3})\s*\|\s*(?<title>[^|]+?)\s*\|",
        RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Compiled,
        TimeSpan.FromSeconds(2));

    private static readonly Regex WhitespacePattern = new(
        @"\s+",
        RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(2));

    private readonly Dictionary<string, string> _titles;

    private ChecklistCatalog(Dictionary<string, string> titles) => _titles = titles;

    /// <summary>
    /// Parses the scenario headings of the manual checklist.
    /// </summary>
    /// <param name="checklistPath">Absolute path of the checklist.</param>
    /// <returns>The parsed catalog; empty when the file is missing.</returns>
    public static ChecklistCatalog Load(string checklistPath)
    {
        var titles = new Dictionary<string, string>(StringComparer.Ordinal);
        if (!File.Exists(checklistPath))
        {
            return new ChecklistCatalog(titles);
        }

        var text = File.ReadAllText(checklistPath);
        foreach (Match match in HeadingPattern.Matches(text))
        {
            var id = match.Groups["id"].Value;
            var title = WhitespacePattern.Replace(match.Groups["title"].Value, " ").Trim();
            titles.TryAdd(id, title);
        }

        // Scenarios from the M23b sections are listed in summary tables rather than as headings.
        foreach (Match match in TableRowPattern.Matches(text))
        {
            var id = match.Groups["id"].Value;
            var title = Shorten(WhitespacePattern.Replace(match.Groups["title"].Value, " ").Trim());
            titles.TryAdd(id, title);
        }

        return new ChecklistCatalog(titles);
    }

    /// <summary>
    /// Turns a summarizing table sentence into a short scenario title: the part before the first
    /// colon, capped at the length that keeps the report table readable.
    /// </summary>
    /// <param name="title">Full table cell text.</param>
    /// <returns>Short title.</returns>
    private static string Shorten(string title)
    {
        var colon = title.IndexOf(':');
        if (colon > 3)
        {
            title = title[..colon].Trim();
        }

        return title.Length <= 90 ? title : title[..87].TrimEnd() + "…";
    }

    /// <summary>
    /// Gets every parsed scenario identifier in checklist order.
    /// </summary>
    public IReadOnlyList<string> Ids => _titles.Keys.ToList();

    /// <summary>
    /// Returns the title of a scenario, or an empty string when the checklist has no such heading.
    /// </summary>
    /// <param name="id">Scenario identifier, for example <c>S07</c>.</param>
    /// <returns>Scenario title without the identifier and trailing period.</returns>
    public string TitleOf(string id) => _titles.TryGetValue(id, out var title) ? title : string.Empty;
}
