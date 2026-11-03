using System.IO;
using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S94: the nested facility editor appends an assignment, edits a repeated drug-item list and adds a
/// production-line block, then saves base values and the translated tooltip to different files.
/// </summary>
internal sealed class NestedCollectionsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S94";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var facilityFile = context.Data(DatasetKind.V4).TableFile("Map/FacilityTypes.xml");
        var overlayFile = Path.Combine(
            Path.GetDirectoryName(facilityFile)!,
            "Russian.FacilityTypes.xml");
        var before = XDocument.Load(facilityFile);
        var facility = before.Root!
            .Elements("FACILITYTYPE")
            .First(element => element.Elements("ASSIGNMENT")
                    .Any(assignment => assignment.Element("DRUNK")?.Element("drugitems") is not null)
                && !element.Elements("PRODUCTIONLINE").Any());
        var facilityKey = facility.Element("ubIndex")?.Value ?? string.Empty;
        var facilityName = facility.Element("szFacilityName")?.Value ?? facilityKey;
        var assignment = facility.Elements("ASSIGNMENT")
            .First(element => element.Element("DRUNK")?.Element("drugitems") is not null);
        var assignmentType = assignment.Element("ubAssignmentType")!.Value;
        var drugItems = assignment
            .Element("DRUNK")!
            .Element("drugitems")!
            .Elements("drugitem")
            .Select(element => element.Value)
            .ToArray();

        ScenarioUi.OpenTable(context, "Map.FacilityTypes");
        Ui.WaitUntil(
            () => ScenarioUi.TabNames(window).Any(name => name.Contains("Map.FacilityTypes", StringComparison.Ordinal)),
            "вкладка Map.FacilityTypes открыта");
        var facilityList = window.WaitById("NestedRecordList");
        Ui.WaitUntil(
            () => facilityList.FindAll(new PropertyCondition(
                AutomationElement.ControlTypeProperty,
                ControlType.ListItem)).Count > 0,
            "список типов объектов заполнен");
        WorkspaceUi.FindListItem(facilityList, facilityName)?.SelectItem();
        window.WaitById("NestedLanguageCombo").SelectComboItemStartingWith("Russian");
        WorkspaceUi.ExpandAll(window.WaitById("NestedSections"));

        window.WaitById("NestedNewAssignmentTypeBox").SetText("TESTM24B");
        var addAssignment = window.WaitById("NestedAddAssignmentButton");
        Ui.WaitUntil(() => addAssignment.IsEnabled(), "кнопка добавления назначения активна");
        addAssignment.InvokeButton();
        var tooltip = window.WaitById(
            "ASSIGNMENT[TESTM24B]/szTooltipText",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Новое назначение TESTM24B добавлено в редактор",
            tooltip is not null,
            "ASSIGNMENT[TESTM24B]/szTooltipText");
        WorkspaceUi.SetTextAndCommit(tooltip!, "Тест M24b");

        var listPath = $"ASSIGNMENT[{assignmentType}]/DRUNK/drugitems/drugitem";
        window.WaitById("NestedListAdd_" + listPath, TimeSpan.FromSeconds(20)).InvokeButton();
        Ui.WaitUntil(
            () => window.FindAllById("NestedListValue_" + listPath).Count == drugItems.Length + 1,
            "в список drugitem добавлен элемент",
            TimeSpan.FromSeconds(20));
        var listValues = window.FindAllById("NestedListValue_" + listPath);
        WorkspaceUi.SetTextAndCommit(listValues[^1], "300");

        window.WaitById("NestedAddBlock_PRODUCTIONLINE").InvokeButton();
        var itemField = window.WaitById(
            "PRODUCTIONLINE[1]/usItemToCreate",
            TimeSpan.FromSeconds(20));
        WorkspaceUi.SetTextAndCommit(itemField, "9999");
        window.WaitById("NestedSearchBox").SetFocus();
        Ui.WaitUntil(
            () => window.WaitById("NestedSaveButton").IsEnabled(),
            "правки сделали вкладку грязной",
            TimeSpan.FromSeconds(15));

        window.WaitById("NestedSaveButton").InvokeButton();
        Ui.WaitUntil(
            () => window.WaitById("NestedStatusText").ReadName() == context.Strings.Get("EditorSaved"),
            "тип объекта сохранён",
            TimeSpan.FromSeconds(60));

        var saved = XDocument.Load(facilityFile).Root!
            .Elements("FACILITYTYPE")
            .Single(element => element.Element("ubIndex")?.Value == facilityKey);
        context.Recorder.Check(
            "Новое назначение записано в базу",
            saved.Elements("ASSIGNMENT")
                .Any(element => element.Element("ubAssignmentType")?.Value == "TESTM24B"),
            string.Join(", ", saved.Elements("ASSIGNMENT")
                .Select(element => element.Element("ubAssignmentType")?.Value)));
        context.Recorder.Check(
            "Список drugitem расширен значением 300",
            saved.Elements("ASSIGNMENT")
                .Single(element => element.Element("ubAssignmentType")?.Value == assignmentType)
                .Element("DRUNK")!
                .Element("drugitems")!
                .Elements("drugitem")
                .Select(element => element.Value)
                .SequenceEqual(drugItems.Append("300")),
            string.Join(", ", saved.Elements("ASSIGNMENT")
                .Single(element => element.Element("ubAssignmentType")?.Value == assignmentType)
                .Element("DRUNK")!
                .Element("drugitems")!
                .Elements("drugitem")
                .Select(element => element.Value)));
        context.Recorder.Check(
            "Блок PRODUCTIONLINE добавлен со значением 9999",
            saved.Elements("PRODUCTIONLINE")
                .SingleOrDefault()
                ?.Element("usItemToCreate")?.Value == "9999",
            saved.Elements("PRODUCTIONLINE").SingleOrDefault()?.Element("usItemToCreate")?.Value ?? "<нет>");

        var overlay = XDocument.Load(overlayFile);
        context.Recorder.Check(
            "Перевод подсказки нового назначения записан в оверлей",
            overlay.Root!
                .Elements("FACILITYTYPE")
                .Single(element => element.Element("ubIndex")?.Value == facilityKey)
                .Elements("ASSIGNMENT")
                .SingleOrDefault(element => element.Element("ubAssignmentType")?.Value == "TESTM24B")
                ?.Element("szTooltipText")?.Value == "Тест M24b",
            overlay.Root!
                .Elements("FACILITYTYPE")
                .Single(element => element.Element("ubIndex")?.Value == facilityKey)
                .Elements("ASSIGNMENT")
                .SingleOrDefault(element => element.Element("ubAssignmentType")?.Value == "TESTM24B")
                ?.Element("szTooltipText")?.Value ?? "<нет>");

        context.ExpectChange(DatasetKind.V4, "TableData/Map/FacilityTypes.xml");
        context.ExpectChange(DatasetKind.V4, "TableData/Map/Russian.FacilityTypes.xml");
    }
}
