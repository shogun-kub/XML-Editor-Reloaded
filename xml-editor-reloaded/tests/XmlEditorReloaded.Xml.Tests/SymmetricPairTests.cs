using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for tables that store a reversed counterpart for every record, such as
/// <c>Items.IncompatibleAttachments</c> and <c>Items.CompatibleFaceItems</c>.
/// </summary>
public class SymmetricPairTests
{
    private static readonly TableName Table = TableName.Parse("Items.IncompatibleAttachments");

    /// <summary>
    /// Completing the second key of a new row creates the reversed row in the same undo step.
    /// </summary>
    [Fact]
    public async Task SettingSecondKey_CreatesCounterpartAndUndoRestoresTheSingleRow()
    {
        using var workspace = new TestWorkspace();
        var path = await WritePairFileAsync(
            workspace,
            """
            <?xml version="1.0" encoding="utf-8"?><INCOMPATIBLEATTACHMENTLIST>
            <INCOMPATIBLEATTACHMENT><itemIndex>1</itemIndex></INCOMPATIBLEATTACHMENT>
            </INCOMPATIBLEATTACHMENTLIST>
            """);
        var table = await LoadAsync(path);
        var record = Assert.Single(table.BaseDocument.Records);

        var edit = table.SetValue(
            record.GetKey()!,
            "incompatibleattachmentIndex",
            "2",
            addIfMissing: true);

        Assert.True(edit.IsSuccess, edit.ErrorText);
        AssertPairRows(table, ("1", "2"), ("2", "1"));

        Assert.True(table.Undo().IsSuccess);
        var restored = Assert.Single(table.BaseDocument.Records);
        Assert.Equal("1", restored.GetRawValue("itemIndex"));
        Assert.Null(restored.GetRawValue("incompatibleattachmentIndex"));

        Assert.True(table.Redo().IsSuccess);
        AssertPairRows(table, ("1", "2"), ("2", "1"));
    }

    /// <summary>
    /// Adding a complete row creates its counterpart, and one undo removes both.
    /// </summary>
    [Fact]
    public async Task AddRecordWithCompleteKey_CreatesCounterpart()
    {
        using var workspace = new TestWorkspace();
        var path = await WritePairFileAsync(workspace, ("9", "8"), ("8", "9"));
        var table = await LoadAsync(path);

        var added = table.AddRecord(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["itemIndex"] = "5",
            ["incompatibleattachmentIndex"] = "6",
        });

        Assert.True(added.IsSuccess, added.ErrorText);
        AssertPairRows(table, ("9", "8"), ("8", "9"), ("5", "6"), ("6", "5"));

