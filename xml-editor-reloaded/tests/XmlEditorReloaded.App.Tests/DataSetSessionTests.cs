using System.IO;
using System.Xml.Linq;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Repository;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the runtime data-set session and its table workspaces.
/// </summary>
public class DataSetSessionTests
{
    /// <summary>
    /// Opening a data set resolves its directories and binds lookup and translation services.
    /// </summary>
    [Fact]
    public async Task Open_ResolvesDirectoriesAndServices()
    {
        using var workspace = new TestWorkspace();
        var (factory, data13, tableData) = await CreateDataSetAsync(workspace);

        var opened = await factory.OpenAsync(data13);

        Assert.True(opened.IsSuccess, opened.ErrorText);
        var session = opened.Data!;
        Assert.Equal(tableData, session.TableDataDirectory);
        Assert.Equal(data13, session.GameDataDirectory);

        var entries = await session.Lookups.GetEntriesAsync(TableName.Parse("Lookup.ItemClass"));
        Assert.True(entries.IsSuccess, entries.ErrorText);
        Assert.Equal("Gun", Assert.Single(entries.Data!).Name);

        var languages = await session.Translations.GetLanguagesForAsync(TableName.Parse("Items.Items"));
        Assert.True(languages.IsSuccess, languages.ErrorText);
        Assert.Equal("ru", Assert.Single(languages.Data!).Code);

        var reopened = await factory.OpenAsync(data13);
        Assert.Same(session, reopened.Data);
    }

    /// <summary>
    /// Opening a table with a language produces an overlay-aware workspace and saves translations there.
    /// </summary>
    [Fact]
    public async Task OpenTable_WithLanguage_RoutesTranslationToOverlay()
    {
        using var workspace = new TestWorkspace();
        var (factory, data13, tableData) = await CreateDataSetAsync(workspace);
        var session = (await factory.OpenAsync(data13)).Data!;
        var tables = await session.Catalog.GetTablesAsync(tableData);
        var descriptor = Assert.Single(tables.Data!, table => table.TableName.Value == "Items.Items");
        Assert.Single(descriptor.Overlays);

        var opened = await session.OpenTableAsync(descriptor, "ru");

        Assert.True(opened.IsSuccess, opened.ErrorText);
        var table = opened.Data!;
        Assert.Equal("ru", table.Language!.Code);
        Assert.Equal("Глок 17", Assert.Single(table.GetRows(), row => row.Key == "1").GetValue("szItemName"));
        var basePath = descriptor.FilePath;
        var baseBefore = await File.ReadAllBytesAsync(basePath);

        Assert.True(table.SetValue("1", "szItemName", "Глок 18").IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);

        Assert.Equal(baseBefore, await File.ReadAllBytesAsync(basePath));
        var overlay = XDocument.Load(descriptor.Overlays[0].OverlayFilePath);
        var item = Assert.Single(overlay.Root!.Elements("ITEM"), element => element.Element("uiIndex")?.Value == "1");
        Assert.Equal("Глок 18", item.Element("szItemName")?.Value);
    }

    /// <summary>
    /// An unknown language code is reported before any file is opened.
    /// </summary>
    [Fact]
    public async Task OpenTable_UnknownLanguage_ReturnsOverlayNotFound()
    {
        using var workspace = new TestWorkspace();
        var (factory, data13, tableData) = await CreateDataSetAsync(workspace);
        var session = (await factory.OpenAsync(data13)).Data!;
        var tables = await session.Catalog.GetTablesAsync(tableData);
        var descriptor = Assert.Single(tables.Data!, table => table.TableName.Value == "Items.Items");

        var opened = await session.OpenTableAsync(descriptor, "de");

        Assert.False(opened.IsSuccess);
        Assert.Equal(ErrorCodes.OverlayNotFound, opened.ErrorCode);
    }

