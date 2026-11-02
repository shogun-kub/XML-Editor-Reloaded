using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Shared operations for the shopkeeper CRUD scenarios: opening the tab, reading the dealer rows,
/// answering the removal confirmation, and choosing a dealer from the add bar.
/// </summary>
internal abstract class ShopkeeperCrudScenarioBase : SmokeScenario
{
    /// <summary>Dealer file that stores the rows of AdditionalDealer_43.</summary>
    protected const string DealerFile = "NPCInventory/AdditionalDealer_43_Inventory.xml";

    /// <summary>Dealer file of Tony, which the scenarios must leave untouched.</summary>
    protected const string TonyFile = "NPCInventory/TonyInventory.xml";

    /// <summary>Dealer file used by the add-row scenario.</summary>
    protected const string AlbertoFile = "NPCInventory/AlbertoInventory.xml";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V5];

    /// <inheritdoc />
    public override DatasetKind ActiveDataset => DatasetKind.V5;

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V5;

    /// <summary>
    /// Selects the shopkeeper tab and waits until its row counter is rendered.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    protected static void OpenShopkeeperTab(ScenarioContext context)
    {
        WorkspaceUi.SelectItemTab(context.Window, context.Strings.Get("ItemTabShopkeepers"));
        var prefix = ScenarioUi.FormatPrefix(context.Strings.Get("ShopkeeperCountFormat"));
        WorkspaceUi.WaitForStatus(
            context,
            "ShopkeeperCountText",
            text => text.StartsWith(prefix, StringComparison.Ordinal),
            "счётчик строк торговцев",
            TimeSpan.FromSeconds(20));
    }

    /// <summary>
    /// Reads the number of dealer rows currently shown.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>Row count.</returns>
    /// <exception cref="SmokeCheckException">Thrown when the counter cannot be parsed.</exception>
    protected static int ReadShopkeeperCount(ScenarioContext context)
    {
        var text = context.Window.WaitById("ShopkeeperCountText").ReadName();
        var match = Regex.Match(text, @"(\d+)\D*$");
        if (!match.Success)
        {
            throw new SmokeCheckException("Счётчик строк торговцев читается", text);
        }

        return int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Waits until the dealer-row counter shows the expected number.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="expected">Expected row count.</param>
    protected static void WaitShopkeeperCount(ScenarioContext context, int expected)
    {
        var expectedText = context.Strings.Format("ShopkeeperCountFormat", expected);
        WorkspaceUi.WaitForStatus(
            context,
            "ShopkeeperCountText",
            text => text == expectedText,
            $"строк торговцев: {expected}",
            TimeSpan.FromSeconds(20));
    }

    /// <summary>
    /// Determines whether a dealer row is visible in the shopkeeper tab.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dealerName">Dealer name shown on the row.</param>
    /// <returns><see langword="true"/> when the row exists.</returns>
    protected static bool HasDealerRow(ScenarioContext context, string dealerName) =>
        context.Window
            .FindAllById("ShopkeeperQuantityBox")
            .Any(box => string.Equals(box.ReadName(), dealerName, StringComparison.Ordinal));

    /// <summary>
    /// Determines whether the add bar offers the dealer. The combo box virtualizes its popup, so the
    /// popup is scrolled from the top until the name is realized.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dealerName">Dealer name shown in the combo box.</param>
    /// <returns><see langword="true"/> when the dealer is offered.</returns>
    protected static bool HasDealerOption(ScenarioContext context, string dealerName)
    {
        var combo = context.Window.WaitById("ShopkeeperDealerCombo");
        combo.ExpandCombo();
        try
        {
            return FindComboItem(combo, dealerName) is not null;
        }
        finally
        {
            ScenarioUi.Collapse(combo);
        }
    }

    /// <summary>
    /// Selects a dealer in the add bar, scrolling the virtualized popup when necessary.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dealerName">Dealer name shown in the combo box.</param>
    /// <exception cref="SmokeCheckException">Thrown when the dealer is not offered.</exception>
    protected static void SelectDealerOption(ScenarioContext context, string dealerName)
    {
        var combo = context.Window.WaitById("ShopkeeperDealerCombo");
        combo.ExpandCombo();
        var item = FindComboItem(combo, dealerName);
        if (item is null)
        {
            ScenarioUi.Collapse(combo);
            throw new SmokeCheckException(
                $"Торговец {dealerName} доступен для добавления",
                "не найден в списке торговцев");
        }

        item.SelectItem();
        Thread.Sleep(350);
        if (!IsDealerSelected(combo, dealerName))
        {
            var rectangle = item.Current.BoundingRectangle;
            if (rectangle.Width > 0 && rectangle.Height > 0)
            {
                item.Click();
                Thread.Sleep(350);
            }
        }

        if (!IsDealerSelected(combo, dealerName))
        {
            ScenarioUi.Collapse(combo);
            throw new SmokeCheckException(
                $"Торговец {dealerName} выбран в списке",
                $"combo показывает «{combo.ReadComboSelection()}»");
        }
    }

    /// <summary>
    /// Replaces the quantity of an existing dealer row and commits it by moving the keyboard focus to the
    /// item search box. The shopkeeper text box writes its binding on lost focus, so this avoids the
    /// synthetic Tab that is unreliable when the window does not own the foreground.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dealerName">Dealer whose quantity is edited.</param>
    /// <param name="value">New quantity text.</param>
    protected static void SetDealerQuantity(ScenarioContext context, string dealerName, string value) =>
        WorkspaceUi.SetShopkeeperQuantity(context, dealerName, value);

    /// <summary>
    /// Types a quantity into the add bar without committing it, so the Add command reads the new text.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="value">Quantity text.</param>
    protected static void SetNewQuantity(ScenarioContext context, string value)
    {
        context.Window.WaitById("ShopkeeperNewQuantityBox").SetText(value);
        Thread.Sleep(120);
    }

    /// <summary>
    /// Invokes Add and waits for the "invalid value" status, leaving the row count unchanged.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="value">Rejected quantity text.</param>
    protected static void AddInvalidQuantity(ScenarioContext context, string value)
    {
        var add = context.Window.WaitById("ShopkeeperAddButton");
        Ui.WaitUntil(() => add.IsEnabled(), "кнопка «Добавить» доступна", TimeSpan.FromSeconds(10));
        add.InvokeButton();
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format(
                "EditorInvalidValue",
                value,
                context.Strings.Get("ShopkeeperQuantityColumn")),
            TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Adds the selected dealer and waits for the row-added status.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dealerName">Dealer name expected in the status.</param>
    protected static void AddDealer(ScenarioContext context, string dealerName)
    {
        var add = context.Window.WaitById("ShopkeeperAddButton");
        Ui.WaitUntil(() => add.IsEnabled(), "кнопка «Добавить» доступна", TimeSpan.FromSeconds(10));
        add.InvokeButton();
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ShopkeeperRowAddedFormat", dealerName),
            TimeSpan.FromSeconds(20));
    }

    /// <summary>
    /// Removes one dealer row through its row button and answers the native confirmation with Yes.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="itemKey">Item identifier expected in the confirmation text.</param>
    /// <param name="dealerName">Dealer whose row is removed.</param>
    /// <param name="quantity">Quantity expected in the confirmation text.</param>
    protected static void RemoveDealer(ScenarioContext context, string itemKey, string dealerName, string quantity)
    {
        var remove = FindRemoveButton(context, dealerName);
        _ = Task.Run(() =>
        {
            try
            {
                remove.InvokeButton();
            }
            catch (Exception exception) when (
                exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The confirmation closed the dialog and the button disappeared before the invoke returned.
            }
        });

        var messageBox = ScenarioUi.WaitForMessageBox(context.App, TimeSpan.FromSeconds(20));
        var message = ScenarioUi.ReadMessageBoxText(messageBox);
        var expected = context.Strings.Format(
            "ShopkeeperConfirmRemoveFormat",
            dealerName,
            itemKey,
            quantity);
        context.Recorder.Check("Подтверждение удаления называет строку", message.Contains(expected, StringComparison.Ordinal), message);
        ScenarioUi.AnswerMessageBox(messageBox, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ShopkeeperRowRemovedFormat", dealerName),
            TimeSpan.FromSeconds(20));
    }

    /// <summary>
    /// Undoes the last workspace edit and waits for the standard status.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    protected static void Undo(ScenarioContext context)
    {
        context.Window.WaitById("ItemUndoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(context, context.Strings.Get("EditorUndone"), TimeSpan.FromSeconds(20));
    }

    /// <summary>
    /// Redoes the last undone workspace edit and waits for the standard status.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    protected static void Redo(ScenarioContext context)
    {
        context.Window.WaitById("ItemRedoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(context, context.Strings.Get("EditorRedone"), TimeSpan.FromSeconds(20));
    }

    /// <summary>
    /// Asserts that the workspace reports no unsaved changes.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    protected static void CheckClean(ScenarioContext context)
    {
        var save = context.Window.WaitById("ItemSaveButton");
        context.Recorder.Check("Вкладка осталась без правок", !save.IsEnabled(), null);
    }

    /// <summary>
    /// Loads a table file and returns the number of records.
    /// </summary>
    /// <param name="file">Absolute XML path.</param>
    /// <returns>Record count.</returns>
    protected static int RecordCount(string file) => XDocument.Load(file).Descendants("INVENTORY").Count();

    /// <summary>
    /// Returns the first stored quantity of the given item, or an empty string when the item has no row.
    /// </summary>
    /// <param name="file">Absolute dealer XML path.</param>
    /// <param name="itemIndex">Value of <c>sItemIndex</c>.</param>
    /// <returns>Stored quantity text.</returns>
    protected static string StoredQuantity(string file, string itemIndex) =>
        XDocument.Load(file)
            .Descendants("INVENTORY")
            .FirstOrDefault(row => row.Element("sItemIndex")?.Value.Trim() == itemIndex)
            ?.Element("ubOptimalNumber")?.Value.Trim() ?? string.Empty;

    /// <summary>
    /// Verifies that the first records of a file did not change at all, so a mutation only appended.
    /// </summary>
    /// <param name="before">Document before the edit.</param>
    /// <param name="after">Document after the edit.</param>
    /// <param name="count">Number of leading records to compare.</param>
    /// <returns><see langword="true"/> when the prefix is unchanged.</returns>
    protected static bool LeadingRecordsUnchanged(XDocument before, XDocument after, int count)
    {
        var expected = before.Descendants("INVENTORY").Take(count).ToList();
        var actual = after.Descendants("INVENTORY").Take(count).ToList();
        return expected.Count == count
            && actual.Count == count
            && expected.Zip(actual).All(pair => XNode.DeepEquals(pair.First, pair.Second));
    }

    /// <summary>
    /// Runs the item validation and asserts the duplicate-key warning for the supplied key.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="key">Duplicated record key, for example <c>26</c>.</param>
    protected static void CheckDuplicateKeyWarning(ScenarioContext context, string key)
    {
        var window = context.Window;
        window.WaitById("ItemValidateButton").InvokeButton();
        var list = window.WaitById("ItemValidationList");
        string? issue = null;
        for (var attempt = 0; attempt < 30 && issue is null; attempt++)
        {
            Thread.Sleep(500);
            issue = WorkspaceUi.ListItemTexts(list).FirstOrDefault(
                text => text.Contains("validation.duplicate_key", StringComparison.Ordinal)
                    && text.Contains($"[{key}]", StringComparison.Ordinal));
            if (issue is null && !Ui.ScrollDown(list))
            {
                break;
            }
        }

        context.Recorder.Check(
            $"Валидатор предупреждает о дубликате ключа {key}",
            issue is not null,
            issue ?? string.Join(" | ", WorkspaceUi.ListItemTexts(list).Take(3)));
    }

    private static AutomationElement FindRemoveButton(ScenarioContext context, string dealerName)
    {
        var entries = context.Window.WaitById("ShopkeeperEntries");
        var quantity = WorkspaceUi.ShopkeeperQuantity(context, dealerName);
        var band = quantity.Current.BoundingRectangle;
        var removeText = context.Strings.Get("ShopkeeperRemoveText");
        return entries
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))
            .FirstOrDefault(button =>
            {
                if (!string.Equals(button.ReadName(), removeText, StringComparison.Ordinal))
                {
                    return false;
                }

                var rectangle = button.Current.BoundingRectangle;
                var center = rectangle.Top + (rectangle.Height / 2);
                return center >= band.Top - 6 && center <= band.Bottom + 6;
            })
            ?? throw new UiTimeoutException($"Кнопка удаления строки {dealerName} не найдена.");
    }

    private static AutomationElement? FindComboItem(AutomationElement combo, string text) =>
        combo
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
            .FirstOrDefault(item => string.Equals(Ui.ItemText(item).Trim(), text, StringComparison.Ordinal));

    private static bool IsDealerSelected(AutomationElement combo, string dealerName)
    {
        if (!combo.TryGetCurrentPattern(SelectionPattern.Pattern, out var raw)
            || raw is not SelectionPattern selection)
        {
            return false;
        }

        try
        {
            return selection.Current.GetSelection().Any(item =>
                string.Equals(Ui.ItemText(item).Trim(), dealerName, StringComparison.Ordinal));
        }
        catch (ElementNotAvailableException)
        {
            return false;
        }
    }
}

