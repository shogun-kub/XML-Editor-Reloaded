using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Imaging.Catalog;

namespace XmlEditorReloaded.Imaging.Tests;

/// <summary>
/// Tests that image loading honors cancellation.
/// </summary>
public class CancellationTests
{
    /// <summary>
    /// A pre-cancelled token is reported before any frame is decoded.
    /// </summary>
    [Fact]
    public async Task Catalog_PreCancelledToken_Throws()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.CopyFixtureDirectory("Graphics");
        var catalog = new ItemImageCatalog(dataDirectory);
        using var source = new CancellationTokenSource();
        source.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => catalog.GetImageAsync(ImageSize.Big, 2, 35, source.Token));
    }
}
