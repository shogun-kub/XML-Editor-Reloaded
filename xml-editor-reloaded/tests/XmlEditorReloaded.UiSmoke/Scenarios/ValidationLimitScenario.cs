using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S66: changing <c>lbeClass</c> to a class with a smaller pocket limit keeps every pocket value and
/// reports <c>validation.class_value_limit</c> for the fields that exceed the new limit; validation
/// never writes the file.
/// </summary>
internal sealed class LbeClassLimitScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S66";

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Items/LoadBearingEquipment.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(file);

        ScenarioUi.OpenTable(context, "Items.LoadBearingEquipment");
        WaitReady(context, "Items.LoadBearingEquipment");
        var grid = window.WaitById("RowsGrid");
        var search = window.WaitById("SearchBox");
        search.SetText("22");
        var row = WaitForRow(grid, "22");

        var classCombo = DetailsUi.ShowField(context, grid, row, "lbeClass");
        var classBefore = ReadClassDisplay(classCombo);
        context.Recorder.Check(
            "lbeClass показывает Combat Pack",
            classBefore.Contains("Combat Pack", StringComparison.Ordinal),
            classBefore);

        var pocketsBefore = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var field in new[] { "lbePocketIndex6", "lbePocketIndex7" })
        {
            pocketsBefore[field] = DetailsUi.ShowField(context, grid, row, field).ReadValue();
        }

        classCombo = DetailsUi.ShowField(context, grid, row, "lbeClass");
        classCombo.SelectComboItemStartingWith("Thigh Pack");
        Ui.WaitUntil(
            () => window.WaitById("SaveButton").IsEnabled(),
            "смена класса сделала таблицу грязной");
        var classAfter = ReadClassDisplay(DetailsUi.ShowField(context, grid, row, "lbeClass"));
        context.Recorder.Check(
            "lbeClass изменён на Thigh Pack",
            classAfter.Contains("Thigh Pack", StringComparison.Ordinal),
            classAfter);

        var pocketsAfter = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var field in new[] { "lbePocketIndex6", "lbePocketIndex7" })
        {
            pocketsAfter[field] = DetailsUi.ShowField(context, grid, row, field).ReadValue();
        }

        context.Recorder.Check(
            "Карманы 6 и 7 не очищены и не нормализованы",
            pocketsAfter["lbePocketIndex6"] == pocketsBefore["lbePocketIndex6"]
                && pocketsAfter["lbePocketIndex7"] == pocketsBefore["lbePocketIndex7"]
                && pocketsAfter.Values.All(static value => value.Length > 0),
            string.Join(", ", pocketsAfter.Select(pair => $"{pair.Key}={pair.Value}")));
        context.Recorder.Check(
            "До сохранения XML не изменился",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);

        window.WaitById("ValidateButton").InvokeButton();
        WaitValidationSummary(context, "проверка таблицы LBE");
        var issues = ValidationTexts(window);
        var limitIssues = issues
            .Where(text => text.Contains("validation.class_value_limit", StringComparison.Ordinal))
            .ToList();
        context.Recorder.Check(
            "Валидация сообщает class_value_limit для полей 6 и 7",
            limitIssues.Any(text => text.Contains("lbePocketIndex6", StringComparison.Ordinal))
                && limitIssues.Any(text => text.Contains("lbePocketIndex7", StringComparison.Ordinal)),
            limitIssues.Count == 0 ? "нет предупреждений class_value_limit" : string.Join(" | ", limitIssues));
        context.Recorder.Check(
            "Предупреждение относится к записи 22",
            limitIssues.Any(text => text.Contains("[22/", StringComparison.Ordinal)),
            string.Join(" | ", limitIssues.Take(2)));

        window.WaitById("UndoButton").InvokeButton();
        Ui.WaitUntil(
            () => !window.WaitById("SaveButton").IsEnabled(),
            "отмена вернула таблицу в чистое состояние");
        var restored = ReadClassDisplay(DetailsUi.ShowField(context, grid, row, "lbeClass"));
        context.Recorder.Check(
            "Undo вернул класс Combat Pack",
            restored.Contains("Combat Pack", StringComparison.Ordinal),
            restored);
        context.Recorder.Check(
            "Проверка и Undo не изменили XML",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);
    }

    private static string ReadClassDisplay(AutomationElement combo)
    {
        var selection = combo.ReadComboSelection();
        return string.IsNullOrWhiteSpace(selection) ? combo.ReadComboText() : selection;
    }

    private static AutomationElement WaitForRow(AutomationElement grid, string key) =>
        Ui.WaitFor(
            () => grid
                .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
                .FirstOrDefault(row => row.ReadRowCells().Contains(key, StringComparer.Ordinal)),
            $"строка с ключом {key}");

    private static void WaitReady(ScenarioContext context, string table)
    {
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            $"{table} открыт",
            TimeSpan.FromSeconds(90));
    }

    private static void WaitValidationSummary(ScenarioContext context, string description)
    {
        var prefix = ScenarioUi.FormatPrefix(context.Strings.Get("EditorValidationSummary"));
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text.StartsWith(prefix, StringComparison.Ordinal),
            description,
            TimeSpan.FromSeconds(90));
    }

    private static List<string> ValidationTexts(AutomationElement window) =>
        WorkspaceUi.ListItemTexts(window.WaitById("ValidationList"))
            .Where(static text => text.Length > 0)
            .ToList();
}

