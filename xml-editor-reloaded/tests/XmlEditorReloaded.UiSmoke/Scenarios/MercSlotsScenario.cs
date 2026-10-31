using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S28: item pickers, status, quantity, drop flag and prices of a gear kit accept edits, the empty item
/// stays selectable in filtered slots, a manual zero is kept, undo and redo work, validation reports the
/// out-of-range price without clamping it, and the save writes only the edited kit fields.
/// </summary>
internal sealed class MercSlotsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S28";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

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
        WorkspaceUi.SetTextAndCommit(search, "Barry");
        Ui.WaitUntil(
            () => window.WaitById("MercDetailHeader").ReadName() == "0: Barry",
            "выбран Barry");

        SetPicker(context, "mHelmet", "Nothing");
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Format("EditorCellUpdated", "mHelmet", "0"),
            "шлем очищен");
        context.Recorder.Check(
            "Первый пункт «Nothing» очищает отфильтрованный слот шлема",
            window.WaitById("mHelmet").ReadValue() == "Nothing",
            window.WaitById("mHelmet").ReadValue());

        SetPicker(context, "mWeapon", "Colt M4 Commando");
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Format("EditorCellUpdated", "mWeapon", "0"),
            "оружие изменено");
        context.Recorder.Check(
            "Picker оружия записывает выбранный предмет",
            window.WaitById("mWeapon").ReadValue() == "Colt M4 Commando",
            window.WaitById("mWeapon").ReadValue());

        SetPicker(context, "lVest", "Nothing");
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Format("EditorCellUpdated", "lVest", "0"),
            "разгрузочный жилет очищен");
        context.Recorder.Check(
            "LBE-слот принимает Nothing",
            window.WaitById("lVest").ReadValue() == "Nothing",
            window.WaitById("lVest").ReadValue());

        var status = window.WaitById("mHelmetStatus");
        WorkspaceUi.SetTextAndCommit(status, "50");
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Format("EditorCellUpdated", "mHelmetStatus", "0"),
            "статус шлема изменён");
        context.Recorder.Check("Статус принимает новое значение", status.ReadValue() == "50", status.ReadValue());

        window.WaitById("MercUndoButton").InvokeButton();
        Ui.WaitUntil(
            () => window.WaitById("mHelmetStatus").ReadValue() == "97",
            "Undo вернул статус 97",
            TimeSpan.FromSeconds(20));
        window.WaitById("MercRedoButton").InvokeButton();
        Ui.WaitUntil(
            () => window.WaitById("mHelmetStatus").ReadValue() == "50",
            "Redo вернул статус 50",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Undo и Redo работают для слота", true);

        var quantity = window.WaitById("mBig0Quantity");
        WorkspaceUi.SetTextAndCommit(quantity, "5");
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Format("EditorCellUpdated", "mBig0Quantity", "0"),
            "количество изменено");
        context.Recorder.Check("Количество принимает новое значение", window.WaitById("mBig0Quantity").ReadValue() == "5", null);

        var drop = window.WaitById("mBig0Drop");
        drop.SetChecked(true);
        Thread.Sleep(200);
        Ui.WaitUntil(
            () => window.WaitById("mBig0Drop").IsChecked() == true,
            "drop включён",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check("Флаг drop переключается", true);

        var manualZero = window.WaitById("mBig1Quantity");
        WorkspaceUi.SetTextAndCommit(manualZero, "0");
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Format("EditorCellUpdated", "mBig1Quantity", "0"),
            "ручной ноль принят");
        context.Recorder.Check(
            "Ручной ввод 0 сохраняется как 0",
            window.WaitById("mBig1Quantity").ReadValue() == "0",
            window.WaitById("mBig1Quantity").ReadValue());

        var price = window.WaitById("MercPriceModBox");
        WorkspaceUi.SetTextAndCommit(price, "40000");
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Format("EditorCellUpdated", "mPriceMod", "0"),
            "модификатор цены изменён");
        context.Recorder.Check(
            "Модификатор цены принимает значение вне диапазона без clamp",
            window.WaitById("MercPriceModBox").ReadValue() == "40000",
            window.WaitById("MercPriceModBox").ReadValue());
        var absolute = window.WaitById("MercAbsolutePriceBox");
        WorkspaceUi.SetTextAndCommit(absolute, "1500");
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Format("EditorCellUpdated", "mAbsolutePrice", "0"),
            "фиксированная цена изменена");
        context.Recorder.Check(
            "Фиксированная цена принимает значение",
            window.WaitById("MercAbsolutePriceBox").ReadValue() == "1500",
            window.WaitById("MercAbsolutePriceBox").ReadValue());

        window.WaitById("MercValidateButton").InvokeButton();
        Ui.WaitUntil(
            () => ValidationTexts(context).Count > 0,
            "валидация вернула предупреждения",
            TimeSpan.FromSeconds(60));
        var issueTexts = ValidationTexts(context);
        var outOfRange = issueTexts.FirstOrDefault(
            text => text.Contains("out_of_range", StringComparison.Ordinal));
        context.Recorder.Check(
            "Валидация сообщает out_of_range для цены",
            outOfRange is not null,
            outOfRange ?? string.Join(" | ", issueTexts.Take(5)));

        WorkspaceUi.SaveMercGear(context);
        var after = XDocument.Load(file);
        var barryBefore = BarryKits(before);
        var barryAfter = BarryKits(after);
        context.Recorder.Check(
            "Комплект 1 сохранён с новыми значениями",
            KitValue(barryAfter[0], "mHelmet") == "0"
                && KitValue(barryAfter[0], "mHelmetStatus") == "50"
                && KitValue(barryAfter[0], "mWeapon") == "12"
                && KitValue(barryAfter[0], "mBig0Quantity") == "5"
                && KitValue(barryAfter[0], "mBig0Drop") == "1"
                && KitValue(barryAfter[0], "mBig1Quantity") == "0"
                && KitValue(barryAfter[0], "lVest") == "0"
                && KitValue(barryAfter[0], "mPriceMod") == "40000"
                && KitValue(barryAfter[0], "mAbsolutePrice") == "1500",
            string.Join(
                ", ",
                new[] { "mHelmet", "mHelmetStatus", "mWeapon", "mBig0Quantity", "mBig0Drop", "mBig1Quantity", "lVest", "mPriceMod", "mAbsolutePrice" }
                    .Select(field => $"{field}={KitValue(barryAfter[0], field)}")));
        context.Recorder.Check(
            "Остальные комплекты Barry не изменились",
            barryBefore.Skip(1).Select(static kit => kit.ToString(SaveOptions.DisableFormatting))
                .SequenceEqual(barryAfter.Skip(1).Select(static kit => kit.ToString(SaveOptions.DisableFormatting))),
            null);
        var otherMercsBefore = before.Descendants("MERCGEAR")
            .Where(row => row.Element("mIndex")?.Value.Trim() != "0")
            .Select(static row => row.ToString(SaveOptions.DisableFormatting))
            .ToList();
        var otherMercsAfter = after.Descendants("MERCGEAR")
            .Where(row => row.Element("mIndex")?.Value.Trim() != "0")
            .Select(static row => row.ToString(SaveOptions.DisableFormatting))
            .ToList();
        context.Recorder.Check(
            "Остальные наёмники не изменились",
            otherMercsBefore.SequenceEqual(otherMercsAfter),
            null);

        context.ExpectChange(DatasetKind.V4, "TableData/Inventory/MercStartingGear.xml");
    }

    private static void SetPicker(ScenarioContext context, string automationId, string text)
    {
        var picker = context.Window.WaitById(automationId);
        picker.Focus();
        Thread.Sleep(120);
        NativeInput.PressIn(picker, NativeInput.VirtualKey.A, control: true);
        NativeInput.TypeTextIn(picker, text);
        Thread.Sleep(400);
        NativeInput.PressIn(picker, NativeInput.VirtualKey.Return);
        Thread.Sleep(300);
    }

    private static List<XElement> BarryKits(XDocument document) =>
        document.Descendants("MERCGEAR")
            .First(row => row.Element("mIndex")?.Value.Trim() == "0")
            .Elements("GEARKIT")
            .ToList();

    private static string KitValue(XElement kit, string field) =>
        kit.Element(field)?.Value.Trim() ?? string.Empty;

    private static List<string> ValidationTexts(ScenarioContext context) =>
        WorkspaceUi.ListItemTexts(context.Window.WaitById("MercValidationList"))
            .Where(static text => text.Length > 0)
            .ToList();
}
