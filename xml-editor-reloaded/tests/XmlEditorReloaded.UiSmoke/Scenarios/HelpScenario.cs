using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S105: F1 opens modeless context help for the active table and cell, the field description is
/// available, search finds a field, related help navigates to another table, and the window closes
/// without disturbing the main window.
/// </summary>
internal sealed class HelpScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S105";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        ScenarioUi.OpenTable(context, "Items.Weapons");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "таблица Items.Weapons открыта");

        var grid = window.WaitById("RowsGrid");
        var headers = ScenarioUi.ColumnHeaderCaptions(window);
        var column = headers.IndexOf("uiIndex");
        context.Recorder.Check(
            "В сетке есть колонка uiIndex",
            column >= 0,
            string.Join(", ", headers));

        var header = ScenarioUi.WaitForColumnHeader(window, "uiIndex");
        var headerText = header.FindFirst(
            TreeScope.Descendants,
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
        var tooltip = headerText?.Current.HelpText ?? string.Empty;
        context.Recorder.Check(
            "Тултип колонки берётся из справки",
            tooltip.Contains("Index of the weapon row", StringComparison.Ordinal),
            tooltip);

        var cell = grid.Cell(0, column);
        cell.Click();
        Thread.Sleep(100);
        cell.SetFocus();
        Thread.Sleep(250);
        window.FindById("TitleBar")?.Click();
        Thread.Sleep(200);
        NativeInput.PressWindowMessage(window, NativeInput.VirtualKey.F1);
        var help = Ui.WaitFor(
            () => TryFindHelpWindow(context.App),
            "окно контекстной справки",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check("F1 открывает окно справки", true, help.Current.Name);
        context.Recorder.Check(
            "Справка открыта на активной таблице",
            help.WaitById("HelpTitleText").ReadName().Contains("Items.Weapons", StringComparison.Ordinal),
            help.WaitById("HelpTitleText").ReadName());
        context.Recorder.Check(
            "Выбрано текущее поле uiIndex",
            help.WaitById("HelpFieldPathText").ReadName() == "WEAPONLIST/WEAPON/uiIndex",
            help.WaitById("HelpFieldPathText").ReadName());
        context.Recorder.Check(
            "Описание поля показано",
            help.WaitById("HelpFieldDescriptionText").ReadName().Length > 0,
            help.WaitById("HelpFieldDescriptionText").ReadName());

        help.WaitById("HelpSearchBox").SetText("bRecoilX");
        help.WaitById("HelpSearchButton").InvokeButton();
        Ui.WaitUntil(
            () => help.FindById("HelpSearchResults") is not null,
            "результаты поиска по справке");
        context.Recorder.Check(
            "Поиск по справке нашёл поле Recoil",
            help.WaitById("HelpSearchResults").FindAll(
                    TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
                .Count > 0,
            "список результатов пуст");

        help.WaitById("HelpOpenSearchResultButton").InvokeButton();
        Ui.WaitUntil(
            () => help.WaitById("HelpFieldPathText").ReadName().Contains("Recoil", StringComparison.Ordinal),
            "выбрано поле с Recoil");
        context.Recorder.Check(
            "Результат поиска открывает поле",
            true,
            help.WaitById("HelpFieldPathText").ReadName());

        help.WaitById("AmmoStrings").InvokeButton();
        Ui.WaitUntil(
            () => help.WaitById("HelpDocumentPathText").ReadName().Contains("AmmoStrings.md", StringComparison.Ordinal),
            "связанная справка AmmoStrings");
        context.Recorder.Check(
            "Связанная справка открывает другую таблицу",
            help.WaitById("HelpTitleText").ReadName().Contains("AmmoStrings", StringComparison.Ordinal),
            help.WaitById("HelpTitleText").ReadName());

        help.WaitById("HelpCloseButton").InvokeButton();
        Ui.WaitUntil(
            () => TryFindHelpWindow(context.App) is null,
            "окно справки закрыто");
        context.Recorder.Check("Справка закрывается, приложение остаётся", !context.App.HasExited, null);
    }

    private static AutomationElement? TryFindHelpWindow(UiApp app) =>
        AutomationElement.RootElement
            .FindAll(
                TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "HelpWindow")))
            .Cast<AutomationElement>()
            .FirstOrDefault();
}