/// <summary>
/// S76: removing the only AdditionalDealer_43 row of item 1422 removes exactly that row after one
/// confirmation, is undoable, and leaves every other dealer row and TonyInventory alone.
/// </summary>
internal sealed class ShopkeeperRemoveScenario : ShopkeeperCrudScenarioBase
{
    /// <inheritdoc />
    public override string Id => "S76";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var dealerFile = context.Data(DatasetKind.V5).TableFile(DealerFile);
        var tonyFile = context.Data(DatasetKind.V5).TableFile(TonyFile);
        var tonyHash = SaveDirtyTabScenario.HashFile(tonyFile);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1422", "12g Box BS");
        OpenShopkeeperTab(context);
        WaitShopkeeperCount(context, 1);
        context.Recorder.Check(
            "У предмета 1422 одна строка AdditionalDealer_43",
            HasDealerRow(context, "AdditionalDealer_43_"),
            null);
        context.Recorder.Check(
            "Количество равно 6",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue() == "6",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue());

        RemoveDealer(context, "1422", "AdditionalDealer_43_", "6");
        WaitShopkeeperCount(context, 0);
        context.Recorder.Check(
            "Строка исчезла из вкладки",
            !HasDealerRow(context, "AdditionalDealer_43_"),
            null);
        context.Recorder.Check(
            "Удаление делает вкладку грязной",
            window.WaitById("ItemSaveButton").IsEnabled(),
            null);

