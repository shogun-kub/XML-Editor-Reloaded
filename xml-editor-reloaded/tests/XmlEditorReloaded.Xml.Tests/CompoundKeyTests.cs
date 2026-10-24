using System.Xml.Linq;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Overlays;
using XmlEditorReloaded.Xml.Saving;
using XmlEditorReloaded.Xml.Validation;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for compound record keys across the document, workspace, validation, and lossless save paths.
/// </summary>
public class CompoundKeyTests
{
    private const string FourRows =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?><ATTACHMENTLIST>\r\n"
        + "\t<ATTACHMENT>\r\n"
        + "\t\t<attachmentIndex>10</attachmentIndex>\r\n"
        + "\t\t<itemIndex>1</itemIndex>\r\n"
        + "\t\t<APCost>5</APCost>\r\n"
        + "\t</ATTACHMENT>\r\n"
        + "\t<ATTACHMENT>\r\n"
        + "\t\t<attachmentIndex>10</attachmentIndex>\r\n"
        + "\t\t<itemIndex>2</itemIndex>\r\n"
        + "\t\t<APCost>6</APCost>\r\n"
        + "\t</ATTACHMENT>\r\n"
        + "\t<ATTACHMENT>\r\n"
        + "\t\t<attachmentIndex>11</attachmentIndex>\r\n"
        + "\t\t<itemIndex>2</itemIndex>\r\n"
        + "\t\t<APCost>7</APCost>\r\n"
        + "\t</ATTACHMENT>\r\n"
        + "</ATTACHMENTLIST>\r\n";

    /// <summary>
    /// Records use the canonical key, the first component does not collapse rows, and a compound table
    /// cannot duplicate a record automatically.
    /// </summary>
    [Fact]
    public async Task Document_UsesCanonicalCompoundKeys()
    {
        using var workspace = new TestWorkspace();
        var path = Write(workspace, FourRows);
        var document = await LoadDocumentAsync(path);

        Assert.Equal(3, document.Records.Count);
        var first = document.Records[0];
        var second = document.Records[1];
        Assert.Equal("2:10|1:1", first.GetKey());
        Assert.Equal("10 | 1", first.KeyDisplay);
        Assert.Equal("2:10|1:2", second.GetKey());
        Assert.True(first.IsKeyComplete);
        Assert.Equal(first, document.FindRecord("2:10|1:1"));
        Assert.Equal(second, document.FindRecord("2:10|1:2"));

        var duplicate = document.DuplicateRecord(first.GetKey()!);
        Assert.False(duplicate.IsSuccess);
        Assert.Equal(ErrorCodes.KeyDuplicateNotSupported, duplicate.ErrorCode);
    }

    /// <summary>
    /// A new compound record starts without key fields, and the user completes identity by setting them.
    /// </summary>
    [Fact]
    public async Task Document_AddRecord_LeavesKeyFieldsForTheUser()
    {
        using var workspace = new TestWorkspace();
        var path = Write(workspace, FourRows);
        var document = await LoadDocumentAsync(path);

        var added = document.AddRecord();

        Assert.True(added.IsSuccess, added.ErrorText);
        var record = added.Data!;
        Assert.Empty(record.FieldNames);
        Assert.False(record.IsKeyComplete);
        Assert.True(record.TrySetValue("attachmentIndex", "12", addIfMissing: true));
        Assert.False(record.IsKeyComplete);
        Assert.True(record.TrySetValue("itemIndex", "3", addIfMissing: true));
        Assert.True(record.IsKeyComplete);
        Assert.Equal("2:12|1:3", record.GetKey());
    }

    /// <summary>
    /// Deleting an exact row removes the selected duplicate, not the first row with the same key.
    /// </summary>
    [Fact]
    public async Task Document_DeleteExactRecord_KeepsOtherDuplicate()
    {
        using var workspace = new TestWorkspace();
        var path = Write(workspace, FourRows);
        var document = await LoadDocumentAsync(path);
        var first = document.Records[0];
        var copied = document.DuplicateRecordPreservingKey(first);
        Assert.True(copied.IsSuccess, copied.ErrorText);
        Assert.Equal(first.GetKey(), copied.Data!.GetKey());

        Assert.True(document.DeleteRecord(first).IsSuccess);
        var survivor = Assert.Single(document.Records, record => record.GetKey() == copied.Data.GetKey());
        Assert.Equal("5", survivor.GetRawValue("APCost"));
        Assert.Single(document.Records, record => record.GetKey() == "2:10|1:2");
    }

