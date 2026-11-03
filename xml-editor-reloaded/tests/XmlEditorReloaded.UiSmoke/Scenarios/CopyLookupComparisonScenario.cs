using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S91: a lookup copy whose source key is absent but whose display name already exists must use the
/// existing target row: Skip leaves it alone, Overwrite updates it instead of adding a second name, and
/// allocating a new key does not create a duplicate display name.
/// </summary>
internal sealed class CopyLookupComparisonScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S91";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override DatasetKind ActiveDataset => DatasetKind.V5;

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V5;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v4 = context.Data(DatasetKind.V4);
        var v5 = context.Data(DatasetKind.V5);
        var targetFile = v5.TableFile("Lookup/ItemClass.xml");
        var target = XDocument.Load(targetFile);
        var gun = target.Descendants("ItemClass")
            .First(element => element.Element("id")?.Value.Trim() == "2");
        gun.Element("id")!.Value = "20";
        target.Save(targetFile);
        context.ExpectChange(DatasetKind.V5, "TableData/Lookup/ItemClass.xml");
        var targetAfterEdit = SaveDirtyTabScenario.HashFile(targetFile);
        var rowCount = target.Descendants("ItemClass").Count();

        var dialog = DataOperationsUi.OpenCopyDialog(context);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopySourceCombo", v4);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopyTargetCombo", v5);
        DataOperationsUi.WaitCopyTablesLoaded(context, dialog);
        DataOperationsUi.SelectCopyTable(context, dialog, "Lookup.ItemClass");
        DataOperationsUi.SetCopyRecordKeys(dialog, "2");

        DataOperationsUi.SelectCopyPolicy(dialog, context.Strings.Get("CopyPolicySkip"));
        var skipSummary = DataOperationsUi.PreviewCopy(dialog);
        context.Recorder.Check(
            "«Пропускать существующие» находит строку по имени",
            skipSummary == context.Strings.Format("CopyPreviewSummary", 0, 0, 0, 1, 0),
            skipSummary);
        context.Recorder.Check(
            "Предпросмотр показывает существующий ключ 20",
            DataOperationsUi.PreviewCell(dialog, 0, 2).Trim() == "20",
            DataOperationsUi.PreviewCell(dialog, 0, 2).Trim());
        context.Recorder.Check(
            "Действие для найденной по имени строки — «Пропустить»",
            DataOperationsUi.PreviewCell(dialog, 0, 3).Trim() == context.Strings.Get("CopyActionSkip"),
            DataOperationsUi.PreviewCell(dialog, 0, 3).Trim());
        context.Recorder.Check(
            "Копирование недоступно, когда строка пропускается",
            !dialog.WaitById("CopyApplyButton").IsEnabled(),
            $"IsEnabled={dialog.WaitById("CopyApplyButton").IsEnabled()}");
        context.Recorder.Check(
            "Skip не изменил файл",
            SaveDirtyTabScenario.HashFile(targetFile) == targetAfterEdit,
            null);

        DataOperationsUi.SelectCopyPolicy(dialog, context.Strings.Get("CopyPolicyOverwrite"));
        var overwriteSummary = DataOperationsUi.PreviewCopy(dialog);
        context.Recorder.Check(
            "«Перезаписывать существующие» планирует одну перезапись",
            overwriteSummary == context.Strings.Format("CopyPreviewSummary", 0, 1, 0, 0, 0),
            overwriteSummary);
        context.Recorder.Check(
            "Перезапись адресована строке 20",
            DataOperationsUi.PreviewCell(dialog, 0, 2).Trim() == "20"
                && DataOperationsUi.PreviewCell(dialog, 0, 3).Trim() == context.Strings.Get("CopyActionOverwrite"),
            DataOperationsUi.PreviewCell(dialog, 0, 2).Trim() + " / "
                + DataOperationsUi.PreviewCell(dialog, 0, 3).Trim());
        DataOperationsUi.ApplyCopy(context, dialog);
        context.App.RefreshWindow();

        var afterOverwrite = XDocument.Load(targetFile);
        context.Recorder.Check(
            "После перезаписи осталась одна строка Gun",
            afterOverwrite.Descendants("ItemClass")
                .Count(element => element.Element("name")?.Value == "Gun") == 1,
            null);
        context.Recorder.Check(
            "Строка Gun сохранила ключ 20",
            afterOverwrite.Descendants("ItemClass")
                .First(element => element.Element("name")?.Value == "Gun")
                .Element("id")?.Value.Trim() == "20",
            afterOverwrite.Descendants("ItemClass")
                .First(element => element.Element("name")?.Value == "Gun")
                .Element("id")?.Value.Trim());
        context.Recorder.Check(
            "Второй Gun с ключом 2 не появился",
            afterOverwrite.Descendants("ItemClass")
                .All(element => element.Element("id")?.Value.Trim() != "2"),
            null);
        context.Recorder.Check(
            "Число строк ItemClass не изменилось",
            afterOverwrite.Descendants("ItemClass").Count() == rowCount,
            $"{rowCount} → {afterOverwrite.Descendants("ItemClass").Count()}");

        dialog = DataOperationsUi.OpenCopyDialog(context);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopySourceCombo", v4);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopyTargetCombo", v5);
        DataOperationsUi.WaitCopyTablesLoaded(context, dialog);
        DataOperationsUi.SelectCopyTable(context, dialog, "Lookup.ItemClass");
        DataOperationsUi.SetCopyRecordKeys(dialog, "2");
        DataOperationsUi.SelectCopyPolicy(dialog, context.Strings.Get("CopyPolicySkip"));
        DataOperationsUi.SelectCopyKeyMode(dialog, context.Strings.Get("CopyKeyAllocate"));
        var allocateSummary = DataOperationsUi.PreviewCopy(dialog);
        context.Recorder.Check(
            "«Выделять новые ключи» тоже пропускает существующее имя",
            allocateSummary == context.Strings.Format("CopyPreviewSummary", 0, 0, 0, 1, 0),
            allocateSummary);
        context.Recorder.Check(
            "Allocate адресован существующей строке 20",
            DataOperationsUi.PreviewCell(dialog, 0, 2).Trim() == "20"
                && DataOperationsUi.PreviewCell(dialog, 0, 3).Trim() == context.Strings.Get("CopyActionSkip"),
            DataOperationsUi.PreviewCell(dialog, 0, 2).Trim() + " / "
                + DataOperationsUi.PreviewCell(dialog, 0, 3).Trim());
        DataOperationsUi.CloseCopyDialog(context, dialog);
        context.Recorder.Check(
            "Allocate не создаёт дубликат имени",
            XDocument.Load(targetFile).Descendants("ItemClass")
                .Count(element => element.Element("name")?.Value == "Gun") == 1,
            null);
    }
}
