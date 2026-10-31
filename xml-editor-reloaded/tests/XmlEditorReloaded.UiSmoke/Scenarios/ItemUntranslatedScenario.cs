using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S24: an item without a Russian overlay row shows the base value, marks every translatable field with
/// the untranslated dot and its tooltip, and does not create a translation by itself.
/// </summary>
internal sealed class ItemUntranslatedScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S24";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override string? OverlayLanguage => "ru";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var overlayFile = context.Data(DatasetKind.V4).TableFile("Items/Russian.Items.xml");
        var overlayHash = SaveDirtyTabScenario.HashFile(overlayFile);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1742", "Wood Shield");
        context.Recorder.Check(
            "Непереведённый предмет показывает английское имя",
            window.WaitById("ItemDetailHeader").ReadName() == "1742: Wood Shield",
            window.WaitById("ItemDetailHeader").ReadName());
        var name = WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szItemName"));
        context.Recorder.Check("Поле берёт значение из базовой таблицы", name == "Wood Shield", name);

        var tabs = window.WaitById("ItemTabs");
        var markers = tabs
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Where(static text => text.ReadName() == "●")
            .ToList();
        context.Recorder.Check(
            "У непереведённых полей есть маркер «●»",
            markers.Count > 0,
            $"маркеров: {markers.Count}");
        var tooltip = Ui.HoverToolTip(markers[0]);
        context.Recorder.Check(
            "Подсказка маркера объясняет, что значение взято из базы",
            tooltip == context.Strings.Get("EditorUntranslated"),
            tooltip);

        var combo = window.WaitById("ItemLanguageCombo");
        combo.SelectComboItem(context.Strings.Get("EditorBaseLanguageOption"));
        WorkspaceUi.WaitForStatus(
            context,
            "ItemDetailHeader",
            text => text == "1742: Wood Shield",
            "базовый режим загружен",
            TimeSpan.FromSeconds(60));
        var baseMarkers = window.WaitById("ItemTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Count(static text => text.ReadName() == "●");
        context.Recorder.Check(
            "В базовом режиме маркеров перевода нет",
            baseMarkers == 0,
            $"маркеров: {baseMarkers}");

        context.Recorder.Check(
            "Оверлей не изменился и не получил запись 1742",
            SaveDirtyTabScenario.HashFile(overlayFile) == overlayHash
                && !XDocument.Load(overlayFile)
                    .Descendants("ITEM")
                    .Any(element => element.Element("uiIndex")?.Value.Trim() == "1742"),
            null);
    }
}
