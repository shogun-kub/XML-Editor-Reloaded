using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Tests for <see cref="TableSetWorkspace"/>: loading an item with its related tables, composite undo,
/// per-file saving, and the item workflow operations that touch several files at once.
/// </summary>
public class TableSetWorkspaceTests
{
    private static readonly TableName Items = TableName.Parse("Items.Items");
    private static readonly TableName Weapons = TableName.Parse("Items.Weapons");
    private static readonly TableName Attachments = TableName.Parse("Items.Attachments");

    /// <summary>
    /// Loading an item set resolves the 1:1 and 1:N targets that exist in the data directory and reports
    /// the missing ones as warnings without failing the load.
    /// </summary>
    [Fact]
    public async Task Load_ResolvesExistingTargetsAndWarnsAboutMissingOnes()
    {
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (set, _) = await LoadSetAsync(workspace, fixture);

        Assert.Equal(Items, set.Primary.Descriptor.TableName);
        var members = set.Members.Select(static member => member.Workspace.Descriptor.TableName).ToArray();
        Assert.Contains(Weapons, members);
        Assert.Contains(Attachments, members);
        Assert.DoesNotContain(members, name => name == TableName.Parse("Items.Magazines"));
        Assert.NotEmpty(set.Warnings);
        Assert.All(set.Warnings, warning => Assert.Contains("Items.", warning, StringComparison.Ordinal));

        var weapon = set.GetWorkspace(Weapons);
        Assert.NotNull(weapon);
        Assert.NotNull(weapon!.Metadata);
        Assert.Equal("WEAPONLIST", weapon.Metadata!.XmlRoot);
    }

    /// <summary>
    /// Duplicating an item copies the item record, its weapon row, and its child attachment rows in one
    /// transaction, and one undo restores all three files.
    /// </summary>
    [Fact]
    public async Task DuplicateRecord_CopiesDependentsAndUndoesAsOneStep()
    {
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (set, _) = await LoadSetAsync(workspace, fixture);

        var duplicate = set.DuplicateRecord("1");

        Assert.True(duplicate.IsSuccess, duplicate.ErrorText);
        Assert.Equal("3", duplicate.Data);
        Assert.Equal("Glock 17", set.Primary.FindRecord("3")!.GetRawValue("szItemName"));
        var copiedWeapon = set.GetWorkspace(Weapons)!.FindRecord("3");
        Assert.NotNull(copiedWeapon);
        Assert.Equal("100", copiedWeapon!.GetRawValue("usRange"));
        var copiedAttachments = set.GetWorkspace(Attachments)!.BaseDocument.Records
            .Where(record => record.GetRawValue("itemIndex") == "3")
            .ToArray();
        Assert.Single(copiedAttachments);
        Assert.Equal("21", copiedAttachments[0].GetRawValue("attachmentIndex"));

        Assert.True(set.IsDirty);
        Assert.True(set.Undo().IsSuccess);
        Assert.Null(set.Primary.FindRecord("3"));
        Assert.Null(set.GetWorkspace(Weapons)!.FindRecord("3"));
        Assert.DoesNotContain(
            set.GetWorkspace(Attachments)!.BaseDocument.Records,
            record => record.GetRawValue("itemIndex") == "3");
        Assert.False(set.IsDirty);

        Assert.True(set.Redo().IsSuccess);
        Assert.NotNull(set.Primary.FindRecord("3"));
        Assert.NotNull(set.GetWorkspace(Weapons)!.FindRecord("3"));
    }

    /// <summary>
    /// Deleting an item removes its weapon and child rows, keeps records of other items, and one undo
    /// restores every deleted record.
    /// </summary>
    [Fact]
    public async Task DeleteRecord_RemovesDependentsAndUndoesAsOneStep()
    {
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (set, _) = await LoadSetAsync(workspace, fixture);

        var dependents = set.DescribeDependents("1");
        Assert.Contains(dependents, entry => entry.Table == Weapons && entry.Keys.SequenceEqual(new[] { "1" }));
        Assert.Contains(dependents, entry => entry.Table == Attachments && entry.Keys.SequenceEqual(new[] { "10" }));

        Assert.True(set.DeleteRecord("1").IsSuccess);
        Assert.Null(set.Primary.FindRecord("1"));
        Assert.Null(set.GetWorkspace(Weapons)!.FindRecord("1"));
        Assert.Null(set.GetWorkspace(Attachments)!.FindRecord("10"));
        Assert.NotNull(set.Primary.FindRecord("2"));
        Assert.NotNull(set.GetWorkspace(Weapons)!.FindRecord("2"));

        Assert.True(set.Undo().IsSuccess);
        Assert.NotNull(set.Primary.FindRecord("1"));
        Assert.NotNull(set.GetWorkspace(Weapons)!.FindRecord("1"));
        Assert.NotNull(set.GetWorkspace(Attachments)!.FindRecord("10"));
    }

