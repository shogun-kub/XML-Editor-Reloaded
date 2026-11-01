using System.IO;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S37: copying one record from the v4 copy to the v5 copy adds exactly that record, leaves the source
/// untouched, and the copied item is visible in the editor afterwards.
/// </summary>
internal sealed class CopyRecordScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S37";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => true;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v4 = context.Data(DatasetKind.V4);
        var v5 = context.Data(DatasetKind.V5);
        var sourceFile = v4.TableFile("Items/Items.xml");
        var targetFile = v5.TableFile("Items/Items.xml");
        var sourceHash = SaveDirtyTabScenario.HashFile(sourceFile);

        var dialog = DataOperationsUi.OpenCopyDialog(context);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopySourceCombo", v4);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopyTargetCombo", v5);
        DataOperationsUi.WaitCopyTablesLoaded(context, dialog);
        DataOperationsUi.SelectCopyTable(context, dialog, "Items.Items");
        DataOperationsUi.SetCopyRecordKeys(dialog, "1775");
        DataOperationsUi.SelectCopyPolicy(dialog, context.Strings.Get("CopyPolicySkip"));
        DataOperationsUi.SelectCopyKeyMode(dialog, context.Strings.Get("CopyKeyPreserve"));
        DataOperationsUi.SetCopyRelated(dialog, false);

        var summary = DataOperationsUi.PreviewCopy(dialog);
        var expected = context.Strings.Format("CopyPreviewSummary", 1, 0, 0, 0, 0);
        context.Recorder.Check("Предпросмотр планирует добавить одну запись", summary == expected, summary);

        var key = DataOperationsUi.PreviewCell(dialog, 0, 0).Trim();
        context.Recorder.Check("Ключ записи в предпросмотре — 1775", key == "1775", key);
        var action = DataOperationsUi.PreviewCell(dialog, 0, 3).Trim();
        context.Recorder.Check(
            "Действие — «Добавить»",
            action == context.Strings.Get("CopyActionAdd"),
            action);
        var targetKey = DataOperationsUi.PreviewCell(dialog, 0, 2).Trim();
        context.Recorder.Check("Ключ в приёмнике сохраняется", targetKey == "1775", targetKey);
        var display = DataOperationsUi.PreviewCell(dialog, 0, 1).Trim();
        context.Recorder.Check(
            "Показ записи содержит имя предмета",
            display.Contains("M82A1", StringComparison.Ordinal),
            display);
        var related = DataOperationsUi.PreviewCell(dialog, 0, 5).Trim();
        context.Recorder.Check("Связанные строки не запланированы", related.Length == 0, related);

        DataOperationsUi.ApplyCopy(context, dialog);
        var resultPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("CopyResultSummary"));
        var status = ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(resultPrefix, StringComparison.Ordinal),
            "итог копирования в строке состояния",
            TimeSpan.FromSeconds(60));
        var expectedResult = context.Strings.Format("CopyResultSummary", 1, 0, 0);
        context.Recorder.Check("Строка состояния сообщает о добавленной записи", status == expectedResult, status);

        context.Recorder.Check(
            "Исходный файл v4 не изменился",
            SaveDirtyTabScenario.HashFile(sourceFile) == sourceHash);

        var target = XDocument.Load(targetFile);
        var copied = target.Root!.Elements()
            .FirstOrDefault(element => element.Element("uiIndex")?.Value.Trim() == "1775");
        context.Recorder.Check("Запись 1775 записана в v5", copied is not null);
        context.Recorder.Check(
            "Имя скопированного предмета — M82A1",
            copied?.Element("szItemName")?.Value.Trim() == "M82A1",
            copied?.Element("szItemName")?.Value ?? "записи нет");
        context.Recorder.Check(
            "Число записей v5 увеличилось на одну",
            target.Root!.Elements().Count() == 1776,
            $"записей: {target.Root!.Elements().Count()}");
        context.ExpectChange(DatasetKind.V5, "TableData/Items/Items.xml");

        // The copied item must be visible in the editor of the target data set.
        ScenarioUi.SelectDataDirectory(context.Window, v5.DataDirectory);
        var loaded = ScenarioUi.FormatPrefix(context.Strings.Get("StatusTablesLoaded"));
        ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(loaded, StringComparison.Ordinal),
            "список v5 загружен");
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1775", "M82A1");
    }
}
