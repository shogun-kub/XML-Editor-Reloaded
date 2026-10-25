using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="TableRecord"/> and <see cref="TableDocument"/> sparse editing behavior.
/// </summary>
public class TableRecordTests
{
    /// <summary>
    /// Setting an absent field without an explicit add request leaves the record unchanged.
    /// </summary>
    [Fact]
    public async Task TrySetValue_MissingFieldWithoutAdd_DoesNotAddIt()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        var record = document.Records[1];

        var changed = record.TrySetValue("MissingField", "value");

        Assert.False(changed);
        Assert.False(record.HasField("MissingField"));
        Assert.False(document.IsDirty);
    }

    /// <summary>
    /// An explicit add request appends the field at the end of the record and marks the document dirty.
    /// </summary>
    [Fact]
    public async Task TrySetValue_MissingFieldWithAdd_AppendsField()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        var record = document.Records[1];

        var changed = record.TrySetValue("MissingField", "value", addIfMissing: true);

        Assert.True(changed);
        Assert.True(record.HasField("MissingField"));
        Assert.Equal("value", record.GetRawValue("MissingField"));
        Assert.True(document.IsDirty);
    }

    /// <summary>
    /// A non-declaring column hint enforces the numeric type on the backend edit path.
    /// </summary>
    [Fact]
    public async Task SetFieldValue_HintType_RejectsInvalidNumber()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            columnHints: new[] { new ColumnHint("ubWeight") { DataType = ColumnDataType.Integer } });
        var document = await LoadAsync(path, metadata);
        var record = document.Records[1];

        var result = record.SetFieldValue("ubWeight", "abc");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.XmlInvalidNumber, result.ErrorCode);
        Assert.Equal("10", record.GetRawValue("ubWeight"));
        Assert.False(document.IsDirty);
    }

    /// <summary>
    /// Flag masks use the full unsigned 64-bit range and reject values beyond it.
    /// </summary>
    [Fact]
    public async Task SetFieldValue_FlagHint_AcceptsUInt64Range()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            columnHints: new[] { new ColumnHint("usItemClass") { DataType = ColumnDataType.Flags } });
        var document = await LoadAsync(path, metadata);
        var record = document.Records[0];

        var accepted = record.SetFieldValue("usItemClass", "8589934592", addIfMissing: true);
        var maximum = record.SetFieldValue("usItemClass", "18446744073709551615");
        var rejected = record.SetFieldValue("usItemClass", "18446744073709551616");

        Assert.True(accepted.IsSuccess, accepted.ErrorText);
        Assert.True(maximum.IsSuccess, maximum.ErrorText);
        Assert.False(rejected.IsSuccess);
        Assert.Equal(ErrorCodes.XmlInvalidNumber, rejected.ErrorCode);
        Assert.Equal("18446744073709551615", record.GetRawValue("usItemClass"));
    }

    /// <summary>
    /// The next primary key is the maximum integer key plus one.
    /// </summary>
    [Fact]
    public async Task GetNextPrimaryKey_ReturnsMaximumPlusOne()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }));
        var document = await LoadAsync(path, metadata);

        Assert.Equal(2, document.GetNextPrimaryKey());
    }

    private static async Task<TableDocument> LoadAsync(string path, TableMetadata? metadata = null)
    {
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: metadata is not null,
            Array.Empty<OverlayDescriptor>());
        var load = await new TableFileLoader().LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }
}
