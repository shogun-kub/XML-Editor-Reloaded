using System.Diagnostics;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Filtering;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Filtering;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;
using XmlEditorReloaded.Xml.Workspace;
using Xunit.Abstractions;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Environment-gated performance gates over a real <c>TableData</c> corpus.
/// Set <c>XER_CORPUS_DIR</c> to a v4 or v5 TableData directory to enable them.
/// </summary>
[Trait("Category", "Performance")]
public class PerformanceTests
{
    private const int GateMilliseconds = 2000;

    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceTests"/> class.
    /// </summary>
    /// <param name="output">Test output used to report measured durations.</param>
    public PerformanceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// The largest item table loads within the interactive gate.
    /// </summary>
    [Fact]
    public Task Load_ItemsXml_RespondsWithinGate() => MeasureLoadAsync(
        "Items/Items.xml",
        "Items.Items",
        "ITEMLIST",
        "ITEM",
        "uiIndex");

    /// <summary>
    /// The largest many-row attachment table loads within the interactive gate.
    /// </summary>
    [Fact]
    public Task Load_AttachmentsXml_RespondsWithinGate() => MeasureLoadAsync(
        "Items/Attachments.xml",
        "Items.Attachments",
        "ATTACHMENTLIST",
        "ATTACHMENT",
        "attachmentIndex");

