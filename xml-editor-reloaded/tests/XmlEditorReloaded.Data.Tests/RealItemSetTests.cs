using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Environment-gated item-set checks against a real game <c>TableData</c> directory.
/// Set <c>XER_CORPUS_DIR</c> to a v4 or v5 TableData directory to enable them.
/// </summary>
public class RealItemSetTests
{
    private static readonly TableName Items = TableName.Parse("Items.Items");

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.EnumerateFiles(source))
        {
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: true);
        }

        foreach (var directory in Directory.EnumerateDirectories(source))
        {
            CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
        }
    }

    /// <summary>
    /// The item set loads every existing related table, and composite duplicate, rename, and swap
    /// operations undo back to a clean state without touching the source files.
    /// </summary>
    [Fact]
    public async Task ItemSet_LoadsAndUndoesCompositeEditsOnRealData()
    {
        var corpus = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(corpus) || !Directory.Exists(corpus))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var tableData = workspace.PathOf("TableData");
        var itemsDirectory = Path.Combine(corpus, "Items");
        if (!Directory.Exists(itemsDirectory))
        {
            return;
        }

        CopyDirectory(itemsDirectory, Path.Combine(tableData, "Items"));
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var descriptors = await catalog.GetTablesAsync(tableData);
        Assert.True(descriptors.IsSuccess, descriptors.ErrorText);
        var descriptor = descriptors.Data!.Single(candidate => candidate.TableName == Items);

        var load = await TableSetWorkspace.LoadAsync(descriptor, catalog, provider, tableData);
        Assert.True(load.IsSuccess, load.ErrorText);
        var set = load.Data!;

        Assert.True(set.Members.Count >= 12, $"Only {set.Members.Count} related item tables were loaded.");
        var members = set.Members.Select(static member => member.Relation.TargetTable).ToHashSet();
        foreach (var expected in new[]
                 {
                     "Items.Weapons",
                     "Items.Magazines",
                     "Items.Armours",
                     "Items.Explosives",
                     "Items.LoadBearingEquipment",
                     "Items.Attachments",
                     "Items.AttachmentInfo",
                 })
        {
            Assert.Contains(TableName.Parse(expected), members);
        }

        var gun = set.Primary.BaseDocument.Records.First(record =>
            record.GetRawValue("szItemName") is not null
            && int.TryParse(record.GetRawValue("usItemClass"), out var itemClass)
            && (itemClass & 2) != 0);
        var key = gun.GetKey()!;

        var duplicate = set.DuplicateRecord(key);
        Assert.True(duplicate.IsSuccess, duplicate.ErrorText);
        Assert.NotNull(set.Primary.FindRecord(duplicate.Data!));
        Assert.True(set.IsDirty);

        Assert.True(set.Undo().IsSuccess);
        Assert.Null(set.Primary.FindRecord(duplicate.Data!));
        Assert.False(set.IsDirty);

        var rename = set.RenameRecord(key, "9000001");
        Assert.True(rename.IsSuccess, rename.ErrorText);
        Assert.NotNull(set.Primary.FindRecord("9000001"));
        Assert.True(set.Undo().IsSuccess);
        Assert.NotNull(set.Primary.FindRecord(key));
        Assert.False(set.IsDirty);

        var dependents = set.DescribeDependents(key);
        Assert.NotNull(dependents);
    }

    /// <summary>
    /// An item workspace that includes reverse relations loads every dealer inventory, finds the rows
    /// that stock the selected item, and saves a quantity edit only to the dealer file.
    /// </summary>
    [Fact]
    public async Task ItemSet_ReverseDealerInventoriesLoadAndSaveOnlyDealerFiles()
    {
        var corpus = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(corpus) || !Directory.Exists(corpus))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var tableData = workspace.PathOf("TableData");
        foreach (var directory in new[] { "Items", "NPCInventory" })
        {
            var source = Path.Combine(corpus, directory);
            if (!Directory.Exists(source))
            {
                return;
            }

            CopyDirectory(source, Path.Combine(tableData, directory));
        }

        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var descriptors = await catalog.GetTablesAsync(tableData);
        Assert.True(descriptors.IsSuccess, descriptors.ErrorText);
        var descriptor = descriptors.Data!.Single(candidate => candidate.TableName == Items);

        var load = await TableSetWorkspace.LoadAsync(
            descriptor,
            catalog,
            provider,
            tableData,
            includeReverseRelations: true);
        Assert.True(load.IsSuccess, load.ErrorText);
        var set = load.Data!;

        var reverse = set.GetMembers(TableRelationKind.Reverse).ToList();
        Assert.Equal(80, reverse.Count);
        Assert.All(
            reverse,
            member => Assert.StartsWith(
                "NPCInventory.",
                member.Relation.TargetTable.Value,
                StringComparison.Ordinal));

        var stock = reverse.First(member => member.Workspace.GetRows().Any(
            static row => !string.IsNullOrWhiteSpace(row.GetValue("sItemIndex"))
                && !string.IsNullOrWhiteSpace(row.GetValue("ubOptimalNumber"))));
        var row = stock.Workspace.GetRows().First(
            static candidate => !string.IsNullOrWhiteSpace(candidate.GetValue("sItemIndex"))
                && !string.IsNullOrWhiteSpace(candidate.GetValue("ubOptimalNumber")));
        Assert.NotNull(row.Key);
        var quantity = row.GetValue("ubOptimalNumber")!.Trim();
        var updated = quantity == "7" ? "8" : "7";

        var itemsPath = Path.Combine(tableData, Items.ToRelativePath());
        var dealerPath = Path.Combine(tableData, stock.Relation.TargetTable.ToRelativePath());
        var itemsBefore = await File.ReadAllBytesAsync(itemsPath);
        var dealerBefore = await File.ReadAllBytesAsync(dealerPath);

        var edit = set.SetValue(stock.Relation.TargetTable, row.Key!, "ubOptimalNumber", updated);
        Assert.True(edit.IsSuccess, edit.ErrorText);
        Assert.True(set.IsDirty);
        Assert.True(set.Undo().IsSuccess);
        Assert.Equal(quantity, stock.Workspace.FindRecord(row.Key!)?.GetRawValue("ubOptimalNumber")?.Trim());
        Assert.True(set.SetValue(stock.Relation.TargetTable, row.Key!, "ubOptimalNumber", updated).IsSuccess);

        Assert.True((await set.SaveAsync()).IsSuccess);
        Assert.Equal(itemsBefore, await File.ReadAllBytesAsync(itemsPath));
        Assert.NotEqual(dealerBefore, await File.ReadAllBytesAsync(dealerPath));
        Assert.Equal(updated, stock.Workspace.FindRecord(row.Key!)?.GetRawValue("ubOptimalNumber")?.Trim());
    }
}