        Undo(context);
        WaitShopkeeperCount(context, 1);
        context.Recorder.Check(
            "Undo возвращает строку",
            HasDealerRow(context, "AdditionalDealer_43_"),
            null);
        context.Recorder.Check(
            "Undo возвращает количество 6",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue() == "6",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue());
        CheckClean(context);

        RemoveDealer(context, "1422", "AdditionalDealer_43_", "6");
        WaitShopkeeperCount(context, 0);
        WorkspaceUi.SaveItem(context);

        context.Recorder.Check("После сохранения в файле 43 строки", RecordCount(dealerFile) == 43, RecordCount(dealerFile).ToString());
        context.Recorder.Check(
            "Строка item 1422 удалена",
            StoredQuantity(dealerFile, "1422").Length == 0,
            StoredQuantity(dealerFile, "1422"));
        context.Recorder.Check(
            "Строка item 1456 не изменилась",
            StoredQuantity(dealerFile, "1456") == "3",
            StoredQuantity(dealerFile, "1456"));
        context.Recorder.Check(
            "TonyInventory не изменился",
            SaveDirtyTabScenario.HashFile(tonyFile) == tonyHash,
            null);

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1422", "12g Box BS");
        OpenShopkeeperTab(context);
        WaitShopkeeperCount(context, 0);
        context.Recorder.Check(
            "После переоткрытия у item 1422 нет строк",
            !HasDealerRow(context, "AdditionalDealer_43_"),
            null);
        context.Recorder.Check(
            "Торговец снова доступен для добавления",
            HasDealerOption(context, "AdditionalDealer_43_"),
            null);

