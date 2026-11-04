using System.Text;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Validation;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="DatasetValidator"/> and <see cref="DatasetValidationReportFormatter"/>.
/// </summary>
public class DatasetValidationTests
{
    /// <summary>
    /// Every table is validated and reported with exact counts, its metadata mode, and its keyed issues.
    /// A mapped table with a broken value and an unmapped table both appear in one report.
    /// </summary>
    [Fact]
    public async Task Validate_ReportsEveryTableWithCountsAndIssues()
    {
        using var workspace = new TestWorkspace();
        var mappedPath = WriteTable(
            workspace,
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubWeight>abc</ubWeight></ITEM></ITEMLIST>");
        var unmappedPath = WriteTable(
            workspace,
            "Custom/Notes.xml",
            "<NOTES><NOTE><id>1</id><text>Hello</text></NOTE></NOTES>");
        var provider = new StubMetadataProvider(new[]
        {
            ItemMetadata(new ColumnMetadata("ubWeight", ColumnDataType.Integer)),
        });
        var tables = new[]
        {
            Mapped(TableName.Parse("Items.Items"), mappedPath, "Items/Items.xml"),
            Unmapped(TableName.Parse("Custom.Notes"), unmappedPath, "Custom/Notes.xml"),
        };

        var report = await new DatasetValidator(provider).ValidateAsync(
            workspace.DirectoryPath,
            tables);

        Assert.Equal(2, report.Tables.Count);
        Assert.Equal(2, report.ValidatedTableCount);
        Assert.Equal(1, report.UnmappedTableCount);
        Assert.Equal(1, report.WarningCount);
        Assert.Equal(1, report.InfoCount);
        Assert.Equal(2, report.IssueCount);
        Assert.Equal(workspace.DirectoryPath, report.TableDataDirectory);

        var items = Assert.Single(report.Tables, table => table.TableName.Value == "Items.Items");
        Assert.True(items.IsMapped);
        Assert.Equal(TableValidationState.Validated, items.State);
        Assert.Equal(1, items.RecordCount);
        Assert.Equal(1, items.WarningCount);
        Assert.Equal("1", Assert.Single(items.Issues).RecordKey);
        Assert.Equal("ubWeight", items.Issues[0].FieldPath);

        var notes = Assert.Single(report.Tables, table => table.TableName.Value == "Custom.Notes");
        Assert.False(notes.IsMapped);
        Assert.Equal(ValidationCodes.Unmapped, Assert.Single(notes.Issues).Code);
    }

    /// <summary>
    /// A malformed file becomes an error issue and does not abort the scan, so the next table is still
    /// validated.
    /// </summary>
    [Fact]
    public async Task Validate_MalformedFile_IsAnErrorAndScanContinues()
    {
        using var workspace = new TestWorkspace();
        var brokenPath = WriteTable(workspace, "Lookup/AttachmentPoint.xml", "<ITEMLIST><ITEM>");
        var goodPath = WriteTable(
            workspace,
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex></ITEM></ITEMLIST>");
        var tables = new[]
        {
            Mapped(TableName.Parse("Lookup.AttachmentPoint"), brokenPath, "Lookup/AttachmentPoint.xml"),
            Mapped(TableName.Parse("Items.Items"), goodPath, "Items/Items.xml"),
        };

        var report = await new DatasetValidator().ValidateAsync(workspace.DirectoryPath, tables);

        var broken = Assert.Single(report.Tables, table => table.State == TableValidationState.Malformed);
        Assert.Equal(1, broken.ErrorCount);
        Assert.Equal(1, report.MalformedTableCount);
        var issue = Assert.Single(broken.Issues);
        Assert.Equal(ValidationCodes.MalformedFile, issue.Code);
        Assert.Equal(ValidationSeverity.Error, issue.Severity);
        Assert.Contains("xml.parse_error", issue.Message, StringComparison.Ordinal);

        var good = Assert.Single(report.Tables, table => table.TableName.Value == "Items.Items");
        Assert.Equal(TableValidationState.Validated, good.State);
        Assert.Equal(1, good.RecordCount);
    }

    /// <summary>
    /// A file that cannot be read is reported as unreadable with the I/O message.
    /// </summary>
    [Fact]
    public async Task Validate_MissingFile_IsUnreadable()
    {
        using var workspace = new TestWorkspace();
        var missingPath = Path.Combine(workspace.DirectoryPath, "Items", "Gone.xml");

        var report = await new DatasetValidator().ValidateAsync(
            workspace.DirectoryPath,
            new[] { Unmapped(TableName.Parse("Items.Gone"), missingPath, "Items/Gone.xml") });

        var table = Assert.Single(report.Tables);
        Assert.Equal(TableValidationState.Unreadable, table.State);
        Assert.Equal(1, report.UnreadableTableCount);
        Assert.Equal(1, table.ErrorCount);
        var issue = Assert.Single(table.Issues);
        Assert.Equal(ValidationCodes.UnreadableFile, issue.Code);
        Assert.Contains("does not exist", issue.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// A malformed language overlay is reported against its table instead of being ignored.
    /// </summary>
    [Fact]
    public async Task Validate_MalformedOverlay_IsReported()
    {
        using var workspace = new TestWorkspace();
        var basePath = WriteTable(
            workspace,
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>Nothing</szItemName></ITEM></ITEMLIST>");
        var overlayPath = WriteTable(workspace, "Items/Russian.Items.xml", "<ITEMLIST><ITEM>");
        var descriptor = Mapped(
            TableName.Parse("Items.Items"),
            basePath,
            "Items/Items.xml",
            new OverlayDescriptor(
                new LanguageDescriptor("ru", "Russian", "Russian", 1),
                basePath,
                overlayPath));

        var report = await new DatasetValidator(new StubMetadataProvider(new[] { ItemMetadata() }))
            .ValidateAsync(workspace.DirectoryPath, new[] { descriptor });

        var table = Assert.Single(report.Tables);
        var issue = Assert.Single(table.Issues, item => item.Code == ValidationCodes.MalformedOverlay);
        Assert.Equal(ValidationSeverity.Error, issue.Severity);
        Assert.Equal("Russian.Items.xml", issue.FieldPath);
        Assert.Contains("xml.parse_error", issue.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Details are capped per table while the severity counts stay exact.
    /// </summary>
    [Fact]
    public async Task Validate_CapsIssueDetailsButKeepsExactCounts()
    {
        using var workspace = new TestWorkspace();
        var builder = new StringBuilder("<ITEMLIST><ITEM><uiIndex>1</uiIndex>");
        for (var index = 0; index < DatasetValidator.MaxIssuesPerTable + 50; index++)
        {
            builder.Append("<unknown").Append(index).Append(">1</unknown").Append(index).Append('>');
        }

        builder.Append("</ITEM></ITEMLIST>");
        var path = WriteTable(workspace, "Items/Items.xml", builder.ToString());
        var provider = new StubMetadataProvider(new[] { ItemMetadata() });

        var report = await new DatasetValidator(provider).ValidateAsync(
            workspace.DirectoryPath,
            new[] { Mapped(TableName.Parse("Items.Items"), path, "Items/Items.xml") });

        var table = Assert.Single(report.Tables);
        Assert.Equal(DatasetValidator.MaxIssuesPerTable, table.Issues.Count);
        Assert.Equal(50, table.SuppressedIssueCount);
        Assert.Equal(DatasetValidator.MaxIssuesPerTable + 50, table.IssueCount);
        Assert.Equal(table.InfoCount, table.Issues.Count + table.SuppressedIssueCount);
        Assert.Equal(50, report.SuppressedIssueCount);
    }

    /// <summary>
    /// Progress is reported once per table, including the total, so a caller can show a counter.
    /// </summary>
    [Fact]
    public async Task Validate_ReportsProgressPerTable()
    {
        using var workspace = new TestWorkspace();
        var first = WriteTable(workspace, "A/One.xml", "<NOTES><NOTE><id>1</id></NOTE></NOTES>");
        var second = WriteTable(workspace, "B/Two.xml", "<NOTES><NOTE><id>2</id></NOTE></NOTES>");
        var progress = new RecordingProgress();

        await new DatasetValidator().ValidateAsync(
            workspace.DirectoryPath,
            new[]
            {
                Unmapped(TableName.Parse("A.One"), first, "A/One.xml"),
                Unmapped(TableName.Parse("B.Two"), second, "B/Two.xml"),
            },
            progress: progress);

        Assert.Equal(2, progress.Reports.Count);
        Assert.Equal(1, progress.Reports[0].Completed);
        Assert.Equal(2, progress.Reports[0].Total);
        Assert.Equal("A.One", progress.Reports[0].TableName?.Value);
        Assert.Equal(2, progress.Reports[1].Completed);
    }

    /// <summary>
    /// The Markdown export contains the summary, the per-table counts, and every retained issue.
    /// </summary>
    [Fact]
    public async Task Formatter_Markdown_ContainsSummaryTablesAndIssues()
    {
        using var workspace = new TestWorkspace();
        var path = WriteTable(
            workspace,
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubWeight>abc</ubWeight></ITEM></ITEMLIST>");
        var provider = new StubMetadataProvider(new[]
        {
            ItemMetadata(new ColumnMetadata("ubWeight", ColumnDataType.Integer)),
        });
        var report = await new DatasetValidator(provider).ValidateAsync(
            workspace.DirectoryPath,
            new[] { Mapped(TableName.Parse("Items.Items"), path, "Items/Items.xml") });

        var markdown = DatasetValidationReportFormatter.FormatMarkdown(report);

        Assert.StartsWith("# TableData validation report", markdown, StringComparison.Ordinal);
        Assert.Contains(workspace.DirectoryPath, markdown, StringComparison.Ordinal);
        Assert.Contains("## Summary", markdown, StringComparison.Ordinal);
        Assert.Contains("| Warnings | 1 |", markdown, StringComparison.Ordinal);
        Assert.Contains("| Items.Items | Validated | mapped | 1 |", markdown, StringComparison.Ordinal);
        Assert.Contains("| Items.Items | Warning | 1 | ubWeight |", markdown, StringComparison.Ordinal);
    }

    /// <summary>
    /// The CSV export writes a table row followed by one row per retained issue.
    /// </summary>
    [Fact]
    public async Task Formatter_Csv_ContainsTableAndIssueRows()
    {
        using var workspace = new TestWorkspace();
        var path = WriteTable(
            workspace,
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubWeight>abc</ubWeight></ITEM></ITEMLIST>");
        var provider = new StubMetadataProvider(new[]
        {
            ItemMetadata(new ColumnMetadata("ubWeight", ColumnDataType.Integer)),
        });
        var report = await new DatasetValidator(provider).ValidateAsync(
            workspace.DirectoryPath,
            new[] { Mapped(TableName.Parse("Items.Items"), path, "Items/Items.xml") });

        var csv = DatasetValidationReportFormatter.FormatCsv(report);
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.StartsWith("Table,State,Mapped,Records,Errors,Warnings,Info", lines[0], StringComparison.Ordinal);
        Assert.StartsWith("Items.Items,Validated,true,1,0,1,", lines[1], StringComparison.Ordinal);
        Assert.Contains("Warning,1,ubWeight,validation.invalid_type", lines[2], StringComparison.Ordinal);
    }

    private static TableMetadata ItemMetadata(params ColumnMetadata[] columns) =>
        new(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            columns: new[] { new ColumnMetadata("uiIndex", ColumnDataType.Integer) }
                .Concat(columns)
                .ToArray());

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

        public StubMetadataProvider(TableMetadata[] tables) => _tables = tables;

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
            Task.FromResult(OperationResult<IReadOnlyList<TableRelation>>.Success(
                Array.Empty<TableRelation>()));

        public Task<OperationResult<IReadOnlyList<LookupEntry>>> GetLookupValuesAsync(
            TableName lookupName,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<LookupEntry>>.Success(Array.Empty<LookupEntry>()));
    }

    private sealed class RecordingProgress : IProgress<DatasetScanProgress>
    {
        public List<DatasetScanProgress> Reports { get; } = new();

        public void Report(DatasetScanProgress value) => Reports.Add(value);
    }
}