    /// <summary>
    /// An unmapped table opens in document-driven mode with no edit session.
    /// </summary>
    [Fact]
    public async Task OpenTable_UnmappedTable_UsesDocumentDrivenSchema()
    {
        using var workspace = new TestWorkspace();
        var (factory, data13, tableData) = await CreateDataSetAsync(workspace);
        var notesDirectory = Path.Combine(tableData, "Custom");
        Directory.CreateDirectory(notesDirectory);
        await File.WriteAllTextAsync(
            Path.Combine(notesDirectory, "Notes.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><NOTES><NOTE><id>1</id><text>Hello</text></NOTE></NOTES>");
        var session = (await factory.OpenAsync(data13)).Data!;
        var tables = await session.Catalog.GetTablesAsync(tableData);
        var descriptor = Assert.Single(tables.Data!, table => table.TableName.Value == "Custom.Notes");
        Assert.False(descriptor.IsMapped);

        var opened = await session.OpenTableAsync(descriptor);

        Assert.True(opened.IsSuccess, opened.ErrorText);
        Assert.True(opened.Data!.Schema.IsDocumentDriven);
        Assert.Null(opened.Data.Session);
        Assert.Single(opened.Data.GetRows());
    }

    /// <summary>
    /// A descriptor marked as mapped but absent from the metadata database falls back to document-driven mode.
    /// </summary>
    [Fact]
    public async Task OpenTable_MappedDescriptorWithoutMetadata_FallsBack()
    {
        using var workspace = new TestWorkspace();
        var (factory, data13, tableData) = await CreateDataSetAsync(workspace);
        var path = Path.Combine(tableData, "Custom", "Extra.xml");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><EXTRA><ROW><id>1</id></ROW></EXTRA>");
        var session = (await factory.OpenAsync(data13)).Data!;
        var descriptor = new TableDescriptor(
            TableName.Parse("Custom.Extra"),
            path,
            "Custom/Extra.xml",
            IsMapped: true,
            Array.Empty<XmlEditorReloaded.Core.Localization.OverlayDescriptor>());

        var opened = await session.OpenTableAsync(descriptor);

        Assert.True(opened.IsSuccess, opened.ErrorText);
        Assert.Null(opened.Data!.Metadata);
        Assert.True(opened.Data.Schema.IsDocumentDriven);
    }

    /// <summary>
    /// A path without a TableData directory is rejected with a file-not-found error.
    /// </summary>
    [Fact]
    public async Task Open_MissingDirectory_ReturnsFileNotFound()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var factory = new DataSetSessionFactory(
            provider,
            new FileSystemTableCatalog(metadataProvider: provider),
            new TableRepository(provider));

        var opened = await factory.OpenAsync(workspace.PathOf("missing"));

        Assert.False(opened.IsSuccess);
        Assert.Equal(ErrorCodes.IoFileNotFound, opened.ErrorCode);
    }

    private static async Task<(IDataSetSessionFactory Factory, string Data13, string TableData)>
        CreateDataSetAsync(TestWorkspace workspace)
    {
        var data13 = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(data13, "TableData");
        var itemsDirectory = Path.Combine(tableData, "Items");
        var lookupDirectory = Path.Combine(tableData, "Lookup");
        Directory.CreateDirectory(itemsDirectory);
        Directory.CreateDirectory(lookupDirectory);
        await File.WriteAllTextAsync(
            Path.Combine(itemsDirectory, "Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>0</uiIndex><szItemName>Nothing</szItemName></ITEM>"
            + "<ITEM><uiIndex>1</uiIndex><szItemName>Glock 17</szItemName></ITEM>"
            + "</ITEMLIST>");
        await File.WriteAllTextAsync(
            Path.Combine(itemsDirectory, "Russian.Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>1</uiIndex><szItemName>Глок 17</szItemName></ITEM>"
            + "</ITEMLIST>");
        await File.WriteAllTextAsync(
            Path.Combine(lookupDirectory, "ItemClass.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
            + "<ItemClass><id>1</id><name>Gun</name></ItemClass>"
            + "</JA2Data>");

        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var factory = new DataSetSessionFactory(provider, catalog, new TableRepository(provider));
        return (factory, data13, tableData);
    }
}
