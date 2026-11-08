using XmlEditorReloaded.Core.Help;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Abstractions;

/// <summary>
/// Provides table and field help from the generated help data.
/// </summary>
public interface ITableHelpProvider
{
    /// <summary>
    /// Gets help for a table family.
    /// </summary>
    /// <param name="table">Dotted table name, including aliases and backup copies.</param>
    /// <returns>Help for the family, or <see langword="null"/> when the table is not documented.</returns>
    TableHelp? GetTableHelp(TableName table);

    /// <summary>
    /// Gets help for one field.
    /// </summary>
    /// <param name="table">Dotted table name that owns the field.</param>
    /// <param name="fieldPath">
    /// Full slash-separated field path, or a leaf field name when it is unique inside the family.
    /// </param>
    /// <returns>Field help, or <see langword="null"/> when the field is not documented or the leaf name is ambiguous.</returns>
    FieldHelp? GetFieldHelp(TableName table, string fieldPath);

    /// <summary>
    /// Searches table titles, field paths, field descriptions, and source text.
    /// </summary>
    /// <param name="query">Case-insensitive search text.</param>
    /// <param name="limit">Maximum number of results; values below one return no results.</param>
    /// <returns>Ranked results, best match first.</returns>
    IReadOnlyList<HelpSearchResult> Search(string query, int limit);
}
