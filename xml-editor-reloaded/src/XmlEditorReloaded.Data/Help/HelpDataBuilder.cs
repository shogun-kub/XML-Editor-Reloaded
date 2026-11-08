using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace XmlEditorReloaded.Data.Help;

/// <summary>
/// Builds <c>help.json</c> from the Markdown table documents.
/// </summary>
public static partial class HelpDataBuilder
{
    private const int SupportedSchema = 1;

    /// <summary>
    /// Builds the serialized help data for every document below a tables directory.
    /// </summary>
    /// <param name="tablesDirectory">Absolute path of the <c>help/tables</c> directory.</param>
    /// <param name="language">Language code written to the generated data.</param>
    /// <returns>Indented JSON terminated by one line feed.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="tablesDirectory"/> is empty.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the tables directory does not exist.</exception>
    /// <exception cref="InvalidDataException">Thrown when a document is malformed or a table name is duplicated.</exception>
    public static string BuildJson(string tablesDirectory, string language = "en")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tablesDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        var tablesRoot = Path.GetFullPath(tablesDirectory);
        if (!Directory.Exists(tablesRoot))
        {
            throw new DirectoryNotFoundException($"The help tables directory '{tablesRoot}' does not exist.");
        }

        var helpRoot = Path.GetFullPath(Path.Combine(tablesRoot, ".."));
        var root = new HelpJsonRoot { Schema = SupportedSchema, Lang = language };
        var files = Directory
            .EnumerateFiles(tablesRoot, "*.md", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.Ordinal);

        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(tablesRoot, file).Replace('\\', '/');
            var documentPath = "tables/" + relativePath;
            var document = ParseDocument(file, documentPath, helpRoot);
            root.Documents[documentPath] = document;
        }

        foreach (var document in root.Documents)
        {
            foreach (var table in document.Value.Tables)
            {
                if (root.Tables.TryGetValue(table, out var existing))
                {
                    throw new InvalidDataException(
                        $"Table '{table}' is documented by both '{existing}' and '{document.Key}'.");
                }

                root.Tables[table] = document.Key;
            }
        }

        return JsonSerializer.Serialize(root, HelpJsonOptions.Value) + "\n";
    }

    private static HelpJsonDocument ParseDocument(string file, string documentPath, string helpRoot)
    {
        var text = File.ReadAllText(file).TrimStart('\uFEFF').Replace("\r\n", "\n").Replace('\r', '\n');
        var lines = text.Split('\n');
        var frontMatter = ParseFrontMatter(lines, documentPath, out var bodyStart);

        var schema = ParseInt(GetScalar(frontMatter, "schema"), documentPath, "schema");
        if (schema != SupportedSchema)
        {
            throw new InvalidDataException(
                $"Document '{documentPath}' declares schema {schema}, but only schema {SupportedSchema} is supported.");
        }

        var title = GetScalar(frontMatter, "title") ?? FindTitle(lines, bodyStart) ?? documentPath;
        var tableIds = GetList(frontMatter, "table_ids");
        if (tableIds.Count == 0)
        {
            throw new InvalidDataException($"Document '{documentPath}' has no table_ids.");
        }

        var sections = ParseSections(lines, bodyStart);
        var fields = ParseFields(sections, documentPath);
        var tags = sections.FirstOrDefault(static section =>
            string.Equals(section.Heading, "Purpose", StringComparison.Ordinal));
        var summary = tags is null ? string.Empty : FirstParagraph(tags.Text);
        var links = ParseLinks(sections, documentPath, helpRoot);

        return new HelpJsonDocument
        {
            Title = NormalizeInline(title),
            Summary = NormalizeInline(summary),
            Root = GetScalar(frontMatter, "root"),
            Record = GetScalar(frontMatter, "record"),
            Key = GetScalar(frontMatter, "key"),
            Tables = tableIds,
            Fields = fields,
            Links = links,
            Sections = sections
                .Where(static section =>
                    !string.Equals(section.Heading, "Contents", StringComparison.Ordinal)
                    && !string.Equals(section.Heading, "Fields", StringComparison.Ordinal))
                .Select(static section => new HelpJsonSection
                {
                    Heading = section.Heading,
                    Anchor = section.Anchor,
                    Text = section.Text,
                })
                .ToList(),
        };
    }

    private static List<Section> ParseSections(string[] lines, int bodyStart)
    {
        var sections = new List<Section>();
        string? heading = null;
        var body = new List<string>();

        void Flush()
        {
            if (heading is null)
            {
                return;
            }

            sections.Add(new Section(heading, Slug(heading), string.Join('\n', body).Trim()));
            body.Clear();
        }

        for (var index = bodyStart; index < lines.Length; index++)
        {
            var line = lines[index];
            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                Flush();
                heading = line[3..].Trim();
                continue;
            }

            if (heading is not null)
            {
                body.Add(line);
            }
        }

        Flush();
        return sections;
    }

    private static SortedDictionary<string, HelpJsonField> ParseFields(
        IReadOnlyList<Section> sections,
        string documentPath)
    {
        var fieldsSection = sections.FirstOrDefault(static section =>
            string.Equals(section.Heading, "Fields", StringComparison.Ordinal));
        if (fieldsSection is null)
        {
            throw new InvalidDataException($"Document '{documentPath}' has no '## Fields' section.");
        }

        var lines = fieldsSection.Text.Split('\n');
        var fields = new SortedDictionary<string, HelpJsonField>(StringComparer.Ordinal);
        var index = 0;
        while (index < lines.Length)
        {
            var line = lines[index];
            if (!line.StartsWith("### ", StringComparison.Ordinal))
            {
                index++;
                continue;
            }

            var rawPath = line[4..].Trim();
            if (rawPath.Length >= 2 && rawPath[0] == '`' && rawPath[^1] == '`')
            {
                rawPath = rawPath[1..^1];
            }

            if (rawPath.Length == 0)
            {
                throw new InvalidDataException($"Document '{documentPath}' has a field heading without a path.");
            }

            var block = new List<string>();
            index++;
            while (index < lines.Length && !lines[index].StartsWith("### ", StringComparison.Ordinal))
            {
                block.Add(lines[index]);
                index++;
            }

            var values = ParseFieldBlock(block);
            if (!values.TryGetValue("type", out var type) || string.IsNullOrWhiteSpace(type))
            {
                throw new InvalidDataException($"Field '{documentPath}#{rawPath}' has no Type bullet.");
            }

            if (!values.TryGetValue("description", out var description)
                || string.IsNullOrWhiteSpace(description))
            {
                throw new InvalidDataException($"Field '{documentPath}#{rawPath}' has no Description bullet.");
            }

            if (!values.TryGetValue("source", out var source) || string.IsNullOrWhiteSpace(source))
            {
                throw new InvalidDataException($"Field '{documentPath}#{rawPath}' has no Source bullet.");
            }

            fields[rawPath] = new HelpJsonField
            {
                Path = rawPath,
                Type = NormalizeInline(type),
                Description = NormalizeInline(description),
                NumericSemantics = Optional(values, "numeric semantics"),
                OnlyUsedWith = Optional(values, "only used with"),
                Values = Optional(values, "values"),
                Default = Optional(values, "default"),
                Source = NormalizeInline(source),
                Confidence = values.TryGetValue("confidence", out var confidence)
                    ? NormalizeInline(confidence)
                    : "unknown",
            };
        }

        if (fields.Count == 0)
        {
            throw new InvalidDataException($"Document '{documentPath}' documents no fields.");
        }

        return fields;
    }

    private static Dictionary<string, string> ParseFieldBlock(IReadOnlyList<string> block)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string? key = null;
        var value = new StringBuilder();

        void Flush()
        {
            if (key is null)
            {
                return;
            }

            values[key] = value.ToString().Trim();
            value.Clear();
        }

        foreach (var line in block)
        {
            var match = FieldBulletRegex().Match(line);
            if (match.Success)
            {
                Flush();
                key = match.Groups["key"].Value.Trim();
                value.Append(match.Groups["value"].Value);
                continue;
            }

            if (key is not null)
            {
                value.Append('\n').Append(line);
            }
        }

        Flush();
        return values;
    }

    private static string? Optional(Dictionary<string, string> values, string key) =>
        values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? NormalizeInline(value)
            : null;

    private static List<HelpJsonLink> ParseLinks(
        IReadOnlyList<Section> sections,
        string documentPath,
        string helpRoot)
    {
        var links = new List<HelpJsonLink>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var documentDirectory = Path.GetDirectoryName(Path.Combine(helpRoot, documentPath))
            ?? helpRoot;

        foreach (var section in sections)
        {
            if (!string.Equals(section.Heading, "Relations", StringComparison.Ordinal)
                && !string.Equals(section.Heading, "Related formulas", StringComparison.Ordinal))
            {
                continue;
            }

            foreach (var line in section.Text.Split('\n'))
            {
                foreach (Match match in LinkRegex().Matches(line))
                {
                    var target = match.Groups["target"].Value.Trim();
                    var hash = target.IndexOf('#');
                    if (hash >= 0)
                    {
                        target = target[..hash];
                    }

                    if (target.Length == 0
                        || target.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                        || target.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                        || target.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var fullPath = Path.GetFullPath(Path.Combine(documentDirectory, target));
                    if (!fullPath.StartsWith(helpRoot, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var relativeTarget = Path.GetRelativePath(helpRoot, fullPath).Replace('\\', '/');
                    if (seen.Add(relativeTarget))
                    {
                        links.Add(new HelpJsonLink
                        {
                            Field = string.Empty,
                            Target = relativeTarget,
                            Kind = "related",
                        });
                    }
                }
            }
        }

        links.Sort(static (left, right) =>
        {
            var byTarget = string.CompareOrdinal(left.Target, right.Target);
            return byTarget != 0 ? byTarget : string.CompareOrdinal(left.Field, right.Field);
        });
        return links;
    }

    private static Dictionary<string, List<string>> ParseFrontMatter(
        string[] lines,
        string documentPath,
        out int bodyStart)
    {
        if (lines.Length == 0 || lines[0].Trim() != "---")
        {
            throw new InvalidDataException($"Document '{documentPath}' has no YAML front matter.");
        }

        var result = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        string? key = null;
        var index = 1;
        for (; index < lines.Length; index++)
        {
            var line = lines[index];
            if (line.Trim() == "---")
            {
                bodyStart = index + 1;
                return result;
            }

            if (line.Length == 0)
            {
                continue;
            }

            if (line.StartsWith("  - ", StringComparison.Ordinal)
                || line.StartsWith("- ", StringComparison.Ordinal))
            {
                if (key is null)
                {
                    throw new InvalidDataException(
                        $"Document '{documentPath}' has a list item outside a front-matter key.");
                }

                result[key].Add(Unquote(line[(line.IndexOf('-') + 1)..].Trim()));
                continue;
            }

            var colon = line.IndexOf(':');
            if (colon <= 0)
            {
                throw new InvalidDataException(
                    $"Document '{documentPath}' has an unsupported front-matter line: {line}");
            }

            key = line[..colon].Trim();
            var value = Unquote(line[(colon + 1)..].Trim());
            result[key] = value.Length == 0 ? [] : [value];
        }

        throw new InvalidDataException($"Document '{documentPath}' has no closing front-matter delimiter.");
    }

    private static string? GetScalar(Dictionary<string, List<string>> frontMatter, string key) =>
        frontMatter.TryGetValue(key, out var values) && values.Count > 0 ? values[0] : null;

    private static List<string> GetList(Dictionary<string, List<string>> frontMatter, string key) =>
        frontMatter.TryGetValue(key, out var values)
            ? values.Where(static value => value.Length > 0).ToList()
            : [];

    private static int ParseInt(string? value, string documentPath, string key)
    {
        if (value is null || !int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
        {
            throw new InvalidDataException(
                $"Document '{documentPath}' has an invalid {key} value '{value}'.");
        }

        return result;
    }

    private static string? FindTitle(string[] lines, int bodyStart)
    {
        for (var index = bodyStart; index < lines.Length; index++)
        {
            if (lines[index].StartsWith("# ", StringComparison.Ordinal))
            {
                return lines[index][2..].Trim();
            }
        }

        return null;
    }

    private static string FirstParagraph(string text)
    {
        var paragraph = new List<string>();
        foreach (var line in text.Split('\n'))
        {
            if (line.Length == 0)
            {
                if (paragraph.Count > 0)
                {
                    break;
                }

                continue;
            }

            paragraph.Add(line);
        }

        return string.Join(' ', paragraph);
    }

    private static string NormalizeInline(string value)
    {
        var withoutLinks = LinkRegex().Replace(value, static match => match.Groups["text"].Value);
        var withoutMarkup = withoutLinks
            .Replace("**", string.Empty, StringComparison.Ordinal)
            .Replace("__", string.Empty, StringComparison.Ordinal)
            .Replace("`", string.Empty, StringComparison.Ordinal)
            .Replace("*", string.Empty, StringComparison.Ordinal);
        return WhitespaceRegex().Replace(withoutMarkup, " ").Trim();
    }

    private static string Slug(string heading)
    {
        var builder = new StringBuilder(heading.Length);
        foreach (var character in heading.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(character) || character == '-')
            {
                builder.Append(character);
            }
            else if (char.IsWhiteSpace(character))
            {
                builder.Append('-');
            }
        }

        return builder.ToString();
    }

    private static string Unquote(string value)
    {
        if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
        {
            return value[1..^1].Replace("\\\"", "\"", StringComparison.Ordinal);
        }

        return value;
    }

    [GeneratedRegex(@"^-\s+\*\*(?<key>[^*]+):\*\*\s*(?<value>.*)$", RegexOptions.CultureInvariant)]
    private static partial Regex FieldBulletRegex();

    [GeneratedRegex(@"\[(?<text>[^\]]*)\]\((?<target>[^)\s]+)(?:\s+""[^""]*"")?\)", RegexOptions.CultureInvariant)]
    private static partial Regex LinkRegex();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhitespaceRegex();

    private sealed record Section(string Heading, string Anchor, string Text);
}
