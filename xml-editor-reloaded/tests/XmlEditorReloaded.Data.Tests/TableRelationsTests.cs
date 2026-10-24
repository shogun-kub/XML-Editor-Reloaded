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
        Assert.Equal(23, relations.Count);
        Assert.Equal(7, relations.Count(relation => relation.Kind == TableRelationKind.Reference));
        Assert.Equal(5, relations.Count(relation => relation.Kind == TableRelationKind.SubTable));
        Assert.Equal(7, relations.Count(relation => relation.Kind == TableRelationKind.ChildRows));
        Assert.Equal(4, relations.Count(relation => relation.Kind == TableRelationKind.Reverse));

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
        Assert.Equal("Items.AmmoStrings", Assert.Single(weapons.Data!).TargetTable.Value);

        var comboMerges = await provider.GetRelationsAsync(TableName.Parse("Items.AttachmentComboMerges"));
        Assert.Equal(22, comboMerges.Data!.Count);
        Assert.Contains(comboMerges.Data, relation => relation.SourceField == "usResult");

        var food = await provider.GetRelationsAsync(TableName.Parse("Items.Items"));
        Assert.Contains(food.Data!, relation => relation.TargetTable.Value == "Items.Food");
        Assert.Contains(food.Data!, relation => relation.TargetTable.Value == "Items.Clothes");
        Assert.Contains(food.Data!, relation => relation.TargetTable.Value == "Items.RandomItem");
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

        Assert.Equal(84, checkedRelations);
    }

    private static bool IsKnownField(TableMetadata table, string field) =>
        table.TryGetColumn(field, out _)
        || table.TryGetColumnHint(field, out _)
        || string.Equals(table.RecordKey?.Primary.KeyField, field, StringComparison.Ordinal)
        || table.ListFields.Any(list => string.Equals(list.XmlFieldName, field, StringComparison.Ordinal))
        || table.DictFields.Any(dict => string.Equals(dict.XmlFieldMask, field, StringComparison.Ordinal));
}
