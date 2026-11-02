using System.IO;
using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S59: the three robot skill-grant fields offer the fixed <c>Lookup.SkillTrait</c> values from
/// "NoTrait" to "Survival", and the selected value is saved as a number. Item 0 stores all three grants
/// and shows the three combos together; item 1697 stores only the utility grant, which is the field
/// edited here.
/// </summary>
internal sealed class RobotSkillGrantScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S59";

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var file = context.Data(DatasetKind.V4).TableFile("Items/Items.xml");
        var original = File.ReadAllText(file);

        WorkspaceUi.OpenItemWorkspace(context);

        // Item 0 is the only record that stores all three robot skill grants (all zero), so it shows
        // the three combos together. Item 1697 stores just the utility grant and is used for the edit.
        WorkspaceUi.SelectItem(context, "0", "Nada");
        var window = context.Window;
        var filter = window.WaitById("ItemFieldFilterBox");
        filter.Focus();
        filter.SetText("Robot");
        Thread.Sleep(400);
        WorkspaceUi.ExpandAll(window.WaitById("ItemTabs"));

        var targeting = window.WaitById("RobotTargetingSkillGrant");
        var targetingValue = targeting.ReadComboSelection();
        context.Recorder.Check(
            "RobotTargetingSkillGrant показывает NoTrait",
            targetingValue.Contains("NoTrait", StringComparison.Ordinal),
            $"выбрано: «{targetingValue}»");

        var utility = window.WaitById("RobotUtilitySkillGrant");
        var utilityValue = utility.ReadComboSelection();
        context.Recorder.Check(
            "RobotUtilitySkillGrant показывает NoTrait",
            utilityValue.Contains("NoTrait", StringComparison.Ordinal),
            $"выбрано: «{utilityValue}»");
        var optionCount = ComboOptionCount(utility);
        context.Recorder.Check(
            "Combo предлагает все 24 значения SkillTrait",
            optionCount == 24,
            $"вариантов: {optionCount}");

        var chassis = window.WaitById("RobotChassisSkillGrant");
        var chassisValue = chassis.ReadComboSelection();
        context.Recorder.Check(
            "RobotChassisSkillGrant показывает NoTrait",
            chassisValue.Contains("NoTrait", StringComparison.Ordinal),
            $"выбрано: «{chassisValue}»");

        WorkspaceUi.SelectItem(context, "1697", "Radio Set");
        utility = WorkspaceUi.ShowField(context, "RobotUtilitySkillGrant");
        var radioValue = utility.ReadComboSelection();
        context.Recorder.Check(
            "У Radio Set RobotUtilitySkillGrant показывает RadioOperator",
            radioValue.Contains("RadioOperator", StringComparison.Ordinal),
            $"выбрано: «{radioValue}»");

        utility = WorkspaceUi.ShowField(context, "RobotUtilitySkillGrant");
        utility.SelectComboItemStartingWith("Survival");
        Ui.WaitUntil(
            () => context.Window.WaitById("ItemSaveButton").IsEnabled(),
            "рабочее место предмета стало грязным");
        context.Recorder.Check(
            "Выбор Survival делает вкладку грязной",
            true,
            "значение записывается числом 23");

        WorkspaceUi.SaveItem(context);

        var expected = original.Replace(
            "<RobotUtilitySkillGrant>21</RobotUtilitySkillGrant>",
            "<RobotUtilitySkillGrant>23</RobotUtilitySkillGrant>",
            StringComparison.Ordinal);
        var saved = File.ReadAllText(file);
        context.Recorder.Check(
            "В файл записано число 23, остальные поля не тронуты",
            saved == expected,
            saved == expected ? null : FirstDifference(expected, saved));
        context.Recorder.Check(
            "Отсутствующие поля не добавлены в разреженную запись",
            XDocument.Load(file).Root!.Elements()
                .First(element => element.Element("uiIndex")?.Value.Trim() == "1697")
                .Elements()
                .All(element => element.Name.LocalName is not ("RobotTargetingSkillGrant" or "RobotChassisSkillGrant")),
            null);
        context.ExpectChange(DatasetKind.V4, "TableData/Items/Items.xml");
    }

    /// <summary>
    /// Expands a combo box, counts its realized options, and closes it again.
    /// </summary>
    /// <param name="combo">Combo box.</param>
    /// <returns>Number of visible list items.</returns>
    private static int ComboOptionCount(AutomationElement combo)
    {
        combo.ExpandCombo();
        Thread.Sleep(250);
        var count = combo
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
            .Count;
        ScenarioUi.Collapse(combo);
        Thread.Sleep(120);
        return count;
    }

    private static string FirstDifference(string expected, string actual)
    {
        var length = Math.Min(expected.Length, actual.Length);
        for (var index = 0; index < length; index++)
        {
            if (expected[index] != actual[index])
            {
                var start = Math.Max(0, index - 40);
                return $"позиция {index}: ожидалось «{expected.Substring(start, Math.Min(80, expected.Length - start))}», "
                    + $"получено «{actual.Substring(start, Math.Min(80, actual.Length - start))}»";
            }
        }

        return $"длина изменилась: {expected.Length} → {actual.Length}";
    }
}

