using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S57: editing a key of an <c>IncompatibleAttachments</c> row moves the reversed row, and adding and
/// deleting a complete row keeps the two symmetric rows in step.
/// </summary>
internal sealed class SymmetricPairScenario : SmokeScenario
{
    private const string TablePrefix = "ItemSection.Items_IncompatibleAttachments";
    private const string TargetColumn = "incompatibleattachmentIndex";

    /// <inheritdoc />
    public override string Id => "S57";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Items/IncompatibleAttachments.xml");
        var beforeCount = Pairs(file).Count;
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "948", "Aimpoint");
        var childTables = context.Strings.Get("ItemTabChildTables");
        WorkspaceUi.SelectChildSection(window, childTables, context.Strings.Get(TablePrefix));
        var grid = window.WaitById("RowsGrid");
        Ui.WaitUntil(
            () => grid.RowCount() == 17,
            "у item 948 семнадцать несовместимых вложений",
            TimeSpan.FromSeconds(30));

        var headers = WorkspaceUi.VisualHeaders(grid).ToList();
        var targetColumn = headers.IndexOf(TargetColumn);
        context.Recorder.Check(
            "Колонка второго ключа видна",
            targetColumn >= 0,
            string.Join(" | ", headers));
        var row208 = Enumerable.Range(0, grid.RowCount())
            .First(cellRow => WorkspaceUi.ReadCell(grid, cellRow, targetColumn) == "Sniper Scope, 10x");
        context.Recorder.Check(
            "Строка 948→208 найдена",
            row208 >= 0,
            $"строка: {row208}");

        EditTarget(context, grid, row208, targetColumn, "Bipod", "Bipod (209)");
        Ui.WaitUntil(
            () => WorkspaceUi.ReadCell(grid, row208, targetColumn) == "Bipod",
            "второй ключ изменён на 209",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "В гриде строка показывает Bipod",
            WorkspaceUi.ReadCell(grid, row208, targetColumn) == "Bipod",
            WorkspaceUi.ReadCell(grid, row208, targetColumn));

        WorkspaceUi.SaveItem(context);
        context.ExpectChange(DatasetKind.V4, "TableData/Items/IncompatibleAttachments.xml");
        var changed = Pairs(file);
        context.Recorder.Check(
            "Смена ключа удалила старую обратную строку",
            !changed.Contains(("948", "208")) && !changed.Contains(("208", "948")),
            $"всего: {changed.Count}");
        context.Recorder.Check(
            "Смена ключа создала новую обратную строку 209↔948",
            changed.Contains(("948", "209")) && changed.Contains(("209", "948")),
            string.Join(" | ", changed.Where(static pair => pair.Item1 == "948" || pair.Item2 == "948")));

        AddRow(context);
        Ui.WaitUntil(
            () => grid.RowCount() == 18,
            "добавлена строка 948→?",
            TimeSpan.FromSeconds(20));
        var addedRow = grid.RowCount() - 1;
        EditTarget(context, grid, addedRow, targetColumn, "Night Vision", "Night Vision Goggles, Gen. I (211)");
        Ui.WaitUntil(
            () => WorkspaceUi.ReadCell(grid, addedRow, targetColumn) == "Night Vision Goggles, Gen. I",
            "новый ключ 211 записан",
            TimeSpan.FromSeconds(20));
        WorkspaceUi.SaveItem(context);
        var added = Pairs(file);
        context.Recorder.Check(
            "Добавление строки создало обратную 211↔948",
            added.Contains(("948", "211")) && added.Contains(("211", "948")),
            string.Join(" | ", added.Where(static pair => pair.Item1 == "948" || pair.Item2 == "948")));
        context.Recorder.Check(
            "Добавление увеличило файл на две строки",
            added.Count == beforeCount + 2,
            $"{beforeCount} → {added.Count}");

        DeleteRow(context, grid, addedRow, targetColumn);
        Ui.WaitUntil(
            () => grid.RowCount() == 17,
            "добавленная строка удалена",
            TimeSpan.FromSeconds(20));
        WorkspaceUi.SaveItem(context);
        var deleted = Pairs(file);
        context.Recorder.Check(
            "Удаление убрало обе строки 211↔948",
            !deleted.Contains(("948", "211")) && !deleted.Contains(("211", "948")),
            string.Join(" | ", deleted.Where(static pair => pair.Item1 == "948" || pair.Item2 == "948")));
        context.Recorder.Check(
            "После удаления снова исходное число строк",
            deleted.Count == beforeCount,
            $"{beforeCount} → {deleted.Count}");
        context.Recorder.Check(
            "Смена ключа 209↔948 сохранилась после add/delete",
            deleted.Contains(("948", "209")) && deleted.Contains(("209", "948")),
            string.Join(" | ", deleted.Where(static pair => pair.Item1 == "948" || pair.Item2 == "948")));
    }

    private static void EditTarget(
        ScenarioContext context,
        AutomationElement grid,
        int row,
        int column,
        string search,
        string optionText)
    {
        var cell = grid.Cell(row, column);
        cell.Click();
        Thread.Sleep(150);
        cell.SetFocus();
        Thread.Sleep(120);
        NativeInput.PressIn(grid, NativeInput.VirtualKey.F2);
        Thread.Sleep(250);
        var picker = context.Window.WaitById(TargetColumn);
        picker.SetFocus();
        NativeInput.TypeTextIn(picker, search);
        Thread.Sleep(400);
        var option = Ui.WaitFor(
            () => AutomationElement.RootElement
                .FindAll(
                    TreeScope.Descendants,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, context.App.ProcessId),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem)))
                .Cast<AutomationElement>()
                .FirstOrDefault(item => Ui.ItemText(item).Trim() == optionText),
            $"подсказка «{optionText}»",
            TimeSpan.FromSeconds(10));
        if (option is null)
        {
            throw new UiTimeoutException($"В picker нет «{optionText}».");
        }

        NativeInput.PressIn(picker, NativeInput.VirtualKey.Return);
        Thread.Sleep(200);
        NativeInput.PressIn(picker, NativeInput.VirtualKey.Tab);
        Thread.Sleep(300);
    }

    private static void AddRow(ScenarioContext context)
    {
        var add = context.Window.WaitById("ItemTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))
            .FirstOrDefault(button => button.ReadName() == context.Strings.Get("EditorAddRowText"))
            ?? throw new UiTimeoutException("Кнопка «Добавить запись» не найдена.");
        add.InvokeButton();
        Thread.Sleep(400);
    }

    private static void DeleteRow(
        ScenarioContext context,
        AutomationElement grid,
        int row,
        int column)
    {
        var cell = grid.Cell(row, column);
        cell.Click();
        Thread.Sleep(150);
        cell.SetFocus();
        Thread.Sleep(120);
        var delete = context.Window.WaitById("ItemTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))
            .FirstOrDefault(button => button.ReadName() == context.Strings.Get("EditorDeleteRowText"))
            ?? throw new UiTimeoutException("Кнопка «Удалить» не найдена.");
        delete.InvokeButton();
        var dialog = ScenarioUi.WaitForMessageBox(context.App);
        ScenarioUi.AnswerMessageBox(dialog, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
    }

    private static IReadOnlyList<(string Item, string Incompatible)> Pairs(string path) =>
        XDocument.Load(path).Root!.Elements("INCOMPATIBLEATTACHMENT")
            .Select(static element => (
                element.Element("itemIndex")?.Value.Trim() ?? string.Empty,
                element.Element("incompatibleattachmentIndex")?.Value.Trim() ?? string.Empty))
            .ToList();
}
