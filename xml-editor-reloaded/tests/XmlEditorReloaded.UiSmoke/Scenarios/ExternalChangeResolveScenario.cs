using System.IO;
using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S47: Reload discards the unsaved edit and shows the external content, Overwrite writes the editor
/// content over the external file, and a clean tab is not interrupted but picks the file up through
/// the Reload button.
/// </summary>
internal sealed class ExternalChangeResolveScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S47";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V5];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V5;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v5 = context.Data(DatasetKind.V5);
        var file = v5.TableFile("Lookup/Cursor.xml");
        var window = context.Window;
        var grid = ExternalChangeUi.OpenCursorTable(context);

        // Reload: the external value replaces both the file and the editor content.
        DetailsUi.EditField(context, grid, 0, "name", "Smoke Reload Local");
        Ui.WaitUntil(() => window.WaitById("SaveButton").IsEnabled(), "вкладка стала грязной");
        ExternalChangeUi.EditFirstRecordName(file, "External Reload Value");
        var reloadExternalHash = SaveDirtyTabScenario.HashFile(file);
        window.WaitById("SaveButton").InvokeButton();
        var dialog = ExternalChangeUi.WaitForDialog(context);
        dialog.WaitById("ReloadButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "OverwriteButton", TimeSpan.FromSeconds(30));
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Format("ExternalChangeReloaded", ExternalChangeUi.Table),
            "статус перезагрузки с диска",
            TimeSpan.FromSeconds(30));
        grid = window.WaitById("RowsGrid");
        ExternalChangeUi.WaitForCell(grid, 0, 1, "External Reload Value", "сетка показывает внешнее значение");
        context.Recorder.Check(
            "«Перезагрузить» отбрасывает локальную правку и показывает внешнее содержимое",
            ExternalChangeUi.CellText(grid, 0, 1) == "External Reload Value",
            ExternalChangeUi.CellText(grid, 0, 1));
        context.Recorder.Check(
            "После перезагрузки сохранение недоступно",
            !window.WaitById("SaveButton").IsEnabled(),
            $"IsEnabled: {window.WaitById("SaveButton").IsEnabled()}");
        context.Recorder.Check(
            "После перезагрузки файл остаётся внешним",
            SaveDirtyTabScenario.HashFile(file) == reloadExternalHash);

        // Overwrite: the editor content wins over the external file.
        grid = window.WaitById("RowsGrid");
        DetailsUi.EditField(context, grid, 0, "name", "Smoke Overwrite Value");
        Ui.WaitUntil(() => window.WaitById("SaveButton").IsEnabled(), "вкладка снова стала грязной");
        ExternalChangeUi.EditFirstRecordName(file, "External Overwrite Value");
        var overwriteExternalHash = SaveDirtyTabScenario.HashFile(file);
        window.WaitById("SaveButton").InvokeButton();
        dialog = ExternalChangeUi.WaitForDialog(context);
        dialog.WaitById("OverwriteButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "OverwriteButton", TimeSpan.FromSeconds(30));
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Format("ExternalChangeOverwritten", ExternalChangeUi.Table),
            "статус перезаписи внешнего файла",
            TimeSpan.FromSeconds(30));
        var textAfterOverwrite = File.ReadAllText(file);
        context.Recorder.Check(
            "«Перезаписать» сохраняет содержимое редактора",
            textAfterOverwrite.Contains("Smoke Overwrite Value", StringComparison.Ordinal)
                && !textAfterOverwrite.Contains("External Overwrite Value", StringComparison.Ordinal),
            "значение редактора отсутствует в файле");
        context.Recorder.Check(
            "Перезапись отличается от внешнего содержимого",
            SaveDirtyTabScenario.HashFile(file) != overwriteExternalHash);
        context.Recorder.Check(
            "После перезаписи сохранение недоступно",
            !window.WaitById("SaveButton").IsEnabled(),
            $"IsEnabled: {window.WaitById("SaveButton").IsEnabled()}");

        // A clean tab is not interrupted; only the Reload button picks the external change up.
        ExternalChangeUi.EditFirstRecordName(file, "External Clean Value");
        var cleanExternalHash = SaveDirtyTabScenario.HashFile(file);
        Thread.Sleep(1500);
        context.Recorder.Check(
            "Чистая вкладка не показывает диалог автоматически",
            ScenarioUi.TopLevelWindowCount(context.App.ProcessId) == 1
                && ScenarioUi.IsWindowWithIdOpen(context.App, "OverwriteButton") is false,
            $"окон процесса: {ScenarioUi.TopLevelWindowCount(context.App.ProcessId)}");
        context.Recorder.Check(
            "До перезагрузки сетка показывает прежнее значение",
            ExternalChangeUi.CellText(grid, 0, 1) == "Smoke Overwrite Value",
            ExternalChangeUi.CellText(grid, 0, 1));

        window.WaitById("ReloadButton").InvokeButton();
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "перезагрузка чистой вкладки",
            TimeSpan.FromSeconds(30));
        grid = window.WaitById("RowsGrid");
        ExternalChangeUi.WaitForCell(grid, 0, 1, "External Clean Value", "кнопка «Перезагрузить» читает файл");
        context.Recorder.Check(
            "Кнопка «Перезагрузить» подхватывает внешнее содержимое",
            ExternalChangeUi.CellText(grid, 0, 1) == "External Clean Value",
            ExternalChangeUi.CellText(grid, 0, 1));
        context.Recorder.Check(
            "Внешний файл не изменился при чтении",
            SaveDirtyTabScenario.HashFile(file) == cleanExternalHash);

        context.ExpectChange(DatasetKind.V5, "TableData/Lookup/Cursor.xml");
    }
}
