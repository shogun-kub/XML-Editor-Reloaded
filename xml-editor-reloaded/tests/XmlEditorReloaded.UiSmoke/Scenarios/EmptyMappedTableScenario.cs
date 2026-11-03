using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S90: deleting every row of a mapped non-lookup table, saving, and reopening it must keep the table
/// editable, show the columns from the column hints, and allow the first row to be added again.
/// </summary>
internal sealed class EmptyMappedTableScenario : SmokeScenario
{
    private const string Table = "NPCInventory.AdditionalDealer_11_Inventory";
    private const string File = "NPCInventory/AdditionalDealer_11_Inventory.xml";

    /// <inheritdoc />
    public override string Id => "S90";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V5];

    /// <inheritdoc />
    public override DatasetKind ActiveDataset => DatasetKind.V5;

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V5;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        ScenarioUi.OpenTable(context, Table);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "пустая mapped-таблица открыта");
        var grid = window.WaitById("RowsGrid");
        context.Recorder.Check("Исходная таблица содержит одну строку", grid.RowCount() == 1, $"строк: {grid.RowCount()}");

        var row = grid
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
            .FirstOrDefault()
            ?? throw new UiTimeoutException("Единственная строка таблицы не реализована.");
        row.SelectItem();
        Thread.Sleep(200);
        var delete = window.WaitById("DeleteRowButton");
        _ = Task.Run(() =>
        {
            try
            {
                delete.InvokeButton();
            }
            catch (Exception exception) when (
                exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The confirmation closed the dialog before the invoke returned.
            }
        });
        var messageBox = ScenarioUi.WaitForMessageBox(context.App, TimeSpan.FromSeconds(20));
        var message = ScenarioUi.ReadMessageBoxText(messageBox);
        context.Recorder.Check(
            "Подтверждение называет удаляемую запись",
            message.Contains(context.Strings.Format("EditorConfirmDelete", "0"), StringComparison.Ordinal),
            message);
        ScenarioUi.AnswerMessageBox(messageBox, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        Ui.WaitUntil(() => grid.RowCount() == 0, "таблица опустела");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Format("EditorRowDeleted", "0"),
            "статус удаления последней записи");
        WorkspaceUi.SaveGenericTable(context);
        context.ExpectChange(DatasetKind.V5, "TableData/" + File);

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        ScenarioUi.OpenTable(context, Table);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "опустевшая таблица открыта повторно");

        var headers = ScenarioUi.ColumnHeaderCaptions(window);
        context.Recorder.Check(
            "После переоткрытия видны колонки из hints",
            headers.Contains("uiIndex")
                && headers.Contains("sItemIndex")
                && headers.Contains("ubOptimalNumber"),
            "колонки: " + string.Join(", ", headers));
        context.Recorder.Check(
            "Пустая таблица редактируема",
            window.WaitById("AddRowButton").IsEnabled(),
            $"Add.IsEnabled={window.WaitById("AddRowButton").IsEnabled()}");
        context.Recorder.Check(
            "Пустая таблица не показывает строк",
            window.WaitById("RowsGrid").RowCount() == 0,
            $"строк: {window.WaitById("RowsGrid").RowCount()}");

        window.WaitById("AddRowButton").InvokeButton();
        Ui.WaitUntil(() => window.WaitById("RowsGrid").RowCount() == 1, "добавлена первая строка");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Format("EditorRowAdded", "0"),
            "статус добавления первой строки");
        context.Recorder.Check(
            "Новая строка получила uiIndex 0",
            window.WaitById("RowsGrid").Cell(0, 0).ReadName() == "0",
            window.WaitById("RowsGrid").Cell(0, 0).ReadName());
        WorkspaceUi.SaveGenericTable(context);

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        ScenarioUi.OpenTable(context, Table);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "таблица с первой строкой открыта повторно");
        context.Recorder.Check(
            "Сохранённая первая строка осталась",
            window.WaitById("RowsGrid").RowCount() == 1,
            $"строк: {window.WaitById("RowsGrid").RowCount()}");
    }
}
