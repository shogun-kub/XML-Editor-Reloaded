using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Lookups;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="LookupUsageService"/>.
/// </summary>
public class LookupUsageServiceTests
{
    /// <summary>
    /// A plain lookup id is matched exactly and the record key and field are reported.
    /// </summary>
    [Fact]
    public async Task FindUsages_PlainLookup_ReturnsMatchingRecords()
    {
        using var workspace = new TestWorkspace();
        var path = WriteTable(
            workspace,
            "Items.xml",
            "<ITEMLIST>"
            + "<ITEM><uiIndex>1</uiIndex><ubCursor>1</ubCursor></ITEM>"
            + "<ITEM><uiIndex>2</uiIndex><ubCursor>2</ubCursor></ITEM>"
            + "<ITEM><uiIndex>3</uiIndex><ubCursor>1</ubCursor></ITEM>"
            + "</ITEMLIST>");
        var items = ItemsMetadata();
        var service = new LookupUsageService(new StubMetadataProvider(items));

        var report = await service.FindUsagesAsync(
            new[] { Descriptor(TableName.Parse("Items.Items"), path) },
            TableName.Parse("Lookup.Cursor"),
            lookupId: 1,
            isFlagLookup: false);

        Assert.True(report.IsSuccess, report.ErrorText);
        Assert.Equal(2, report.Data!.UsageCount);
        Assert.True(report.Data.IsUsed);
        Assert.Equal(1, report.Data.ScannedTableCount);
        Assert.Equal(0, report.Data.FailedTableCount);
        Assert.Collection(
            report.Data.Usages,
            usage =>
            {
                Assert.Equal("Items.Items", usage.TableName.Value);
                Assert.Equal("1", usage.RecordKey);
                Assert.Equal("ubCursor", usage.FieldPath);
                Assert.Equal("1", usage.Value);
            },
            usage => Assert.Equal("3", usage.RecordKey));
    }

    /// <summary>
    /// A missing id produces an empty report and not a failure.
    /// </summary>
    [Fact]
    public async Task FindUsages_UnusedId_ReportsZero()
    {
        using var workspace = new TestWorkspace();
        var path = WriteTable(
            workspace,
            "Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubCursor>1</ubCursor></ITEM></ITEMLIST>");
        var service = new LookupUsageService(new StubMetadataProvider(ItemsMetadata()));

        var report = await service.FindUsagesAsync(
            new[] { Descriptor(TableName.Parse("Items.Items"), path) },
            TableName.Parse("Lookup.Cursor"),
            lookupId: 99,
            isFlagLookup: false);

        Assert.True(report.IsSuccess, report.ErrorText);
        Assert.False(report.Data!.IsUsed);
        Assert.Empty(report.Data.Usages);
    }

    /// <summary>
    /// A bit-mask lookup matches a single-bit entry by the bit and a named combination only exactly, so
    /// a combination used as part of a larger mask is not reported as that combination.
    /// </summary>
    [Fact]
    public async Task FindUsages_FlagLookup_UsesBitAndExactRules()
    {
        using var workspace = new TestWorkspace();
        var path = WriteTable(
            workspace,
            "Items.xml",
            "<ITEMLIST>"
            + "<ITEM><uiIndex>1</uiIndex><AttachmentPoint>1</AttachmentPoint></ITEM>"
            + "<ITEM><uiIndex>2</uiIndex><AttachmentPoint>3</AttachmentPoint></ITEM>"
            + "<ITEM><uiIndex>3</uiIndex><AttachmentPoint>7</AttachmentPoint></ITEM>"
            + "<ITEM><uiIndex>4</uiIndex><AttachmentPoint>0</AttachmentPoint></ITEM>"
            + "</ITEMLIST>");
        var service = new LookupUsageService(new StubMetadataProvider(FlagItemsMetadata()));

        var singleBit = await service.FindUsagesAsync(
            new[] { Descriptor(TableName.Parse("Items.Items"), path) },
            TableName.Parse("Lookup.AttachmentPoint"),
            lookupId: 1,
            isFlagLookup: true);
        var combination = await service.FindUsagesAsync(
            new[] { Descriptor(TableName.Parse("Items.Items"), path) },
            TableName.Parse("Lookup.AttachmentPoint"),
            lookupId: 3,
            isFlagLookup: true);
        var zero = await service.FindUsagesAsync(
            new[] { Descriptor(TableName.Parse("Items.Items"), path) },
            TableName.Parse("Lookup.AttachmentPoint"),
            lookupId: 0,
            isFlagLookup: true);

        Assert.Equal(3, singleBit.Data!.UsageCount);
        Assert.Equal(new[] { "1", "2", "3" }, singleBit.Data.Usages.Select(static usage => usage.RecordKey));
        Assert.Equal(1, combination.Data!.UsageCount);
        Assert.Equal("2", Assert.Single(combination.Data.Usages).RecordKey);
        Assert.Equal(1, zero.Data!.UsageCount);
        Assert.Equal("4", Assert.Single(zero.Data.Usages).RecordKey);
    }

