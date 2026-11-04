using System.IO;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S103: the global XML switch also works for the mercenary-gear workspace.
/// </summary>
internal sealed class MercXmlViewScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S103";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        WorkspaceUi.OpenMercGearWorkspace(context);
        window.WaitById("MercKitTabs", TimeSpan.FromSeconds(30));

        window.WaitById("TableXmlViewButton").SelectItem();
        var xmlBox = window.WaitById("XmlTextBox");
        Ui.WaitUntil(
            () => xmlBox.ReadValue().Contains("<MERCGEARLIST>", StringComparison.Ordinal),
            "XML merc-таблицы загружен",
            TimeSpan.FromSeconds(30));
        Ui.WaitUntil(
            () => window.FindById("MercKitTabs") is null,
            "merc-редактор скрыт в режиме XML",
            TimeSpan.FromSeconds(10));

        context.Recorder.Check(
            "Merc-редактор скрыт",
            window.FindById("MercKitTabs") is null,
            "MercKitTabs");
        context.Recorder.Check(
            "XML merc-таблицы содержит записи",
            xmlBox.ReadValue().Contains("<MERCGEAR>", StringComparison.Ordinal),
            "MERCGEAR");

        Screenshots.Capture(
            context.App,
            Path.Combine(context.Directory, "S103-merc-xml.png"));

        window.WaitById("TableGridViewButton").SelectItem();
        window.WaitById("MercKitTabs", TimeSpan.FromSeconds(30));
        context.Recorder.Check(
            "Возврат к Grid восстанавливает merc-редактор",
            window.FindById("MercKitTabs") is not null,
            "MercKitTabs");
    }
}
