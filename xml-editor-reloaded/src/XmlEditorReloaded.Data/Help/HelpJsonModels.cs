using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XmlEditorReloaded.Data.Help;

/// <summary>
/// Root object of the generated <c>help.json</c> file.
/// </summary>
internal sealed class HelpJsonRoot
{
    /// <summary>Gets or sets the schema version.</summary>
    public int Schema { get; set; }

    /// <summary>Gets or sets the document language code.</summary>
    public string Lang { get; set; } = "en";

    /// <summary>Gets or sets documents keyed by their path relative to the help root.</summary>
    public SortedDictionary<string, HelpJsonDocument> Documents { get; set; } =
        new(StringComparer.Ordinal);

    /// <summary>Gets or sets table aliases mapped to document paths.</summary>
    public SortedDictionary<string, string> Tables { get; set; } = new(StringComparer.Ordinal);

    /// <summary>Gets or sets standalone pages keyed by their path relative to the help root.</summary>
    public SortedDictionary<string, HelpJsonPage> Pages { get; set; } = new(StringComparer.Ordinal);
}

/// <summary>
/// One standalone help page in <c>help.json</c>.
/// </summary>
internal sealed class HelpJsonPage
{
    /// <summary>Gets or sets the page title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the page body as Markdown.</summary>
    public string Text { get; set; } = string.Empty;
}

/// <summary>
/// One table-family document in <c>help.json</c>.
/// </summary>
internal sealed class HelpJsonDocument
{
    /// <summary>Gets or sets the document title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the short plain-text summary.</summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>Gets or sets the root XML element.</summary>
    public string? Root { get; set; }

    /// <summary>Gets or sets the record XML element.</summary>
    public string? Record { get; set; }

    /// <summary>Gets or sets the key field name.</summary>
    public string? Key { get; set; }

    /// <summary>Gets or sets the dotted table names documented by this family.</summary>
    public List<string> Tables { get; set; } = [];

    /// <summary>Gets or sets the field help keyed by full field path.</summary>
    public SortedDictionary<string, HelpJsonField> Fields { get; set; } = new(StringComparer.Ordinal);

    /// <summary>Gets or sets cross-document links.</summary>
    public List<HelpJsonLink> Links { get; set; } = [];

    /// <summary>Gets or sets document sections other than the field reference.</summary>
    public List<HelpJsonSection> Sections { get; set; } = [];
}

/// <summary>
/// One field entry in <c>help.json</c>.
/// </summary>
internal sealed class HelpJsonField
{
    /// <summary>Gets or sets the full field path.</summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>Gets or sets the declared type.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets numeric semantics, when documented.</summary>
    public string? NumericSemantics { get; set; }

    /// <summary>Gets or sets the option or condition that gates the field, when documented.</summary>
    public string? OnlyUsedWith { get; set; }

    /// <summary>Gets or sets accepted values, when documented.</summary>
    public string? Values { get; set; }

    /// <summary>Gets or sets default behaviour, when documented.</summary>
    public string? Default { get; set; }

    /// <summary>Gets or sets the evidence source.</summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>Gets or sets the confidence label.</summary>
    public string Confidence { get; set; } = "unknown";
}

/// <summary>
/// One cross-document link in <c>help.json</c>.
/// </summary>
internal sealed class HelpJsonLink
{
    /// <summary>Gets or sets the field path, or an empty string for a document-level link.</summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>Gets or sets the target path relative to the help root.</summary>
    public string Target { get; set; } = string.Empty;

    /// <summary>Gets or sets the link kind.</summary>
    public string Kind { get; set; } = "related";
}

/// <summary>
/// One prose section in <c>help.json</c>.
/// </summary>
internal sealed class HelpJsonSection
{
    /// <summary>Gets or sets the heading text.</summary>
    public string Heading { get; set; } = string.Empty;

    /// <summary>Gets or sets the heading anchor.</summary>
    public string Anchor { get; set; } = string.Empty;

    /// <summary>Gets or sets the section body in Markdown.</summary>
    public string Text { get; set; } = string.Empty;
}

/// <summary>
/// Shared JSON options for generating and reading <c>help.json</c>.
/// </summary>
internal static class HelpJsonOptions
{
    /// <summary>Gets the options used by the generator and the provider.</summary>
    public static JsonSerializerOptions Value { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NewLine = "\n",
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
}