    /// <summary>
    /// Tables that do not reference the lookup and descriptors without metadata are not scanned.
    /// </summary>
    [Fact]
    public async Task FindUsages_SkipsUnrelatedAndUnmappedTables()
    {
        using var workspace = new TestWorkspace();
        var itemsPath = WriteTable(
            workspace,
            "Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubCursor>1</ubCursor></ITEM></ITEMLIST>");
        var otherPath = WriteTable(
            workspace,
            "Other.xml",
            "<OTHERLIST><OTHER><id>1</id><value>1</value></OTHER></OTHERLIST>");
        var unknownPath = WriteTable(
            workspace,
            "Unknown.xml",
            "<UNKNOWNLIST><UNKNOWN><id>1</id><value>1</value></UNKNOWN></UNKNOWNLIST>");
        var service = new LookupUsageService(new StubMetadataProvider(ItemsMetadata(), OtherMetadata()));

        var report = await service.FindUsagesAsync(
            new[]
            {
                Descriptor(TableName.Parse("Items.Items"), itemsPath),
                Descriptor(TableName.Parse("Items.Other"), otherPath),
                Descriptor(TableName.Parse("Unknown.Table"), unknownPath),
            },
            TableName.Parse("Lookup.Cursor"),
            lookupId: 1,
            isFlagLookup: false);

        Assert.True(report.IsSuccess, report.ErrorText);
        Assert.Equal(1, report.Data!.ScannedTableCount);
        Assert.Single(report.Data.Usages);
    }

    /// <summary>
    /// The retained list is capped per table while the exact count still covers every match.
    /// </summary>
    [Fact]
    public async Task FindUsages_CapsDetailsAndKeepsExactCount()
    {
        using var workspace = new TestWorkspace();
        var records = string.Concat(Enumerable.Range(1, 250)
            .Select(index => $"<ITEM><uiIndex>{index}</uiIndex><ubCursor>1</ubCursor></ITEM>"));
        var path = WriteTable(workspace, "Items.xml", $"<ITEMLIST>{records}</ITEMLIST>");
        var service = new LookupUsageService(new StubMetadataProvider(ItemsMetadata()));

        var report = await service.FindUsagesAsync(
            new[] { Descriptor(TableName.Parse("Items.Items"), path) },
            TableName.Parse("Lookup.Cursor"),
            lookupId: 1,
            isFlagLookup: false);

        Assert.Equal(250, report.Data!.UsageCount);
        Assert.Equal(LookupUsageService.MaxUsagesPerTable, report.Data.Usages.Count);
        Assert.Equal(250 - LookupUsageService.MaxUsagesPerTable, report.Data.SuppressedUsageCount);
    }

    /// <summary>
    /// A malformed referencing table is counted as failed and does not abort the scan.
    /// </summary>
    [Fact]
    public async Task FindUsages_MalformedTable_IsCountedAsFailed()
    {
        using var workspace = new TestWorkspace();
        var path = WriteTable(workspace, "Items.xml", "<ITEMLIST><ITEM><uiIndex>1</uiIndex></ITEMLIST>");
        var service = new LookupUsageService(new StubMetadataProvider(ItemsMetadata()));

        var report = await service.FindUsagesAsync(
            new[] { Descriptor(TableName.Parse("Items.Items"), path) },
            TableName.Parse("Lookup.Cursor"),
            lookupId: 1,
            isFlagLookup: false);

        Assert.True(report.IsSuccess, report.ErrorText);
        Assert.Equal(1, report.Data!.FailedTableCount);
        Assert.Equal(0, report.Data.UsageCount);
    }

