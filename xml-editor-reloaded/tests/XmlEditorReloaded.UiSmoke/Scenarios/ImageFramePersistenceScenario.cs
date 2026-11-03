using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S75: the frame selected in the Images tab survives saving and reopening the item, and the save
/// changes only <c>ubGraphicNum</c> while keeping the other elements and their order.
/// </summary>
internal sealed class ImageFramePersistenceScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S75";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var itemsFile = context.Data(DatasetKind.V4).TableFile("Items/Items.xml");
        var before = Item(itemsFile);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        ImageUi.Open(context);
        var frameCount = ImageUi.ReadCount(context);
        ImageUi.SelectFrame(context, 111);
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("EditorCellUpdated", "ubGraphicNum", "12"),
            TimeSpan.FromSeconds(20));
        WorkspaceUi.SaveItem(context);
        context.ExpectChange(DatasetKind.V4, "TableData/Items/Items.xml");

        var after = Item(itemsFile);
        context.Recorder.Check(
            "Сохранение изменило только ubGraphicNum",
            SameExceptGraphicIndex(before, after),
            $"было: {before.Element("ubGraphicNum")?.Value}, стало: {after.Element("ubGraphicNum")?.Value}");
        context.Recorder.Check(
            "Порядок элементов предмета сохранён",
            before.Elements().Select(static element => element.Name.LocalName)
                .SequenceEqual(
                    after.Elements().Select(static element => element.Name.LocalName),
                    StringComparer.Ordinal),
            string.Join(" | ", after.Elements().Select(static element => element.Name.LocalName)));
        context.Recorder.Check(
            "Сохранён кадр 111",
            after.Element("ubGraphicNum")?.Value.Trim() == "111",
            after.Element("ubGraphicNum")?.Value);

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        ImageUi.Open(context);
        ImageUi.SelectFrame(context, 111);
        context.Recorder.Check(
            "После повторного открытия выбран кадр 111",
            ImageUi.SelectedFrame(context) == 111,
            $"выбран: {ImageUi.SelectedFrame(context)}");
        context.Recorder.Check(
            "Список после открытия показывает исходное число кадров",
            ImageUi.ReadCount(context) == frameCount,
            ImageUi.ReadCount(context).ToString());
        var preview = ImageUi.PreviewInfo(context, "Big");
        context.Recorder.Check(
            "Big-превью кадра 111 декодировано после открытия",
            preview.Contains('×', StringComparison.Ordinal),
            preview);
        context.Recorder.Check(
            "Поле ubGraphicNum после открытия равно 111",
            ImageUi.ReadField(context, "ubGraphicNum") == "111",
            ImageUi.ReadField(context, "ubGraphicNum"));
    }

    private static XElement Item(string path) =>
        XDocument.Load(path).Root!.Elements()
            .First(element => element.Element("uiIndex")?.Value.Trim() == "12");

    private static bool SameExceptGraphicIndex(XElement before, XElement after)
    {
        var beforeFields = before.Elements().ToList();
        var afterFields = after.Elements().ToList();
        if (beforeFields.Count != afterFields.Count)
        {
            return false;
        }

        for (var index = 0; index < beforeFields.Count; index++)
        {
            var left = beforeFields[index];
            var right = afterFields[index];
            if (!string.Equals(left.Name.LocalName, right.Name.LocalName, StringComparison.Ordinal))
            {
                return false;
            }

            if (string.Equals(left.Name.LocalName, "ubGraphicNum", StringComparison.Ordinal))
            {
                continue;
            }

            if (!string.Equals(left.Value, right.Value, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}
