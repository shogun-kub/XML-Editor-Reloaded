using Microsoft.Extensions.DependencyInjection;
using XmlEditorReloaded.Core;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml;
using XmlEditorReloaded.Xml.Copy;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// End-to-end tests for the Data + Xml composition root.
/// </summary>
public class RepositoryIntegrationTests
{
    /// <summary>
    /// A mapped table loads with metadata, edits, and saves through the repository.
    /// </summary>
    [Fact]
    public async Task Repository_LoadEditSave_MappedTable()
    {
        using var workspace = new TestWorkspace();
        var tableData = CreateTableData(workspace);
        var provider = await BuildProviderAsync(workspace, tableData);
        var catalog = provider.GetRequiredService<ITableCatalog>();
        var repository = provider.GetRequiredService<ITableRepository>();
        var items = await FindAsync(catalog, tableData, "Items.Items");

        var load = await repository.LoadTableAsync(items);

        Assert.True(load.IsSuccess, load.ErrorText);
        Assert.Equal(TableDocumentStatus.Loaded, load.Data!.Status);
        Assert.True(load.Data.Descriptor.IsMapped);
        var document = load.Data.GetDocument<TableDocument>()!;
        Assert.Equal(2, document.Records.Count);

        Assert.True(document.Records[1].SetFieldValue("szItemName", "Glock 18").IsSuccess);
        Assert.True((await repository.SaveTableAsync(load.Data)).IsSuccess);
        Assert.Equal("Glock 18", document.Records[1].GetRawValue("szItemName"));
    }

    /// <summary>
    /// Malformed files are reported as diagnostics and cannot be saved.
    /// </summary>
    [Fact]
    public async Task Repository_Malformed_FailsSoft()
    {
        using var workspace = new TestWorkspace();
        var tableData = CreateTableData(workspace);
        var provider = await BuildProviderAsync(workspace, tableData);
        var catalog = provider.GetRequiredService<ITableCatalog>();
        var repository = provider.GetRequiredService<ITableRepository>();
        var malformed = await FindAsync(catalog, tableData, "Lookup.Bad");

        var load = await repository.LoadTableAsync(malformed);
        var save = await repository.SaveTableAsync(load.Data!);

        Assert.True(load.IsSuccess, load.ErrorText);
        Assert.Equal(TableDocumentStatus.Malformed, load.Data!.Status);
        Assert.Contains(load.Data.Diagnostics, diagnostic => diagnostic.Code == ErrorCodes.XmlParseError);
        Assert.False(save.IsSuccess);
        Assert.Equal(ErrorCodes.XmlNotSaveable, save.ErrorCode);
    }

    /// <summary>
    /// Tables without metadata load document-driven and carry an unmapped diagnostic.
    /// </summary>
    [Fact]
    public async Task Repository_Unmapped_LoadsDocumentDriven()
    {
        using var workspace = new TestWorkspace();
        var tableData = CreateTableData(workspace);
        var provider = await BuildProviderAsync(workspace, tableData);
        var catalog = provider.GetRequiredService<ITableCatalog>();
        var repository = provider.GetRequiredService<ITableRepository>();
        var custom = await FindAsync(catalog, tableData, "Custom.Thing");

        var load = await repository.LoadTableAsync(custom);

        Assert.True(load.IsSuccess, load.ErrorText);
        Assert.Equal(TableDocumentStatus.Loaded, load.Data!.Status);
        Assert.False(load.Data.Descriptor.IsMapped);
        Assert.Contains(load.Data.Diagnostics, diagnostic => diagnostic.Code == ErrorCodes.MetadataUnmapped);
        var document = load.Data.GetDocument<TableDocument>()!;
        Assert.Null(document.Metadata);
        Assert.Single(document.Records);
    }

