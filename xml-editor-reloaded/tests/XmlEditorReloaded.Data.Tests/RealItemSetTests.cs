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
}
