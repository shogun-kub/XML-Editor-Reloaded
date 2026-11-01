using System.IO;
using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S42: the comparison window exports its report as Markdown and as CSV through the native save dialog,
/// and neither export writes into the compared data sets.
/// </summary>
internal sealed class CompareExportScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S42";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v4 = context.Data(DatasetKind.V4);
        var v5 = context.Data(DatasetKind.V5);
        var leftHash = DataOperationsUi.HashTree(v4.TableDataDirectory);
        var rightHash = DataOperationsUi.HashTree(v5.TableDataDirectory);

        var window = DataOperationsUi.OpenCompareWindow(context);
        var export = window.WaitById("CompareExportButton");
        context.Recorder.Check(
            "До сравнения экспорт недоступен",
            !export.IsEnabled(),
            $"IsEnabled: {export.IsEnabled()}");

        DataOperationsUi.SetCompareDirectories(window, v4, v5);
        DataOperationsUi.RunCompare(context, window);
        Ui.WaitUntil(() => export.IsEnabled(), "экспорт доступен после сравнения");

        var markdownPath = Path.Combine(context.Directory, "comparison-report.md");
        Export(context, window, markdownPath);
        VerifyMarkdown(context, markdownPath);

        var csvPath = Path.Combine(context.Directory, "comparison-report.csv");
        Export(context, window, csvPath);
        VerifyCsv(context, csvPath);

        context.Recorder.Check(
            "Вкладка сравнения не изменила данные v4",
            DataOperationsUi.HashTree(v4.TableDataDirectory) == leftHash);
        context.Recorder.Check(
            "Вкладка сравнения не изменила данные v5",
            DataOperationsUi.HashTree(v5.TableDataDirectory) == rightHash);

        DataOperationsUi.CloseCompareWindow(context, window);
    }

    private static void Export(ScenarioContext context, AutomationElement window, string path)
    {
        var export = window.WaitById("CompareExportButton");
        _ = Task.Run(() =>
        {
            try
            {
                export.InvokeButton();
            }
            catch (Exception exception) when (
                exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The command waits for the native save dialog; the dialog is the visible result.
            }
        });

        var dialog = NativeDialogUi.WaitForDialog(
            context.App,
            context.Strings.Get("CompareExportTitle"),
            "диалог сохранения отчёта",
            TimeSpan.FromSeconds(60));
        NativeDialogUi.AcceptPath(dialog, path);

        var prefix = ScenarioUi.FormatPrefix(context.Strings.Get("CompareExportedText"));
        var status = string.Empty;
        Ui.WaitUntil(
            () =>
            {
                status = window.WaitById("CompareStatusText").ReadName();
                return status.StartsWith(prefix, StringComparison.Ordinal)
                    && status.Contains(Path.GetFileName(path), StringComparison.Ordinal);
            },
            "статус экспорта отчёта",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check(
            $"Отчёт «{Path.GetFileName(path)}» записан и подтверждён статусом",
            File.Exists(path) && new FileInfo(path).Length > 0,
            status);
    }

    private static void VerifyMarkdown(ScenarioContext context, string path)
    {
        var text = File.ReadAllText(path);
        context.Recorder.Check(
            "Markdown-отчёт содержит заголовок и сводку",
            text.Contains("# TableData comparison report", StringComparison.Ordinal)
                && text.Contains("## Summary", StringComparison.Ordinal)
                && text.Contains("Common tables", StringComparison.Ordinal),
            $"символов: {text.Length}");
        context.Recorder.Check(
            "Markdown-отчёт содержит таблицы и их статусы",
            text.Contains("Items.Items", StringComparison.Ordinal)
                && text.Contains("Lookup.AttachmentPoint", StringComparison.Ordinal)
                && text.Contains("malformed", StringComparison.OrdinalIgnoreCase),
            text.Length > 200 ? text[..200] : text);
        context.Recorder.Check(
            "Markdown-отчёт непустой",
            text.Length > 1000,
            $"символов: {text.Length}");
    }

    private static void VerifyCsv(ScenarioContext context, string path)
    {
        var lines = File.ReadAllLines(path);
        context.Recorder.Check(
            "CSV-отчёт начинается с заголовка колонок",
            lines.Length > 0
                && lines[0].StartsWith(
                    "RecordType,Table,Name,Status,Left,Right,LeftDetail,RightDetail",
                    StringComparison.Ordinal),
            lines.Length == 0 ? "файл пуст" : lines[0]);
        context.Recorder.Check(
            "CSV-отчёт содержит строки таблиц",
            lines.Length > 30
                && lines.Any(line => line.StartsWith("Table,Items.Items,", StringComparison.Ordinal))
                && lines.Any(line => line.Contains("Lookup.AttachmentPoint", StringComparison.Ordinal)),
            $"строк: {lines.Length}");
    }
}