/// <summary>
/// S67: setting two exclusive launcher flags on one item reports both fields in
/// <c>validation.exclusive_flags</c>; clearing one flag removes the warning and keeps the other value.
/// The legacy "N/A" state with all four flags unset stays valid.
/// </summary>
internal sealed class ExclusiveLauncherFlagsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S67";

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Items/Items.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(file);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1", "Glock 17");

        window.WaitById("ItemValidateButton").InvokeButton();
        WaitValidationSummary(context);
        context.Recorder.Check(
            "Все четыре флага = 0 не дают exclusive_flags",
            !ValidationTexts(window).Any(text => text.Contains("validation.exclusive_flags", StringComparison.Ordinal)),
            string.Join(" | ", ValidationTexts(window).Take(3)));

        EditField(context, "Cannon", "1");
        EditField(context, "Mortar", "1");
        Ui.WaitUntil(
            () => window.WaitById("ItemSaveButton").IsEnabled(),
            "два флага сделали вкладку грязной");
        context.Recorder.Check(
            "Cannon = 1",
            WorkspaceUi.ReadFieldValue(ShowField(context, "Cannon")) == "1",
            null);
        context.Recorder.Check(
            "Mortar = 1",
            WorkspaceUi.ReadFieldValue(ShowField(context, "Mortar")) == "1",
            null);

        window.WaitById("ItemValidateButton").InvokeButton();
        Ui.WaitUntil(
            () => ValidationTexts(window).Any(text =>
                text.Contains("validation.exclusive_flags", StringComparison.Ordinal)
                && text.Contains("[1/", StringComparison.Ordinal)),
            "валидация сообщает о двух флагах",
            TimeSpan.FromSeconds(120));
        var conflict = ValidationTexts(window)
            .First(text => text.Contains("validation.exclusive_flags", StringComparison.Ordinal));
        context.Recorder.Check(
            "Предупреждение перечисляет Cannon и Mortar",
            conflict.Contains("Cannon", StringComparison.Ordinal)
                && conflict.Contains("Mortar", StringComparison.Ordinal),
            conflict);

        EditField(context, "Mortar", "0");
        window.WaitById("ItemValidateButton").InvokeButton();
        Ui.WaitUntil(
            () => !ValidationTexts(window).Any(text => text.Contains("validation.exclusive_flags", StringComparison.Ordinal)),
            "предупреждение исчезло после снятия флага",
            TimeSpan.FromSeconds(120));
        context.Recorder.Check(
            "Cannon сохранил 1, Mortar стал 0",
            WorkspaceUi.ReadFieldValue(ShowField(context, "Cannon")) == "1"
                && WorkspaceUi.ReadFieldValue(ShowField(context, "Mortar")) == "0",
            null);

        WorkspaceUi.UndoAll(context, "ItemUndoButton", "ItemSaveButton");
        context.Recorder.Check(
            "Флаги и XML восстановлены",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);
    }

    private static AutomationElement ShowField(ScenarioContext context, string field)
    {
        var window = context.Window;
        var filter = window.WaitById("ItemFieldFilterBox");
        filter.Focus();
        filter.SetText(field);
        Thread.Sleep(400);
        WorkspaceUi.ExpandAll(window.WaitById("ItemTabs"));
        return window.WaitById(field, TimeSpan.FromSeconds(20));
    }

    private static void EditField(ScenarioContext context, string field, string value)
    {
        var editor = ShowField(context, field);
        if (editor.Current.ControlType == ControlType.CheckBox)
        {
            editor.SetChecked(value == "1");
        }
        else
        {
            editor.Focus();
            Thread.Sleep(100);
            editor.SetText(value);
            context.Window.WaitById("ItemSearchBox").SetFocus();
        }

        Thread.Sleep(300);
    }

    private static void WaitValidationSummary(ScenarioContext context)
    {
        var prefix = ScenarioUi.FormatPrefix(context.Strings.Get("EditorValidationSummary"));
        WorkspaceUi.WaitForStatus(
            context,
            "ItemStatus",
            text => text.StartsWith(prefix, StringComparison.Ordinal),
            "проверка Items.Items",
            TimeSpan.FromSeconds(120));
    }

    private static List<string> ValidationTexts(AutomationElement window) =>
        WorkspaceUi.ListItemTexts(window.WaitById("ItemValidationList"))
            .Where(static text => text.Length > 0)
            .ToList();
}

