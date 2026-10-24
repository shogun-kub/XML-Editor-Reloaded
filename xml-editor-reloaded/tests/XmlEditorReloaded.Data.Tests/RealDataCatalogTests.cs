using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Environment-gated catalog check against a real game <c>TableData</c> directory.
/// Set <c>XER_CORPUS_DIR</c> to a v4 or v5 TableData directory to enable it.
/// </summary>
public class RealDataCatalogTests
{
    /// <summary>
    /// The catalog finds base tables, attaches Russian overlays, and marks known tables as mapped.
    /// </summary>
    [Fact]
    public async Task Catalog_ReadsRealTableData()
    {
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var languages = await provider.GetLanguagesAsync();
        Assert.True(languages.IsSuccess, languages.ErrorText);
        var catalog = new FileSystemTableCatalog(languages.Data!, provider);

        var result = await catalog.GetTablesAsync(tableData);

        Assert.True(result.IsSuccess, result.ErrorText);
        var isV4 = tableData.Contains("v4", StringComparison.OrdinalIgnoreCase);
        Assert.Equal(isV4 ? 238 : 239, result.Data!.Count);
        var expectedOverlays = isV4 ? 25 : 0;
        Assert.Equal(expectedOverlays, result.Data.Sum(static table => table.Overlays.Count));
        var items = Assert.Single(result.Data, table => table.TableName.Value == "Items.Items");
        Assert.True(items.IsMapped);
        Assert.Equal(expectedOverlays == 0 ? 0 : 1, items.Overlays.Count);
        Assert.Empty(Assert.Single(result.Data, table => table.TableName.Value == "Lookup.ItemClass").Overlays);
    }
}
