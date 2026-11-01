using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S38: copying a new record with related rows enabled plans the sub-table and child rows, writes them
/// to the target data set together with the record, and explains why related rows of an existing record
/// are not copied.
/// </summary>
internal sealed class CopyRelatedScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S38";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => true;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        const string sourceKey = "1775";
        var v4 = context.Data(DatasetKind.V4);
        var v5 = context.Data(DatasetKind.V5);
        var sourceHash = SaveDirtyTabScenario.HashFile(v4.TableFile("Items/Items.xml"));
        var sourceAttachmentsHash = SaveDirtyTabScenario.HashFile(v4.TableFile("Items/Attachments.xml"));

        var dialog = DataOperationsUi.OpenCopyDialog(context);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopySourceCombo", v4);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopyTargetCombo", v5);
        DataOperationsUi.WaitCopyTablesLoaded(context, dialog);
        DataOperationsUi.SelectCopyTable(context, dialog, "Items.Items");
        DataOperationsUi.SetCopyRecordKeys(dialog, sourceKey);
        DataOperationsUi.SelectCopyPolicy(dialog, context.Strings.Get("CopyPolicyMerge"));
        DataOperationsUi.SetCopyRelated(dialog, true);

        var summary = DataOperationsUi.PreviewCopy(dialog);
        var expected = context.Strings.Format("CopyPreviewSummary", 1, 0, 0, 0, 6);
        context.Recorder.Check(
            "Предпросмотр планирует запись и шесть связанных строк",
            summary == expected,
            summary);

        var related = DataOperationsUi.PreviewCell(dialog, 0, 5).Trim();
        context.Recorder.Check(
            "Колонка связанных строк перечисляет подтаблицы",
            related.Contains("Items.Attachments: +5", StringComparison.Ordinal)
            && related.Contains("Items.Weapons: +1", StringComparison.Ordinal),
            related);

        DataOperationsUi.ApplyCopy(context, dialog);
        var resultPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("CopyResultSummary"));
        ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(resultPrefix, StringComparison.Ordinal),
            "итог копирования со связанными строками",
            TimeSpan.FromSeconds(60));

        var items = XDocument.Load(v5.TableFile("Items/Items.xml"));
        context.Recorder.Check(
            "Запись 1775 записана в v5",
            items.Root!.Elements().Any(element => element.Element("uiIndex")?.Value.Trim() == sourceKey));

        var attachments = XDocument.Load(v5.TableFile("Items/Attachments.xml"));
        var copiedAttachments = attachments.Root!.Elements()
            .Where(element => element.Element("itemIndex")?.Value.Trim() == sourceKey)
            .ToList();
        context.Recorder.Check(
            "В v5 скопированы пять вложений предмета",
            copiedAttachments.Count == 5,
            $"строк: {copiedAttachments.Count}");

        var weapons = XDocument.Load(v5.TableFile("Items/Weapons.xml"));
        context.Recorder.Check(
            "В v5 скопирована подтаблица оружия",
            weapons.Root!.Elements().Any(element => element.Element("uiIndex")?.Value.Trim() == sourceKey));

        context.Recorder.Check(
            "Исходный Items.xml v4 не изменился",
            SaveDirtyTabScenario.HashFile(v4.TableFile("Items/Items.xml")) == sourceHash);
        context.Recorder.Check(
            "Исходный Attachments.xml v4 не изменился",
            SaveDirtyTabScenario.HashFile(v4.TableFile("Items/Attachments.xml")) == sourceAttachmentsHash);

        context.ExpectChange(DatasetKind.V5, "TableData/Items/Items.xml");
        context.ExpectChange(DatasetKind.V5, "TableData/Items/Attachments.xml");
        context.ExpectChange(DatasetKind.V5, "TableData/Items/Weapons.xml");

        // An existing record is merged, but its child rows are intentionally not copied; the preview
        // must say so instead of silently duplicating attachments.
        var existingDialog = DataOperationsUi.OpenCopyDialog(context);
        DataOperationsUi.SelectCopyDirectory(existingDialog, "CopySourceCombo", v4);
        DataOperationsUi.SelectCopyDirectory(existingDialog, "CopyTargetCombo", v5);
        DataOperationsUi.WaitCopyTablesLoaded(context, existingDialog);
        DataOperationsUi.SelectCopyTable(context, existingDialog, "Items.Items");
        DataOperationsUi.SetCopyRecordKeys(existingDialog, "12");
        DataOperationsUi.SelectCopyPolicy(existingDialog, context.Strings.Get("CopyPolicyMerge"));
        DataOperationsUi.SetCopyRelated(existingDialog, true);
        var existingSummary = DataOperationsUi.PreviewCopy(existingDialog);
        var expectedExisting = context.Strings.Format("CopyPreviewSummary", 0, 0, 1, 0, 1);
        context.Recorder.Check(
            "Существующая запись планируется как «дополнить» с одной связанной строкой",
            existingSummary == expectedExisting,
            existingSummary);
        var existingRelated = DataOperationsUi.PreviewCell(existingDialog, 0, 5).Trim();
        context.Recorder.Check(
            "Вложения существующей записи пропускаются, а не дублируются",
            existingRelated.Contains("Items.Attachments: +0 ~0 / ", StringComparison.Ordinal),
            existingRelated);
        var existingAction = DataOperationsUi.PreviewCell(existingDialog, 0, 3).Trim();
        context.Recorder.Check(
            "Действие для существующей записи — «Дополнить»",
            existingAction == context.Strings.Get("CopyActionMerge"),
            existingAction);
        DataOperationsUi.CloseCopyDialog(context, existingDialog);
        context.Recorder.Note(
            "Связанные строки копируются только вместе с новой записью: для существующей записи "
            + "предпросмотр пропускает вложения и подтаблицы, поэтому они не дублируются, а политика "
            + "«дополнить» относится только к строкам подтаблиц.");
    }
}
