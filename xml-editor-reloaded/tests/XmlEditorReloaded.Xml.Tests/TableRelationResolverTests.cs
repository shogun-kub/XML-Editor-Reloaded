using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Relations;
using XmlEditorReloaded.Xml.Repository;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="TableRelationResolver"/>: display mapping, structured filters, language
/// overlays, remapping by name, and target caching.
/// </summary>
public class TableRelationResolverTests
{
    private const string ItemsXml =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
        + "<ITEM><uiIndex>0</uiIndex><szLongItemName>Nothing</szLongItemName><usItemClass>1</usItemClass></ITEM>"
        + "<ITEM><uiIndex>1</uiIndex><szLongItemName>Glock 17</szLongItemName><usItemClass>2</usItemClass></ITEM>"
        + "<ITEM><uiIndex>2</uiIndex><szLongItemName>Grenade</szLongItemName><usItemClass>256</usItemClass></ITEM>"
        + "</ITEMLIST>";

    /// <summary>
    /// The resolver reads value and display fields, applies the filter only to options, and supports
    /// display-based remapping.
    /// </summary>
    [Fact]
    public async Task Resolve_MapsDisplayAndAppliesFilter()
    {
        using var workspace = new TestWorkspace();
        var (resolver, descriptor) = CreateResolver(workspace, ItemsXml);
        var relation = new TableRelation(
            TableName.Parse("Items.Weapons"),
            "ubCalibre",
            TableName.Parse("Items.Items"),
            TableRelationKind.Reference,
            "uiIndex",
            "szLongItemName",
            new RelationFilter("usItemClass", RelationFilterOperator.NotEquals, "1"));

        var result = await resolver.ResolveAsync(relation);

        Assert.True(result.IsSuccess, result.ErrorText);
        var target = result.Data!;
        Assert.Equal(3, target.Entries.Count);
        Assert.Equal(new[] { "1", "2" }, target.Options.Select(entry => entry.Value));
        Assert.Equal("Glock 17", target.Find("1")!.Display);
        Assert.True(target.Contains("0"));
        Assert.Equal("1", target.FindValueByDisplay("glock 17"));
        Assert.Equal("Grenade", Assert.Single(target.Search("gren")).Display);
        Assert.Equal(descriptor.FilePath, Path.Combine(workspace.DirectoryPath, "Items", "Items.xml"));
    }

    /// <summary>
    /// A bitmask filter selects every target whose class shares a bit with the configured mask.
    /// </summary>
    [Fact]
    public async Task Resolve_HasFlagFilter_SelectsExplosiveItems()
    {
        using var workspace = new TestWorkspace();
        var (resolver, _) = CreateResolver(workspace, ItemsXml);
        var relation = new TableRelation(
            TableName.Parse("Items.AmmoTypes"),
            "highExplosive",
            TableName.Parse("Items.Items"),
            TableRelationKind.Reference,
            "uiIndex",
            "szLongItemName",
            new RelationFilter("usItemClass", RelationFilterOperator.HasFlag, "768"));

        var result = await resolver.ResolveAsync(relation);

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal("2", Assert.Single(result.Data!.Options).Value);
    }

