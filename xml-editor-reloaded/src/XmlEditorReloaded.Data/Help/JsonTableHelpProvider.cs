using System.Text.Json;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Help;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Data.Help;

/// <summary>
/// Provides table and field help from the embedded <c>help.json</c> resource.
/// </summary>
public sealed class JsonTableHelpProvider : ITableHelpProvider
{
    private const string ResourceName = "XmlEditorReloaded.Data.Help.help.json";
    private readonly IReadOnlyDictionary<string, TableHelp> _documents;
    private readonly IReadOnlyDictionary<string, string> _tableDocuments;

    /// <summary>
    /// Initializes a provider from the embedded help data.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the embedded data is missing or invalid.</exception>
    public JsonTableHelpProvider()
        : this(OpenEmbeddedResource())
    {
    }

    /// <summary>
    /// Initializes a provider from a JSON stream.
    /// </summary>
    /// <param name="jsonStream">Stream that contains a <c>help.json</c> document.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="jsonStream"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown when the stream does not contain valid help data.</exception>
    public JsonTableHelpProvider(Stream jsonStream)
    {
        ArgumentNullException.ThrowIfNull(jsonStream);
        var root = JsonSerializer.Deserialize<HelpJsonRoot>(jsonStream, HelpJsonOptions.Value)
            ?? throw new InvalidDataException("The help data is empty.");
        if (root.Schema != 1)
        {
            throw new InvalidDataException($"Unsupported help data schema {root.Schema}; expected 1.");
        }

        var documents = new Dictionary<string, TableHelp>(StringComparer.Ordinal);
        var tableDocuments = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var pair in root.Documents)
        {
            var document = pair.Value;
            var tableNames = document.Tables
                .Select(static name => TableName.Parse(name))
                .ToArray();
            if (tableNames.Length == 0)
            {
                throw new InvalidDataException($"Help document '{pair.Key}' has no table names.");
            }

            var fields = new Dictionary<string, FieldHelp>(StringComparer.Ordinal);
            foreach (var fieldPair in document.Fields)
            {
                var field = fieldPair.Value;
                fields[fieldPair.Key] = new FieldHelp(
                    field.Path,
                    field.Type,
                    field.Description,
                    field.NumericSemantics,
                    field.OnlyUsedWith,
                    field.Values,
                    field.Default,
                    field.Source,
                    field.Confidence);
            }

            var links = document.Links
                .Select(static link => new HelpLink(link.Field, link.Target, link.Kind))
                .ToArray();
            var sections = document.Sections
                .Select(static section => new HelpSection(section.Heading, section.Anchor, section.Text))
                .ToArray();
            var tableHelp = new TableHelp(
                tableNames[0],
                tableNames,
                pair.Key,
                document.Title,
                document.Summary,
                document.Root,
                document.Record,
                document.Key,
                fields,
                links,
                sections);
            documents[pair.Key] = tableHelp;
            foreach (var tableName in tableNames)
            {
                if (tableDocuments.TryGetValue(tableName.Value, out var existing))
                {
                    throw new InvalidDataException(
                        $"Table '{tableName.Value}' is present in both '{existing}' and '{pair.Key}'.");
                }

                tableDocuments[tableName.Value] = pair.Key;
            }
        }