/// <summary>
/// S60: the weapon sound fields resolve their stored indices through <c>Sounds.Sounds</c> and
/// <c>Sounds.BurstSounds</c> and show the record text, including the explicit unknown marker for a
/// value that has no sound record.
/// </summary>
internal sealed class WeaponSoundPickerScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S60";

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

        var glock = DataOperationsUi.FindRowIndex(grid, "2");
        context.Recorder.Check(
            "Строка Glock 18 (item 2) показывает имя",
            WorkspaceUi.ReadCell(grid, glock, 1).Contains("Glock 18", StringComparison.Ordinal),
            WorkspaceUi.ReadCell(grid, glock, 1));

        AssertField(context, grid, glock, "sSound", "Glock18 Single Shot");
        AssertField(context, grid, glock, "sBurstSound", "Glock18 Burst");
        AssertField(context, grid, glock, "sSilencedBurstSound", "SILENCER Burst");
        AssertField(context, grid, glock, "sReloadSound", "PISTOL RELOAD");
        AssertField(context, grid, glock, "sLocknLoadSound", "PISTOL LNL");

        // The search finds a weapon by name and by its identifier; the last check uses that to bring a
        // record with an empty burst-sound reference into view.
        var search = window.WaitById("SearchBox");
        search.SetText("Glock 18");
        Ui.WaitUntil(() => grid.RowCount() == 1, "поиск по имени оставил одну строку", TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Поиск по имени «Glock 18» находит запись",
            WorkspaceUi.ReadCell(grid, 0, 1).Contains("Glock 18", StringComparison.Ordinal),
            WorkspaceUi.ReadCell(grid, 0, 1));

        search.SetText("1761");
        Ui.WaitUntil(() => grid.RowCount() == 1, "поиск по ID оставил одну строку", TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Поиск по ID «1761» находит запись",
            WorkspaceUi.ReadCell(grid, 0, 0) == "1761",
            WorkspaceUi.ReadCell(grid, 0, 0));

        var burst = DetailsUi.ShowField(context, grid, 0, "sBurstSound");
        var unknown = context.Strings.Format("LookupUnknownFormat", "505");
        var burstValue = WorkspaceUi.ReadFieldValue(burst);
        context.Recorder.Check(
            "Пустой звук 505 показывается как unknown и не подменяется другим именем",
            burstValue == unknown,
            $"«{burstValue}», ожидалось «{unknown}»");
    }

    private static void AssertField(
        ScenarioContext context,
        AutomationElement grid,
        int row,
        string field,
        string expectedFragment)
    {
        var editor = DetailsUi.ShowField(context, grid, row, field);
        var value = WorkspaceUi.ReadFieldValue(editor);
        context.Recorder.Check(
            $"{field} показывает «{expectedFragment}»",
            value.Contains(expectedFragment, StringComparison.OrdinalIgnoreCase),
            $"«{value}»");
    }
}

