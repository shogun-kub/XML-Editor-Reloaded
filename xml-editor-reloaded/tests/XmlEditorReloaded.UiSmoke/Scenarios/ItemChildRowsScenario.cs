using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S20: an added attachment row of item 12 presets the item reference, the relation picker offers only
/// attachment items, the chosen key is written, and delete plus undo restore the row count.
/// </summary>
internal sealed class ItemChildRowsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S20";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        const string AttachmentName = "Gun Barrel Extender";
        const string AttachmentOption = "Gun Barrel Extender (310)";

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        context.Recorder.Check(
            "На вкладке «Поля» показана подсказка о фильтре вложений",
            window.FindByName(context.Strings.Get("ItemCompatibleOnly")) is not null,
            context.Strings.Get("ItemCompatibleOnly"));

        var childTables = context.Strings.Get("ItemTabChildTables");
        var attachments = context.Strings.Get("ItemSection.Items_Attachments");
        WorkspaceUi.SelectChildSection(window, childTables, attachments);
        var caption27 = $"{attachments} {context.Strings.Format("ItemChildRowCountFormat", 27)}";
        Ui.WaitUntil(
            () => WorkspaceUi.ChildSectionCaptions(window, childTables).Contains(caption27),
            "раздел «Вложения» показывает 27 строк");
        var grid = window.WaitById("RowsGrid");
        context.Recorder.Check("До добавления в гриде 27 строк", grid.RowCount() == 27, $"строк: {grid.RowCount()}");

        var add = window.WaitById("ItemTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))
            .FirstOrDefault(button => button.ReadName() == context.Strings.Get("EditorAddRowText"))
            ?? throw new UiTimeoutException("Кнопка «Добавить запись» не найдена.");
        add.InvokeButton();

        var caption28 = $"{attachments} {context.Strings.Format("ItemChildRowCountFormat", 28)}";
        Ui.WaitUntil(
            () => WorkspaceUi.ChildSectionCaptions(window, childTables).Contains(caption28),
            "счётчик раздела вырос до 28 строк",
            TimeSpan.FromSeconds(20));
        Ui.WaitUntil(() => grid.RowCount() == 28, "в гриде 28 строк", TimeSpan.FromSeconds(20));
        context.Recorder.Check("Добавлена одна строка вложения", true, caption28);

        var headers = WorkspaceUi.VisualHeaders(grid).ToList();
        var attachmentColumn = headers.IndexOf("attachmentIndex");
        var itemColumn = headers.IndexOf("itemIndex");
        context.Recorder.Check(
            "Колонки ключа вложения и предмета видны",
            attachmentColumn >= 0 && itemColumn >= 0,
            string.Join(" | ", headers));
        var lastRow = 27;
        context.Recorder.Check(
            "Новая строка привязана к предмету 12",
            WorkspaceUi.ReadCell(grid, lastRow, itemColumn) == "Colt M4 Commando",
            WorkspaceUi.ReadCell(grid, lastRow, itemColumn));

        var cell = grid.Cell(lastRow, attachmentColumn);
        cell.Click();
        Thread.Sleep(150);
        cell.SetFocus();
        Thread.Sleep(120);
        NativeInput.PressIn(grid, NativeInput.VirtualKey.F2);
        Thread.Sleep(250);
        var picker = window.WaitById("attachmentIndex");
        picker.SetFocus();
        NativeInput.TypeTextIn(picker, AttachmentName);
        Thread.Sleep(400);

        var option = Ui.WaitFor(
            () => AutomationElement.RootElement
                .FindAll(
                    TreeScope.Descendants,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, context.App.ProcessId),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem)))
                .Cast<AutomationElement>()
                .FirstOrDefault(item => Ui.ItemText(item).Trim() == AttachmentOption),
            $"подсказка списка «{AttachmentOption}»",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check(
            "Picker предлагает вложение с его ID",
            option is not null,
            AttachmentOption);

        NativeInput.PressIn(picker, NativeInput.VirtualKey.Return);
        Thread.Sleep(200);
        NativeInput.PressIn(picker, NativeInput.VirtualKey.Tab);
        Thread.Sleep(300);
        var chosen = WorkspaceUi.ReadCell(grid, lastRow, attachmentColumn);
        context.Recorder.Check("Выбор записывает вложение", chosen == AttachmentName, chosen);

        var delete = window.WaitById("ItemTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))
            .FirstOrDefault(button => button.ReadName() == context.Strings.Get("EditorDeleteRowText"))
            ?? throw new UiTimeoutException("Кнопка «Удалить» не найдена.");
        grid.Cell(lastRow, itemColumn).Click();
        Thread.Sleep(150);
        grid.Cell(lastRow, itemColumn).SetFocus();
        Thread.Sleep(120);
        delete.InvokeButton();
        var dialog = ScenarioUi.WaitForMessageBox(context.App);
        var text = ScenarioUi.ReadMessageBoxText(dialog);
        context.Recorder.Check(
            "Удаление строки запрашивает подтверждение",
            text.Contains("12", StringComparison.Ordinal),
            text);
        ScenarioUi.AnswerMessageBox(dialog, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        Ui.WaitUntil(
            () => grid.RowCount() == 27,
            "после удаления в гриде снова 27 строк",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Удаление убирает строку", true);

        window.WaitById("ItemUndoButton").InvokeButton();
        Ui.WaitUntil(
            () => grid.RowCount() == 28
                && WorkspaceUi.ReadCell(grid, lastRow, attachmentColumn) == AttachmentName,
            "Undo возвращает строку с выбранным вложением",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Undo возвращает удалённую строку", true);

        WorkspaceUi.UndoAll(context, "ItemUndoButton", "ItemSaveButton");
        Ui.WaitUntil(
            () => grid.RowCount() == 27,
            "после отмены всех правок снова 27 строк",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Все правки отменены, вкладка чистая",
            !window.WaitById("ItemSaveButton").IsEnabled(),
            null);
    }
}
