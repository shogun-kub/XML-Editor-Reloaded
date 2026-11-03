using System.IO;
using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S93: the dedicated nested-record workspace edits a city and a facility assignment, saves the
/// changes inside the declared nested containers, and keeps the facility tooltip translation in the
/// overlay.
/// </summary>
internal sealed class NestedTableWorkspaceScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S93";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var cityFile = context.Data(DatasetKind.V4).TableFile("Map/Cities.xml");
        var cityBefore = XDocument.Load(cityFile);
        var city = cityBefore.Root!
            .Element("CITYLIST")!
            .Elements("CITY")
            .First(element => !string.IsNullOrWhiteSpace(element.Element("townName")?.Value));
        var cityKey = city.Element("uiIndex")?.Value ?? string.Empty;
        var townName = city.Element("townName")!.Value;

        ScenarioUi.OpenTable(context, "Map.Cities");
        Ui.WaitUntil(
            () => ScenarioUi.TabNames(window).Any(name => name.Contains("Map.Cities", StringComparison.Ordinal)),
            "вкладка Map.Cities открыта");
        var cityList = window.WaitById("NestedRecordList");
        Ui.WaitUntil(
            () => cityList.FindAll(new PropertyCondition(
                AutomationElement.ControlTypeProperty,
                ControlType.ListItem)).Count > 0,
            "список городов заполнен");
        WorkspaceUi.FindListItem(cityList, townName)?.SelectItem();
        var townField = window.WaitById("townName", TimeSpan.FromSeconds(15));
        context.Recorder.Check(
            "Поле города townName доступно в отдельном редакторе",
            WorkspaceUi.ReadFieldValue(townField) == townName,
            WorkspaceUi.ReadFieldValue(townField));
        WorkspaceUi.SetTextAndCommit(townField, townName + " test");
        window.WaitById("NestedSearchBox").SetFocus();
        Ui.WaitUntil(
            () => window.WaitById("NestedSaveButton").IsEnabled(),
            "правка города сделала вкладку грязной",
            TimeSpan.FromSeconds(15));

        window.WaitById("NestedSaveButton").InvokeButton();
        WaitForNestedStatus(context, context.Strings.Get("EditorSaved"), "город сохранён");
        var cityAfter = XDocument.Load(cityFile);
        var savedCity = AssertCity(cityAfter, cityKey);
        context.Recorder.Check(
            "Новое имя города записано внутри CITYLIST/CITY",
            savedCity.Element("townName")?.Value == townName + " test",
            savedCity.Element("townName")?.Value ?? "<нет>");

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);

        var facilityFile = context.Data(DatasetKind.V4).TableFile("Map/FacilityTypes.xml");
        var facilityBefore = XDocument.Load(facilityFile);
        var facility = facilityBefore.Root!
            .Elements("FACILITYTYPE")
            .First(element => element.Elements("ASSIGNMENT")
                .Any(assignment => !string.IsNullOrWhiteSpace(assignment.Element("ubAssignmentType")?.Value)
                    && assignment.Element("szTooltipText") is not null));
        var facilityKey = facility.Element("ubIndex")?.Value ?? string.Empty;
        var facilityName = facility.Element("szFacilityName")?.Value ?? facilityKey;
        var assignment = facility.Elements("ASSIGNMENT")
            .First(element => element.Element("szTooltipText") is not null);
        var assignmentType = assignment.Element("ubAssignmentType")!.Value;
        var baseTooltip = assignment.Element("szTooltipText")!.Value;
        var overlayFile = Path.Combine(
            Path.GetDirectoryName(facilityFile)!,
            "Russian.FacilityTypes.xml");

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
        WorkspaceUi.ExpandAll(window.WaitById("NestedSections"));
        var languageCombo = window.WaitById("NestedLanguageCombo");
        languageCombo.SelectComboItemStartingWith("Russian");
        WorkspaceUi.ExpandAll(window.WaitById("NestedSections"));
        var tooltipField = window.WaitById(
            $"ASSIGNMENT[{assignmentType}]/szTooltipText",
            TimeSpan.FromSeconds(20));
        Ui.WaitUntil(
            () => WorkspaceUi.ReadFieldValue(window.WaitById(
                $"ASSIGNMENT[{assignmentType}]/szTooltipText")) != baseTooltip,
            "русский перевод подсказки загружен",
            TimeSpan.FromSeconds(20));
        tooltipField = window.WaitById(
            $"ASSIGNMENT[{assignmentType}]/szTooltipText",
            TimeSpan.FromSeconds(20));
        var translatedTooltip = WorkspaceUi.ReadFieldValue(tooltipField);
        context.Recorder.Check(
            "Поле ASSIGNMENT/szTooltipText доступно в редакторе",
            translatedTooltip.Length > 0,
            translatedTooltip);
        WorkspaceUi.SetTextAndCommit(tooltipField, translatedTooltip + " test");
        window.WaitById("NestedSearchBox").SetFocus();
        Ui.WaitUntil(
            () => window.WaitById("NestedSaveButton").IsEnabled(),
            "правка подсказки сделала вкладку грязной",
            TimeSpan.FromSeconds(15));

        window.WaitById("NestedSaveButton").InvokeButton();
        WaitForNestedStatus(context, context.Strings.Get("EditorSaved"), "тип объекта сохранён");
        var facilityAfter = XDocument.Load(facilityFile);
        var savedFacility = facilityAfter.Root!
            .Elements("FACILITYTYPE")
            .First(element => element.Element("ubIndex")?.Value == facilityKey);
        var savedAssignment = savedFacility.Elements("ASSIGNMENT")
            .First(element => element.Element("ubAssignmentType")?.Value == assignmentType);
        context.Recorder.Check(
            "Базовая подсказка назначения не изменена",
            savedAssignment.Element("szTooltipText")?.Value == baseTooltip,
            savedAssignment.Element("szTooltipText")?.Value ?? "<нет>");

        var overlay = XDocument.Load(overlayFile);
        var translated = overlay.Root!
            .Elements("FACILITYTYPE")
            .First(element => element.Element("ubIndex")?.Value == facilityKey)
            .Elements("ASSIGNMENT")
            .First(element => element.Element("ubAssignmentType")?.Value == assignmentType);
        context.Recorder.Check(
            "Перевод подсказки записан в Russian.FacilityTypes.xml",
            translated.Element("szTooltipText")?.Value == translatedTooltip + " test",
            translated.Element("szTooltipText")?.Value ?? "<нет>");

        context.ExpectChange(DatasetKind.V4, "TableData/Map/Cities.xml");
        context.ExpectChange(DatasetKind.V4, "TableData/Map/Russian.FacilityTypes.xml");
    }

    private static XElement AssertCity(XDocument document, string key) =>
        document.Root!
            .Element("CITYLIST")!
            .Elements("CITY")
            .First(element => element.Element("uiIndex")?.Value == key);

    private static void WaitForNestedStatus(
        ScenarioContext context,
        string expected,
        string description) =>
        Ui.WaitUntil(
            () => context.Window.WaitById("NestedStatusText").ReadName() == expected,
            description,
            TimeSpan.FromSeconds(60));
}
