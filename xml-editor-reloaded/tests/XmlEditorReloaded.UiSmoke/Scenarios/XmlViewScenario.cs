using System.IO;
using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S101: the table editor exposes a read-only XML snapshot without affecting the data grid.
/// </summary>
internal sealed class XmlViewScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S101";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        ScenarioUi.OpenTable(context, "Items.Weapons");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.Weapons открыт");

        var xmlTab = window.WaitById("TableXmlTab");
        xmlTab.SelectItem();
        var xmlBox = window.WaitById("XmlTextBox");
        Ui.WaitUntil(
            () => xmlBox.ReadValue().Contains("<WEAPON", StringComparison.Ordinal),
            "XML-снимок таблицы загружен",
            TimeSpan.FromSeconds(30));

        var xml = xmlBox.ReadValue();
        context.Recorder.Check(
            "XML начинается с объявления",
            xml.StartsWith("<?xml", StringComparison.Ordinal),
            xml[..Math.Min(80, xml.Length)]);
        context.Recorder.Check(
            "XML содержит записи таблицы",
            xml.Contains("<WEAPON>", StringComparison.Ordinal)
            && xml.Contains("<uiIndex>", StringComparison.Ordinal),
            "WEAPON/uiIndex");

        var readOnly = false;
        if (xmlBox.TryGetCurrentPattern(ValuePattern.Pattern, out var raw)
            && raw is ValuePattern value)
        {
            readOnly = value.Current.IsReadOnly;
        }

        context.Recorder.Check("XML-поле только для чтения", readOnly, readOnly.ToString());
        context.Recorder.Check(
            "Кнопка копирования XML доступна",
            window.FindById("CopyXmlButton") is not null,
            "CopyXmlButton");

        Screenshots.Capture(
            context.App,
            Path.Combine(context.Directory, "S101-xml-view.png"));

        window.WaitById("TableDataTab").SelectItem();
        window.WaitById("RowsGrid");
        context.Recorder.Check(
            "Возврат к данным не ломает сетку",
            window.FindById("RowsGrid") is not null,
            "RowsGrid");
    }
}
