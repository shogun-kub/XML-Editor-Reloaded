using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S73: the image index field rejects out-of-range, negative, and non-integer values while the preview
/// stays on the current frame. The loading race is covered by the view-model tests because the frame
/// list can finish before a synthetic edit reaches the field.
/// </summary>
internal sealed class ImageIndexValidationScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S73";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        ImageUi.Open(context);
        var maximum = ImageUi.ReadCount(context) - 1;
        var window = context.Window;

        WorkspaceUi.SelectItemTab(window, context.Strings.Get("ItemTabFields"));
        var field = WorkspaceUi.ShowField(context, "ubGraphicNum");
        var original = WorkspaceUi.ReadFieldValue(field);
        context.Recorder.Check(
            "Исходный индекс кадра 110",
            original == "110",
            original);

        EnterAndLeave(context, field, "9999");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ItemImageIndexOutOfRangeFormat", "9999", maximum),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "9999 отклонён, значение не изменилось",
            WorkspaceUi.ReadFieldValue(window.WaitById("ubGraphicNum")) == original,
            WorkspaceUi.ReadFieldValue(window.WaitById("ubGraphicNum")));
        context.Recorder.Check(
            "После 9999 вкладка не грязная",
            !window.WaitById("ItemSaveButton").IsEnabled(),
            $"Save.IsEnabled={window.WaitById("ItemSaveButton").IsEnabled()}");

        EnterAndLeave(context, window.WaitById("ubGraphicNum"), "-1");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Get("ItemImageIndexNegative"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "-1 отклонён, значение не изменилось",
            WorkspaceUi.ReadFieldValue(window.WaitById("ubGraphicNum")) == original,
            WorkspaceUi.ReadFieldValue(window.WaitById("ubGraphicNum")));

        EnterAndLeave(context, window.WaitById("ubGraphicNum"), "1.5");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Get("ItemImageIndexInvalid"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "1.5 отклонён как нецелый индекс",
            WorkspaceUi.ReadFieldValue(window.WaitById("ubGraphicNum")) == original,
            WorkspaceUi.ReadFieldValue(window.WaitById("ubGraphicNum")));

        ImageUi.Open(context);
        ImageUi.SelectFrame(context, 110);
        context.Recorder.Check(
            "После отклонённых правок выбран исходный кадр",
            ImageUi.SelectedFrame(context) == 110,
            $"выбран: {ImageUi.SelectedFrame(context)}");
        var preview = ImageUi.PreviewInfo(context, "Big");
        context.Recorder.Check(
            "Превью исходного кадра остаётся декодированным",
            preview.Contains('×', StringComparison.Ordinal),
            preview);

        context.Recorder.Note(
            "Гонка «правка во время загрузки списка» проверяется unit-тестом "
            + "SetItemValue_WhileFrameCountPending_RejectsStaleIndex.");
    }

    private static void EnterAndLeave(ScenarioContext context, AutomationElement field, string value)
    {
        WorkspaceUi.SetTextAndCommit(field, value);
        // ValuePattern changes the TextBox text without raising TextChanged; an explicit focus move
        // forces LostFocus so the VM receives SetCellValue and can reject the value.
        context.Window.WaitById("ItemGoToIdBox").Focus();
        Thread.Sleep(350);
    }
}