/// <summary>
/// S68: the shipped <c>mSmall1Status = 600</c> of Kelly's Premium kit stays visible and is saved as-is,
/// and changing another status to 120 adds the same range warning without clamping either value.
/// </summary>
internal sealed class MercStatusRangeScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S68";

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Inventory/MercStartingGear.xml");
        var before = XDocument.Load(file);

        WorkspaceUi.OpenMercGearWorkspace(context);
        var search = window.WaitById("MercSearchBox");
        search.SetText("Kelly");
        var merc = Ui.WaitFor(
            () => WorkspaceUi.FindListItem(window.WaitById("MercList"), "237: Kelly"),
            "Kelly в списке наёмников");
        merc.SelectItem();
        WorkspaceUi.WaitForStatus(
            context,
            "MercDetailHeader",
            text => text == "237: Kelly",
            "выбран Kelly",
            TimeSpan.FromSeconds(20));
        WorkspaceUi.SelectTab(window, "MercKitTabs", "Premium");

        var smallStatus = window.WaitById("mSmall1Status");
        context.Recorder.Check(
            "mSmall1Status показывает 600",
            smallStatus.ReadValue() == "600",
            smallStatus.ReadValue());

        window.WaitById("MercValidateButton").InvokeButton();
        Ui.WaitUntil(
            () => MercIssues(window).Any(text => text.Contains("validation.out_of_range", StringComparison.Ordinal)),
            "валидация сообщает out_of_range для 600",
            TimeSpan.FromSeconds(90));
        var issue600 = MercIssues(window)
            .First(text => text.Contains("validation.out_of_range", StringComparison.Ordinal));
        context.Recorder.Check(
            "Предупреждение называет поле и значение 600",
            issue600.Contains("mSmall1Status", StringComparison.Ordinal)
                && issue600.Contains("600", StringComparison.Ordinal),
            issue600);
        context.Recorder.Check(
            "Предупреждение называет максимальное значение 100",
            issue600.Contains("100", StringComparison.Ordinal),
            issue600);

        window.WaitById("MercSearchBox").SetText(string.Empty);
        var otherMerc = Ui.WaitFor(
            () => window.WaitById("MercList")
                .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
                .FirstOrDefault(item => !Ui.ItemText(item).Contains("237: Kelly", StringComparison.Ordinal)),
            "другой наёмник в списке");
        otherMerc.SelectItem();
        Ui.WaitUntil(
            () => window.WaitById("MercDetailHeader").ReadName() != "237: Kelly",
            "выбран другой наёмник",
            TimeSpan.FromSeconds(10));
        var issueItem = window.WaitById("MercValidationList")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
            .FirstOrDefault(item => Ui.ItemText(item).Contains("mSmall1Status", StringComparison.Ordinal));
        context.Recorder.Check("Замечание Kelly доступно в списке", issueItem is not null);
        issueItem!.SelectItem();
        WorkspaceUi.WaitForStatus(
            context,
            "MercDetailHeader",
            text => text == "237: Kelly",
            "выбор замечания вернул Kelly",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check(
            "Выбор замечания перешёл к записи 237",
            window.WaitById("MercDetailHeader").ReadName() == "237: Kelly",
            window.WaitById("MercDetailHeader").ReadName());

        WorkspaceUi.SelectTab(window, "MercKitTabs", "Premium");
        Ui.WaitUntil(
            () => window.WaitById("mSmall1Status").ReadValue() == "600",
            "комплект Premium восстановлен после перехода",
            TimeSpan.FromSeconds(10));

        var helmetStatus = window.WaitById("mHelmetStatus");
        helmetStatus.Focus();
        Thread.Sleep(100);
        helmetStatus.SetText("120");
        window.WaitById("MercSearchBox").SetFocus();
        Thread.Sleep(300);
        Ui.WaitUntil(
            () => window.WaitById("MercSaveButton").IsEnabled(),
            "смена статуса сделала вкладку грязной");
        context.Recorder.Check(
            "mHelmetStatus показывает 120 без clamp",
            window.WaitById("mHelmetStatus").ReadValue() == "120",
            window.WaitById("mHelmetStatus").ReadValue());

        window.WaitById("MercValidateButton").InvokeButton();
        Ui.WaitUntil(
            () => MercIssues(window).Any(text => text.Contains("validation.out_of_range", StringComparison.Ordinal)
                && text.Contains("120", StringComparison.Ordinal)),
            "валидация сообщает out_of_range для 120",
            TimeSpan.FromSeconds(90));
        context.Recorder.Check(
            "mSmall1Status по-прежнему 600",
            window.WaitById("mSmall1Status").ReadValue() == "600",
            window.WaitById("mSmall1Status").ReadValue());

        WorkspaceUi.SaveMercGear(context);
        var after = XDocument.Load(file);
        var kitBefore = KellyPremiumKit(before);
        var kitAfter = KellyPremiumKit(after);
        context.Recorder.Check(
            "Сохранены 600 и 120",
            KitValue(kitAfter, "mSmall1Status") == "600"
                && KitValue(kitAfter, "mHelmetStatus") == "120",
            $"mSmall1Status={KitValue(kitAfter, "mSmall1Status")}, mHelmetStatus={KitValue(kitAfter, "mHelmetStatus")}");
        var changed = kitBefore.Elements()
            .Where(element => !string.Equals(
                element.Value,
                KitValue(kitAfter, element.Name.LocalName),
                StringComparison.Ordinal))
            .Select(static element => element.Name.LocalName)
            .ToList();
        context.Recorder.Check(
            "Сохранение изменило только mHelmetStatus",
            changed.SequenceEqual(new[] { "mHelmetStatus" }),
            string.Join(", ", changed));
        context.Recorder.Check(
            "Остальные комплекты не изменены",
            before.Descendants("GEARKIT")
                .Count() == after.Descendants("GEARKIT").Count()
                && OtherKits(before).SequenceEqual(OtherKits(after)),
            null);

        context.ExpectChange(DatasetKind.V4, "TableData/Inventory/MercStartingGear.xml");
    }

    private static List<string> MercIssues(AutomationElement window) =>
        WorkspaceUi.ListItemTexts(window.WaitById("MercValidationList"))
            .Where(static text => text.Length > 0)
            .ToList();

    private static XElement KellyPremiumKit(XDocument document) =>
        document.Descendants("MERCGEAR")
            .First(row => row.Element("mIndex")?.Value.Trim() == "237")
            .Elements("GEARKIT")
            .ElementAt(4);

    private static string KitValue(XElement kit, string field) =>
        kit.Element(field)?.Value.Trim() ?? string.Empty;

    private static List<string> OtherKits(XDocument document) =>
        document.Descendants("GEARKIT")
            .Where(kit => kit.Parent?.Element("mIndex")?.Value.Trim() != "237"
                || !ReferenceEquals(kit, kit.Parent.Elements("GEARKIT").ElementAt(4)))
            .Select(static kit => kit.ToString(SaveOptions.DisableFormatting))
            .ToList();
}

