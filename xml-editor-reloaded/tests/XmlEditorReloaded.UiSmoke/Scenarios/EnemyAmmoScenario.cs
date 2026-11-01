using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S58: the enemy-ammo window presents the seven <c>uiIndex=19</c> rows with ammunition pickers. The
/// scenario checks the rows and their labels; changing a picker value and the close prompt need
/// keyboard input and stay in the manual checklist.
/// </summary>
internal sealed class EnemyAmmoScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S58";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        ScenarioUi.OpenMenuItem(context, "DataMenu", "EnemyAmmoButton");
        var window = ScenarioUi.WaitForWindowWithId(
            context.App,
            "EnemyAmmoWindow",
            "окно боеприпасов врагов",
            TimeSpan.FromSeconds(90));
        var texts = window
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static text => text.ReadName())
            .ToList();

        var profiles = new[]
        {
            "Default Enemy Ammo",
            "Admin Enemy Ammo",
            "Regular Enemy Ammo",
            "Elite Enemy Ammo",
            "Green Militia Ammo",
            "Regular MilitiaAmmo",
            "Elite Militia Ammo",
        };
        foreach (var profile in profiles)
        {
            context.Recorder.Check(
                $"Строка «{profile}» показана",
                texts.Any(text => string.Equals(text, profile, StringComparison.Ordinal)),
                string.Join(" | ", texts.Take(12)));
        }

        var slotPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("EnemyAmmoFieldFormat"));
        var slots = texts.Count(text => text.StartsWith(slotPrefix, StringComparison.Ordinal));
        context.Recorder.Check("У строк есть подписи слотов", slots >= 21, $"подписей слотов: {slots}");
        context.Recorder.Check(
            "Кнопки «Сохранить», «Перезагрузить» и «Закрыть» доступны",
            window.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))
                .Select(static button => button.ReadName())
                .Where(static name => !string.IsNullOrWhiteSpace(name))
                .Count() >= 3,
            null);

        window.FindFirst(
                TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                    new PropertyCondition(AutomationElement.NameProperty, context.Strings.Get("EnemyAmmoCloseText"))))
            ?.InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "EnemyAmmoWindow", TimeSpan.FromSeconds(30));
        context.Recorder.Note("Правка значений и запрос сохранения при закрытии требуют ввода и остаются в ручном чек-листе.");
    }
}
