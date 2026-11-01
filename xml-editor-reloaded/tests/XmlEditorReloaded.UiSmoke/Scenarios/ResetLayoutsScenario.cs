using System.IO;
using System.Text.Json;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S45: "Reset all layouts" warns about the pending reset, deletes the stored layout on OK, restores
/// the default columns of an open tab, and leaves the XML untouched.
/// </summary>
internal sealed class ResetLayoutsScenario : SmokeScenario
{
    private const string Table = "Items.Weapons";

    /// <inheritdoc />
    public override string Id => "S45";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v4 = context.Data(DatasetKind.V4);
        var file = v4.TableFile("Items/Weapons.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(file);
        var layoutsPath = Path.Combine(context.App.AppDataDirectory, "layouts.json");

        ScenarioUi.OpenTable(context, Table);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            $"{Table} открыт");

        // A default click persists the baseline layout, so the reset has a file to delete.
        ScenarioUi.SetExpanded(context.Window.WaitById("ColumnsExpander"), expanded: true);
        context.Window.WaitById("DefaultColumnsButton").InvokeButton();
        Ui.WaitUntil(
            () => ReadLayout(layoutsPath)?.HasEntry == true,
            "layouts.json содержит исходную раскладку",
            TimeSpan.FromSeconds(30));
        var defaultSort = ReadLayout(layoutsPath)?.SortField;
        context.Recorder.Check(
            "Исходная раскладка не имеет сортировки",
            string.IsNullOrEmpty(defaultSort),
            defaultSort ?? "нет");

        // Sorting through the column header is persisted; the reset has to undo it for the open tab.
        var firstFileIndex = ReadFirstIndex(file);
        var uiIndexHeader = ScenarioUi.WaitForColumnHeader(context.Window, "uiIndex");
        uiIndexHeader.InvokeButton();
        Thread.Sleep(400);
        ScenarioUi.WaitForColumnHeader(context.Window, "uiIndex").InvokeButton();
        var sortField = string.Empty;
        var sortDirection = string.Empty;
        Ui.WaitUntil(
            () =>
            {
                var layout = ReadLayout(layoutsPath);
                sortField = layout?.SortField ?? string.Empty;
                sortDirection = layout?.SortDirection ?? string.Empty;
                return sortField == "uiIndex"
                    && string.Equals(sortDirection, "descending", StringComparison.Ordinal);
            },
            "layouts.json сохраняет сортировку по uiIndex",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check(
            "Раскладка изменена до сброса",
            sortField == "uiIndex" && sortDirection == "descending",
            $"{sortField} {sortDirection}");

        var dialog = SettingsUi.Open(context);
        var message = SettingsUi.ResetLayouts(context, dialog);
        context.Recorder.Check(
            "Показано предупреждение об удалении всех макетов",
            message.Contains(
                ScenarioUi.FormatPrefix(context.Strings.Get("SettingsResetLayoutsConfirm")),
                StringComparison.Ordinal),
            message);
        context.Recorder.Check(
            "После подтверждения показано ожидание сброса",
            dialog.WaitById("SettingsResetLayoutsPendingText").ReadName()
                == context.Strings.Get("SettingsResetLayoutsPending"));

        SettingsUi.Close(context, dialog, accept: true);
        ScenarioUi.WaitForStatus(
            context,
            text => text == context.Strings.Get("SettingsApplied"),
            "настройки сохранены после сброса макетов",
            TimeSpan.FromSeconds(30));

        Ui.WaitUntil(
            () => !File.Exists(layoutsPath),
            "layouts.json удалён после сброса",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Файл макетов очищен", !File.Exists(layoutsPath));

        var firstAfterReset = string.Empty;
        Ui.WaitUntil(
            () =>
            {
                firstAfterReset = context.Window.WaitById("RowsGrid").Cell(0, 0).ReadName();
                return string.Equals(firstAfterReset, firstFileIndex, StringComparison.Ordinal);
            },
            "открытая вкладка вернулась к исходному порядку строк",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Открытая вкладка получила исходную сортировку",
            firstAfterReset == firstFileIndex,
            $"{firstAfterReset} против {firstFileIndex}");

        context.Recorder.Check(
            "Сброс макетов не изменил XML",
            SaveDirtyTabScenario.HashFile(file) == hashBefore);
    }

    private static string ReadFirstIndex(string file)
    {
        var root = System.Xml.Linq.XDocument.Load(file).Root
            ?? throw new UiTimeoutException($"Файл «{file}» не содержит корневого элемента.");
        return root.Elements().First().Element("uiIndex")?.Value.Trim()
            ?? throw new UiTimeoutException($"Файл «{file}» не содержит uiIndex.");
    }

    private static LayoutSnapshot? ReadLayout(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using var document = JsonDocument.Parse(File.ReadAllText(path));
            if (!document.RootElement.TryGetProperty("layouts", out var layouts))
            {
                return new LayoutSnapshot(false, null, null);
            }

            foreach (var entry in layouts.EnumerateArray())
            {
                if (!entry.TryGetProperty("table", out var table)
                    || !string.Equals(table.GetString(), Table, StringComparison.Ordinal))
                {
                    continue;
                }

                string? sortField = null;
                string? sortDirection = null;
                if (entry.TryGetProperty("sort", out var sort) && sort.ValueKind == JsonValueKind.Object)
                {
                    sortField = sort.TryGetProperty("field", out var field) ? field.GetString() : null;
                    sortDirection = sort.TryGetProperty("direction", out var direction)
                        ? direction.GetString()
                        : null;
                }

                return new LayoutSnapshot(true, sortField, sortDirection);
            }

            return new LayoutSnapshot(false, null, null);
        }
        catch (Exception exception) when (
            exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    private sealed record LayoutSnapshot(bool HasEntry, string? SortField, string? SortDirection);
}