    /// <summary>
    /// Renaming an item key rewrites weapon and attachment references; a name already used by another
    /// item is rejected, and one undo restores the original references.
    /// </summary>
    [Fact]
    public async Task RenameRecord_RewritesDependentReferences()
    {
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (set, _) = await LoadSetAsync(workspace, fixture);

        var conflict = set.RenameRecord("1", "2");
        Assert.False(conflict.IsSuccess);
        Assert.Equal("key.conflict", conflict.ErrorCode);

        var rename = set.RenameRecord("1", "42");
        Assert.True(rename.IsSuccess, rename.ErrorText);
        Assert.NotNull(set.Primary.FindRecord("42"));
        Assert.Equal("42", set.GetWorkspace(Weapons)!.FindRecord("42")!.GetRawValue("uiIndex"));
        Assert.Equal("42", set.GetWorkspace(Attachments)!.FindRecord("10")!.GetRawValue("itemIndex"));

        Assert.True(set.Undo().IsSuccess);
        Assert.NotNull(set.Primary.FindRecord("1"));
        Assert.Equal("1", set.GetWorkspace(Weapons)!.FindRecord("1")!.GetRawValue("uiIndex"));
        Assert.Equal("1", set.GetWorkspace(Attachments)!.FindRecord("10")!.GetRawValue("itemIndex"));
    }

    /// <summary>
    /// Swapping two item keys exchanges the primary records and every dependent reference.
    /// </summary>
    [Fact]
    public async Task SwapRecords_ExchangesKeysAndDependents()
    {
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (set, _) = await LoadSetAsync(workspace, fixture);

        var swap = set.SwapRecords("1", "2");
        Assert.True(swap.IsSuccess, swap.ErrorText);
        Assert.Equal("Glock 17", set.Primary.FindRecord("2")!.GetRawValue("szItemName"));
        Assert.Equal("Glock 18", set.Primary.FindRecord("1")!.GetRawValue("szItemName"));
        Assert.Equal("1", set.GetWorkspace(Weapons)!.FindRecord("1")!.GetRawValue("uiIndex"));
        Assert.Equal("2", set.GetWorkspace(Weapons)!.FindRecord("2")!.GetRawValue("uiIndex"));
        Assert.Equal("2", set.GetWorkspace(Attachments)!.FindRecord("10")!.GetRawValue("itemIndex"));
    }

    /// <summary>
    /// Saving writes only dirty files; a clean related file keeps identical bytes, and a load/save cycle
    /// without edits rewrites nothing.
    /// </summary>
    [Fact]
    public async Task Save_WritesDirtyFilesOnlyAndKeepsCleanBytes()
    {
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (set, _) = await LoadSetAsync(workspace, fixture);
        var itemsBytes = await File.ReadAllBytesAsync(fixture.ItemsPath);
        var weaponBytes = await File.ReadAllBytesAsync(fixture.WeaponPath);

        Assert.True((await set.SaveAsync()).IsSuccess);
        Assert.Equal(itemsBytes, await File.ReadAllBytesAsync(fixture.ItemsPath));
        Assert.Equal(weaponBytes, await File.ReadAllBytesAsync(fixture.WeaponPath));

        Assert.True(set.SetValue(Items, "1", "szItemName", "Glock 17A").IsSuccess);
        Assert.True(set.SetValue(Weapons, "1", "usRange", "120").IsSuccess);
        Assert.True(set.IsDirty);
        Assert.True((await set.SaveAsync()).IsSuccess);
        Assert.False(set.IsDirty);

        Assert.NotEqual(itemsBytes, await File.ReadAllBytesAsync(fixture.ItemsPath));
        Assert.NotEqual(weaponBytes, await File.ReadAllBytesAsync(fixture.WeaponPath));
        var savedItems = XDocument.Load(fixture.ItemsPath);
        Assert.Equal(
            "Glock 17A",
            savedItems.Root!.Elements("ITEM").First(item => item.Element("uiIndex")?.Value == "1").Element("szItemName")!.Value);
        var savedWeapons = XDocument.Load(fixture.WeaponPath);
        Assert.Equal(
            "120",
            savedWeapons.Root!.Elements("WEAPON").First(weapon => weapon.Element("uiIndex")?.Value == "1").Element("usRange")!.Value);
    }

