using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S09: three selected rows are edited through the bulk dialog, only those rows change, and undo and
/// redo restore and reapply the values without touching the XML until the table is saved.
/// </summary>
internal sealed class BulkEditScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S09";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Items/Weapons.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(file);

        ScenarioUi.OpenTable(context, "Items.Weapons");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.Weapons открыт");
        var grid = window.WaitById("RowsGrid");
        grid.ScrollToTop();

        var rows = grid
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
            .Take(3)
            .ToList();
        context.Recorder.Check("В сетке видны три строки для выделения", rows.Count == 3, $"строк: {rows.Count}");
        rows[0].SelectItem();
        rows[1].AddToSelection();
        rows[2].AddToSelection();
        var selectedText = string.Format(context.Strings.Get("EditorSelectedRowsFormat"), 3);
        Ui.WaitUntil(
            () => window.WaitById("SelectedRowCount").ReadName() == selectedText,
            $"счётчик выделения показывает «{selectedText}»");
        context.Recorder.Check(
            "Ctrl-выделение показывает «Выделено строк: 3»",
            window.WaitById("SelectedRowCount").ReadName() == selectedText,
            window.WaitById("SelectedRowCount").ReadName());

        var column = ScenarioUi.ColumnHeaderCaptions(window)
            .Select((caption, index) => (caption, index))
            .First(pair => pair.caption == "szWeaponName")
            .index;
        var before = Enumerable.Range(0, 4)
            .Select(row => grid.Cell(row, column).ReadName())
            .ToList();

        ScenarioUi.OpenMenuItem(context, "DataMenu", "BulkEditButton");
        var dialog = ScenarioUi.WaitForWindowWithId(
            context.App,
            "BulkEditColumnCombo",
            "диалог массовой правки");
        dialog.WaitById("BulkEditColumnCombo").SelectComboItemStartingWith("ubWeaponClass");
        dialog.WaitById("BulkEditOperandBox").SelectComboItem("Submachinegun (2)");
        context.Recorder.Check(
            "Поле массовой правки предлагает lookup-имя",
            dialog.WaitById("BulkEditOperandBox").ReadValue() == "2",
            dialog.WaitById("BulkEditOperandBox").ReadValue());
        dialog.WaitById("BulkEditColumnCombo").SelectComboItemStartingWith("szWeaponName");
        dialog.WaitById("BulkEditOperationCombo").SelectComboItem(context.Strings.Get("BulkEditOperationSet"));
        dialog.WaitById("BulkEditOperandBox").SetText("42");

        var summary = context.Strings.Format("BulkEditSummaryFormat", 3, 3, 0, 0, 0);
        Ui.WaitUntil(
            () => dialog.WaitById("BulkEditSummary").ReadName() == summary,
            $"сводка массовой правки «{summary}»");
        context.Recorder.Check("Сводка обещает изменить три строки", true, summary);
        var preview = dialog.WaitById("BulkEditPreviewGrid");
        context.Recorder.Check(
            "Предпросмотр содержит три изменённые строки",
            preview.RowCount() == 3,
            $"строк: {preview.RowCount()}");
        var statuses = preview
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
            .Select(row => string.Join(" ", row.ReadRowCells()))
            .ToList();
        context.Recorder.Check(
            "Все строки предпросмотра помечены «Изменено»",
            statuses.Count == 3
                && statuses.All(status => status.Contains(context.Strings.Get("BulkEditStatusChanged"), StringComparison.Ordinal)),
            string.Join(" | ", statuses));

        var apply = dialog.WaitById("BulkEditApplyButton");
        context.Recorder.Check("«Применить» доступна при корректном предпросмотре", apply.IsEnabled(), null);
        apply.InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "BulkEditColumnCombo");

        var applied = context.Strings.Format("EditorBulkAppliedFormat", 3, "szWeaponName", "42");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == applied,
            "статус массовой правки",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Массовая правка сообщает о трёх изменённых строках", true, applied);

        var after = Enumerable.Range(0, 4)
            .Select(row => grid.Cell(row, column).ReadName())
            .ToList();
        context.Recorder.Check(
            "Значение изменено только у трёх выделенных строк",
            after.Take(3).All(value => value == "42") && after[3] == before[3],
            $"до: {string.Join(", ", before)}; после: {string.Join(", ", after)}");

        window.WaitById("UndoButton").InvokeButton();
        Ui.WaitUntil(
            () => Enumerable.Range(0, 3).All(row => grid.Cell(row, column).ReadName() == before[row]),
            "отмена вернула прежние значения");
        context.Recorder.Check("Undo восстанавливает значения", true, string.Join(", ", before));

        window.WaitById("RedoButton").InvokeButton();
        Ui.WaitUntil(
            () => Enumerable.Range(0, 3).All(row => grid.Cell(row, column).ReadName() == "42"),
            "повтор применил значения снова");
        context.Recorder.Check("Redo повторно применяет значения", true, null);

        window.WaitById("UndoButton").InvokeButton();
        Ui.WaitUntil(() => !window.WaitById("SaveButton").IsEnabled(), "после отмены вкладка стала чистой");
        context.Recorder.Check(
            "После отмены сохранение снова недоступно",
            !window.WaitById("SaveButton").IsEnabled(),
            null);
        context.Recorder.Check(
            "Массовая правка не изменила XML",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);
    }
}