        _documents = documents;
        _tableDocuments = tableDocuments;
        DocumentCount = documents.Count;
        TableCount = tableDocuments.Count;
        UniqueFieldCount = documents.Values.Sum(static document => document.Fields.Count);
    }

    /// <summary>
    /// Gets the number of loaded table-family documents.
    /// </summary>
    public int DocumentCount { get; }

    /// <summary>
    /// Gets the number of distinct dotted table names, including aliases and backups.
    /// </summary>
    public int TableCount { get; }

    /// <summary>
    /// Gets the number of unique field entries across all documents.
    /// </summary>
    /// <remarks>
    /// Table aliases share the same field entries, so this value is lower than the number of
    /// table/field pairs counted by the help coverage report.
    /// </remarks>
    public int UniqueFieldCount { get; }

    /// <inheritdoc />
    public TableHelp? GetTableHelp(TableName table) =>
        _tableDocuments.TryGetValue(table.Value, out var document)
            ? _documents[document]
            : null;

    /// <inheritdoc />
    public FieldHelp? GetFieldHelp(TableName table, string fieldPath)
    {
        if (string.IsNullOrWhiteSpace(fieldPath)
            || !_tableDocuments.TryGetValue(table.Value, out var document)
            || !_documents.TryGetValue(document, out var help))
        {
            return null;
        }

        var normalized = fieldPath.Trim().Trim('`').Replace('\\', '/');
        if (help.Fields.TryGetValue(normalized, out var exact))
        {
            return exact;
        }

        var suffix = "/" + normalized.Trim('/');
        var matches = help.Fields
            .Where(pair => pair.Key.EndsWith(suffix, StringComparison.Ordinal))
            .Select(static pair => pair.Value)
            .Take(2)
            .ToArray();
        if (matches.Length == 1)
        {
            return matches[0];
        }

        var leaf = normalized[(normalized.LastIndexOf('/') + 1)..];
        matches = help.Fields
            .Where(pair => string.Equals(pair.Key, leaf, StringComparison.Ordinal)
                || pair.Key.EndsWith("/" + leaf, StringComparison.Ordinal))
            .Select(static pair => pair.Value)
            .Take(2)
            .ToArray();
        return matches.Length == 1 ? matches[0] : null;
    }

    /// <inheritdoc />
    public IReadOnlyList<HelpSearchResult> Search(string query, int limit)
    {
        if (string.IsNullOrWhiteSpace(query) || limit <= 0)
        {
            return [];
        }

        var needle = query.Trim();
        var results = new List<ScoredResult>();
        foreach (var help in _documents.Values)
        {
            var documentScore = Score(help.Title, needle);
            if (documentScore > 0)
            {
                results.Add(new ScoredResult(
                    documentScore,
                    new HelpSearchResult(help.Table, null, help.Title, Snippet(help.Summary))));
            }

            foreach (var field in help.Fields.Values)
            {
                var score = Math.Max(
                    Score(field.Path, needle),
                    Math.Max(Score(field.Type, needle), Score(field.Description, needle)));
                if (score <= 0)
                {
                    continue;
                }

                results.Add(new ScoredResult(
                    score,
                    new HelpSearchResult(
                        help.Table,
                        field.Path,
                        $"{help.Title} — {field.Path}",
                        Snippet(field.Description))));
            }
        }

        return results
            .OrderByDescending(static result => result.Score)
            .ThenBy(static result => result.Value.Table.Value, StringComparer.Ordinal)
            .ThenBy(static result => result.Value.FieldPath ?? string.Empty, StringComparer.Ordinal)
            .Take(Math.Min(limit, 200))
            .Select(static result => result.Value)
            .ToArray();
    }

    private static Stream OpenEmbeddedResource()
    {
        var stream = typeof(JsonTableHelpProvider).Assembly.GetManifestResourceStream(ResourceName);
        if (stream is null)
        {
            throw new InvalidOperationException(
                $"The embedded help resource '{ResourceName}' was not found. "
                + "Run the help generator and rebuild the application.");
        }

        return stream;
    }

    private static int Score(string text, string needle)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        if (string.Equals(text, needle, StringComparison.OrdinalIgnoreCase))
        {
            return 100;
        }

        if (text.StartsWith(needle, StringComparison.OrdinalIgnoreCase))
        {
            return 80;
        }

        if (text.Contains(needle, StringComparison.OrdinalIgnoreCase))
        {
            return 60;
        }

        return 0;
    }

    private static string Snippet(string text)
    {
        var normalized = string.Join(' ', text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        return normalized.Length <= 180 ? normalized : normalized[..177] + "...";
    }

    private sealed record ScoredResult(int Score, HelpSearchResult Value);
}
