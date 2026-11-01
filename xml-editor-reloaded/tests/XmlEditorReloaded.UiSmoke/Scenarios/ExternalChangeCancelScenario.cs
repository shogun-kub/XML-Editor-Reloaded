using System.IO;
using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S46: when the file changed on disk, saving asks how to resolve the conflict; Cancel keeps the tab
/// dirty and the external content on disk, and the editor writes nothing.
/// </summary>
internal sealed class ExternalChangeCancelScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S46";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V5];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V5;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v5 = context.Data(DatasetKind.V5);
        var file = v5.TableFile("Lookup/Cursor.xml");
        const string LocalValue = "Smoke External Cancel";

        var window = context.Window;
        var grid = ExternalChangeUi.OpenCursorTable(context);
        DetailsUi.EditField(context, grid, 0, "name", LocalValue);
        Ui.WaitUntil(() => window.WaitById("SaveButton").IsEnabled(), "вкладка стала грязной");
        ExternalChangeUi.WaitForCell(grid, 0, 1, LocalValue, "локальная правка видна в сетке");

        ExternalChangeUi.EditFirstRecordName(file, "External Cancel Value");
        var externalHash = SaveDirtyTabScenario.HashFile(file);

        window.WaitById("SaveButton").InvokeButton();
        var dialog = ExternalChangeUi.WaitForDialog(context);
        var message = dialog.WaitById("MessageText").ReadName();
        context.Recorder.Check(
            "Диалог называет изменённый файл",
            message.Contains(ExternalChangeUi.Table, StringComparison.Ordinal),
            message);
        context.Recorder.Check(
            "Диалог объясняет внешнее изменение",
            message.Contains(ExternalChangeUi.ExpectedMessageFragment(context), StringComparison.Ordinal),
            message);
        context.Recorder.Check(
            "Диалог предлагает «Перезагрузить», «Перезаписать» и «Отмена»",
            dialog.WaitById("ReloadButton").ReadName() == context.Strings.Get("ExternalChangeReload")
                && dialog.WaitById("OverwriteButton").ReadName() == context.Strings.Get("ExternalChangeOverwrite")
                && dialog.WaitById("CancelButton").ReadName() == context.Strings.Get("ExternalChangeCancel"),
            string.Join(
                " | ",
                dialog.WaitById("ReloadButton").ReadName(),
                dialog.WaitById("OverwriteButton").ReadName(),
                dialog.WaitById("CancelButton").ReadName()));

        dialog.WaitById("CancelButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "OverwriteButton", TimeSpan.FromSeconds(30));
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Format("ExternalChangeCancelled", ExternalChangeUi.Table),
            "статус отмены сохранения",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check("Отмена сообщает об отказе от сохранения", true);
        context.Recorder.Check(
            "Вкладка осталась грязной",
            window.WaitById("SaveButton").IsEnabled(),
            $"IsEnabled: {window.WaitById("SaveButton").IsEnabled()}");
        context.Recorder.Check(
            "На диске осталось внешнее содержимое",
            SaveDirtyTabScenario.HashFile(file) == externalHash);
        context.Recorder.Check(
            "Локальная правка не потеряна",
            ExternalChangeUi.CellText(grid, 0, 1) == LocalValue,
            ExternalChangeUi.CellText(grid, 0, 1));
        context.Recorder.Check(
            "Отмена не создала копий рядом с файлом",
            ScenarioUi.BackupCopies(file).Count == 0,
            string.Join(", ", ScenarioUi.BackupCopies(file)));

        context.ExpectChange(DatasetKind.V5, "TableData/Lookup/Cursor.xml");

        // Close the dirty tab without writing so the process can exit cleanly.
        ScenarioUi.CloseTab(window, ExternalChangeUi.Table);
        var unsaved = ScenarioUi.WaitForWindowWithId(
            context.App,
            "DiscardButton",
            "запрос о несохранённых изменениях",
            TimeSpan.FromSeconds(30));
        unsaved.WaitById("DiscardButton").InvokeButton();
        ScenarioUi.WaitForTabClosed(window, ExternalChangeUi.Table, TimeSpan.FromSeconds(30));
        context.Recorder.Check(
            "Отмена не мешает закрыть вкладку без записи",
            File.Exists(file) && SaveDirtyTabScenario.HashFile(file) == externalHash);
    }
}
