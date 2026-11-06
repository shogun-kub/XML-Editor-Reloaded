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
                    var screenLeft = SystemParameters.VirtualScreenLeft;
                    var screenTop = SystemParameters.VirtualScreenTop;
                    var screenWidth = SystemParameters.VirtualScreenWidth;
                    var screenHeight = SystemParameters.VirtualScreenHeight;
                    var placedWidth = Math.Min(1200, screenWidth);
                    var placedHeight = Math.Min(700, screenHeight);
                    var placedLeft = screenLeft + Math.Min(100, Math.Max(0, screenWidth - placedWidth));
                    var placedTop = screenTop + Math.Min(80, Math.Max(0, screenHeight - placedHeight));

                    window.ApplySavedPlacement(new WindowPlacement(
                        placedLeft,
                        placedTop,
                        placedWidth,
                        placedHeight,
                        false));
                    Assert.Equal(WindowStartupLocation.Manual, window.WindowStartupLocation);
                    Assert.Equal(placedLeft, window.Left);
                    Assert.Equal(placedTop, window.Top);
                    Assert.Equal(placedWidth, window.Width);
                    Assert.Equal(placedHeight, window.Height);

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
                    Assert.True(window.Width >= Math.Min(window.MinWidth, screenWidth));
                    Assert.True(window.Height >= Math.Min(window.MinHeight, screenHeight));

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
