using System.Globalization;
using System.Windows.Media.Imaging;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.App.Views;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Xml.Repository;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Smoke test that the image viewer XAML loads on an STA thread and binds its view model.
/// </summary>
public class ImageViewerWindowTests
{
    /// <summary>
    /// The window can be constructed and closed without an application instance.
    /// </summary>
    [Fact]
    public void Window_LoadsXamlAndBindsViewModel()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                CultureInfo.CurrentUICulture = new CultureInfo("en");
                var viewModel = new ImageViewerViewModel(
                    new NoopImageService(),
                    new FileSystemTableCatalog(),
                    new TableRepository());
                var window = new ImageViewerWindow(viewModel);

                Assert.Same(viewModel, window.DataContext);
                Assert.Equal("Item images", window.Title);
                window.Close();
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The viewer window test did not finish in time.");
        Assert.Null(failure);
    }

    private sealed class NoopImageService : IItemImageService
    {
        public Task<OperationResult<BitmapSource>> GetImageAsync(
            string gameDataDirectory,
            ImageSize size,
            int graphicType,
            int graphicIndex,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<BitmapSource>.Failure(
                ErrorCodes.ImageNotFound,
                "The image does not exist."));
    }
}
