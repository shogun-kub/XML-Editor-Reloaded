using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Comparison;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Environment-gated cross-version comparison against the two reference data sets.
/// Set <c>XER_COMPARE_LEFT_DIR</c> to v4 <c>TableData</c> and <c>XER_COMPARE_RIGHT_DIR</c>
/// to v5 <c>TableData</c> to enable it.
/// </summary>
public class RealDataComparisonTests
{
    /// <summary>
    /// The v4/v5 comparison reports the known table, record-count, overlay, and malformed-lookup
    /// differences and does not modify either directory.
    /// </summary>
    [Fact]
    public async Task CompareReferenceDataSets_ReportsKnownDifferences()
    {
        var leftDirectory = Environment.GetEnvironmentVariable("XER_COMPARE_LEFT_DIR");
        var rightDirectory = Environment.GetEnvironmentVariable("XER_COMPARE_RIGHT_DIR");
        if (string.IsNullOrWhiteSpace(leftDirectory)
            || string.IsNullOrWhiteSpace(rightDirectory)
            || !Directory.Exists(leftDirectory)
            || !Directory.Exists(rightDirectory))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var comparer = new TableDataComparer(catalog, provider);
        var before = Snapshot(leftDirectory).Concat(Snapshot(rightDirectory)).ToArray();

        var result = await comparer.CompareAsync(leftDirectory, rightDirectory);

        Assert.True(result.IsSuccess, result.ErrorText);
        var report = result.Data!;
        Assert.Empty(report.Warnings);
        Assert.Equal(238, report.Summary.LeftTableCount);
        Assert.Equal(239, report.Summary.RightTableCount);
        Assert.Equal(238, report.Summary.CommonTableCount);
        Assert.Equal(0, report.Summary.LeftOnlyTableCount);
        Assert.Equal(1, report.Summary.RightOnlyTableCount);
        var addedFields = report.Tables
            .Where(table => table.Status == TableComparisonStatus.Both)
            .SelectMany(table => table.Fields
                .Where(field => field.Status == FieldComparisonStatus.RightOnly)
                .Select(field => $"{table.TableName.Value}.{field.Name}"))
            .ToList();
        var removedFields = report.Tables
            .Where(table => table.Status == TableComparisonStatus.Both)
            .SelectMany(table => table.Fields
                .Where(field => field.Status == FieldComparisonStatus.LeftOnly)
                .Select(field => $"{table.TableName.Value}.{field.Name}"))
            .ToList();
        Assert.True(addedFields.Count == 0, $"Added fields: {string.Join(", ", addedFields)}");
        Assert.True(removedFields.Count == 0, $"Removed fields: {string.Join(", ", removedFields)}");
        Assert.Equal(0, report.Summary.AddedFieldCount);
        Assert.Equal(0, report.Summary.RemovedFieldCount);
        Assert.Equal(1, report.Summary.MalformedTableCount);
        Assert.Equal(0, report.Summary.FailedTableCount);

        var rightOnly = Assert.Single(
            report.Tables,
            table => table.Status == TableComparisonStatus.RightOnly);
        Assert.Equal("Email.EmailSenderNameList", rightOnly.TableName.Value);
        Assert.Null(rightOnly.Left);
        var expectedRightOnlyRecords = System.Xml.Linq.XDocument
            .Load(Path.Combine(rightDirectory, "Email", "EmailSenderNameList.xml"))
            .Root!.Elements()
            .Count();
        Assert.Equal(expectedRightOnlyRecords, rightOnly.Right!.Snapshot!.RecordCount);

        var attachmentPoint = Assert.Single(
            report.Tables,
            table => table.TableName.Value == "Lookup.AttachmentPoint");
        Assert.Equal(TableSideStatus.Loaded, attachmentPoint.Left!.Status);
        Assert.Equal(TableSideStatus.Malformed, attachmentPoint.Right!.Status);

        AssertRecordCount(report, "Items.Items", 1776, 1775);
        AssertRecordCount(report, "Items.Weapons", 389, 388);
        AssertRecordCount(report, "Items.Attachments", 6079, 6023);

        Assert.Equal(
            25,
            report.Tables.Count(table => table.Left?.Snapshot?.OverlayLanguages.Count > 0));
        Assert.All(
            report.Tables.Where(table => table.Left?.Snapshot?.OverlayLanguages.Count > 0),
            table => Assert.Equal(new[] { "ru" }, table.Left!.Snapshot!.OverlayLanguages));
        Assert.All(
            report.Tables,
            table => Assert.Empty(table.Right?.Snapshot?.OverlayLanguages ?? Array.Empty<string>()));

        var itemClass = Assert.Single(
            report.LookupReferences,
            reference => reference.Table.Value == "Items.Items"
                && reference.FieldName == "usItemClass");
        Assert.Equal(LookupAvailability.Available, itemClass.LeftAvailability);
        Assert.Equal(LookupAvailability.Available, itemClass.RightAvailability);

        var attachmentReferences = report.LookupReferences
            .Where(reference => reference.LookupTable == "Lookup.AttachmentPoint")
            .ToList();
        Assert.NotEmpty(attachmentReferences);
        Assert.All(
            attachmentReferences,
            reference =>
            {
                Assert.Equal(LookupAvailability.Available, reference.LeftAvailability);
                Assert.Equal(LookupAvailability.Unreadable, reference.RightAvailability);
            });

        var markdown = TableComparisonReportFormatter.FormatMarkdown(report);
        Assert.Contains("| Items.Items | Both (changed)", markdown, StringComparison.Ordinal);
        Assert.Contains("Lookup.AttachmentPoint", markdown, StringComparison.Ordinal);
        Assert.Contains("| Email.EmailSenderNameList | Only right", markdown, StringComparison.Ordinal);
        Assert.Contains("Left: not present", markdown, StringComparison.Ordinal);
        Assert.Equal(before, Snapshot(leftDirectory).Concat(Snapshot(rightDirectory)).ToArray());
    }

    private static void AssertRecordCount(
        TableComparisonReport report,
        string tableName,
        int left,
        int right)
    {
        var entry = Assert.Single(report.Tables, table => table.TableName.Value == tableName);
        Assert.Equal(left, entry.Left!.Snapshot!.RecordCount);
        Assert.Equal(right, entry.Right!.Snapshot!.RecordCount);
    }

    private static IReadOnlyList<string> Snapshot(string directory) =>
        Directory.EnumerateFiles(directory, "*.xml", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(path => $"{path}|{Convert.ToHexString(File.ReadAllBytes(path))}")
            .ToList();
}