/// <summary>
/// S69: the 64-bit bit editor shows all bits, labels the bits that have lookup names, fits the default
/// window without scrolling, cancels without changing the value, and writes bit 33 as 8589934592.
/// <c>Lookup.ItemFlag</c> has no consumer column, so the high-bit precision is proven on
/// <c>usActionItemFlag</c>; the 2^32/2^33 names are checked in the lookup document.
/// </summary>
internal sealed class FlagBitEditorScenario : SmokeScenario
{
    private const string HighBitValue = "8589934592";

    /// <inheritdoc />
    public override string Id => "S69";

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Items/Items.xml");
        var beforeText = File.ReadAllText(file);
        var before = XDocument.Parse(beforeText);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1", "Glock 17");

        var attachField = ShowField(context, "AttachmentClass");
        var attachBefore = attachField.ReadValue();
        var attachDialog = OpenBitEditor(context, "AttachmentClass", attachField);
        var attachBits = BitCheckBoxes(attachDialog);
        context.Recorder.Check(
            "Битовый редактор AttachmentClass показывает 64 бита",
            BitNumbers(attachBits).SequenceEqual(Enumerable.Range(0, 64)),
            $"битов: {attachBits.Count}");
        context.Recorder.Check(
            "Подписи битов из Lookup.AttachmentClass присутствуют",
            attachBits.Any(box => box.ReadName() == "Bipod (0)")
                && attachBits.Any(box => box.ReadName() == "Rifle Grenade (25)"),
            string.Join(" | ", attachBits.Take(5).Select(static box => box.ReadName())));
        context.Recorder.Check(
            "При размере окна по умолчанию прокрутки нет",
            !HasVerticalScroll(attachDialog),
            null);

