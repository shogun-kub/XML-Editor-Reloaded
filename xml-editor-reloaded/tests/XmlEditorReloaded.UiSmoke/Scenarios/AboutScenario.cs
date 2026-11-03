using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S81: the About window shows the assembly product, version, and description, is centred on its owner,
/// is modal, closes with OK, Enter, and Escape, and is not duplicated by repeated opening. The same
/// content is shown after switching the interface to English.
/// </summary>
internal sealed class AboutScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S81";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var main = context.Window;
        var russian = context.Strings;
        OpenAbout(context);
        var found = FindAbout(context.App);
        context.Recorder.Check(
            "Окно About открылось",
            found is not null,
            null);
        var about = found ?? throw new UiTimeoutException("Окно About не найдено.");

        context.Recorder.Check(
            "Заголовок About на русском",
            about.ReadName() == russian.Format("AboutTitleFormat", "XML Editor Reloaded"),
            about.ReadName());
        context.Recorder.Check(
            "Продукт в About",
            about.WaitById("AboutProductText").ReadName() == "XML Editor Reloaded",
            about.WaitById("AboutProductText").ReadName());
        context.Recorder.Check(
            "Версия в About",
            about.WaitById("AboutVersionText").ReadName() == "0.1.0",
            about.WaitById("AboutVersionText").ReadName());
        context.Recorder.Check(
            "Описание сборки в About",
            about.WaitById("AboutDescriptionText").ReadName().Length > 0,
            about.WaitById("AboutDescriptionText").ReadName());
        context.Recorder.Check(
            "Окно About модально",
            WindowUi.IsModal(about),
            $"IsModal={WindowUi.IsModal(about)}");
        context.Recorder.Check(
            "Открыто ровно одно окно About",
            AboutWindowCount(context.App) == 1,
            $"окон: {AboutWindowCount(context.App)}");

        var mainBounds = WindowUi.Bounds(main);
        var aboutBounds = WindowUi.Bounds(about);
        var centerDelta = Math.Abs(
            (aboutBounds.Left + (aboutBounds.Width / 2)) - (mainBounds.Left + (mainBounds.Width / 2)));
        var centerDeltaY = Math.Abs(
            (aboutBounds.Top + (aboutBounds.Height / 2)) - (mainBounds.Top + (mainBounds.Height / 2)));
        context.Recorder.Check(
            "Окно About центрировано по владельцу",
            centerDelta <= 40 && centerDeltaY <= 60,
            $"main={WindowUi.Describe(mainBounds)}; about={WindowUi.Describe(aboutBounds)}; "
                + $"dx={centerDelta:0}, dy={centerDeltaY:0}");
        context.Recorder.Note(
            "ShowInTaskbar=False и CenterOwner заданы в AboutWindow.xaml; UIA не наблюдает кнопку панели задач.");

        about.WaitById("AboutOkButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "AboutOkButton");
        context.Recorder.Check("OK закрывает About и приложение остаётся", !context.App.HasExited, null);

        OpenAbout(context);
        about = FindAbout(context.App)!;
        NativeInput.PressIn(about, NativeInput.VirtualKey.Return);
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "AboutOkButton");
        context.Recorder.Check("Enter закрывает About", !context.App.HasExited, null);

        OpenAbout(context);
        about = FindAbout(context.App)!;
        NativeInput.PressIn(about, NativeInput.VirtualKey.Escape);
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "AboutOkButton");
        context.Recorder.Check("Esc закрывает About", !context.App.HasExited, null);

        OpenAbout(context);
        var second = FindAbout(context.App)!;
        context.Recorder.Check(
            "Повторное открытие не создаёт второе окно About",
            AboutWindowCount(context.App) == 1,
            $"окон: {AboutWindowCount(context.App)}");
        second.WaitById("AboutOkButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "AboutOkButton");

        var english = UiStrings.Load(ResourcesDirectory(context), "en");
        var settings = SettingsUi.ReadSettings(context.App) ?? new SettingsModel();
        settings.UiLanguageCode = "en";
        context.RestartApp(settings);
        Ui.WaitUntil(
            () => context.Window.WaitById("SaveAllButton").ReadName() == english.Get("EditorSaveAllText"),
            "интерфейс на английском после перезапуска",
            TimeSpan.FromSeconds(60));

        OpenAbout(context);
        about = FindAbout(context.App)!;
        context.Recorder.Check(
            "Заголовок About на английском",
            about.ReadName() == english.Format("AboutTitleFormat", "XML Editor Reloaded"),
            about.ReadName());
        context.Recorder.Check(
            "Продукт и описание на английском",
            about.WaitById("AboutProductText").ReadName() == "XML Editor Reloaded"
                && about.WaitById("AboutDescriptionText").ReadName().Length > 0,
            about.WaitById("AboutDescriptionText").ReadName());
        about.WaitById("AboutOkButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "AboutOkButton");
        context.Recorder.Check("Английское окно About закрывается", !context.App.HasExited, null);
    }

    private static void OpenAbout(ScenarioContext context)
    {
        ScenarioUi.OpenMenuItemDetached(context, "HelpMenu", "AboutMenuItem");
        _ = Ui.WaitFor(
            () => FindAbout(context.App),
            "окно About",
            TimeSpan.FromSeconds(30));
    }

    private static AutomationElement? FindAbout(UiApp app) =>
        AutomationElement.RootElement
            .FindAll(
                TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "AboutWindow")))
            .Cast<AutomationElement>()
            .FirstOrDefault();

    private static int AboutWindowCount(UiApp app) =>
        AutomationElement.RootElement
            .FindAll(
                TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "AboutWindow")))
            .Count;

    private static string ResourcesDirectory(ScenarioContext context) =>
        System.IO.Path.Combine(
            context.Options.RepositoryRoot,
            "xml-editor-reloaded",
            "src",
            "XmlEditorReloaded.App",
            "Resources");
}