    /// <summary>
    /// A base-only workspace exposes one row per compound record with its own values, edits by row
    /// reference across a key change, and undoes both the edit and the deletion.
    /// </summary>
    [Fact]
    public async Task Workspace_RowsKeepTheirOwnValuesAndRowIdentity()
    {
        using var workspace = new TestWorkspace();
        var path = Write(workspace, FourRows);
        var table = await LoadWorkspaceAsync(path);

        var rows = table.GetRows();
        Assert.Equal(3, rows.Count);
        Assert.All(rows, row => Assert.Null(row.Merged));
        var first = Assert.Single(rows, row => row.Key == "2:10|1:1");
        var second = Assert.Single(rows, row => row.Key == "2:10|1:2");
        Assert.Equal("5", first.GetValue("APCost"));
        Assert.Equal("6", second.GetValue("APCost"));
        Assert.NotEqual(first.GetValue("APCost"), second.GetValue("APCost"));

        var edit = table.SetValue(first, "itemIndex", "3");
        Assert.True(edit.IsSuccess, edit.ErrorText);
        Assert.Equal("2:10|1:3", first.Key);
        Assert.Equal("10 | 3", first.KeyDisplay);
        Assert.True(table.Undo().IsSuccess);
        var restored = Assert.Single(table.GetRows(), row => row.GetValue("APCost") == "5");
        Assert.Equal("2:10|1:1", restored.Key);

        var delete = table.DeleteRow(restored);
        Assert.True(delete.IsSuccess, delete.ErrorText);
        Assert.DoesNotContain(table.GetRows(), row => row.Key == "2:10|1:1");
        Assert.Contains(table.GetRows(), row => row.Key == "2:10|1:2");
        Assert.True(table.Undo().IsSuccess);
        Assert.Contains(table.GetRows(), row => row.Key == "2:10|1:1");
    }

