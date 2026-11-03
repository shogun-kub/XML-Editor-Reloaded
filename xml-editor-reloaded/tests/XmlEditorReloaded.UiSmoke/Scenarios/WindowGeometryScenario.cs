using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S82: the window persists its size and position, restores a maximized state with the same restore
/// bounds, and opens in Normal state after the application was closed while minimized.
/// </summary>
internal sealed class WindowGeometryScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S82";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var placement = ConfigureNormal(context);
        placement = ConfigureMaximized(context, placement);
        placement = ConfigureFinalNormal(context);
        VerifyMinimizedClose(context, placement);
    }

    private static WindowPlacementModel ConfigureNormal(ScenarioContext context)
    {
        var window = context.Window;
        WindowUi.SetBounds(context.App, 140, 110, 1200, 700);
        var bounds = WaitForSize(window, 1200, 700);
        context.Recorder.Check(
            "Окно изменяет размер и положение",
            WindowUi.NearlyEqual(bounds, new System.Windows.Rect(140, 110, 1200, 700), 20),
            WindowUi.Describe(bounds));

        CloseAndReadPlacement(context);
        var placement = WindowUi.ReadPlacement(context.App);
        context.Recorder.Check(
            "settings.json содержит windowPlacement",
            placement is not null,
            "windowPlacement отсутствует");
        var saved = placement ?? throw new UiTimeoutException("windowPlacement отсутствует.");
        context.Recorder.Check(
            "Размер и позиция сохранены",
            Math.Abs(saved.Left - bounds.Left) <= 16
                && Math.Abs(saved.Top - bounds.Top) <= 16
                && Math.Abs(saved.Width - bounds.Width) <= 16
                && Math.Abs(saved.Height - bounds.Height) <= 16,
            $"bounds={WindowUi.Describe(bounds)}; placement="
                + $"{saved.Left:0},{saved.Top:0} {saved.Width:0}x{saved.Height:0}");
        context.Recorder.Check(
            "Обычное окно сохранено не maximized",
            !saved.IsMaximized,
            $"IsMaximized={saved.IsMaximized}");

        RestartWithPlacement(context, saved);
        window = context.Window;
        var restored = WaitForSize(window, saved.Width, saved.Height);
        context.Recorder.Check(
            "Размер и позиция восстановлены после перезапуска",
            WindowUi.NearlyEqual(restored, bounds, 20)
                && WindowUi.State(window) == WindowVisualState.Normal,
            $"restored={WindowUi.Describe(restored)}; state={WindowUi.State(window)}");
        return saved;
    }

    private static WindowPlacementModel ConfigureMaximized(
        ScenarioContext context,
        WindowPlacementModel normal)
    {
        var window = context.Window;
        WindowUi.SetState(window, WindowVisualState.Maximized);
        WindowUi.WaitForState(window, WindowVisualState.Maximized, "окно развернулось");
        context.Recorder.Check("Окно разворачивается", true, null);

        CloseAndReadPlacement(context);
        var saved = WindowUi.ReadPlacement(context.App)
            ?? throw new UiTimeoutException("windowPlacement после maximized отсутствует.");
        context.Recorder.Check(
            "Maximized сохранён",
            saved.IsMaximized,
            $"IsMaximized={saved.IsMaximized}");
        context.Recorder.Check(
            "Restore-bounds совпадают с прежней нормальной геометрией",
            Math.Abs(saved.Left - normal.Left) <= 16
                && Math.Abs(saved.Top - normal.Top) <= 16
                && Math.Abs(saved.Width - normal.Width) <= 16
                && Math.Abs(saved.Height - normal.Height) <= 16,
            $"placement={saved.Left:0},{saved.Top:0} {saved.Width:0}x{saved.Height:0}; "
                + $"normal={normal.Left:0},{normal.Top:0} {normal.Width:0}x{normal.Height:0}");

        RestartWithPlacement(context, saved);
        window = context.Window;
        WindowUi.WaitForState(window, WindowVisualState.Maximized, "окно восстановилось maximized");
        context.Recorder.Check(
            "После перезапуска окно maximized",
            WindowUi.State(window) == WindowVisualState.Maximized,
            WindowUi.State(window).ToString());
        WindowUi.SetState(window, WindowVisualState.Normal);
        WindowUi.WaitForState(window, WindowVisualState.Normal, "окно вернулось в Normal");
        context.Recorder.Check("Окно возвращается в Normal", true, null);
        return saved;
    }

    private static WindowPlacementModel ConfigureFinalNormal(ScenarioContext context)
    {
        var window = context.Window;
        WindowUi.SetBounds(context.App, 200, 150, 1150, 650);
        var bounds = WaitForSize(window, 1150, 650);
        context.Recorder.Check(
            "Окно снова меняет нормальную геометрию",
            WindowUi.NearlyEqual(bounds, new System.Windows.Rect(200, 150, 1150, 650), 20),
            WindowUi.Describe(bounds));

        CloseAndReadPlacement(context);
        var saved = WindowUi.ReadPlacement(context.App)
            ?? throw new UiTimeoutException("windowPlacement после нормального закрытия отсутствует.");
        context.Recorder.Check(
            "Новая нормальная геометрия сохранена",
            !saved.IsMaximized
                && Math.Abs(saved.Left - bounds.Left) <= 16
                && Math.Abs(saved.Top - bounds.Top) <= 16
                && Math.Abs(saved.Width - bounds.Width) <= 16
                && Math.Abs(saved.Height - bounds.Height) <= 16,
            $"placement={saved.Left:0},{saved.Top:0} {saved.Width:0}x{saved.Height:0}; "
                + $"bounds={WindowUi.Describe(bounds)}");

        RestartWithPlacement(context, saved);
        context.Recorder.Check(
            "Нормальная геометрия восстанавливается",
            WindowUi.State(context.Window) == WindowVisualState.Normal,
            WindowUi.State(context.Window).ToString());
        return saved;
    }

    private static void VerifyMinimizedClose(
        ScenarioContext context,
        WindowPlacementModel saved)
    {
        var window = context.Window;
        var bounds = WaitForSize(window, saved.Width, saved.Height);
        context.Recorder.Check(
            "Окно открылось с последней нормальной геометрией",
            WindowUi.NearlyEqual(bounds, new System.Windows.Rect(
                saved.Left,
                saved.Top,
                saved.Width,
                saved.Height), 20),
            WindowUi.Describe(bounds));

        WindowUi.SetState(window, WindowVisualState.Minimized);
        WindowUi.WaitForState(window, WindowVisualState.Minimized, "окно свернулось");
        CloseAndReadPlacement(context);
        var placement = WindowUi.ReadPlacement(context.App)
            ?? throw new UiTimeoutException("windowPlacement после свёрнутого закрытия отсутствует.");
        context.Recorder.Check(
            "Свёрнутое окно не записывает minimized в настройки",
            !placement.IsMaximized
                && Math.Abs(placement.Left - saved.Left) <= 2
                && Math.Abs(placement.Top - saved.Top) <= 2
                && Math.Abs(placement.Width - saved.Width) <= 2
                && Math.Abs(placement.Height - saved.Height) <= 2,
            $"placement={placement.Left:0},{placement.Top:0} {placement.Width:0}x{placement.Height:0}");

        RestartWithPlacement(context, placement);
        window = context.Window;
        context.Recorder.Check(
            "После закрытия свёрнутым окно открывается в Normal",
            WindowUi.State(window) == WindowVisualState.Normal,
            WindowUi.State(window).ToString());
        var reopened = WaitForSize(window, saved.Width, saved.Height);
        context.Recorder.Check(
            "Последняя сохранённая геометрия не потеряна",
            WindowUi.NearlyEqual(reopened, new System.Windows.Rect(
                saved.Left,
                saved.Top,
                saved.Width,
                saved.Height), 20),
            WindowUi.Describe(reopened));
    }

    private static void RestartWithPlacement(
        ScenarioContext context,
        WindowPlacementModel placement)
    {
        var settings = SettingsUi.ReadSettings(context.App) ?? new SettingsModel();
        settings.WindowPlacement = placement;
        context.RestartApp(settings);
    }

    private static System.Windows.Rect WaitForSize(
        AutomationElement window,
        double width,
        double height)
    {
        Ui.WaitUntil(
            () =>
            {
                var bounds = WindowUi.Bounds(window);
                return Math.Abs(bounds.Width - width) <= 20 && Math.Abs(bounds.Height - height) <= 20;
            },
            $"окно размером {width:0}x{height:0}",
            TimeSpan.FromSeconds(20));
        return WindowUi.Bounds(window);
    }

    private static void CloseAndReadPlacement(ScenarioContext context)
    {
        context.App.RequestClose();
        Ui.WaitUntil(
            () => context.App.HasExited,
            "приложение закрылось для сохранения геометрии",
            TimeSpan.FromSeconds(40));
    }
}
