using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S56: dict-family columns are edited as concrete columns — <c>ItemCapacityPerSizeN</c> in
/// <c>Items.Pockets</c> and <c>itemN</c> in <c>Items.RandomItem</c>, where the latter keeps its item
/// picker.
/// </summary>
internal sealed class DictColumnsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S56";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;

        ScenarioUi.OpenTable(context, "Items.Pockets");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.Pockets открыт");
        var grid = window.WaitById("RowsGrid");
        var pockets = grid.RowCount();
        context.Recorder.Check("Items.Pockets содержит 62 записи", pockets == 62, $"строк: {pockets}");

        var edited = DetailsUi.EditField(context, grid, 0, "ItemCapacityPerSize24", "7");
        context.Recorder.Check("Поле ItemCapacityPerSize24 доступно для правки", edited.IsEnabled(), null);
        Ui.WaitUntil(() => window.WaitById("SaveButton").IsEnabled(), "правка сделала таблицу грязной");
        context.Recorder.Check("Правка ItemCapacityPerSize24 помечает таблицу грязной", true);
        window.WaitById("UndoButton").InvokeButton();
        Ui.WaitUntil(() => !window.WaitById("SaveButton").IsEnabled(), "правка отменена");

        ScenarioUi.OpenTable(context, "Items.RandomItem");
        Ui.WaitUntil(
            () => ScenarioUi.TabNames(window).Any(name => name.Contains("Items.RandomItem", StringComparison.Ordinal)),
            "Items.RandomItem открыт",
            TimeSpan.FromSeconds(30));
        grid = window.WaitById("RowsGrid");
        grid.Cell(1, 0).SelectItem();
        Thread.Sleep(200);
        window.WaitById("DetailFilterBox").SetText("item1");
        Thread.Sleep(300);
        foreach (var group in window.FindAll(new PropertyCondition(
                     AutomationElement.ControlTypeProperty,
                     ControlType.Group)))
        {
            if (string.IsNullOrEmpty(group.ReadName()))
            {
                ScenarioUi.SetExpanded(group, expanded: true);
            }
        }

        var picker = window.WaitById("item1", TimeSpan.FromSeconds(20));
        var value = picker.ReadValue();
        context.Recorder.Check(
            "item1 редактируется через picker и показывает имя предмета",
            !string.IsNullOrWhiteSpace(value),
            value);
        context.Recorder.Note("Выбор значения в picker требует Enter и на обычном рабочем столе; сценарий проверяет наличие и отображение picker.");
    }
}
