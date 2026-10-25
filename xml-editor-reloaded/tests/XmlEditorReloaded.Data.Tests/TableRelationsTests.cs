using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Metadata;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Tests for the seeded <c>TableRelations</c> metadata.
/// </summary>
public class TableRelationsTests
{
    /// <summary>
    /// The Items table declares reference, sub-table, child-row, and reverse relations, including the
    /// filtered DefaultAttachment picker and the ITEMTOEXPLOSIVE replacement.
    /// </summary>
    [Fact]
    public async Task GetRelations_Items_DeclaresAllKinds()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var result = await provider.GetRelationsAsync(TableName.Parse("Items.Items"));

        Assert.True(result.IsSuccess, result.ErrorText);
        var relations = result.Data!;
        Assert.Equal(103, relations.Count);
        Assert.Equal(7, relations.Count(relation => relation.Kind == TableRelationKind.Reference));
        Assert.Equal(5, relations.Count(relation => relation.Kind == TableRelationKind.SubTable));
        Assert.Equal(7, relations.Count(relation => relation.Kind == TableRelationKind.ChildRows));
        Assert.Equal(84, relations.Count(relation => relation.Kind == TableRelationKind.Reverse));
        Assert.Contains(
            relations,
            relation => relation.Kind == TableRelationKind.Reverse
                && relation.TargetTable.Value == "NPCInventory.AlbertoInventory"
                && relation.TargetValueField == "sItemIndex"
                && relation.TargetDisplayField == "ubOptimalNumber");

        var defaultAttachment = Assert.Single(
            relations,
            relation => relation.SourceField == "DefaultAttachment");
        Assert.Equal("Items.Items", defaultAttachment.TargetTable.Value);
        Assert.Equal("uiIndex", defaultAttachment.TargetValueField);
        Assert.Equal("szLongItemName", defaultAttachment.TargetDisplayField);
        Assert.NotNull(defaultAttachment.Filter);
        Assert.Equal("Attachment", defaultAttachment.Filter!.Field);
        Assert.Equal(RelationFilterOperator.Equals, defaultAttachment.Filter.Operator);
        Assert.True(defaultAttachment.RemapByText);

        var magazines = Assert.Single(
            relations,
            relation => relation.Kind == TableRelationKind.SubTable
                && relation.TargetTable.Value == "Items.Magazines");
        Assert.Equal("ubClassIndex", magazines.SourceField);
        Assert.Equal("uiIndex", magazines.TargetValueField);

        var lbe = Assert.Single(
            relations,
            relation => relation.Kind == TableRelationKind.SubTable
                && relation.TargetTable.Value == "Items.LoadBearingEquipment");
        Assert.Equal("lbeIndex", lbe.TargetValueField);

        var compatibleFaces = Assert.Single(
            relations,
            relation => relation.Kind == TableRelationKind.ChildRows
                && relation.TargetTable.Value == "Items.CompatibleFaceItems");
        Assert.Equal("itemIndex", compatibleFaces.TargetValueField);
        Assert.Equal("compatiblefaceitemIndex", compatibleFaces.TargetDisplayField);

        var compatibleFacesReverse = Assert.Single(
            relations,
            relation => relation.Kind == TableRelationKind.Reverse
                && relation.TargetTable.Value == "Items.CompatibleFaceItems");
        Assert.Equal("compatiblefaceitemIndex", compatibleFacesReverse.TargetValueField);

        var attachTo = Assert.Single(
            relations,
            relation => relation.Kind == TableRelationKind.Reverse
                && relation.TargetTable.Value == "Items.Attachments");
        Assert.Equal("attachmentIndex", attachTo.TargetValueField);
        Assert.Equal("ItemSectionReverse.Items_Attachments", attachTo.LabelKey);

        var ammunition = Assert.Single(
            relations,
            relation => relation.Kind == TableRelationKind.Reverse
                && relation.TargetTable.Value == "Items.Launchables");
        Assert.Equal("itemIndex", ammunition.TargetValueField);
        Assert.Equal("ItemSectionReverse.Items_Launchables", ammunition.LabelKey);

