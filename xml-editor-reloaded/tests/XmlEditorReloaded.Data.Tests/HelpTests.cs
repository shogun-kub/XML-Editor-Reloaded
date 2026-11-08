using System.Text;
using XmlEditorReloaded.Core.Help;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Help;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Tests for the Markdown help generator, the embedded help data, and the help provider.
/// </summary>
public class HelpTests
{
    /// <summary>
    /// The generator reads front matter, sections, fields, and links, and the provider resolves aliases and leaf paths.
    /// </summary>
    [Fact]
    public void Generator_ParsesDocumentAndProviderResolvesAliases()
    {
        using var workspace = new TestWorkspace();
        var tables = workspace.PathOf("help/tables");
        Directory.CreateDirectory(tables);
        File.WriteAllText(Path.Combine(tables, "Sample.md"), FixtureDocument, new UTF8Encoding(false));
        File.WriteAllText(
            workspace.PathOf("help/GLOSSARY.md"),
            "# Glossary\n\n| English | Russian |\n|---|---|\n| field | поле |\n",
            new UTF8Encoding(false));

        var json = HelpDataBuilder.BuildJson(tables);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var provider = new JsonTableHelpProvider(stream);

        Assert.Equal(1, provider.DocumentCount);
        Assert.Equal(1, provider.TableCount);
        Assert.Equal(1, provider.PageCount);
        Assert.Equal(2, provider.UniqueFieldCount);

        var glossary = Assert.IsType<HelpDocument>(provider.GetDocument("GLOSSARY.md"));
        Assert.Equal("Glossary", glossary.Title);
        Assert.Contains("field", glossary.Text, StringComparison.Ordinal);
        Assert.Null(provider.GetDocument("missing.md"));

        var table = Assert.IsType<TableHelp>(provider.GetTableHelp(TableName.Parse("Test.Sample")));
        Assert.Equal("Test - sample", table.Title);
        Assert.Equal("A short purpose paragraph.", table.Summary);
        Assert.Equal("TESTLIST", table.Root);
        Assert.Equal("TEST", table.Record);
        Assert.Equal("uiIndex", table.Key);
        Assert.Equal("tables/Sample.md", table.DocumentPath);
        Assert.Equal(2, table.Fields.Count);
        Assert.Equal("tables/Other.md", Assert.Single(table.Links).Target);

        var field = Assert.IsType<FieldHelp>(provider.GetFieldHelp(TableName.Parse("Test.Sample"), "uiIndex"));
        Assert.Equal("Integer (UINT16)", field.Type);
        Assert.Equal("0..10.", field.Values);
        Assert.Equal("source", field.Confidence);

        var nested = Assert.IsType<FieldHelp>(provider.GetFieldHelp(
            TableName.Parse("Test.Sample"), "TESTLIST/TEST/flag"));
        Assert.Equal("Option.", nested.OnlyUsedWith);
        Assert.Equal("1 means on.", nested.NumericSemantics);
        Assert.Null(provider.GetFieldHelp(TableName.Parse("Test.Sample"), "missing"));
    }

    /// <summary>
    /// A duplicated table name is rejected instead of silently overwriting a document.
    /// </summary>
    [Fact]
    public void Generator_RejectsDuplicateTableName()
    {
        using var workspace = new TestWorkspace();
        var tables = workspace.PathOf("help/tables");
        Directory.CreateDirectory(tables);
        File.WriteAllText(Path.Combine(tables, "One.md"), FixtureDocument, new UTF8Encoding(false));
        File.WriteAllText(Path.Combine(tables, "Two.md"), FixtureDocument, new UTF8Encoding(false));

        var exception = Assert.Throws<InvalidDataException>(() => HelpDataBuilder.BuildJson(tables));
        Assert.Contains("Test.Sample", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// The committed data covers every family and resolves the catalog's dotted names.
    /// </summary>
    [Fact]
    public void EmbeddedData_CoversEveryFamily()
    {
        var provider = new JsonTableHelpProvider();

        Assert.Equal(94, provider.DocumentCount);
        Assert.Equal(239, provider.TableCount);
        Assert.Equal(1614, provider.UniqueFieldCount);
        Assert.Equal(8, provider.PageCount);

        var glossary = Assert.IsType<HelpDocument>(provider.GetDocument("GLOSSARY.md"));
        Assert.Equal("Glossary", glossary.Title);
        Assert.Contains("approved Russian translations", glossary.Text, StringComparison.Ordinal);
        var indexPage = Assert.IsType<HelpDocument>(provider.GetDocument("INDEX.md"));
        Assert.Equal("Table help index", indexPage.Title);
        Assert.Null(provider.GetDocument("missing.md"));

        var weapons = Assert.IsType<TableHelp>(provider.GetTableHelp(TableName.Parse("Items.Weapons")));
        Assert.Equal(48, weapons.Fields.Count);
        Assert.Equal("WEAPONLIST", weapons.Root);
        var index = Assert.IsType<FieldHelp>(provider.GetFieldHelp(TableName.Parse("Items.Weapons"), "uiIndex"));
        Assert.Equal("Integer (UINT32)", index.Type);

        var lookup = Assert.IsType<TableHelp>(provider.GetTableHelp(TableName.Parse("Lookup.AmmoChoices")));
        Assert.Equal(2, lookup.Fields.Count);
        Assert.NotNull(provider.GetFieldHelp(TableName.Parse("Lookup.AmmoChoices"), "id"));

        Assert.NotNull(provider.GetTableHelp(TableName.Parse("Inventory.Backup original Gun Choices.GunChoices_Enemy_Admin")));
        Assert.Null(provider.GetTableHelp(TableName.Parse("Missing.Table")));
    }

    /// <summary>
    /// Search is case-insensitive and returns field-level and document-level matches.
    /// </summary>
    [Fact]
    public void Search_FindsFieldDescriptions()
    {
        var provider = new JsonTableHelpProvider();

        var results = provider.Search("recoil", 10);

        Assert.NotEmpty(results);
        Assert.Contains(results, result =>
            result.Table.Value == "Items.Weapons"
            && result.FieldPath is not null
            && result.FieldPath.Contains("Recoil", StringComparison.Ordinal));
        Assert.Empty(provider.Search("   ", 10));
        Assert.Empty(provider.Search("recoil", 0));
    }

    private const string FixtureDocument = """
        ---
        schema: 1
        lang: en
        family: test
        title: "Test - sample"
        tables:
          - Test
        table_ids:
          - Test.Sample
        files:
          - Test/Sample.xml
        root: TESTLIST
        record: TEST
        key: uiIndex
        sources:
          - fixture
        status: reviewed
        updated: 2026-11-08
        ---

        # Test - sample

        ## Contents

        - [Purpose](#purpose)
        - [Fields](#fields)

        ## Purpose

        A short purpose paragraph.

        Second paragraph is not the summary.

        ## Fields

        ### `TESTLIST/TEST/uiIndex`

        - **Type:** `Integer` (`UINT16`)
        - **Description:** Index of the test record.
        - **Values:** `0..10`.
        - **Default:** `0`.
        - **Source:** game code.
        - **Confidence:** `source`

        ### `TESTLIST/TEST/flag`

        - **Type:** `Boolean` (`BOOLEAN`)
        - **Description:** A gated flag.
        - **Only used with:** Option.
        - **Numeric semantics:** `1` means on.
        - **Default:** `0`.
        - **Source:** game code.
        - **Confidence:** `source`

        ## Relations

        - [Other](Other.md) `uiIndex` points to another table.
        """;
}
