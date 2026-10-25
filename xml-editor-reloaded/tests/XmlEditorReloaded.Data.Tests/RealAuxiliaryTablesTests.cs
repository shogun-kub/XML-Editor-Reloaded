using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Lookups;
using XmlEditorReloaded.Xml.Overlays;
using XmlEditorReloaded.Xml.Repository;
using XmlEditorReloaded.Xml.Schema;
using XmlEditorReloaded.Xml.Validation;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Environment-gated M20 acceptance checks against a real game <c>TableData</c> directory.
/// Set <c>XER_CORPUS_DIR</c> to a v4 or v5 TableData directory to enable them.
/// </summary>
public class RealAuxiliaryTablesTests
{
    private static readonly string[] AuxiliaryPrefixes =
    {
        "Inventory.",
        "NPCInventory.",
        "Sounds.",
    };

    /// <summary>
    /// Every auxiliary table exists in both reference data sets, opens with its declared key, and
    /// validates without false type or lookup warnings.
    /// </summary>
    [Fact]
    public async Task AuxiliaryTables_OpenWithKeysAndValidateCleanly()
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
        var tables = await provider.GetAllTablesAsync();
        Assert.True(tables.IsSuccess, tables.ErrorText);
        var languages = await provider.GetLanguagesAsync();
        Assert.True(languages.IsSuccess, languages.ErrorText);
        var lookups = new XmlFileLookupRepository(tableData, languages.Data!, provider);
        var loader = new TableFileLoader();
        var validator = new TableValidator();
        var failures = new List<string>();
        var checkedTables = 0;
        var knownDuplicateKeys = 0;
        var knownRangeWarnings = 0;

        foreach (var metadata in tables.Data!.Where(static table =>
                     AuxiliaryPrefixes.Any(prefix => table.TableName.Value.StartsWith(
                         prefix,
                         StringComparison.Ordinal))))
        {
            var relativePath = metadata.TableName.ToRelativePath();
            var filePath = Path.Combine(tableData, relativePath);
            if (!File.Exists(filePath))
            {
                failures.Add($"{relativePath}: file is missing.");
                continue;
            }

            var descriptor = new TableDescriptor(
                metadata.TableName,
                filePath,
                relativePath,
                IsMapped: true,
                Array.Empty<OverlayDescriptor>());
            var load = await loader.LoadAsync(descriptor, metadata);
            if (!load.IsSuccess || load.Data!.Status != TableDocumentStatus.Loaded)
            {
                failures.Add($"{relativePath}: {load.ErrorCode} {load.ErrorText}");
                continue;
            }

            var document = load.Data.GetDocument<TableDocument>();
            if (document is null || document.Records.Count == 0)
            {
                failures.Add($"{relativePath}: no records were loaded.");
                continue;
            }

            Assert.NotNull(metadata.RecordKey);
            var schema = new TableSchemaBuilder().Build(document);
            if (metadata.RecordKey!.Primary.Kind == RecordKeyKind.Scalar)
            {
                var keyField = metadata.RecordKey.Primary.KeyField;
                Assert.False(string.IsNullOrEmpty(keyField));
                if (schema.FindColumn(keyField!) is null)
                {
                    failures.Add($"{relativePath}: key column '{keyField}' was not discovered.");
                }
            }
            else
            {
                if (schema.FindColumn(RecordFields.RecordText) is null)
                {
                    failures.Add($"{relativePath}: text-only record column was not discovered.");
                }
            }

            var report = await validator.ValidateAsync(document, lookups);
            foreach (var issue in report.Issues)
            {
                // Both reference data sets contain one duplicated uiIndex in this dealer file. The
                // editor reports it and must never renumber the user's data.
                if (issue.Code == ValidationCodes.DuplicateKey
                    && relativePath == "NPCInventory/AdditionalDealer_43_Inventory.xml")
                {
                    knownDuplicateKeys++;
                    continue;
                }

                // Both reference data sets contain one gear status above the legacy 0..100 spinner
                // limit. The editor keeps the value, reports it, and never clamps it.
                if (issue.Code == ValidationCodes.OutOfRange
                    && relativePath == "Inventory/MercStartingGear.xml"
                    && issue.FieldPath == "GEARKIT/mSmall1Status"
                    && issue.RecordKey == "237")
                {
                    knownRangeWarnings++;
                    continue;
                }

                failures.Add($"{relativePath}: {issue.Code} at {issue.FieldPath}: {issue.Message}");
            }

            checkedTables++;
        }

