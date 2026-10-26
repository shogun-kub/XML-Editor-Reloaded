namespace XmlEditorReloaded.Core.Abstractions;

/// <summary>
/// Saved position, size, and maximized state of the main window. The values are device-independent
/// pixels in the virtual-screen coordinate space, so a layout restored on a secondary monitor keeps
/// its negative coordinates.
/// </summary>
/// <param name="Left">Left edge of the restored (non-maximized) window.</param>
/// <param name="Top">Top edge of the restored (non-maximized) window.</param>
/// <param name="Width">Width of the restored (non-maximized) window.</param>
/// <param name="Height">Height of the restored (non-maximized) window.</param>
/// <param name="IsMaximized">Whether the window was maximized when it was closed.</param>
public sealed record WindowPlacement(double Left, double Top, double Width, double Height, bool IsMaximized)
{
    /// <summary>
    /// Minimum width and height of a placement that can be restored; smaller values are treated as
    /// corrupt settings rather than as a tiny window.
    /// </summary>
    public const double MinimumSize = 200;

    /// <summary>
    /// Gets a value indicating whether the placement has finite coordinates and a usable size.
    /// </summary>
    public bool HasUsableSize =>
        double.IsFinite(Left)
        && double.IsFinite(Top)
        && double.IsFinite(Width)
        && double.IsFinite(Height)
        && Width >= MinimumSize
        && Height >= MinimumSize;

    /// <summary>
    /// Fits the placement into the given virtual screen. A window that was saved on a monitor that no
    /// longer exists, or that is partly outside the screen, is moved back into view instead of being
    /// restored off-screen. The size is reduced only when it exceeds the screen.
    /// </summary>
    /// <param name="screenLeft">Left edge of the virtual screen.</param>
    /// <param name="screenTop">Top edge of the virtual screen.</param>
    /// <param name="screenWidth">Width of the virtual screen.</param>
    /// <param name="screenHeight">Height of the virtual screen.</param>
    /// <param name="minimumWidth">
    /// Smallest width the window may have, for example its <c>MinWidth</c>; defaults to
    /// <see cref="MinimumSize"/>.
    /// </param>
    /// <param name="minimumHeight">
    /// Smallest height the window may have, for example its <c>MinHeight</c>; defaults to
    /// <see cref="MinimumSize"/>.
    /// </param>
    /// <returns>
    /// A placement that lies inside the virtual screen, or <see langword="null"/> when the saved values
    /// are corrupt or the screen size is unusable.
    /// </returns>
    public WindowPlacement? FitToScreen(
        double screenLeft,
        double screenTop,
        double screenWidth,
        double screenHeight,
        double minimumWidth = MinimumSize,
        double minimumHeight = MinimumSize)
    {
        if (!HasUsableSize
            || !double.IsFinite(screenLeft)
            || !double.IsFinite(screenTop)
            || !double.IsFinite(screenWidth)
            || !double.IsFinite(screenHeight)
            || screenWidth < MinimumSize
            || screenHeight < MinimumSize)
        {
            return null;
        }

        var effectiveMinimumWidth = double.IsFinite(minimumWidth) && minimumWidth > MinimumSize
            ? minimumWidth
            : MinimumSize;
        var effectiveMinimumHeight = double.IsFinite(minimumHeight) && minimumHeight > MinimumSize
            ? minimumHeight
            : MinimumSize;
        var width = Math.Min(Math.Max(Width, effectiveMinimumWidth), screenWidth);
        var height = Math.Min(Math.Max(Height, effectiveMinimumHeight), screenHeight);
        var left = Math.Clamp(Left, screenLeft, screenLeft + screenWidth - width);
        var top = Math.Clamp(Top, screenTop, screenTop + screenHeight - height);
        return this with { Left = left, Top = top, Width = width, Height = height };
    }
}
