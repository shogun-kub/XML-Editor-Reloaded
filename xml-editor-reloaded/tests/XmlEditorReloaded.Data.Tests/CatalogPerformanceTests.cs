using System.Diagnostics;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using Xunit.Abstractions;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Environment-gated catalog and metadata performance gates.
/// Set <c>XER_CORPUS_DIR</c> to a v4 or v5 TableData directory to enable them.
/// </summary>
[Trait("Category", "Performance")]
public class CatalogPerformanceTests
{
    private const int GateMilliseconds = 2000;

    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Initializes a new instance of the <see cref="CatalogPerformanceTests"/> class.
    /// </summary>
    /// <param name="output">Test output used to report measured durations.</param>
    public CatalogPerformanceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Discovering every table in a real data set completes within the interactive gate.
    /// </summary>
    [Fact]
    public async Task Catalog_RealDataSet_RespondsWithinGate()
    {
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            _output.WriteLine("XER_CORPUS_DIR is not set; the performance gate was skipped.");
            return;
        }

        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);

        var stopwatch = Stopwatch.StartNew();
        var result = await catalog.GetTablesAsync(tableData);
        stopwatch.Stop();

        Assert.True(result.IsSuccess, result.ErrorText);
        var elapsed = stopwatch.ElapsedMilliseconds;
        _output.WriteLine($"Catalog discovered {result.Data!.Count} tables in {elapsed} ms.");
        Assert.True(
            elapsed < GateMilliseconds,
            $"Catalog discovery took {elapsed} ms, exceeding the {GateMilliseconds} ms gate.");
    }

    /// <summary>
    /// Reading every table definition from the metadata database completes within the interactive gate.
    /// </summary>
    [Fact]
    public async Task Metadata_AllTables_RespondsWithinGate()
    {
        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);

        var stopwatch = Stopwatch.StartNew();
        var result = await provider.GetAllTablesAsync();
        stopwatch.Stop();

        Assert.True(result.IsSuccess, result.ErrorText);
        var elapsed = stopwatch.ElapsedMilliseconds;
        _output.WriteLine($"Metadata returned {result.Data!.Count} table definitions in {elapsed} ms.");
        Assert.True(
            elapsed < GateMilliseconds,
            $"Metadata loading took {elapsed} ms, exceeding the {GateMilliseconds} ms gate.");
    }
}