        Assert.Equal(1, knownDuplicateKeys);
        Assert.Equal(1, knownRangeWarnings);
        Assert.Equal(104, checkedTables);
        Assert.True(
            failures.Count == 0,
            $"Auxiliary table failures ({failures.Count}):{Environment.NewLine}{string.Join(Environment.NewLine, failures.Take(25))}");
    }

    /// <summary>
    /// A real mercenary gear table edits one kit occurrence in place, keeps every other element and
    /// value, and leaves a language overlay untouched when the edit is not translated.
    /// </summary>
    [Fact]
    public async Task MercGear_EditsOneKitOnRealData()
    {
        var corpus = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(corpus) || !Directory.Exists(corpus))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var inventory = Path.Combine(workspace.DirectoryPath, "Inventory");
        Directory.CreateDirectory(inventory);
        var basePath = Path.Combine(inventory, "MercStartingGear.xml");
        File.Copy(Path.Combine(corpus, "Inventory", "MercStartingGear.xml"), basePath);
        var overlaySource = Path.Combine(corpus, "Inventory", "Russian.MercStartingGear.xml");
        var overlayPath = Path.Combine(inventory, "Russian.MercStartingGear.xml");
        var hasOverlay = File.Exists(overlaySource);
        if (hasOverlay)
        {
            File.Copy(overlaySource, overlayPath);
        }

        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var gear = await provider.GetTableAsync(TableName.Parse("Inventory.MercStartingGear"));
        Assert.True(gear.IsSuccess, gear.ErrorText);
        var language = new LanguageDescriptor("ru", "Russian", "Russian", 1);
        var descriptor = new TableDescriptor(
            gear.Data!.TableName,
            basePath,
            "Inventory/MercStartingGear.xml",
            IsMapped: true,
            hasOverlay
                ? new[] { new OverlayDescriptor(language, basePath, overlayPath) }
                : Array.Empty<OverlayDescriptor>());
        var beforeBase = await File.ReadAllBytesAsync(basePath);
        var beforeOverlay = hasOverlay ? await File.ReadAllBytesAsync(overlayPath) : null;
        var load = await TableWorkspace.LoadAsync(
            descriptor,
            gear.Data,
            hasOverlay ? language : null);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        var first = table.BaseDocument.Records.First(static record => record.GetKey() == "0");
        Assert.Equal(5, first.GetValues("GEARKIT").Count);
        var kitOneWeapon = first.GetRawValue(FieldPath.Parse("GEARKIT/mWeapon"), 1);
        Assert.False(string.IsNullOrEmpty(kitOneWeapon));

        var itemNamesBefore = first.FieldNames.ToArray();
        var edit = table.SetValue("0", "GEARKIT/mWeapon", 1, "1");
        Assert.True(edit.IsSuccess, edit.ErrorText);
        Assert.True(table.Undo().IsSuccess);
        Assert.Equal(
            kitOneWeapon,
            table.BaseDocument.FindRecord("0")!.GetRawValue(FieldPath.Parse("GEARKIT/mWeapon"), 1));
        Assert.True(table.SetValue("0", "GEARKIT/mWeapon", 1, "1").IsSuccess);

        Assert.True((await table.SaveAsync()).IsSuccess);
        Assert.NotEqual(beforeBase, await File.ReadAllBytesAsync(basePath));
        if (hasOverlay)
        {
            Assert.Equal(beforeOverlay, await File.ReadAllBytesAsync(overlayPath));
        }

        var saved = XDocument.Load(basePath, LoadOptions.PreserveWhitespace);
        var savedMerc = saved.Root!.Elements("MERCGEAR").First(static element => element.Element("mIndex")?.Value == "0");
        var savedKits = savedMerc.Elements("GEARKIT").ToArray();
        Assert.Equal(5, savedKits.Length);
        Assert.Equal("1", savedKits[1].Element("mWeapon")?.Value);
        Assert.Equal(kitOneWeapon, savedKits[0].Element("mWeapon")?.Value);
        Assert.Equal(
            itemNamesBefore,
            savedMerc.Elements()
                .Select(static element => element.Name.LocalName)
                .Distinct(StringComparer.Ordinal)
                .ToArray());
    }

    /// <summary>
    /// A real sound list exposes one positional record per text value and saves an edited value as
    /// record text.
    /// </summary>
    [Fact]
    public async Task Sounds_EditTextOnlyRecordsOnRealData()
    {
        var corpus = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(corpus) || !Directory.Exists(corpus))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var soundsDirectory = Path.Combine(workspace.DirectoryPath, "Sounds");
        Directory.CreateDirectory(soundsDirectory);
        var basePath = Path.Combine(soundsDirectory, "Sounds.xml");
        File.Copy(Path.Combine(corpus, "Sounds", "Sounds.xml"), basePath);
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var sounds = await provider.GetTableAsync(TableName.Parse("Sounds.Sounds"));
        Assert.True(sounds.IsSuccess, sounds.ErrorText);
        var descriptor = new TableDescriptor(
            sounds.Data!.TableName,
            basePath,
            "Sounds/Sounds.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await TableWorkspace.LoadAsync(descriptor, sounds.Data);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        Assert.Equal(506, table.BaseDocument.Records.Count);
        Assert.Equal("0", table.BaseDocument.Records[0].GetKey());
        var original = table.BaseDocument.Records[2].GetRawValue(RecordFields.RecordText);
        Assert.False(string.IsNullOrEmpty(original));
        var schema = new TableSchemaBuilder().Build(table.BaseDocument);
        Assert.NotNull(schema.FindColumn(RecordFields.RecordText));
        Assert.Null(schema.KeyColumn);

        Assert.True(table.SetValue("2", RecordFields.RecordText, "SOUNDS\\M20 TEST.wav").IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);

        var saved = XDocument.Load(basePath, LoadOptions.PreserveWhitespace);
        var entries = saved.Root!.Elements("SOUND").ToArray();
        Assert.Equal(506, entries.Length);
        Assert.Equal("SOUNDS\\M20 TEST.wav", entries[2].Value);
        Assert.All(entries, static entry => Assert.False(entry.HasElements));
    }
}
