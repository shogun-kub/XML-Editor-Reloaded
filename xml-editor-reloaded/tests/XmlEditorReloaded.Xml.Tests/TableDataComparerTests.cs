using System.Text;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Comparison;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="TableDataComparer"/> and <see cref="TableComparisonReportFormatter"/>.
/// </summary>
public class TableDataComparerTests
{
    /// <summary>
    /// Comparing two versions reports record-count, field-set, and inferred-type differences and
    /// leaves both directories untouched.
    /// </summary>
    [Fact]
    public async Task Compare_ReportsFieldAndTypeDifferences_WithoutWriting()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        Write(left, "Items/Items.xml",
            "<ITEMLIST>"
            + "<ITEM><uiIndex>0</uiIndex><szItemName>Nothing</szItemName><ubWeight>2</ubWeight></ITEM>"
            + "<ITEM><uiIndex>1</uiIndex><szItemName>Glock</szItemName></ITEM>"
            + "</ITEMLIST>");
        Write(right, "Items/Items.xml",
            "<ITEMLIST>"
            + "<ITEM><uiIndex>0</uiIndex><szItemName>Nothing</szItemName><ubWeight>1.5</ubWeight><NewField>x</NewField></ITEM>"
            + "<ITEM><uiIndex>2</uiIndex><szItemName>Glock</szItemName><NewField>y</NewField></ITEM>"
            + "<ITEM><uiIndex>3</uiIndex></ITEM>"
            + "</ITEMLIST>");
        var leftBefore = Snapshot(left);
        var rightBefore = Snapshot(right);
        var comparer = new TableDataComparer(new TestCatalog(), new TestMetadataProvider());

        var result = await comparer.CompareAsync(left, right);

        Assert.True(result.IsSuccess, result.ErrorText);
        var report = result.Data!;
        Assert.Empty(report.Warnings);
        var entry = Assert.Single(report.Tables);
        Assert.Equal("Items.Items", entry.TableName.Value);
        Assert.Equal(TableComparisonStatus.Both, entry.Status);
        Assert.True(entry.HasDifferences);
        Assert.Equal(2, entry.Left!.Snapshot!.RecordCount);
        Assert.Equal(3, entry.Right!.Snapshot!.RecordCount);
        Assert.Equal(3, entry.Left.Snapshot.FieldCount);
        Assert.Equal(4, entry.Right.Snapshot.FieldCount);
        Assert.Equal(2, entry.Left.Snapshot.DistinctRecordShapes);
        Assert.Equal(3, entry.Right.Snapshot.DistinctRecordShapes);

        Assert.Equal(FieldComparisonStatus.Same, FindField(entry, "uiIndex").Status);
        Assert.Equal(ColumnDataType.Integer, FindField(entry, "uiIndex").LeftType);
        Assert.Equal(FieldComparisonStatus.TypeChanged, FindField(entry, "ubWeight").Status);
        Assert.Equal(ColumnDataType.Integer, FindField(entry, "ubWeight").LeftType);
        Assert.Equal(ColumnDataType.Decimal, FindField(entry, "ubWeight").RightType);
        Assert.Equal(FieldComparisonStatus.RightOnly, FindField(entry, "NewField").Status);

        Assert.Equal(1, entry.AddedFieldCount);
        Assert.Equal(0, entry.RemovedFieldCount);
        Assert.Equal(1, entry.TypeChangedFieldCount);
        Assert.Equal(1, report.Summary.CommonTableCount);
        Assert.Equal(1, report.Summary.ChangedTableCount);
        Assert.Equal(1, report.Summary.AddedFieldCount);
        Assert.Equal(1, report.Summary.TypeChangedFieldCount);
        Assert.Equal(2, report.Summary.LeftRecordTotal);
        Assert.Equal(3, report.Summary.RightRecordTotal);

