using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S49: the malformed v5 <c>Lookup.AttachmentPoint</c> is reported as malformed in the comparison and
/// makes the copy preview fail with a parse error instead of planning a write.
/// </summary>
internal sealed class MalformedCopyScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S49";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v4 = context.Data(DatasetKind.V4);
        var v5 = context.Data(DatasetKind.V5);
        var leftHash = DataOperationsUi.HashTree(v4.TableDataDirectory);
        var rightHash = DataOperationsUi.HashTree(v5.TableDataDirectory);

        var compare = DataOperationsUi.OpenCompareWindow(context);
        DataOperationsUi.SetCompareDirectories(compare, v4, v5);
        DataOperationsUi.RunCompare(context, compare);
        var malformed = ScenarioUi.FindTableRow(compare, "Lookup.AttachmentPoint");
        context.Recorder.Check("Повреждённая таблица есть в списке сравнения", malformed is not null);
        if (malformed is not null)
        {
            var cells = malformed.ReadRowCells();
            context.Recorder.Check(
                "Сравнение помечает v5-таблицу как повреждённую",
                cells.ElementAtOrDefault(1) == context.Strings.Get("CompareStatusMalformed"),
                string.Join(" | ", cells));
        }

        DataOperationsUi.CloseCompareWindow(context, compare);

        var dialog = DataOperationsUi.OpenCopyDialog(context);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopySourceCombo", v4);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopyTargetCombo", v5);
        DataOperationsUi.WaitCopyTablesLoaded(context, dialog);
        DataOperationsUi.SelectCopyTable(context, dialog, "Lookup.AttachmentPoint");

        dialog.WaitById("CopyPreviewButton").InvokeButton();
        var errorPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("StatusError"));
        var status = string.Empty;
        Ui.WaitUntil(
            () =>
            {
                status = dialog.WaitById("CopyStatusText").ReadName();
                return status.StartsWith(errorPrefix, StringComparison.Ordinal);
            },
            "предпросмотр копирования отказал",
            TimeSpan.FromSeconds(90));
        context.Recorder.Check(
            "Предпросмотр сообщает об ошибке разбора повреждённой таблицы",
            status.Contains("xml.parse_error", StringComparison.Ordinal),
            status);
        context.Recorder.Check(
            "Предпросмотр не построен",
            dialog.WaitById("CopySummaryText").ReadName().Length == 0
                && dialog.WaitById("PreviewGrid").RowCount() == 0,
            dialog.WaitById("CopySummaryText").ReadName());
        context.Recorder.Check(
            "Копирование не предлагается",
            !dialog.WaitById("CopyApplyButton").IsEnabled(),
            $"IsEnabled: {dialog.WaitById("CopyApplyButton").IsEnabled()}");

        DataOperationsUi.CloseCopyDialog(context, dialog);

        context.Recorder.Check(
            "Сравнение и предпросмотр не изменили данные v4",
            DataOperationsUi.HashTree(v4.TableDataDirectory) == leftHash);
        context.Recorder.Check(
            "Сравнение и предпросмотр не изменили данные v5",
            DataOperationsUi.HashTree(v5.TableDataDirectory) == rightHash);
    }
}