    /// <summary>
    /// Validation groups records by the full compound key and names the missing key fields.
    /// </summary>
    [Fact]
    public async Task Validator_ReportsCompoundDuplicatesAndMissingFields()
    {
        using var workspace = new TestWorkspace();
        const string xml =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ATTACHMENTLIST>"
            + "<ATTACHMENT><attachmentIndex>10</attachmentIndex><itemIndex>1</itemIndex></ATTACHMENT>"
            + "<ATTACHMENT><attachmentIndex>10</attachmentIndex><itemIndex>1</itemIndex></ATTACHMENT>"
            + "<ATTACHMENT><attachmentIndex>12</attachmentIndex></ATTACHMENT>"
            + "</ATTACHMENTLIST>";
        var path = Write(workspace, xml);
        var document = await LoadDocumentAsync(path);

        var report = await new TableValidator().ValidateAsync(document);

        var duplicate = Assert.Single(report.Issues, issue => issue.Code == ValidationCodes.DuplicateKey);
        Assert.Equal("2:10|1:1", duplicate.RecordKey);
        var missing = Assert.Single(report.Issues, issue => issue.Code == ValidationCodes.MissingKey);
        Assert.Contains("itemIndex", missing.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Saving after a scalar edit patches only that value, and adding then deleting a row restores the
    /// original file bytes.
    /// </summary>
    [Fact]
    public async Task Saving_PatchesCompoundRowsWithoutRewritingTheFile()
    {
        using var workspace = new TestWorkspace();
        var path = Write(workspace, FourRows);
        var original = await File.ReadAllBytesAsync(path);
        var table = await LoadWorkspaceAsync(path);

        Assert.True((await table.SaveAsync()).IsSuccess);
        Assert.Equal(original, await File.ReadAllBytesAsync(path));

        var first = Assert.Single(table.GetRows(), row => row.Key == "2:10|1:2");
        Assert.True(table.SetValue(first, "APCost", "66").IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);
        var edited = await File.ReadAllTextAsync(path);
        Assert.Contains("<APCost>66</APCost>", edited, StringComparison.Ordinal);
        Assert.Contains("<APCost>5</APCost>", edited, StringComparison.Ordinal);
        Assert.Contains("<APCost>7</APCost>", edited, StringComparison.Ordinal);

        var bytesAfterEdit = await File.ReadAllBytesAsync(path);
        var added = table.AddRecord(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["attachmentIndex"] = "12",
            ["itemIndex"] = "3",
        });
        Assert.True(added.IsSuccess, added.ErrorText);
        Assert.True((await table.SaveAsync()).IsSuccess);
        var addedKey = added.Data!.GetKey()
            ?? throw new InvalidOperationException("The added record has no key.");
        var addedRow = Assert.Single(table.GetRows(), row => row.Key == addedKey);
        Assert.True(table.DeleteRow(addedRow).IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);
        Assert.Equal(bytesAfterEdit, await File.ReadAllBytesAsync(path));
    }

    /// <summary>
    /// Merging a compound-key table by canonical key keeps rows that share their first component apart.
    /// </summary>
    [Fact]
    public async Task OverlayMerger_SharedFirstComponent_DoesNotMixRows()
    {
        using var workspace = new TestWorkspace();
        var basePath = Write(workspace, FourRows);
        var overlayPath = Path.Combine(workspace.DirectoryPath, "Attachments-ru.xml");
        await File.WriteAllTextAsync(
            overlayPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ATTACHMENTLIST>"
            + "<ATTACHMENT><attachmentIndex>10</attachmentIndex><itemIndex>1</itemIndex><APCost>50</APCost></ATTACHMENT>"
            + "<ATTACHMENT><attachmentIndex>10</attachmentIndex><itemIndex>2</itemIndex><APCost>60</APCost></ATTACHMENT>"
            + "</ATTACHMENTLIST>");
        var metadata = Metadata(translatableFieldPaths: new[] { "APCost" });
        var overlayDescriptor = new OverlayDescriptor(OverlayTestData.Russian, basePath, overlayPath);
        var overlayLoad = await OverlayDocument.LoadAsync(overlayDescriptor, metadata);
        Assert.True(overlayLoad.IsSuccess, overlayLoad.ErrorText);
        var baseDocument = await LoadDocumentAsync(basePath);

        var merged = new OverlayMerger(metadata).Merge(baseDocument, overlayLoad.Data!.Document);

        var first = Assert.Single(merged.Records, record => record.Key == "2:10|1:1");
        var second = Assert.Single(merged.Records, record => record.Key == "2:10|1:2");
        var firstCost = Assert.Single(first.Cells, cell => cell.FieldPath == "APCost");
        var secondCost = Assert.Single(second.Cells, cell => cell.FieldPath == "APCost");
        Assert.Equal("5", firstCost.BaseValue);
        Assert.Equal("50", firstCost.OverlayValue);
        Assert.Equal("6", secondCost.BaseValue);
        Assert.Equal("60", secondCost.OverlayValue);
    }

    /// <summary>
    /// Saving an edited key component keeps every record and every other value.
    /// </summary>
    [Fact]
    public async Task Saving_KeyEdit_KeepsAllRecordsAndValues()
    {
        using var workspace = new TestWorkspace();
        var path = Write(workspace, FourRows);
        var table = await LoadWorkspaceAsync(path);
        var first = Assert.Single(table.GetRows(), row => row.Key == "2:10|1:1");

        Assert.True(table.SetValue(first, "itemIndex", "3").IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);

        var saved = XDocument.Load(path, LoadOptions.PreserveWhitespace).Root!.Elements("ATTACHMENT").ToArray();
        Assert.Equal(3, saved.Length);
        var edited = Assert.Single(saved, element =>
            element.Element("attachmentIndex")?.Value == "10"
            && element.Element("itemIndex")?.Value == "3");
        Assert.Equal("5", edited.Element("APCost")?.Value);
        Assert.Contains(saved, element =>
            element.Element("attachmentIndex")?.Value == "10"
            && element.Element("itemIndex")?.Value == "2"
            && element.Element("APCost")?.Value == "6");
        Assert.Contains(saved, element =>
            element.Element("attachmentIndex")?.Value == "11"
            && element.Element("itemIndex")?.Value == "2"
            && element.Element("APCost")?.Value == "7");
    }

    /// <summary>
    /// A compound key table must not also declare translatable fields.
    /// </summary>
    [Fact]
    public void Metadata_CompoundTableWithoutOverlayFields_IsMarkedCompound()
    {
        var metadata = Metadata(translatableFieldPaths: null);
        Assert.True(metadata.HasCompoundRecordKey);
        Assert.True(metadata.HasRecordKey);
        Assert.Empty(metadata.TranslatableFieldPaths);
    }

    private static TableMetadata Metadata(IReadOnlyList<string>? translatableFieldPaths = null) => new(
        TableName.Parse("Items.Attachments"),
        "ATTACHMENTLIST",
        "ATTACHMENT",
        isLookup: false,
        recordKey: null,
        translatableFieldPaths: translatableFieldPaths,
        recordKeyFields: new[] { "attachmentIndex", "itemIndex" });

    private static string Write(TestWorkspace workspace, string xml)
    {
        var path = Path.Combine(workspace.DirectoryPath, "Attachments.xml");
        File.WriteAllText(path, xml);
        return path;
    }

    private static TableDescriptor Descriptor(string path) => new(
        TableName.Parse("Items.Attachments"),
        path,
        "Items/Attachments.xml",
        IsMapped: true,
        Array.Empty<OverlayDescriptor>());

    private static async Task<TableDocument> LoadDocumentAsync(string path)
    {
        var load = await new TableFileLoader().LoadAsync(Descriptor(path), Metadata());
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }

    private static async Task<TableWorkspace> LoadWorkspaceAsync(string path)
    {
        var load = await TableWorkspace.LoadAsync(Descriptor(path), Metadata());
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!;
    }
}