        ScenarioUi.OpenTable(context, "NPCInventory.AdditionalDealer_43_Inventory");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "таблица дилера открыта",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check(
            "Таблица дилера содержит 43 строки",
            window.WaitById("RowsGrid").RowCount() == 43,
            $"строк: {window.WaitById("RowsGrid").RowCount()}");

        context.ExpectChange(DatasetKind.V5, "TableData/NPCInventory/AdditionalDealer_43_Inventory.xml");
    }
}

/// <summary>
/// S77: changing the AdditionalDealer_43 quantity of item 1456 to 7 keeps item 1422 at 6 even though both
/// rows share <c>uiIndex=26</c>; the duplicate-key warning stays and TonyInventory is untouched.
/// </summary>
internal sealed class ShopkeeperDuplicateEditScenario : ShopkeeperCrudScenarioBase
{
    /// <inheritdoc />
    public override string Id => "S77";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var dealerFile = context.Data(DatasetKind.V5).TableFile(DealerFile);
        var tonyFile = context.Data(DatasetKind.V5).TableFile(TonyFile);
        var tonyHash = SaveDirtyTabScenario.HashFile(tonyFile);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1456", "Dart Box");
        OpenShopkeeperTab(context);
        WaitShopkeeperCount(context, 2);
        context.Recorder.Check(
            "Строка AdditionalDealer_43 содержит 3",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue() == "3",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue());
        context.Recorder.Check(
            "Строка Tony содержит 1",
            WorkspaceUi.ShopkeeperQuantity(context, "Tony").ReadValue() == "1",
            WorkspaceUi.ShopkeeperQuantity(context, "Tony").ReadValue());

