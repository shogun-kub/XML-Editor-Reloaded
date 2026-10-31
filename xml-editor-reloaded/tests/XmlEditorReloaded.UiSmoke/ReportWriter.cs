using System.Text;
using System.IO;
using System.Text.Json;
using XmlEditorReloaded.UiSmoke.Scenarios;

namespace XmlEditorReloaded.UiSmoke;

/// <summary>
/// Writes the smoke result as a Markdown table that mirrors the manual checklist and as JSON.
/// </summary>
internal static class ReportWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// Writes <c>report.md</c> and <c>report.json</c> into the work directory.
    /// </summary>
    /// <param name="options">Runner options.</param>
    /// <param name="results">Automated results in execution order.</param>
    /// <param name="catalog">Checklist catalog.</param>
    /// <param name="originalsChanged">Reference files that changed during the run.</param>
    /// <param name="commit">Current git commit, or <see langword="null"/>.</param>
    /// <param name="dirty">Whether the working tree is dirty.</param>
    /// <param name="byId">Automated results by identifier.</param>
    /// <param name="implementedIds">Identifiers of every implemented scenario.</param>
    public static void Write(
        SmokeOptions options,
        IReadOnlyList<ScenarioResult> results,
        ChecklistCatalog catalog,
        IReadOnlyList<string> originalsChanged,
        string? commit,
        bool dirty,
        IReadOnlyDictionary<string, ScenarioResult> byId,
        IReadOnlyCollection<string> implementedIds)
    {
        Directory.CreateDirectory(options.WorkRoot);
        var ids = MergeIds(results, catalog);
        var builder = new StringBuilder();
        builder.AppendLine("# UI smoke — XML Editor Reloaded");
        builder.AppendLine();
        builder.AppendLine($"- Дата: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
        builder.AppendLine($"- Приложение: `{options.ApplicationPath}`");
        builder.AppendLine($"- Коммит: `{commit ?? "unknown"}`{(dirty ? " (рабочее дерево с изменениями)" : string.Empty)}");
        builder.AppendLine($"- Язык интерфейса: `{options.UiLanguage}`");
        builder.AppendLine($"- Каталог прогона: `{options.WorkRoot}`");
        var automated = results.Count(static result => result.Status is ScenarioStatus.Passed or ScenarioStatus.Failed);
        var passed = results.Count(static result => result.Status == ScenarioStatus.Passed);
        var failed = results.Count(static result => result.Status == ScenarioStatus.Failed);
        builder.AppendLine($"- Автоматизированные сценарии: {passed}/{automated} прошли, {failed} с ошибками");
        builder.AppendLine($"- Оригиналы в `References/`: {(originalsChanged.Count == 0 ? "не изменены" : "ИЗМЕНЕНЫ")}");
        builder.AppendLine();

        builder.AppendLine("## Сценарии");
        builder.AppendLine();
        builder.AppendLine("| № | Сценарий | Результат | Примечание |");
        builder.AppendLine("|---|---|---|---|");
        foreach (var id in ids)
        {
            var title = catalog.TitleOf(id);
            var status = StatusOf(id, results, implementedIds);
            builder.AppendLine($"| {id} | {Escape(title)} | {Symbol(status)} | {Escape(NoteOf(id, results, status))} |");
        }

        builder.AppendLine();
        builder.AppendLine("## Проверки");
        builder.AppendLine();
        foreach (var result in results.Where(static result => result.Checks.Count > 0 || result.Notes.Count > 0))
        {
            builder.AppendLine($"### {result.Id}. {result.Title}");
            builder.AppendLine();
            foreach (var check in result.Checks)
            {
                var details = string.IsNullOrWhiteSpace(check.Details) ? string.Empty : $" — {check.Details}";
                builder.AppendLine($"- {(check.Passed ? "✅" : "❌")} {check.Description}{details}");
            }

            foreach (var note in result.Notes)
            {
                builder.AppendLine($"- ℹ️ {note}");
            }

            if (!string.IsNullOrWhiteSpace(result.Failure))
            {
                builder.AppendLine($"- ❌ Ошибка сценария: {result.Failure}");
            }

            builder.AppendLine();
        }

        var manual = catalog.Ids.Where(id => !implementedIds.Contains(id) && ScenarioCatalog.IsManual(id)).ToList();
        if (manual.Count > 0)
        {
            builder.AppendLine("## Требуют человека");
            builder.AppendLine();
            foreach (var id in manual)
            {
                builder.AppendLine($"- **{id}. {catalog.TitleOf(id)}** — {ScenarioCatalog.ManualReason(id)}");
            }

            builder.AppendLine();
        }

        var planned = catalog.Ids
            .Where(id => !implementedIds.Contains(id) && !ScenarioCatalog.IsManual(id))
            .ToList();
        builder.AppendLine("## Пока не автоматизированы");
        builder.AppendLine();
        builder.AppendLine(planned.Count == 0
            ? "Все сценарии чеклиста автоматизированы или отнесены к ручным."
            : string.Join(", ", planned));
        builder.AppendLine();

        File.WriteAllText(Path.Combine(options.WorkRoot, "report.md"), builder.ToString(), new UTF8Encoding(false));

        var payload = new
        {
            generatedAt = DateTimeOffset.Now,
            application = options.ApplicationPath,
            commit,
            dirty,
            uiLanguage = options.UiLanguage,
            workRoot = options.WorkRoot,
            originalsChanged,
            scenarios = results.Select(result => new
            {
                id = result.Id,
                title = result.Title,
                status = result.Status.ToString(),
                durationMs = (int)result.Duration.TotalMilliseconds,
                changedFiles = result.ChangedFiles,
                notes = result.Notes,
                failure = result.Failure,
                checks = result.Checks.Select(check => new
                {
                    check.Description,
                    check.Passed,
                    check.Details,
                }),
            }),
        };
        File.WriteAllText(
            Path.Combine(options.WorkRoot, "report.json"),
            JsonSerializer.Serialize(payload, JsonOptions),
            new UTF8Encoding(false));
    }

    /// <summary>
    /// Describes the status of a checklist row.
    /// </summary>
    /// <param name="id">Scenario identifier.</param>
    /// <param name="results">Automated results.</param>
    /// <param name="implementedIds">Identifiers of every implemented scenario.</param>
    /// <returns>Scenario status.</returns>
    public static ScenarioStatus StatusOf(
        string id,
        IReadOnlyList<ScenarioResult> results,
        IReadOnlyCollection<string> implementedIds)
    {
        var result = results.FirstOrDefault(item => item.Id == id);
        if (result is not null)
        {
            return result.Status;
        }

        if (implementedIds.Contains(id))
        {
            return ScenarioStatus.Skipped;
        }

        return ScenarioCatalog.IsManual(id) ? ScenarioStatus.Manual : ScenarioStatus.Planned;
    }

    private static IEnumerable<string> MergeIds(
        IReadOnlyList<ScenarioResult> results,
        ChecklistCatalog catalog)
    {
        var ids = new List<string>(catalog.Ids);
        foreach (var result in results)
        {
            if (!ids.Contains(result.Id, StringComparer.Ordinal))
            {
                ids.Add(result.Id);
            }
        }

        return ids;
    }

    private static string NoteOf(string id, IReadOnlyList<ScenarioResult> results, ScenarioStatus status)
    {
        var result = results.FirstOrDefault(item => item.Id == id);
        if (result is null)
        {
            return status == ScenarioStatus.Manual ? ScenarioCatalog.ManualReason(id) : "автоматизация запланирована";
        }

        if (!string.IsNullOrWhiteSpace(result.Failure))
        {
            return result.Failure;
        }

        if (result.Notes.Count > 0)
        {
            return string.Join(" ", result.Notes);
        }

        return status == ScenarioStatus.Skipped ? "не выбран для этого прогона" : string.Empty;
    }

    private static string Symbol(ScenarioStatus status) => status switch
    {
        ScenarioStatus.Passed => "✅",
        ScenarioStatus.Failed => "❌",
        ScenarioStatus.Skipped => "⏭",
        ScenarioStatus.Manual => "🖐",
        _ => "➖",
    };

    private static string Escape(string value) => value.Replace("|", "\\|");
}