        Assert.Equal(leftBefore, Snapshot(left));
        Assert.Equal(rightBefore, Snapshot(right));
    }

    /// <summary>
    /// Tables that exist on one side only are reported with their side and counted in the summary.
    /// </summary>
    [Fact]
    public async Task Compare_ReportsMissingAndExtraTables()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        Write(left, "Items/Items.xml", "<ITEMLIST><ITEM><uiIndex>1</uiIndex></ITEM></ITEMLIST>");
        Write(left, "Items/Old.xml", "<OLDLIST><OLD><id>1</id></OLD></OLDLIST>");
        Write(right, "Items/Items.xml", "<ITEMLIST><ITEM><uiIndex>1</uiIndex></ITEM></ITEMLIST>");
        Write(right, "Items/New.xml", "<NEWLIST><NEW><id>1</id></NEW></NEWLIST>");

        var result = await new TableDataComparer(new TestCatalog()).CompareAsync(left, right);

        Assert.True(result.IsSuccess, result.ErrorText);
        var report = result.Data!;
        Assert.Equal(3, report.Tables.Count);
        var removed = Assert.Single(report.Tables, entry => entry.TableName.Value == "Items.Old");
        Assert.Equal(TableComparisonStatus.LeftOnly, removed.Status);
        Assert.NotNull(removed.Left?.Snapshot);
        Assert.Null(removed.Right);
        Assert.True(removed.HasDifferences);
        var added = Assert.Single(report.Tables, entry => entry.TableName.Value == "Items.New");
        Assert.Equal(TableComparisonStatus.RightOnly, added.Status);
        Assert.Null(added.Left);
        Assert.NotNull(added.Right?.Snapshot);
        Assert.Equal(1, report.Summary.LeftOnlyTableCount);
        Assert.Equal(1, report.Summary.RightOnlyTableCount);
        Assert.Equal(1, report.Summary.IdenticalTableCount);
    }

    /// <summary>
    /// A malformed file on one side is reported without failing the comparison, and the other side
    /// snapshot is still analyzed.
    /// </summary>
    [Fact]
    public async Task Compare_ReportsMalformedSide()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        Write(left, "Lookup/Points.xml", "<POINTS><Point><id>1</id><name>A</name></Point></POINTS>");
        Write(right, "Lookup/Points.xml", "<POINTS><Point><id>1</id><name>A</name></Point></POINTS></POINTS>");

        var result = await new TableDataComparer(new TestCatalog()).CompareAsync(left, right);

        Assert.True(result.IsSuccess, result.ErrorText);
        var entry = Assert.Single(result.Data!.Tables);
        Assert.Equal(TableComparisonStatus.Both, entry.Status);
        Assert.True(entry.HasMalformedSide);
        Assert.True(entry.HasDifferences);
        Assert.NotNull(entry.Left?.Snapshot);
        Assert.Equal(TableSideStatus.Malformed, entry.Right!.Status);
        Assert.Null(entry.Right.Snapshot);
        Assert.Contains(entry.Right.Diagnostics, diagnostic => diagnostic.Code == ErrorCodes.XmlParseError);
        Assert.Equal(1, result.Data.Summary.MalformedTableCount);
    }

    /// <summary>
    /// An unreadable file on one side is reported as a failed side, not as an operation failure.
    /// </summary>
    [Fact]
    public async Task Compare_ReportsUnreadableSide()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        Write(left, "Items/Items.xml", "<ITEMLIST><ITEM><uiIndex>1</uiIndex></ITEM></ITEMLIST>");
        Write(right, "Items/Items.xml", "<ITEMLIST><ITEM><uiIndex>1</uiIndex></ITEM></ITEMLIST>");
        var catalog = new TestCatalog(
            missingRelativePaths: new[] { "Items/Items.xml" },
            missingDirectoryName: "left");

        var result = await new TableDataComparer(catalog).CompareAsync(left, right);

        Assert.True(result.IsSuccess, result.ErrorText);
        var entry = Assert.Single(result.Data!.Tables);
        Assert.True(entry.HasLoadFailure);
        Assert.Equal(TableSideStatus.Failed, entry.Left!.Status);
        Assert.Equal(TableSideStatus.Loaded, entry.Right!.Status);
        Assert.Equal(1, result.Data.Summary.FailedTableCount);
    }

    /// <summary>
    /// A field that becomes a repeated element on one side is reported as a cardinality change.
    /// </summary>
    [Fact]
    public async Task Compare_ReportsCardinalityChange()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        Write(left, "Items/Tags.xml", "<TAGS><TAG><uiIndex>1</uiIndex><Tag>a</Tag></TAG></TAGS>");
        Write(right, "Items/Tags.xml", "<TAGS><TAG><uiIndex>1</uiIndex><Tag>a</Tag><Tag>b</Tag></TAG></TAGS>");

        var result = await new TableDataComparer(new TestCatalog()).CompareAsync(left, right);

        Assert.True(result.IsSuccess, result.ErrorText);
        var entry = Assert.Single(result.Data!.Tables);
        var field = FindField(entry, "Tag");
        Assert.Equal(FieldComparisonStatus.CardinalityChanged, field.Status);
        Assert.Equal(1, field.LeftMaxOccurrences);
        Assert.Equal(2, field.RightMaxOccurrences);
        Assert.Equal(1, result.Data.Summary.CardinalityChangedFieldCount);
    }

    /// <summary>
    /// Nested-key tables use descendant records such as <c>CITYLIST/CITY</c> instead of direct
    /// root children.
    /// </summary>
    [Fact]
    public async Task Compare_UsesNestedRecordElementFromMetadata()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        Write(left, "Map/Cities.xml",
            "<CITY_INFO><COUNTRY><countryName>A</countryName></COUNTRY>"
            + "<CITYLIST><CITY><uiIndex>1</uiIndex><townName>Omerta</townName></CITY></CITYLIST></CITY_INFO>");
        Write(right, "Map/Cities.xml",
            "<CITY_INFO><COUNTRY><countryName>A</countryName></COUNTRY>"
            + "<CITYLIST><CITY><uiIndex>1</uiIndex><townName>Omerta</townName></CITY>"
            + "<CITY><uiIndex>2</uiIndex><townName>Drassen</townName></CITY></CITYLIST></CITY_INFO>");
        var metadata = new TestMetadataProvider(CreateCitiesMetadata());

        var result = await new TableDataComparer(new TestCatalog(), metadata).CompareAsync(left, right);

        Assert.True(result.IsSuccess, result.ErrorText);
        var entry = Assert.Single(result.Data!.Tables);
        Assert.Equal("CITY", entry.Left!.Snapshot!.RecordElement);
        Assert.Equal(1, entry.Left.Snapshot.RecordCount);
        Assert.Equal(2, entry.Right!.Snapshot!.RecordCount);
        Assert.NotNull(entry.Fields.FirstOrDefault(field => field.Name == "townName"));
    }

    /// <summary>
    /// Lookup columns declared in metadata are reported with their availability on both sides.
    /// </summary>
    [Fact]
    public async Task Compare_ReportsLookupReferencesAvailability()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        var items = "<ITEMLIST><ITEM><uiIndex>1</uiIndex><usItemClass>2</usItemClass></ITEM></ITEMLIST>";
        Write(left, "Items/Items.xml", items);
        Write(right, "Items/Items.xml", items);
        Write(left, "Lookup/ItemClass.xml", "<ITEMCLASS><ItemClass><id>2</id><name>Gun</name></ItemClass></ITEMCLASS>");
        var metadata = new TestMetadataProvider(CreateItemsMetadata());

        var result = await new TableDataComparer(new TestCatalog(), metadata).CompareAsync(left, right);

        Assert.True(result.IsSuccess, result.ErrorText);
        var reference = Assert.Single(result.Data!.LookupReferences);
        Assert.Equal("Items.Items", reference.Table.Value);
        Assert.Equal("usItemClass", reference.FieldName);
        Assert.Equal("Lookup.ItemClass", reference.LookupTable);
        Assert.Equal(LookupAvailability.Available, reference.LeftAvailability);
        Assert.Equal(LookupAvailability.Missing, reference.RightAvailability);
    }

    /// <summary>
    /// A malformed lookup file is reported as unreadable rather than missing.
    /// </summary>
    [Fact]
    public async Task Compare_ReportsUnreadableLookupReference()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        var items = "<ITEMLIST><ITEM><uiIndex>1</uiIndex><usItemClass>2</usItemClass></ITEM></ITEMLIST>";
        var lookup = "<ITEMCLASS><ItemClass><id>2</id><name>Gun</name></ItemClass></ITEMCLASS>";
        Write(left, "Items/Items.xml", items);
        Write(right, "Items/Items.xml", items);
        Write(left, "Lookup/ItemClass.xml", lookup);
        Write(right, "Lookup/ItemClass.xml", lookup + "</ITEMCLASS>");
        var metadata = new TestMetadataProvider(CreateItemsMetadata());

        var result = await new TableDataComparer(new TestCatalog(), metadata).CompareAsync(left, right);

        Assert.True(result.IsSuccess, result.ErrorText);
        var reference = Assert.Single(result.Data!.LookupReferences);
        Assert.Equal(LookupAvailability.Available, reference.LeftAvailability);
        Assert.Equal(LookupAvailability.Unreadable, reference.RightAvailability);
    }

    /// <summary>
    /// Without a metadata provider the comparison still works document-driven and records a warning.
    /// </summary>
    [Fact]
    public async Task Compare_WithoutMetadata_ReportsDocumentDrivenWarning()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        var content = "<LIST><ROW><id>1</id></ROW></LIST>";
        Write(left, "Custom/List.xml", content);
        Write(right, "Custom/List.xml", content);

        var result = await new TableDataComparer(new TestCatalog()).CompareAsync(left, right);

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Single(result.Data!.Warnings);
        var entry = Assert.Single(result.Data.Tables);
        Assert.False(entry.HasDifferences);
        Assert.Equal(1, entry.Left!.Snapshot!.RecordCount);
    }

    /// <summary>
    /// A missing directory fails the comparison through the catalog.
    /// </summary>
    [Fact]
    public async Task Compare_MissingDirectory_Fails()
    {
        using var workspace = new TestWorkspace();
        var existing = CreateDirectory(workspace, "left");
        var missing = Path.Combine(workspace.DirectoryPath, "missing");

        var result = await new TableDataComparer(new TestCatalog()).CompareAsync(existing, missing);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.IoFileNotFound, result.ErrorCode);
    }

    /// <summary>
    /// A pre-cancelled token stops the comparison before any file is loaded.
    /// </summary>
    [Fact]
    public async Task Compare_PreCancelledToken_Throws()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        using var source = new CancellationTokenSource();
        source.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            new TableDataComparer(new TestCatalog()).CompareAsync(left, right, source.Token));
    }

    /// <summary>
    /// Markdown export contains the summary, table list, and changed-field details.
    /// </summary>
    [Fact]
    public async Task FormatMarkdown_ContainsSummaryAndDetails()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        Write(left, "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubWeight>2</ubWeight></ITEM></ITEMLIST>");
        Write(right, "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubWeight>1.5</ubWeight></ITEM></ITEMLIST>");
        var report = (await new TableDataComparer(new TestCatalog()).CompareAsync(left, right)).Data!;

        var markdown = TableComparisonReportFormatter.FormatMarkdown(report);

        Assert.Contains("# TableData comparison report", markdown, StringComparison.Ordinal);
        Assert.Contains("## Summary", markdown, StringComparison.Ordinal);
        Assert.Contains("## Lookup references", markdown, StringComparison.Ordinal);
        Assert.Contains("## Table details", markdown, StringComparison.Ordinal);
        Assert.Contains("Both (changed)", markdown, StringComparison.Ordinal);
        Assert.Contains("| ubWeight | TypeChanged | Integer | Decimal |", markdown, StringComparison.Ordinal);
        Assert.Contains("Fields with changed type | 1", markdown, StringComparison.Ordinal);
    }

    /// <summary>
    /// CSV export uses long-format rows and quotes cells containing separators or quotes.
    /// </summary>
    [Fact]
    public async Task FormatCsv_EscapesCellsAndIncludesAllRecordTypes()
    {
        using var workspace = new TestWorkspace();
        var left = CreateDirectory(workspace, "left");
        var right = CreateDirectory(workspace, "right");
        Write(left, "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubWeight>2</ubWeight></ITEM></ITEMLIST>");
        Write(right, "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubWeight>1.5</ubWeight></ITEM></ITEMLIST>");
        var report = (await new TableDataComparer(new TestCatalog()).CompareAsync(left, right)).Data!;
        report = report with { Warnings = new[] { "line, with \"quotes\"" } };

        var csv = TableComparisonReportFormatter.FormatCsv(report);
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        Assert.StartsWith("RecordType,Table,Name,Status,Left,Right,LeftDetail,RightDetail", lines[0], StringComparison.Ordinal);
        Assert.Contains(lines, line => line.StartsWith("Table,Items.Items,", StringComparison.Ordinal));
        Assert.Contains(lines, line => line.StartsWith("Field,Items.Items,ubWeight,TypeChanged,Integer,Decimal", StringComparison.Ordinal));
        Assert.Contains(lines, line => line.StartsWith("Warning,,,Warning,,,", StringComparison.Ordinal));
        Assert.Contains("\"line, with \"\"quotes\"\"\"", csv, StringComparison.Ordinal);
    }

    private static FieldComparison FindField(TableComparisonEntry entry, string name) =>
        Assert.Single(entry.Fields, field => field.Name == name);

    private static string CreateDirectory(TestWorkspace workspace, string name)
    {
        var path = Path.Combine(workspace.DirectoryPath, name);
        Directory.CreateDirectory(path);
        return path;
    }

    private static void Write(string root, string relativePath, string content)
    {
        var path = Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static IReadOnlyList<string> Snapshot(string directory) =>
        Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(path => $"{Path.GetRelativePath(directory, path)}|{Convert.ToHexString(File.ReadAllBytes(path))}")
            .ToList();

    private static TableMetadata CreateCitiesMetadata() =>
        new(
            new TableName("Map.Cities"),
            "CITY_INFO",
            "CITY",
            isLookup: false,
            recordKey: new RecordKeySpec(new[]
            {
                new RecordKeySegment("CITYLIST/CITY", RecordKeyKind.Scalar, "uiIndex"),
                new RecordKeySegment("COUNTRY", RecordKeyKind.Singleton),
            }));

    private static TableMetadata CreateItemsMetadata() =>
        new(
            new TableName("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            recordKey: new RecordKeySpec(new[]
            {
                new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex"),
            }),
            columnHints: new[]
            {
                new ColumnHint("usItemClass") { LookupTable = "Lookup.ItemClass" },
            });

    private sealed class TestCatalog : ITableCatalog
    {
        private readonly HashSet<string> _missingRelativePaths;
        private readonly string? _missingDirectoryName;

        public TestCatalog(
            IEnumerable<string>? missingRelativePaths = null,
            string? missingDirectoryName = null)
        {
            _missingRelativePaths = new HashSet<string>(
                missingRelativePaths ?? Array.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);
            _missingDirectoryName = missingDirectoryName;
        }

        public Task<OperationResult<IReadOnlyList<TableDescriptor>>> GetTablesAsync(
            string tableDataDirectory,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!Directory.Exists(tableDataDirectory))
            {
                return Task.FromResult(OperationResult<IReadOnlyList<TableDescriptor>>.Failure(
                    ErrorCodes.IoFileNotFound,
                    $"The TableData directory '{tableDataDirectory}' does not exist."));
            }

            var descriptors = new List<TableDescriptor>();
            foreach (var file in Directory.EnumerateFiles(tableDataDirectory, "*.xml", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(tableDataDirectory, file).Replace('\\', '/');
                if (!TableName.TryFromRelativePath(relativePath, out var tableName))
                {
                    continue;
                }

                descriptors.Add(new TableDescriptor(
                    tableName,
                    file,
                    relativePath,
                    IsMapped: false,
                    Array.Empty<OverlayDescriptor>()));
            }

            descriptors.Sort(static (left, right) => left.TableName.CompareTo(right.TableName));
            IReadOnlyList<TableDescriptor> result = descriptors;
            var appliesToThisDirectory = _missingDirectoryName is null
                || string.Equals(
                    Path.GetFileName(Path.TrimEndingDirectorySeparator(tableDataDirectory)),
                    _missingDirectoryName,
                    StringComparison.OrdinalIgnoreCase);
            if (_missingRelativePaths.Count > 0 && appliesToThisDirectory)
            {
                result = descriptors
                    .Select(descriptor => _missingRelativePaths.Contains(descriptor.RelativePath)
                        ? descriptor with { FilePath = descriptor.FilePath + ".missing" }
                        : descriptor)
                    .ToArray();
            }

            return Task.FromResult(OperationResult<IReadOnlyList<TableDescriptor>>.Success(result));
        }
    }

    private sealed class TestMetadataProvider : ITableMetadataProvider
    {
        private readonly Dictionary<TableName, TableMetadata> _tables;

        public TestMetadataProvider(params TableMetadata[] tables)
        {
            _tables = tables.ToDictionary(static table => table.TableName);
        }

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
    }
}
