using System.Text;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests that scalar edits preserve the original file's formatting instead of reserializing it.
/// </summary>
public class LosslessPatchTests
{
    /// <summary>
    /// Editing one value changes only that value and keeps declaration casing, attribute spacing,
    /// entity references, bare greater-than characters, and empty-element style intact.
    /// </summary>
    [Fact]
    public async Task Save_EditedValue_PreservesOriginalFormatting()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("quirky.xml");
        var originalText = Encoding.UTF8.GetString(await File.ReadAllBytesAsync(path));
        var document = await LoadAsync(path);
        Assert.True(document.Records[0].TrySetValue("szItemName", "New"));

        var result = await new TableFileSaver().SaveAsync(document);

        Assert.True(result.IsSuccess, result.ErrorText);
        var savedText = Encoding.UTF8.GetString(await File.ReadAllBytesAsync(path));
        var expected = originalText.Replace(
            "<szItemName>Old</szItemName>",
            "<szItemName>New</szItemName>",
            StringComparison.Ordinal);
        Assert.Equal(expected, savedText);
    }

    /// <summary>
    /// Filling a self-closing element preserves its attributes and expands only that element.
    /// </summary>
    [Fact]
    public async Task Save_SelfClosingElement_ExpandsOnlyThatElement()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("quirky.xml");
        var originalText = Encoding.UTF8.GetString(await File.ReadAllBytesAsync(path));
        var document = await LoadAsync(path);
        Assert.True(document.Records[0].TrySetValue("Empty", "filled"));

        var result = await new TableFileSaver().SaveAsync(document);

        Assert.True(result.IsSuccess, result.ErrorText);
        var savedText = Encoding.UTF8.GetString(await File.ReadAllBytesAsync(path));
        var expected = originalText.Replace("<Empty/>", "<Empty>filled</Empty>", StringComparison.Ordinal);
        Assert.Equal(expected, savedText);
    }

    /// <summary>
    /// A second save compares against the previously saved baseline, so reverting a value works.
    /// </summary>
    [Fact]
    public async Task Save_SecondEditAfterSave_UsesUpdatedBaseline()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("quirky.xml");
        var document = await LoadAsync(path);
        var saver = new TableFileSaver();
        var record = document.Records[0];

        Assert.True(record.TrySetValue("szItemName", "First"));
        Assert.True((await saver.SaveAsync(document)).IsSuccess);
        Assert.True(record.TrySetValue("szItemName", "Second"));
        Assert.True((await saver.SaveAsync(document)).IsSuccess);
        Assert.True(record.TrySetValue("szItemName", "Old"));
        Assert.True((await saver.SaveAsync(document)).IsSuccess);

        var savedText = Encoding.UTF8.GetString(await File.ReadAllBytesAsync(path));
        Assert.Contains("<szItemName>Old</szItemName>", savedText, StringComparison.Ordinal);
    }

    private static async Task<TableDocument> LoadAsync(string path)
    {
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: false,
            Array.Empty<OverlayDescriptor>());
        var load = await new TableFileLoader().LoadAsync(descriptor);
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }
}
