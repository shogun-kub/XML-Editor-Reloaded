namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S72: changing the graphic type rebuilds the chooser from the new small sheet, keeps a now
/// out-of-range stored index unselected and un-clamped, and selecting a valid frame replaces the
/// out-of-range message with the frame counter.
/// </summary>
internal sealed class ImageTypeScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S72";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        ImageUi.Open(context);
        var originalCount = ImageUi.ReadCount(context);

        ImageUi.SelectType(context, 3);
        ImageUi.WaitForOutOfRange(context, 110, 11);
        var staleInfo = context.Window.WaitById("ItemGraphicImageListInfo").ReadName();
        context.Recorder.Check(
            "Смена типа перестраивает список на 12 кадров",
            staleInfo == context.Strings.Format("ItemImageIndexOutOfRangeFormat", 110, 11),
            staleInfo);
        context.Recorder.Check(
            "Старый индекс 110 не зажат и не записан",
            ImageUi.ReadField(context, "ubGraphicNum") == "110",
            ImageUi.ReadField(context, "ubGraphicNum"));

        ImageUi.Open(context);
        context.Recorder.Check(
            "Список сообщает индекс вне нового листа",
            staleInfo == context.Strings.Format("ItemImageIndexOutOfRangeFormat", 110, 11),
            staleInfo);
        context.Recorder.Check(
            "Вне диапазона ни один кадр не выбран",
            ImageUi.SelectedFrame(context) is null,
            $"выбран: {ImageUi.SelectedFrame(context)}");

        ImageUi.SelectFrame(context, 0);
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("EditorCellUpdated", "ubGraphicNum", "12"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Выбор кадра 0 пишет индекс",
            ImageUi.ReadField(context, "ubGraphicNum") == "0",
            ImageUi.ReadField(context, "ubGraphicNum"));
        ImageUi.Open(context);
        context.Recorder.Check(
            "Счётчик заменяет сообщение о диапазоне",
            context.Window.WaitById("ItemGraphicImageListInfo").ReadName()
                == context.Strings.Format("ItemImageListInfoFormat", 12),
            context.Window.WaitById("ItemGraphicImageListInfo").ReadName());

        ImageUi.SelectType(context, 0);
        ImageUi.WaitForCount(context, originalCount);
        context.Recorder.Check(
            "Возврат к типу 0 восстанавливает исходный список",
            ImageUi.ReadCount(context) == originalCount
                && context.Window.WaitById("ItemGraphicImageListInfo").ReadName()
                    == context.Strings.Format("ItemImageListInfoFormat", originalCount),
            context.Window.WaitById("ItemGraphicImageListInfo").ReadName());
        context.Recorder.Check(
            "Возврат типа не меняет выбранный кадр 0",
            ImageUi.ReadField(context, "ubGraphicNum") == "0",
            ImageUi.ReadField(context, "ubGraphicNum"));

        context.Recorder.Note(
            "Раскладка/миниатюры нового списка — визуальная часть; проверены количество, "
            + "сообщение о диапазоне и запись ubGraphicNum.");
    }
}