    /// <summary>
    /// Without metadata the scan cannot discover referencing columns and fails closed.
    /// </summary>
    [Fact]
    public async Task FindUsages_WithoutMetadata_Fails()
    {
        using var workspace = new TestWorkspace();
        var path = WriteTable(
            workspace,
            "Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubCursor>1</ubCursor></ITEM></ITEMLIST>");
        var service = new LookupUsageService();

        var report = await service.FindUsagesAsync(
            new[] { Descriptor(TableName.Parse("Items.Items"), path) },
            TableName.Parse("Lookup.Cursor"),
            lookupId: 1,
            isFlagLookup: false);

        Assert.Equal(ErrorCodes.MetadataNotFound, report.ErrorCode);
    }

    private static TableDescriptor Descriptor(TableName tableName, string path) => new(
        tableName,
        path,
        $"Items/{tableName.Value.Split('.')[1]}.xml",
        true,
        Array.Empty<OverlayDescriptor>());

    private static string WriteTable(TestWorkspace workspace, string fileName, string body)
    {
        var path = Path.Combine(workspace.DirectoryPath, fileName);
        File.WriteAllText(path, body);
        return path;
    }

    private static TableMetadata ItemsMetadata() => new(
        TableName.Parse("Items.Items"),
        "ITEMLIST",
        "ITEM",
        isLookup: false,
        recordKey: new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
        columns: new[]
        {
            new ColumnMetadata("uiIndex", ColumnDataType.Integer, ColumnEditorKind.IntegerBox),
            new ColumnMetadata("ubCursor", ColumnDataType.Lookup, ColumnEditorKind.LookupCombo)
            {
                LookupTable = "Lookup.Cursor",
            },
        });

    private static TableMetadata OtherMetadata() => new(
        TableName.Parse("Items.Other"),
        "OTHERLIST",
        "OTHER",
        isLookup: false,
        recordKey: new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "id") }),
        columns: new[]
        {
            new ColumnMetadata("id", ColumnDataType.Integer, ColumnEditorKind.IntegerBox),
            new ColumnMetadata("value", ColumnDataType.Integer, ColumnEditorKind.IntegerBox),
        });

    private static TableMetadata FlagItemsMetadata() => new(
        TableName.Parse("Items.Items"),
        "ITEMLIST",
        "ITEM",
        isLookup: false,
        recordKey: new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
        columns: new[]
        {
            new ColumnMetadata("uiIndex", ColumnDataType.Integer, ColumnEditorKind.IntegerBox),
            new ColumnMetadata("AttachmentPoint", ColumnDataType.Flags, ColumnEditorKind.FlagEditor)
            {
                LookupTable = "Lookup.AttachmentPoint",
            },
        });

    private sealed class StubMetadataProvider(params TableMetadata[] tables) : ITableMetadataProvider
    {
        public Task<OperationResult<TableMetadata>> GetTableAsync(
            TableName tableName,
            CancellationToken cancellationToken = default)
        {
            var metadata = tables.FirstOrDefault(table => table.TableName == tableName);
            return Task.FromResult(metadata is null
                ? OperationResult<TableMetadata>.Failure(
                    ErrorCodes.MetadataNotFound,
                    $"No metadata for '{tableName.Value}'.")
                : OperationResult<TableMetadata>.Success(metadata));
        }

        public Task<OperationResult<IReadOnlyList<TableMetadata>>> GetAllTablesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<TableMetadata>>.Success(tables));

        public Task<OperationResult<IReadOnlyList<LanguageDescriptor>>> GetLanguagesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<LanguageDescriptor>>.Success(
                new[] { new LanguageDescriptor("ru", "Russian", "Russian", 1) }));

        public Task<OperationResult<IReadOnlyList<TableRelation>>> GetRelationsAsync(
            TableName sourceTable,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<TableRelation>>.Success(Array.Empty<TableRelation>()));

        public Task<OperationResult<IReadOnlyList<LookupEntry>>> GetLookupValuesAsync(
            TableName lookupName,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<LookupEntry>>.Success(Array.Empty<LookupEntry>()));
    }
}
