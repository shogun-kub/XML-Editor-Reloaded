using Microsoft.Data.Sqlite;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Metadata;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Tests for migrations and <see cref="SqliteMetadataProvider"/>.
/// </summary>
public class MetadataTests
{
    /// <summary>
    /// A fresh database migrates, seeds the mapped and auxiliary tables, and exposes keys, list fields,
    /// translatable paths, and languages.
    /// </summary>
    [Fact]
    public async Task MigrateAndRead_MetadataIsAvailable()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        var migrator = new DatabaseMigrator();

        var migration = await migrator.MigrateAsync(databasePath);

        Assert.True(migration.IsSuccess, migration.ErrorText);
        Assert.Equal(16, migration.Data);

        var provider = new SqliteMetadataProvider(databasePath);
        var tables = await provider.GetAllTablesAsync();
        Assert.True(tables.IsSuccess, tables.ErrorText);
        Assert.True(tables.Data!.Count >= 73, $"Expected at least 73 tables, got {tables.Data.Count}.");

        var items = await provider.GetTableAsync(TableName.Parse("Items.Items"));
        Assert.True(items.IsSuccess, items.ErrorText);
        var metadata = items.Data!;
        Assert.Equal("ITEMLIST", metadata.XmlRoot);
        Assert.Equal("ITEM", metadata.RecordElement);
        Assert.NotNull(metadata.RecordKey);
        Assert.Equal("uiIndex", metadata.RecordKey!.Primary.KeyField);
        Assert.Equal(2, metadata.ListFields.Count);
        Assert.Equal(5, metadata.TranslatableFieldPaths.Count);
        Assert.Contains("szItemName", metadata.TranslatableFieldPaths);
        Assert.Equal(224, metadata.ColumnHints.Count);
        Assert.True(metadata.TryGetColumnHint("usItemClass", out var classHint));
        Assert.Equal(ColumnDataType.Flags, classHint!.DataType);
        Assert.Equal(ColumnEditorKind.FlagEditor, classHint.EditorKind);
        Assert.Equal("Lookup.ItemClass", classHint.LookupTable);
        Assert.Equal("classification", classHint.GroupKey);
        Assert.True(metadata.TryGetColumnHint("AttachmentClass", out var attachmentHint));
        Assert.Equal("Lookup.AttachmentClass", attachmentHint!.LookupTable);
        Assert.Equal("attachments", attachmentHint.GroupKey);
        Assert.True(metadata.TryGetColumnHint("STAND_MODIFIERS", out var stanceHint));
        Assert.Equal("stance", stanceHint!.GroupKey);
        Assert.True(metadata.TryGetColumnHint("szItemName", out var nameHint));
        Assert.Null(nameHint!.DataType);
        Assert.Equal("identity", nameHint.GroupKey);
        Assert.True(metadata.TryGetColumnHint("usOverheatingCooldownFactor", out var overheatHint));
        Assert.Equal("overheating", overheatHint!.GroupKey);
        Assert.True(metadata.TryGetColumnHint("sBackpackWeightModifier", out var pocketsHint));
        Assert.Equal("pockets", pocketsHint!.GroupKey);
        Assert.Equal(25, metadata.ClassVisibilityRules.Count);
        Assert.True(metadata.TryGetClassVisibilityRule(
            ClassVisibilitySectionKind.Group,
            "combat",
            out var combatRule));
        Assert.Equal(268484606, combatRule!.VisibleClasses);
        Assert.True(combatRule.IsVisible(2));
        Assert.False(combatRule.IsVisible(65536));
        Assert.True(metadata.TryGetClassVisibilityRule(
            ClassVisibilitySectionKind.SubTable,
            "Items.Magazines",
            out var magazineRule));
        Assert.True(magazineRule!.IsVisible(1024));
        Assert.False(magazineRule.IsVisible(2));
        Assert.True(metadata.TryGetClassVisibilityRule(
            ClassVisibilitySectionKind.Group,
            "identity",
            out var identityRule));
        Assert.True(identityRule!.IsVisible(0));
        Assert.True(metadata.TryGetColumnHint("DrugType", out var drugHint));
        Assert.Equal(ColumnDataType.Flags, drugHint!.DataType);
        Assert.Equal(ColumnEditorKind.FlagEditor, drugHint.EditorKind);
        Assert.Equal("Lookup.DrugType", drugHint.LookupTable);
        Assert.True(metadata.TryGetColumnHint("DefaultAttachment", out var defaultAttachmentHint));
        Assert.Equal(ColumnDataType.Integer, defaultAttachmentHint!.DataType);
        Assert.True(metadata.TryGetColumnHint("AvailableAttachmentPoint", out var availablePointHint));
        Assert.Equal(ColumnDataType.Flags, availablePointHint!.DataType);
        Assert.Equal("Lookup.AttachmentPoint", availablePointHint.LookupTable);
        Assert.True(metadata.TryGetColumnHint("Inseparable", out var inseparableHint));
        Assert.Equal(ColumnDataType.Lookup, inseparableHint!.DataType);
        Assert.Equal("Lookup.Separability", inseparableHint.LookupTable);
        Assert.True(metadata.TryGetColumnHint("RobotTargetingSkillGrant", out var robotTraitHint));
        Assert.Equal(ColumnDataType.Lookup, robotTraitHint!.DataType);
        Assert.Equal(ColumnEditorKind.LookupCombo, robotTraitHint.EditorKind);
        Assert.Equal("Lookup.SkillTrait", robotTraitHint.LookupTable);
        Assert.Empty(metadata.Columns);

