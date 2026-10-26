using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Abstractions;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for restoring and capturing the main-window placement. The window is built through the real
/// composition root on an STA thread.
/// </summary>
public class MainWindowPlacementTests
{
    /// <summary>
    /// The saved placement and capture logic keep the window reachable on the current virtual screen.
    /// </summary>
    [Fact]
    public void ApplyAndCapture_HandlesSavedPlacements()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var workspace = new TestWorkspace();
                var services = new ServiceCollection();
                App.ConfigureServices(services, workspace.PathOf("metadata.sqlite"));
                using var provider = services.BuildServiceProvider();
                var window = provider.GetRequiredService<MainWindow>();
                try
                {
                    window.ApplySavedPlacement(new WindowPlacement(100, 80, 1280, 780, false));
                    Assert.Equal(WindowStartupLocation.Manual, window.WindowStartupLocation);
                    Assert.Equal(100, window.Left);
                    Assert.Equal(80, window.Top);
                    Assert.Equal(1280, window.Width);
                    Assert.Equal(780, window.Height);

                    window.ApplySavedPlacement(new WindowPlacement(
                        SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth + 500,
                        SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight + 500,
                        1280,
                        780,
                        false));
                    Assert.True(
                        window.Left + window.Width
                        <= SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth);
                    Assert.True(
                        window.Top + window.Height
                        <= SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight);

                    window.ApplySavedPlacement(new WindowPlacement(0, 0, 900, 300, false));
                    Assert.True(window.Width >= window.MinWidth);
                    Assert.True(window.Height >= window.MinHeight);

                    window.ApplySavedPlacement(new WindowPlacement(120, 90, 1280, 780, true));
                    Assert.Equal(WindowState.Maximized, window.WindowState);

                    window.WindowState = WindowState.Normal;
                    window.Left = 140;
                    window.Top = 120;
                    window.Width = 1300;
                    window.Height = 820;
                    var captured = window.CapturePlacement();
                    Assert.NotNull(captured);
                    Assert.Equal(140, captured!.Left);
                    Assert.Equal(120, captured.Top);
                    Assert.Equal(1300, captured.Width);
                    Assert.Equal(820, captured.Height);
                    Assert.False(captured.IsMaximized);

                    window.WindowState = WindowState.Minimized;
                    Assert.Null(window.CapturePlacement());
                }
                finally
                {
                    window.Close();
                }
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

        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "The main-window placement test did not finish in time.");
        Assert.Null(failure);
    }

    /// <summary>
    /// A missing or corrupt placement keeps the designer defaults instead of moving the window.
    /// </summary>
    [Fact]
    public void ApplySavedPlacement_MissingOrCorruptPlacement_KeepsDefaults()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var workspace = new TestWorkspace();
                var services = new ServiceCollection();
                App.ConfigureServices(services, workspace.PathOf("metadata.sqlite"));
                using var provider = services.BuildServiceProvider();
                var window = provider.GetRequiredService<MainWindow>();
                try
                {
                    window.ApplySavedPlacement(null);
                    Assert.Equal(WindowStartupLocation.CenterScreen, window.WindowStartupLocation);

                    window.ApplySavedPlacement(new WindowPlacement(double.NaN, double.NaN, 0, 0, false));
                    Assert.Equal(WindowStartupLocation.CenterScreen, window.WindowStartupLocation);
                }
                finally
                {
                    window.Close();
                }
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

        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "The main-window placement test did not finish in time.");
        Assert.Null(failure);
    }
}
