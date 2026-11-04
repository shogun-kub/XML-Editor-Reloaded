using System.IO;
using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S100: the table tree groups files by folder, filters by file name, folds and unfolds, and finds the
/// selected table with the filter cleared.
/// </summary>
internal sealed class TableTreeScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S100";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var tree = window.WaitById("TableTree");
        var filter = window.WaitById("TableTreeFilterBox");

        context.Recorder.Check(
            "Дерево таблиц показано по умолчанию",
            window.FindById("TableTreeTab") is not null,
            "вкладка дерева");
        context.Recorder.Check(
            "Фильтр дерева пуст",
            filter.ReadValue() == string.Empty,
            filter.ReadValue());
        context.Recorder.Check(
            "В дереве есть папки",
            TreeNames(tree).Any(name => name == "Items"),
            string.Join(" | ", TreeNames(tree).Take(12)));

        filter.SetText("Weapons");
        Ui.WaitUntil(
            () => TreeNames(tree).Contains("Weapons"),
            "фильтр дерева показывает Weapons",
            TimeSpan.FromSeconds(10));
        var filtered = TreeNames(tree);
        context.Recorder.Check(
            "Фильтр оставляет только совпадающие файлы",
            filtered.Contains("Weapons") && !filtered.Contains("Backgrounds"),
            string.Join(" | ", filtered.Take(12)));
        Screenshots.Capture(
            context.App,
            Path.Combine(context.Directory, "S100-tree-filtered.png"));

        var weapons = TreeItems(tree).First(item => item.ReadName() == "Weapons");
        weapons.SelectItem();
        NativeInput.DoubleClick(weapons);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.Weapons открыт двойным щелчком из дерева");
        ScenarioUi.WaitForEditorTabCount(window, 1);

        var find = window.WaitById("TableTreeFindButton");
        find.InvokeButton();
        Ui.WaitUntil(
            () => filter.ReadValue() == string.Empty,
            "Find in tree очищает фильтр",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check("Фильтр после Find in tree пуст", true, filter.ReadValue());
        context.Recorder.Check(
            "Выбранная таблица остаётся в дереве",
            TreeNames(tree).Contains("Weapons"),
            string.Join(" | ", TreeNames(tree).Take(12)));
        Screenshots.Capture(
            context.App,
            Path.Combine(context.Directory, "S100-tree-selected.png"));

        var fold = window.WaitById("TableTreeFoldButton");
        fold.InvokeButton();
        Thread.Sleep(150);
        context.Recorder.Check(
            "Fold all сворачивает папку Items",
            !IsExpanded(TreeItems(tree).First(item => item.ReadName() == "Items")),
            "состояние Items");

        var unfold = window.WaitById("TableTreeUnfoldButton");
        unfold.InvokeButton();
        Thread.Sleep(150);
        context.Recorder.Check(
            "Unfold all разворачивает папку Items",
            IsExpanded(TreeItems(tree).First(item => item.ReadName() == "Items")),
            "состояние Items");
    }

    private static IReadOnlyList<string> TreeNames(AutomationElement tree) =>
        TreeItems(tree).Select(item => item.ReadName()).ToArray();

    private static IReadOnlyList<AutomationElement> TreeItems(AutomationElement tree) =>
        tree.FindAll(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TreeItem))
            .Cast<AutomationElement>()
            .ToArray();

    private static bool IsExpanded(AutomationElement item) =>
        item.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out var raw)
        && raw is ExpandCollapsePattern pattern
        && pattern.Current.ExpandCollapseState == ExpandCollapseState.Expanded;
}
