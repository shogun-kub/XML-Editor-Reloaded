using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Localization;
using XmlEditorReloaded.Data.Metadata;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Tests for <see cref="FileSystemTableCatalog"/> and <see cref="TranslationCatalog"/>.
/// </summary>
public class CatalogTests
{
    private static readonly LanguageDescriptor Russian = new("ru", "Russian", "Russian", 1);

    /// <summary>
    /// Overlay files are excluded from the base table list and attached to their base table.
    /// </summary>
    [Fact]
    public async Task Catalog_SeparatesBaseTablesAndOverlays()
    {
        using var workspace = new TestWorkspace();
        var tableData = CreateTableData(workspace);
        var catalog = new FileSystemTableCatalog(new[] { Russian });

        var result = await catalog.GetTablesAsync(tableData);

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(2, result.Data!.Count);
        var items = Assert.Single(result.Data, table => table.TableName.Value == "Items.Items");
        Assert.False(items.IsMapped);
        Assert.Single(items.Overlays);
        Assert.Equal("ru", items.Overlays[0].Language.Code);
        Assert.EndsWith("Russian.Items.xml", items.Overlays[0].OverlayFilePath, StringComparison.Ordinal);

        var lookup = Assert.Single(result.Data, table => table.TableName.Value == "Lookup.ItemClass");
        Assert.Empty(lookup.Overlays);
    }

    /// <summary>
    /// Metadata marks known tables as mapped.
    /// </summary>
    [Fact]
    public async Task Catalog_UsesMetadataForMappedFlag()
    {
        using var workspace = new TestWorkspace();
        var tableData = CreateTableData(workspace);
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var catalog = new FileSystemTableCatalog(new[] { Russian }, new SqliteMetadataProvider(databasePath));

        var result = await catalog.GetTablesAsync(tableData);

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.All(result.Data!, table => Assert.True(table.IsMapped, table.TableName.Value));
    }

    /// <summary>
    /// A missing TableData directory is reported as a file-not-found failure.
    /// </summary>
    [Fact]
    public async Task Catalog_MissingDirectory_ReturnsFileNotFound()
    {
        using var workspace = new TestWorkspace();
        var catalog = new FileSystemTableCatalog(new[] { Russian });

        var result = await catalog.GetTablesAsync(workspace.PathOf("missing"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.IoFileNotFound, result.ErrorCode);
    }

    /// <summary>
    /// TranslationCatalog returns translatable paths and overlay languages for a table.
    /// </summary>
    [Fact]
    public async Task TranslationCatalog_ReturnsFieldsAndLanguages()
    {
        using var workspace = new TestWorkspace();
        var tableData = CreateTableData(workspace);
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(new[] { Russian }, provider);
        var translations = new TranslationCatalog(provider, catalog, tableData);

        var fields = await translations.GetTranslatableFieldsAsync(Core.Metadata.TableName.Parse("Items.Items"));
        Assert.True(fields.IsSuccess, fields.ErrorText);
        Assert.Equal(5, fields.Data!.Count);

        var languages = await translations.GetLanguagesForAsync(Core.Metadata.TableName.Parse("Items.Items"));
        Assert.True(languages.IsSuccess, languages.ErrorText);
        Assert.Equal("ru", Assert.Single(languages.Data!).Code);
    }

    private static string CreateTableData(TestWorkspace workspace)
    {
        var root = workspace.PathOf("Data-1.13/TableData");
        Directory.CreateDirectory(Path.Combine(root, "Items"));
        Directory.CreateDirectory(Path.Combine(root, "Lookup"));
        File.WriteAllText(
            Path.Combine(root, "Items", "Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST><ITEM><uiIndex>0</uiIndex></ITEM></ITEMLIST>");
        File.WriteAllText(
            Path.Combine(root, "Items", "Russian.Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST><ITEM><uiIndex>0</uiIndex><szItemName>Ничто</szItemName></ITEM></ITEMLIST>");
        File.WriteAllText(
            Path.Combine(root, "Lookup", "ItemClass.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data><ItemClass><id>1</id><name>Gun</name></ItemClass></JA2Data>");
        return root;
    }
}
