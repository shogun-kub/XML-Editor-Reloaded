using System.Diagnostics;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;
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
