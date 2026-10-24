using System.Xml.Linq;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Validation;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Environment-gated M22c acceptance checks for compound record keys on a real <c>TableData</c>
/// directory. Set <c>XER_CORPUS_DIR</c> to a v4 or v5 TableData directory to enable them.
/// </summary>
public class RealRecordKeyTests
{
    private static readonly string[] CompoundTables =
    {
        "Items.Attachments",
        "Items.CompatibleFaceItems",
        "Items.IncompatibleAttachments",
        "Items.Item_Transformations",
        "Items.Launchables",
        "Items.Merges",
        "Items.StructureConstruct",
        "Items.StructureDeconstruct",
    };

    /// <summary>
    /// The eight compound tables validate with no missing keys, no other false duplicates, and only the
    /// two known v5 attachment duplicate groups plus the known dealer duplicate.
    /// </summary>
    [Fact]
    public async Task CompoundTables_HaveExpectedDuplicateAndMissingBaselines()
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
        var loader = new TableFileLoader();
        var validator = new TableValidator();
        var isRussianBuild = File.Exists(Path.Combine(tableData, "Items", "Russian.Items.xml"));

        foreach (var tableName in CompoundTables)
        {
            var metadata = await provider.GetTableAsync(TableName.Parse(tableName));
            Assert.True(metadata.IsSuccess, metadata.ErrorText);
            Assert.True(metadata.Data!.HasCompoundRecordKey, tableName);

            var document = await LoadDocumentAsync(loader, tableData, metadata.Data);
            var report = await validator.ValidateAsync(document);
            var duplicates = report.Issues.Count(static issue => issue.Code == ValidationCodes.DuplicateKey);
            var missing = report.Issues.Count(static issue => issue.Code == ValidationCodes.MissingKey);

            Assert.Equal(0, missing);
            var expected = tableName == "Items.Attachments" && !isRussianBuild ? 2 : 0;
            Assert.True(
                duplicates == expected,
                $"{tableName}: expected {expected} duplicate-key warnings, got {duplicates}.");
        }

