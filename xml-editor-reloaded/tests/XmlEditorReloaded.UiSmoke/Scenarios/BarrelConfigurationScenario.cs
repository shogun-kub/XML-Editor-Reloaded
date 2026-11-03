using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S55: the repeated <c>BarrelConfiguration</c> field of a weapon is edited as a list, appended and
/// removed, and a save writes repeated <c>BarrelConfiguration</c> tags without generating the legacy
/// <c>BarrelConfigurationN</c> columns.
/// </summary>
internal sealed class BarrelConfigurationScenario : SmokeScenario
{
    private const string FieldId = "ItemListSection_BarrelConfiguration";
    private const string ValueId = "ItemListValue_BarrelConfiguration";
    private const string AddId = "ItemListAdd_BarrelConfiguration";
    private const string RemoveId = "ItemListRemove_BarrelConfiguration";

    /// <inheritdoc />
    public override string Id => "S55";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var weaponsFile = context.Data(DatasetKind.V4).TableFile("Items/Weapons.xml");
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "686", "Surf Zone");
        WorkspaceUi.SelectItemTab(window, context.Strings.Get("ItemTabSubTables"));
        WorkspaceUi.ExpandAll(window.WaitById("ItemTabs"));

        var group = window.WaitById(FieldId, TimeSpan.FromSeconds(30));
        var values = Editors(group);
        context.Recorder.Check(
            "BarrelConfiguration показан списком из трёх значений",
            values.Count == 3
                && values.Select(static value => value.ReadValue()).SequenceEqual(["4", "6", "8"]),
            string.Join(",", values.Select(static value => value.ReadValue())));

        WriteValue(context, values[0], "7");
        context.Recorder.Check(
            "Правка первого значения записана в список",
            Editors(window.WaitById(FieldId))[0].ReadValue() == "7",
            Editors(window.WaitById(FieldId))[0].ReadValue());

        window.WaitById(AddId).InvokeButton();
        Ui.WaitUntil(
            () => Editors(window.WaitById(FieldId)).Count == 4,
            "в списке появилось четвёртое значение",
            TimeSpan.FromSeconds(20));
        var added = Editors(window.WaitById(FieldId));
        context.Recorder.Check(
            "Добавление создаёт пустое значение 0",
            added[^1].ReadValue() == "0",
            added[^1].ReadValue());

        WorkspaceUi.SaveItem(context);
        context.ExpectChange(DatasetKind.V4, "TableData/Items/Weapons.xml");
        var saved = Barrels(weaponsFile, "686");
        context.Recorder.Check(
            "Сохранены повторяющиеся BarrelConfiguration без BarrelConfigurationN",
            saved.SequenceEqual(["7", "6", "8", "0"])
                && Weapon(weaponsFile, "686").Elements("BarrelConfiguration0").Any() is false,
            string.Join(",", saved));

        var remove = window.WaitById(FieldId)
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))
            .LastOrDefault(button => button.Current.AutomationId == RemoveId)
            ?? throw new UiTimeoutException("Кнопка удаления значения не найдена.");
        remove.InvokeButton();
        Ui.WaitUntil(
            () => Editors(window.WaitById(FieldId)).Count == 3,
            "последнее значение удалено",
            TimeSpan.FromSeconds(20));
        WorkspaceUi.SaveItem(context);

        var afterRemove = Barrels(weaponsFile, "686");
        context.Recorder.Check(
            "Удаление убирает только последнее значение",
            afterRemove.SequenceEqual(["7", "6", "8"]),
            string.Join(",", afterRemove));
        context.Recorder.Check(
            "В файле нет динамических BarrelConfigurationN",
            Weapon(weaponsFile, "686")
                .Elements()
                .All(element => !element.Name.LocalName.StartsWith("BarrelConfiguration", StringComparison.Ordinal)
                    || element.Name.LocalName == "BarrelConfiguration"),
            string.Join(" | ", Weapon(weaponsFile, "686").Elements().Select(static element => element.Name.LocalName)
                .Where(static name => name.StartsWith("BarrelConfiguration", StringComparison.Ordinal))));
    }

    private static IReadOnlyList<AutomationElement> Editors(AutomationElement group) =>
        group.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit))
            .Cast<AutomationElement>()
            .Where(element => element.Current.AutomationId == ValueId)
            .ToList();

    private static void WriteValue(ScenarioContext context, AutomationElement editor, string value)
    {
        WorkspaceUi.SetTextAndCommit(editor, value);
        context.Window.WaitById("ItemGoToIdBox").Focus();
        Thread.Sleep(350);
    }

    private static XElement Weapon(string path, string index) =>
        XDocument.Load(path).Root!.Elements("WEAPON")
            .First(element => element.Element("uiIndex")?.Value.Trim() == index);

    private static string[] Barrels(string path, string index) =>
        Weapon(path, index).Elements("BarrelConfiguration")
            .Select(static element => element.Value.Trim())
            .ToArray();
}
