using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S96: a bit-mask lookup shows its kind, a new value receives the lowest free bit, the free-form id
/// dialog rejects a duplicate and warns about a combination, and an accepted id is saved verbatim.
/// </summary>
internal sealed class LookupIdScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S96";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Lookup/AttachmentPoint.xml");
        var before = XDocument.Load(file);

        ScenarioUi.OpenTable(context, "Lookup.AttachmentPoint");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.AttachmentPoint открыт",
            TimeSpan.FromSeconds(30));
        var grid = window.WaitById("RowsGrid");
        context.Recorder.Check("Справочник содержит 8 записей", grid.RowCount() == 8, $"строк: {grid.RowCount()}");
        context.Recorder.Check(
            "Справочник помечен как битовая маска",
            window.WaitById("LookupKindText").ReadName() == context.Strings.Get("LookupKindFlags"),
            window.WaitById("LookupKindText").ReadName());

        window.WaitById("AddRowButton").InvokeButton();
        WaitForRowCount(window, 9);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Format("LookupValueAddedFormat", "32"),
            "статус добавления значения справочника",
            TimeSpan.FromSeconds(20));
        grid = window.WaitById("RowsGrid");
        grid.ScrollToBottom();
        context.Recorder.Check(
            "Новое значение получает свободный бит 32",
            DataOperationsUi.GridCellText(grid, 8, 0) == "32",
            DataOperationsUi.GridCellText(grid, 8, 0));

        FindRealizedRow(grid, "32").SelectItem();
        window.WaitById("ChangeLookupIdButton").InvokeButton();
        var dialog = ScenarioUi.WaitForWindowWithId(
            context.App,
            "LookupIdBox",
            "диалог идентификатора справочника",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Диалог показывает текущий ID и свободный бит",
            dialog.WaitById("LookupIdSummaryText").ReadName()
                .Contains(context.Strings.Format("LookupIdSummaryFormat", "32", "64"), StringComparison.Ordinal),
            dialog.WaitById("LookupIdSummaryText").ReadName());

        dialog.WaitById("LookupIdBox").SetText("1");
        Ui.WaitUntil(
            () => !dialog.WaitById("LookupIdOkButton").IsEnabled(),
            "дубликат ID блокирует подтверждение",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check(
            "Дубликат ID объяснён в диалоге",
            dialog.WaitById("LookupIdErrorText").ReadName()
                .Contains(context.Strings.Format("LookupIdDuplicateFormat", "1"), StringComparison.Ordinal),
            dialog.WaitById("LookupIdErrorText").ReadName());

        dialog.WaitById("LookupIdBox").SetText("5");
        Ui.WaitUntil(
            () => dialog.WaitById("LookupIdOkButton").IsEnabled(),
            "комбинация бит не блокирует подтверждение",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check(
            "Комбинация бит предупреждает, но принимается",
            dialog.WaitById("LookupIdWarningText").ReadName()
                .Contains(context.Strings.Format("LookupIdCombinationFormat", "5"), StringComparison.Ordinal),
            dialog.WaitById("LookupIdWarningText").ReadName());

        dialog.WaitById("LookupIdBox").SetText("64");
        Ui.WaitUntil(
            () => dialog.WaitById("LookupIdWarningText").ReadName().Length == 0,
            "отдельный бит не предупреждает",
            TimeSpan.FromSeconds(10));
        dialog.WaitById("LookupIdOkButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "LookupIdBox");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Format("LookupIdChangedFormat", "32", "64"),
            "статус изменения ID",
            TimeSpan.FromSeconds(20));

        window.WaitById("SaveButton").InvokeButton();
        WaitForSaved(context, "сохранение нового ID");
        var after = XDocument.Load(file);
        var records = after.Descendants("AttachmentPoint").ToList();
        var added = records.Last();
        context.Recorder.Check("После сохранения 9 записей", records.Count == 9, $"записей: {records.Count}");
        context.Recorder.Check(
            "Новое значение хранит свободный ID 64",
            added.Element("id")?.Value.Trim() == "64",
            added.Element("id")?.Value.Trim());
        context.Recorder.Check(
            "Новое значение хранит пустое имя",
            added.Element("name") is not null && added.Element("name")!.Value.Length == 0,
            added.Element("name")?.Value);
        context.Recorder.Check(
            "Прежние записи справочника не изменились",
            LeadingRecordsUnchanged(before, after, 8),
            null);

        grid = window.WaitById("RowsGrid");
        grid.ScrollToBottom();
        FindRealizedRow(grid, "64").SelectItem();
        var delete = window.WaitById("DeleteRowButton");
        _ = Task.Run(() =>
        {
            try
            {
                delete.InvokeButton();
            }
            catch (Exception exception) when (exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The confirmation closed the dialog and the button disappeared before the invoke returned.
            }
        });
        var messageBox = ScenarioUi.WaitForMessageBox(context.App, TimeSpan.FromSeconds(20));
        ScenarioUi.AnswerMessageBox(messageBox, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        WaitForRowCount(window, 8);
        window.WaitById("SaveButton").InvokeButton();
        WaitForSaved(context, "сохранение после удаления значения");

        var final = XDocument.Load(file);
        context.Recorder.Check(
            "После удаления снова 8 записей",
            final.Descendants("AttachmentPoint").Count() == 8,
            $"записей: {final.Descendants("AttachmentPoint").Count()}");
        context.Recorder.Check(
            "Исходные записи справочника восстановлены",
            LeadingRecordsUnchanged(before, final, 8),
            null);

        context.Recorder.Note(
            "Свободный бит, свободный ID, дубликат и комбинация бит проверены в диалоге; "
            + "после удаления тестового значения файл вернулся к исходному набору записей.");
    }

    private static AutomationElement FindRealizedRow(AutomationElement grid, string key) =>
        grid.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
            .FirstOrDefault(row => row.ReadRowCells().Contains(key))
        ?? throw new UiTimeoutException($"Строка с ключом «{key}» не реализована в сетке.");

    private static void WaitForRowCount(AutomationElement window, int expected) =>
        Ui.WaitUntil(
            () => window.WaitById("RowsGrid").RowCount() == expected,
            $"в сетке {expected} строк",
            TimeSpan.FromSeconds(20));

    private static void WaitForSaved(ScenarioContext context, string description) =>
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorSaved"),
            description,
            TimeSpan.FromSeconds(30));

    private static bool LeadingRecordsUnchanged(XDocument before, XDocument after, int count)
    {
        var expected = before.Descendants("AttachmentPoint").Take(count).ToList();
        var actual = after.Descendants("AttachmentPoint").Take(count).ToList();
        return expected.Count == count
            && actual.Count == count
            && expected.Zip(actual).All(pair => XNode.DeepEquals(pair.First, pair.Second));
    }
}
