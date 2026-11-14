using System.IO;
using System.Text.RegularExpressions;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S92: the session log has one header and structured UTC entries; a failed save writes an error with its
/// stack; reaching 4 MiB rotates the file to <c>*.1</c>; a failed save leaves the data file unchanged.
/// </summary>
internal sealed class OperationLogScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S92";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V5];

    /// <inheritdoc />
    public override DatasetKind ActiveDataset => DatasetKind.V5;

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V5;

    private static readonly Regex EntryPattern = new(
        @"^\[\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d+(?:Z|[+-]\d{2}:\d{2})\] \[(INFO|WARNING|ERROR|FATAL)\] \S+",
        RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(2));

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var data = context.Data(DatasetKind.V5);
        var file = data.TableFile("Lookup/Cursor.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(file);
        var logPath = Path.Combine(context.App.AppDataDirectory, "XmlEditorReloaded.log");

        Ui.WaitUntil(() => File.Exists(logPath), "файл журнала создан", TimeSpan.FromSeconds(20));
        var header = File.ReadAllLines(logPath);
        context.Recorder.Check(
            "Журнал содержит один заголовок сессии",
            header.Count(line => line.StartsWith("=== XML Editor Reloaded ", StringComparison.Ordinal)) == 1,
            string.Join(" | ", header.Take(4)));
        context.Recorder.Check(
            "Заголовок содержит версию, пользователя, ОС, runtime и память",
            header.Any(line => line.StartsWith("User: ", StringComparison.Ordinal))
                && header.Any(line => line.StartsWith("OS: ", StringComparison.Ordinal))
                && header.Any(line => line.StartsWith("Runtime: ", StringComparison.Ordinal))
                && header.Any(line => line.StartsWith("Process: ", StringComparison.Ordinal))
                && header.Any(line => line.StartsWith("Working set: ", StringComparison.Ordinal))
                && header.Any(line => line.StartsWith("Managed memory: ", StringComparison.Ordinal)),
            string.Join(" | ", header.Take(8)));

        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.Cursor открыт");
        var grid = window.WaitById("RowsGrid");
        DetailsUi.EditField(context, grid, 0, "name", "Log Test");
        TriggerReadOnlySaveFailure(context, file, "Log Test 2");
        context.Recorder.Check(
            "Неудачное сохранение не изменило файл",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);

        var afterError = File.ReadAllText(logPath);
        var errorLine = afterError
            .Split('\n')
            .Select(static line => line.TrimEnd('\r'))
            .FirstOrDefault(line => EntryPattern.IsMatch(line) && line.Contains("[ERROR]", StringComparison.Ordinal));
        context.Recorder.Check(
            "Ошибка записана с UTC-временем, уровнем и источником",
            errorLine is not null && errorLine.Contains("TableFileSaver", StringComparison.Ordinal),
            errorLine ?? "ERROR-запись не найдена");
        context.Recorder.Check(
            "Ошибка содержит код и стек",
            errorLine is not null
                && errorLine.Contains("io.", StringComparison.Ordinal)
                && afterError.Contains("TableFileSaver", StringComparison.Ordinal)
                && afterError.Contains("   at ", StringComparison.Ordinal),
            null);

        File.AppendAllText(logPath, new string('x', 4 * 1024 * 1024));
        TriggerReadOnlySaveFailure(context, file, "Log Test 3");
        var rotated = logPath + ".1";
        context.Recorder.Check(
            "При 4 MiB появляется файл .1",
            File.Exists(rotated),
            rotated);
        context.Recorder.Check(
            "Новый журнал начинается с нового заголовка сессии",
            File.Exists(logPath)
                && File.ReadLines(logPath).FirstOrDefault()?.StartsWith(
                    "=== XML Editor Reloaded ",
                    StringComparison.Ordinal) == true,
            File.Exists(logPath) ? File.ReadLines(logPath).FirstOrDefault() : "журнал отсутствует");
        context.Recorder.Check(
            "Ротация не изменила игровой файл",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);
        context.Recorder.Note(
            "Фатальный MessageBox с путём к журналу и запись стека при старте отдельно не воспроизводились; "
            + "они покрыты unit-тестами FileOperationLog и App.");
    }

    private static void TriggerReadOnlySaveFailure(ScenarioContext context, string file, string value)
    {
        var window = context.Window;
        var grid = window.WaitById("RowsGrid");
        DetailsUi.EditField(context, grid, 0, "name", value);
        File.SetAttributes(file, FileAttributes.ReadOnly);
        try
        {
            window.WaitById("SaveButton").InvokeButton();
            var prefix = ScenarioUi.FormatPrefix(context.Strings.Get("StatusError"));
            ScenarioUi.WaitForEditorStatus(
                context,
                text => text.Contains(prefix, StringComparison.Ordinal),
                "сохранение в файл только для чтения отклонено",
                TimeSpan.FromSeconds(30));
        }
        finally
        {
            File.SetAttributes(file, FileAttributes.Normal);
        }
    }
}