        Assert.True(metadata.TryGetColumnHint("ItemSize", out var itemSizeHint));
        Assert.Equal(0L, itemSizeHint!.Minimum);
        Assert.Equal(99L, itemSizeHint.Maximum);
        Assert.Equal(61, metadata.ColumnHints.Count(hint => hint.Maximum is not null));
        var launcherGroup = Assert.Single(metadata.ExclusiveFlagGroups);
        Assert.Equal("launcher_type", launcherGroup.GroupKey);
        Assert.Equal(
            new[] { "Cannon", "Mortar", "GrenadeLauncher", "RocketLauncher" },
            launcherGroup.FieldNames);
        Assert.Empty(metadata.ClassValueLimits);
        Assert.Equal(7, metadata.TextLimits.Count);

        var gear = await provider.GetTableAsync(TableName.Parse("Inventory.MercStartingGear"));
        Assert.True(gear.IsSuccess, gear.ErrorText);
        Assert.True(gear.Data!.TryGetColumnHint("mSmall1Status", out var statusHint));
        Assert.Equal(0L, statusHint!.Minimum);
        Assert.Equal(100L, statusHint.Maximum);
        Assert.True(gear.Data.TryGetColumnHint("mBig0Quantity", out var quantityHint));
        Assert.Equal(0L, quantityHint!.Minimum);
        Assert.Equal(10L, quantityHint.Maximum);
        Assert.True(gear.Data.TryGetColumnHint("mSmall2Quantity", out var smallQuantityHint));
        Assert.Equal(10L, smallQuantityHint!.Maximum);
        Assert.True(gear.Data.TryGetColumnHint("mSmall0Quantity", out var firstSmallQuantityHint));
        Assert.Equal(100L, firstSmallQuantityHint!.Maximum);
        Assert.True(gear.Data.TryGetColumnHint("mPriceMod", out var priceModHint));
        Assert.Equal(-32000L, priceModHint!.Minimum);
        Assert.Equal(32000L, priceModHint.Maximum);
        Assert.True(gear.Data.TryGetColumnHint("mAbsolutePrice", out var absolutePriceHint));
        Assert.Equal(-1L, absolutePriceHint!.Minimum);
        Assert.Equal(32000L, absolutePriceHint.Maximum);
        Assert.Equal(35, gear.Data.ColumnHints.Count(hint => hint.Maximum is not null));

        var weapons = await provider.GetTableAsync(TableName.Parse("Items.Weapons"));
        Assert.True(weapons.IsSuccess, weapons.ErrorText);
        Assert.True(weapons.Data!.TryGetColumnHint("usOverheatingJamThreshold", out var jamHint));
        Assert.Equal(60000L, jamHint!.Maximum);
        Assert.True(weapons.Data.TryGetColumnHint("bAccuracy", out var accuracyHint));
        Assert.Equal(-100L, accuracyHint!.Minimum);
        Assert.Equal(14, weapons.Data.ColumnHints.Count(hint => hint.Maximum is not null));

