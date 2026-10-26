using XmlEditorReloaded.Core.Abstractions;

namespace XmlEditorReloaded.Core.Tests;

/// <summary>
/// Tests for the saved main-window placement and its virtual-screen fitting.
/// </summary>
public class WindowPlacementTests
{
    /// <summary>
    /// A placement that already fits the primary screen is restored unchanged.
    /// </summary>
    [Fact]
    public void FitToScreen_PlacementInsideScreen_IsUnchanged()
    {
        var placement = new WindowPlacement(100, 50, 1280, 780, false);

        var fitted = placement.FitToScreen(0, 0, 1920, 1080);

        Assert.Equal(placement, fitted);
    }

    /// <summary>
    /// A window saved on a removed secondary monitor is moved back inside the virtual screen.
    /// </summary>
    [Fact]
    public void FitToScreen_PlacementOutsideScreen_IsMovedIntoView()
    {
        var placement = new WindowPlacement(3000, 1200, 1280, 780, false);

        var fitted = placement.FitToScreen(0, 0, 1920, 1080);

        Assert.NotNull(fitted);
        Assert.Equal(1920 - 1280, fitted!.Left);
        Assert.Equal(1080 - 780, fitted.Top);
        Assert.Equal(1280, fitted.Width);
        Assert.Equal(780, fitted.Height);
    }

    /// <summary>
    /// A window larger than the screen is reduced to the screen so its title bar stays reachable.
    /// </summary>
    [Fact]
    public void FitToScreen_OversizedWindow_IsReducedToScreen()
    {
        var placement = new WindowPlacement(0, 0, 3000, 2000, false);

        var fitted = placement.FitToScreen(0, 0, 1920, 1080);

        Assert.NotNull(fitted);
        Assert.Equal(1920, fitted!.Width);
        Assert.Equal(1080, fitted.Height);
    }

    /// <summary>
    /// Negative virtual-screen coordinates are legitimate on a monitor placed to the left of the
    /// primary one.
    /// </summary>
    [Fact]
    public void FitToScreen_NegativeVirtualScreenCoordinates_ArePreserved()
    {
        var placement = new WindowPlacement(-1700, 100, 1280, 780, false);

        var fitted = placement.FitToScreen(-1920, 0, 3840, 1080);

        Assert.Equal(placement, fitted);
    }

    /// <summary>
    /// A maximized window keeps its maximized state while its restore bounds are fitted.
    /// </summary>
    [Fact]
    public void FitToScreen_MaximizedPlacement_KeepsState()
    {
        var placement = new WindowPlacement(5000, 5000, 1280, 780, true);

        var fitted = placement.FitToScreen(0, 0, 1920, 1080);

        Assert.NotNull(fitted);
        Assert.True(fitted!.IsMaximized);
        Assert.InRange(fitted.Left, 0, 1920 - 1280);
        Assert.InRange(fitted.Top, 0, 1080 - 780);
    }

    /// <summary>
    /// Corrupt coordinates or an unusably small size are rejected instead of producing a window with
    /// its title bar outside the screen.
    /// </summary>
    [Theory]
    [InlineData(double.NaN, 0, 1280, 780)]
    [InlineData(0, double.PositiveInfinity, 1280, 780)]
    [InlineData(0, 0, 100, 780)]
    [InlineData(0, 0, 1280, 100)]
    [InlineData(0, 0, double.NaN, 780)]
    public void FitToScreen_InvalidPlacement_IsRejected(double left, double top, double width, double height)
    {
        var placement = new WindowPlacement(left, top, width, height, false);

        Assert.Null(placement.FitToScreen(0, 0, 1920, 1080));
    }

    /// <summary>
    /// A saved window smaller than the current minimum size is raised to the minimum before it is
    /// fitted, so WPF cannot coerce it and push the right or bottom edge off-screen.
    /// </summary>
    [Fact]
    public void FitToScreen_SavedSizeBelowMinimum_IsRaisedToMinimum()
    {
        var placement = new WindowPlacement(0, 0, 900, 300, false);

        var fitted = placement.FitToScreen(0, 0, 1920, 1080, 1100, 520);

        Assert.NotNull(fitted);
        Assert.Equal(1100, fitted!.Width);
        Assert.Equal(520, fitted.Height);
    }

    /// <summary>
    /// A screen smaller than the minimum window size still wins, because an oversized window could not
    /// be reached at all.
    /// </summary>
    [Fact]
    public void FitToScreen_ScreenSmallerThanMinimum_UsesScreenSize()
    {
        var placement = new WindowPlacement(0, 0, 1280, 780, false);

        var fitted = placement.FitToScreen(0, 0, 1000, 700, 1100, 520);

        Assert.NotNull(fitted);
        Assert.Equal(1000, fitted!.Width);
        Assert.Equal(700, fitted.Height);
    }

    /// <summary>
    /// A virtual screen smaller than the minimum window size cannot host a restored placement.
    /// </summary>
    [Fact]
    public void FitToScreen_UnusableScreen_IsRejected()
    {
        var placement = new WindowPlacement(0, 0, 1280, 780, false);

        Assert.Null(placement.FitToScreen(0, 0, 100, 1080));
    }
}