    /// <summary>
    /// Loading, editing one value, and saving the largest item table stay within the interactive gate.
    /// </summary>
    [Fact]
    public async Task EditAndSave_ItemsXml_RespondsWithinGate()
    {
        var corpusPath = FindCorpusFile("Items/Items.xml");
        if (corpusPath is null)
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "Items.xml");
        File.Copy(corpusPath, path, overwrite: true);
        var metadata = CreateMetadata("Items.Items", "ITEMLIST", "ITEM", "uiIndex");
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());

        var stopwatch = Stopwatch.StartNew();
        var load = await new TableFileLoader().LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var document = load.Data!.GetDocument<TableDocument>()!;
        var record = document.Records[0];
        Assert.True(record.TrySetValue("szItemName", "Nothing (edited)"));
        var save = await new TableFileSaver().SaveAsync(document);
        stopwatch.Stop();

        Assert.True(save.IsSuccess, save.ErrorText);
        var elapsed = stopwatch.ElapsedMilliseconds;
        _output.WriteLine(
            $"Items/Items.xml: load + edit + save of {document.Records.Count} records took {elapsed} ms.");
        Assert.True(
            elapsed < GateMilliseconds,
            $"Load + edit + save of Items/Items.xml took {elapsed} ms, exceeding the {GateMilliseconds} ms gate.");
    }

    /// <summary>
    /// Opening the largest table through the workspace, including schema discovery, stays within the gate.
    /// </summary>
    [Fact]
    public async Task Workspace_ItemsXml_LoadsWithinGate()
    {
        var corpusPath = FindCorpusFile("Items/Items.xml");
        if (corpusPath is null)
        {
            return;
        }

        var metadata = CreateMetadata("Items.Items", "ITEMLIST", "ITEM", "uiIndex");
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            corpusPath,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());

        var stopwatch = Stopwatch.StartNew();
        var workspace = await TableWorkspace.LoadAsync(descriptor, metadata);
        stopwatch.Stop();

        Assert.True(workspace.IsSuccess, workspace.ErrorText);
        var table = workspace.Data!;
        var elapsed = stopwatch.ElapsedMilliseconds;
        _output.WriteLine(
            $"Workspace opened {table.GetRows().Count} rows and {table.Schema.Columns.Count} columns in {elapsed} ms.");
        Assert.True(
            elapsed < GateMilliseconds,
            $"Opening Items/Items.xml took {elapsed} ms, exceeding the {GateMilliseconds} ms gate.");
    }

    /// <summary>
    /// Previewing, applying, and undoing a bulk edit over every row of the largest table stays within
    /// the interactive gate.
    /// </summary>
    [Fact]
    public async Task BulkEdit_ItemsXml_RespondsWithinGate()
    {
        var corpusPath = FindCorpusFile("Items/Items.xml");
        if (corpusPath is null)
        {
            return;
        }

        var metadata = CreateMetadata("Items.Items", "ITEMLIST", "ITEM", "uiIndex");
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            corpusPath,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await TableWorkspace.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;
        var keys = table.GetRows()
            .Where(static row => row.Key is not null)
            .Select(static row => row.Key!)
            .ToArray();
        var request = new BulkEditRequest(
            "ubWeight",
            BulkEditOperation.Set,
            "5",
            IncludeMissingFields: false,
            keys);

        var previewWatch = Stopwatch.StartNew();
        var preview = table.PreviewBulk(request);
        previewWatch.Stop();
        Assert.True(preview.IsSuccess, preview.ErrorText);
        Assert.True(preview.Data!.ChangedCount > 0);

        var applyWatch = Stopwatch.StartNew();
        var applied = table.ApplyBulk(request);
        applyWatch.Stop();
        Assert.True(applied.IsSuccess, applied.ErrorText);

        var undoWatch = Stopwatch.StartNew();
        var undone = table.Undo();
        undoWatch.Stop();
        Assert.True(undone.IsSuccess, undone.ErrorText);

        _output.WriteLine(
            $"Bulk edit over {keys.Length} rows of Items/Items.xml: preview {previewWatch.ElapsedMilliseconds} ms, "
            + $"apply {applyWatch.ElapsedMilliseconds} ms, undo {undoWatch.ElapsedMilliseconds} ms.");
        Assert.True(
            previewWatch.ElapsedMilliseconds < GateMilliseconds,
            $"Bulk preview took {previewWatch.ElapsedMilliseconds} ms, exceeding the {GateMilliseconds} ms gate.");
        Assert.True(
            applyWatch.ElapsedMilliseconds < GateMilliseconds,
            $"Bulk apply took {applyWatch.ElapsedMilliseconds} ms, exceeding the {GateMilliseconds} ms gate.");
        Assert.True(
            undoWatch.ElapsedMilliseconds < GateMilliseconds,
            $"Bulk undo took {undoWatch.ElapsedMilliseconds} ms, exceeding the {GateMilliseconds} ms gate.");
    }

    /// <summary>
    /// Bulk-editing translations over every row of a real v4 overlay stays within the interactive gate.
    /// The test is skipped when the corpus has no Russian overlay.
    /// </summary>
    [Fact]
    public async Task BulkTranslation_ItemsXml_RespondsWithinGate()
    {
        var corpusPath = FindCorpusFile("Items/Items.xml");
        if (corpusPath is null)
        {
            return;
        }

        var overlayPath = Path.Combine(
            Path.GetDirectoryName(corpusPath)!,
            "Russian.Items.xml");
        if (!File.Exists(overlayPath))
        {
            _output.WriteLine("The corpus has no Russian.Items.xml; the bulk translation gate was skipped.");
            return;
        }

        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            translatableFieldPaths: new[] { "szItemName" });
        var language = new LanguageDescriptor("ru", "Russian", "Russian", 1);
        var descriptor = new TableDescriptor(
            metadata.TableName,
            corpusPath,
            "Items/Items.xml",
            IsMapped: true,
            new[] { new OverlayDescriptor(language, corpusPath, overlayPath) });
        var load = await TableWorkspace.LoadAsync(descriptor, metadata, language);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;
        var keys = table.GetRows()
            .Where(static row => row.Key is not null)
            .Select(static row => row.Key!)
            .ToArray();
        var request = new BulkEditRequest(
            "szItemName",
            BulkEditOperation.Set,
            "Перевод",
            IncludeMissingFields: true,
            keys);

        var previewWatch = Stopwatch.StartNew();
        var preview = table.PreviewBulk(request);
        previewWatch.Stop();
        Assert.True(preview.IsSuccess, preview.ErrorText);
        Assert.True(preview.Data!.ChangedCount > 0);

        var applyWatch = Stopwatch.StartNew();
        var applied = table.ApplyBulk(request);
        applyWatch.Stop();
        Assert.True(applied.IsSuccess, applied.ErrorText);

        var undoWatch = Stopwatch.StartNew();
        var undone = table.Undo();
        undoWatch.Stop();
        Assert.True(undone.IsSuccess, undone.ErrorText);

        _output.WriteLine(
            $"Bulk translation over {keys.Length} rows of Items/Items.xml: preview {previewWatch.ElapsedMilliseconds} ms, "
            + $"apply {applyWatch.ElapsedMilliseconds} ms, undo {undoWatch.ElapsedMilliseconds} ms.");
        Assert.True(
            previewWatch.ElapsedMilliseconds < GateMilliseconds,
            $"Bulk translation preview took {previewWatch.ElapsedMilliseconds} ms, exceeding the {GateMilliseconds} ms gate.");
        Assert.True(
            applyWatch.ElapsedMilliseconds < GateMilliseconds,
            $"Bulk translation apply took {applyWatch.ElapsedMilliseconds} ms, exceeding the {GateMilliseconds} ms gate.");
        Assert.True(
            undoWatch.ElapsedMilliseconds < GateMilliseconds,
            $"Bulk translation undo took {undoWatch.ElapsedMilliseconds} ms, exceeding the {GateMilliseconds} ms gate.");
    }

    private async Task MeasureLoadAsync(
        string relativePath,
        string tableName,
        string root,
        string recordElement,
        string keyField)
    {
        var corpusPath = FindCorpusFile(relativePath);
        if (corpusPath is null)
        {
            return;
        }

        var metadata = CreateMetadata(tableName, root, recordElement, keyField);
        var descriptor = new TableDescriptor(
            TableName.Parse(tableName),
            corpusPath,
            relativePath,
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());

        var stopwatch = Stopwatch.StartNew();
        var load = await new TableFileLoader().LoadAsync(descriptor, metadata);
        stopwatch.Stop();

        Assert.True(load.IsSuccess, load.ErrorText);
        Assert.Equal(TableDocumentStatus.Loaded, load.Data!.Status);
        var document = load.Data.GetDocument<TableDocument>()!;
        var elapsed = stopwatch.ElapsedMilliseconds;
        _output.WriteLine($"{relativePath}: loaded {document.Records.Count} records in {elapsed} ms.");
        Assert.True(
            elapsed < GateMilliseconds,
            $"Loading {relativePath} took {elapsed} ms, exceeding the {GateMilliseconds} ms gate.");
    }

    /// <summary>
    /// Evaluating a structured filter over every item row stays within the interactive gate.
    /// </summary>
    [Fact]
    public async Task Filter_ItemsXml_RespondsWithinGate()
    {
        var corpusPath = FindCorpusFile("Items/Items.xml");
        if (corpusPath is null)
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "Items.xml");
        File.Copy(corpusPath, path, overwrite: true);
        var metadata = CreateMetadata("Items.Items", "ITEMLIST", "ITEM", "uiIndex");
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await TableWorkspace.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        var table = load.Data!;
        var rows = table.GetRows();
        var filter = new RowFilterGroup(
            RowFilterLogicalOperator.And,
            new[]
            {
                new RowFilterCriterion("ubWeight", RowFilterOperator.GreaterThan, "0"),
                new RowFilterCriterion("szItemName", RowFilterOperator.Contains, "Gun"),
            });

        var stopwatch = Stopwatch.StartNew();
        var matched = rows
            .Where(row => TableRowFilterMatcher.Matches(row, table.Schema, filter))
            .ToArray();
        stopwatch.Stop();

        var elapsed = stopwatch.ElapsedMilliseconds;
        _output.WriteLine(
            $"Items/Items.xml: filtered {rows.Count} rows to {matched.Length} in {elapsed} ms.");
        Assert.True(
            elapsed < GateMilliseconds,
            $"Filtering Items/Items.xml took {elapsed} ms, exceeding the {GateMilliseconds} ms gate.");
    }

    private string? FindCorpusFile(string relativePath)
    {
        var corpusDirectory = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(corpusDirectory) || !Directory.Exists(corpusDirectory))
        {
            _output.WriteLine("XER_CORPUS_DIR is not set; the performance gate was skipped.");
            return null;
        }

        var path = Path.Combine(corpusDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(path))
        {
            return path;
        }

        _output.WriteLine($"Corpus file '{relativePath}' is not present; the performance gate was skipped.");
        return null;
    }

    private static TableMetadata CreateMetadata(
        string tableName,
        string root,
        string recordElement,
        string keyField) =>
        new(
            TableName.Parse(tableName),
            root,
            recordElement,
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, keyField) }));
}
