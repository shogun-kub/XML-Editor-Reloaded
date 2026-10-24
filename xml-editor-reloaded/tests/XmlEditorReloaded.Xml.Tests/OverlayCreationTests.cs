using System.Text;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// M14 tests for creating new language overlays without copying the base table.
/// </summary>
public class OverlayCreationTests
{
    /// <summary>
    /// A new overlay contains only the root element, does not touch the base file, and becomes
    /// editable through the standard overlay writer.
    /// </summary>
    [Fact]
    public async Task Create_WritesEmptyOverlayAndWritesLazyTranslations()
    {
        using var workspace = new TestWorkspace();
        var basePath = workspace.CopyFixture("Overlays/items-base.xml");
        var overlayPath = Path.Combine(workspace.DirectoryPath, "Russian.Items.xml");
        var metadata = OverlayTestData.ItemsMetadata();
        var descriptor = new OverlayDescriptor(OverlayTestData.Russian, basePath, overlayPath);
        var baseBefore = await File.ReadAllBytesAsync(basePath);

        var create = await OverlayDocument.CreateAsync(descriptor, metadata);

        Assert.True(create.IsSuccess, create.ErrorText);
        Assert.True(File.Exists(overlayPath));
        Assert.Equal(baseBefore, await File.ReadAllBytesAsync(basePath));
        var bytes = await File.ReadAllBytesAsync(overlayPath);
        Assert.False(
            bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF,
            "New overlays must be written as UTF-8 without BOM.");
        var created = XDocument.Load(overlayPath);
        Assert.Equal("ITEMLIST", created.Root!.Name.LocalName);
        Assert.Empty(created.Root.Elements());
        Assert.Empty(create.Data!.Document.Records);

        var write = new OverlayWriter().SetValue(
            create.Data,
            OverlayRecordKey.ForRecord("1"),
            FieldPath.Parse("szItemName"),
            "Глок 17");
        Assert.True(write.IsSuccess, write.ErrorText);
        var save = await create.Data.SaveAsync();
        Assert.True(save.IsSuccess, save.ErrorText);

        Assert.Equal(baseBefore, await File.ReadAllBytesAsync(basePath));
        var saved = XDocument.Load(overlayPath);
        var item = Assert.Single(saved.Root!.Elements("ITEM"));
        Assert.Equal("1", item.Element("uiIndex")?.Value);
        Assert.Equal("Глок 17", item.Element("szItemName")?.Value);
        Assert.Equal(2, item.Elements().Count());
    }

    /// <summary>
    /// Creation refuses to overwrite an existing overlay, even an empty one.
    /// </summary>
    [Fact]
    public async Task Create_ExistingOverlay_ReturnsOverlayExists()
    {
        using var workspace = new TestWorkspace();
        var basePath = workspace.CopyFixture("Overlays/items-base.xml");
        var overlayPath = Path.Combine(workspace.DirectoryPath, "Russian.Items.xml");
        var descriptor = new OverlayDescriptor(OverlayTestData.Russian, basePath, overlayPath);
        var metadata = OverlayTestData.ItemsMetadata();
        Assert.True((await OverlayDocument.CreateAsync(descriptor, metadata)).IsSuccess);
        var existing = await File.ReadAllTextAsync(overlayPath, Encoding.UTF8);

        var second = await OverlayDocument.CreateAsync(descriptor, metadata);

        Assert.False(second.IsSuccess);
        Assert.Equal(ErrorCodes.OverlayExists, second.ErrorCode);
        Assert.Equal(existing, await File.ReadAllTextAsync(overlayPath, Encoding.UTF8));
    }

    /// <summary>
    /// A pre-cancelled token leaves no overlay file behind.
    /// </summary>
    [Fact]
    public async Task Create_Cancelled_LeavesNoFile()
    {
        using var workspace = new TestWorkspace();
        var basePath = workspace.CopyFixture("Overlays/items-base.xml");
        var overlayPath = Path.Combine(workspace.DirectoryPath, "Russian.Items.xml");
        var descriptor = new OverlayDescriptor(OverlayTestData.Russian, basePath, overlayPath);
        var metadata = OverlayTestData.ItemsMetadata();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => OverlayDocument.CreateAsync(descriptor, metadata, cancellationToken: cancellation.Token));

        Assert.False(File.Exists(overlayPath));
    }
}
