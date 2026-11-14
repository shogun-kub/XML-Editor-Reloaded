using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Drives the Images tab of the item workspace: the frame chooser, the graphic-type combo, and the
/// decoded preview info lines. Frame selection is a real list selection because the application writes
/// <c>ubGraphicNum</c> from it.
/// </summary>
internal static class ImageUi
{
    private static readonly Regex Digits = new(
        @"\d+",
        RegexOptions.CultureInvariant | RegexOptions.Compiled,
        TimeSpan.FromSeconds(2));

    /// <summary>
    /// Opens the Images tab and waits until the frame chooser has a count.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>The visible frame info text.</returns>
    public static string Open(ScenarioContext context)
    {
        var window = context.Window;
        UiTimeoutException? last = null;
        for (var attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                WorkspaceUi.SelectItemTab(window, context.Strings.Get("ItemTabImages"));
                _ = window.WaitById("ItemImageList", TimeSpan.FromSeconds(5));
                var info = window.WaitById("ItemGraphicImageListInfo", TimeSpan.FromSeconds(5));
                Ui.WaitUntil(
                    () => info.ReadName().Length > 0,
                    "список кадров изображения загружен",
                    TimeSpan.FromSeconds(20));
                return info.ReadName();
            }
            catch (UiTimeoutException exception)
            {
                // An asynchronous reload may have selected the Fields tab again; select Images once more.
                last = exception;
                Thread.Sleep(400);
            }
        }

        throw new UiTimeoutException(
            $"Вкладка «{context.Strings.Get("ItemTabImages")}» не открылась: {last?.Message}");
    }

    /// <summary>
    /// Waits until the frame info shows exactly the expected number of frames.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="count">Expected frame count.</param>
    public static void WaitForCount(ScenarioContext context, int count)
    {
        var expected = context.Strings.Format("ItemImageListInfoFormat", count);
        Ui.WaitUntil(
            () => context.Window.WaitById("ItemGraphicImageListInfo").ReadName() == expected,
            $"кадров изображения: {count}",
            TimeSpan.FromSeconds(60));
    }

    /// <summary>
    /// Reads the number of frames currently offered by the chooser.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>Frame count parsed from the localized info line.</returns>
    /// <exception cref="SmokeCheckException">Thrown when the info line has no number.</exception>
    public static int ReadCount(ScenarioContext context)
    {
        var text = context.Window.WaitById("ItemGraphicImageListInfo").ReadName();
        var match = Digits.Match(text);
        if (!match.Success)
        {
            throw new SmokeCheckException("Счётчик кадров читается", text);
        }

        return int.Parse(match.Value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Selects a frame by its zero-based index, scrolling the virtualized chooser until the item is
    /// realized.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="index">Frame index.</param>
    /// <returns>The realized list item.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the frame cannot be realized.</exception>
    public static AutomationElement SelectFrame(ScenarioContext context, int index)
    {
        var list = context.Window.WaitById("ItemImageList");
        for (var percent = 0; percent <= 100; percent += 5)
        {
            ScrollTo(list, percent);
            var item = FindFrame(list, index);
            if (item is not null)
            {
                item.SelectItem();
                Thread.Sleep(250);
                return item;
            }
        }

        throw new UiTimeoutException($"Кадр {index} не найден в списке изображений.");
    }

    /// <summary>
    /// Returns the frame index selected in the chooser, or <see langword="null"/>.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>Selected frame index.</returns>
    public static int? SelectedFrame(ScenarioContext context)
    {
        var list = context.Window.WaitById("ItemImageList");
        if (!list.TryGetCurrentPattern(SelectionPattern.Pattern, out var raw)
            || raw is not SelectionPattern selection)
        {
            return null;
        }

        var selected = selection.Current.GetSelection().FirstOrDefault();
        if (selected is null)
        {
            return null;
        }

        var text = Ui.ItemText(selected);
        var match = Digits.Match(text);
        return match.Success ? int.Parse(match.Value, CultureInfo.InvariantCulture) : null;
    }

    /// <summary>
    /// Selects a graphic type by its number and waits for the new frame list.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="type">Graphic type number.</param>
    /// <param name="expectedCount">Expected frame count of the new sheet.</param>
    public static void SelectType(ScenarioContext context, int type)
    {
        var combo = context.Window.WaitById("ItemGraphicTypeCombo");
        var caption = type == 0
            ? context.Strings.Get("GraphicTypeGuns")
            : context.Strings.Format("GraphicTypeFormat", type);
        combo.SelectComboItem(caption);
        Thread.Sleep(600);
    }

    /// <summary>
    /// Waits until the frame list reports that the stored index is outside the new sheet.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="index">Stored index.</param>
    /// <param name="maximum">Last index of the new sheet.</param>
    public static void WaitForOutOfRange(ScenarioContext context, int index, int maximum)
    {
        var expected = context.Strings.Format("ItemImageIndexOutOfRangeFormat", index, maximum);
        Ui.WaitUntil(
            () => context.Window.WaitById("ItemGraphicImageListInfo").ReadName() == expected,
            $"индекс {index} вне диапазона 0…{maximum}",
            TimeSpan.FromSeconds(60));
    }

    /// <summary>
    /// Reads the decoded preview dimensions or error text for one image size.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="size">Automation suffix: <c>Big</c>, <c>Medium</c>, or <c>Small</c>.</param>
    /// <returns>Info text of the panel.</returns>
    public static string PreviewInfo(ScenarioContext context, string size) =>
        context.Window.WaitById($"ItemImage{size}Info", TimeSpan.FromSeconds(30)).ReadName();

    /// <summary>
    /// Reads a field value after switching back to the Fields tab.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="field">Field path.</param>
    /// <returns>Field value.</returns>
    public static string ReadField(ScenarioContext context, string field)
    {
        WorkspaceUi.SelectItemTab(context.Window, context.Strings.Get("ItemTabFields"));
        return WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, field));
    }

    private static void ScrollTo(AutomationElement list, int percent)
    {
        if (list.TryGetCurrentPattern(ScrollPattern.Pattern, out var raw)
            && raw is ScrollPattern scroll)
        {
            scroll.SetScrollPercent(ScrollPattern.NoScroll, percent);
            Thread.Sleep(120);
        }
    }

    private static AutomationElement? FindFrame(AutomationElement list, int index) =>
        list.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
            .FirstOrDefault(item =>
            {
                var match = Digits.Match(Ui.ItemText(item));
                return match.Success
                    && int.TryParse(match.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                    && parsed == index;
            });
}