/// <summary>
/// S61: the explosive animation identifier is shown as the name of the <c>ExplosionData</c> row while
/// the document keeps the numeric index. Choosing another entry requires the searchable picker's
/// Enter key and is left to the manual pass; the display and the stored number are checked here.
/// </summary>
internal sealed class ExplosiveAnimationScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S61";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Items/Explosives.xml");

        ScenarioUi.OpenTable(context, "Items.Explosives");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.Explosives открыт");
        var grid = window.WaitById("RowsGrid");
        var row = DataOperationsUi.FindRowIndex(grid, "0");

        var animation = DetailsUi.ShowField(context, grid, row, "ubAnimationID");
        Ui.WaitUntil(
            () => WorkspaceUi.ReadFieldValue(animation).Length > 0,
            "ubAnimationID показывает значение",
            TimeSpan.FromSeconds(10));
        var shown = WorkspaceUi.ReadFieldValue(animation);
        context.Recorder.Check(
            "ubAnimationID показывает имя Stun для значения 4",
            shown.Contains("Stun", StringComparison.Ordinal),
            $"выбрано: «{shown}»");

        var raw = XDocument.Load(file).Root!.Elements()
            .First(element => element.Element("uiIndex")?.Value.Trim() == "0")
            .Element("ubAnimationID")?.Value.Trim();
        context.Recorder.Check(
            "В XML хранится числовой идентификатор 4, а не имя",
            raw == "4",
            $"значение: «{raw}»");
        context.Recorder.Note(
            "Выбор другого значения в searchable picker требует клавиши Enter и проверяется вручную.");
    }
}

/// <summary>
/// S62: all twelve pocket fields of a load-bearing equipment record resolve their indices through
/// <c>Items.Pockets</c>, including pocket 0 shown as "Nothing".
/// </summary>
internal sealed class LbePocketPickerScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S62";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        ScenarioUi.OpenTable(context, "Items.LoadBearingEquipment");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.LoadBearingEquipment открыт");
        var grid = window.WaitById("RowsGrid");
        var row = DataOperationsUi.FindRowIndex(grid, "0");

        var expected = new[]
        {
            "Tiny General",
            "Tiny General",
            "Belt Clip",
            "Nothing",
            "Nothing",
            "Nothing",
            "Nothing",
            "Nothing",
            "Nothing",
            "Nothing",
            "Nothing",
            "Nothing",
        };

        for (var index = 1; index <= 12; index++)
        {
            var field = $"lbePocketIndex{index}";
            var editor = DetailsUi.ShowField(context, grid, row, field);
            var value = WorkspaceUi.ReadFieldValue(editor);
            context.Recorder.Check(
                $"{field} показывает «{expected[index - 1]}»",
                value == expected[index - 1],
                $"«{value}»");
        }
    }
}

/// <summary>
/// S63: both explosion sound fields resolve <c>Sounds.Sounds</c> record text, the alternate field
/// offers <c>-1</c> as "None", and that value passes validation without a warning.
/// </summary>
internal sealed class ExplosionSoundPickerScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S63";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        ScenarioUi.OpenTable(context, "Items.ExplosionData");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.ExplosionData открыт");
        var grid = window.WaitById("RowsGrid");
        context.Recorder.Check(
            "Items.ExplosionData содержит 16 записей",
            grid.RowCount() == 16,
            $"строк: {grid.RowCount()}");

        var row = DataOperationsUi.FindRowIndex(grid, "2");
        var main = DetailsUi.ShowField(context, grid, row, "ExplosionSoundID");
        var mainValue = WorkspaceUi.ReadFieldValue(main);
        context.Recorder.Check(
            "ExplosionSoundID показывает имя звука из Sounds",
            mainValue.Contains("explosion 12.wav", StringComparison.OrdinalIgnoreCase),
            $"«{mainValue}»");

        var alternate = DetailsUi.ShowField(context, grid, row, "AltExplosionSoundID");
        var alternateValue = WorkspaceUi.ReadFieldValue(alternate);
        context.Recorder.Check(
            "AltExplosionSoundID показывает None для -1",
            alternateValue == "None",
            $"«{alternateValue}»");

        window.WaitById("ValidateButton").InvokeButton();
        var summary = WorkspaceUi.WaitForStatus(
            context,
            "EditorStatus",
            text => text == context.Strings.Format("EditorValidationSummary", 0),
            "валидация не нашла замечаний",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check(
            "None для AltExplosionSoundID не даёт предупреждения",
            summary == context.Strings.Format("EditorValidationSummary", 0),
            summary);
    }
}

