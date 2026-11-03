using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Window geometry, state, and restart helpers shared by the shell scenarios. Geometry is read from UI
/// Automation and changed through the native window handle so the values the application persists are
/// exactly the values the test later verifies.
/// </summary>
internal static class WindowUi
{
    /// <summary>
    /// Returns the screen rectangle of a top-level window.
    /// </summary>
    /// <param name="window">Top-level window.</param>
    /// <returns>Screen bounds in physical pixels.</returns>
    public static System.Windows.Rect Bounds(AutomationElement window) => window.Current.BoundingRectangle;

    /// <summary>
    /// Returns the visual state of a top-level window.
    /// </summary>
    /// <param name="window">Top-level window.</param>
    /// <returns>Normal, maximized, or minimized.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the window does not expose the window pattern.</exception>
    public static WindowVisualState State(AutomationElement window)
    {
        if (window.TryGetCurrentPattern(WindowPattern.Pattern, out var raw)
            && raw is WindowPattern pattern)
        {
            return pattern.Current.WindowVisualState;
        }

        throw new UiTimeoutException("Окно не предоставляет WindowPattern.");
    }

    /// <summary>
    /// Returns whether a top-level window reports itself as modal.
    /// </summary>
    /// <param name="window">Top-level window.</param>
    /// <returns><see langword="true"/> for a modal window.</returns>
    public static bool IsModal(AutomationElement window) =>
        window.TryGetCurrentPattern(WindowPattern.Pattern, out var raw)
        && raw is WindowPattern pattern
        && pattern.Current.IsModal;

    /// <summary>
    /// Changes the visual state of a top-level window.
    /// </summary>
    /// <param name="window">Top-level window.</param>
    /// <param name="state">Requested state.</param>
    /// <exception cref="UiTimeoutException">Thrown when the window does not expose the window pattern.</exception>
    public static void SetState(AutomationElement window, WindowVisualState state)
    {
        if (!window.TryGetCurrentPattern(WindowPattern.Pattern, out var raw)
            || raw is not WindowPattern pattern)
        {
            throw new UiTimeoutException("Окно не предоставляет WindowPattern.");
        }

        pattern.SetWindowVisualState(state);
        Thread.Sleep(300);
    }

    /// <summary>
    /// Moves and resizes a top-level window through its native handle.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="left">Left edge.</param>
    /// <param name="top">Top edge.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    public static void SetBounds(UiApp app, int left, int top, int width, int height) =>
        NativeInput.SetWindowBounds(app.WindowHandle, left, top, width, height);

    /// <summary>
    /// Waits until the window reports the requested visual state.
    /// </summary>
    /// <param name="window">Top-level window.</param>
    /// <param name="state">Expected state.</param>
    /// <param name="description">Description used in the timeout message.</param>
    /// <param name="timeout">Optional timeout.</param>
    public static void WaitForState(
        AutomationElement window,
        WindowVisualState state,
        string description,
        TimeSpan? timeout = null) =>
        Ui.WaitUntil(() => State(window) == state, description, timeout);

    /// <summary>
    /// Determines whether two rectangles agree within a tolerance that covers the non-client border and
    /// DPI rounding.
    /// </summary>
    /// <param name="left">First rectangle.</param>
    /// <param name="right">Second rectangle.</param>
    /// <param name="tolerance">Allowed difference per edge in pixels.</param>
    /// <returns><see langword="true"/> when every edge is within the tolerance.</returns>
    public static bool NearlyEqual(System.Windows.Rect left, System.Windows.Rect right, double tolerance = 12) =>
        Math.Abs(left.Left - right.Left) <= tolerance
        && Math.Abs(left.Top - right.Top) <= tolerance
        && Math.Abs(left.Width - right.Width) <= tolerance
        && Math.Abs(left.Height - right.Height) <= tolerance;

    /// <summary>
    /// Formats a rectangle for failure details.
    /// </summary>
    /// <param name="rectangle">Rectangle to format.</param>
    /// <returns>Compact text with integer coordinates.</returns>
    public static string Describe(System.Windows.Rect rectangle) =>
        $"{rectangle.Left:0},{rectangle.Top:0} {rectangle.Width:0}x{rectangle.Height:0}";

    /// <summary>
    /// Reads the persisted window placement from the isolated settings file.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <returns>The saved placement, or <see langword="null"/> when it is missing.</returns>
    public static WindowPlacementModel? ReadPlacement(UiApp app) =>
        SettingsUi.ReadSettings(app)?.WindowPlacement;
}
