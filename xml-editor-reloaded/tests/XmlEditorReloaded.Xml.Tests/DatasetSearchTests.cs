using System.Text;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Search;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="DatasetSearchService"/>.
/// </summary>
public class DatasetSearchTests
{
    /// <summary>
    /// Base values are found with the record key and the field path, and values of other records are
    /// not reported.
    /// </summary>
    [Fact]
    public async Task Search_FindsBaseValueWithKeyAndPath()
    {
        using var workspace = new TestWorkspace();
        var path = WriteTable(
            workspace,
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>Glock 17</szItemName></ITEM>"
            + "<ITEM><uiIndex>2</uiIndex><szItemName>Beretta</szItemName></ITEM></ITEMLIST>");
        var provider = new StubMetadataProvider(ItemMetadata());

        var report = await new DatasetSearchService(provider).SearchAsync(new[] { Mapped(TableName.Parse("Items.Items"), path, "Items/Items.xml") },
            "glock");

        var hit = Assert.Single(report.Hits);
        Assert.Equal("Items.Items", hit.TableName.Value);
        Assert.Equal("1", hit.RecordKey);
        Assert.Equal("szItemName", hit.FieldPath);
        Assert.Equal("Glock 17", hit.Value);
        Assert.Null(hit.LanguageCode);
        Assert.Equal(1, report.MatchCount);
        Assert.Equal(1, report.MatchedTableCount);
        Assert.Equal(0, report.SuppressedHitCount);
        Assert.Equal(1, report.SearchedTableCount);
    }

