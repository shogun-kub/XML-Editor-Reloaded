using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S08: the structured filter narrows the grid, combines criteria with AND and OR, keeps a cell value
/// filter from the context menu, and never changes the XML or discards unsaved edits.
/// </summary>
internal sealed class GridFilterScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S08";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Items/Weapons.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(file);
        var total = 389;

        ScenarioUi.OpenTable(context, "Items.Weapons");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.Weapons открыт");
        var grid = window.WaitById("RowsGrid");
        context.Recorder.Check(
            "Items.Weapons содержит 389 записей",
            grid.RowCount() == total,
            $"строк: {grid.RowCount()}");

        // The filter must not discard an unsaved edit of a row that the filter hides.
        grid.EditCell(0, 1, "Smoke filter edit");
        Ui.WaitUntil(() => window.WaitById("SaveButton").IsEnabled(), "таблица стала грязной");

        var expander = window.WaitById("FilterExpander");
        ScenarioUi.SetExpanded(expander, expanded: true);
        var add = window.WaitById("FilterAddButton");
        add.InvokeButton();

        SelectCriterion(
            window,
            0,
            "ubWeaponClass",
            context.Strings.Get("FilterOperatorEquals"),
            "2",
            suggestedItem: "Submachinegun (2)");
        WaitForRowCount(context, 48, total);
        grid.SetFocus();
        Ui.WaitUntil(
            () => window.FindAllById("FilterOperandBox")[0].ReadValue() == "Submachinegun",
            "поле класса показывает lookup-имя");
        context.Recorder.Check("Фильтр по ubWeaponClass=2 оставляет 48 строк", true, CounterText(window));
        context.Recorder.Check(
            "Поле значения показывает lookup-имя класса оружия",
            window.FindAllById("FilterOperandBox")[0].ReadValue() == "Submachinegun",
            window.FindAllById("FilterOperandBox")[0].ReadValue());

        add.InvokeButton();
        SelectCriterion(
            window,
            1,
            "ubCalibre",
            context.Strings.Get("FilterOperatorEquals"),
            "2",
            suggestedItem: "9x19mm (2)");
        WaitForRowCount(context, 25, total);
        grid.SetFocus();
        Ui.WaitUntil(
            () => window.FindAllById("FilterOperandBox")[1].ReadValue() == "9x19mm",
            "поле калибра показывает имя связанной таблицы");
        context.Recorder.Check("Два условия объединяются через «И»: 25 строк", true, CounterText(window));
        context.Recorder.Check(
            "Поле значения показывает имя калибра из связанной таблицы",
            window.FindAllById("FilterOperandBox")[1].ReadValue() == "9x19mm",
            window.FindAllById("FilterOperandBox")[1].ReadValue());
        context.Recorder.Check(
            "Сводка показывает два условия",
            window.WaitById("FilterSummary").ReadName() == context.Strings.Format("EditorFilterSummaryFormat", 2),
            window.WaitById("FilterSummary").ReadName());

        window.WaitById("FilterLogicalOperatorCombo").SelectComboItem(context.Strings.Get("FilterOr"));
        WaitForRowCount(context, 72, total);
        context.Recorder.Check("Переключение на «Любое (ИЛИ)» даёт 72 строки", true, CounterText(window));

        window.WaitById("FilterClearButton").InvokeButton();
        WaitForRowCount(context, total, total);
        context.Recorder.Check(
            "Сброс фильтра возвращает исходный вид",
            window.WaitById("FilterSummary").ReadName() == context.Strings.Get("EditorFilterSummaryEmpty"),
            window.WaitById("FilterSummary").ReadName());

        var editedCell = grid.Cell(0, 1).ReadName();
        context.Recorder.Check(
            "Фильтр не скрывает несохранённую правку",
            editedCell.Contains("Smoke filter edit", StringComparison.Ordinal),
            editedCell);

        // The context menu adds an equality or inequality criterion for the current cell.
        var classColumn = ScenarioUi.ColumnHeaderCaptions(window)
            .Select((caption, index) => (caption, index))
            .First(pair => pair.caption == "ubWeaponClass")
            .index;
        OpenCellContextMenu(context, classColumn, context.Strings.Get("EditorFilterByValue"));
        WaitForRowCount(context, 1, total);
        context.Recorder.Check("«Фильтровать по значению» оставляет одну строку", true, CounterText(window));

        window.WaitById("FilterClearButton").InvokeButton();
        WaitForRowCount(context, total, total);
        context.Recorder.Check("Сброс фильтра после фильтра по значению", true, CounterText(window));

        OpenCellContextMenu(context, classColumn, context.Strings.Get("EditorFilterExcludeValue"));
        WaitForRowCount(context, total - 1, total);
        context.Recorder.Check("«Исключить значение» скрывает одну строку", true, CounterText(window));

        window.WaitById("FilterClearButton").InvokeButton();
        WaitForRowCount(context, total, total);

        window.WaitById("UndoButton").InvokeButton();
        Ui.WaitUntil(() => !window.WaitById("SaveButton").IsEnabled(), "отмена правки вернула чистое состояние");
        context.Recorder.Check(
            "XML не изменился за время работы фильтров",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);
    }

    private static void SelectCriterion(
        AutomationElement window,
        int index,
        string column,
        string operation,
        string operand,
        string? suggestedItem = null)
    {
        var columns = window.FindAllById("FilterColumnCombo");
        var operations = window.FindAllById("FilterOperatorCombo");
        var operands = window.FindAllById("FilterOperandBox");
        columns[index].SelectComboItem(column);
        operations[index].SelectComboItem(operation);
        operands[index].SetText(operand);
        if (suggestedItem is not null)
        {
            operands[index].SelectComboItem(suggestedItem);
        }

        Thread.Sleep(300);
    }

    private static void OpenCellContextMenu(ScenarioContext context, int column, string itemCaption)
    {
        for (var attempt = 0; attempt < 3; attempt++)
        {
            var grid = context.Window.WaitById("RowsGrid");
            grid.ScrollToTop();
            var cell = grid.Cell(0, column);
            cell.SetFocus();
            NativeInput.EnsureForeground(grid);
            NativeInput.RightClickWindowMessage(cell);
            try
            {
                ScenarioUi.WaitForMenuItem(context.App, itemCaption, TimeSpan.FromSeconds(4)).InvokeButton();
                return;
            }
            catch (UiTimeoutException) when (attempt < 2)
            {
                NativeInput.PressWindowMessage(grid, NativeInput.VirtualKey.Escape);
                NativeInput.RightClick(cell);
                Thread.Sleep(300);
            }
        }

        throw new UiTimeoutException($"пункт меню «{itemCaption}»");
    }

    private static void WaitForRowCount(ScenarioContext context, int visible, int total)
    {
        var expected = string.Format(context.Strings.Get("EditorRowCount"), visible, total);
        Ui.WaitUntil(
            () => CounterText(context.Window) == expected,
            $"счётчик показывает «{expected}»");
    }

    private static string CounterText(AutomationElement window) =>
        window.WaitById("TableRecordCount").ReadName();
}
