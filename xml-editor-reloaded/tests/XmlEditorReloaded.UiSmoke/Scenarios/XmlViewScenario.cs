using System.IO;
using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S101: the global view switch replaces the whole table editor with a read-only XML snapshot.
/// </summary>
internal sealed class XmlViewScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S101";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        ScenarioUi.OpenTable(context, "Items.Weapons");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Items.Weapons открыт");

        context.Recorder.Check("Редактор по умолчанию в режиме Grid", window.FindById("RowsGrid") is not null);

        window.WaitById("TableXmlViewButton").SelectItem();
        var xmlBox = window.WaitById("XmlTextBox");
        Ui.WaitUntil(
            () => xmlBox.ReadValue().Contains("<WEAPON", StringComparison.Ordinal),
            "XML-снимок таблицы загружен",
            TimeSpan.FromSeconds(30));
        Ui.WaitUntil(
            () => window.FindById("RowsGrid") is null,
            "сетка скрыта в режиме XML",
            TimeSpan.FromSeconds(10));

        context.Recorder.Check("Сетка скрыта", window.FindById("RowsGrid") is null);
        context.Recorder.Check("Тулбар редактирования скрыт", window.FindById("UndoButton") is null);
        context.Recorder.Check("Поиск скрыт", window.FindById("SearchBox") is null);
        context.Recorder.Check("Фильтр скрыт", window.FindById("FilterExpander") is null);
        context.Recorder.Check("Выбор колонок скрыт", window.FindById("ColumnsList") is null);

        var xml = xmlBox.ReadValue();
        context.Recorder.Check(
            "XML начинается с объявления",
            xml.StartsWith("<?xml", StringComparison.Ordinal),
            xml[..Math.Min(80, xml.Length)]);
        context.Recorder.Check(
            "XML содержит записи таблицы",
            xml.Contains("<WEAPON>", StringComparison.Ordinal)
            && xml.Contains("<uiIndex>", StringComparison.Ordinal),
            "WEAPON/uiIndex");

        var readOnly = false;
        if (xmlBox.TryGetCurrentPattern(ValuePattern.Pattern, out var raw)
            && raw is ValuePattern value)
        {
            readOnly = value.Current.IsReadOnly;
        }

        context.Recorder.Check("XML-поле только для чтения", readOnly, readOnly.ToString());
        context.Recorder.Check(
            "Кнопка копирования XML доступна",
            window.FindById("CopyXmlButton") is not null,
            "CopyXmlButton");

        var search = window.WaitById("XmlSearchBox");
        context.Recorder.Check(
            "Поиск по ID/ключу доступен",
            window.FindById("XmlSearchButton") is not null,
            "XmlSearchBox/XmlSearchButton");
        search.SetText("1");
        window.WaitById("XmlSearchButton").InvokeButton();
        var found = ScenarioUi.WaitForStatus(
            context,
            text => text == context.Strings.Format("EditorXmlSearchFound", "1"),
            "найдена запись 1",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check(
            "Поиск по ID/ключу нашёл запись 1",
            found == context.Strings.Format("EditorXmlSearchFound", "1"),
            found);
        var selectedXml = string.Empty;
        if (xmlBox.TryGetCurrentPattern(TextPattern.Pattern, out var textPattern)
            && textPattern is TextPattern pattern)
        {
            var ranges = pattern.GetSelection();
            selectedXml = ranges.Length > 0 ? ranges[0].GetText(-1) : string.Empty;
        }

        context.Recorder.Check(
            "Поиск выделил открывающий тег записи",
            selectedXml.StartsWith("<WEAPON>", StringComparison.Ordinal),
            selectedXml);

        search.SetText("no-such-key");
        window.WaitById("XmlSearchButton").InvokeButton();
        var notFound = ScenarioUi.WaitForStatus(
            context,
            text => text == context.Strings.Format("EditorXmlSearchNotFound", "no-such-key"),
            "сообщение об отсутствии записи",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check(
            "Поиск сообщает об отсутствии записи",
            notFound == context.Strings.Format("EditorXmlSearchNotFound", "no-such-key"),
            notFound);

        Screenshots.Capture(
            context.App,
            Path.Combine(context.Directory, "S101-xml-view.png"));

        window.WaitById("TableGridViewButton").SelectItem();
        window.WaitById("RowsGrid");
        context.Recorder.Check(
            "Возврат к Grid восстанавливает сетку",
            window.FindById("RowsGrid") is not null,
            "RowsGrid");
    }
}