        var dealer = await provider.GetTableAsync(TableName.Parse("NPCInventory.AdditionalDealer_43_Inventory"));
        Assert.True(dealer.IsSuccess, dealer.ErrorText);
        var dealerDocument = await LoadDocumentAsync(loader, tableData, dealer.Data!);
        var dealerReport = await validator.ValidateAsync(dealerDocument);
        Assert.Equal(
            1,
            dealerReport.Issues.Count(static issue => issue.Code == ValidationCodes.DuplicateKey));
    }

    /// <summary>
    /// The attachment rows of item 12 in the row view exactly match the raw XML instead of inheriting
    /// the values of other rows that share an attachment index.
    /// </summary>
    [Fact]
    public async Task Item12Attachments_RowViewMatchesRawXml()
    {
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var attachmentsPath = Path.Combine(workspace.DirectoryPath, "Attachments.xml");
        var sourcePath = Path.Combine(tableData, "Items", "Attachments.xml");
        File.Copy(sourcePath, attachmentsPath);
        var rawRows = XDocument.Load(attachmentsPath, LoadOptions.PreserveWhitespace)
            .Root!
            .Elements("ATTACHMENT")
            .Where(element => element.Element("itemIndex")?.Value == "12")
            .ToArray();
        Assert.NotEmpty(rawRows);

        var metadata = await GetMetadataAsync(workspace);
        var workspaceResult = await TableWorkspace.LoadAsync(Descriptor(attachmentsPath), metadata);
        Assert.True(workspaceResult.IsSuccess, workspaceResult.ErrorText);
        var table = workspaceResult.Data!;

        var allRows = table.GetRows();
        Assert.Equal(table.BaseDocument.Records.Count, allRows.Count);
        var itemRows = allRows
            .Where(row => string.Equals(row.GetValue("itemIndex"), "12", StringComparison.Ordinal))
            .ToArray();
        Assert.Equal(rawRows.Length, itemRows.Length);
        Assert.All(itemRows, row => Assert.Equal("12", row.GetValue("itemIndex")));
        Assert.Equal(
            rawRows.Select(element => element.Element("attachmentIndex")!.Value).OrderBy(static value => value),
            itemRows.Select(row => row.GetValue("attachmentIndex")!).OrderBy(static value => value));
        Assert.Equal(
            rawRows.Select(element => element.Element("APCost")?.Value ?? string.Empty).OrderBy(static value => value),
            itemRows.Select(row => row.GetValue("APCost") ?? string.Empty).OrderBy(static value => value));
    }

    /// <summary>
    /// Deleting an attachment row whose first key component is shared removes exactly that row, and undo
    /// restores the original file without rewriting it.
    /// </summary>
    [Fact]
    public async Task DeleteSharedComponentRow_RemovesOnlyTheSelectedRow()
    {
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var attachmentsPath = Path.Combine(workspace.DirectoryPath, "Attachments.xml");
        File.Copy(Path.Combine(tableData, "Items", "Attachments.xml"), attachmentsPath);
        var original = await File.ReadAllBytesAsync(attachmentsPath);
        var metadata = await GetMetadataAsync(workspace);
        var workspaceResult = await TableWorkspace.LoadAsync(Descriptor(attachmentsPath), metadata);
        Assert.True(workspaceResult.IsSuccess, workspaceResult.ErrorText);
        var table = workspaceResult.Data!;
        var originalCount = table.BaseDocument.Records.Count;

        var allRows = table.GetRows();
        var candidate = allRows
            .Where(row => row.GetValue("itemIndex") == "12")
            .First(row => allRows.Count(other => string.Equals(
                other.GetValue("attachmentIndex"),
                row.GetValue("attachmentIndex"),
                StringComparison.Ordinal)) > 1);
        var attachmentIndex = candidate.GetValue("attachmentIndex");
        var itemIndex = candidate.GetValue("itemIndex");
        var combinationCount = table.BaseDocument.Records.Count(record =>
            string.Equals(record.GetRawValue("itemIndex"), itemIndex, StringComparison.Ordinal)
            && string.Equals(record.GetRawValue("attachmentIndex"), attachmentIndex, StringComparison.Ordinal));

        var deleted = table.DeleteRow(candidate);
        Assert.True(deleted.IsSuccess, deleted.ErrorText);
        Assert.Equal(original, await File.ReadAllBytesAsync(attachmentsPath));

        Assert.True(table.Undo().IsSuccess);
        Assert.False(table.IsDirty);
        Assert.True((await table.SaveAsync()).IsSuccess);
        Assert.Equal(original, await File.ReadAllBytesAsync(attachmentsPath));

        var rowToDelete = table.GetRows()
            .First(row => row.GetValue("itemIndex") == itemIndex
                && row.GetValue("attachmentIndex") == attachmentIndex);
        Assert.True(table.DeleteRow(rowToDelete).IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);

        var savedRows = XDocument.Load(attachmentsPath, LoadOptions.PreserveWhitespace)
            .Root!
            .Elements("ATTACHMENT")
            .ToArray();
        Assert.Equal(originalCount - 1, savedRows.Length);
        Assert.Equal(
            combinationCount - 1,
            savedRows.Count(element =>
                element.Element("itemIndex")?.Value == itemIndex
                && element.Element("attachmentIndex")?.Value == attachmentIndex));
    }

    private static async Task<TableMetadata> GetMetadataAsync(TestWorkspace workspace)
    {
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var result = await new SqliteMetadataProvider(databasePath)
            .GetTableAsync(TableName.Parse("Items.Attachments"));
        Assert.True(result.IsSuccess, result.ErrorText);
        return result.Data!;
    }

    private static async Task<TableDocument> LoadDocumentAsync(
        TableFileLoader loader,
        string tableData,
        TableMetadata metadata)
    {
        var relativePath = metadata.TableName.ToRelativePath();
        var descriptor = new TableDescriptor(
            metadata.TableName,
            Path.Combine(tableData, relativePath),
            relativePath,
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await loader.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }

    private static TableDescriptor Descriptor(string path) => new(
        TableName.Parse("Items.Attachments"),
        path,
        "Items/Attachments.xml",
        IsMapped: true,
        Array.Empty<OverlayDescriptor>());
}
