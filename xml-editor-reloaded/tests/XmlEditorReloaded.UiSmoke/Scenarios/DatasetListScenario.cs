using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S02: an active v4 data directory loads the table list with language and metadata columns.
/// </summary>
internal sealed class DatasetListScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S02";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var template = context.Strings.Get("StatusTablesLoaded");
        var prefix = ScenarioUi.FormatPrefix(template);
        var status = ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(prefix, StringComparison.Ordinal) && text.EndsWith('.'),
            "список таблиц загружен");
        context.Recorder.Check("Статус сообщает о загрузке таблиц", true, status);

        var rowCount = window.WaitById("TablesGrid").RowCount();
        var expectedStatus = string.Format(template, rowCount);
        context.Recorder.Check(
            "Счётчик загруженных таблиц совпадает со списком",
            status == expectedStatus,
            $"статус: «{status}», строк: {rowCount}");

        var items = ScenarioUi.FindTableRow(window, "Items.Items");
        context.Recorder.Check("Items.Items есть в списке", items is not null);
        if (items is null)
        {
            return;
        }

        var itemsCells = items.ReadRowCells();
        context.Recorder.Check(
            "Items.Items помечен русским оверлеем",
            itemsCells.ElementAtOrDefault(1) == "ru",
            $"колонки: {string.Join(" | ", itemsCells)}");
        context.Recorder.Check(
            "Items.Items имеет метаданные",
            itemsCells.ElementAtOrDefault(2) == context.Strings.Get("Mapped"),
            $"колонки: {string.Join(" | ", itemsCells)}");

        // Data that exists only in the other version must not appear in this list, and overlay files are
        // not standalone tables.
        context.Recorder.Check(
            "Таблица, которой нет в v4, отсутствует в списке",
            ScenarioUi.FindTableRow(window, "Email.EmailSenderNameList") is null);
        context.Recorder.Check(
            "Файл русского оверлея не показан как таблица",
            ScenarioUi.FindTableRow(window, "Russian.Items") is null);

        foreach (var (name, expectedLanguage) in new[]
                 {
                     ("Inventory.MercStartingGear", "ru"),
                     ("NPCInventory.AdditionalDealer_43_Inventory", "—"),
                     ("Lookup.AttachmentPoint", "—"),
                 })
        {
            var row = ScenarioUi.FindTableRow(window, name);
            context.Recorder.Check($"{name} есть в списке", row is not null);
            if (row is null)
            {
                continue;
            }

            var cells = row.ReadRowCells();
            context.Recorder.Check(
                $"{name}: колонка «Языки» = {expectedLanguage}",
                cells.ElementAtOrDefault(1) == expectedLanguage,
                $"колонки: {string.Join(" | ", cells)}");
        }

        var windowCount = ScenarioUi.TopLevelWindowCount(context.App.ProcessId);
        context.Recorder.Check(
            "Модальных ошибок при загрузке нет",
            windowCount == 1,
            $"окон процесса: {windowCount}");
    }
}
