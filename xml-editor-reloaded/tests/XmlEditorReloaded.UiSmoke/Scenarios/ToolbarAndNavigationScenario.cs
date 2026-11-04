using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S98: toolbar buttons in one row have identical height and top position, icon buttons keep their
/// accessible names, and the grid navigates to the first/last row with Home/End and by pages with
/// PageUp/PageDown.
/// </summary>
internal sealed class ToolbarAndNavigationScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S98";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        ScenarioUi.OpenTable(context, "Items.Weapons");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.Weapons открыт");

        var grid = window.WaitById("RowsGrid");
        var tableButtons = new[]
        {
            "UndoButton",
            "RedoButton",
            "AddRowButton",
            "DuplicateRowButton",
            "DeleteRowButton",
            "SaveButton",
            "ValidateButton",
            "ReloadButton",
        };
        CheckSameGeometry(window, tableButtons, "тулбар таблицы");

        var mainButtons = new[]
        {
            "BrowseDataDirectoryButton",
            "RemoveDataDirectoryButton",
            "RefreshTablesButton",
            "SaveAllButton",
        };
        CheckSameGeometry(window, mainButtons, "главный тулбар");

        foreach (var id in tableButtons.Concat(mainButtons))
        {
            var button = window.WaitById(id);
            context.Recorder.Check(
                $"У кнопки {id} есть доступное имя",
                !string.IsNullOrWhiteSpace(button.ReadName()),
                button.ReadName());
            var help = button.Current.HelpText;
            context.Recorder.Check(
                $"Подсказка {id} не содержит имя типа",
                !help.Contains("System.Windows.Data", StringComparison.Ordinal),
                help);
        }

        var saveAllHelp = window.WaitById("SaveAllButton");
        context.Recorder.Check(
            "Кнопка Save all доступна",
            saveAllHelp.Current.IsEnabled,
            "Сохранить всё");

        var search = window.WaitById("SearchBox");
        grid.Cell(0, 0).Click();
        Thread.Sleep(150);
        NativeInput.PressIn(grid, NativeInput.VirtualKey.F, control: true);
        Thread.Sleep(150);
        var focused = AutomationElement.FocusedElement;
        context.Recorder.Check(
            "Ctrl+F фокусирует поиск",
            focused is not null && focused.GetRuntimeId().SequenceEqual(search.GetRuntimeId()),
            focused?.Current.AutomationId ?? string.Empty);

        NativeInput.PressIn(grid, NativeInput.VirtualKey.G, control: true);
        Thread.Sleep(250);
        var filter = window.WaitById("FilterExpander");
        context.Recorder.Check(
            "Ctrl+G раскрывает фильтр",
            filter.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out var rawFilter)
                && rawFilter is ExpandCollapsePattern filterPattern
                && filterPattern.Current.ExpandCollapseState == ExpandCollapseState.Expanded,
            "состояние фильтра");
        context.Recorder.Check(
            "Ctrl+G создаёт условие фильтра",
            window.FindAllById("FilterOperandBox").Count > 0,
            "поля значения фильтра");

        grid.Cell(5, 0).Click();
        Thread.Sleep(150);
        NativeInput.PressIn(grid, NativeInput.VirtualKey.Home);
        WaitForSelectedRow(context, grid, 0, "Home выбирает первую строку");

        var lastValue = LastRowValue(grid);
        NativeInput.PressIn(grid, NativeInput.VirtualKey.End);
        WaitForSelectedRow(context, grid, lastValue, "End выбирает последнюю строку");

        var last = SelectedRowValue(grid);
        NativeInput.PressIn(grid, NativeInput.VirtualKey.PageUp);
        Thread.Sleep(200);
        var afterPageUp = SelectedRowValue(grid);
        context.Recorder.Check(
            "PageUp уводит выделение вверх",
            afterPageUp >= 0 && afterPageUp < last,
            $"было {last}, стало {afterPageUp}");

        NativeInput.PressIn(grid, NativeInput.VirtualKey.PageDown);
        Thread.Sleep(200);
        var afterPageDown = SelectedRowValue(grid);
        context.Recorder.Check(
            "PageDown уводит выделение вниз",
            afterPageDown > afterPageUp,
            $"было {afterPageUp}, стало {afterPageDown}");
    }

    private static void CheckSameGeometry(
        AutomationElement window,
        IReadOnlyList<string> ids,
        string label)
    {
        var rects = ids.Select(id => window.WaitById(id).Current.BoundingRectangle).ToArray();
        var top = rects[0].Top;
        var height = rects[0].Height;
        var sameTop = rects.All(rect => Math.Abs(rect.Top - top) < 0.5);
        var sameHeight = rects.All(rect => Math.Abs(rect.Height - height) < 0.5);
        if (!sameTop || !sameHeight)
        {
            var detail = string.Join(
                " | ",
                ids.Zip(rects, (id, rect) => $"{id}: top={rect.Top:F1}, h={rect.Height:F1}"));
            throw new UiTimeoutException($"{label}: кнопки разной высоты или на разном уровне. {detail}");
        }
    }

    private static void WaitForSelectedRow(
        ScenarioContext context,
        AutomationElement grid,
        int expected,
        string message)
    {
        try
        {
            Ui.WaitUntil(
                () => SelectedRowValue(grid) == expected,
                message,
                TimeSpan.FromSeconds(5));
        }
        catch (UiTimeoutException exception)
        {
            throw new UiTimeoutException(
                $"{exception.Message}; фактически {SelectedRowValue(grid)}");
        }

        context.Recorder.Check(message, true, $"строка {expected}");
    }

    private static int LastRowValue(AutomationElement grid)
    {
        var cell = grid.Cell(grid.RowCount() - 1, 0);
        return int.TryParse(cell.ReadName(), out var value) ? value : -1;
    }

    private static int SelectedRowValue(AutomationElement grid)
    {
        if (!grid.TryGetCurrentPattern(SelectionPattern.Pattern, out var raw)
            || raw is not SelectionPattern selection)
        {
            return -1;
        }

        var selected = selection.Current.GetSelection();
        if (selected.Length == 0)
        {
            return -1;
        }

        // The grid virtualizes rows, so the position among realized DataItems is not the record index.
        // The first Custom child is the uiIndex data cell; later cells may contain larger numbers.
        var firstCell = selected[0].FindFirst(
            TreeScope.Children,
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Custom));
        return firstCell is not null && int.TryParse(firstCell.ReadName(), out var index)
            ? index
            : -1;
    }
}
