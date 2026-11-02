using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S01: a cold start without configured data directories shows the empty-state texts and an empty
/// table list, and the window stays responsive.
/// </summary>
internal sealed class ColdStartScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S01";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;

        var status = window.WaitById("StatusText").ReadName();
        var expectedStatus = context.Strings.Get("StatusNoDataDirectory");
        context.Recorder.Check(
            "Статус пустого каталога корректен",
            status == expectedStatus,
            status);

        var placeholder = window.FindByName(context.Strings.Get("EditorPlaceholder"));
        context.Recorder.Check(
            "Подсказка о двойном щелчке видна",
            placeholder is not null,
            placeholder is null ? "подсказка не найдена" : null);

        var tableGrid = window.WaitById("TablesGrid");
        var rows = tableGrid.FindAll(new PropertyCondition(
            AutomationElement.ControlTypeProperty,
            ControlType.DataItem));
        context.Recorder.Check(
            "Список таблиц пуст",
            rows.Count == 0,
            $"строк: {rows.Count}");

        var saveAll = window.WaitById("SaveAllButton");
        context.Recorder.Check(
            "Команда «Сохранить всё» доступна в оболочке",
            saveAll is not null,
            null);

        ScenarioUi.ExpandMenu(window.WaitById("FileMenu"));
        var exit = ScenarioUi.WaitForMenuItemById(context.App, "ExitMenuItem");
        context.Recorder.Check("В меню «Файл» есть «Выход»", exit is not null, exit?.ReadName());
        ScenarioUi.Collapse(window.WaitById("FileMenu"));
        context.Recorder.Check(
            "Отдельного меню «Правка» больше нет",
            window.FindById("EditMenu") is null,
            null);

        context.Recorder.Check(
            "Окно не упало и отвечает",
            window.Current.IsEnabled,
            null);
    }
}
