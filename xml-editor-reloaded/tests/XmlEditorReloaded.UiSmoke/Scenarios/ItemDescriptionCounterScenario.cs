using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S84: the description field of item 12 shows the legacy remaining-character counter, limits typing to
/// 400 characters, keeps one undo record for the whole edit, and persists the value and counter across a
/// save and reload.
/// </summary>
internal sealed class ItemDescriptionCounterScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S84";

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

        var field = WorkspaceUi.ShowField(context, "szItemDesc");
        var original = WorkspaceUi.ReadFieldValue(field);
        var counter = window.WaitById("szItemDesc_counter");
        context.Recorder.Check(
            "Счётчик описания виден рядом с полем",
            counter.Current.BoundingRectangle.Width > 0,
            WindowUi.Describe(counter.Current.BoundingRectangle));
        context.Recorder.Check(
            "Счётчик показывает остаток для исходного текста",
            counter.ReadName() == context.Strings.Format("DetailCharactersLeftFormat", 400 - original.Length),
            counter.ReadName());

        var shortText = "Короткое описание";
        WorkspaceUi.SetTextAndCommit(field, shortText);
        context.Recorder.Check(
            "Счётчик уменьшается после ввода",
            counter.ReadName() == context.Strings.Format("DetailCharactersLeftFormat", 400 - shortText.Length),
            counter.ReadName());

        var longText = new string('A', 401);
        TypeLongText(field, longText);
        NativeInput.PressIn(field, NativeInput.VirtualKey.Tab);
        Thread.Sleep(300);
        var limited = WorkspaceUi.ReadFieldValue(field);
        context.Recorder.Check(
            "Ввод длиннее 400 символов ограничен 400",
            limited.Length == 400,
            $"длина: {limited.Length}");
        context.Recorder.Check(
            "Счётчик после ограничения показывает 0",
            counter.ReadName() == context.Strings.Format("DetailCharactersLeftFormat", 0),
            counter.ReadName());

        window.WaitById("ItemUndoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(context, context.Strings.Get("EditorUndone"));
        Ui.WaitUntil(
            () => WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szItemDesc")) == shortText,
            "одна отмена возвращает текст до длинного ввода",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Undo возвращает весь текст одной записью", true);

        field = WorkspaceUi.ShowField(context, "szItemDesc");
        TypeLongText(field, longText);
        NativeInput.PressIn(field, NativeInput.VirtualKey.Tab);
        Thread.Sleep(300);
        WorkspaceUi.SaveItem(context);
        context.ExpectChange(DatasetKind.V4, "TableData/Items/Items.xml");

        window.WaitById("ItemReloadButton").InvokeButton();
        Ui.WaitUntil(
            () =>
            {
                var reloaded = WorkspaceUi.ShowField(context, "szItemDesc");
                return WorkspaceUi.ReadFieldValue(reloaded).Length == 400;
            },
            "описание перечитано из файла",
            TimeSpan.FromSeconds(30));
        var savedCounter = window.WaitById("szItemDesc_counter");
        context.Recorder.Check(
            "После сохранения и переоткрытия счётчик снова 0",
            savedCounter.ReadName() == context.Strings.Format("DetailCharactersLeftFormat", 0),
            savedCounter.ReadName());
        context.Recorder.Check(
            "После сохранения и переоткрытия значение осталось 400 символов",
            WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szItemDesc")).Length == 400,
            null);
    }

    /// <summary>
    /// Replaces an input with a long text in chunks, because one SendInput batch of several hundred
    /// Unicode characters can be partially dropped by the target message loop.
    /// </summary>
    /// <param name="field">Text input to replace.</param>
    /// <param name="text">Replacement text.</param>
    private static void TypeLongText(AutomationElement field, string text)
    {
        field.Focus();
        NativeInput.PressIn(field, NativeInput.VirtualKey.A, control: true);
        const int Chunk = 40;
        for (var offset = 0; offset < text.Length; offset += Chunk)
        {
            NativeInput.TypeTextIn(field, text.Substring(offset, Math.Min(Chunk, text.Length - offset)));
            Thread.Sleep(80);
        }
    }
}
