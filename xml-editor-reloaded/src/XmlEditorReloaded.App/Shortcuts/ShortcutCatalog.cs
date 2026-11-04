using System.Windows.Input;

namespace XmlEditorReloaded.App.Shortcuts;

/// <summary>
/// Central definition of keyboard gestures shown in menus, tooltips, and used by input bindings.
/// </summary>
public static class ShortcutCatalog
{
    /// <summary>
    /// Gesture for opening a table from the table list.
    /// </summary>
    public const string OpenTable = "OpenTable";

    /// <summary>
    /// Gesture for saving the active editor.
    /// </summary>
    public const string Save = "Save";

    /// <summary>
    /// Gesture for saving every open editor.
    /// </summary>
    public const string SaveAll = "SaveAll";

    /// <summary>
    /// Gesture for undoing the last edit.
    /// </summary>
    public const string Undo = "Undo";

    /// <summary>
    /// Gesture for redoing the last undone edit.
    /// </summary>
    public const string Redo = "Redo";

    /// <summary>
    /// Gesture for focusing the table search box.
    /// </summary>
    public const string Find = "Find";

    /// <summary>
    /// Gesture for finding the selected record in the table tree.
    /// </summary>
    public const string FindInTree = "FindInTree";

    /// <summary>
    /// Gesture for switching between the grid and the XML code view.
    /// </summary>
    public const string GridCode = "GridCode";

    /// <summary>
    /// Gesture for opening the data-set validation window.
    /// </summary>
    public const string Problems = "Problems";

    /// <summary>
    /// Gesture for opening the data-set search window.
    /// </summary>
    public const string DatasetSearch = "DatasetSearch";

    /// <summary>
    /// Gesture for opening the settings dialog.
    /// </summary>
    public const string Settings = "Settings";

    /// <summary>
    /// Gesture for closing the active editor tab.
    /// </summary>
    public const string CloseTab = "CloseTab";

    /// <summary>
    /// Gesture for reloading the active table from disk.
    /// </summary>
    public const string Reload = "Reload";

    /// <summary>
    /// Gesture for validating the active table.
    /// </summary>
    public const string Validate = "Validate";

    /// <summary>
    /// Gesture for opening the About window.
    /// </summary>
    public const string About = "About";

    /// <summary>
    /// Gesture for adding a row.
    /// </summary>
    public const string AddRow = "AddRow";

    /// <summary>
    /// Gesture for duplicating the selected row.
    /// </summary>
    public const string DuplicateRow = "DuplicateRow";

    /// <summary>
    /// Gesture for deleting the selected row.
    /// </summary>
    public const string DeleteRow = "DeleteRow";

    /// <summary>
    /// Gesture for changing a lookup record identifier.
    /// </summary>
    public const string ChangeId = "ChangeId";

    /// <summary>
    /// Gesture for showing where a lookup record is used.
    /// </summary>
    public const string LookupUsage = "LookupUsage";

    /// <summary>
    /// Gesture for adding a filter criterion.
    /// </summary>
    public const string FilterAdd = "FilterAdd";

    /// <summary>
    /// Gesture for clearing all filter criteria.
    /// </summary>
    public const string FilterClear = "FilterClear";

    /// <summary>
    /// Gesture for focusing the filter value field.
    /// </summary>
    public const string FilterGoTo = "FilterGoTo";

    /// <summary>
    /// Gesture for creating a new item.
    /// </summary>
    public const string NewItem = "NewItem";

    private static readonly IReadOnlyDictionary<string, KeyGesture> Gestures =
        new Dictionary<string, KeyGesture>(StringComparer.Ordinal)
        {
            [OpenTable] = new(Key.O, ModifierKeys.Control),
            [Save] = new(Key.S, ModifierKeys.Control),
            [SaveAll] = new(Key.S, ModifierKeys.Control | ModifierKeys.Shift),
            [Undo] = new(Key.Z, ModifierKeys.Control),
            [Redo] = new(Key.Y, ModifierKeys.Control),
            [Find] = new(Key.F, ModifierKeys.Control),
            [FindInTree] = new(Key.T, ModifierKeys.Control | ModifierKeys.Shift),
            [GridCode] = new(Key.E, ModifierKeys.Control),
            [Problems] = new(Key.P, ModifierKeys.Control | ModifierKeys.Shift),
            [DatasetSearch] = new(Key.F, ModifierKeys.Control | ModifierKeys.Shift),
            [Settings] = new(Key.OemComma, ModifierKeys.Control),
            [CloseTab] = new(Key.W, ModifierKeys.Control),
            [Reload] = new(Key.F5, ModifierKeys.None),
            [Validate] = new(Key.F8, ModifierKeys.None),
            [About] = new(Key.F1, ModifierKeys.None),
            [AddRow] = new(Key.Insert, ModifierKeys.None),
            [DuplicateRow] = new(Key.D, ModifierKeys.Control),
            [DeleteRow] = new(Key.Delete, ModifierKeys.None),
            [ChangeId] = new(Key.L, ModifierKeys.Control),
            [LookupUsage] = new(Key.U, ModifierKeys.Control),
            [FilterAdd] = new(Key.A, ModifierKeys.Control | ModifierKeys.Shift),
            [FilterClear] = new(Key.X, ModifierKeys.Control | ModifierKeys.Shift),
            [FilterGoTo] = new(Key.G, ModifierKeys.Control),
            [NewItem] = new(Key.N, ModifierKeys.Control),
        };

    /// <summary>
    /// Gets every action identifier defined by the catalog.
    /// </summary>
    public static IReadOnlyCollection<string> Actions => Gestures.Keys.ToArray();

    /// <summary>
    /// Gets the gesture for an action.
    /// </summary>
    /// <param name="action">Action identifier from this class.</param>
    /// <returns>The configured gesture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The action is not defined.</exception>
    public static KeyGesture Get(string action)
    {
        if (Gestures.TryGetValue(action, out var gesture))
        {
            return gesture;
        }

        throw new ArgumentOutOfRangeException(nameof(action), action, "Unknown shortcut action.");
    }

    /// <summary>
    /// Gets the localized display text for an action, for example <c>Ctrl+S</c>.
    /// </summary>
    /// <param name="action">Action identifier from this class.</param>
    /// <returns>Display text of the configured gesture.</returns>
    public static string GetText(string action) => Format(Get(action));

    private static string Format(KeyGesture gesture)
    {
        var parts = new List<string>(4);
        if (gesture.Modifiers.HasFlag(ModifierKeys.Control))
        {
            parts.Add("Ctrl");
        }

        if (gesture.Modifiers.HasFlag(ModifierKeys.Shift))
        {
            parts.Add("Shift");
        }

        if (gesture.Modifiers.HasFlag(ModifierKeys.Alt))
        {
            parts.Add("Alt");
        }

        if (gesture.Modifiers.HasFlag(ModifierKeys.Windows))
        {
            parts.Add("Win");
        }

        parts.Add(KeyText(gesture.Key));
        return string.Join("+", parts);
    }

    private static string KeyText(Key key) => key switch
    {
        Key.OemComma => ",",
        Key.OemPeriod => ".",
        Key.OemPlus => "+",
        Key.OemMinus => "-",
        _ => key.ToString(),
    };
}