    /// <summary>
    /// A descriptor marked as mapped falls back to document-driven mode when metadata is missing.
    /// </summary>
    [Fact]
    public async Task Repository_MappedDescriptorWithoutMetadata_FallsBack()
    {
        using var workspace = new TestWorkspace();
        var tableData = CreateTableData(workspace);
        var provider = await BuildProviderAsync(workspace, tableData);
        var catalog = provider.GetRequiredService<ITableCatalog>();
        var repository = provider.GetRequiredService<ITableRepository>();
        var custom = await FindAsync(catalog, tableData, "Custom.Thing");

        var load = await repository.LoadTableAsync(custom with { IsMapped = true });

        Assert.True(load.IsSuccess, load.ErrorText);
        Assert.False(load.Data!.Descriptor.IsMapped);
        Assert.Contains(load.Data.Diagnostics, diagnostic => diagnostic.Code == ErrorCodes.MetadataUnmapped);
        Assert.Null(load.Data.GetDocument<TableDocument>()!.Metadata);
    }

    /// <summary>
    /// The DI-registered lookup repository discovers languages from metadata and applies the overlay.
    /// </summary>
    [Fact]
    public async Task LookupRepository_AppliesOverlayFromDi()
    {
        using var workspace = new TestWorkspace();
        var tableData = CreateTableData(workspace);
        var provider = await BuildProviderAsync(workspace, tableData);
        var lookup = provider.GetRequiredService<ILookupRepository>();

        var result = await lookup.GetEntriesAsync(TableName.Parse("Lookup.ItemClass"), "ru");

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(new LookupEntry(1, "Оружие"), Assert.Single(result.Data!));
    }

    /// <summary>
    /// The translation session writes only to the overlay and leaves the base file untouched.
    /// </summary>
    [Fact]
    public async Task TranslationCycle_BaseFileIsUntouched()
    {
        using var workspace = new TestWorkspace();
        var tableData = CreateTableData(workspace);
        var provider = await BuildProviderAsync(workspace, tableData);
        var catalog = provider.GetRequiredService<ITableCatalog>();
        var repository = provider.GetRequiredService<ITableRepository>();
        var metadataProvider = provider.GetRequiredService<ITableMetadataProvider>();
        var items = await FindAsync(catalog, tableData, "Items.Items");
        var metadata = (await metadataProvider.GetTableAsync(items.TableName)).Data!;
        var baseLoad = await repository.LoadTableAsync(items);
        var baseDocument = baseLoad.Data!.GetDocument<TableDocument>()!;
        var overlay = await OverlayDocument.LoadAsync(Assert.Single(items.Overlays), metadata);
        Assert.True(overlay.IsSuccess, overlay.ErrorText);
        var beforeBase = await File.ReadAllBytesAsync(items.FilePath);
        var baseWriteTime = File.GetLastWriteTimeUtc(items.FilePath);
        var session = new TableEditSession(baseDocument, metadata, overlay.Data!);

        Assert.True(session.SetValue("1", FieldPath.Parse("szItemName"), "Глок 18").IsSuccess);
        Assert.True((await session.SaveAsync()).IsSuccess);

        Assert.Equal(beforeBase, await File.ReadAllBytesAsync(items.FilePath));
        Assert.Equal(baseWriteTime, File.GetLastWriteTimeUtc(items.FilePath));
        var overlayText = await File.ReadAllTextAsync(items.Overlays[0].OverlayFilePath);
        Assert.Contains("Глок 18", overlayText, StringComparison.Ordinal);
    }

