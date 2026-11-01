using System.IO;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S33: a v5 auxiliary inventory table opens in the generic grid, an edit is undone and then saved on
/// the writable copy, and saving rewrites only the edited value.
/// </summary>
internal sealed class AuxTableScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S33";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V5];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V5;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V5).TableFile("NPCInventory/AdditionalDealer_43_Inventory.xml");
        var before = XDocument.Parse(File.ReadAllText(file));
        var hashBefore = SaveDirtyTabScenario.HashFile(file);

        ScenarioUi.OpenTable(context, "NPCInventory.AdditionalDealer_43_Inventory");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "дополнительная таблица открыта",
            TimeSpan.FromSeconds(60));

        var grid = window.WaitById("RowsGrid");
        var rows = grid.RowCount();
        context.Recorder.Check("Дополнительная таблица содержит 44 записи", rows == 44, $"строк: {rows}");

        var headers = ScenarioUi.ColumnHeaderCaptions(window);
        context.Recorder.Check(
            "Схема выведена из документа без нормализации",
            headers.SequenceEqual(new[] { "uiIndex", "sItemIndex", "ubOptimalNumber" }, StringComparer.Ordinal),
            "колонки: " + string.Join(", ", headers));
        context.Recorder.Check(
            "Валидация доступна и не падает",
            window.WaitById("ValidationExpander") is not null);

        var save = window.WaitById("SaveButton");
        var optimalColumn = headers.IndexOf("ubOptimalNumber");
        DataOperationsUi.EditCellUntilDirty(window, grid, 0, optimalColumn, "9");
        context.Recorder.Check("Правка ячейки делает вкладку грязной", true);

        window.WaitById("UndoButton").InvokeButton();
        Ui.WaitUntil(() => !save.IsEnabled(), "отмена сняла признак изменений");
        context.Recorder.Check(
            "Undo возвращает исходное значение",
            WorkspaceUi.ReadCell(window.WaitById("RowsGrid"), 0, optimalColumn).Trim() == "1",
            WorkspaceUi.ReadCell(window.WaitById("RowsGrid"), 0, optimalColumn));
        context.Recorder.Check(
            "Отменённая правка не пишет файл",
            SaveDirtyTabScenario.HashFile(file) == hashBefore);

        DataOperationsUi.EditCellUntilDirty(window, grid, 0, optimalColumn, "9", TimeSpan.FromSeconds(15));
        context.Recorder.Check("Повторная правка снова делает вкладку грязной", save.IsEnabled(), null);
        save.InvokeButton();
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorSaved"),
            "статус «Сохранено.»",
            TimeSpan.FromSeconds(30));

        var after = XDocument.Load(file);
        var (onlyField, details) = DataOperationsUi.OnlyFieldChanged(
            before,
            after,
            recordIndex: 0,
            field: "ubOptimalNumber",
            expectedOld: "1",
            expectedNew: "9");
        context.Recorder.Check("Сохранение изменило только отредактированную ячейку", onlyField, details);
        context.Recorder.Check(
            "Схема и порядок полей не нормализованы",
            after.Root!.Elements().First().Elements().Select(static element => element.Name.LocalName)
                .SequenceEqual(
                    before.Root!.Elements().First().Elements().Select(static element => element.Name.LocalName),
                    StringComparer.Ordinal));

        context.ExpectChange(DatasetKind.V5, "TableData/NPCInventory/AdditionalDealer_43_Inventory.xml");
    }
}
