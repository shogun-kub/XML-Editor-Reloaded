using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S21: nested structure values of item 12 are edited, saved, and still present after the table is
/// reopened, while every repeated element of the other records stays byte-for-byte equivalent.
/// </summary>
internal sealed class ItemNestedScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S21";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Items/Items.xml");
        var before = XDocument.Load(file);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        WorkspaceUi.SelectItemTab(window, context.Strings.Get("ItemTabNested"));
        WorkspaceUi.ExpandAll(window.WaitById("ItemTabs"));

        var stand = window.WaitById("STAND_MODIFIERS/FlatBase");
        context.Recorder.Check(
            "Вложенное поле STAND_MODIFIERS/FlatBase доступно",
            WorkspaceUi.ReadFieldValue(stand).Length == 0,
            WorkspaceUi.ReadFieldValue(stand));
        WorkspaceUi.SetTextAndCommit(stand, "25");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("EditorCellUpdated", "STAND_MODIFIERS/FlatBase", "12"),
            TimeSpan.FromSeconds(20));

        var crouch = window.WaitById("CROUCH_MODIFIERS/FlatBase");
        WorkspaceUi.SetTextAndCommit(crouch, "7");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("EditorCellUpdated", "CROUCH_MODIFIERS/FlatBase", "12"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Оба вложенных значения записаны в память", true);
        context.Recorder.Check(
            "Правка вложенной структуры делает вкладку грязной",
            window.WaitById("ItemSaveButton").IsEnabled(),
            null);

        WorkspaceUi.SaveItem(context);
        var after = XDocument.Load(file);
        var structure = CompareStructures(before, after, "12", new[] { "STAND_MODIFIERS", "CROUCH_MODIFIERS" });
        context.Recorder.Check(
            "Изменена только запись 12, остальные элементы и их порядок сохранены",
            structure.OnlyExpectedChanges,
            structure.Details);
        var savedItem = after.Descendants("ITEM")
            .First(element => element.Element("uiIndex")?.Value.Trim() == "12");
        context.Recorder.Check(
            "В файл записано STAND_MODIFIERS/FlatBase = 25",
            NestedValue(savedItem, "STAND_MODIFIERS", "FlatBase") == "25",
            NestedValue(savedItem, "STAND_MODIFIERS", "FlatBase"));
        context.Recorder.Check(
            "В файл записано CROUCH_MODIFIERS/FlatBase = 7",
            NestedValue(savedItem, "CROUCH_MODIFIERS", "FlatBase") == "7",
            NestedValue(savedItem, "CROUCH_MODIFIERS", "FlatBase"));

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        WorkspaceUi.SelectItemTab(window, context.Strings.Get("ItemTabNested"));
        WorkspaceUi.ExpandAll(window.WaitById("ItemTabs"));
        var reopenedStand = WorkspaceUi.ReadFieldValue(window.WaitById("STAND_MODIFIERS/FlatBase"));
        var reopenedCrouch = WorkspaceUi.ReadFieldValue(window.WaitById("CROUCH_MODIFIERS/FlatBase"));
        context.Recorder.Check(
            "После повторного открытия значения на месте",
            reopenedStand == "25" && reopenedCrouch == "7",
            $"стоя: {reopenedStand}, на колене: {reopenedCrouch}");

        context.ExpectChange(DatasetKind.V4, "TableData/Items/Items.xml");
    }

    private static string NestedValue(XElement item, string container, string leaf) =>
        item.Element(container)?.Element(leaf)?.Value ?? string.Empty;

    /// <summary>
    /// Compares every item record before and after a save, ignoring the two nested containers of the
    /// edited record, whose children are expected to appear.
    /// </summary>
    /// <param name="before">Document before the save.</param>
    /// <param name="after">Document after the save.</param>
    /// <param name="editedKey">Key of the edited record.</param>
    /// <param name="editContainers">Nested containers of the edited record that may gain children.</param>
    /// <returns>Whether only the expected values changed, and a failure description.</returns>
    private static (bool OnlyExpectedChanges, string? Details) CompareStructures(
        XDocument before,
        XDocument after,
        string editedKey,
        IReadOnlyList<string> editContainers)
    {
        var beforeItems = Index(before, editedKey, editContainers);
        var afterItems = Index(after, editedKey, editContainers);
        if (beforeItems.Count != afterItems.Count)
        {
            return (false, $"число записей: {beforeItems.Count} → {afterItems.Count}");
        }

        foreach (var (key, text) in beforeItems)
        {
            if (!afterItems.TryGetValue(key, out var current) || !string.Equals(text, current, StringComparison.Ordinal))
            {
                return (false, $"изменилась запись {key}");
            }
        }

        return (true, null);
    }

    private static Dictionary<string, string> Index(
        XDocument document,
        string editedKey,
        IReadOnlyList<string> editContainers)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var item in document.Descendants("ITEM"))
        {
            var key = item.Element("uiIndex")?.Value.Trim() ?? string.Empty;
            XElement copy = new(item);
            if (string.Equals(key, editedKey, StringComparison.Ordinal))
            {
                foreach (var container in editContainers)
                {
                    copy.Element(container)?.Elements().Remove();
                }
            }

            result[key] = copy.ToString(SaveOptions.DisableFormatting);
        }

        return result;
    }
}