        var explosives = await provider.GetTableAsync(TableName.Parse("Items.Explosives"));
        Assert.True(explosives.IsSuccess, explosives.ErrorText);
        Assert.True(explosives.Data!.TryGetColumnHint("bIndoorModifier", out var indoorHint));
        Assert.Equal(-1L, indoorHint!.Minimum);
        Assert.Equal(10L, indoorHint.Maximum);
        Assert.Equal(8, explosives.Data.ColumnHints.Count(hint => hint.Maximum is not null));

        var magazines = await provider.GetTableAsync(TableName.Parse("Items.Magazines"));
        Assert.True(magazines.IsSuccess, magazines.ErrorText);
        Assert.Equal(1, magazines.Data!.ColumnHints.Count(hint => hint.Maximum is not null));

        var lbe = await provider.GetTableAsync(TableName.Parse("Items.LoadBearingEquipment"));
        Assert.True(lbe.IsSuccess, lbe.ErrorText);
        Assert.Equal(5, lbe.Data!.ClassValueLimits.Count);
        var lbeClassOne = Assert.Single(lbe.Data.ClassValueLimits, limit => limit.ClassValue == "1");
        Assert.Equal("lbeClass", lbeClassOne.ClassField);
        Assert.Equal("lbePocketIndex%Index%", lbeClassOne.FieldMask);
        Assert.Equal(5, lbeClassOne.MaxOrdinal);
        Assert.Contains(lbe.Data.ClassValueLimits, limit => limit.ClassValue == "3" && limit.MaxOrdinal == 7);
        Assert.Contains(lbe.Data.ClassValueLimits, limit => limit.ClassValue == "0" && limit.MaxOrdinal == 0);
        Assert.True(lbe.Data.TryGetColumnHint("lbePocketsAvailable", out var lbePocketsHint));
        Assert.Equal(4095L, lbePocketsHint!.Maximum);
        Assert.Equal(4, lbe.Data.ColumnHints.Count(hint => hint.Maximum is not null));

        var flagLookup = await provider.GetTableAsync(TableName.Parse("Lookup.ItemFlag"));
        Assert.True(flagLookup.IsSuccess, flagLookup.ErrorText);
        Assert.Equal(2, flagLookup.Data!.Columns.Count);
        Assert.True(flagLookup.Data.TryGetColumn("id", out var flagIdColumn));
        Assert.Equal(ColumnDataType.Raw, flagIdColumn!.DataType);

        var disease = await provider.GetTableAsync(TableName.Parse("Disease"));
        Assert.True(disease.IsSuccess, disease.ErrorText);
        Assert.True(disease.Data!.TryGetColumnHint("InfectionChance_BADFOOD", out var chanceHint));
        Assert.Equal(ColumnDataType.Decimal, chanceHint!.DataType);

        var slots = await provider.GetTableAsync(TableName.Parse("Items.AttachmentSlots"));
        Assert.True(slots.IsSuccess, slots.ErrorText);
        Assert.True(slots.Data!.TryGetColumnHint("nasAttachmentClass", out var slotClassHint));
        Assert.Equal("Lookup.NasAttachmentClass", slotClassHint!.LookupTable);
        Assert.True(slots.Data.TryGetColumnHint("nasLayoutClass", out var slotLayoutHint));
        Assert.Equal("Lookup.NasLayoutClass", slotLayoutHint!.LookupTable);

        var pockets = await provider.GetTableAsync(TableName.Parse("Items.Pockets"));
        Assert.True(pockets.IsSuccess, pockets.ErrorText);
        Assert.True(pockets.Data!.TryGetColumnHint("pRestriction", out var restrictionHint));
        Assert.Equal("Lookup.ItemClass", restrictionHint!.LookupTable);

        var incompatible = await provider.GetTableAsync(TableName.Parse("Items.IncompatibleAttachments"));
        Assert.True(incompatible.IsSuccess, incompatible.ErrorText);
        Assert.NotNull(incompatible.Data!.SymmetricKeyPair);
        Assert.Equal("itemIndex", incompatible.Data.SymmetricKeyPair!.FirstField);
        Assert.Equal("incompatibleattachmentIndex", incompatible.Data.SymmetricKeyPair.SecondField);

