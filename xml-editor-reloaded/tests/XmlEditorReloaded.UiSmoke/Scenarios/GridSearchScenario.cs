using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S07: the grid search narrows the rows, the counter shows the filtered total, and a query without
/// matches shows the empty-result text.
/// </summary>
internal sealed class GridSearchScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S07";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        ScenarioUi.OpenTable(context, "Items.AttachmentSlots");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "таблица поиска открыта");

        var grid = window.WaitById("RowsGrid");
        var total = grid.RowCount();
        context.Recorder.Check("Таблица Items.AttachmentSlots непустая", total > 0, $"строк: {total}");

        var counter = window.WaitById("TableRecordCount");
        var search = window.WaitById("SearchBox");

        search.SetText("Laser");
        Ui.WaitUntil(
            () =>
            {
                var text = counter.ReadName();
                return text.Contains(" из ", StringComparison.Ordinal)
                    || text == context.Strings.Get("EditorNoMatches");
            },
            "счётчик показывает результат поиска");
        var filteredText = counter.ReadName();
        context.Recorder.Check(
            "Поиск по «Laser» уменьшает число строк",
            filteredText != string.Format(context.Strings.Get("RecordCount"), total),
            filteredText);

        var visibleText = string.Join(" ", grid
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
            .Select(row => string.Join(" ", row.ReadRowCells())));
        context.Recorder.Check(
            "В результатах поиска есть Laser Attachments",
            visibleText.Contains("Laser", StringComparison.OrdinalIgnoreCase),
            filteredText);

        search.SetText("zzzz-no-such-value");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorNoMatches"),
            "сообщение об отсутствии совпадений");
        context.Recorder.Check("Бессмысленный запрос даёт «Нет записей по запросу.»", true);
        var emptyCounter = counter.ReadName();
        context.Recorder.Check(
            "Счётчик показывает 0 из N",
            emptyCounter == string.Format(context.Strings.Get("EditorRowCount"), 0, total),
            emptyCounter);

        search.SetText(string.Empty);
        Ui.WaitUntil(
            () => grid.RowCount() == total
                && counter.ReadName().Contains(total.ToString(), StringComparison.Ordinal),
            "очистка поиска возвращает все строки");
        context.Recorder.Check(
            "Очистка поля возвращает все строки",
            true,
            $"счётчик: {counter.ReadName()}, строк: {grid.RowCount()}");
    }
}
