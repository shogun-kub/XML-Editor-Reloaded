using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S03: switching the active data set keeps open tabs, their data-set labels and their unsaved edits,
/// and lets a table from the other data set be opened in a second tab.
/// </summary>
internal sealed class DatasetSwitchScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S03";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override DatasetKind ActiveDataset => DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        const string editedValue = "Unsaved Switch Value";

        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.Cursor v4 открыт");

        var grid = window.WaitById("RowsGrid");
        var save = window.WaitById("SaveButton");
        grid.EditCell(0, 1, editedValue);
        Ui.WaitUntil(() => save.IsEnabled(), "правка v4 отмечена как несохранённая");

        ScenarioUi.SelectDataDirectory(window, context.Data(DatasetKind.V5).DataDirectory);
        var loaded = ScenarioUi.FormatPrefix(context.Strings.Get("StatusTablesLoaded"));
        ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(loaded, StringComparison.Ordinal),
            "список v5 загружен");

        var tabs = ScenarioUi.TabNames(window);
        context.Recorder.Check(
            "Вкладка v4 осталась открытой после переключения",
            tabs.Any(name => name.Contains("Lookup.Cursor", StringComparison.Ordinal)
                && name.Contains("JA 2 1.13 v4", StringComparison.Ordinal)),
            "вкладки: " + string.Join(" / ", tabs));
        context.Recorder.Check(
            "Несохранённая правка пережила переключение датасета",
            save.IsEnabled(),
            $"IsEnabled: {save.IsEnabled()}");
        var preservedCell = window.WaitById("RowsGrid").Cell(0, 1).ReadName();
        context.Recorder.Check(
            "В сетке v4 видно несохранённое значение",
            preservedCell.Contains(editedValue, StringComparison.Ordinal),
            preservedCell);

        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        Ui.WaitUntil(
            () => ScenarioUi.TabNames(window).Any(
                name => name.Contains("Lookup.Cursor", StringComparison.Ordinal)
                    && name.Contains("JA 2 1.13 v5", StringComparison.Ordinal)),
            "вкладка Lookup.Cursor v5 открыта",
            TimeSpan.FromSeconds(30));
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.Cursor v5 открыт");

        tabs = ScenarioUi.TabNames(window);
        context.Recorder.Check(
            "Обе вкладки сосуществуют с метками датасетов",
            tabs.Count(name => name.Contains("Lookup.Cursor", StringComparison.Ordinal)) == 2,
            "вкладки: " + string.Join(" / ", tabs));

        var v5Rows = window.WaitById("RowsGrid").RowCount();
        context.Recorder.Check("Сетка v5 содержит строки", v5Rows > 0, $"строк: {v5Rows}");
        var v5Cell = window.WaitById("RowsGrid").Cell(0, 1).ReadName();
        context.Recorder.Check(
            "Сетка v5 показывает данные своего датасета",
            !v5Cell.Contains(editedValue, StringComparison.Ordinal),
            v5Cell);
    }
}