        var ammoTypes = await provider.GetRelationsAsync(TableName.Parse("Items.AmmoTypes"));
        var highExplosive = Assert.Single(
            ammoTypes.Data!,
            relation => relation.SourceField == "highExplosive");
        Assert.Equal(RelationFilterOperator.HasFlag, highExplosive.Filter!.Operator);
        Assert.Equal("768", highExplosive.Filter.Value);
        Assert.DoesNotContain(relations, relation => relation.TargetTable.Value == "ITEMTOEXPLOSIVE");
    }

    /// <summary>
    /// Other item tables receive their main-table pickers, including auxiliary Food/Clothes/RandomItem
    /// targets that the legacy editor referenced by table name.
    /// </summary>
    [Fact]
    public async Task GetRelations_OtherSeededTables_DeclareMainTableReferences()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var attachments = await provider.GetRelationsAsync(TableName.Parse("Items.Attachments"));
        Assert.True(attachments.IsSuccess, attachments.ErrorText);
        Assert.Equal(2, attachments.Data!.Count);
        Assert.All(attachments.Data, relation =>
        {
            Assert.Equal(TableRelationKind.Reference, relation.Kind);
            Assert.Equal("Items.Items", relation.TargetTable.Value);
        });
        Assert.Contains(attachments.Data, relation => relation.SourceField == "attachmentIndex"
            && relation.Filter!.Field == "Attachment");
        Assert.Contains(attachments.Data, relation => relation.SourceField == "itemIndex"
            && relation.Filter!.Operator == RelationFilterOperator.NotEquals);

        var weapons = await provider.GetRelationsAsync(TableName.Parse("Items.Weapons"));
        Assert.True(weapons.IsSuccess, weapons.ErrorText);
        Assert.Equal(8, weapons.Data!.Count);
        var calibre = Assert.Single(weapons.Data, relation => relation.TargetTable.Value == "Items.AmmoStrings");
        Assert.Equal("ubCalibre", calibre.SourceField);
        Assert.Equal("uiIndex", calibre.TargetValueField);
        Assert.Equal("AmmoCaliber", calibre.TargetDisplayField);

        var soundTargets = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["sSound"] = "Sounds.Sounds",
            ["SilencedSound"] = "Sounds.Sounds",
            ["sReloadSound"] = "Sounds.Sounds",
            ["sLocknLoadSound"] = "Sounds.Sounds",
            ["ManualReloadSound"] = "Sounds.Sounds",
            ["sBurstSound"] = "Sounds.BurstSounds",
            ["sSilencedBurstSound"] = "Sounds.BurstSounds",
        };
        foreach (var pair in soundTargets)
        {
            var sound = Assert.Single(weapons.Data, relation => relation.SourceField == pair.Key);
            Assert.Equal(pair.Value, sound.TargetTable.Value);
            Assert.Equal(RecordFields.RecordKey, sound.TargetValueField);
            Assert.Equal(RecordFields.RecordText, sound.TargetDisplayField);
        }

        var comboMerges = await provider.GetRelationsAsync(TableName.Parse("Items.AttachmentComboMerges"));
        Assert.Equal(22, comboMerges.Data!.Count);
        Assert.Contains(comboMerges.Data, relation => relation.SourceField == "usResult");

        var food = await provider.GetRelationsAsync(TableName.Parse("Items.Items"));
        Assert.Contains(food.Data!, relation => relation.TargetTable.Value == "Items.Food");
        Assert.Contains(food.Data!, relation => relation.TargetTable.Value == "Items.Clothes");
        Assert.Contains(food.Data!, relation => relation.TargetTable.Value == "Items.RandomItem");
    }

    /// <summary>
    /// Auxiliary inventory tables declare item pickers: dealer stock, enemy/militia choices, drops, and
    /// every mercenary gear slot.
    /// </summary>
    [Fact]
    public async Task GetRelations_AuxiliaryTables_DeclareItemReferences()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var dealer = await provider.GetRelationsAsync(TableName.Parse("NPCInventory.TonyInventory"));
        Assert.True(dealer.IsSuccess, dealer.ErrorText);
        var stock = Assert.Single(dealer.Data!, relation => relation.Kind == TableRelationKind.Reference);
        Assert.Equal("sItemIndex", stock.SourceField);
        Assert.Equal("Items.Items", stock.TargetTable.Value);
        Assert.Equal("szLongItemName", stock.TargetDisplayField);
        Assert.True(stock.RemapByText);

        var gear = await provider.GetRelationsAsync(TableName.Parse("Inventory.MercStartingGear"));
        Assert.True(gear.IsSuccess, gear.ErrorText);
        Assert.Equal(21, gear.Data!.Count);
        Assert.All(gear.Data, relation =>
        {
            Assert.Equal(TableRelationKind.Reference, relation.Kind);
            Assert.Equal("Items.Items", relation.TargetTable.Value);
        });
        Assert.Contains(gear.Data, relation => relation.SourceField == "mHelmet"
            && relation.Filter!.Operator == RelationFilterOperator.Equals
            && relation.Filter.Value == "2048");
        Assert.Contains(gear.Data, relation => relation.SourceField == "mWeapon" && relation.Filter is null);
        var filteredSlots = new[]
        {
            "mHelmet", "mVest", "mLeg", "lVest", "lLeftThigh", "lRightThigh", "lCPack", "lBPack",
        };
        foreach (var slot in filteredSlots)
        {
            var relation = Assert.Single(gear.Data, relation => relation.SourceField == slot);
            Assert.True(relation.IncludeKeyZero, slot);
        }

        Assert.False(Assert.Single(gear.Data, relation => relation.SourceField == "mWeapon").IncludeKeyZero);

        var choices = await provider.GetRelationsAsync(TableName.Parse("Inventory.GunChoices_Enemy_Admin"));
        Assert.True(choices.IsSuccess, choices.ErrorText);
        Assert.Equal(50, choices.Data!.Count);
        Assert.All(choices.Data, relation => Assert.Equal(RelationFilterOperator.Equals, relation.Filter!.Operator));

        var drops = await provider.GetRelationsAsync(TableName.Parse("Inventory.EnemyAmmoDrops"));
        Assert.True(drops.IsSuccess, drops.ErrorText);
        var ammo = Assert.Single(drops.Data!);
        Assert.Equal("uiType", ammo.SourceField);
        Assert.Equal("Items.AmmoTypes", ammo.TargetTable.Value);
    }

    /// <summary>
    /// The M23b3 relations address positional sound lists, pockets, and explosion data, and the
    /// alternate explosion sound additionally declares its "no sound" value.
    /// </summary>
    [Fact]
    public async Task GetRelations_ValueSources_DeclareTargetsAndNoneValues()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var explosives = await provider.GetRelationsAsync(TableName.Parse("Items.Explosives"));
        Assert.True(explosives.IsSuccess, explosives.ErrorText);
        var animation = Assert.Single(explosives.Data!, relation => relation.SourceField == "ubAnimationID");
        Assert.Equal(TableRelationKind.Reference, animation.Kind);
        Assert.Equal("Items.ExplosionData", animation.TargetTable.Value);
        Assert.Equal("uiIndex", animation.TargetValueField);
        Assert.Equal("name", animation.TargetDisplayField);
        Assert.Null(animation.Filter);
        Assert.True(animation.RemapByText);
        Assert.Equal(40, animation.Ordinal);

        var lbe = await provider.GetRelationsAsync(TableName.Parse("Items.LoadBearingEquipment"));
        Assert.True(lbe.IsSuccess, lbe.ErrorText);
        Assert.Equal(12, lbe.Data!.Count);
        Assert.All(lbe.Data, relation =>
        {
            Assert.Equal(TableRelationKind.Reference, relation.Kind);
            Assert.Equal("Items.Pockets", relation.TargetTable.Value);
            Assert.Equal("pIndex", relation.TargetValueField);
            Assert.Equal("pName", relation.TargetDisplayField);
            Assert.True(relation.RemapByText);
            Assert.Equal(40, relation.Ordinal);
        });
        var pocketFields = lbe.Data.Select(relation => relation.SourceField).ToHashSet(StringComparer.Ordinal);
        for (var index = 1; index <= 12; index++)
        {
            Assert.Contains($"lbePocketIndex{index}", pocketFields);
        }

        var explosionData = await provider.GetRelationsAsync(TableName.Parse("Items.ExplosionData"));
        Assert.True(explosionData.IsSuccess, explosionData.ErrorText);
        Assert.Equal(2, explosionData.Data!.Count);
        var explosionSound = Assert.Single(
            explosionData.Data,
            relation => relation.SourceField == "ExplosionSoundID");
        Assert.Equal("Sounds.Sounds", explosionSound.TargetTable.Value);
        Assert.Equal(RecordFields.RecordKey, explosionSound.TargetValueField);
        Assert.Equal(RecordFields.RecordText, explosionSound.TargetDisplayField);
        Assert.Null(explosionSound.NoneValue);
        Assert.Null(explosionSound.NoneDisplay);

        var alternateSound = Assert.Single(
            explosionData.Data,
            relation => relation.SourceField == "AltExplosionSoundID");
        Assert.Equal("Sounds.Sounds", alternateSound.TargetTable.Value);
        Assert.Equal(RecordFields.RecordKey, alternateSound.TargetValueField);
        Assert.Equal(RecordFields.RecordText, alternateSound.TargetDisplayField);
        Assert.Equal("-1", alternateSound.NoneValue);
        Assert.Equal("None", alternateSound.NoneDisplay);
    }

    /// <summary>
    /// Tables without relations and unknown tables return an empty list, not an error.
    /// </summary>
    [Fact]
    public async Task GetRelations_UnknownTable_ReturnsEmpty()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var unknown = await provider.GetRelationsAsync(TableName.Parse("Missing.Table"));
        var noRelations = await provider.GetRelationsAsync(TableName.Parse("Disease"));

        Assert.True(unknown.IsSuccess, unknown.ErrorText);
        Assert.Empty(unknown.Data!);
        Assert.True(noRelations.IsSuccess, noRelations.ErrorText);
        Assert.Empty(noRelations.Data!);
    }

    /// <summary>
    /// Every seeded relation points at a seeded target table and at fields that the source and target
    /// metadata describe through keys, columns, or hints.
    /// </summary>
    [Fact]
    public async Task SeededRelations_ReferenceKnownTablesAndFields()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var tables = await provider.GetAllTablesAsync();
        Assert.True(tables.IsSuccess, tables.ErrorText);
        var allTables = tables.Data!;
        var byName = allTables.ToDictionary(static table => table.TableName);

        var checkedRelations = 0;
        foreach (var source in allTables)
        {
            var relations = await provider.GetRelationsAsync(source.TableName);
            Assert.True(relations.IsSuccess, relations.ErrorText);
            foreach (var relation in relations.Data!)
            {
                checkedRelations++;
                Assert.True(
                    byName.ContainsKey(relation.TargetTable),
                    $"{source.TableName}: target '{relation.TargetTable}' is not seeded.");
                Assert.True(
                    IsKnownField(source, relation.SourceField),
                    $"{source.TableName}: source field '{relation.SourceField}' is not described by metadata.");

                var target = byName[relation.TargetTable];
                Assert.True(
                    IsKnownField(target, relation.TargetValueField),
                    $"{relation.TargetTable}: target value field '{relation.TargetValueField}' is not described by metadata.");
            }
        }

        Assert.Equal(1042, checkedRelations);
    }

    private static bool IsKnownField(TableMetadata table, string field) =>
        table.TryGetColumn(field, out _)
        || table.TryGetColumnHint(field, out _)
        || string.Equals(table.RecordKey?.Primary.KeyField, field, StringComparison.Ordinal)
        || (string.Equals(field, RecordFields.RecordKey, StringComparison.Ordinal)
            && table.RecordKey?.Primary.Kind == RecordKeyKind.Positional)
        || table.ListFields.Any(list => string.Equals(list.XmlFieldName, field, StringComparison.Ordinal))
        || table.DictFields.Any(dict => string.Equals(dict.XmlFieldMask, field, StringComparison.Ordinal));
}