        attachBits.First(box => box.ReadName() == "Bipod (0)").SetChecked(true);
        CloseBitEditor(context, attachDialog, ok: false);
        attachField = ShowField(context, "AttachmentClass");
        context.Recorder.Check(
            "«Отмена» не меняет AttachmentClass",
            attachField.ReadValue() == attachBefore,
            $"«{attachField.ReadValue()}» вместо «{attachBefore}»");
        context.Recorder.Check(
            "После «Отмены» вкладка чистая",
            !window.WaitById("ItemSaveButton").IsEnabled(),
            null);

        var actionField = ShowField(context, "usActionItemFlag");
        var actionDialog = OpenBitEditor(context, "usActionItemFlag", actionField);
        var actionBits = BitCheckBoxes(actionDialog);
        context.Recorder.Check(
            "Битовый редактор usActionItemFlag показывает 64 бита",
            BitNumbers(actionBits).SequenceEqual(Enumerable.Range(0, 64)),
            $"битов: {actionBits.Count}");
        actionBits.First(box => box.ReadName() == "33").SetChecked(true);
        CloseBitEditor(context, actionDialog, ok: true);
        Ui.WaitUntil(
            () => window.WaitById("ItemSaveButton").IsEnabled(),
            "установка бита 33 сделала вкладку грязной");
        context.Recorder.Check(
            "Бит 33 даёт 8589934592",
            ShowField(context, "usActionItemFlag").ReadValue() == HighBitValue,
            ShowField(context, "usActionItemFlag").ReadValue());