        var compatibleFaces = await provider.GetTableAsync(TableName.Parse("Items.CompatibleFaceItems"));
        Assert.True(compatibleFaces.IsSuccess, compatibleFaces.ErrorText);
        Assert.NotNull(compatibleFaces.Data!.SymmetricKeyPair);
        Assert.Equal("compatiblefaceitemIndex", compatibleFaces.Data.SymmetricKeyPair!.FirstField);
        Assert.Equal("itemIndex", compatibleFaces.Data.SymmetricKeyPair.SecondField);
        Assert.Null(metadata.SymmetricKeyPair);

        var languages = await provider.GetLanguagesAsync();
        Assert.True(languages.IsSuccess, languages.ErrorText);
        Assert.Equal(7, languages.Data!.Count);
        var russian = languages.Data[0];
        Assert.Equal("ru", russian.Code);
        Assert.Equal("Russian", russian.OverlayPrefix);
        Assert.Equal(
            new[] { "de", "pl", "fr", "it", "nl", "zh" },
            languages.Data.Skip(1).Select(static language => language.Code));
    }

    /// <summary>
    /// Auxiliary M20 tables declare their real roots, record elements, keys, hints, and item pickers,
    /// and prototype-only mappings without files are absent.
    /// </summary>
    [Fact]
    public async Task AuxiliaryTables_DeclareKeysHintsAndLanguages()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var gear = await provider.GetTableAsync(TableName.Parse("Inventory.MercStartingGear"));
        Assert.True(gear.IsSuccess, gear.ErrorText);
        Assert.True(gear.Data!.TryGetColumnHint("mHelmet", out var helmetHint));
        Assert.Equal(ColumnDataType.Integer, helmetHint!.DataType);
        Assert.True(gear.Data.TryGetColumnHint("mGearKitName", out var kitNameHint));
        Assert.Equal(ColumnDataType.String, kitNameHint!.DataType);
        Assert.Contains("GEARKIT/mGearKitName", gear.Data.TranslatableFieldPaths);

        var merchant = await provider.GetTableAsync(TableName.Parse("NPCInventory.Merchants"));
        Assert.True(merchant.IsSuccess, merchant.ErrorText);
        Assert.Equal("MERCHANTSLIST", merchant.Data!.XmlRoot);
        Assert.Equal("MERCHANT", merchant.Data.RecordElement);
        Assert.Equal("uiIndex", merchant.Data.RecordKey!.Primary.KeyField);
        Assert.True(merchant.Data.TryGetColumnHint("uiFlags", out var flagsHint));
        Assert.Equal(ColumnDataType.Flags, flagsHint!.DataType);
        Assert.True(merchant.Data.TryGetColumnHint("dBuyModifier", out var buyHint));
        Assert.Equal(ColumnDataType.Decimal, buyHint!.DataType);

        var dealer = await provider.GetTableAsync(TableName.Parse("NPCInventory.AdditionalDealer_60_Inventory"));
        Assert.True(dealer.IsSuccess, dealer.ErrorText);
        Assert.Equal("INVENTORYLIST", dealer.Data!.XmlRoot);
        Assert.Equal("INVENTORY", dealer.Data.RecordElement);
        Assert.Equal("uiIndex", dealer.Data.RecordKey!.Primary.KeyField);
        Assert.True(dealer.Data.TryGetColumnHint("sItemIndex", out var itemHint));
        Assert.Equal(ColumnDataType.Integer, itemHint!.DataType);
        Assert.True(dealer.Data.TryGetColumnHint("ubOptimalNumber", out var quantityHint));
        Assert.Equal(ColumnDataType.Integer, quantityHint!.DataType);

        var sounds = await provider.GetTableAsync(TableName.Parse("Sounds.Sounds"));
        Assert.True(sounds.IsSuccess, sounds.ErrorText);
        Assert.Equal("SOUNDLIST", sounds.Data!.XmlRoot);
        Assert.Equal("SOUND", sounds.Data.RecordElement);
        Assert.Equal(RecordKeyKind.Positional, sounds.Data.RecordKey!.Primary.Kind);
        Assert.Null(sounds.Data.RecordKey.Primary.KeyField);

        var choices = await provider.GetTableAsync(TableName.Parse("Inventory.ItemChoices_Militia_Green"));
        Assert.True(choices.IsSuccess, choices.ErrorText);
        Assert.Equal("ENEMYITEMCHOICESLIST", choices.Data!.XmlRoot);
        Assert.Equal("ENEMYITEMCHOICES", choices.Data.RecordElement);
        Assert.Equal("uiIndex", choices.Data.RecordKey!.Primary.KeyField);

        var removed = await provider.GetTableAsync(TableName.Parse("Items.Keys"));
        Assert.False(removed.IsSuccess);
        Assert.Equal(ErrorCodes.MetadataNotFound, removed.ErrorCode);
    }

    /// <summary>
    /// Nested and positional overlay keys are read from the seed.
    /// </summary>
    [Fact]
    public async Task NestedKeys_AreRead()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var gear = await provider.GetTableAsync(TableName.Parse("Inventory.MercStartingGear"));
        Assert.True(gear.IsSuccess, gear.ErrorText);
        Assert.Equal(2, gear.Data!.RecordKey!.Segments.Count);
        Assert.Equal(RecordKeyKind.Scalar, gear.Data.RecordKey.Primary.Kind);
        Assert.Equal(RecordKeyKind.Positional, gear.Data.RecordKey.Segments[1].Kind);

        var cities = await provider.GetTableAsync(TableName.Parse("Map.Cities"));
        Assert.True(cities.IsSuccess, cities.ErrorText);
        Assert.Equal("CITYLIST/CITY", cities.Data!.RecordKey!.Primary.Path);
        Assert.Equal("uiIndex", cities.Data.RecordKey.Primary.KeyField);
    }

    /// <summary>
    /// The eight natural composite keys are read in field order, their old single-field keys are gone,
    /// and compound-key tables never declare translatable fields.
    /// </summary>
    [Fact]
    public async Task CompoundRecordKeys_ReplaceSingleKeys()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var expected = new Dictionary<string, string[]>
        {
            ["Items.Attachments"] = new[] { "attachmentIndex", "itemIndex" },
            ["Items.CompatibleFaceItems"] = new[] { "compatiblefaceitemIndex", "itemIndex" },
            ["Items.IncompatibleAttachments"] = new[] { "itemIndex", "incompatibleattachmentIndex" },
            ["Items.Item_Transformations"] = new[]
            {
                "usItem", "usResult1", "usResult2", "usResult3", "usResult4", "usResult5",
                "usResult6", "usResult7", "usResult8", "usResult9", "usResult10",
            },
            ["Items.Launchables"] = new[] { "launchableIndex", "itemIndex" },
            ["Items.Merges"] = new[]
            {
                "firstItemIndex", "secondItemIndex", "firstResultingItemIndex", "secondResultingItemIndex",
            },
            ["Items.StructureConstruct"] = new[] { "usCreationItem", "usItemStatusLoss" },
            ["Items.StructureDeconstruct"] = new[] { "usDeconstructItem", "usItemToCreate", "usCreatedItemStatus" },
        };

        foreach (var pair in expected)
        {
            var metadata = await provider.GetTableAsync(TableName.Parse(pair.Key));
            Assert.True(metadata.IsSuccess, metadata.ErrorText);
            Assert.Equal(pair.Value, metadata.Data!.RecordKeyFields);
            Assert.True(metadata.Data.HasCompoundRecordKey, pair.Key);
            Assert.True(metadata.Data.HasRecordKey, pair.Key);
            Assert.Null(metadata.Data.RecordKey);
            Assert.Empty(metadata.Data.TranslatableFieldPaths);
        }

        var items = await provider.GetTableAsync(TableName.Parse("Items.Items"));
        Assert.True(items.IsSuccess, items.ErrorText);
        Assert.Empty(items.Data!.RecordKeyFields);
        Assert.Equal("uiIndex", items.Data.RecordKey!.Primary.KeyField);

        await using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT COUNT(*)
            FROM TableKeys k
            INNER JOIN Tables t ON t.Id = k.TableId
            WHERE t.XmlTable IN (
                'Items.Attachments', 'Items.CompatibleFaceItems', 'Items.IncompatibleAttachments',
                'Items.Item_Transformations', 'Items.Launchables', 'Items.Merges',
                'Items.StructureConstruct', 'Items.StructureDeconstruct');
            """;
        Assert.Equal(0L, (long)(await command.ExecuteScalarAsync())!);
    }

    /// <summary>
    /// The curated SkillTrait lookup returns the fixed robot skill values in game enum order.
    /// </summary>
    [Fact]
    public async Task GetLookupValues_SkillTrait_ReturnsSeededValues()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var result = await provider.GetLookupValuesAsync(TableName.Parse("Lookup.SkillTrait"));

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(24, result.Data!.Count);
        Assert.Equal(new LookupEntry(0, "NoTrait"), result.Data[0]);
        Assert.Equal(new LookupEntry(23, "Survival"), result.Data[^1]);
    }

    /// <summary>
    /// Running migrations twice is idempotent.
    /// </summary>
    [Fact]
    public async Task Migrate_SecondRunAppliesNothing()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        var migrator = new DatabaseMigrator();

        var first = await migrator.MigrateAsync(databasePath);
        var second = await migrator.MigrateAsync(databasePath);

        Assert.True(first.IsSuccess, first.ErrorText);
        Assert.True(second.IsSuccess, second.ErrorText);
        Assert.Equal(0, second.Data);
    }

    /// <summary>
    /// Item text limits describe the legacy description counters, the name limits without counters, and
    /// the Chinese description overrides; any other language falls back to the neutral limit.
    /// </summary>
    [Fact]
    public async Task ItemTextLimits_DeclareDescriptionCountersAndChineseOverrides()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var items = await provider.GetTableAsync(TableName.Parse("Items.Items"));
        Assert.True(items.IsSuccess, items.ErrorText);
        var metadata = items.Data!;

        Assert.True(metadata.TryGetTextLimit("szItemDesc", null, out var description));
        Assert.Equal(400, description!.MaxLength);
        Assert.True(description.ShowCounter);
        Assert.True(metadata.TryGetTextLimit("szBRDesc", null, out var brDescription));
        Assert.Equal(400, brDescription!.MaxLength);
        Assert.True(brDescription.ShowCounter);

        Assert.True(metadata.TryGetTextLimit("szItemName", null, out var name));
        Assert.Equal(80, name!.MaxLength);
        Assert.False(name.ShowCounter);
        Assert.True(metadata.TryGetTextLimit("szLongItemName", null, out var longName));
        Assert.Equal(80, longName!.MaxLength);
        Assert.False(longName.ShowCounter);
        Assert.True(metadata.TryGetTextLimit("szBRName", null, out var brName));
        Assert.Equal(80, brName!.MaxLength);
        Assert.False(brName.ShowCounter);

        Assert.True(metadata.TryGetTextLimit("szItemDesc", "zh", out var chineseDescription));
        Assert.Equal(63, chineseDescription!.MaxLength);
        Assert.True(chineseDescription.ShowCounter);
        Assert.True(metadata.TryGetTextLimit("szBRDesc", "zh", out var chineseBrDescription));
        Assert.Equal(90, chineseBrDescription!.MaxLength);
        Assert.True(chineseBrDescription.ShowCounter);

        Assert.True(metadata.TryGetTextLimit("szItemDesc", "ru", out var russianDescription));
        Assert.Equal(400, russianDescription!.MaxLength);
        Assert.False(metadata.TryGetTextLimit("usItemClass", null, out _));
    }

    /// <summary>
    /// Missing tables return <c>metadata.not_found</c>.
    /// </summary>
    [Fact]
    public async Task GetTable_Missing_ReturnsMetadataNotFound()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var result = await provider.GetTableAsync(TableName.Parse("Items.DoesNotExist"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.MetadataNotFound, result.ErrorCode);
    }

    /// <summary>
    /// A database created by a newer application version is not silently downgraded.
    /// </summary>
    [Fact]
    public async Task Migrate_NewerSchema_ReturnsDowngradeUnsupported()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);

        await using (var connection = new SqliteConnection($"Data Source={databasePath}"))
        {
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO SchemaMigrations (Version, Name, AppliedAtUtc) VALUES (99, 'Future', '2026-01-01T00:00:00Z');";
            await command.ExecuteNonQueryAsync();
        }

        var result = await new DatabaseMigrator().MigrateAsync(databasePath);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.MetadataDowngradeUnsupported, result.ErrorCode);
    }
}
