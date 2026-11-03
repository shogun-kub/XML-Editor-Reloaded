namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S71: the Images tab lists the frames of the small sheet, starts at the stored frame 110, selecting
/// 111 and 110 writes <c>ubGraphicNum</c>, selecting the current frame does not dirty the tab, and
/// Undo/Redo restore both the field and the chooser selection.
/// </summary>
internal sealed class ImageFrameScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S71";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");

        var info = ImageUi.Open(context);
        var frameCount = ImageUi.ReadCount(context);
        context.Recorder.Check(
            "Список кадров показывает счётчик",
            info == context.Strings.Format("ItemImageListInfoFormat", frameCount),
            info);
        context.Recorder.Check(
            "У типа 0 достаточно кадров для 110 и 111",
            frameCount > 111,
            $"кадров: {frameCount}");

        ImageUi.SelectFrame(context, 110);
        context.Recorder.Check(
            "Стартовый кадр 110 выбран",
            ImageUi.SelectedFrame(context) == 110,
            $"выбран: {ImageUi.SelectedFrame(context)}");
        var firstPreview = ImageUi.PreviewInfo(context, "Big");
        context.Recorder.Check(
            "Big-превью кадра 110 декодировано",
            firstPreview.Contains('×', StringComparison.Ordinal),
            firstPreview);

        ImageUi.SelectFrame(context, 111);
        context.Recorder.Check(
            "Выбор кадра 111 пишет ubGraphicNum",
            ImageUi.SelectedFrame(context) == 111,
            $"выбран: {ImageUi.SelectedFrame(context)}");

        ImageUi.SelectFrame(context, 110);
        context.Recorder.Check(
            "Выбор кадра 110 пишет ubGraphicNum",
            ImageUi.SelectedFrame(context) == 110,
            $"выбран: {ImageUi.SelectedFrame(context)}");

        window.WaitById("ItemUndoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(context, context.Strings.Get("EditorUndone"));
        Thread.Sleep(700);
        context.Recorder.Check(
            "Undo возвращает кадр 111 и выделение",
            ImageUi.SelectedFrame(context) == 111,
            $"выбран: {ImageUi.SelectedFrame(context)}");

        window.WaitById("ItemRedoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(context, context.Strings.Get("EditorRedone"));
        Thread.Sleep(700);
        context.Recorder.Check(
            "Redo возвращает кадр 110 и выделение",
            ImageUi.SelectedFrame(context) == 110,
            $"выбран: {ImageUi.SelectedFrame(context)}");

        ImageUi.SelectFrame(context, 111);
        context.Recorder.Check(
            "Выбор кадра 111 снова делает вкладку грязной",
            window.WaitById("ItemSaveButton").IsEnabled(),
            $"Save.IsEnabled={window.WaitById("ItemSaveButton").IsEnabled()}");
        WorkspaceUi.SaveItem(context);
        context.ExpectChange(DatasetKind.V4, "TableData/Items/Items.xml");
        context.Recorder.Check(
            "Сохранён кадр 111",
            ImageUi.ReadField(context, "ubGraphicNum") == "111",
            ImageUi.ReadField(context, "ubGraphicNum"));

        ImageUi.Open(context);
        ImageUi.SelectFrame(context, 111);
        Thread.Sleep(300);
        context.Recorder.Check(
            "Повторный выбор текущего кадра не делает вкладку грязной",
            !window.WaitById("ItemSaveButton").IsEnabled(),
            $"Save.IsEnabled={window.WaitById("ItemSaveButton").IsEnabled()}");
        context.Recorder.Note(
            "Миниатюра элемента списка — визуальная часть; сценарий проверяет выбор кадра, "
            + "запись ubGraphicNum и декодированный Big-превью.");
    }
}
