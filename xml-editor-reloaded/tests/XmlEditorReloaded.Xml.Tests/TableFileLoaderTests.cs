using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;

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
    /// A table whose declared root has no child elements loads as editable, accepts its first record with
    /// key <c>0</c>, and the saved file reloads with that record.
    /// </summary>
    [Fact]
    public async Task Load_EmptyDeclaredRoot_LoadsEditableAndAcceptsFirstRecord()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "Items.xml");
        await File.WriteAllTextAsync(
            path,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <ITEMLIST></ITEMLIST>
            """);
        var loader = new TableFileLoader();
        var metadata = OverlayTestData.ItemsMetadata();

        var result = await loader.LoadAsync(Descriptor(path, "Items.Items"), metadata);

        Assert.True(result.IsSuccess);
        Assert.Equal(TableDocumentStatus.Loaded, result.Data!.Status);
        Assert.Empty(result.Data.Diagnostics);
        var document = result.Data.GetDocument<TableDocument>();
        Assert.NotNull(document);
        Assert.Empty(document.Records);

        var added = document.AddRecord();

        Assert.True(added.IsSuccess, added.ErrorText);
        var record = added.Data!;
        Assert.Equal("0", record.GetKey());

        var save = await new TableFileSaver().SaveAsync(document);

        Assert.True(save.IsSuccess, save.ErrorText);

        var reloaded = await loader.LoadAsync(Descriptor(path, "Items.Items"), metadata);

        Assert.True(reloaded.IsSuccess);
        Assert.Equal(TableDocumentStatus.Loaded, reloaded.Data!.Status);
        var reloadedDocument = reloaded.Data.GetDocument<TableDocument>();
        Assert.NotNull(reloadedDocument);
        Assert.Equal("0", Assert.Single(reloadedDocument.Records).GetKey());
    }

    /// <summary>
    /// Comments, processing instructions, and self-closing syntax are not child elements, so they still
    /// describe an editable empty table.
    /// </summary>
    /// <param name="body">XML body written to the table file.</param>
    [Theory]
    [InlineData("<ITEMLIST />")]
    [InlineData("<ITEMLIST><!-- no rows yet --></ITEMLIST>")]
    [InlineData("<ITEMLIST><?rows none?></ITEMLIST>")]
    public async Task Load_RootWithoutChildElements_LoadsEditable(string body)
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "Items.xml");
        await File.WriteAllTextAsync(path, body);
        var loader = new TableFileLoader();

        var result = await loader.LoadAsync(Descriptor(path, "Items.Items"), OverlayTestData.ItemsMetadata());

        Assert.True(result.IsSuccess);
        Assert.Equal(TableDocumentStatus.Loaded, result.Data!.Status);
        Assert.Empty(result.Data.Diagnostics);
        var document = result.Data.GetDocument<TableDocument>();
        Assert.NotNull(document);
        Assert.Empty(document.Records);
    }

    /// <summary>
    /// An empty lookup table loads as editable and its first record gets the next id and the empty name
    /// element required by the lookup file contract.
    /// </summary>
    [Fact]
    public async Task Load_EmptyLookupRoot_LoadsEditableAndAddsIdAndName()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "ItemClass.xml");
        await File.WriteAllTextAsync(
            path,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <JA2Data></JA2Data>
            """);
        var metadata = new TableMetadata(
            TableName.Parse("Lookup.ItemClass"),
            "JA2Data",
            "ItemClass",
            isLookup: true,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "id") }));
        var loader = new TableFileLoader();

        var result = await loader.LoadAsync(Descriptor(path, "Lookup.ItemClass"), metadata);

        Assert.True(result.IsSuccess);
        Assert.Equal(TableDocumentStatus.Loaded, result.Data!.Status);
        Assert.Empty(result.Data.Diagnostics);
        var document = result.Data.GetDocument<TableDocument>();
        Assert.NotNull(document);
        Assert.Empty(document.Records);

        var added = document.AddRecord();

        Assert.True(added.IsSuccess, added.ErrorText);
        var record = added.Data!;
        Assert.Equal(new[] { "id", "name" }, record.FieldNames);
        Assert.Equal("0", record.GetRawValue("id"));
        Assert.Equal(string.Empty, record.GetRawValue("name"));
    }

    /// <summary>
    /// Without metadata the record element name cannot be verified, so an empty root keeps the read-only
    /// missing-record diagnostic.
    /// </summary>
    [Fact]
    public async Task Load_EmptyRootWithoutMetadata_StaysReadOnly()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "Custom.xml");
        await File.WriteAllTextAsync(
            path,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <ITEMLIST></ITEMLIST>
            """);
        var loader = new TableFileLoader();

        var result = await loader.LoadAsync(Descriptor(path, "Custom.Empty"));

        Assert.True(result.IsSuccess);
        Assert.Equal(TableDocumentStatus.LoadedReadOnly, result.Data!.Status);
        Assert.Contains(result.Data.Diagnostics, diagnostic => diagnostic.Code == ErrorCodes.XmlRecordElementMissing);
        var document = result.Data.GetDocument<TableDocument>();
        Assert.NotNull(document);
        var added = document.AddRecord();
        Assert.False(added.IsSuccess);
        Assert.Equal(ErrorCodes.XmlNotSaveable, added.ErrorCode);
    }

    /// <summary>
    /// An empty root still has to declare the expected root name, so a mismatch keeps both the
    /// unexpected-root and the missing-record diagnostics.
    /// </summary>
    [Fact]
    public async Task Load_EmptyRootWithWrongMetadata_StaysReadOnly()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "Items.xml");
        await File.WriteAllTextAsync(
            path,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <ITEMLIST></ITEMLIST>
            """);
        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "WRONGROOT",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }));
        var loader = new TableFileLoader();

        var result = await loader.LoadAsync(Descriptor(path, "Items.Items"), metadata);

        Assert.True(result.IsSuccess);
        Assert.Equal(TableDocumentStatus.LoadedReadOnly, result.Data!.Status);
        Assert.Contains(result.Data.Diagnostics, diagnostic => diagnostic.Code == ErrorCodes.XmlUnexpectedRoot);
        Assert.Contains(result.Data.Diagnostics, diagnostic => diagnostic.Code == ErrorCodes.XmlRecordElementMissing);
    }

    /// <summary>
    /// A root with child elements that are not records still reports the record element as missing and
    /// stays read-only.
    /// </summary>
    [Fact]
    public async Task Load_RootWithForeignChild_MatchingMetadata_StaysReadOnly()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "Items.xml");
        await File.WriteAllTextAsync(
            path,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <ITEMLIST><UNKNOWN /></ITEMLIST>
            """);
        var loader = new TableFileLoader();

        var result = await loader.LoadAsync(Descriptor(path, "Items.Items"), OverlayTestData.ItemsMetadata());

        Assert.True(result.IsSuccess);
        Assert.Equal(TableDocumentStatus.LoadedReadOnly, result.Data!.Status);
        Assert.Contains(result.Data.Diagnostics, diagnostic => diagnostic.Code == ErrorCodes.XmlRecordElementMissing);
        Assert.DoesNotContain(result.Data.Diagnostics, diagnostic => diagnostic.Code == ErrorCodes.XmlUnexpectedRoot);
        var document = result.Data.GetDocument<TableDocument>();
        Assert.NotNull(document);
        Assert.Empty(document.Records);
        Assert.False(document.AddRecord().IsSuccess);
    }

    /// <summary>
    /// A legacy item-transformations record stores its result key as repeated <c>usResult</c> elements
    /// instead of numbered fields. The loader reports it without changing a byte of the file.
    /// </summary>
    [Fact]
    public async Task Load_LegacyRepeatedResult_ReportsDiagnosticWithoutMutation()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "Item_Transformations.xml");
        await File.WriteAllTextAsync(
            path,
            """
            <?xml version="1.0" encoding="utf-8"?><TRANSFORMATIONS_LIST>
            <TRANSFORM><usItem>1</usItem><usResult>2</usResult><usResult>3</usResult></TRANSFORM>
            </TRANSFORMATIONS_LIST>
            """);
        var before = await File.ReadAllBytesAsync(path);
        var loader = new TableFileLoader();

        var result = await loader.LoadAsync(Descriptor(path, "Items.Item_Transformations"), TransformationMetadata());

        Assert.True(result.IsSuccess);
        Assert.Equal(TableDocumentStatus.Loaded, result.Data!.Status);
        var diagnostic = Assert.Single(result.Data.Diagnostics);
        Assert.Equal(ErrorCodes.XmlLegacyRepeatedKey, diagnostic.Code);
        Assert.Contains("usResult", diagnostic.Message, StringComparison.Ordinal);
        Assert.NotNull(diagnostic.Line);
        Assert.Equal(before, await File.ReadAllBytesAsync(path));
    }

    /// <summary>
    /// The numbered representation used by v4/v5 produces no legacy-format diagnostic.
    /// </summary>
    [Fact]
    public async Task Load_NumberedResult_HasNoLegacyDiagnostic()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "Item_Transformations.xml");
        await File.WriteAllTextAsync(
            path,
            """
            <?xml version="1.0" encoding="utf-8"?><TRANSFORMATIONS_LIST>
            <TRANSFORM><usItem>1</usItem><usResult1>2</usResult1><usResult2>3</usResult2></TRANSFORM>
            </TRANSFORMATIONS_LIST>
            """);
        var loader = new TableFileLoader();

        var result = await loader.LoadAsync(Descriptor(path, "Items.Item_Transformations"), TransformationMetadata());

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(
            result.Data!.Diagnostics,
            diagnostic => diagnostic.Code == ErrorCodes.XmlLegacyRepeatedKey);
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

    private static TableDescriptor Descriptor(string path, string tableName) => new(
        TableName.Parse(tableName),
        path,
        tableName.Replace('.', '/') + ".xml",
        IsMapped: true,
        Array.Empty<Core.Localization.OverlayDescriptor>());

    private static TableMetadata TransformationMetadata() => new(
        TableName.Parse("Items.Item_Transformations"),
        "TRANSFORMATIONS_LIST",
        "TRANSFORM",
        isLookup: false,
        recordKeyFields: new[]
        {
            "usItem",
            "usResult1",
            "usResult2",
            "usResult3",
            "usResult4",
            "usResult5",
            "usResult6",
            "usResult7",
            "usResult8",
            "usResult9",
            "usResult10",
        });
}