    /// <summary>
    /// Nested modifier fields and repeated attachment defaults are written without disturbing the rest of
    /// the sparse record.
    /// </summary>
    [Fact]
    public async Task NestedAndListFields_RoundTripWithoutRebuildingTheRecord()
    {
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (set, _) = await LoadSetAsync(workspace, fixture);

        var nested = set.SetValue(Items, "2", "STAND_MODIFIERS/FlatBase", "-3", addIfMissing: true);
        Assert.True(nested.IsSuccess, nested.ErrorText);
        var list = set.SetListValues(Items, "1", "DefaultAttachment", new[] { "7", "8" });
        Assert.True(list.IsSuccess, list.ErrorText);

        var item = set.Primary.FindRecord("2")!;
        Assert.Equal("-3", item.GetRawValue(FieldPath.Parse("STAND_MODIFIERS/FlatBase")));
        Assert.Equal("Glock 18", item.GetRawValue("szItemName"));
        Assert.True(item.HasField("CustomUnknown"));
        Assert.Equal(new[] { "7", "8" }, set.Primary.FindRecord("1")!.GetValues("DefaultAttachment").Select(static value => value.RawText));

        Assert.True((await set.SaveAsync()).IsSuccess);
        var saved = XDocument.Load(fixture.ItemsPath);
        var savedItem = saved.Root!.Elements("ITEM").First(element => element.Element("uiIndex")?.Value == "2");
        Assert.Equal("-3", savedItem.Element("STAND_MODIFIERS")!.Element("FlatBase")!.Value);
        Assert.Equal("keep me", savedItem.Element("CustomUnknown")!.Value);
        Assert.Equal("1.0", savedItem.Element("ProjectionFactor")!.Value);
        var savedFirst = saved.Root!.Elements("ITEM").First(element => element.Element("uiIndex")?.Value == "1");
        Assert.Equal(
            new[] { "7", "8" },
            savedFirst.Elements("DefaultAttachment").Select(static element => element.Value));
        Assert.Equal("Glock 17", savedFirst.Element("szItemName")!.Value);
    }

    /// <summary>
    /// A rename that would collide with an existing 1:1 related row is rejected before anything is
    /// mutated.
    /// </summary>
    [Fact]
    public async Task RenameRecord_RelatedRowConflict_LeavesSetUntouched()
    {
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace, includeWeaponFor42: true);
        var (set, _) = await LoadSetAsync(workspace, fixture);

        var rename = set.RenameRecord("1", "42");