        SetDealerQuantity(context, "AdditionalDealer_43_", "7");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("EditorCellUpdated", "ubOptimalNumber", "26"),
            TimeSpan.FromSeconds(20));
        WorkspaceUi.SaveItem(context);

        context.Recorder.Check(
            "Строка item 1456 сохранена со значением 7",
            StoredQuantity(dealerFile, "1456") == "7",
            StoredQuantity(dealerFile, "1456"));
        context.Recorder.Check(
            "Строка item 1422 осталась 6",
            StoredQuantity(dealerFile, "1422") == "6",
            StoredQuantity(dealerFile, "1422"));
        context.Recorder.Check("В файле по-прежнему 44 строки", RecordCount(dealerFile) == 44, RecordCount(dealerFile).ToString());
        context.Recorder.Check(
            "В файле две строки с ключом 26",
            XDocument.Load(dealerFile).Descendants("INVENTORY")
                .Count(row => row.Element("uiIndex")?.Value.Trim() == "26") == 2,
            null);
        context.Recorder.Check(
            "Строка Tony у item 1456 осталась 1",
            XDocument.Load(tonyFile).Descendants("INVENTORY")
                .Any(row => row.Element("sItemIndex")?.Value.Trim() == "1456"
                    && row.Element("ubOptimalNumber")?.Value.Trim() == "1"),
            null);
        context.Recorder.Check(
            "TonyInventory не изменился",
            SaveDirtyTabScenario.HashFile(tonyFile) == tonyHash,
            null);

        CheckDuplicateKeyWarning(context, "26");

        WorkspaceUi.SelectItem(context, "1422", "12g Box BS");
        OpenShopkeeperTab(context);
        WaitShopkeeperCount(context, 1);
        context.Recorder.Check(
            "Item 1422 показывает 6",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue() == "6",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue());
        WorkspaceUi.SelectItem(context, "1456", "Dart Box");
        OpenShopkeeperTab(context);
        WaitShopkeeperCount(context, 2);
        context.Recorder.Check(
            "Item 1456 показывает 7",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue() == "7",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue());

        context.ExpectChange(DatasetKind.V5, "TableData/NPCInventory/AdditionalDealer_43_Inventory.xml");
    }
}

