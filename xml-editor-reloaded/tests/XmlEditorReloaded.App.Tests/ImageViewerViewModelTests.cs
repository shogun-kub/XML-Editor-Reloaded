using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Xml.Repository;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for <see cref="ImageViewerViewModel"/>.
/// </summary>
public class ImageViewerViewModelTests
{
    /// <summary>
    /// Initialization projects the item list and auto-selects the first item with a graphic index.
    /// </summary>
    [Fact]
    public async Task Initialize_SelectsFirstItemWithGraphics_AndLoadsAllSizes()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateTableData(workspace);
        var images = new FakeItemImageService();
        images.FailSizes.Add(ImageSize.Small);
        var viewModel = CreateViewModel(images);

        await viewModel.InitializeAsync(data13);

        Assert.Equal(2, viewModel.Items.Count);
        Assert.Equal(311, viewModel.SelectedItem!.Index);
        Assert.Equal(2, viewModel.GraphicType);
        Assert.Equal(35, viewModel.GraphicIndex);
        Assert.NotNull(viewModel.BigImage);
        Assert.Equal("2×2", viewModel.BigInfoText);
        Assert.NotNull(viewModel.MediumImage);
        Assert.Null(viewModel.SmallImage);
        Assert.Contains("image.not_found", viewModel.SmallInfoText, StringComparison.Ordinal);
        Assert.Equal("Type 2, index 35.", viewModel.StatusText);
        Assert.False(viewModel.IsBusy);

        Assert.Equal(3, images.Requests.Count);
        Assert.All(images.Requests, request =>
        {
            Assert.Equal(data13, request.DataDirectory);
            Assert.Equal(2, request.GraphicType);
            Assert.Equal(35, request.GraphicIndex);
        });
    }

    /// <summary>
    /// Without a data directory the viewer reports a message and does not request images.
    /// </summary>
    [Fact]
    public async Task Initialize_NoDataDirectory_ReportsMessage()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var images = new FakeItemImageService();
        var viewModel = CreateViewModel(images);

        await viewModel.InitializeAsync(null);

        Assert.Empty(viewModel.Items);
        Assert.Null(viewModel.SelectedItem);
        Assert.Empty(images.Requests);
        Assert.Equal("Choose a data directory in the main window first.", viewModel.StatusText);
    }

    /// <summary>
    /// The refresh command uses manually entered graphic type and index.
    /// </summary>
    [Fact]
    public async Task Refresh_UsesManualGraphicValues()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateTableData(workspace);
        var images = new FakeItemImageService();
        var viewModel = CreateViewModel(images);
        await viewModel.InitializeAsync(data13);
        images.Requests.Clear();

        viewModel.GraphicType = 1;
        viewModel.GraphicIndex = 7;
        await viewModel.RefreshCommand.ExecuteAsync(null);

        Assert.Equal(3, images.Requests.Count);
        Assert.All(images.Requests, request =>
        {
            Assert.Equal(1, request.GraphicType);
            Assert.Equal(7, request.GraphicIndex);
        });
        Assert.Equal("Type 1, index 7.", viewModel.StatusText);
    }

    /// <summary>
    /// Selecting an item without <c>ubGraphicNum</c> clears the previews and explains why.
    /// </summary>
    [Fact]
    public async Task SelectItemWithoutGraphic_ClearsImages()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var data13 = CreateTableData(workspace);
        var viewModel = CreateViewModel(new FakeItemImageService());
        await viewModel.InitializeAsync(data13);

        viewModel.SelectedItem = viewModel.Items[0];

        Assert.Null(viewModel.BigImage);
        Assert.Null(viewModel.MediumImage);
        Assert.Null(viewModel.SmallImage);
        Assert.Equal("The selected item has no ubGraphicNum.", viewModel.StatusText);
    }

    /// <summary>
    /// With <c>XER_CORPUS_DIR</c> set, the viewer reads the real item list and decodes a real STI image.
    /// </summary>
    [Fact]
    public async Task Viewer_RealDataSet_LoadsItemsAndImage()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        var data13 = Directory.GetParent(tableData)?.FullName;
        Assert.NotNull(data13);
        var viewModel = new ImageViewerViewModel(
            new WpfItemImageService(),
            new FileSystemTableCatalog(),
            new TableRepository());

        await viewModel.InitializeAsync(data13);

        Assert.True(viewModel.Items.Count > 1000, $"Only {viewModel.Items.Count} items were projected.");
        viewModel.GraphicType = 2;
        viewModel.GraphicIndex = 35;
        await viewModel.RefreshCommand.ExecuteAsync(null);

        Assert.NotNull(viewModel.BigImage);
        Assert.True(viewModel.BigImage!.PixelWidth > 0);
        Assert.Equal("Type 2, index 35.", viewModel.StatusText);
    }

    private static ImageViewerViewModel CreateViewModel(FakeItemImageService images) =>
        new(images, new FileSystemTableCatalog(), new TableRepository());

    private static string CreateTableData(TestWorkspace workspace)
    {
        var data13 = workspace.PathOf("Data-1.13");
        var itemsDirectory = Path.Combine(data13, "TableData", "Items");
        Directory.CreateDirectory(itemsDirectory);
        File.WriteAllText(
            Path.Combine(itemsDirectory, "Items.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ITEMLIST>"
            + "<ITEM><uiIndex>0</uiIndex><szItemName>Nothing</szItemName></ITEM>"
            + "<ITEM><uiIndex>311</uiIndex><szItemName>String</szItemName>"
            + "<ubGraphicType>2</ubGraphicType><ubGraphicNum>35</ubGraphicNum></ITEM>"
            + "</ITEMLIST>");
        return data13;
    }

    private sealed class FakeItemImageService : IItemImageService
    {
        public List<ImageRequest> Requests { get; } = new();

        public HashSet<ImageSize> FailSizes { get; } = new();

        public Task<OperationResult<BitmapSource>> GetImageAsync(
            string gameDataDirectory,
            ImageSize size,
            int graphicType,
            int graphicIndex,
            CancellationToken cancellationToken = default)
        {
            Requests.Add(new ImageRequest(gameDataDirectory, size, graphicType, graphicIndex));
            if (FailSizes.Contains(size))
            {
                return Task.FromResult(OperationResult<BitmapSource>.Failure(
                    ErrorCodes.ImageNotFound,
                    "The image does not exist."));
            }

            var bitmap = BitmapSource.Create(
                2,
                2,
                96,
                96,
                PixelFormats.Bgra32,
                null,
                new byte[16],
                8);
            bitmap.Freeze();
            return Task.FromResult(OperationResult<BitmapSource>.Success(bitmap));
        }
    }

    private sealed record ImageRequest(
        string DataDirectory,
        ImageSize ImageSize,
        int GraphicType,
        int GraphicIndex);
}
