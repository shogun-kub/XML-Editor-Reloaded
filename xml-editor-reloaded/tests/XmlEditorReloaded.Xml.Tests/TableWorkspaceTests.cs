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

        Assert.Empty(table.BaseDocument.Records);
        var rows = table.GetRows();
        Assert.Equal(2, rows.Count);
        Assert.All(rows, row => Assert.Null(row.Record));
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
