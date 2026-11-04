using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Lookups;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="TableWorkspace"/>.
/// </summary>
public class TableWorkspaceTests
{
    /// <summary>
    /// A base-only workspace builds rows, edits scalar fields, and saves through the lossless patcher.
    /// </summary>
    [Fact]
    public async Task BaseOnly_LoadEditSave_UpdatesValueAndKeepsSparseFields()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var load = await TableWorkspace.LoadAsync(Descriptor(path), OverlayTestData.ItemsMetadata());
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        Assert.True(table.CanEdit);
        Assert.Null(table.Overlay);
        Assert.NotNull(table.Session);
        Assert.Equal(2, table.GetRows().Count);
        var row = Assert.Single(table.GetRows(), candidate => candidate.Key == "1");
        Assert.Equal("Glock 17", row.GetValue("szItemName"));
        Assert.False(row.IsTranslated("szItemName"));

        Assert.True(table.SetValue("1", "szItemName", "Glock 18").IsSuccess);
        Assert.True(table.IsDirty);
        Assert.Equal(
            "Glock 18",
            Assert.Single(table.GetRows(), candidate => candidate.Key == "1").GetValue("szItemName"));
        Assert.True((await table.SaveAsync()).IsSuccess);
        Assert.False(table.IsDirty);

        var saved = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        var items = saved.Root!.Elements("ITEM").ToArray();
        Assert.Equal("Glock 18", items[1].Element("szItemName")!.Value);
        Assert.Equal("keep me", items[0].Element("CustomUnknown")!.Value);
        Assert.Equal("1.0", items[1].Element("ProjectionFactor")!.Value);
    }

    /// <summary>
    /// A read-only XML snapshot reflects unsaved grid edits without changing the source file.
    /// </summary>
    [Fact]
    public async Task BaseDocument_XmlSnapshot_ReflectsUnsavedEdit()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var load = await TableWorkspace.LoadAsync(Descriptor(path), OverlayTestData.ItemsMetadata());
        var table = load.Data!;

        Assert.True(table.SetValue("1", "szItemName", "Glock 18").IsSuccess);
        var snapshot = table.BaseDocument.CreateXmlSnapshot();

        Assert.StartsWith("<?xml", snapshot.Text, StringComparison.Ordinal);
        Assert.Contains("<szItemName>Glock 18</szItemName>", snapshot.Text, StringComparison.Ordinal);
        Assert.DoesNotContain("<szItemName>Glock 17</szItemName>", snapshot.Text, StringComparison.Ordinal);
        Assert.Equal(new[] { "0", "1" }, snapshot.Records.Select(static record => record.Key));
        foreach (var record in snapshot.Records)
        {
            Assert.StartsWith(
                "<ITEM>",
                snapshot.Text.Substring(record.Start, record.Length),
                StringComparison.Ordinal);
        }

        var editedRecord = Assert.Single(snapshot.Records, static record => record.Key == "1");
        Assert.Contains(
            "<szItemName>Glock 18</szItemName>",
            snapshot.Text.Substring(editedRecord.Start, snapshot.Text.Length - editedRecord.Start),
            StringComparison.Ordinal);
        Assert.Same(editedRecord, snapshot.FindRecord("1"));
        Assert.Null(snapshot.FindRecord("missing"));
        Assert.Contains(
            "<szItemName>Glock 17</szItemName>",
            await File.ReadAllTextAsync(path),
            StringComparison.Ordinal);
    }

    /// <summary>
    /// A sparse record can receive a field that other records already define.
    /// </summary>
    [Fact]
    public async Task BaseOnly_AddMissingField_AppendsFieldToRecord()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var load = await TableWorkspace.LoadAsync(Descriptor(path), OverlayTestData.ItemsMetadata());
        var table = load.Data!;

        var edit = table.SetValue("0", "ProjectionFactor", "1.5", addIfMissing: true);

        Assert.True(edit.IsSuccess, edit.ErrorText);
        Assert.Equal("1.5", table.BaseDocument.Records[0].GetRawValue("ProjectionFactor"));
        Assert.True((await table.SaveAsync()).IsSuccess);
        Assert.Contains("<ProjectionFactor>1.5</ProjectionFactor>", await File.ReadAllTextAsync(path), StringComparison.Ordinal);
    }

    /// <summary>
    /// With an overlay selected, translatable edits go to the overlay while other edits stay in the base.
    /// </summary>
    [Fact]
    public async Task Overlay_MergesValuesAndRoutesEditsByField()
    {
        using var workspace = new TestWorkspace();
        var (basePath, overlayPath, metadata, descriptor) = CopyItems(workspace);
        var load = await TableWorkspace.LoadAsync(descriptor, metadata, OverlayTestData.Russian);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        Assert.NotNull(table.Overlay);
        Assert.Equal("ru", table.Language!.Code);
        var translated = Assert.Single(table.GetRows(), candidate => candidate.Key == "1");
        Assert.Equal("Глок 17", translated.GetValue("szItemName"));
        Assert.True(translated.IsTranslated("szItemName"));
        var untranslated = Assert.Single(table.GetRows(), candidate => candidate.Key == "0");
        Assert.Equal("Nothing", untranslated.GetValue("szItemName"));
        Assert.False(untranslated.IsTranslated("szItemName"));

        var baseBytes = await File.ReadAllBytesAsync(basePath);
        Assert.True(table.SetValue("0", "szItemName", "Ничто").IsSuccess);
        Assert.Equal(baseBytes, await File.ReadAllBytesAsync(basePath));
        var refreshed = table.GetRow("0");
        Assert.NotNull(refreshed);
        Assert.Equal("Ничто", refreshed.GetValue("szItemName"));
        Assert.True(refreshed.IsTranslated("szItemName"));
        var created = table.Overlay.Document.FindRecord("0");
        Assert.NotNull(created);
        Assert.Equal(new[] { "uiIndex", "szItemName" }, created.FieldNames);

        Assert.True(table.SetValue("0", "ubWeight", "5").IsSuccess);
        Assert.Equal("5", table.BaseDocument.Records[0].GetRawValue("ubWeight"));
        Assert.Equal("5", refreshed.GetValue("ubWeight"));
        Assert.Equal(
            "Ничто",
            Assert.Single(table.GetRows(), candidate => candidate.Key == "0").GetValue("szItemName"));

        Assert.True((await table.SaveAsync()).IsSuccess);
        Assert.False(table.IsDirty);

        var savedBase = XDocument.Load(basePath);
        var savedItem = Assert.Single(savedBase.Root!.Elements("ITEM"), item => item.Element("uiIndex")?.Value == "0");
        Assert.Equal("5", savedItem.Element("ubWeight")?.Value);
        Assert.Equal("Nothing", savedItem.Element("szItemName")?.Value);

        var savedOverlay = XDocument.Load(overlayPath);
        var translatedItem = Assert.Single(savedOverlay.Root!.Elements("ITEM"), item => item.Element("uiIndex")?.Value == "0");
        Assert.Equal("Ничто", translatedItem.Element("szItemName")?.Value);
        Assert.NotNull(Assert.Single(savedOverlay.Root!.Elements("ITEM"), item => item.Element("uiIndex")?.Value == "1"));
        Assert.NotNull(table.AnalyzeTranslations());
    }

    /// <summary>
    /// Deleting a base record also removes its translation from the overlay.
    /// </summary>
    [Fact]
    public async Task Overlay_DeleteRecord_RemovesTranslation()
    {
        using var workspace = new TestWorkspace();
        var (basePath, overlayPath, metadata, descriptor) = CopyItems(workspace);
        var load = await TableWorkspace.LoadAsync(descriptor, metadata, OverlayTestData.Russian);
        var table = load.Data!;

        Assert.True(table.DeleteRecord("1").IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);

        var savedBase = XDocument.Load(basePath);
        Assert.DoesNotContain(savedBase.Root!.Elements("ITEM"), item => item.Element("uiIndex")?.Value == "1");
        var savedOverlay = XDocument.Load(overlayPath);
        Assert.DoesNotContain(savedOverlay.Root!.Elements("ITEM"), item => item.Element("uiIndex")?.Value == "1");
    }

    /// <summary>
    /// Adding and deleting a record through the workspace returns the base file to its original bytes.
    /// </summary>
    [Fact]
    public async Task BaseOnly_AddThenDeleteRecord_RestoresBytes()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var original = await File.ReadAllBytesAsync(path);
        var load = await TableWorkspace.LoadAsync(Descriptor(path), OverlayTestData.ItemsMetadata());
        var table = load.Data!;

        var added = table.AddRecord();
        Assert.True(added.IsSuccess, added.ErrorText);
        var key = added.Data!.GetKey()!;
        Assert.True((await table.SaveAsync()).IsSuccess);
        Assert.True(table.DeleteRecord(key).IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);

        Assert.Equal(original, await File.ReadAllBytesAsync(path));
    }

    /// <summary>
    /// Adding a lookup record creates the empty <c>name</c> element required by the lookup file contract,
    /// and the saved file is accepted by the lookup repository.
    /// </summary>
    [Fact]
    public async Task Lookup_AddRecord_CreatesEmptyNameAndSavesLoadableFile()
    {
        using var workspace = new TestWorkspace();
        var directory = Path.Combine(workspace.DirectoryPath, "Lookup");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "ItemClass.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
            + "<JA2Data>\n"
            + "\t<ItemClass>\n\t\t<id>1</id>\n\t\t<name>Gun</name>\n\t</ItemClass>\n"
            + "\t<ItemClass>\n\t\t<id>2</id>\n\t\t<name>Knife</name>\n\t</ItemClass>\n"
            + "</JA2Data>\n");
        var metadata = new TableMetadata(
            TableName.Parse("Lookup.ItemClass"),
            "JA2Data",
            "ItemClass",
            isLookup: true,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "id") }));
        var descriptor = new TableDescriptor(
            metadata.TableName,
            path,
            "Lookup/ItemClass.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await TableWorkspace.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        var added = table.AddRecord();

        Assert.True(added.IsSuccess, added.ErrorText);
        var record = added.Data!;
        Assert.Equal(new[] { "id", "name" }, record.FieldNames);
        Assert.Equal("3", record.GetRawValue("id"));
        Assert.Equal(string.Empty, record.GetRawValue("name"));
        Assert.True((await table.SaveAsync()).IsSuccess);

        var saved = XDocument.Load(path);
        var savedRecord = Assert.Single(saved.Root!.Elements("ItemClass"), element => element.Element("id")?.Value == "3");
        Assert.Equal(new[] { "id", "name" }, savedRecord.Elements().Select(static element => element.Name.LocalName));
        Assert.Equal(string.Empty, savedRecord.Element("name")!.Value);

        var repository = new XmlFileLookupRepository(workspace.DirectoryPath);
        var entries = await repository.GetEntriesAsync(TableName.Parse("Lookup.ItemClass"));
        Assert.True(entries.IsSuccess, entries.ErrorText);
        Assert.Equal(new LookupEntry(3, string.Empty), Assert.Single(entries.Data!, entry => entry.Id == 3));
    }

    /// <summary>
    /// Adding a record to a non-lookup table does not create a <c>name</c> element.
    /// </summary>
    [Fact]
    public async Task NonLookup_AddRecord_DoesNotAddNameElement()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var load = await TableWorkspace.LoadAsync(Descriptor(path), OverlayTestData.ItemsMetadata());
        Assert.True(load.IsSuccess, load.ErrorText);

        var added = load.Data!.AddRecord();

        Assert.True(added.IsSuccess, added.ErrorText);
        Assert.Equal(new[] { "uiIndex" }, added.Data!.FieldNames);
        Assert.False(added.Data.HasField("name"));
    }

    /// <summary>
    /// A document-driven workspace has no edit session but still supports direct record edits.
    /// </summary>
    [Fact]
    public async Task DocumentDriven_EditsWithoutMetadata()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var load = await TableWorkspace.LoadAsync(Descriptor(path, isMapped: false), metadata: null);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        Assert.Null(table.Session);
        Assert.True(table.Schema.IsDocumentDriven);
        Assert.True(table.SetValue("1", "szItemName", "Glock 18").IsSuccess);
        Assert.True(table.SetValue("0", "ProjectionFactor", "1.5", addIfMissing: true).IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);
        Assert.Contains("<ProjectionFactor>1.5</ProjectionFactor>", await File.ReadAllTextAsync(path), StringComparison.Ordinal);
    }

    /// <summary>
    /// Loading and saving a nested-record table without edits leaves its bytes untouched.
    /// </summary>
    [Fact]
    public async Task NestedCity_WithoutEdits_DoesNotRewrite()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("Overlays/cities-base.xml");
        var before = await File.ReadAllBytesAsync(path);
        var metadata = OverlayTestData.CitiesMetadata();
        var descriptor = new TableDescriptor(
            metadata.TableName,
            path,
            "Map/Cities.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await TableWorkspace.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);

        var result = await load.Data!.SaveAsync();

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(before, await File.ReadAllBytesAsync(path));
    }

    /// <summary>
    /// A root singleton field is editable in the base table and undo restores both the value and the
    /// root-cell view.
    /// </summary>
    [Fact]
    public async Task RootSingleton_BaseEdit_IsUndoable()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("Overlays/cities-base.xml");
        var metadata = OverlayTestData.CitiesMetadata();
        var descriptor = new TableDescriptor(
            metadata.TableName,
            path,
            "Map/Cities.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await TableWorkspace.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        var before = Assert.Single(table.GetRootCells(), cell => cell.FieldPath == "COUNTRY/countryName");
        Assert.Equal("Arulco", before.Value);

        var result = table.SetRootValue(FieldPath.Parse("COUNTRY/countryName"), "Arulco New");

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.True(table.IsDirty);
        var changed = Assert.Single(table.GetRootCells(), cell => cell.FieldPath == "COUNTRY/countryName");
        Assert.Equal("Arulco New", changed.Value);

        Assert.True(table.Undo().IsSuccess);
        var restored = Assert.Single(table.GetRootCells(), cell => cell.FieldPath == "COUNTRY/countryName");
        Assert.Equal("Arulco", restored.Value);
    }

    /// <summary>
    /// Nested records are exposed as merged rows and root singleton translations as root cells.
    /// </summary>
    [Fact]
    public async Task NestedRecords_ExposeMergedRowsAndRootCells()
    {
        using var workspace = new TestWorkspace();
        var basePath = workspace.CopyFixture("Overlays/cities-base.xml");
        var overlayPath = workspace.CopyFixture("Overlays/cities-ru.xml");
        var metadata = OverlayTestData.CitiesMetadata();
        var descriptor = new TableDescriptor(
            metadata.TableName,
            basePath,
            "Map/Cities.xml",
            IsMapped: true,
            new[] { new OverlayDescriptor(OverlayTestData.Russian, basePath, overlayPath) });
        var load = await TableWorkspace.LoadAsync(descriptor, metadata, OverlayTestData.Russian);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        Assert.Equal(2, table.BaseDocument.Records.Count);
        var rows = table.GetRows();
        Assert.Equal(2, rows.Count);
        Assert.All(rows, row => Assert.NotNull(row.Record));
        var translated = Assert.Single(rows, row => row.Key == "1");
        var translatedCell = Assert.Single(
            translated.Merged!.Cells,
            cell => cell.FieldPath.EndsWith("townName", StringComparison.Ordinal));
        Assert.Equal("Омерта", translatedCell.Value);
        Assert.True(translatedCell.IsTranslated);
        Assert.Equal("Омерта", translated.GetValue("townName"));
        Assert.Equal("1", translated.GetValue("townUsesLoyalty"));
        var untranslated = Assert.Single(rows, row => row.Key == "2");
        var untranslatedCell = Assert.Single(
            untranslated.Merged!.Cells,
            cell => cell.FieldPath.EndsWith("townName", StringComparison.Ordinal));
        Assert.Equal("Drassen", untranslatedCell.Value);
        Assert.False(untranslatedCell.IsTranslated);
        Assert.Equal("Drassen", untranslated.GetValue("townName"));
        Assert.Equal("1", untranslated.GetValue("townUsesLoyalty"));

        var rootCells = table.GetRootCells();
        var countryName = Assert.Single(rootCells, cell => cell.FieldPath.EndsWith("countryName", StringComparison.Ordinal));
        Assert.Equal("Арулько", countryName.Value);
        Assert.True(countryName.IsTranslated);
    }

    /// <summary>
    /// Requesting an overlay that the table does not have fails with a dedicated error code.
    /// </summary>
    [Fact]
    public async Task Load_MissingOverlay_ReturnsOverlayNotFound()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");

        var load = await TableWorkspace.LoadAsync(
            Descriptor(path),
            OverlayTestData.ItemsMetadata(),
            OverlayTestData.Russian);

        Assert.False(load.IsSuccess);
        Assert.Equal(ErrorCodes.OverlayNotFound, load.ErrorCode);
    }

    /// <summary>
    /// A malformed base file is reported instead of producing an editable workspace.
    /// </summary>
    [Fact]
    public async Task Load_MalformedFile_ReturnsParseError()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("malformed.xml");

        var load = await TableWorkspace.LoadAsync(Descriptor(path, isMapped: false), metadata: null);

        Assert.False(load.IsSuccess);
        Assert.Equal(ErrorCodes.XmlParseError, load.ErrorCode);
    }

    /// <summary>
    /// A new assignment is appended after the existing nested records, keeps its key, and can be undone.
    /// </summary>
    [Fact]
    public async Task AddNestedRecord_Assignment_AppendsAfterLastAssignment()
    {
        using var workspace = new TestWorkspace();
        var (basePath, _, metadata, descriptor) = CopyFacility(workspace, withOverlay: false);
        var load = await TableWorkspace.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        var result = table.AddNestedRecord(
            "1",
            segmentIndex: 1,
            "CHEF",
            new[] { new KeyValuePair<string, string>("ubStaffLimit", "1") });

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.True(table.IsDirty);
        var nested = table.GetNestedRecords("1", 1);
        Assert.Equal(3, nested.Count);
        Assert.Equal("CHEF", nested[^1].Key);
        Assert.Equal("1", nested[^1].Record.GetRawValue("ubStaffLimit"));

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal(2, table.GetNestedRecords("1", 1).Count);

        Assert.True(table.Redo().IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);
        var saved = XDocument.Load(basePath);
        var types = saved.Root!
            .Element("FACILITYTYPE")!
            .Elements("ASSIGNMENT")
            .Select(element => element.Element("ubAssignmentType")?.Value)
            .ToArray();
        Assert.Equal(new[] { "DOCTOR", "NURSE", "CHEF" }, types);
    }

    /// <summary>
    /// A duplicate nested key is rejected instead of appending an ambiguous assignment.
    /// </summary>
    [Fact]
    public async Task AddNestedRecord_DuplicateKey_ReturnsKeyConflict()
    {
        using var workspace = new TestWorkspace();
        var (_, _, metadata, descriptor) = CopyFacility(workspace, withOverlay: false);
        var load = await TableWorkspace.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);

        var result = load.Data!.AddNestedRecord("1", 1, "DOCTOR");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.KeyConflict, result.ErrorCode);
        Assert.Equal(2, load.Data.GetNestedRecords("1", 1).Count);
    }

    /// <summary>
    /// Deleting an assignment removes its overlay translation too, and undo restores both.
    /// </summary>
    [Fact]
    public async Task DeleteNestedRecord_RemovesAssignmentAndOverlayEntry()
    {
        using var workspace = new TestWorkspace();
        var (_, _, metadata, descriptor) = CopyFacility(workspace, withOverlay: true);
        var load = await TableWorkspace.LoadAsync(descriptor, metadata, OverlayTestData.Russian);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;

        var result = table.DeleteNestedRecord("1", 1, occurrenceIndex: 0);

        Assert.True(result.IsSuccess, result.ErrorText);
        var remaining = table.GetNestedRecords("1", 1);
        Assert.Equal("NURSE", Assert.Single(remaining).Key);
        var overlayRecord = table.Overlay!.Document.Records.Single(record => record.GetKey() == "1");
        Assert.False(overlayRecord.HasField("ASSIGNMENT"));

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal(2, table.GetNestedRecords("1", 1).Count);
        overlayRecord = table.Overlay.Document.Records.Single(record => record.GetKey() == "1");
        Assert.True(overlayRecord.HasField("ASSIGNMENT"));
        Assert.Equal(
            "Доктор",
            overlayRecord.GetRawValue(FieldPath.Parse("ASSIGNMENT/szTooltipText")));
    }

    /// <summary>
    /// Repeated production-line blocks can be appended and removed without touching their siblings.
    /// </summary>
    [Fact]
    public async Task RepeatedBlock_AddAndDeleteProductionLine()
    {
        using var workspace = new TestWorkspace();
        var (_, _, metadata, descriptor) = CopyFacility(workspace, withOverlay: false);
        var load = await TableWorkspace.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;
        var record = Assert.IsType<TableRecord>(table.FindRecord("1"));
        Assert.Equal(2, record.GetOccurrenceCount("PRODUCTIONLINE"));

        var added = table.AddRepeatedBlock(
            "1",
            "PRODUCTIONLINE",
            new[] { new KeyValuePair<string, string>("usItemToCreate", "9999") });

        Assert.True(added.IsSuccess, added.ErrorText);
        Assert.Equal(3, record.GetOccurrenceCount("PRODUCTIONLINE"));
        Assert.Equal("9999", record.GetRawValue(FieldPath.Parse("PRODUCTIONLINE/usItemToCreate"), 2));

        Assert.True(table.DeleteRepeatedBlock("1", "PRODUCTIONLINE", 2).IsSuccess);
        Assert.Equal(2, record.GetOccurrenceCount("PRODUCTIONLINE"));
        Assert.True(table.Undo().IsSuccess);
        Assert.Equal(3, record.GetOccurrenceCount("PRODUCTIONLINE"));
    }

    /// <summary>
    /// A repeated leaf inside a nested assignment is replaced without touching the surrounding block.
    /// </summary>
    [Fact]
    public async Task SetNestedRepeatedValues_DrugItems_ReplacesOnlyTargetChildren()
    {
        using var workspace = new TestWorkspace();
        var (_, _, metadata, descriptor) = CopyFacility(workspace, withOverlay: false);
        var load = await TableWorkspace.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;
        var doctor = table.GetNestedRecords("1", 1)[0];
        var path = FieldPath.Parse("DRUNK/drugitems/drugitem");
        Assert.Equal(new[] { "245", "255" }, table.GetNestedRepeatedValues(doctor, path));

        var result = table.SetNestedRepeatedValues(
            "1",
            doctor,
            path,
            new[] { "300", "301", "302" });

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(new[] { "300", "301", "302" }, table.GetNestedRepeatedValues(doctor, path));
        Assert.Equal("100", doctor.Record.GetRawValue(FieldPath.Parse("DRUNK/ubChance")));

        Assert.True(table.Undo().IsSuccess);
        var restored = table.GetNestedRecords("1", 1)[0];
        Assert.Equal(new[] { "245", "255" }, table.GetNestedRepeatedValues(restored, path));
    }

    /// <summary>
    /// A repeated leaf inside one production line is edited by block occurrence, so other blocks keep
    /// their own values.
    /// </summary>
    [Fact]
    public async Task SetRepeatedValues_ProductionLine_OnlySelectedBlock()
    {
        using var workspace = new TestWorkspace();
        var (_, _, metadata, descriptor) = CopyFacility(workspace, withOverlay: false);
        var load = await TableWorkspace.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;
        var path = FieldPath.Parse("PRODUCTIONLINE/usOptional_ItemUsedUp");
        Assert.Equal(new[] { "1", "2" }, table.GetRepeatedValues("1", path, occurrenceIndex: 0));
        Assert.Empty(table.GetRepeatedValues("1", path, occurrenceIndex: 1));

        var result = table.SetRepeatedValues("1", path, new[] { "7", "8" }, occurrenceIndex: 0);

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(new[] { "7", "8" }, table.GetRepeatedValues("1", path, occurrenceIndex: 0));
        Assert.Equal("1447", table.GetRepeatedValues(
            "1",
            FieldPath.Parse("PRODUCTIONLINE/usItemToCreate"),
            occurrenceIndex: 1).Single());
    }

    /// <summary>
    /// The nested city schema marks the qualified <c>CITYLIST/CITY/townName</c> translation and an
    /// overlay edit leaves the base city name untouched.
    /// </summary>
    [Fact]
    public async Task CityTownName_OverlayEdit_WritesOnlyOverlay()
    {
        using var workspace = new TestWorkspace();
        var basePath = workspace.CopyFixture("Overlays/cities-base.xml");
        var overlayPath = workspace.CopyFixture("Overlays/cities-ru.xml");
        var metadata = OverlayTestData.CitiesMetadata();
        var descriptor = new TableDescriptor(
            metadata.TableName,
            basePath,
            "Map/Cities.xml",
            IsMapped: true,
            new[] { new OverlayDescriptor(OverlayTestData.Russian, basePath, overlayPath) });
        var load = await TableWorkspace.LoadAsync(descriptor, metadata, OverlayTestData.Russian);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;
        Assert.True(Assert.Single(table.Schema.Columns, column => column.XmlFieldName == "townName").IsTranslatable);

        var result = table.SetValue("1", "townName", "Омерта Новая");

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal("Omerta", XDocument.Load(basePath).Root!
            .Element("CITYLIST")!
            .Elements("CITY")
            .Single(element => element.Element("uiIndex")?.Value == "1")
            .Element("townName")?.Value);
        Assert.Equal("Омерта Новая", OverlayCityName(table, "1"));

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal("Омерта", OverlayCityName(table, "1"));

        Assert.True(table.SetValue("1", "townName", "Омерта Новая").IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);
        Assert.Equal("Омерта Новая", XDocument.Load(overlayPath).Root!
            .Element("CITYLIST")!
            .Elements("CITY")
            .Single(element => element.Element("uiIndex")?.Value == "1")
            .Element("townName")?.Value);
        Assert.Equal("Omerta", XDocument.Load(basePath).Root!
            .Element("CITYLIST")!
            .Elements("CITY")
            .Single(element => element.Element("uiIndex")?.Value == "1")
            .Element("townName")?.Value);
    }

    private static string? OverlayCityName(TableWorkspace table, string key) =>
        table.Overlay!.Document.Records
            .Single(record => record.GetKey() == key)
            .GetRawValue("townName");

    private static (string BasePath, string OverlayPath, TableMetadata Metadata, TableDescriptor Descriptor)
        CopyFacility(TestWorkspace workspace, bool withOverlay)
    {
        var basePath = workspace.CopyFixture("Overlays/facility-base.xml");
        var overlayPath = workspace.CopyFixture("Overlays/facility-ru.xml");
        var metadata = OverlayTestData.FacilityMetadata();
        var descriptor = new TableDescriptor(
            metadata.TableName,
            basePath,
            "Map/FacilityTypes.xml",
            IsMapped: true,
            withOverlay
                ? new[] { new OverlayDescriptor(OverlayTestData.Russian, basePath, overlayPath) }
                : Array.Empty<OverlayDescriptor>());
        return (basePath, overlayPath, metadata, descriptor);
    }

    private static (string BasePath, string OverlayPath, TableMetadata Metadata, TableDescriptor Descriptor)
        CopyItems(TestWorkspace workspace)
    {
        var basePath = workspace.CopyFixture("Overlays/items-base.xml");
        var overlayPath = workspace.CopyFixture("Overlays/items-ru.xml");
        var metadata = OverlayTestData.ItemsMetadata();
        var descriptor = new TableDescriptor(
            metadata.TableName,
            basePath,
            "Items/Items.xml",
            IsMapped: true,
            new[] { new OverlayDescriptor(OverlayTestData.Russian, basePath, overlayPath) });
        return (basePath, overlayPath, metadata, descriptor);
    }

    private static TableDescriptor Descriptor(string path, bool isMapped = true) =>
        new(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            isMapped,
            Array.Empty<OverlayDescriptor>());
}