/// <summary>
/// S64: the positional sound lists open and their virtual <c>Text</c> value is edited in place without
/// turning the record into a child element.
/// </summary>
internal sealed class SoundTextScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S64";

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        AssertList(context, "Sounds.Sounds", "Sounds/Sounds.xml", 506, "SOUNDS\\RICOCHET 01.wav");
        AssertList(context, "Sounds.BurstSounds", "Sounds/BurstSounds.xml", 128, "NOSOUND");
    }

    private static void AssertList(
        ScenarioContext context,
        string tableName,
        string relativePath,
        int expectedRows,
        string expectedFirst)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile(relativePath);
        var original = File.ReadAllText(file);

        ScenarioUi.OpenTable(context, tableName);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            $"{tableName} открыт");
        var grid = ItemRelationUi.ActiveGrid(window);
        context.Recorder.Check(
            $"{tableName} содержит {expectedRows} записей",
            grid.RowCount() == expectedRows,
            $"строк: {grid.RowCount()}");
        context.Recorder.Check(
            $"{tableName} показывает текст первой записи",
            WorkspaceUi.ReadCell(grid, 0, 0) == expectedFirst,
            WorkspaceUi.ReadCell(grid, 0, 0));

        var replacement = expectedFirst + " (smoke)";
        DetailsUi.EditField(context, grid, 0, "Text", replacement);
        Ui.WaitUntil(
            () => window.WaitById("SaveButton").IsEnabled(),
            $"{tableName} стала грязной");
        window.WaitById("SaveButton").InvokeButton();
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorSaved"),
            $"{tableName} сохранён",
            TimeSpan.FromSeconds(30));

        var saved = File.ReadAllText(file);
        var marker = $"<SOUND>{expectedFirst}</SOUND>";
        var start = original.IndexOf(marker, StringComparison.Ordinal);
        var expected = original[..start] + $"<SOUND>{replacement}</SOUND>" + original[(start + marker.Length)..];
        context.Recorder.Check(
            $"{tableName}: изменена только первая запись, дочерние элементы не появились",
            saved == expected,
            saved == expected ? null : "текст файла отличается от ожидаемого");
        context.ExpectChange(DatasetKind.V4, "TableData/" + relativePath);
    }
}

/// <summary>
/// S65: opening tables that use the new value sources and asking to save without edits leaves every
/// file byte-for-byte untouched.
/// </summary>
internal sealed class NoRewriteScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S65";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var tableData = context.Data(DatasetKind.V4).TableDataDirectory;
        var hashBefore = DataOperationsUi.HashTree(tableData);

        foreach (var table in new[] { "Items.Explosives", "Items.LoadBearingEquipment", "Items.ExplosionData" })
        {
            ScenarioUi.OpenTable(context, table);
            ScenarioUi.WaitForEditorStatus(
                context,
                text => text == context.Strings.Get("EditorReady"),
                $"{table} открыт");
            context.Recorder.Check(
                $"{table}: сохранение чистой таблицы недоступно",
                !window.WaitById("SaveButton").IsEnabled(),
                $"IsEnabled: {window.WaitById("SaveButton").IsEnabled()}");
        }

        var saveAll = window.WaitById("SaveAllButton");
        if (saveAll.IsEnabled())
        {
            saveAll.InvokeButton();
            Thread.Sleep(500);
        }

        context.Recorder.Check(
            "«Сохранить всё» на чистых таблицах не перезаписывает файлы",
            DataOperationsUi.HashTree(tableData) == hashBefore,
            null);
    }
}
