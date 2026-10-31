using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S05: the details panel shows the record count for a valid table, and the diagnostics panel reports
/// a malformed v5 file without trying to repair it.
/// </summary>
internal sealed class TableDetailsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S05";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override DatasetKind ActiveDataset => DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;

        var itemsRow = ScenarioUi.FindTableRow(window, "Items.Items")
            ?? throw new SmokeCheckException("Items.Items есть в списке", "строка не найдена");
        itemsRow.SelectItem();
        Ui.WaitUntil(
            () => window.WaitById("DetailsRecordCount").ReadName()
                == string.Format(context.Strings.Get("RecordCount"), 1776),
            "детали Items.Items показывают 1776 записей");
        context.Recorder.Check(
            "Items.Items: «Записей: 1776»",
            true,
            window.WaitById("DetailsRecordCount").ReadName());
        var diagnostics = ReadDiagnostics(window);
        context.Recorder.Check(
            "У Items.Items нет диагностики",
            diagnostics.Count == 0,
            string.Join(" | ", diagnostics));

        ScenarioUi.SelectDataDirectory(window, context.Data(DatasetKind.V5).DataDirectory);
        var template = context.Strings.Get("StatusTablesLoaded");
        ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(ScenarioUi.FormatPrefix(template), StringComparison.Ordinal),
            "список таблиц v5 загружен");

        var brokenRow = ScenarioUi.FindTableRow(window, "Lookup.AttachmentPoint")
            ?? throw new SmokeCheckException("Lookup.AttachmentPoint есть в списке v5", "строка не найдена");
        brokenRow.SelectItem();
        var malformed = context.Strings.Get("StatusMalformed");
        Ui.WaitUntil(
            () => window.WaitById("DetailsText").ReadName().Contains(malformed, StringComparison.Ordinal)
                || ReadDiagnostics(window).Count > 0,
            "детали повреждённой таблицы показывают ошибку");
        var details = window.WaitById("DetailsText").ReadName();
        var brokenDiagnostics = ReadDiagnostics(window);
        context.Recorder.Check(
            "Lookup.AttachmentPoint v5 помечен как повреждённый",
            details.Contains(malformed, StringComparison.Ordinal)
                || brokenDiagnostics.Any(line => line.Contains(malformed, StringComparison.Ordinal)),
            $"детали: «{details}»; диагностика: {string.Join(" | ", brokenDiagnostics)}");
        context.Recorder.Check(
            "Диагностика содержит код и сообщение",
            brokenDiagnostics.Count > 0,
            "строк диагностики: " + brokenDiagnostics.Count);
    }

    private static IReadOnlyList<string> ReadDiagnostics(AutomationElement window)
    {
        var list = window.FindById("DiagnosticsList");
        return list is null
            ? []
            : list.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
                .Select(static text => text.ReadName())
                .Where(static text => !string.IsNullOrWhiteSpace(text))
                .ToList();
    }
}
