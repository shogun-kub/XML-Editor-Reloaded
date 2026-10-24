using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Tests that metadata and catalog operations honor cancellation.
/// </summary>
public class CancellationTests
{
    /// <summary>
    /// Catalog discovery throws when the token is already cancelled.
    /// </summary>
    [Fact]
    public async Task Catalog_PreCancelledToken_Throws()
    {
        using var workspace = new TestWorkspace();
        var tableData = workspace.PathOf("TableData");
        Directory.CreateDirectory(tableData);
        using var source = new CancellationTokenSource();
        source.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => new FileSystemTableCatalog().GetTablesAsync(tableData, source.Token));
    }

    /// <summary>
    /// Metadata queries throw when the token is already cancelled.
    /// </summary>
    [Fact]
    public async Task MetadataProvider_PreCancelledToken_Throws()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        using var source = new CancellationTokenSource();
        source.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => provider.GetAllTablesAsync(source.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => provider.GetLanguagesAsync(source.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => provider.GetTableAsync(Core.Metadata.TableName.Parse("Items.Items"), source.Token));
    }
}
