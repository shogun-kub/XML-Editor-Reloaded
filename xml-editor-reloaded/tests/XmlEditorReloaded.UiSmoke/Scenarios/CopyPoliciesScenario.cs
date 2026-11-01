using System.IO;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S39: the copy dialog maps every conflict policy to the matching preview action, enables key
/// allocation only for simple keys, reports ambiguous references instead of writing them silently, and
/// reports target tabs that were not reloaded because they hold unsaved edits.
/// </summary>
internal sealed class CopyPoliciesScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S39";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => true;

    /// <inheritdoc />
    public override DatasetKind ActiveDataset => DatasetKind.V5;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var v4 = context.Data(DatasetKind.V4);
        var v5 = context.Data(DatasetKind.V5);
        var targetFile = v5.TableFile("Items/Weapons.xml");
        var targetHash = SaveDirtyTabScenario.HashFile(targetFile);

        // A dirty tab in the target data set must survive the copy and be reported.
        ScenarioUi.OpenTable(context, "Items.Weapons");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "целевая таблица Items.Weapons v5 открыта",
            TimeSpan.FromSeconds(60));
        DataOperationsUi.EditCellUntilDirty(window, window.WaitById("RowsGrid"), 0, 1, "Policies Smoke");

        var dialog = DataOperationsUi.OpenCopyDialog(context);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopySourceCombo", v4);
        DataOperationsUi.SelectCopyDirectory(dialog, "CopyTargetCombo", v5);
        DataOperationsUi.WaitCopyTablesLoaded(context, dialog);
        DataOperationsUi.SelectCopyTable(context, dialog, "Items.Weapons");
        DataOperationsUi.SetCopyRecordKeys(dialog, "12");

        DataOperationsUi.SelectCopyPolicy(dialog, context.Strings.Get("CopyPolicySkip"));
        var skipSummary = DataOperationsUi.PreviewCopy(dialog);
        context.Recorder.Check(
            "Политика «Пропускать существующие» планирует пропуск",
            skipSummary == context.Strings.Format("CopyPreviewSummary", 0, 0, 0, 1, 0),
            skipSummary);
        context.Recorder.Check(
            "Действие для пропуска — «Пропустить»",
            DataOperationsUi.PreviewCell(dialog, 0, 3).Trim() == context.Strings.Get("CopyActionSkip"),
            DataOperationsUi.PreviewCell(dialog, 0, 3).Trim());
        context.Recorder.Check(
            "Копирование недоступно, когда вся запись пропускается",
            !dialog.WaitById("CopyApplyButton").IsEnabled(),
            $"IsEnabled: {dialog.WaitById("CopyApplyButton").IsEnabled()}");

        DataOperationsUi.SelectCopyPolicy(dialog, context.Strings.Get("CopyPolicyOverwrite"));
        var overwriteSummary = DataOperationsUi.PreviewCopy(dialog);
        context.Recorder.Check(
            "Политика «Перезаписывать существующие» планирует перезапись",
            overwriteSummary == context.Strings.Format("CopyPreviewSummary", 0, 1, 0, 0, 0),
            overwriteSummary);
        context.Recorder.Check(
            "Действие для перезаписи — «Перезаписать»",
            DataOperationsUi.PreviewCell(dialog, 0, 3).Trim() == context.Strings.Get("CopyActionOverwrite"),
            DataOperationsUi.PreviewCell(dialog, 0, 3).Trim());

        DataOperationsUi.SelectCopyPolicy(dialog, context.Strings.Get("CopyPolicyMerge"));
        var mergeSummary = DataOperationsUi.PreviewCopy(dialog);
        context.Recorder.Check(
            "Политика «Дополнять отсутствующие поля» планирует дополнение",
            mergeSummary == context.Strings.Format("CopyPreviewSummary", 0, 0, 1, 0, 0),
            mergeSummary);
        context.Recorder.Check(
            "Действие для дополнения — «Дополнить»",
            DataOperationsUi.PreviewCell(dialog, 0, 3).Trim() == context.Strings.Get("CopyActionMerge"),
            DataOperationsUi.PreviewCell(dialog, 0, 3).Trim());

        // Key allocation is offered for a simple key and refused for a compound key.
        var keyMode = dialog.WaitById("CopyKeyModeCombo");
        context.Recorder.Check("Выделение ключей доступно для простого ключа", keyMode.IsEnabled(), null);
        DataOperationsUi.SelectCopyTable(context, dialog, "Items.Attachments");
        Ui.WaitUntil(
            () => !dialog.WaitById("CopyKeyModeCombo").IsEnabled(),
            "выделение ключей отключено для составного ключа",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check("Составной ключ запрещает выделение новых ключей", true);
        DataOperationsUi.SelectCopyTable(context, dialog, "Items.Weapons");
        Ui.WaitUntil(
            () => dialog.WaitById("CopyKeyModeCombo").IsEnabled(),
            "выделение ключей снова доступно",
            TimeSpan.FromSeconds(30));

        // Ambiguous references are reported in the preview instead of being written silently.
        DataOperationsUi.SelectCopyTable(context, dialog, "Items.Items");
        DataOperationsUi.SetCopyRecordKeys(dialog, "12");
        DataOperationsUi.SelectCopyPolicy(dialog, context.Strings.Get("CopyPolicyOverwrite"));
        DataOperationsUi.PreviewCopy(dialog);
        var references = DataOperationsUi.PreviewCell(dialog, 0, 4).Trim();
        context.Recorder.Check(
            "Колонка ссылок показывает неоднозначную ссылку",
            references.Contains("DefaultAttachment", StringComparison.Ordinal)
            && references.Contains(context.Strings.Get("CopyRemapAmbiguous"), StringComparison.Ordinal),
            references);
        var warnings = DataOperationsUi.PreviewCell(dialog, 0, 6).Trim();
        context.Recorder.Check(
            "Предупреждение объясняет, что значение оставлено как есть",
            warnings.Contains("matches several target records", StringComparison.Ordinal),
            warnings);

        // Apply an overwrite of the identical weapon row: the copy succeeds and reports the dirty tab.
        DataOperationsUi.SelectCopyTable(context, dialog, "Items.Weapons");
        DataOperationsUi.SetCopyRecordKeys(dialog, "12");
        DataOperationsUi.SelectCopyPolicy(dialog, context.Strings.Get("CopyPolicyOverwrite"));
        DataOperationsUi.PreviewCopy(dialog);
        DataOperationsUi.ApplyCopy(context, dialog);

        var resultPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("CopyResultSummary"));
        var status = ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(resultPrefix, StringComparison.Ordinal),
            "итог копирования в строке состояния",
            TimeSpan.FromSeconds(60));
        context.Recorder.Check(
            "Строка состояния сообщает об обновлении записи",
            status.StartsWith(context.Strings.Format("CopyResultSummary", 0, 1, 0), StringComparison.Ordinal),
            status);
        var notReloaded = context.Strings.Format("CopyTabsNotReloaded", 1);
        context.Recorder.Check(
            "Строка состояния сообщает о несохранённой вкладке",
            status.Contains(notReloaded, StringComparison.Ordinal),
            status);
        context.Recorder.Check(
            "Идентичная перезапись не меняет файл назначения",
            SaveDirtyTabScenario.HashFile(targetFile) == targetHash);

        // The in-memory edit of the target tab is undone so the application closes without a prompt.
        window.WaitById("UndoButton").InvokeButton();
        Ui.WaitUntil(() => !window.WaitById("SaveButton").IsEnabled(), "несохранённая правка отменена");
    }
}
