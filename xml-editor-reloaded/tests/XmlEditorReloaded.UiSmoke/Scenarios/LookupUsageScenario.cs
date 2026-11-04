using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S97: a used lookup value shows "where used", cannot be deleted or renamed, and its first usage opens
/// the referencing table with the record selected. The lookup file stays unchanged.
/// </summary>
internal sealed class LookupUsageScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S97";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Lookup/ItemClass.xml");
        var before = XDocument.Load(file);

        ScenarioUi.OpenTable(context, "Lookup.ItemClass");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.ItemClass открыт",
            TimeSpan.FromSeconds(30));
        var grid = window.WaitById("RowsGrid");
        context.Recorder.Check("Справочник ItemClass содержит 23 записи", grid.RowCount() == 23, $"строк: {grid.RowCount()}");

        FindRealizedRow(grid, "2").SelectItem();
        window.WaitById("LookupUsageButton").InvokeButton();
        var usageWindow = ScenarioUi.WaitForWindowWithId(
            context.App,
            "LookupUsageGrid",
            "окно «Где используется»",
            TimeSpan.FromSeconds(30));
        var summary = usageWindow.WaitById("LookupUsageSummaryText").ReadName();
        context.Recorder.Check(
            "Использования ItemClass 2 найдены",
            summary.Contains("338", StringComparison.Ordinal),
            summary);
        context.Recorder.Check(
            "Список использований не пуст",
            usageWindow.WaitById("LookupUsageGrid").RowCount() > 0,
            $"строк: {usageWindow.WaitById("LookupUsageGrid").RowCount()}");
        usageWindow.WaitById("LookupUsageCloseButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "LookupUsageGrid");

        FindRealizedRow(grid, "2").SelectItem();
        window.WaitById("DeleteRowButton").InvokeButton();
        var deleteBox = ScenarioUi.WaitForMessageBox(context.App, TimeSpan.FromSeconds(30));
        var deleteText = ScenarioUi.ReadMessageBoxText(deleteBox);
        context.Recorder.Check(
            "Удаление используемого значения заблокировано",
            deleteText.Contains("Нельзя удалить", StringComparison.Ordinal)
                && deleteText.Contains("338", StringComparison.Ordinal),
            deleteText);
        ScenarioUi.AnswerMessageBox(deleteBox, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        context.Recorder.Check(
            "Строка ItemClass 2 осталась в справочнике",
            grid.RowCount() == 23,
            $"строк: {grid.RowCount()}");

        window.WaitById("ChangeLookupIdButton").InvokeButton();
        var changeBox = ScenarioUi.WaitForMessageBox(context.App, TimeSpan.FromSeconds(30));
        var changeText = ScenarioUi.ReadMessageBoxText(changeBox);
        context.Recorder.Check(
            "Смена ID используемого значения заблокирована",
            changeText.Contains("Нельзя изменить", StringComparison.Ordinal),
            changeText);
        ScenarioUi.AnswerMessageBox(changeBox, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        context.Recorder.Check(
            "Диалог свободного ID не открывался",
            !ScenarioUi.IsWindowWithIdOpen(context.App, "LookupIdBox"),
            null);

        FindRealizedRow(grid, "2").SelectItem();
        window.WaitById("LookupUsageButton").InvokeButton();
        usageWindow = ScenarioUi.WaitForWindowWithId(
            context.App,
            "LookupUsageGrid",
            "окно «Где используется» для перехода",
            TimeSpan.FromSeconds(30));
        usageWindow.WaitById("LookupUsageOpenButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "LookupUsageGrid");
        Ui.WaitUntil(
            () => ScenarioUi.TabNames(window).Any(name => name.Contains("Items.AttachmentInfo", StringComparison.Ordinal)),
            "вкладка Items.AttachmentInfo открыта из использования",
            TimeSpan.FromSeconds(30));

        context.Recorder.Check(
            "Файл ItemClass не изменён",
            XNode.DeepEquals(before.Root, XDocument.Load(file).Root),
            null);
        context.Recorder.Note(
            "Использования показаны, удаление и смена ID используемого значения заблокированы, "
            + "переход из списка открыл Items.AttachmentInfo; справочник не изменялся.");
    }

    private static AutomationElement FindRealizedRow(AutomationElement grid, string key) =>
        grid.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
            .FirstOrDefault(row => row.ReadRowCells().Contains(key))
        ?? throw new UiTimeoutException($"Строка с ключом «{key}» не реализована в сетке.");
}