/// <summary>
/// S78: a dealer that does not stock item 12 can be given one row with quantity 3. The add is undoable
/// and redoable together with the dealer option, negative and fractional quantities are rejected, and
/// zero is accepted.
/// </summary>
internal sealed class ShopkeeperAddScenario : ShopkeeperCrudScenarioBase
{
    /// <inheritdoc />
    public override string Id => "S78";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var albertoFile = context.Data(DatasetKind.V5).TableFile(AlbertoFile);
        var before = XDocument.Load(albertoFile);
        var beforeRows = before.Descendants("INVENTORY").Count();

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        OpenShopkeeperTab(context);
        var baseCount = ReadShopkeeperCount(context);
        context.Recorder.Check(
            "У item 12 нет строки Alberto",
            !HasDealerRow(context, "Alberto"),
            null);
        SelectDealerOption(context, "Alberto");
        SetNewQuantity(context, "-1");
        AddInvalidQuantity(context, "-1");
        context.Recorder.Check("Отрицательное количество не добавило строку", ReadShopkeeperCount(context) == baseCount, null);
        CheckClean(context);

        SetNewQuantity(context, "1.5");
        AddInvalidQuantity(context, "1.5");
        context.Recorder.Check("Дробное количество не добавило строку", ReadShopkeeperCount(context) == baseCount, null);

        SetNewQuantity(context, "0");
        AddDealer(context, "Alberto");
        WaitShopkeeperCount(context, baseCount + 1);
        context.Recorder.Check(
            "Нулевое количество допустимо",
            WorkspaceUi.ShopkeeperQuantity(context, "Alberto").ReadValue() == "0",
            WorkspaceUi.ShopkeeperQuantity(context, "Alberto").ReadValue());
        context.Recorder.Check(
            "Alberto исчез из списка добавления",
            !HasDealerOption(context, "Alberto"),
            null);
        context.Recorder.Check(
            "Добавление делает вкладку грязной",
            window.WaitById("ItemSaveButton").IsEnabled(),
            null);

        Undo(context);
        WaitShopkeeperCount(context, baseCount);
        context.Recorder.Check(
            "Undo убирает строку",
            !HasDealerRow(context, "Alberto"),
            null);
        context.Recorder.Check(
            "Undo возвращает Alberto в список",
            HasDealerOption(context, "Alberto"),
            null);
        CheckClean(context);

        SelectDealerOption(context, "Alberto");
        SetNewQuantity(context, "3");
        AddDealer(context, "Alberto");
        WaitShopkeeperCount(context, baseCount + 1);
        context.Recorder.Check(
            "Количество новой строки равно 3",
            WorkspaceUi.ShopkeeperQuantity(context, "Alberto").ReadValue() == "3",
            WorkspaceUi.ShopkeeperQuantity(context, "Alberto").ReadValue());

        Undo(context);
        WaitShopkeeperCount(context, baseCount);
        context.Recorder.Check("Undo убирает строку с количеством 3", !HasDealerRow(context, "Alberto"), null);
        Redo(context);
        WaitShopkeeperCount(context, baseCount + 1);
        context.Recorder.Check(
            "Redo возвращает строку с количеством 3",
            WorkspaceUi.ShopkeeperQuantity(context, "Alberto").ReadValue() == "3",
            WorkspaceUi.ShopkeeperQuantity(context, "Alberto").ReadValue());

        WorkspaceUi.SaveItem(context);

        var after = XDocument.Load(albertoFile);
        var afterRows = after.Descendants("INVENTORY").ToList();
        var added = afterRows.Last();
        context.Recorder.Check("В файл добавлена одна строка", afterRows.Count == beforeRows + 1, $"строк: {afterRows.Count}");
        context.Recorder.Check(
            "Новая строка имеет uiIndex 26",
            added.Element("uiIndex")?.Value.Trim() == "26",
            added.Element("uiIndex")?.Value.Trim());
        context.Recorder.Check(
            "Новая строка ссылается на item 12",
            added.Element("sItemIndex")?.Value.Trim() == "12",
            added.Element("sItemIndex")?.Value.Trim());
        context.Recorder.Check(
            "Новая строка хранит количество 3",
            added.Element("ubOptimalNumber")?.Value.Trim() == "3",
            added.Element("ubOptimalNumber")?.Value.Trim());
        context.Recorder.Check(
            "Прежние строки файла не изменились",
            LeadingRecordsUnchanged(before, after, beforeRows),
            null);

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        OpenShopkeeperTab(context);
        context.Recorder.Check(
            "После переоткрытия Alberto показывает 3",
            WorkspaceUi.ShopkeeperQuantity(context, "Alberto").ReadValue() == "3",
            WorkspaceUi.ShopkeeperQuantity(context, "Alberto").ReadValue());
        context.Recorder.Check(
            "После переоткрытия Alberto не предлагается для добавления",
            !HasDealerOption(context, "Alberto"),
            null);