    /// <summary>
    /// Overlay values are searched too and carry the overlay language, so a translation is found next
    /// to the untranslated base value of the same record.
    /// </summary>
    [Fact]
    public async Task Search_FindsOverlayValueWithLanguage()
    {
        using var workspace = new TestWorkspace();
        var basePath = WriteTable(
            workspace,
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>Nothing</szItemName></ITEM></ITEMLIST>");
        var overlayPath = WriteTable(
            workspace,
            "Items/Russian.Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>Ничто</szItemName></ITEM></ITEMLIST>");
        var descriptor = Mapped(
            TableName.Parse("Items.Items"),
            basePath,
            "Items/Items.xml",
            new OverlayDescriptor(
                new LanguageDescriptor("ru", "Russian", "Russian", 1),
                basePath,
                overlayPath));

        var report = await new DatasetSearchService(new StubMetadataProvider(ItemMetadata()))
            .SearchAsync(new[] { descriptor }, "ничто");

        var hit = Assert.Single(report.Hits);
        Assert.Equal("ru", hit.LanguageCode);
        Assert.Equal("1", hit.RecordKey);
        Assert.Equal("szItemName", hit.FieldPath);
        Assert.Equal("Ничто", hit.Value);
    }

    /// <summary>
    /// Matching is case-insensitive by default and case-sensitive on request.
    /// </summary>
    [Fact]
    public async Task Search_MatchesCaseOnlyWhenRequested()
    {
        using var workspace = new TestWorkspace();
        var path = WriteTable(
            workspace,
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>Glock</szItemName></ITEM></ITEMLIST>");
        var tables = new[] { Mapped(TableName.Parse("Items.Items"), path, "Items/Items.xml") };
        var service = new DatasetSearchService(new StubMetadataProvider(ItemMetadata()));

        var insensitive = await service.SearchAsync(tables, "GLOCK");
        var sensitive = await service.SearchAsync(tables, "GLOCK", matchCase: true);

        Assert.Equal(1, insensitive.MatchCount);
        Assert.False(insensitive.MatchCase);
        Assert.True(sensitive.MatchCase);
        Assert.Equal(0, sensitive.MatchCount);
    }

    /// <summary>
    /// Nested elements keep their path relative to the record, and root-level singleton fields are
    /// found without a record key.
    /// </summary>
    [Fact]
    public async Task Search_FindsNestedAndRootPaths()
    {
        using var workspace = new TestWorkspace();
        var path = WriteTable(
            workspace,
            "Map/Cities.xml",
            "<CITY_INFO><COUNTRY><countryName>Arulco</countryName></COUNTRY><CITYLIST>"
            + "<CITY><uiIndex>1</uiIndex><townName>Omerta</townName>"
            + "<PRODUCTIONLINE><usItemToCreate>9</usItemToCreate></PRODUCTIONLINE></CITY>"
            + "</CITYLIST></CITY_INFO>");
        var provider = new StubMetadataProvider(CityMetadata());

        var report = await new DatasetSearchService(provider).SearchAsync(new[] { Mapped(TableName.Parse("Map.Cities"), path, "Map/Cities.xml") },
            "arulco");

        var rootHit = Assert.Single(report.Hits);
        Assert.Equal(string.Empty, rootHit.RecordKey);
        Assert.Equal("COUNTRY/countryName", rootHit.FieldPath);

        var nested = await new DatasetSearchService(provider).SearchAsync(new[] { Mapped(TableName.Parse("Map.Cities"), path, "Map/Cities.xml") },
            "9");
        var nestedHit = Assert.Single(nested.Hits);
        Assert.Equal("1", nestedHit.RecordKey);
        Assert.Equal("PRODUCTIONLINE/usItemToCreate", nestedHit.FieldPath);
    }

    /// <summary>
    /// An unmapped table is searched in document-driven mode: the first field of a record supplies the
    /// key the editor would use.
    /// </summary>
    [Fact]
    public async Task Search_UnmappedTable_UsesFirstFieldAsKey()
    {
        using var workspace = new TestWorkspace();
        var path = WriteTable(
            workspace,
            "Custom/Notes.xml",
            "<NOTES><NOTE><id>7</id><text>Hello world</text></NOTE></NOTES>");

        var report = await new DatasetSearchService().SearchAsync(new[] { Unmapped(TableName.Parse("Custom.Notes"), path, "Custom/Notes.xml") },
            "hello");

        var hit = Assert.Single(report.Hits);
        Assert.Equal("7", hit.RecordKey);
        Assert.Equal("text", hit.FieldPath);
        Assert.Equal("Hello world", hit.Value);
    }

    /// <summary>
    /// Hit details are capped per table while the total match count stays exact.
    /// </summary>
    [Fact]
    public async Task Search_CapsHitsButKeepsExactCount()
    {
        using var workspace = new TestWorkspace();
        var builder = new StringBuilder("<ITEMLIST><ITEM><uiIndex>1</uiIndex>");
        var fieldCount = DatasetSearchService.MaxHitsPerTable + 40;
        for (var index = 0; index < fieldCount; index++)
        {
            builder.Append("<f").Append(index).Append(">match</f").Append(index).Append('>');
        }

        builder.Append("</ITEM></ITEMLIST>");
        var path = WriteTable(workspace, "Items/Items.xml", builder.ToString());

        var report = await new DatasetSearchService(new StubMetadataProvider(ItemMetadata()))
            .SearchAsync(new[] { Mapped(TableName.Parse("Items.Items"), path, "Items/Items.xml") },
                "match");

        Assert.Equal(DatasetSearchService.MaxHitsPerTable, report.Hits.Count);
        Assert.Equal(fieldCount, report.MatchCount);
        Assert.Equal(40, report.SuppressedHitCount);
        Assert.Equal(1, report.MatchedTableCount);
    }

    /// <summary>
    /// A malformed file is counted as a failed table instead of aborting the scan.
    /// </summary>
    [Fact]
    public async Task Search_MalformedTable_IsCountedAsFailed()
    {
        using var workspace = new TestWorkspace();
        var brokenPath = WriteTable(workspace, "Lookup/AttachmentPoint.xml", "<ITEMLIST><ITEM>");
        var goodPath = WriteTable(
            workspace,
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>Glock</szItemName></ITEM></ITEMLIST>");

        var report = await new DatasetSearchService(new StubMetadataProvider(ItemMetadata()))
            .SearchAsync(new[]
                {
                    Mapped(TableName.Parse("Lookup.AttachmentPoint"), brokenPath, "Lookup/AttachmentPoint.xml"),
                    Mapped(TableName.Parse("Items.Items"), goodPath, "Items/Items.xml"),
                },
                "glock");

        Assert.Equal(1, report.FailedTableCount);
        Assert.Equal(1, report.SearchedTableCount);
        Assert.Equal(1, report.MatchCount);
    }

    /// <summary>
    /// An empty term is rejected instead of matching every value.
    /// </summary>
    [Fact]
    public async Task Search_EmptyTerm_Throws()
    {
        using var workspace = new TestWorkspace();

        await Assert.ThrowsAnyAsync<ArgumentException>(() =>
            new DatasetSearchService().SearchAsync(Array.Empty<TableDescriptor>(), "  "));
    }

    private static TableMetadata ItemMetadata() =>
        new(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }));

    private static TableMetadata CityMetadata() =>
        new(
            TableName.Parse("Map.Cities"),
            "CITY_INFO",
            "CITY",
            isLookup: false,
            new RecordKeySpec(new[]
            {
                new RecordKeySegment("CITYLIST/CITY", RecordKeyKind.Scalar, "uiIndex"),
                new RecordKeySegment("COUNTRY", RecordKeyKind.Singleton),
            }));

    private static string WriteTable(TestWorkspace workspace, string relativePath, string content)
    {
        var path = Path.Combine(workspace.DirectoryPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        return path;
    }

    private static TableDescriptor Mapped(
        TableName name,
        string path,
        string relativePath,
        params OverlayDescriptor[] overlays) =>
        new(name, path, relativePath, IsMapped: true, overlays);

    private static TableDescriptor Unmapped(TableName name, string path, string relativePath) =>
        new(name, path, relativePath, IsMapped: false, Array.Empty<OverlayDescriptor>());

    private sealed class StubMetadataProvider : ITableMetadataProvider
    {
        private readonly TableMetadata[] _tables;

        public StubMetadataProvider(params TableMetadata[] tables) => _tables = tables;

        public Task<OperationResult<TableMetadata>> GetTableAsync(
            TableName tableName,
            CancellationToken cancellationToken = default)
        {
            var metadata = _tables.FirstOrDefault(table => table.TableName == tableName);
            return Task.FromResult(metadata is null
                ? OperationResult<TableMetadata>.Failure(
                    ErrorCodes.MetadataNotFound,
                    $"No metadata for '{tableName.Value}'.")
                : OperationResult<TableMetadata>.Success(metadata));
        }

        public Task<OperationResult<IReadOnlyList<TableMetadata>>> GetAllTablesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<TableMetadata>>.Success(_tables));

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