    /// <summary>
    /// Copying a lookup row through the real SQLite metadata resolves an absent id by the seeded
    /// comparison field, so an existing target row with the same name is reused instead of duplicated.
    /// </summary>
    [Fact]
    public async Task Copy_LookupRowWithAbsentId_MatchesSeededComparisonField()
    {
        using var workspace = new TestWorkspace();
        var sourceTableData = CreateTableData(workspace);
        var targetTableData = workspace.PathOf("target/TableData");
        var targetLookup = Path.Combine(targetTableData, "Lookup");
        Directory.CreateDirectory(targetLookup);
        var targetPath = Path.Combine(targetLookup, "ItemClass.xml");
        await File.WriteAllTextAsync(
            targetPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
            + "<ItemClass><id>20</id><name>Gun</name></ItemClass>"
            + "</JA2Data>");
        var targetBefore = await File.ReadAllBytesAsync(targetPath);
        var provider = await BuildProviderAsync(workspace, sourceTableData);
        var copy = provider.GetRequiredService<ITableCopyService>();

        var result = await copy.ApplyAsync(new TableCopyRequest(
            sourceTableData,
            targetTableData,
            TableName.Parse("Lookup.ItemClass"),
            new[] { "1" },
            TableCopyConflictPolicy.Skip));

        Assert.True(result.IsSuccess, result.ErrorText);
        var table = Assert.Single(result.Data!.Tables);
        Assert.Equal(0, table.Added);
        Assert.Equal(1, table.Skipped);
        Assert.Equal(targetBefore, await File.ReadAllBytesAsync(targetPath));
    }

    /// <summary>
    /// All backend services resolve from the shared composition root.
    /// </summary>
    [Fact]
    public async Task CompositionRoot_ResolvesBackendServices()
    {
        using var workspace = new TestWorkspace();
        var tableData = CreateTableData(workspace);
        var provider = await BuildProviderAsync(workspace, tableData);

        Assert.NotNull(provider.GetService<ITableMetadataProvider>());
        Assert.NotNull(provider.GetService<ITableCatalog>());
        Assert.NotNull(provider.GetService<ITranslationCatalog>());
        Assert.NotNull(provider.GetService<ITableRepository>());
        Assert.NotNull(provider.GetService<ILookupRepository>());
        Assert.NotNull(provider.GetService<ISettingsStore>());
    }

    private static async Task<ServiceProvider> BuildProviderAsync(TestWorkspace workspace, string tableData)
    {
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var services = new ServiceCollection();
        services.AddXmlEditorCore();
        services.AddXmlEditorData(databasePath, tableData);
        services.AddXmlEditorXml(tableData);
        return services.BuildServiceProvider();
    }

    private static async Task<TableDescriptor> FindAsync(
        ITableCatalog catalog,
        string tableData,
        string tableName)
    {
        var tables = await catalog.GetTablesAsync(tableData);
        Assert.True(tables.IsSuccess, tables.ErrorText);
        return Assert.Single(tables.Data!, table => table.TableName.Value == tableName);
    }

    private static string CreateTableData(TestWorkspace workspace)
    {
        var root = workspace.PathOf("Data-1.13/TableData");
        Directory.CreateDirectory(Path.Combine(root, "Items"));
        Directory.CreateDirectory(Path.Combine(root, "Lookup"));
        Directory.CreateDirectory(Path.Combine(root, "Custom"));

        File.WriteAllText(
            Path.Combine(root, "Items", "Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>0</uiIndex><szItemName>Nothing</szItemName></ITEM>"
            + "<ITEM><uiIndex>1</uiIndex><szItemName>Glock 17</szItemName></ITEM>"
            + "</ITEMLIST>");
        File.WriteAllText(
            Path.Combine(root, "Items", "Russian.Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>1</uiIndex><szItemName>Глок 17</szItemName></ITEM>"
            + "</ITEMLIST>");
        File.WriteAllText(
            Path.Combine(root, "Lookup", "ItemClass.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
            + "<ItemClass><id>1</id><name>Gun</name></ItemClass>"
            + "</JA2Data>");
        File.WriteAllText(
            Path.Combine(root, "Lookup", "Russian.ItemClass.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data>"
            + "<ItemClass><id>1</id><name>Оружие</name></ItemClass>"
            + "</JA2Data>");
        File.WriteAllText(
            Path.Combine(root, "Lookup", "Bad.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data><ItemClass><id>1</id></ItemClass></ItemClass></JA2Data>");
        File.WriteAllText(
            Path.Combine(root, "Custom", "Thing.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><THINGLIST><THING><id>1</id><name>Custom</name></THING></THINGLIST>");
        return root;
    }
}
