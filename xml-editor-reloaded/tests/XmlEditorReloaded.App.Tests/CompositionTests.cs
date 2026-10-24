using Microsoft.Extensions.DependencyInjection;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests that the application composition root resolves the shell and the image viewer.
/// </summary>
public class CompositionTests
{
    /// <summary>
    /// All registered services required by the main and viewer view models resolve from the container.
    /// </summary>
    [Fact]
    public void ConfigureServices_ResolvesShellAndViewer()
    {
        using var workspace = new TestWorkspace();
        var services = new ServiceCollection();

        XmlEditorReloaded.App.App.ConfigureServices(services, workspace.PathOf("metadata.sqlite"));

        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetRequiredService<MainViewModel>());
        Assert.NotNull(provider.GetRequiredService<ImageViewerViewModel>());
        Assert.NotNull(provider.GetRequiredService<IImageViewerService>());
        Assert.NotNull(provider.GetRequiredService<CompareViewModel>());
        Assert.NotNull(provider.GetRequiredService<IVersionCompareService>());
    }

    /// <summary>
    /// The main window XAML loads on an STA thread and binds the shell view model.
    /// </summary>
    [Fact]
    public void ConfigureServices_ResolvesMainWindowOnStaThread()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var workspace = new TestWorkspace();
                var services = new ServiceCollection();
                XmlEditorReloaded.App.App.ConfigureServices(services, workspace.PathOf("metadata.sqlite"));
                using var provider = services.BuildServiceProvider();

                var window = provider.GetRequiredService<global::XmlEditorReloaded.App.MainWindow>();

                Assert.IsType<MainViewModel>(window.DataContext);
                window.Close();

                var compareWindow = new global::XmlEditorReloaded.App.Views.CompareWindow(
                    provider.GetRequiredService<CompareViewModel>());
                Assert.IsType<CompareViewModel>(compareWindow.DataContext);
                compareWindow.Close();
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

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The main window test did not finish in time.");
        Assert.Null(failure);
    }
}