        window.WaitById("ItemUndoButton").InvokeButton();
        Ui.WaitUntil(
            () => !window.WaitById("ItemSaveButton").IsEnabled(),
            "Undo убрал установленный бит");
        window.WaitById("ItemRedoButton").InvokeButton();
        Ui.WaitUntil(
            () => window.WaitById("ItemSaveButton").IsEnabled(),
            "Redo вернул установленный бит");
        context.Recorder.Check(
            "Redo снова даёт 8589934592",
            ShowField(context, "usActionItemFlag").ReadValue() == HighBitValue,
            null);

        window.WaitById("ItemSaveButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Get("EditorSaved"),
            TimeSpan.FromSeconds(60));

        var after = XDocument.Load(file);
        var beforeItems = before.Root!.Elements().ToList();
        var afterItems = after.Root!.Elements().ToList();
        context.Recorder.Check(
            "Число предметов не изменилось",
            beforeItems.Count == afterItems.Count,
            $"{beforeItems.Count} → {afterItems.Count}");
        var itemBefore = beforeItems[1];
        var itemAfter = afterItems[1];
        var added = itemAfter.Elements()
            .Select(static element => element.Name.LocalName)
            .Except(itemBefore.Elements().Select(static element => element.Name.LocalName), StringComparer.Ordinal)
            .ToList();
        context.Recorder.Check(
            "У item 1 добавлен только usActionItemFlag",
            added.SequenceEqual(new[] { "usActionItemFlag" }),
            string.Join(", ", added));
        context.Recorder.Check(
            "XML хранит 8589934592 без потери точности",
            itemAfter.Element("usActionItemFlag")?.Value.Trim() == HighBitValue,
            itemAfter.Element("usActionItemFlag")?.Value);
        context.Recorder.Check(
            "Остальные предметы не изменились",
            beforeItems.Where((_, index) => index != 1)
                .Select(static element => element.ToString(SaveOptions.DisableFormatting))
                .SequenceEqual(afterItems.Where((_, index) => index != 1)
                    .Select(static element => element.ToString(SaveOptions.DisableFormatting))),
            null);

        var lookup = Path.Combine(
            Path.GetDirectoryName(file)!,
            "..",
            "Lookup",
            "ItemFlag.xml");
        var flags = XDocument.Load(lookup).Descendants("ItemFlag")
            .ToDictionary(
                element => element.Element("id")!.Value.Trim(),
                element => element.Element("name")!.Value.Trim(),
                StringComparer.Ordinal);
        context.Recorder.Check(
            "Lookup.ItemFlag называет биты 32 и 33",
            flags.TryGetValue("4294967296", out var name32)
                && name32 == "Empty Blood Bag"
                && flags.TryGetValue(HighBitValue, out var name33)
                && name33 == "Medical Splint",
            $"{flags.GetValueOrDefault("4294967296")}, {flags.GetValueOrDefault(HighBitValue)}");

