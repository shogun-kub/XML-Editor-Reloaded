using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using XmlEditorReloaded.App.Shortcuts;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests the single shortcut definition used by menus, tooltips, and input bindings.
/// </summary>
public sealed class ShortcutCatalogTests
{
    /// <summary>
    /// Every action must have a distinct gesture; otherwise two commands would compete for one key.
    /// </summary>
    [Fact]
    public void Gestures_are_unique()
    {
        var duplicates = ShortcutCatalog.Actions
            .GroupBy(ShortcutCatalog.Get)
            .Where(group => group.Count() > 1)
            .Select(group => string.Join(", ", group))
            .ToArray();

        Assert.Empty(duplicates);
    }

    /// <summary>
    /// The catalog exposes readable gesture text for menus and tooltips.
    /// </summary>
    /// <param name="action">Action identifier.</param>
    /// <param name="expected">Expected display fragment.</param>
    [Theory]
    [InlineData(ShortcutCatalog.Save, "Ctrl")]
    [InlineData(ShortcutCatalog.SaveAll, "Shift")]
    [InlineData(ShortcutCatalog.Settings, ",")]
    [InlineData(ShortcutCatalog.Reload, "F5")]
    [InlineData(ShortcutCatalog.Help, "F1")]
    [InlineData(ShortcutCatalog.DeleteRow, "Delete")]
    public void Gesture_text_is_readable(string action, string expected)
    {
        var text = ShortcutCatalog.GetText(action);

        Assert.Contains(expected, text, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// The tooltip markup extension must return a visual element with a bound action text and a literal
    /// shortcut, not a raw MultiBinding that WPF would render as its type name.
    /// </summary>
    [Fact]
    public void Shortcut_tip_returns_visual_content()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var extension = new ShortcutTipExtension
                {
                    Action = ShortcutCatalog.Save,
                    Text = new Binding("Text"),
                };
                var tip = Assert.IsType<ToolTip>(extension.ProvideValue(null!));
                var text = Assert.IsType<TextBlock>(tip.Content);
                var runs = text.Inlines.OfType<Run>().ToArray();
                Assert.Equal(2, runs.Length);
                Assert.NotNull(BindingOperations.GetBindingExpression(runs[0], Run.TextProperty));
                Assert.Contains("Ctrl+S", runs[1].Text, StringComparison.Ordinal);
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        Assert.Null(failure);
    }

    /// <summary>
    /// An unknown action must fail loudly rather than silently binding the wrong key.
    /// </summary>
    [Fact]
    public void Unknown_action_is_rejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ShortcutCatalog.Get("unknown"));
    }
}