        Assert.True(table.Undo().IsSuccess);
        AssertPairRows(table, ("9", "8"), ("8", "9"));
    }

    /// <summary>
    /// Deleting one half deletes the reversed half as well, and one undo restores the pair.
    /// </summary>
    [Fact]
    public async Task DeleteRow_RemovesCounterpartAndUndoRestoresBoth()
    {
        using var workspace = new TestWorkspace();
        var path = await WritePairFileAsync(workspace, ("1", "2"), ("2", "1"));
        var table = await LoadAsync(path);
        var row = Assert.Single(table.GetRows(), candidate => candidate.Key == Key("1", "2"));

        Assert.True(table.DeleteRow(row).IsSuccess);
        Assert.Empty(table.BaseDocument.Records);

        Assert.True(table.Undo().IsSuccess);
        AssertPairRows(table, ("1", "2"), ("2", "1"));
    }

    /// <summary>
    /// Editing a key component moves the counterpart to the new reversed key instead of leaving an
    /// orphan behind.
    /// </summary>
    [Fact]
    public async Task EditingKey_MovesCounterpart()
    {
        using var workspace = new TestWorkspace();
        var path = await WritePairFileAsync(workspace, ("1", "2"), ("2", "1"));
        var table = await LoadAsync(path);

        var edit = table.SetValue(Key("1", "2"), "itemIndex", "3");
        Assert.True(edit.IsSuccess, edit.ErrorText);
        AssertPairRows(table, ("3", "2"), ("2", "3"));

        Assert.True(table.Undo().IsSuccess);
        AssertPairRows(table, ("1", "2"), ("2", "1"));
    }

    /// <summary>
    /// Validation reports a missing counterpart and accepts a complete pair; validation does not mutate
    /// the document.
    /// </summary>
    [Fact]
    public async Task Validate_ReportsOnlyAsymmetricRows()
    {
        using var asymmetricWorkspace = new TestWorkspace();
        var asymmetricPath = await WritePairFileAsync(asymmetricWorkspace, ("1", "2"));
        var asymmetric = await LoadAsync(asymmetricPath);

        var report = await asymmetric.ValidateAsync();

        var issue = Assert.Single(report.Issues, candidate => candidate.Code == ValidationCodes.AsymmetricPair);
        Assert.Equal(ValidationSeverity.Warning, issue.Severity);
        Assert.Equal(Key("1", "2"), issue.RecordKey);
        Assert.Equal("itemIndex", issue.FieldPath);
        Assert.Single(asymmetric.BaseDocument.Records);

        using var symmetricWorkspace = new TestWorkspace();
        var symmetricPath = await WritePairFileAsync(symmetricWorkspace, ("1", "2"), ("2", "1"));
        var symmetric = await LoadAsync(symmetricPath);

        var symmetricReport = await symmetric.ValidateAsync();

        Assert.DoesNotContain(
            symmetricReport.Issues,
            candidate => candidate.Code == ValidationCodes.AsymmetricPair);
    }

    private static async Task<TableWorkspace> LoadAsync(string path)
    {
        var load = await TableWorkspace.LoadAsync(Descriptor(path), Metadata());
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!;
    }

    private static void AssertPairRows(TableWorkspace table, params (string First, string Second)[] expected)
    {
        var actual = table.BaseDocument.Records
            .Select(record => (
                First: record.GetRawValue("itemIndex")?.Trim(),
                Second: record.GetRawValue("incompatibleattachmentIndex")?.Trim()))
            .OrderBy(static pair => pair.First, StringComparer.Ordinal)
            .ThenBy(static pair => pair.Second, StringComparer.Ordinal)
            .ToArray();
        var expectedOrdered = expected
            .OrderBy(static pair => pair.First, StringComparer.Ordinal)
            .ThenBy(static pair => pair.Second, StringComparer.Ordinal)
            .Select(static pair => (First: (string?)pair.First, Second: (string?)pair.Second))
            .ToArray();
        Assert.Equal(expectedOrdered, actual);
    }

    private static string Key(string first, string second) =>
        RecordKeyCodec.Compose(new[] { first, second });

    private static TableMetadata Metadata() => new(
        Table,
        "INCOMPATIBLEATTACHMENTLIST",
        "INCOMPATIBLEATTACHMENT",
        isLookup: false,
        recordKeyFields: new[] { "itemIndex", "incompatibleattachmentIndex" },
        symmetricKeyPair: new SymmetricKeyPair("itemIndex", "incompatibleattachmentIndex"));

    private static TableDescriptor Descriptor(string path) => new(
        Table,
        path,
        "Items/IncompatibleAttachments.xml",
        IsMapped: true,
        Array.Empty<OverlayDescriptor>());

    private static async Task<string> WritePairFileAsync(
        TestWorkspace workspace,
        params (string First, string Second)[] rows)
    {
        var path = Path.Combine(workspace.DirectoryPath, "IncompatibleAttachments.xml");
        var content = "<?xml version=\"1.0\" encoding=\"utf-8\"?><INCOMPATIBLEATTACHMENTLIST>"
            + string.Concat(rows.Select(static row =>
                $"<INCOMPATIBLEATTACHMENT><itemIndex>{row.First}</itemIndex>"
                + $"<incompatibleattachmentIndex>{row.Second}</incompatibleattachmentIndex>"
                + "</INCOMPATIBLEATTACHMENT>"))
            + "</INCOMPATIBLEATTACHMENTLIST>";
        await File.WriteAllTextAsync(path, content);
        return path;
    }

    private static async Task<string> WritePairFileAsync(TestWorkspace workspace, string content)
    {
        var path = Path.Combine(workspace.DirectoryPath, "IncompatibleAttachments.xml");
        await File.WriteAllTextAsync(path, content);
        return path;
    }
}