        context.ExpectChange(DatasetKind.V4, "TableData/Items/Items.xml");
    }

    private static AutomationElement ShowField(ScenarioContext context, string field)
    {
        var window = context.Window;
        var filter = window.WaitById("ItemFieldFilterBox");
        filter.Focus();
        filter.SetText(field);
        Thread.Sleep(400);
        WorkspaceUi.ExpandAll(window.WaitById("ItemTabs"));
        return window.WaitById(field, TimeSpan.FromSeconds(20));
    }

    private static List<AutomationElement> BitCheckBoxes(AutomationElement dialog) =>
        dialog.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.CheckBox))
            .ToList();

    private static IEnumerable<int> BitNumbers(IEnumerable<AutomationElement> boxes) =>
        boxes.Select(static box => BitNumber(box.ReadName()))
            .OrderBy(static number => number);

    private static int BitNumber(string text)
    {
        var open = text.LastIndexOf('(');
        if (open >= 0 && text.EndsWith(')')
            && int.TryParse(text[(open + 1)..^1], NumberStyles.None, CultureInfo.InvariantCulture, out var numbered))
        {
            return numbered;
        }

        return int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var plain) ? plain : -1;
    }

    private static bool HasVerticalScroll(AutomationElement dialog) =>
        dialog.FindAll(Condition.TrueCondition).Any(element =>
        {
            try
            {
                return element.TryGetCurrentPattern(ScrollPattern.Pattern, out var raw)
                    && raw is ScrollPattern scroll
                    && scroll.Current.VerticallyScrollable;
            }
            catch (ElementNotAvailableException)
            {
                return false;
            }
        });

    private static AutomationElement OpenBitEditor(
        ScenarioContext context,
        string field,
        AutomationElement fieldEditor)
    {
        var window = context.Window;
        var caption = context.Strings.Get("DetailBitsText");
        var fieldTop = fieldEditor.Current.BoundingRectangle.Top;
        var button = window.FindAll(Condition.TrueCondition)
            .Where(element =>
            {
                try
                {
                    return element.Current.ControlType == ControlType.Button
                        && element.ReadName() == caption
                        && !element.Current.IsOffscreen
                        && Math.Abs(element.Current.BoundingRectangle.Top - fieldTop) < 40;
                }
                catch (ElementNotAvailableException)
                {
                    return false;
                }
            })
            .OrderBy(static element => element.Current.BoundingRectangle.Left)
            .FirstOrDefault()
            ?? throw new UiTimeoutException($"Кнопка «{caption}» для {field} не найдена.");

        // InvokePattern.Invoke does not return until the modal editor closes, so it runs in the
        // background while the scenario drives the dialog.
        _ = Task.Run(() =>
        {
            try
            {
                button.InvokeButton();
            }
            catch (Exception exception) when (
                exception is UiTimeoutException or ElementNotAvailableException or COMException)
            {
                // The dialog closed and the button disappeared before the invoke returned.
            }
        });

        var title = context.Strings.Format("BitEditorTitleFormat", field);
        return Ui.WaitFor(
            () => FindWindow(context, title),
            $"битовый редактор «{title}»",
            TimeSpan.FromSeconds(20));
    }

    private static void CloseBitEditor(ScenarioContext context, AutomationElement dialog, bool ok)
    {
        var caption = context.Strings.Get(ok ? "BitEditorOk" : "BitEditorCancel");
        var title = dialog.ReadName();
        var button = dialog.FindAll(new PropertyCondition(
                AutomationElement.ControlTypeProperty,
                ControlType.Button))
            .FirstOrDefault(candidate => candidate.ReadName() == caption)
            ?? throw new UiTimeoutException($"Кнопка «{caption}» в битовом редакторе не найдена.");
        button.InvokeButton();
        Ui.WaitUntil(
            () => FindWindow(context, title) is null,
            "битовый редактор закрылся",
            TimeSpan.FromSeconds(20));
    }

    private static AutomationElement? FindWindow(ScenarioContext context, string title)
    {
        try
        {
            return AutomationElement.RootElement
                .FindAll(
                    TreeScope.Descendants,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, context.App.ProcessId),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window)))
                .Cast<AutomationElement>()
                .FirstOrDefault(candidate => candidate.ReadName() == title);
        }
        catch (ElementNotAvailableException)
        {
            return null;
        }
    }
}