        context.ExpectChange(DatasetKind.V5, "TableData/NPCInventory/AlbertoInventory.xml");
    }
}

/// <summary>
/// S79: with two rows sharing <c>uiIndex=26</c>, deleting AdditionalDealer_43 from item 1456 must keep the
/// row of item 1422, keep the Tony row, and be undoable.
/// </summary>
internal sealed class ShopkeeperDuplicateRemoveScenario : ShopkeeperCrudScenarioBase
{
    /// <inheritdoc />
    public override string Id => "S79";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var dealerFile = context.Data(DatasetKind.V5).TableFile(DealerFile);
        var tonyFile = context.Data(DatasetKind.V5).TableFile(TonyFile);
        var tonyHash = SaveDirtyTabScenario.HashFile(tonyFile);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1456", "Dart Box");
        OpenShopkeeperTab(context);
        WaitShopkeeperCount(context, 2);
        context.Recorder.Check(
            "Строка AdditionalDealer_43 содержит 3",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue() == "3",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue());

        RemoveDealer(context, "1456", "AdditionalDealer_43_", "3");
        WaitShopkeeperCount(context, 1);
        context.Recorder.Check(
            "Строка Tony осталась",
            HasDealerRow(context, "Tony"),
            null);
        context.Recorder.Check(
            "Строка AdditionalDealer_43 исчезла",
            !HasDealerRow(context, "AdditionalDealer_43_"),
            null);

        Undo(context);
        WaitShopkeeperCount(context, 2);
        context.Recorder.Check(
            "Undo возвращает удалённую строку",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue() == "3",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue());
        CheckClean(context);

        RemoveDealer(context, "1456", "AdditionalDealer_43_", "3");
        WaitShopkeeperCount(context, 1);
        WorkspaceUi.SaveItem(context);

        context.Recorder.Check("После сохранения в файле 43 строки", RecordCount(dealerFile) == 43, RecordCount(dealerFile).ToString());
        context.Recorder.Check(
            "Строка item 1456 удалена",
            StoredQuantity(dealerFile, "1456").Length == 0,
            StoredQuantity(dealerFile, "1456"));
        context.Recorder.Check(
            "Строка item 1422 с тем же ключом осталась",
            StoredQuantity(dealerFile, "1422") == "6",
            StoredQuantity(dealerFile, "1422"));
        context.Recorder.Check(
            "В файле осталась одна строка с ключом 26",
            XDocument.Load(dealerFile).Descendants("INVENTORY")
                .Count(row => row.Element("uiIndex")?.Value.Trim() == "26") == 1,
            null);
        context.Recorder.Check(
            "Строка Tony у item 1456 осталась",
            XDocument.Load(tonyFile).Descendants("INVENTORY")
                .Any(row => row.Element("sItemIndex")?.Value.Trim() == "1456"
                    && row.Element("ubOptimalNumber")?.Value.Trim() == "1"),
            null);
        context.Recorder.Check(
            "TonyInventory не изменился",
            SaveDirtyTabScenario.HashFile(tonyFile) == tonyHash,
            null);

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1422", "12g Box BS");
        OpenShopkeeperTab(context);
        WaitShopkeeperCount(context, 1);
        context.Recorder.Check(
            "После переоткрытия item 1422 показывает 6",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue() == "6",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue());
        WorkspaceUi.SelectItem(context, "1456", "Dart Box");
        OpenShopkeeperTab(context);
        WaitShopkeeperCount(context, 1);
        context.Recorder.Check(
            "После переоткрытия item 1456 показывает только Tony",
            HasDealerRow(context, "Tony") && !HasDealerRow(context, "AdditionalDealer_43_"),
            null);

        context.ExpectChange(DatasetKind.V5, "TableData/NPCInventory/AdditionalDealer_43_Inventory.xml");
    }
}
