using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests that XML load and save operations honor cancellation without leaving partial writes.
/// </summary>
public class CancellationTests
{
    /// <summary>
    /// Loading with an already-cancelled token throws instead of reading the file.
    /// </summary>
    [Fact]
    public async Task Load_PreCancelledToken_Throws()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        using var source = new CancellationTokenSource();
        source.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => new TableFileLoader().LoadAsync(Descriptor(path), cancellationToken: source.Token));
    }

    /// <summary>
    /// Saving a dirty document with an already-cancelled token leaves the file untouched.
    /// </summary>
    [Fact]
    public async Task Save_PreCancelledToken_DoesNotWrite()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        Assert.True(document.Records[0].TrySetValue("szItemName", "Changed"));
        var before = await File.ReadAllBytesAsync(path);
        var lastWrite = File.GetLastWriteTimeUtc(path);
        using var source = new CancellationTokenSource();
        source.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => new TableFileSaver().SaveAsync(document, source.Token));

        Assert.Equal(before, await File.ReadAllBytesAsync(path));
        Assert.Equal(lastWrite, File.GetLastWriteTimeUtc(path));
        Assert.True(document.IsDirty);
    }

    private static async Task<TableDocument> LoadAsync(string path)
    {
        var load = await new TableFileLoader().LoadAsync(Descriptor(path));
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }

    private static TableDescriptor Descriptor(string path) =>
        new(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: false,
            Array.Empty<OverlayDescriptor>());
}
