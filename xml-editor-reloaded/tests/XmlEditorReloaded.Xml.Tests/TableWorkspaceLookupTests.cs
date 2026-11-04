using System.Xml.Linq;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for lookup editing in <see cref="TableWorkspace"/>: allocated ids, free-form id changes, and
/// unsigned 64-bit identifiers above the 32-bit column range.
/// </summary>
public class TableWorkspaceLookupTests
{
    /// <summary>
    /// A new lookup value receives the exact identifier and an empty name, and the file stays readable.
    /// </summary>
    [Fact]
    public async Task AddLookupRecord_WritesIdAndEmptyName()
    {
        using var fixture = new TestWorkspace();
        var path = WriteLookup(fixture, "<Cursor><id>0</id><name>Invalid</name></Cursor>"
            + "<Cursor><id>1</id><name>Normal</name></Cursor>");
        var table = await LoadAsync(path, LookupMetadata());

        var result = table.AddLookupRecord("24");

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.True(table.IsDirty);
        var added = Assert.Single(table.BaseDocument.Records, record => record.GetKey() == "24");
        Assert.Equal(string.Empty, added.GetRawValue("name"));
        Assert.True((await table.SaveAsync()).IsSuccess);

        var saved = XDocument.Load(path);
        var records = saved.Root!.Elements("Cursor").ToArray();
        Assert.Equal(3, records.Length);
        Assert.Equal("24", records[2].Element("id")!.Value);
        Assert.Equal(string.Empty, records[2].Element("name")!.Value);
        Assert.Equal("Invalid", records[0].Element("name")!.Value);
    }

    /// <summary>
    /// A lookup identifier above <see cref="int.MaxValue"/> is written verbatim even though the declared
    /// id column is a 32-bit integer.
    /// </summary>
    [Fact]
    public async Task AddLookupRecord_Accepts64BitIdAboveInt32()
    {
        using var fixture = new TestWorkspace();
        var path = WriteLookup(fixture, "<ItemFlag><id>8589934592</id><name>Existing</name></ItemFlag>");
        var table = await LoadAsync(path, LookupMetadata("Lookup.ItemFlag"));

        var result = table.AddLookupRecord("8589934593");

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.NotNull(table.BaseDocument.FindRecord("8589934593"));
        Assert.True((await table.SaveAsync()).IsSuccess);
        var savedText = await File.ReadAllTextAsync(path);
        Assert.Contains("8589934593", savedText);
    }

    /// <summary>
    /// A numeric duplicate is rejected even when its lexical form differs, and a non-numeric id is
    /// rejected before the record is created.
    /// </summary>
    [Fact]
    public async Task AddLookupRecord_RejectsDuplicateAndInvalidId()
    {
        using var fixture = new TestWorkspace();
        var path = WriteLookup(fixture, "<Cursor><id>1</id><name>Normal</name></Cursor>");
        var table = await LoadAsync(path, LookupMetadata());

        var duplicate = table.AddLookupRecord("01");
        Assert.Equal(ErrorCodes.KeyConflict, duplicate.ErrorCode);

        var invalid = table.AddLookupRecord("-2");
        Assert.Equal(ErrorCodes.XmlInvalidNumber, invalid.ErrorCode);

        Assert.Single(table.BaseDocument.Records);
        Assert.False(table.IsDirty);
    }

    /// <summary>
    /// The identifier of a lookup value can be changed to any unused 64-bit id and undone as one step.
    /// </summary>
    [Fact]
    public async Task ChangeLookupId_RenamesKeyAndUndoesAsOneStep()
    {
        using var fixture = new TestWorkspace();
        var path = WriteLookup(fixture, "<Cursor><id>1</id><name>Normal</name></Cursor>"
            + "<Cursor><id>2</id><name>Other</name></Cursor>");
        var table = await LoadAsync(path, LookupMetadata());
        var row = Assert.Single(table.GetRows(), candidate => candidate.Key == "1");

        var result = table.ChangeLookupId(row, "8589934593");

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Null(table.BaseDocument.FindRecord("1"));
        var changed = table.BaseDocument.FindRecord("8589934593");
        Assert.NotNull(changed);
        Assert.Equal("Normal", changed!.GetRawValue("name"));

        Assert.True(table.Undo().IsSuccess);
        Assert.NotNull(table.BaseDocument.FindRecord("1"));
        Assert.Null(table.BaseDocument.FindRecord("8589934593"));
    }

