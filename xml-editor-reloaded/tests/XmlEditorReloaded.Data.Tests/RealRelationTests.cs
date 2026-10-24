using System.Globalization;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Relations;
using XmlEditorReloaded.Xml.Repository;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Environment-gated relation checks against a real game <c>TableData</c> directory.
/// Set <c>XER_CORPUS_DIR</c> to a v4 or v5 TableData directory to enable them.
/// </summary>
public class RealRelationTests
{
    /// <summary>
    /// Every seeded main-table reference resolves against the target document on both reference data
    /// sets, and resolving relations never rewrites a table file.
    /// </summary>
    [Fact]
    public async Task SeededReferences_ResolveOnRealData()
    {
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        var before = SnapshotFiles(tableData);
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var repository = new TableRepository(provider);
        var resolver = new TableRelationResolver(catalog, repository, tableData, provider);

        var descriptors = await catalog.GetTablesAsync(tableData);
        Assert.True(descriptors.IsSuccess, descriptors.ErrorText);
        var byName = descriptors.Data!.ToDictionary(static descriptor => descriptor.TableName);
        var tables = await provider.GetAllTablesAsync();
        Assert.True(tables.IsSuccess, tables.ErrorText);

        var failures = new List<string>();
        var checkedReferences = 0;
        foreach (var source in tables.Data!)
        {
            var relations = await provider.GetRelationsAsync(source.TableName);
            if (!relations.IsSuccess)
            {
                failures.Add($"{source.TableName}: relations failed: {relations.ErrorText}");
                continue;
            }

            var references = relations.Data!
                .Where(relation => relation.Kind == TableRelationKind.Reference)
                .ToArray();
            if (references.Length == 0 || !byName.TryGetValue(source.TableName, out var descriptor))
            {
                continue;
            }

            var load = await repository.LoadTableAsync(descriptor);
            if (!load.IsSuccess || load.Data!.Status != TableDocumentStatus.Loaded)
            {
                failures.Add($"{source.TableName}: source document could not be loaded.");
                continue;
            }

            var document = load.Data.GetDocument<TableDocument>();
            if (document is null)
            {
                failures.Add($"{source.TableName}: no document returned.");
                continue;
            }

            foreach (var relation in references)
            {
                if (!byName.ContainsKey(relation.TargetTable))
                {
                    failures.Add($"{source.TableName}.{relation.SourceField}: target '{relation.TargetTable}' is missing.");
                    continue;
                }

                var target = await resolver.ResolveAsync(relation);
                if (!target.IsSuccess)
                {
                    failures.Add(
                        $"{source.TableName}.{relation.SourceField}: target '{relation.TargetTable}' failed: "
                        + $"{target.ErrorCode} {target.ErrorText}");
                    continue;
                }

                checkedReferences++;
                foreach (var record in document.Records)
                {
                    var value = record.GetRawValue(relation.SourceField)?.Trim();
                    if (!string.IsNullOrEmpty(value) && !target.Data!.Contains(value))
                    {
                        failures.Add(
                            $"{source.TableName}.{relation.SourceField}='{value}' (record {record.GetKey()}) "
                            + $"is missing in '{relation.TargetTable}'.");
                    }
                }
            }
        }

        Assert.True(checkedReferences >= 60, $"Only {checkedReferences} reference relations were checked.");
        Assert.True(
            failures.Count == 0,
            $"Unresolved relations ({failures.Count}):{Environment.NewLine}{string.Join(Environment.NewLine, failures.Take(25))}");
        Assert.Equal(before, SnapshotFiles(tableData));
    }

    /// <summary>
    /// Each item 1:1 sub-table contains the row referenced by the item's key field on both reference
    /// data sets: weapons by <c>uiIndex</c>, other sub-tables by <c>ubClassIndex</c>.
    /// </summary>
    [Fact]
    public async Task ItemSubTables_CoverEveryClassItem()
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
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var repository = new TableRepository(provider);
        var resolver = new TableRelationResolver(catalog, repository, tableData, provider);
        var descriptors = await catalog.GetTablesAsync(tableData);
        Assert.True(descriptors.IsSuccess, descriptors.ErrorText);
        var byName = descriptors.Data!.ToDictionary(static descriptor => descriptor.TableName);

        var load = await repository.LoadTableAsync(byName[TableName.Parse("Items.Items")]);
        Assert.True(load.IsSuccess, load.ErrorText);
        var items = load.Data!.GetDocument<TableDocument>()!;
        var relations = await provider.GetRelationsAsync(TableName.Parse("Items.Items"));
        Assert.True(relations.IsSuccess, relations.ErrorText);
        var subTables = relations.Data!
            .Where(relation => relation.Kind == TableRelationKind.SubTable)
            .ToArray();
        Assert.Equal(5, subTables.Length);

        var classMasks = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            ["Items.Weapons"] = 2 | 4 | 8 | 16 | 32 | 64 | 128,
            ["Items.Magazines"] = 1024,
            ["Items.Armours"] = 2048,
            ["Items.Explosives"] = 256 | 512,
            ["Items.LoadBearingEquipment"] = 131072,
        };
        var failures = new List<string>();
        var itemsWithoutKey = 0;
        foreach (var relation in subTables)
        {
            var target = await resolver.ResolveAsync(relation);
            Assert.True(target.IsSuccess, $"{relation.TargetTable}: {target.ErrorText}");
            var mask = classMasks[relation.TargetTable.Value];
            foreach (var record in items.Records)
            {
                var classText = record.GetRawValue("usItemClass");
                if (!int.TryParse(classText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var itemClass)
                    || (itemClass & mask) == 0)
                {
                    continue;
                }

                var key = record.GetRawValue(relation.SourceField)?.Trim();
                if (string.IsNullOrEmpty(key))
                {
                    // A handful of placeholder items ("none" ammo/armour entries) have no class
                    // index at all; they legitimately have no sub-table row.
                    itemsWithoutKey++;
                    continue;
                }

                if (!target.Data!.Contains(key))
                {
                    failures.Add(
                        $"{relation.TargetTable}: item {record.GetKey()} has no row for "
                        + $"{relation.SourceField}='{key}'.");
                }
            }
        }

        Assert.InRange(itemsWithoutKey, 0, 10);
        Assert.True(
            failures.Count == 0,
            $"Missing sub-table rows ({failures.Count}):{Environment.NewLine}{string.Join(Environment.NewLine, failures.Take(25))}");
    }

    private static string[] SnapshotFiles(string directory) =>
        Directory
            .EnumerateFiles(directory, "*.xml", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(static path =>
            {
                var info = new FileInfo(path);
                return $"{path}|{info.Length}|{info.LastWriteTimeUtc.Ticks}";
            })
            .ToArray();
}