        Assert.False(rename.IsSuccess);
        Assert.Equal("key.conflict", rename.ErrorCode);
        Assert.NotNull(set.Primary.FindRecord("1"));
        Assert.Equal("1", set.GetWorkspace(Weapons)!.FindRecord("1")!.GetRawValue("uiIndex"));
        Assert.Equal("42", set.GetWorkspace(Weapons)!.FindRecord("42")!.GetRawValue("uiIndex"));
        Assert.False(set.IsDirty);
    }

    /// <summary>
    /// Creating a missing 1:1 row reuses an existing row and creates exactly one record otherwise.
    /// </summary>
    [Fact]
    public async Task EnsureSubTableRecord_IsIdempotent()
    {
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace);
        var (set, _) = await LoadSetAsync(workspace, fixture);
        var relation = set.Members.Single(member => member.Relation.TargetTable == Weapons).Relation;

        var existing = set.EnsureSubTableRecord(relation, "1");
        Assert.True(existing.IsSuccess, existing.ErrorText);
        Assert.Equal(2, set.GetWorkspace(Weapons)!.BaseDocument.Records.Count);

        var created = set.EnsureSubTableRecord(relation, "3");
        Assert.True(created.IsSuccess, created.ErrorText);
        Assert.Equal(3, set.GetWorkspace(Weapons)!.BaseDocument.Records.Count);
        Assert.NotNull(set.GetWorkspace(Weapons)!.FindRecord("3"));

        Assert.True(set.Undo().IsSuccess);
        Assert.Null(set.GetWorkspace(Weapons)!.FindRecord("3"));
    }

    /// <summary>
    /// Including reverse relations makes dealer inventories editable members of an item set: quantity
    /// edits are undoable, and renaming or deleting an item rewrites or removes the stock rows that
    /// reference it.
    /// </summary>
    [Fact]
    public async Task ReverseRelations_LoadDealerInventoriesAndTrackItemReferences()
    {
        using var workspace = new TestWorkspace();
        var fixture = CreateFixture(workspace, includeDealer: true);
        var (set, _) = await LoadSetAsync(workspace, fixture, includeReverseRelations: true);
        var dealer = TableName.Parse("NPCInventory.AlbertoInventory");
        var stock = Assert.Single(
            set.GetMembers(TableRelationKind.Reverse),
            member => member.Relation.TargetTable == dealer);
        Assert.Equal("sItemIndex", stock.Relation.TargetValueField);
        Assert.Equal("1", stock.Workspace.FindRecord("0")!.GetRawValue("sItemIndex"));

        var quantity = set.SetValue(dealer, "0", "ubOptimalNumber", "11");
        Assert.True(quantity.IsSuccess, quantity.ErrorText);
        Assert.Equal("11", stock.Workspace.FindRecord("0")!.GetRawValue("ubOptimalNumber"));
        Assert.True(set.Undo().IsSuccess);
        Assert.Equal("9", stock.Workspace.FindRecord("0")!.GetRawValue("ubOptimalNumber"));

        var rename = set.RenameRecord("1", "42");
        Assert.True(rename.IsSuccess, rename.ErrorText);
        Assert.Equal("42", stock.Workspace.FindRecord("0")!.GetRawValue("sItemIndex"));
        Assert.True(set.Undo().IsSuccess);
        Assert.Equal("1", stock.Workspace.FindRecord("0")!.GetRawValue("sItemIndex"));

        var dependents = Assert.Single(
            set.DescribeDependents("1"),
            entry => entry.Table == dealer);
        Assert.Equal(new[] { "0", "2" }, dependents.Keys.OrderBy(static key => key, StringComparer.Ordinal));
        Assert.True(set.DeleteRecord("1").IsSuccess);
        Assert.Null(stock.Workspace.FindRecord("0"));
        Assert.Null(stock.Workspace.FindRecord("2"));
        Assert.NotNull(stock.Workspace.FindRecord("1"));
        Assert.True(set.Undo().IsSuccess);
        Assert.NotNull(stock.Workspace.FindRecord("0"));
        Assert.NotNull(stock.Workspace.FindRecord("2"));

        var itemBytes = await File.ReadAllBytesAsync(fixture.ItemsPath);
        Assert.True((await set.SaveAsync()).IsSuccess);
        Assert.Equal(itemBytes, await File.ReadAllBytesAsync(fixture.ItemsPath));
    }

    private static async Task<(TableSetWorkspace Set, SqliteMetadataProvider Provider)> LoadSetAsync(
        TestWorkspace workspace,
        Fixture fixture,
        bool includeReverseRelations = false)
    {
        var databasePath = workspace.PathOf("metadata.sqlite");
        var migration = await new DatabaseMigrator().MigrateAsync(databasePath);
        Assert.True(migration.IsSuccess, migration.ErrorText);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var descriptors = await catalog.GetTablesAsync(fixture.TableDataDirectory);
        Assert.True(descriptors.IsSuccess, descriptors.ErrorText);
        var descriptor = descriptors.Data!.Single(candidate => candidate.TableName == Items);
        var load = await TableSetWorkspace.LoadAsync(
            descriptor,
            catalog,
            provider,
            fixture.TableDataDirectory,
            includeReverseRelations: includeReverseRelations);
        Assert.True(load.IsSuccess, load.ErrorText);
        return (load.Data!, provider);
    }

    private static Fixture CreateFixture(
        TestWorkspace workspace,
        bool includeWeaponFor42 = false,
        bool includeDealer = false)
    {
        var tableData = workspace.PathOf(Path.Combine("Data-1.13", "TableData"));
        var itemsDirectory = Path.Combine(tableData, "Items");
        Directory.CreateDirectory(itemsDirectory);
        var itemsPath = Path.Combine(itemsDirectory, "Items.xml");
        var weaponPath = Path.Combine(itemsDirectory, "Weapons.xml");
        var attachmentsPath = Path.Combine(itemsDirectory, "Attachments.xml");
        var dealerPath = Path.Combine(tableData, "NPCInventory", "AlbertoInventory.xml");
        if (includeDealer)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dealerPath)!);
            File.WriteAllText(
                dealerPath,
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><INVENTORYLIST>\r\n"
                + "\t<INVENTORY>\r\n"
                + "\t\t<uiIndex>0</uiIndex>\r\n"
                + "\t\t<sItemIndex>1</sItemIndex>\r\n"
                + "\t\t<ubOptimalNumber>9</ubOptimalNumber>\r\n"
                + "\t</INVENTORY>\r\n"
                + "\t<INVENTORY>\r\n"
                + "\t\t<uiIndex>1</uiIndex>\r\n"
                + "\t\t<sItemIndex>2</sItemIndex>\r\n"
                + "\t\t<ubOptimalNumber>6</ubOptimalNumber>\r\n"
                + "\t</INVENTORY>\r\n"
                + "\t<INVENTORY>\r\n"
                + "\t\t<uiIndex>2</uiIndex>\r\n"
                + "\t\t<sItemIndex>1</sItemIndex>\r\n"
                + "\t\t<ubOptimalNumber>4</ubOptimalNumber>\r\n"
                + "\t</INVENTORY>\r\n"
                + "</INVENTORYLIST>\r\n");
        }

        File.WriteAllText(
            itemsPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>\r\n"
            + "\t<ITEM>\r\n"
            + "\t\t<uiIndex>1</uiIndex>\r\n"
            + "\t\t<szItemName>Glock 17</szItemName>\r\n"
            + "\t\t<usItemClass>2</usItemClass>\r\n"
            + "\t\t<ubClassIndex>1</ubClassIndex>\r\n"
            + "\t\t<ProjectionFactor>1.0</ProjectionFactor>\r\n"
            + "\t\t<DefaultAttachment>5</DefaultAttachment>\r\n"
            + "\t\t<STAND_MODIFIERS />\r\n"
            + "\t\t<CustomUnknown>keep me</CustomUnknown>\r\n"
            + "\t</ITEM>\r\n"
            + "\t<ITEM>\r\n"
            + "\t\t<uiIndex>2</uiIndex>\r\n"
            + "\t\t<szItemName>Glock 18</szItemName>\r\n"
            + "\t\t<usItemClass>2</usItemClass>\r\n"
            + "\t\t<ubClassIndex>2</ubClassIndex>\r\n"
            + "\t\t<STAND_MODIFIERS />\r\n"
            + "\t\t<CustomUnknown>keep me</CustomUnknown>\r\n"
            + "\t\t<ProjectionFactor>1.0</ProjectionFactor>\r\n"
            + "\t</ITEM>\r\n"
            + "</ITEMLIST>\r\n");
        File.WriteAllText(
            weaponPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><WEAPONLIST>\r\n"
            + "\t<WEAPON>\r\n"
            + "\t\t<uiIndex>1</uiIndex>\r\n"
            + "\t\t<usRange>100</usRange>\r\n"
            + "\t\t<ubCalibre>0</ubCalibre>\r\n"
            + "\t</WEAPON>\r\n"
            + "\t<WEAPON>\r\n"
            + "\t\t<uiIndex>2</uiIndex>\r\n"
            + "\t\t<usRange>110</usRange>\r\n"
            + "\t\t<ubCalibre>0</ubCalibre>\r\n"
            + "\t</WEAPON>\r\n"
            + (includeWeaponFor42
                ? "\t<WEAPON>\r\n\t\t<uiIndex>42</uiIndex>\r\n\t\t<usRange>120</usRange>\r\n"
                  + "\t\t<ubCalibre>0</ubCalibre>\r\n\t</WEAPON>\r\n"
                : string.Empty)
            + "</WEAPONLIST>\r\n");
        File.WriteAllText(
            attachmentsPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ATTACHMENTLIST>\r\n"
            + "\t<ATTACHMENT>\r\n"
            + "\t\t<attachmentIndex>10</attachmentIndex>\r\n"
            + "\t\t<itemIndex>1</itemIndex>\r\n"
            + "\t\t<APCost>5</APCost>\r\n"
            + "\t</ATTACHMENT>\r\n"
            + "\t<ATTACHMENT>\r\n"
            + "\t\t<attachmentIndex>20</attachmentIndex>\r\n"
            + "\t\t<itemIndex>2</itemIndex>\r\n"
            + "\t\t<APCost>6</APCost>\r\n"
            + "\t</ATTACHMENT>\r\n"
            + "</ATTACHMENTLIST>\r\n");

        return new Fixture(tableData, itemsPath, weaponPath, attachmentsPath, dealerPath);
    }

    private sealed record Fixture(
        string TableDataDirectory,
        string ItemsPath,
        string WeaponPath,
        string AttachmentsPath,
        string DealerPath);
}