    /// <summary>
    /// A missing target table fails the relation without throwing, so the source table can still open.
    /// </summary>
    [Fact]
    public async Task Resolve_MissingTarget_ReturnsFailure()
    {
        using var workspace = new TestWorkspace();
        var (resolver, _) = CreateResolver(workspace, ItemsXml);
        var relation = new TableRelation(
            TableName.Parse("Items.Weapons"),
            "ubCalibre",
            TableName.Parse("Items.Missing"),
            TableRelationKind.Reference,
            "uiIndex",
            "szLongItemName");

        var result = await resolver.ResolveAsync(relation);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.IoFileNotFound, result.ErrorCode);
    }

    /// <summary>
    /// Language overlays replace display text while every base record stays resolvable.
    /// </summary>
    [Fact]
    public async Task Resolve_AppliesLanguageOverlayToDisplay()
    {
        using var workspace = new TestWorkspace();
        var (resolver, descriptor) = CreateResolver(workspace, ItemsXml, withRussianOverlay: true);
        Assert.NotNull(descriptor.Overlays.Single());
        var relation = new TableRelation(
            TableName.Parse("Items.Weapons"),
            "ubCalibre",
            TableName.Parse("Items.Items"),
            TableRelationKind.Reference,
            "uiIndex",
            "szLongItemName");

        var translated = await resolver.ResolveAsync(relation, "ru");
        var baseOnly = await resolver.ResolveAsync(relation);

        Assert.True(translated.IsSuccess, translated.ErrorText);
        Assert.Equal("Глок 17", translated.Data!.Find("1")!.Display);
        Assert.Equal("Nothing", translated.Data.Find("0")!.Display);
        Assert.Equal("Glock 17", baseOnly.Data!.Find("1")!.Display);
    }

    /// <summary>
    /// Changing the target file invalidates cached rows; an explicit invalidation has the same effect.
    /// </summary>
    [Fact]
    public async Task Resolve_RefreshesChangedTargetAndInvalidates()
    {
        using var workspace = new TestWorkspace();
        var (resolver, _) = CreateResolver(workspace, ItemsXml);
        var relation = new TableRelation(
            TableName.Parse("Items.Weapons"),
            "ubCalibre",
            TableName.Parse("Items.Items"),
            TableRelationKind.Reference,
            "uiIndex",
            "szLongItemName");

        var first = await resolver.ResolveAsync(relation);
        Assert.Equal(3, first.Data!.Entries.Count);

        await File.WriteAllTextAsync(
            Path.Combine(workspace.DirectoryPath, "Items", "Items.xml"),
            ItemsXml.Replace(
                "</ITEMLIST>",
                "<ITEM><uiIndex>3</uiIndex><szLongItemName>Knife</szLongItemName><usItemClass>4</usItemClass></ITEM></ITEMLIST>",
                StringComparison.Ordinal));

        var refreshed = await resolver.ResolveAsync(relation);
        Assert.Equal(4, refreshed.Data!.Entries.Count);
        Assert.Equal("Knife", refreshed.Data.Find("3")!.Display);

        resolver.Invalidate(TableName.Parse("Items.Items"));
        var afterInvalidate = await resolver.ResolveAsync(relation);
        Assert.Equal(4, afterInvalidate.Data!.Entries.Count);
    }

    /// <summary>
    /// Unmapped targets still resolve through document discovery.
    /// </summary>
    [Fact]
    public async Task Resolve_UnmappedTarget_UsesDocumentDiscovery()
    {
        using var workspace = new TestWorkspace();
        var (_, _) = CreateResolver(workspace, ItemsXml);
        var path = Path.Combine(workspace.DirectoryPath, "Items", "Items.xml");
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: false,
            Array.Empty<OverlayDescriptor>());
        var resolver = new TableRelationResolver(
            new TestCatalog(new[] { descriptor }),
            new TableRepository(),
            workspace.DirectoryPath);
        var relation = new TableRelation(
            TableName.Parse("Items.Weapons"),
            "ubCalibre",
            TableName.Parse("Items.Items"),
            TableRelationKind.Reference,
            "uiIndex",
            "szLongItemName");

        var result = await resolver.ResolveAsync(relation);

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(3, result.Data!.Entries.Count);
    }

    private static (TableRelationResolver Resolver, TableDescriptor Descriptor) CreateResolver(
        TestWorkspace workspace,
        string itemsXml,
        bool withRussianOverlay = false)
    {
        var itemsDirectory = Path.Combine(workspace.DirectoryPath, "Items");
        Directory.CreateDirectory(itemsDirectory);
        var basePath = Path.Combine(itemsDirectory, "Items.xml");
        File.WriteAllText(basePath, itemsXml);

        var language = new LanguageDescriptor("ru", "Russian", "Russian", 1);
        var overlays = Array.Empty<OverlayDescriptor>();
        if (withRussianOverlay)
        {
            var overlayPath = Path.Combine(itemsDirectory, "Russian.Items.xml");
            File.WriteAllText(
                overlayPath,
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
                + "<ITEM><uiIndex>1</uiIndex><szLongItemName>Глок 17</szLongItemName></ITEM>"
                + "</ITEMLIST>");
            overlays = new[] { new OverlayDescriptor(language, basePath, overlayPath) };
        }

        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            basePath,
            "Items/Items.xml",
            IsMapped: true,
            overlays);
        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            columns: new[]
            {
                new ColumnMetadata("uiIndex", ColumnDataType.Integer),
                new ColumnMetadata("szLongItemName", ColumnDataType.String),
                new ColumnMetadata("usItemClass", ColumnDataType.Integer),
            },
            translatableFieldPaths: new[] { "szLongItemName" });
        var provider = new TestMetadataProvider(metadata);
        var resolver = new TableRelationResolver(
            new TestCatalog(new[] { descriptor }),
            new TableRepository(provider),
            workspace.DirectoryPath,
            provider);
        return (resolver, descriptor);
    }

    private sealed class TestCatalog : ITableCatalog
    {
        private readonly IReadOnlyList<TableDescriptor> _tables;

        public TestCatalog(IReadOnlyList<TableDescriptor> tables) => _tables = tables;

        public Task<OperationResult<IReadOnlyList<TableDescriptor>>> GetTablesAsync(
            string tableDataDirectory,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<TableDescriptor>>.Success(_tables));
    }

    private sealed class TestMetadataProvider : ITableMetadataProvider
    {
        private readonly Dictionary<TableName, TableMetadata> _tables;

        public TestMetadataProvider(params TableMetadata[] tables) =>
            _tables = tables.ToDictionary(static table => table.TableName);

        public Task<OperationResult<TableMetadata>> GetTableAsync(
            TableName tableName,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_tables.TryGetValue(tableName, out var table)
                ? OperationResult<TableMetadata>.Success(table)
                : OperationResult<TableMetadata>.Failure(
                    ErrorCodes.MetadataNotFound,
                    $"Table '{tableName}' is not present in the metadata store."));

        public Task<OperationResult<IReadOnlyList<TableMetadata>>> GetAllTablesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<TableMetadata>>.Success(_tables.Values.ToArray()));

        public Task<OperationResult<IReadOnlyList<LanguageDescriptor>>> GetLanguagesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<LanguageDescriptor>>.Success(
                Array.Empty<LanguageDescriptor>()));

        public Task<OperationResult<IReadOnlyList<TableRelation>>> GetRelationsAsync(
            TableName sourceTable,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<TableRelation>>.Success(
                Array.Empty<TableRelation>()));
    }
}
