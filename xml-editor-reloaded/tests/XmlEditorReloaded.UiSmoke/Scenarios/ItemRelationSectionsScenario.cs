using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Shared helpers for the item-workspace relation sections of M23b2 (S51, S52, S54).
/// </summary>
internal static class ItemRelationUi
{
    /// <summary>
    /// Selects an item by identifier through the "Go to ID" control.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="id">Item identifier.</param>
    public static void SelectItem(ScenarioContext context, string id)
    {
        var window = context.Window;
        var goTo = window.WaitById("ItemGoToIdBox");
        goTo.Focus();
        goTo.SetText(id);
        window.WaitById("ItemGoToIdButton").InvokeButton();
        Ui.WaitUntil(
            () => window.WaitById("ItemDetailHeader").ReadName().StartsWith(id + ":", StringComparison.Ordinal),
            $"выбран предмет {id}",
            TimeSpan.FromSeconds(30));
    }

    /// <summary>
    /// Selects a child-table section and returns its visible row count.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="label">Section caption without the row count.</param>
    /// <returns>Visible rows of the section grid.</returns>
    public static int SectionRowCount(ScenarioContext context, string label)
    {
        var window = context.Window;
        WorkspaceUi.SelectChildSection(window, context.Strings.Get("ItemTabChildTables"), label);
        var grid = ActiveGrid(window);
        return grid.RowCount();
    }

    /// <summary>
    /// Returns the row count of the visible child grid, ignoring hidden tab content.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <returns>The realized rows grid.</returns>
    /// <exception cref="UiTimeoutException">Thrown when no visible grid exists.</exception>
    public static AutomationElement ActiveGrid(AutomationElement window) =>
        window.FindAllById("RowsGrid")
            .FirstOrDefault(static grid => grid.Current.BoundingRectangle.Width > 0)
        ?? throw new UiTimeoutException("Видимая сетка раздела не найдена.");
}

/// <summary>
/// S51: an item that is used as an attachment shows the reverse "Attachable items" section with the
/// rows that reference it through <c>attachmentIndex</c>.
/// </summary>
internal sealed class AttachableItemsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S51";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        WorkspaceUi.OpenItemWorkspace(context);
        ItemRelationUi.SelectItem(context, "37");
        var label = context.Strings.Get("ItemSectionReverse.Items_Attachments");
        var count = ItemRelationUi.SectionRowCount(context, label);
        context.Recorder.Check("«Attachable items» показывает 122 строки", count == 122, $"строк: {count}");

        var grid = ItemRelationUi.ActiveGrid(context.Window);
        var column = ScenarioUi.ColumnHeaderCaptions(context.Window).ToList().IndexOf("itemIndex");
        var value = WorkspaceUi.ReadCell(grid, 0, column);
        context.Recorder.Check("Строка ссылается на предмет-владельца", !string.IsNullOrWhiteSpace(value), value);
    }
}

/// <summary>
/// S52: a launcher shows the reverse "Ammunition" section with the rows that reference it through
/// <c>itemIndex</c>.
/// </summary>
internal sealed class AmmunitionSectionScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S52";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        WorkspaceUi.OpenItemWorkspace(context);
        ItemRelationUi.SelectItem(context, "41");
        var label = context.Strings.Get("ItemSectionReverse.Items_Launchables");
        var count = ItemRelationUi.SectionRowCount(context, label);
        context.Recorder.Check("«Ammunition» показывает пять строк", count == 5, $"строк: {count}");

        var grid = ItemRelationUi.ActiveGrid(context.Window);
        var column = ScenarioUi.ColumnHeaderCaptions(context.Window).ToList().IndexOf("launchableIndex");
        var value = WorkspaceUi.ReadCell(grid, 0, column);
        context.Recorder.Check("Строка ссылается на боеприпас", !string.IsNullOrWhiteSpace(value), value);
    }
}

/// <summary>
/// S54: <c>CompatibleFaceItems</c> is filtered by the selected face's <c>itemIndex</c>, and the row
/// exposes <c>compatiblefaceitemIndex</c>.
/// </summary>
internal sealed class CompatibleFaceItemsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S54";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        WorkspaceUi.OpenItemWorkspace(context);
        ItemRelationUi.SelectItem(context, "213");
        var label = context.Strings.Get("ItemSection.Items_CompatibleFaceItems");
        var count = ItemRelationUi.SectionRowCount(context, label);
        context.Recorder.Check("«Compatible face items» показывает 8 строк", count == 8, $"строк: {count}");

        var grid = ItemRelationUi.ActiveGrid(context.Window);
        var column = ScenarioUi.ColumnHeaderCaptions(context.Window).ToList().IndexOf("compatiblefaceitemIndex");
        var value = WorkspaceUi.ReadCell(grid, 0, column);
        context.Recorder.Check("Строка показывает совместимый лицевой предмет", !string.IsNullOrWhiteSpace(value), value);
    }
}
