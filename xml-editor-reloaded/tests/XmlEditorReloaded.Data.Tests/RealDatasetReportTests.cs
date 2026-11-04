using System.Globalization;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Search;
using XmlEditorReloaded.Xml.Validation;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Environment-gated data-set-wide validation and search checks against a real game <c>TableData</c>
/// directory. Set <c>XER_CORPUS_DIR</c> to a v4 or v5 TableData directory to enable them.
/// </summary>
public class RealDatasetReportTests
{
    /// <summary>
    /// Validating the whole corpus produces one report with exact counts and no unexpected problem.
    /// </summary>
    [Fact]
    public async Task DatasetValidation_ReportsWholeDataSet()
    {
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var tables = await catalog.GetTablesAsync(tableData);
        Assert.True(tables.IsSuccess, tables.ErrorText);

        var report = await new DatasetValidator(provider)
            .ValidateAsync(tableData, tables.Data!, lookups: null);

        Assert.True(report.Tables.Count >= 200, Describe(report));
        Assert.True(report.UnmappedTableCount > 0, Describe(report));
        Assert.True(report.ValidatedTableCount >= 200, Describe(report));
        Assert.True(report.ErrorCount <= 1, Describe(report));
        Assert.True(report.RecordCount > 25000, Describe(report));
        Assert.True(report.WarningCount <= 4, Describe(report));
    }

    /// <summary>
    /// The global search finds base and overlay values and keeps its counts consistent with the hits.
    /// </summary>
    [Fact]
    public async Task DatasetSearch_FindsValuesAcrossTables()
    {
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var tables = await catalog.GetTablesAsync(tableData);
        Assert.True(tables.IsSuccess, tables.ErrorText);
        var service = new DatasetSearchService(provider);

        var nothing = await service.SearchAsync(tables.Data!, "Nothing");
        var nothingItems = await service.SearchAsync(
            tables.Data!.Where(table => table.TableName.Value == "Items.Items").ToArray(),
            "Nothing");
        var russian = await service.SearchAsync(
            tables.Data!.Where(table => table.TableName.Value == "Items.Items").ToArray(),
            "Ничто");

        Assert.True(nothing.MatchCount > 0, Describe(nothing));
        Assert.True(nothing.MatchedTableCount > 1, Describe(nothing));
        Assert.True(nothing.Hits.Count + nothing.SuppressedHitCount == nothing.MatchCount, Describe(nothing));
        Assert.True(
            nothingItems.Hits.Count > 0
            && nothingItems.Hits.All(hit => hit.TableName.Value == "Items.Items"),
            Describe(nothingItems));
        if (russian.MatchCount > 0)
        {
            Assert.True(russian.Hits.All(hit => hit.LanguageCode == "ru"), Describe(russian));
        }
    }

    private static string Describe(DatasetValidationReport report)
    {
        var codes = report.Tables
            .SelectMany(static table => table.Issues)
            .GroupBy(static issue => issue.Code)
            .Select(group => string.Format(CultureInfo.InvariantCulture, "{0}={1}", group.Key, group.Count()));
        var states = report.Tables
            .Where(static table => table.State != TableValidationState.Validated)
            .Select(table => string.Format(
                CultureInfo.InvariantCulture,
                "{0}:{1}",
                table.TableName.Value,
                table.State));
        return string.Format(
            CultureInfo.InvariantCulture,
            "tables={0} validated={1} malformed={2} unreadable={3} unmapped={4} records={5} "
            + "errors={6} warnings={7} info={8} suppressed={9}; codes: {10}; states: {11}",
            report.Tables.Count,
            report.ValidatedTableCount,
            report.MalformedTableCount,
            report.UnreadableTableCount,
            report.UnmappedTableCount,
            report.RecordCount,
            report.ErrorCount,
            report.WarningCount,
            report.InfoCount,
            report.SuppressedIssueCount,
            string.Join(", ", codes),
            string.Join(", ", states));
    }

    private static string Describe(DatasetSearchReport report)
    {
        var languages = report.Hits
            .GroupBy(static hit => hit.LanguageCode ?? "base")
            .Select(group => string.Format(CultureInfo.InvariantCulture, "{0}={1}", group.Key, group.Count()));
        return string.Format(
            CultureInfo.InvariantCulture,
            "term='{0}' matches={1} retained={2} suppressed={3} tables={4} searched={5} failed={6}; "
            + "languages: {7}",
            report.Term,
            report.MatchCount,
            report.Hits.Count,
            report.SuppressedHitCount,
            report.MatchedTableCount,
            report.SearchedTableCount,
            report.FailedTableCount,
            string.Join(", ", languages));
    }
}
