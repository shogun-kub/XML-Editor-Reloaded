using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="TableFileSaver"/> and <see cref="DocumentSerializer"/>.
/// </summary>
public class TableFileSaverTests
{
    private static readonly TableFileLoader Loader = new();
    private static readonly TableFileSaver Saver = new();

    /// <summary>
    /// Saving a document without edits must not touch the file.
    /// </summary>
    [Fact]
    public async Task Save_UnmodifiedDocument_DoesNotRewriteFile()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var before = await File.ReadAllBytesAsync(path);
        var lastWrite = File.GetLastWriteTimeUtc(path);
        var document = await LoadAsync(path);

        var result = await Saver.SaveAsync(document);

        Assert.True(result.IsSuccess);
        Assert.Equal(before, await File.ReadAllBytesAsync(path));
        Assert.Equal(lastWrite, File.GetLastWriteTimeUtc(path));
        Assert.False(document.IsDirty);
    }

    /// <summary>
    /// Serializing an unedited document should reproduce the source bytes.
    /// </summary>
    [Fact]
    public async Task Serialize_UnmodifiedDocument_MatchesSourceBytes()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);

        var bytes = new DocumentSerializer().Serialize(document);

        Assert.Equal(await File.ReadAllBytesAsync(path), bytes);
    }

    /// <summary>
    /// Editing one field rewrites only that value and preserves every other element.
    /// </summary>
    [Fact]
    public async Task Save_EditedValue_ChangesOnlyThatValue()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        var second = document.Records[1];

        Assert.True(second.TrySetValue("szItemName", "Glock 18"));
        Assert.True(document.IsDirty);

        var result = await Saver.SaveAsync(document);

        Assert.True(result.IsSuccess);
        Assert.False(document.IsDirty);
        var saved = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        var items = saved.Root!.Elements("ITEM").ToArray();
        Assert.Equal("Glock 18", items[1].Element("szItemName")!.Value);
        Assert.Equal("1.0", items[1].Element("ProjectionFactor")!.Value);
        Assert.Equal("keep me", items[0].Element("CustomUnknown")!.Value);
        Assert.Equal(string.Empty, items[0].Element("spreadPattern")!.Value);
        Assert.Equal(2, items[1].Elements("AvailableAttachmentPoint").Count());
    }

    /// <summary>
    /// A CRLF source keeps CRLF after a save.
    /// </summary>
    [Fact]
    public async Task Save_CrlfSource_PreservesNewlines()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        document.Records[0].TrySetValue("szItemName", "Nothing 2");

        Assert.True((await Saver.SaveAsync(document)).IsSuccess);

        var bytes = await File.ReadAllBytesAsync(path);
        for (var index = 0; index < bytes.Length; index++)
        {
            if (bytes[index] == (byte)'\n')
            {
                Assert.True(index > 0 && bytes[index - 1] == (byte)'\r', "A lone LF was written to a CRLF file.");
            }
        }
    }

    /// <summary>
    /// An LF-only source keeps LF after a save.
    /// </summary>
    [Fact]
    public async Task Save_LfSource_PreservesNewlines()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini-lf.xml");
        var document = await LoadAsync(path);
        document.Records[0].TrySetValue("szItemName", "Nothing 2");

        Assert.True((await Saver.SaveAsync(document)).IsSuccess);

        var bytes = await File.ReadAllBytesAsync(path);
        Assert.DoesNotContain((byte)'\r', bytes);
    }

    /// <summary>
    /// A UTF-8 BOM is preserved after a save.
    /// </summary>
    [Fact]
    public async Task Save_BomSource_PreservesBom()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini-bom.xml");
        var document = await LoadAsync(path);
        document.Records[0].TrySetValue("szItemName", "Nothing 2");

        Assert.True((await Saver.SaveAsync(document)).IsSuccess);

        var bytes = await File.ReadAllBytesAsync(path);
        Assert.True(bytes.Length >= 3);
        Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, bytes[..3]);
    }

    /// <summary>
    /// A file changed after loading is not overwritten.
    /// </summary>
    [Fact]
    public async Task Save_ExternallyChangedFile_IsRefused()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        document.Records[0].TrySetValue("szItemName", "Changed");
        await File.AppendAllTextAsync(path, " ");

        var result = await Saver.SaveAsync(document);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.XmlFileChangedExternally, result.ErrorCode);
    }

    /// <summary>
    /// A document loaded read-only cannot be saved.
    /// </summary>
    [Fact]
    public async Task Save_ReadOnlyDocument_IsRefused()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "WRONGROOT",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }));
        var descriptor = Descriptor("Items.Items", path, "Items/Items.xml");
        var load = await Loader.LoadAsync(descriptor, metadata);
        var document = load.Data!.GetDocument<TableDocument>()!;
        document.Records[0].TrySetValue("szItemName", "Changed");

        var result = await Saver.SaveAsync(document);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.XmlNotSaveable, result.ErrorCode);
    }

    private static async Task<TableDocument> LoadAsync(string path)
    {
        var load = await Loader.LoadAsync(Descriptor("Items.Items", path, "Items/Items.xml"));
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }

    private static TableDescriptor Descriptor(string tableName, string path, string relativePath) =>
        new(TableName.Parse(tableName), path, relativePath, IsMapped: false, Array.Empty<OverlayDescriptor>());
}
