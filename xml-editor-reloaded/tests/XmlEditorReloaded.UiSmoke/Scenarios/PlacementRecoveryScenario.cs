using System.IO;
using System.Text.Json;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S83: the main window recovers from unusable or off-screen saved placements, malformed settings do
/// not prevent start-up, and a read-only settings file does not block closing the application.
/// Screenshots are captured after every restart as visual evidence.
/// </summary>
internal sealed class PlacementRecoveryScenario : SmokeScenario
{
    private const string NormalPlacement =
        """{ "left": 120, "top": 90, "width": 1280, "height": 780, "isMaximized": false }""";

    private const string OffScreenPlacement =
        """{ "left": -32000, "top": -32000, "width": 1280, "height": 780, "isMaximized": false }""";

    private const string ZeroSizePlacement =
        """{ "left": 100, "top": 100, "width": 0, "height": 0, "isMaximized": false }""";

    private const string IncompletePlacement =
        """{ "left": 100, "top": 100, "width": 1280 }""";

    private const string NonNumericPlacement =
        """{ "left": 100, "top": 100, "width": "NaN", "height": 780 }""";

    /// <inheritdoc />
    public override string Id => "S83";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        Screenshots.EnableDpiAwareness();
        var screen = NativeInput.VirtualScreenBounds();
        context.Recorder.Check(
            "Виртуальный экран определён",
            screen.Width >= 800 && screen.Height >= 600,
            WindowUi.Describe(screen));

        Capture(context, "S83-00-start.png");
        CheckInside(context, screen, "стартовое окно");

        context.RestartAppWithJson(BuildSettings(context, OffScreenPlacement));
        Capture(context, "S83-01-offscreen.png");
        CheckInside(context, screen, "окно с координатами −32000");
        var offScreen = NativeInput.WindowBounds(context.App.WindowHandle);
        context.Recorder.Check(
            "Сохранённые координаты не восстановлены буквально",
            offScreen.Left > -30000 && offScreen.Top > -30000,
            WindowUi.Describe(offScreen));

        context.RestartAppWithJson(BuildSettings(context, ZeroSizePlacement));
        Capture(context, "S83-02-zero-size.png");
        CheckUsableDefaultSize(context, screen, "нулевая ширина и высота");
        CheckInside(context, screen, "окно вместо нулевого размера");

        context.RestartAppWithJson(BuildSettings(context, IncompletePlacement));
        Capture(context, "S83-03-incomplete.png");
        CheckInside(context, screen, "неполный windowPlacement");
        CheckUsableDefaultSize(context, screen, "неполный windowPlacement");

        context.RestartAppWithJson(BuildSettings(context, NonNumericPlacement));
        Capture(context, "S83-04-non-numeric.png");
        context.Recorder.Check(
            "Некорректный settings.json не мешает запуску",
            !context.App.HasExited,
            context.App.HasExited ? "приложение завершилось" : null);
        CheckInside(context, screen, "нечисловая ширина");
        CheckUsableDefaultSize(context, screen, "нечисловая ширина");

        VerifyReadOnlySettings(context);
    }

    private static void VerifyReadOnlySettings(ScenarioContext context)
    {
        context.RestartAppWithJson(BuildSettings(context, NormalPlacement));
        var settingsPath = Path.Combine(context.App.AppDataDirectory, "settings.json");
        File.SetAttributes(settingsPath, FileAttributes.ReadOnly);
        try
        {
            var before = File.ReadAllText(settingsPath);
            WindowUi.SetBounds(context.App, 160, 120, 1200, 700);
            Thread.Sleep(300);
            Capture(context, "S83-05-readonly.png");

            var closed = context.App.Close(TimeSpan.FromSeconds(25));
            context.Recorder.Check(
                "Ошибка записи настроек не мешает закрытию",
                closed,
                closed ? null : "приложение не завершилось и было снято принудительно");
            context.Recorder.Check(
                "Процесс завершился после закрытия",
                context.App.HasExited,
                context.App.HasExited ? null : "процесс ещё работает");

            var after = File.ReadAllText(settingsPath);
            context.Recorder.Check(
                "Настройки не перезаписаны при read-only",
                string.Equals(before, after, StringComparison.Ordinal),
                string.Equals(before, after, StringComparison.Ordinal)
                    ? null
                    : "файл настроек изменился");
        }
        finally
        {
            File.SetAttributes(settingsPath, FileAttributes.Normal);
        }
    }

    private static void CheckUsableDefaultSize(
        ScenarioContext context,
        System.Windows.Rect screen,
        string description)
    {
        var bounds = NativeInput.WindowBounds(context.App.WindowHandle);
        context.Recorder.Check(
            $"{description}: окно получило обычный размер",
            bounds.Width >= 1000
                && bounds.Height >= 500
                && bounds.Width <= screen.Width + 2
                && bounds.Height <= screen.Height + 2,
            WindowUi.Describe(bounds));
    }

    private static void CheckInside(
        ScenarioContext context,
        System.Windows.Rect screen,
        string description)
    {
        var bounds = NativeInput.WindowBounds(context.App.WindowHandle);
        const double tolerance = 2;
        var inside = bounds.Width > 0
            && bounds.Height > 0
            && bounds.Left >= screen.Left - tolerance
            && bounds.Top >= screen.Top - tolerance
            && bounds.Right <= screen.Right + tolerance
            && bounds.Bottom <= screen.Bottom + tolerance;
        context.Recorder.Check(
            $"{description}: окно в пределах виртуального экрана",
            inside,
            $"{WindowUi.Describe(bounds)}; экран {WindowUi.Describe(screen)}");
    }

    private static void Capture(ScenarioContext context, string fileName)
    {
        var path = Path.Combine(context.Directory, fileName);
        Screenshots.Capture(context.App, path);
        context.Recorder.Note($"Скриншот: {path}");
    }

    private static string BuildSettings(ScenarioContext context, string placementJson)
    {
        var gameRoot = JsonSerializer.Serialize(context.Data(DatasetKind.V4).GameRoot);
        return $$"""
        {
          "dataDirectories": [{{gameRoot}}],
          "activeDataDirectory": {{gameRoot}},
          "activeLanguageCode": null,
          "uiLanguageCode": "ru",
          "recentDataDirectories": [{{gameRoot}}],
          "windowPlacement": {{placementJson}}
        }
        """;
    }
}
