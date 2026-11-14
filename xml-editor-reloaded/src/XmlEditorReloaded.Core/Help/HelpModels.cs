using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.Core.Help;

/// <summary>
/// Help for one table family, loaded from one Markdown document.
/// </summary>
/// <param name="Table">Canonical dotted name of the first table documented by the family.</param>
/// <param name="Tables">Every dotted table name documented by the family, including aliases and backup copies.</param>
/// <param name="DocumentPath">Document path relative to the help root, for example <c>tables/Items/Weapons.md</c>.</param>
/// <param name="Title">Document title from the front matter.</param>
/// <param name="Summary">Short plain-text summary used in lists and tooltips.</param>
/// <param name="Root">Root XML element of the table family, when declared.</param>
/// <param name="Record">Repeated record element of the table family, when declared.</param>
/// <param name="Key">Record key field of the table family, when declared.</param>
/// <param name="Fields">Field help keyed by the full field path.</param>
/// <param name="Links">Cross-document links collected from the document.</param>
/// <param name="Sections">Document sections other than the field reference.</param>
public sealed record TableHelp(
    TableName Table,
    IReadOnlyList<TableName> Tables,
    string DocumentPath,
    string Title,
    string Summary,
    string? Root,
    string? Record,
    string? Key,
    IReadOnlyDictionary<string, FieldHelp> Fields,
    IReadOnlyList<HelpLink> Links,
    IReadOnlyList<HelpSection> Sections);

/// <summary>
/// Help for one field path inside a table family.
/// </summary>
/// <param name="Path">Full slash-separated path, for example <c>WEAPONLIST/WEAPON/uiIndex</c>.</param>
/// <param name="Type">Declared field type, for example <c>Integer (UINT32)</c>.</param>
/// <param name="Description">What the field means and how the game uses it.</param>
/// <param name="NumericSemantics">Interpretation of larger, smaller, and negative numeric values, when documented.</param>
/// <param name="OnlyUsedWith">Option, table, or record condition that gates the field, when documented.</param>
/// <param name="Values">Accepted values, ranges, enumerations, or references, when documented.</param>
/// <param name="Default">Default or omitted-value behaviour, when documented.</param>
/// <param name="Source">Evidence source for the description.</param>
/// <param name="Confidence">Confidence label: <c>source</c>, <c>inferred</c>, <c>comment</c>, or <c>unknown</c>.</param>
public sealed record FieldHelp(
    string Path,
    string Type,
    string Description,
    string? NumericSemantics,
    string? OnlyUsedWith,
    string? Values,
    string? Default,
    string Source,
    string Confidence);

/// <summary>
/// A link from a help document to a related document or table.
/// </summary>
/// <param name="Field">Field path named by the link, or an empty string when the link is document-level.</param>
/// <param name="Target">Target document path relative to the help root.</param>
/// <param name="Kind">Link kind, for example <c>related</c>.</param>
public sealed record HelpLink(string Field, string Target, string Kind);

/// <summary>
/// One prose section of a help document, excluding the field reference.
/// </summary>
/// <param name="Heading">Section heading as written in the document.</param>
/// <param name="Anchor">GitHub-style anchor of the heading.</param>
/// <param name="Text">Section body in Markdown.</param>
public sealed record HelpSection(string Heading, string Anchor, string Text);

/// <summary>
/// One standalone help page, such as the glossary or the table index.
/// </summary>
/// <param name="Path">Page path relative to the help root, for example <c>GLOSSARY.md</c>.</param>
/// <param name="Title">Page title from its first level-one heading.</param>
/// <param name="Text">Page body as Markdown.</param>
public sealed record HelpDocument(string Path, string Title, string Text);

/// <summary>
/// One result of a help search.
/// </summary>
/// <param name="Table">Table family that owns the result.</param>
/// <param name="FieldPath">Matching field path, or <see langword="null"/> for a document-level match.</param>
/// <param name="Title">Display title of the result.</param>
/// <param name="Snippet">Short plain-text excerpt that explains the match.</param>
public sealed record HelpSearchResult(
    TableName Table,
    string? FieldPath,
    string Title,
    string Snippet);
