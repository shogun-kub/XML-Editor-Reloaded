using Microsoft.Data.Sqlite;
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
    /// A fresh database migrates, seeds 73 tables, and exposes keys, list fields, translatable paths, and languages.
    /// </summary>
    [Fact]
    public async Task MigrateAndRead_MetadataIsAvailable()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        var migrator = new DatabaseMigrator();

        var migration = await migrator.MigrateAsync(databasePath);

        Assert.True(migration.IsSuccess, migration.ErrorText);
        Assert.Equal(5, migration.Data);

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
        Assert.Equal("classification", classHint.GroupKey);
        Assert.True(metadata.TryGetColumnHint("AttachmentClass", out var attachmentHint));
        Assert.Equal("attachments", attachmentHint!.GroupKey);
        Assert.True(metadata.TryGetColumnHint("STAND_MODIFIERS", out var stanceHint));
        Assert.Equal("stance", stanceHint!.GroupKey);
        Assert.True(metadata.TryGetColumnHint("szItemName", out var nameHint));
        Assert.Null(nameHint!.DataType);
        Assert.Equal("identity", nameHint.GroupKey);
        Assert.True(metadata.TryGetColumnHint("DrugType", out var drugHint));
        Assert.Equal(ColumnDataType.Flags, drugHint!.DataType);
        Assert.Equal(ColumnEditorKind.FlagEditor, drugHint.EditorKind);
        Assert.True(metadata.TryGetColumnHint("DefaultAttachment", out var defaultAttachmentHint));
        Assert.Equal(ColumnDataType.Integer, defaultAttachmentHint!.DataType);
        Assert.True(metadata.TryGetColumnHint("AvailableAttachmentPoint", out var availablePointHint));
        Assert.Equal(ColumnDataType.Flags, availablePointHint!.DataType);
        Assert.True(metadata.TryGetColumnHint("Inseparable", out var inseparableHint));
        Assert.Equal(ColumnDataType.Lookup, inseparableHint!.DataType);
        Assert.Equal("Lookup.Separability", inseparableHint.LookupTable);
        Assert.Empty(metadata.Columns);

        var flagLookup = await provider.GetTableAsync(TableName.Parse("Lookup.ItemFlag"));
        Assert.True(flagLookup.IsSuccess, flagLookup.ErrorText);
        Assert.Equal(2, flagLookup.Data!.Columns.Count);
        Assert.True(flagLookup.Data.TryGetColumn("id", out var flagIdColumn));
        Assert.Equal(ColumnDataType.Raw, flagIdColumn!.DataType);

        var disease = await provider.GetTableAsync(TableName.Parse("Disease"));
        Assert.True(disease.IsSuccess, disease.ErrorText);
        Assert.True(disease.Data!.TryGetColumnHint("InfectionChance_BADFOOD", out var chanceHint));
        Assert.Equal(ColumnDataType.Decimal, chanceHint!.DataType);

        var languages = await provider.GetLanguagesAsync();
        Assert.True(languages.IsSuccess, languages.ErrorText);
        var language = Assert.Single(languages.Data!);
        Assert.Equal("ru", language.Code);
        Assert.Equal("Russian", language.OverlayPrefix);
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
