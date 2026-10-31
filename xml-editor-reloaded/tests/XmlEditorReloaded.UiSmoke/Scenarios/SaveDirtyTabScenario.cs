using System.IO;
using System.Security.Cryptography;
using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S12: saving a dirty tab writes the file, while a tab without edits never rewrites it.
/// </summary>
internal sealed class SaveDirtyTabScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S12";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Lookup/Cursor.xml");
        var hashBefore = HashFile(file);

        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.Cursor открыт");

        var save = window.WaitById("SaveButton");
        var grid = window.WaitById("RowsGrid");
        context.Recorder.Check(
            "На чистой вкладке сохранение недоступно",
            !save.IsEnabled(),
            $"IsEnabled: {save.IsEnabled()}");

        grid.EditCell(0, 1, "Smoke Cursor Value");
        Ui.WaitUntil(() => save.IsEnabled(), "вкладка стала грязной");
        context.Recorder.Check("После правки сохранение доступно", true);

        var hashBeforeSave = HashFile(file);
        context.Recorder.Check(
            "Правка в памяти не пишет файл до сохранения",
            hashBeforeSave == hashBefore,
            null);

        save.InvokeButton();
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorSaved"),
            "статус «Сохранено.»",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check("Статус «Сохранено.»", true);

        var hashAfter = HashFile(file);
        context.Recorder.Check("Хеш файла изменился после сохранения", hashAfter != hashBefore);
        context.Recorder.Check(
            "Сохранение снимает признак несохранённых изменений",
            !save.IsEnabled(),
            $"IsEnabled: {save.IsEnabled()}");

        var document = XDocument.Load(file);
        var names = document.Descendants("name").Select(static element => element.Value).ToList();
        context.Recorder.Check(
            "Изменённое значение записано в файл",
            names.Contains("Smoke Cursor Value", StringComparer.Ordinal),
            null);
        context.Recorder.Check(
            "Число элементов name не изменилось",
            names.Count == 23,
            $"name: {names.Count}");

        context.ExpectChange(DatasetKind.V4, "TableData/Lookup/Cursor.xml");
    }

    /// <summary>
    /// Computes the SHA-256 hash of a file.
    /// </summary>
    /// <param name="path">File path.</param>
    /// <returns>Lowercase hexadecimal hash.</returns>
    public static string HashFile(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }
}
