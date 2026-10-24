using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for structural edits that must preserve the surrounding file formatting.
/// </summary>
public class StructuralPatchTests
{
    private static readonly TableFileLoader Loader = new();
    private static readonly TableFileSaver Saver = new();

    /// <summary>
    /// Appending a record inserts a formatted block and leaves every other byte untouched.
    /// </summary>
    [Fact]
    public async Task Save_AddRecord_InsertsFormattedRecordAndPreservesFile()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var original = await File.ReadAllTextAsync(path);
        var document = await LoadAsync(path);
        Assert.True(document.AddRecord().IsSuccess);

        var result = await Saver.SaveAsync(document);

        Assert.True(result.IsSuccess, result.ErrorText);
        var expectedBlock = "\t<ITEM>\r\n\t\t<uiIndex>2</uiIndex>\r\n\t</ITEM>\r\n";
        var expected = original.Replace("</ITEMLIST>", expectedBlock + "</ITEMLIST>", StringComparison.Ordinal);
        Assert.Equal(expected, await File.ReadAllTextAsync(path));
    }

    /// <summary>
    /// Duplicating a record inserts a fully formatted copy after the source record.
    /// </summary>
    [Fact]
    public async Task Save_DuplicateRecord_InsertsFormattedCopy()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var original = await File.ReadAllTextAsync(path);
        var document = await LoadAsync(path);
        Assert.True(document.DuplicateRecord("1").IsSuccess);

        var result = await Saver.SaveAsync(document);

        Assert.True(result.IsSuccess, result.ErrorText);
        var expectedBlock =
            "\t<ITEM>\r\n"
            + "\t\t<uiIndex>2</uiIndex>\r\n"
            + "\t\t<szItemName>Glock 17</szItemName>\r\n"
            + "\t\t<ubWeight>10</ubWeight>\r\n"
            + "\t\t<ProjectionFactor>1.0</ProjectionFactor>\r\n"
            + "\t\t<AvailableAttachmentPoint>1</AvailableAttachmentPoint>\r\n"
            + "\t\t<AvailableAttachmentPoint>2</AvailableAttachmentPoint>\r\n"
            + "\t</ITEM>\r\n";
        var expected = original.Replace("</ITEMLIST>", expectedBlock + "</ITEMLIST>", StringComparison.Ordinal);
        Assert.Equal(expected, await File.ReadAllTextAsync(path));
    }

    /// <summary>
    /// Deleting a record removes its block including the indentation that preceded it.
    /// </summary>
    [Fact]
    public async Task Save_DeleteRecord_RemovesRecordAndItsIndentation()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var original = await File.ReadAllTextAsync(path);
        var document = await LoadAsync(path);
        Assert.True(document.DeleteRecord("0").IsSuccess);

        var result = await Saver.SaveAsync(document);

        Assert.True(result.IsSuccess, result.ErrorText);
        var deletedBlock =
            "\t<ITEM>\r\n"
            + "\t\t<uiIndex>0</uiIndex>\r\n"
            + "\t\t<szItemName>Nothing</szItemName>\r\n"
            + "\t\t<ubWeight>0</ubWeight>\r\n"
            + "\t\t<spreadPattern></spreadPattern>\r\n"
            + "\t\t<CustomUnknown>keep me</CustomUnknown>\r\n"
            + "\t</ITEM>\r\n";
        var expected = original.Replace(deletedBlock, string.Empty, StringComparison.Ordinal);
        Assert.Equal(expected, await File.ReadAllTextAsync(path));
    }

    /// <summary>
    /// Adding a missing field appends a formatted element to that record only.
    /// </summary>
    [Fact]
    public async Task Save_AddMissingField_InsertsFieldIntoRecord()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var original = await File.ReadAllTextAsync(path);
        var document = await LoadAsync(path);
        Assert.True(document.Records[0].SetFieldValue("NewField", "value", addIfMissing: true).IsSuccess);

        var result = await Saver.SaveAsync(document);

        Assert.True(result.IsSuccess, result.ErrorText);
        var expected = original.Replace(
            "\t\t<CustomUnknown>keep me</CustomUnknown>\r\n",
            "\t\t<CustomUnknown>keep me</CustomUnknown>\r\n\t\t<NewField>value</NewField>\r\n",
            StringComparison.Ordinal);
        Assert.Equal(expected, await File.ReadAllTextAsync(path));
    }

    /// <summary>
    /// Multiple appended records keep their order and each gets its own indentation.
    /// </summary>
    [Fact]
    public async Task Save_AddTwoRecords_InsertsBothInOrder()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var original = await File.ReadAllTextAsync(path);
        var document = await LoadAsync(path);
        Assert.True(document.AddRecord().IsSuccess);
        Assert.True(document.AddRecord().IsSuccess);

        var result = await Saver.SaveAsync(document);

        Assert.True(result.IsSuccess, result.ErrorText);
        var expectedBlock =
            "\t<ITEM>\r\n\t\t<uiIndex>2</uiIndex>\r\n\t</ITEM>\r\n"
            + "\t<ITEM>\r\n\t\t<uiIndex>3</uiIndex>\r\n\t</ITEM>\r\n";
        var expected = original.Replace("</ITEMLIST>", expectedBlock + "</ITEMLIST>", StringComparison.Ordinal);
        Assert.Equal(expected, await File.ReadAllTextAsync(path));
    }

    /// <summary>
    /// Structural insertion leaves entity references and attribute spacing in other records untouched.
    /// </summary>
    [Fact]
    public async Task Save_AddFieldToQuirkyFile_PreservesEntityReferences()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("quirky.xml");
        var original = await File.ReadAllTextAsync(path);
        var document = await LoadAsync(path);
        Assert.True(document.Records[0].SetFieldValue("NewField", "x", addIfMissing: true).IsSuccess);

        var result = await Saver.SaveAsync(document);

        Assert.True(result.IsSuccess, result.ErrorText);
        var saved = await File.ReadAllTextAsync(path);
        var newLine = original.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        Assert.Contains("<Desc>a &gt; b &amp; c</Desc>", saved, StringComparison.Ordinal);
        Assert.Contains("<Note>line1&#10;line2</Note>", saved, StringComparison.Ordinal);
        Assert.Contains("<Empty/>", saved, StringComparison.Ordinal);
        Assert.Contains("<SelfClosing attr = \"x\"/>", saved, StringComparison.Ordinal);
        Assert.Contains("<NewField>x</NewField>", saved, StringComparison.Ordinal);
        var expected = original.Replace(
            "\t\t<Bare>></Bare>" + newLine,
            "\t\t<Bare>></Bare>" + newLine + "\t\t<NewField>x</NewField>" + newLine,
            StringComparison.Ordinal);
        Assert.Equal(expected, saved);
    }

    /// <summary>
    /// Removing one occurrence of a repeated field keeps the remaining elements formatted.
    /// </summary>
    [Fact]
    public async Task Save_RemoveOneListOccurrence_KeepsFormatting()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var original = await File.ReadAllTextAsync(path);
        var document = await LoadAsync(path);
        Assert.True(document.Records[1].SetListField("AvailableAttachmentPoint", new[] { "1" }).IsSuccess);

        var result = await Saver.SaveAsync(document);

        Assert.True(result.IsSuccess, result.ErrorText);
        var expected = original.Replace(
            "\t\t<AvailableAttachmentPoint>1</AvailableAttachmentPoint>\r\n\t\t<AvailableAttachmentPoint>2</AvailableAttachmentPoint>\r\n",
            "\t\t<AvailableAttachmentPoint>1</AvailableAttachmentPoint>\r\n",
            StringComparison.Ordinal);
        Assert.Equal(expected, await File.ReadAllTextAsync(path));
    }

    private static async Task<TableDocument> LoadAsync(string path)
    {
        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }));
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await Loader.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }
}
