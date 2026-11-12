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
            tooltip.Contains("Индекс строки оружия", StringComparison.Ordinal),
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
        var resultCount = help.WaitById("HelpSearchResults").FindAll(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
            .Count;
        context.Recorder.Check(
            "Поиск по справке нашёл поле Recoil",
            resultCount > 0,
            $"результатов: {resultCount}");
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

        help.WaitById("HelpOpenTableButton").InvokeButton();
        Ui.WaitUntil(
            () => window.WaitById("TableTitle").ReadName().Contains("AmmoStrings", StringComparison.Ordinal),
            "таблица AmmoStrings открыта в главном окне");
        context.Recorder.Check(
            "Кнопка открывает таблицу в главном окне",
            window.WaitById("TableTitle").ReadName().Contains("AmmoStrings", StringComparison.Ordinal),
            window.WaitById("TableTitle").ReadName());

        help.WaitById("HelpCloseButton").InvokeButton();
        Ui.WaitUntil(
            () => TryFindHelpWindow(context.App) is null,
            "окно справки закрыто");
        context.Recorder.Check("Справка закрывается, приложение остаётся", !context.App.HasExited, null);

        ScenarioUi.OpenMenuItemDetached(context, "HelpMenu", "GlossaryMenuItem");
        var glossary = Ui.WaitFor(
            () => TryFindDocumentWindow(context.App, "HelpDocumentWindow"),
            "окно глоссария",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check(
            "Меню открывает глоссарий",
            glossary.WaitById("HelpDocumentTitleText").ReadName() == "Глоссарий"
                && glossary.WaitById("HelpDocumentText").ReadValue().Contains("поле", StringComparison.Ordinal),
            glossary.WaitById("HelpDocumentTitleText").ReadName());
        glossary.WaitById("HelpDocumentCloseButton").InvokeButton();
        Ui.WaitUntil(
            () => TryFindDocumentWindow(context.App, "HelpDocumentWindow") is null,
            "окно глоссария закрыто");

        ScenarioUi.OpenMenuItemDetached(context, "HelpMenu", "IndexMenuItem");
        var index = Ui.WaitFor(
            () => TryFindDocumentWindow(context.App, "HelpDocumentWindow"),
            "окно индекса",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check(
            "Меню открывает индекс",
            index.WaitById("HelpDocumentTitleText").ReadName() == "Индекс справки по таблицам"
                && index.WaitById("HelpDocumentText").ReadValue().Contains("AmmoStrings", StringComparison.Ordinal),
            index.WaitById("HelpDocumentTitleText").ReadName());
        index.WaitById("HelpDocumentCloseButton").InvokeButton();
        Ui.WaitUntil(
            () => TryFindDocumentWindow(context.App, "HelpDocumentWindow") is null,
            "окно индекса закрыто");
    }

    private static AutomationElement? TryFindDocumentWindow(UiApp app, string automationId) =>
        AutomationElement.RootElement
            .FindAll(
                TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, automationId)))
            .Cast<AutomationElement>()
            .FirstOrDefault();

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