    /// <summary>
    /// Changing an id to an existing numeric value is rejected without touching the document.
    /// </summary>
    [Fact]
    public async Task ChangeLookupId_RejectsNumericDuplicate()
    {
        using var fixture = new TestWorkspace();
        var path = WriteLookup(fixture, "<Cursor><id>1</id><name>Normal</name></Cursor>"
            + "<Cursor><id>2</id><name>Other</name></Cursor>");
        var table = await LoadAsync(path, LookupMetadata());
        var row = Assert.Single(table.GetRows(), candidate => candidate.Key == "1");

        var result = table.ChangeLookupId(row, "02");

        Assert.Equal(ErrorCodes.KeyConflict, result.ErrorCode);
        Assert.NotNull(table.BaseDocument.FindRecord("1"));
        Assert.False(table.IsDirty);
    }

    /// <summary>
    /// A regular table has no lookup key field, so the lookup operations refuse it.
    /// </summary>
    [Fact]
    public async Task LookupOperations_RejectRegularTable()
    {
        using var fixture = new TestWorkspace();
        var path = Path.Combine(fixture.DirectoryPath, "items.xml");
        await File.WriteAllTextAsync(
            path,
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex></ITEM></ITEMLIST>");
        var table = await LoadAsync(path, new TableMetadata(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            recordKey: new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") })));

        Assert.Null(table.LookupKeyField);
        Assert.Empty(table.GetLookupIds());
        Assert.Null(table.SuggestLookupId());
        Assert.Equal(ErrorCodes.XmlNotSaveable, table.AddLookupRecord("2").ErrorCode);

        var row = Assert.Single(table.GetRows());
        Assert.Equal(ErrorCodes.XmlNotSaveable, table.ChangeLookupId(row, "2").ErrorCode);
    }

    /// <summary>
    /// Suggestion follows the lookup kind: a flag lookup gets the lowest free bit and a sequential one
    /// gets the maximum id plus one.
    /// </summary>
    [Fact]
    public async Task SuggestLookupId_FollowsLookupKind()
    {
        using var fixture = new TestWorkspace();
        var flagPath = WriteLookup(fixture, "<AttachmentPoint><id>0</id><name>None</name></AttachmentPoint>"
            + "<AttachmentPoint><id>1</id><name>Small</name></AttachmentPoint>"
            + "<AttachmentPoint><id>2</id><name>Medium</name></AttachmentPoint>"
            + "<AttachmentPoint><id>3</id><name>All</name></AttachmentPoint>"
            + "<AttachmentPoint><id>4</id><name>Hydration</name></AttachmentPoint>"
            + "<AttachmentPoint><id>7</id><name>All and carrier</name></AttachmentPoint>"
            + "<AttachmentPoint><id>8</id><name>Mask</name></AttachmentPoint>"
            + "<AttachmentPoint><id>16</id><name>Batteries</name></AttachmentPoint>",
            "Lookup.AttachmentPoint");
        var flags = await LoadAsync(flagPath, LookupMetadata("Lookup.AttachmentPoint", isFlagLookup: true));
        Assert.Equal("32", flags.SuggestLookupId());

        var sequentialPath = WriteLookup(fixture, "<Cursor><id>0</id><name>Invalid</name></Cursor>"
            + "<Cursor><id>1</id><name>Normal</name></Cursor>");
        var sequential = await LoadAsync(sequentialPath, LookupMetadata());
        Assert.Equal("2", sequential.SuggestLookupId());
    }

    private static async Task<TableWorkspace> LoadAsync(string path, TableMetadata metadata)
    {
        var descriptor = new TableDescriptor(
            metadata.TableName,
            path,
            $"Lookup/{metadata.TableName.Value.Split('.')[1]}.xml",
            true,
            Array.Empty<OverlayDescriptor>());
        var load = await TableWorkspace.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!;
    }

    private static string WriteLookup(TestWorkspace fixture, string records, string tableName = "Lookup.Cursor")
    {
        var recordElement = tableName.Split('.')[1];
        var path = Path.Combine(fixture.DirectoryPath, recordElement + ".xml");
        File.WriteAllText(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<JA2Data>\n" + records + "\n</JA2Data>\n");
        return path;
    }

    private static TableMetadata LookupMetadata(string tableName = "Lookup.Cursor", bool isFlagLookup = false)
    {
        var name = TableName.Parse(tableName);
        return new TableMetadata(
            name,
            "JA2Data",
            name.Value.Split('.')[1],
            isLookup: true,
            recordKey: new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "id") }),
            columns: new[]
            {
                new ColumnMetadata("id", ColumnDataType.Integer, ColumnEditorKind.IntegerBox),
                new ColumnMetadata("name", ColumnDataType.String),
            },
            isFlagLookup: isFlagLookup);
    }
}
