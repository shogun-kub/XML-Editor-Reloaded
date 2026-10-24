using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="TableFileLoader"/>.
/// </summary>
public class TableFileLoaderTests
{
    /// <summary>
    /// A valid file loads with sparse records, empty elements, repeated fields, and unknown fields preserved.
    /// </summary>
    [Fact]
    public async Task Load_ValidFile_ExposesSparseRecords()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var loader = new TableFileLoader();
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: false,
            Array.Empty<Core.Localization.OverlayDescriptor>());

        var result = await loader.LoadAsync(descriptor);

        Assert.True(result.IsSuccess);
        var load = result.Data!;
        Assert.Equal(TableDocumentStatus.Loaded, load.Status);
        var document = load.GetDocument<TableDocument>();
        Assert.NotNull(document);
        Assert.Equal(2, document.Records.Count);

        var first = document.Records[0];
        Assert.True(first.HasField("spreadPattern"));
        Assert.True(first.GetValue("spreadPattern")!.IsEmpty);
        Assert.Equal("keep me", first.GetRawValue("CustomUnknown"));

        var second = document.Records[1];
        Assert.False(second.HasField("spreadPattern"));
        Assert.Equal("1.0", second.GetRawValue("ProjectionFactor"));
        Assert.Equal(2, second.GetValues("AvailableAttachmentPoint").Count);
        Assert.Equal("1", second.GetValues("AvailableAttachmentPoint")[0].RawText);
    }

    /// <summary>
    /// A malformed document is reported as a successful load with a malformed status and a parse diagnostic.
    /// </summary>
    [Fact]
    public async Task Load_MalformedFile_ReturnsMalformedStatus()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("malformed.xml");
        var loader = new TableFileLoader();
        var descriptor = new TableDescriptor(
            TableName.Parse("Lookup.Malformed"),
            path,
            "Lookup/Malformed.xml",
            IsMapped: false,
            Array.Empty<Core.Localization.OverlayDescriptor>());

        var result = await loader.LoadAsync(descriptor);

        Assert.True(result.IsSuccess);
        Assert.Equal(TableDocumentStatus.Malformed, result.Data!.Status);
        Assert.Null(result.Data.Document);
        var diagnostic = Assert.Single(result.Data.Diagnostics);
        Assert.Equal(ErrorCodes.XmlParseError, diagnostic.Code);
        Assert.NotNull(diagnostic.Line);
    }

    /// <summary>
    /// A missing file is an operation failure, not a malformed document.
    /// </summary>
    [Fact]
    public async Task Load_MissingFile_ReturnsFileNotFound()
    {
        var loader = new TableFileLoader();
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Missing"),
            Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "Missing.xml"),
            "Items/Missing.xml",
            IsMapped: false,
            Array.Empty<Core.Localization.OverlayDescriptor>());

        var result = await loader.LoadAsync(descriptor);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.IoFileNotFound, result.ErrorCode);
    }

    /// <summary>
    /// Metadata whose root does not match keeps the document readable but not saveable.
    /// </summary>
    [Fact]
    public async Task Load_MetadataRootMismatch_ReturnsReadOnly()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "WRONGROOT",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }));
        var loader = new TableFileLoader();
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<Core.Localization.OverlayDescriptor>());

        var result = await loader.LoadAsync(descriptor, metadata);

        Assert.True(result.IsSuccess);
        Assert.Equal(TableDocumentStatus.LoadedReadOnly, result.Data!.Status);
        Assert.Contains(result.Data.Diagnostics, diagnostic => diagnostic.Code == ErrorCodes.XmlUnexpectedRoot);
    }

    /// <summary>
    /// BOM and CRLF are detected and exposed through the document format.
    /// </summary>
    [Fact]
    public async Task Load_BomFile_DetectsFormat()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini-bom.xml");
        var loader = new TableFileLoader();
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: false,
            Array.Empty<Core.Localization.OverlayDescriptor>());

        var result = await loader.LoadAsync(descriptor);

        var document = result.Data!.GetDocument<TableDocument>();
        Assert.NotNull(document);
        Assert.True(document.Format.HasByteOrderMark);
        Assert.Equal("\r\n", document.Format.NewLine);
        Assert.Equal("utf-8", document.Format.EncodingName);
    }
}
