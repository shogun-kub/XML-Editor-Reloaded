using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// M4 tests for validated field editing, list/dict fields, record operations, and the change journal.
/// </summary>
public class EditingTests
{
    /// <summary>
    /// An invalid integer is rejected without mutating the document.
    /// </summary>
    [Fact]
    public async Task SetFieldValue_InvalidInteger_IsRejected()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        var record = document.Records[1];

        var result = record.SetFieldValue("ubWeight", "not-a-number");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.XmlInvalidNumber, result.ErrorCode);
        Assert.Equal("10", record.GetRawValue("ubWeight"));
        Assert.False(document.IsDirty);
        Assert.Empty(document.ChangeTracker.Changes);
    }

    /// <summary>
    /// A valid integer is accepted and the exact text is preserved.
    /// </summary>
    [Fact]
    public async Task SetFieldValue_ValidInteger_IsAccepted()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        var record = document.Records[1];

        var result = record.SetFieldValue("ubWeight", "11");

        Assert.True(result.IsSuccess);
        Assert.Equal("11", record.GetRawValue("ubWeight"));
        Assert.True(document.IsDirty);
        var change = Assert.IsType<FieldValueChanged>(Assert.Single(document.ChangeTracker.Changes));
        Assert.Equal("10", change.OldValue);
    }

    /// <summary>
    /// Decimal lexical forms are validated and stored without normalization.
    /// </summary>
    [Fact]
    public async Task SetFieldValue_Decimal_PreservesLexicalForm()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);

        var result = document.Records[1].SetFieldValue("ProjectionFactor", "1.00");

        Assert.True(result.IsSuccess);
        Assert.Equal("1.00", document.Records[1].GetRawValue("ProjectionFactor"));
    }

    /// <summary>
    /// A missing field is not added unless explicitly requested.
    /// </summary>
    [Fact]
    public async Task SetFieldValue_MissingField_RequiresExplicitAdd()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        var record = document.Records[0];

        var missing = record.SetFieldValue("NewField", "value");
        Assert.False(missing.IsSuccess);
        Assert.Equal(ErrorCodes.KeyNotFound, missing.ErrorCode);

        var added = record.SetFieldValue("NewField", "value", addIfMissing: true);
        Assert.True(added.IsSuccess);
        Assert.Equal("value", record.GetRawValue("NewField"));
        var change = Assert.IsType<FieldValueChanged>(Assert.Single(document.ChangeTracker.Changes));
        Assert.Null(change.OldValue);
    }

    /// <summary>
    /// Replacing a repeated field updates only that field's occurrences.
    /// </summary>
    [Fact]
    public async Task SetListField_ReplacesOnlyTheList()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        var record = document.Records[1];

        var result = record.SetListField("AvailableAttachmentPoint", new[] { "3", "4" });

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(new[] { "3", "4" }, record.GetValues("AvailableAttachmentPoint").Select(value => value.RawText));
        Assert.Equal("Glock 17", record.GetRawValue("szItemName"));
        Assert.True(document.IsDirty);
        var change = Assert.IsType<ListFieldChanged>(Assert.Single(document.ChangeTracker.Changes));
        Assert.Equal(new[] { "1", "2" }, change.OldValues);
        Assert.Equal(new[] { "3", "4" }, change.NewValues);
    }

    /// <summary>
    /// Invalid list values are rejected before any mutation.
    /// </summary>
    [Fact]
    public async Task SetListField_InvalidValue_IsRejected()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        var record = document.Records[1];

        var result = record.SetListField("AvailableAttachmentPoint", new[] { "3", "bad" });

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.XmlInvalidNumber, result.ErrorCode);
        Assert.Equal(new[] { "1", "2" }, record.GetValues("AvailableAttachmentPoint").Select(value => value.RawText));
        Assert.False(document.IsDirty);
    }

    /// <summary>
    /// A dictionary-mask field can be added by index.
    /// </summary>
    [Fact]
    public async Task SetDictField_AddsIndexedField()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        var record = document.Records[0];

        var result = record.SetDictField("ItemCapacityPerSize%Index%", 3, "5", addIfMissing: true);

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal("5", record.GetRawValue("ItemCapacityPerSize3"));
        Assert.True(document.IsDirty);
    }

    /// <summary>
    /// Add, duplicate, and delete maintain document order, keys, and the change journal.
    /// </summary>
    [Fact]
    public async Task RecordOperations_AddDuplicateDelete_UpdateDocument()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);

        var added = document.AddRecord();
        Assert.True(added.IsSuccess);
        Assert.Equal("2", added.Data!.GetRawValue("uiIndex"));
        Assert.Equal(3, document.Records.Count);

        var duplicate = document.DuplicateRecord("1");
        Assert.True(duplicate.IsSuccess);
        Assert.Equal("3", duplicate.Data!.GetRawValue("uiIndex"));
        Assert.Equal("Glock 17", duplicate.Data.GetRawValue("szItemName"));
        Assert.Equal(4, document.Records.Count);

        var deleted = document.DeleteRecord("0");
        Assert.True(deleted.IsSuccess);
        Assert.Equal(3, document.Records.Count);
        Assert.Null(document.FindRecord("0"));

        Assert.Collection(
            document.ChangeTracker.Changes,
            change => Assert.IsType<RecordAdded>(change),
            change => Assert.IsType<RecordDuplicated>(change),
            change => Assert.IsType<RecordDeleted>(change));
    }

    /// <summary>
    /// Structural operations produce valid XML when saved.
    /// </summary>
    [Fact]
    public async Task RecordOperations_Save_ProducesValidXml()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        document.AddRecord();
        document.DuplicateRecord("1");
        document.DeleteRecord("0");

        var result = await new TableFileSaver().SaveAsync(document);

        Assert.True(result.IsSuccess, result.ErrorText);
        var saved = System.Xml.Linq.XDocument.Load(path);
        Assert.Equal(3, saved.Root!.Elements("ITEM").Count());
    }

    private static async Task<TableDocument> LoadAsync(string path)
    {
        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            columns: new[]
            {
                new ColumnMetadata("szItemName", ColumnDataType.String),
                new ColumnMetadata("ubWeight", ColumnDataType.Integer),
                new ColumnMetadata("ProjectionFactor", ColumnDataType.Decimal),
                new ColumnMetadata("AvailableAttachmentPoint", ColumnDataType.Integer),
            },
            dictFields: new[] { new DictFieldDefinition("ItemCapacityPerSize%Index%") });

        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await new TableFileLoader().LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }
}
