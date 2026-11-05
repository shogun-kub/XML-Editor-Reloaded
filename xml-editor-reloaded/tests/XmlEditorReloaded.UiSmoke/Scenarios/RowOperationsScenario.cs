using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S10: a lookup row can be added, renamed, duplicated, and deleted with confirmation, every step is
/// undoable, and a table with a compound key refuses duplication.
/// </summary>
internal sealed class RowOperationsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S10";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Lookup/Cursor.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(file);

        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.Cursor открыт");
        var grid = window.WaitById("RowsGrid");
        context.Recorder.Check("Lookup.Cursor содержит 23 записи", grid.RowCount() == 23, $"строк: {grid.RowCount()}");

        window.WaitById("AddRowButton").InvokeButton();
        WaitForRowCount(window, 24);
        grid.ScrollToBottom();
        var addedKey = grid.Cell(23, 0).ReadName();
        context.Recorder.Check("Новая запись получает следующий ключ", addedKey == "24", addedKey);

        grid.EditCell(23, 1, "Smoke added row");
        Ui.WaitUntil(() => grid.Cell(23, 1).ReadName() == "Smoke added row", "имя новой записи записано");

        window.WaitById("DuplicateRowButton").InvokeButton();
        WaitForRowCount(window, 25);
        grid.ScrollToBottom();
        var duplicateKey = grid.Cell(24, 0).ReadName();
        context.Recorder.Check("Дубликат получает новый ключ", duplicateKey == "25", duplicateKey);
        context.Recorder.Check(
            "Дубликат содержит имя исходной записи",
            grid.Cell(24, 1).ReadName() == "Smoke added row",
            grid.Cell(24, 1).ReadName());

        var delete = window.WaitById("DeleteRowButton");
        _ = Task.Run(() =>
        {
            try
            {
                delete.InvokeButton();
            }
            catch (Exception exception) when (exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The confirmation closed the dialog and the button disappeared before the invoke returned.
            }
        });
        var messageBox = ScenarioUi.WaitForMessageBox(context.App, TimeSpan.FromSeconds(20));
        var message = ScenarioUi.ReadMessageBoxText(messageBox);
        context.Recorder.Check(
            "Подтверждение удаления называет запись и перевод",
            message.Contains(context.Strings.Format("EditorConfirmDelete", "25"), StringComparison.Ordinal),
            message);
        ScenarioUi.AnswerMessageBox(messageBox, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        WaitForRowCount(window, 24);
        var deleted = context.Strings.Format("EditorRowDeleted", "25");
        ScenarioUi.WaitForEditorStatus(context, text => text == deleted, "статус удаления записи");
        context.Recorder.Check("Удаление убирает только выбранную запись", true, deleted);

        window.WaitById("UndoButton").InvokeButton();
        WaitForRowCount(window, 25);
        context.Recorder.Check("Undo возвращает удалённую запись", true, null);

        window.WaitById("UndoButton").InvokeButton();
        WaitForRowCount(window, 24);
        context.Recorder.Check("Undo убирает дубликат", true, null);

        window.WaitById("UndoButton").InvokeButton();
        WaitForRowCount(window, 24);
        window.WaitById("UndoButton").InvokeButton();
        WaitForRowCount(window, 23);
        Ui.WaitUntil(() => !window.WaitById("SaveButton").IsEnabled(), "после отмены вкладка стала чистой");
        context.Recorder.Check("Все операции отменены, таблица чистая", true, null);
        context.Recorder.Check(
            "Операции со строками не изменили XML",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);

        // A table with a compound key cannot be duplicated; the button must be disabled.
        ScenarioUi.OpenTable(context, "Items.Attachments");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.Attachments открыт");
        var attachments = window.WaitById("RowsGrid");
        attachments.ScrollToTop();
        var row = attachments
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
            .FirstOrDefault();
        context.Recorder.Check("Items.Attachments содержит строки", row is not null, $"строк: {attachments.RowCount()}");
        row!.SelectItem();
        var duplicate = window.WaitById("DuplicateRowButton");
        context.Recorder.Check(
            "В таблице с составным ключом «Дублировать» недоступно",
            !duplicate.IsEnabled(),
            $"IsEnabled: {duplicate.IsEnabled()}");
        context.Recorder.Note(
            "Кнопка «Дублировать» отключена: явный отказ для составного ключа (L-OPS-009), поэтому "
            + "текст EditorDuplicateUnsupported в интерфейсе не появляется.");
    }

    private static void WaitForRowCount(AutomationElement window, int expected)
    {
        Ui.WaitUntil(
            () => window.WaitById("RowsGrid").RowCount() == expected,
            $"в сетке {expected} строк");
    }
}

