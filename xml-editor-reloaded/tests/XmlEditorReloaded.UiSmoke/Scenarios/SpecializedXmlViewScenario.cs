using System.IO;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S102: the global XML switch also works for the specialized item workspace.
/// </summary>
internal sealed class SpecializedXmlViewScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S102";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        ScenarioUi.OpenTable(context, "Items.Items");
        window.WaitById("ItemTabs", TimeSpan.FromSeconds(60));
        Screenshots.Capture(
            context.App,
            Path.Combine(context.Directory, "S102-item-grid.png"));

        window.WaitById("TableXmlViewButton").SelectItem();
        var xmlBox = window.WaitById("XmlTextBox");
        Ui.WaitUntil(
            () => xmlBox.ReadValue().Contains("<ITEMLIST>", StringComparison.Ordinal),
            "XML item-таблицы загружен",
            TimeSpan.FromSeconds(30));
        Ui.WaitUntil(
            () => window.FindById("ItemTabs") is null,
            "item-редактор скрыт в режиме XML",
            TimeSpan.FromSeconds(10));

        context.Recorder.Check(
            "Специализированный редактор скрыт",
            window.FindById("ItemTabs") is null,
            "ItemTabs");
        context.Recorder.Check(
            "XML item-таблицы содержит записи",
            xmlBox.ReadValue().Contains("<ITEM>", StringComparison.Ordinal),
            "ITEM");

        Screenshots.Capture(
            context.App,
            Path.Combine(context.Directory, "S102-item-xml.png"));

        window.WaitById("TableGridViewButton").SelectItem();
        window.WaitById("ItemTabs", TimeSpan.FromSeconds(30));
        Screenshots.Capture(
            context.App,
            Path.Combine(context.Directory, "S102-item-grid-restored.png"));
        context.Recorder.Check(
            "Возврат к Grid восстанавливает item-редактор",
            window.FindById("ItemTabs") is not null,
            "ItemTabs");
    }
}
