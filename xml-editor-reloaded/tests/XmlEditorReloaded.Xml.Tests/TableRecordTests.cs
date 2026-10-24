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
