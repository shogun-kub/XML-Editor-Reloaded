using System.IO;
using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S36: the v4 AttachmentPoint lookup is editable and saved on the writable copy, while the malformed
/// v5 file is only diagnosed, cannot be saved, and is never rewritten.
/// </summary>
internal sealed class LookupDiagnosticsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S36";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var v4File = context.Data(DatasetKind.V4).TableFile("Lookup/AttachmentPoint.xml");
        var v5File = context.Data(DatasetKind.V5).TableFile("Lookup/AttachmentPoint.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(v5File);
        var before = XDocument.Parse(File.ReadAllText(v4File));

        ScenarioUi.OpenTable(context, "Lookup.AttachmentPoint");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "v4-справочник AttachmentPoint открыт",
            TimeSpan.FromSeconds(30));

        var grid = window.WaitById("RowsGrid");
        var rows = grid.RowCount();
        context.Recorder.Check("v4-справочник содержит 8 записей", rows == 8, $"строк: {rows}");

        var row = DataOperationsUi.FindRowIndex(grid, "1");
        var original = WorkspaceUi.ReadCell(grid, row, 1).Trim();
        context.Recorder.Check("Строка id=1 называется Small Molle", original == "Small Molle", original);

        var save = window.WaitById("SaveButton");
        DataOperationsUi.EditCellUntilDirty(window, grid, row, 1, "Small Molle Smoke");
        save.InvokeButton();
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorSaved"),
            "v4-справочник сохранён",
            TimeSpan.FromSeconds(30));

        var after = XDocument.Load(v4File);
        var (onlyField, details) = DataOperationsUi.OnlyFieldChanged(
            before,
            after,
            recordIndex: row,
            field: "name",
            expectedOld: "Small Molle",
            expectedNew: "Small Molle Smoke");
        context.Recorder.Check("Правка v4 затрагивает только имя записи id=1", onlyField, details);
        context.Recorder.Check(
            "Число записей v4-справочника не изменилось",
            after.Root!.Elements().Count() == 8,
            $"записей: {after.Root!.Elements().Count()}");
        context.ExpectChange(DatasetKind.V4, "TableData/Lookup/AttachmentPoint.xml");

        // The v5 file has a mismatched tag; it must stay untouched and read-only.
        ScenarioUi.SelectDataDirectory(window, context.Data(DatasetKind.V5).DataDirectory);
        var loaded = ScenarioUi.FormatPrefix(context.Strings.Get("StatusTablesLoaded"));
        ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(loaded, StringComparison.Ordinal),
            "список v5 загружен");

        ScenarioUi.OpenTable(context, "Lookup.AttachmentPoint");
        var malformed = context.Strings.Get("StatusMalformed");
        var errorPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("StatusError"));
        bool ReportsMalformed(string text) =>
            text.Contains(malformed, StringComparison.Ordinal)
            || text.Contains(errorPrefix, StringComparison.Ordinal);
        var status = ScenarioUi.WaitForEditorStatus(
            context,
            ReportsMalformed,
            "v5-файл диагностирован как повреждённый",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check("v5-файл диагностируется, а не открывается на правку", ReportsMalformed(status), status);

        var v5Save = window.WaitById("SaveButton");
        context.Recorder.Check(
            "Сохранение повреждённого v5-справочника недоступно",
            !v5Save.IsEnabled(),
            $"IsEnabled: {v5Save.IsEnabled()}");
        var v5Rows = window.WaitById("RowsGrid").RowCount();
        context.Recorder.Check("Повреждённый v5-справочник не показывает строк", v5Rows == 0, $"строк: {v5Rows}");

        var diagnostics = window.WaitById("DiagnosticsList")
            .FindAll(new OrCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem)))
            .Select(static item => string.IsNullOrWhiteSpace(item.ReadName()) ? Ui.ItemText(item) : item.ReadName())
            .Where(static text => !string.IsNullOrWhiteSpace(text))
            .ToList();
        context.Recorder.Check(
            "Диагностика называет ошибку разбора",
            diagnostics.Any(text => text.Contains("xml.parse_error", StringComparison.Ordinal)),
            diagnostics.Count == 0 ? "диагностика пуста" : string.Join(" | ", diagnostics));

        context.Recorder.Check("Повреждённый v5-файл не перезаписан", SaveDirtyTabScenario.HashFile(v5File) == hashBefore);
        context.Recorder.Check(
            "Рядом с повреждённым файлом не появилось копий",
            ScenarioUi.BackupCopies(v5File).Count == 0,
            string.Join(", ", ScenarioUi.BackupCopies(v5File)));
    }
}
