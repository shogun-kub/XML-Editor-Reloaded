using System.IO;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S104: the global XML switch also works for the nested map workspace.
/// </summary>
internal sealed class NestedXmlViewScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S104";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        ScenarioUi.OpenTable(context, "Map.Cities");
        window.WaitById("NestedRecordList", TimeSpan.FromSeconds(60));

        window.WaitById("TableXmlViewButton").SelectItem();
        var xmlBox = window.WaitById("XmlTextBox");
        Ui.WaitUntil(
            () => xmlBox.ReadValue().Contains("<CITYLIST>", StringComparison.Ordinal),
            "XML карты загружен",
            TimeSpan.FromSeconds(30));
        Ui.WaitUntil(
            () => window.FindById("NestedRecordList") is null,
            "nested-редактор скрыт в режиме XML",
            TimeSpan.FromSeconds(10));

        context.Recorder.Check(
            "Nested-редактор скрыт",
            window.FindById("NestedRecordList") is null,
            "NestedRecordList");
        context.Recorder.Check(
            "XML карты содержит города",
            xmlBox.ReadValue().Contains("<CITY>", StringComparison.Ordinal),
            "CITY");

        Screenshots.Capture(
            context.App,
            Path.Combine(context.Directory, "S104-map-xml.png"));

        window.WaitById("TableGridViewButton").SelectItem();
        window.WaitById("NestedRecordList", TimeSpan.FromSeconds(30));
        context.Recorder.Check(
            "Возврат к Grid восстанавливает nested-редактор",
            window.FindById("NestedRecordList") is not null,
            "NestedRecordList");
    }
}
