using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S06: opening a generic table shows a tab with the table name and data-set label, a populated grid,
/// and the overlay language selector; the same works for v5 without overlays.
/// </summary>
internal sealed class OpenGenericTableScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S06";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override DatasetKind ActiveDataset => DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;

        ScenarioUi.OpenTable(context, "Backgrounds");
        Ui.WaitUntil(
            () => ScenarioUi.TabNames(window).Any(name => name.Contains("Backgrounds", StringComparison.Ordinal)),
            "вкладка Backgrounds открыта");
        var tabs = ScenarioUi.TabNames(window);
        context.Recorder.Check(
            "Вкладка Backgrounds имеет метку датасета v4",
            tabs.Any(name => name.Contains("Backgrounds", StringComparison.Ordinal)
                && name.Contains("JA 2 1.13 v4", StringComparison.Ordinal)),
            "вкладки: " + string.Join(" / ", tabs));

        var grid = window.WaitById("RowsGrid");
        var rows = grid.RowCount();
        context.Recorder.Check("Сетка Backgrounds содержит строки", rows > 0, $"строк: {rows}");
        var status = ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "статус «Готово.»");
        context.Recorder.Check("Статус редактора «Готово.»", true, status);

        var languageCombo = window.WaitById("LanguageCombo");
        Ui.WaitUntil(() => languageCombo.IsEnabled(), "селектор языка доступен");
        var languages = ScenarioUi.ReadComboItems(languageCombo);
        context.Recorder.Check(
            "У v4 Backgrounds есть базовая и русская таблица",
            languages.Contains(context.Strings.Get("EditorBaseLanguageOption"), StringComparer.Ordinal)
                && languages.Any(static option => option.StartsWith("Russian", StringComparison.OrdinalIgnoreCase)),
            "варианты: " + string.Join(", ", languages));

        // Switch the active data set without closing the v4 tab.
        ScenarioUi.SelectDataDirectory(window, context.Data(DatasetKind.V5).DataDirectory);
        var template = context.Strings.Get("StatusTablesLoaded");
        ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(ScenarioUi.FormatPrefix(template), StringComparison.Ordinal),
            "список таблиц v5 загружен");
        context.Recorder.Check(
            "Вкладка v4 осталась открытой после переключения",
            ScenarioUi.TabNames(window).Any(name => name.Contains("Backgrounds", StringComparison.Ordinal)),
            "вкладки: " + string.Join(" / ", ScenarioUi.TabNames(window)));

        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        Ui.WaitUntil(
            () => ScenarioUi.TabNames(window).Any(name => name.Contains("Lookup.Cursor", StringComparison.Ordinal)
                && name.Contains("JA 2 1.13 v5", StringComparison.Ordinal)),
            "вкладка Lookup.Cursor v5 открыта",
            TimeSpan.FromSeconds(30));

        var v5Languages = ScenarioUi.ReadComboItems(window.WaitById("LanguageCombo"));
        context.Recorder.Check(
            "У v5 Lookup.Cursor только базовая таблица",
            v5Languages.Count == 1 && v5Languages[0] == context.Strings.Get("EditorBaseLanguageOption"),
            "варианты: " + string.Join(", ", v5Languages));

        var v5Rows = window.WaitById("RowsGrid").RowCount();
        context.Recorder.Check("Сетка Lookup.Cursor содержит строки", v5Rows > 0, $"строк: {v5Rows}");
    }
}
