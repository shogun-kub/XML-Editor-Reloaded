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
    [InlineData(ShortcutCatalog.DeleteRow, "Delete")]
    public void Gesture_text_is_readable(string action, string expected)
    {
        var text = ShortcutCatalog.GetText(action);

        Assert.Contains(expected, text, StringComparison.OrdinalIgnoreCase);
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