/// <summary>
/// S70: after the limit edits, undo and redo restore the class, saving writes it, reopening the table
/// shows the same warnings without touching the file, a clean tab cannot be saved, and closing a clean
/// tab does not ask for confirmation.
/// </summary>
internal sealed class ValidationUndoScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S70";

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Items/LoadBearingEquipment.xml");

        OpenLbe(context);
        var grid = window.WaitById("RowsGrid");
        var search = window.WaitById("SearchBox");
        search.SetText("22");
        var row = WaitForRow(grid, "22");
        var combo = DetailsUi.ShowField(context, grid, row, "lbeClass");
        combo.SelectComboItemStartingWith("Thigh Pack");
        Ui.WaitUntil(() => window.WaitById("SaveButton").IsEnabled(), "класс изменён");

        window.WaitById("ValidateButton").InvokeButton();
        Ui.WaitUntil(
            () => ValidationTexts(window).Any(text => text.Contains("validation.class_value_limit", StringComparison.Ordinal)),
            "валидация показала class_value_limit",
            TimeSpan.FromSeconds(90));

        window.WaitById("UndoButton").InvokeButton();
        Ui.WaitUntil(
            () => DetailsUi.ShowField(context, grid, row, "lbeClass")
                .ReadComboSelection()
                .Contains("Combat Pack", StringComparison.Ordinal),
            "Undo вернул Combat Pack");
        context.Recorder.Check(
            "Undo вернул класс и чистое состояние",
            !window.WaitById("SaveButton").IsEnabled(),
            null);

        window.WaitById("RedoButton").InvokeButton();
        Ui.WaitUntil(
            () => DetailsUi.ShowField(context, grid, row, "lbeClass")
                .ReadComboSelection()
                .Contains("Thigh Pack", StringComparison.Ordinal),
            "Redo вернул Thigh Pack");
        context.Recorder.Check(
            "Redo снова сделал вкладку грязной",
            window.WaitById("SaveButton").IsEnabled(),
            null);

        window.WaitById("SaveButton").InvokeButton();
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorSaved"),
            "таблица сохранена",
            TimeSpan.FromSeconds(60));
        var hashAfterSave = SaveDirtyTabScenario.HashFile(file);
        context.Recorder.Check(
            "После сохранения вкладка чистая",
            !window.WaitById("SaveButton").IsEnabled(),
            null);

        ScenarioUi.CloseTab(window, "Items.LoadBearingEquipment");
        ScenarioUi.WaitForTabClosed(window, "Items.LoadBearingEquipment");
        context.Recorder.Check(
            "Закрытие чистой вкладки не спрашивает сохранение",
            ScenarioUi.TopLevelWindowCount(context.App.ProcessId) == 1,
            $"окон: {ScenarioUi.TopLevelWindowCount(context.App.ProcessId)}");

        OpenLbe(context);
        window.WaitById("ValidateButton").InvokeButton();
        Ui.WaitUntil(
            () => ValidationTexts(window).Any(text => text.Contains("validation.class_value_limit", StringComparison.Ordinal)),
            "повторная проверка показала class_value_limit",
            TimeSpan.FromSeconds(90));
        context.Recorder.Check(
            "Открытие и проверка не изменили файл",
            SaveDirtyTabScenario.HashFile(file) == hashAfterSave,
            null);
        context.Recorder.Check(
            "На чистой вкладке сохранение недоступно",
            !window.WaitById("SaveButton").IsEnabled(),
            null);

        ScenarioUi.CloseTab(window, "Items.LoadBearingEquipment");
        ScenarioUi.WaitForTabClosed(window, "Items.LoadBearingEquipment");
        context.Recorder.Check(
            "Повторное закрытие без правок не спрашивает сохранение",
            ScenarioUi.TopLevelWindowCount(context.App.ProcessId) == 1,
            $"окон: {ScenarioUi.TopLevelWindowCount(context.App.ProcessId)}");

        context.ExpectChange(DatasetKind.V4, "TableData/Items/LoadBearingEquipment.xml");
    }

    private static AutomationElement WaitForRow(AutomationElement grid, string key) =>
        Ui.WaitFor(
            () => grid
                .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
                .FirstOrDefault(row => row.ReadRowCells().Contains(key, StringComparer.Ordinal)),
            $"строка с ключом {key}");

    private static void OpenLbe(ScenarioContext context)
    {
        ScenarioUi.OpenTable(context, "Items.LoadBearingEquipment");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.LoadBearingEquipment открыт",
            TimeSpan.FromSeconds(90));
    }

    private static List<string> ValidationTexts(AutomationElement window) =>
        WorkspaceUi.ListItemTexts(window.WaitById("ValidationList"))
            .Where(static text => text.Length > 0)
            .ToList();
}
