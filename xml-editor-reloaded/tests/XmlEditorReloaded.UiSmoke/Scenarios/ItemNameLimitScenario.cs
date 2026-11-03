namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S86: the item name fields accept at most 80 characters and show no character counter, the description
/// fields show the counter, and a generic table without configured text limits shows no counter.
/// </summary>
internal sealed class ItemNameLimitScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S86";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");

        foreach (var fieldName in new[] { "szItemName", "szLongItemName", "szBRName" })
        {
            var field = WorkspaceUi.ShowField(context, fieldName);
            field.Focus();
            NativeInput.PressIn(field, NativeInput.VirtualKey.A, control: true);
            NativeInput.TypeTextIn(field, new string('N', 100));
            NativeInput.PressIn(field, NativeInput.VirtualKey.Tab);
            Thread.Sleep(250);
            var value = WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, fieldName));
            context.Recorder.Check(
                $"{fieldName} ограничен 80 символами",
                value.Length == 80,
                $"длина: {value.Length}");
            context.Recorder.Check(
                $"У {fieldName} нет счётчика",
                window.FindById(fieldName + "_counter") is null,
                "счётчик найден");
            window.WaitById("ItemUndoButton").InvokeButton();
            WorkspaceUi.WaitItemStatus(context, context.Strings.Get("EditorUndone"));
        }

        WorkspaceUi.ShowField(context, "szItemDesc");
        context.Recorder.Check(
            "У описания счётчик есть",
            window.FindById("szItemDesc_counter") is not null,
            "счётчик не найден");

        // The generic detail panel resolves limits from the metadata of its own table. Items.Weapons has
        // no configured text limits, so its name field is unlimited and shows no counter.
        ScenarioUi.OpenTable(context, "Items.Weapons");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.Weapons открыт");
        var grid = window.WaitById("RowsGrid");
        DetailsUi.ShowField(context, grid, 0, "szWeaponName");
        context.Recorder.Check(
            "В generic grid без лимита счётчика нет",
            window.FindById("szWeaponName_counter") is null,
            "счётчик найден");
        context.Recorder.Note(
            "Items.Weapons не имеет строк ColumnTextLimits; лимиты generic grid берутся из метаданных "
            + "своей таблицы, поэтому счётчик здесь не показывается.");
    }
}
