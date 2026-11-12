using System.Diagnostics;
using System.IO;
using System.Windows.Automation;
using XmlEditorReloaded.UiSmoke.Scenarios;

namespace XmlEditorReloaded.UiSmoke;

/// <summary>
/// Status of one checklist scenario in the smoke report.
/// </summary>
internal enum ScenarioStatus
{
    /// <summary>The scenario was automated and every check passed.</summary>
    Passed,

    /// <summary>The scenario was automated and at least one check failed.</summary>
    Failed,

    /// <summary>The scenario is automated but was not selected for this run.</summary>
    Skipped,

    /// <summary>The scenario can be automated but has no implementation yet.</summary>
    Planned,

    /// <summary>The scenario needs a human for a visual or interactive check.</summary>
    Manual,
}

/// <summary>
/// Result of one checklist scenario.
/// </summary>
internal sealed class ScenarioResult
{
    /// <summary>
    /// Gets or sets the checklist identifier.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the scenario title from the checklist.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the result status.
    /// </summary>
    public ScenarioStatus Status { get; set; }

    /// <summary>
    /// Gets the recorded checks.
    /// </summary>
    public List<CheckResult> Checks { get; } = [];

    /// <summary>
    /// Gets the notes and failure details.
    /// </summary>
    public List<string> Notes { get; } = [];

    /// <summary>
    /// Gets or sets the duration of the scenario.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets or sets the exception message when the scenario crashed.
    /// </summary>
    public string? Failure { get; set; }

    /// <summary>
    /// Gets or sets the paths of changed files for write scenarios.
    /// </summary>
    public List<string> ChangedFiles { get; } = [];
}

/// <summary>
/// Prepares fixtures, runs the selected scenarios, verifies data integrity, and writes the report.
/// </summary>
internal sealed class SmokeRunner
{
    private static readonly TimeSpan ElementTimeout = TimeSpan.FromSeconds(20);

    private readonly SmokeOptions _options;
    private readonly UiStrings _strings;
    private readonly ChecklistCatalog _catalog;
    private readonly FixtureBuilder _fixtures;
    private readonly List<ScenarioResult> _results = [];
    private readonly Dictionary<string, ScenarioResult> _byId = new(StringComparer.Ordinal);
    private readonly List<DatasetKind> _originalDatasets = [DatasetKind.V4, DatasetKind.V5];
    private readonly List<string> _copyIntegrityProblems = [];
    private int _originalFileCount;
    private int _copyChangedFileCount;
    private int _copyExpectedChangeCount;
    private UiApp? _lastApp;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmokeRunner"/> class.
    /// </summary>
    /// <param name="options">Runner options.</param>
    /// <param name="strings">Localized application strings.</param>
    /// <param name="catalog">Checklist catalog.</param>
    public SmokeRunner(SmokeOptions options, UiStrings strings, ChecklistCatalog catalog)
    {
        _options = options;
        _strings = strings;
        _catalog = catalog;
        _fixtures = new FixtureBuilder(options);
    }

    /// <summary>
    /// Runs the selected scenarios and writes the report.
    /// </summary>
    /// <returns>Process exit code: zero when every selected scenario passed.</returns>
    public int Run()
    {
        Directory.CreateDirectory(_options.WorkRoot);
        Console.WriteLine($"Work directory: {_options.WorkRoot}");
        Console.WriteLine($"Application:    {_options.ApplicationPath}");
        Console.WriteLine($"UI language:    {_options.UiLanguage}");
        Console.WriteLine();

        var originalsBefore = HashOriginals();
        _originalFileCount = originalsBefore.Count;
        var selected = SelectScenarios();
        foreach (var scenario in selected)
        {
            RunScenario(scenario);
        }

        var originalsAfter = HashOriginals();
        var originalsChanged = FixtureBuilder.Diff(originalsBefore, originalsAfter);
        AddIntegrityResult(originalsChanged);

        var commit = ReadGitCommit();
        var dirty = ReadGitDirty();
        ReportWriter.Write(
            _options,
            _results,
            _catalog,
            originalsChanged,
            commit,
            dirty,
            _byId,
            ScenarioCatalog.All.Select(static scenario => scenario.Id).ToList());

        var failed = _results.Count(static result => result.Status == ScenarioStatus.Failed);
        var passed = _results.Count(static result => result.Status == ScenarioStatus.Passed);
        Console.WriteLine();
        Console.WriteLine($"Automated scenarios: {passed} passed, {failed} failed, "
            + $"{_results.Count(static result => result.Status == ScenarioStatus.Skipped)} skipped.");
        Console.WriteLine($"Report: {Path.Combine(_options.WorkRoot, "report.md")}");
        if (originalsChanged.Count > 0)
        {
            Console.WriteLine("WARNING: reference data changed during the run:");
            foreach (var path in originalsChanged)
            {
                Console.WriteLine("  " + path);
            }
        }

        return failed == 0 && originalsChanged.Count == 0 ? 0 : 1;
    }

    private List<SmokeScenario> SelectScenarios()
    {
        var all = ScenarioCatalog.All;
        if (string.IsNullOrWhiteSpace(_options.Only))
        {
            return all.ToList();
        }

        var requested = _options.Only
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var selected = all.Where(scenario => requested.Contains(scenario.Id)).ToList();
        var missing = requested.Except(selected.Select(static scenario => scenario.Id), StringComparer.OrdinalIgnoreCase).ToList();
        if (missing.Count > 0)
        {
            Console.WriteLine("Not automated yet: " + string.Join(", ", missing));
            Console.WriteLine();
        }

        return selected;
    }

    private void RunScenario(SmokeScenario scenario)
    {
        var title = _catalog.TitleOf(scenario.Id);
        Console.Write($"{scenario.Id} {title} ... ");
        var stopwatch = Stopwatch.StartNew();
        var result = new ScenarioResult
        {
            Id = scenario.Id,
            Title = title,
            Status = ScenarioStatus.Failed,
        };
        _byId[scenario.Id] = result;
        _results.Add(result);

        var scenarioDirectory = Path.Combine(_options.WorkRoot, "scenarios", scenario.Id);
        UiApp? running = null;
        try
        {
            if (Directory.Exists(scenarioDirectory))
            {
                Directory.Delete(scenarioDirectory, recursive: true);
            }

            Directory.CreateDirectory(scenarioDirectory);

            var datasets = new Dictionary<DatasetKind, DatasetFixture>();
            foreach (var kind in scenario.Datasets)
            {
                datasets[kind] = _fixtures.Resolve(kind, scenarioDirectory, scenario.WritesTo(kind));
            }

            if (scenario.CorruptsV5AttachmentPoint)
            {
                FixtureBuilder.CorruptAttachmentPoint(datasets[DatasetKind.V5]);
            }

            var baseline = datasets
                .Where(static pair => pair.Value.IsCopy)
                .ToDictionary(
                    static pair => pair.Key,
                    static pair => FixtureBuilder.HashTree(pair.Value.DataDirectory));

            var appDataDirectory = Path.Combine(scenarioDirectory, "appdata");
            var recorder = new ScenarioRecorder();

            SettingsModel? settings = scenario.WritesSettings ? BuildSettings(scenario, datasets) : null;
            var app = UiApp.Start(_options, appDataDirectory, settings);
            running = app;
            _lastApp = app;
            UiApp Restart(SettingsModel? overrideSettings)
            {
                app.Close();
                app.Dispose();
                var next = UiApp.Start(_options, appDataDirectory, overrideSettings ?? settings);
                app = next;
                running = next;
                _lastApp = next;
                return next;
            }

            UiApp RestartWithJson(string settingsJson)
            {
                app.Close();
                app.Dispose();
                var next = UiApp.Start(
                    _options,
                    appDataDirectory,
                    settings: null,
                    rawSettingsJson: settingsJson);
                app = next;
                running = next;
                _lastApp = next;
                return next;
            }

            var context = new ScenarioContext(
                _options,
                _strings,
                recorder,
                scenarioDirectory,
                datasets,
                app,
                Restart,
                RestartWithJson);
            try
            {
                scenario.Run(context);
            }
            finally
            {
                // Checks are copied after the run because the recorder lives in the context.
                result.Checks.Clear();
                result.Checks.AddRange(recorder.Checks);
                result.Notes.Clear();
                result.Notes.AddRange(recorder.Notes);
                result.ChangedFiles.AddRange(VerifyFileChanges(scenario, datasets, baseline, context, result));
            }

            result.Status = result.Checks.All(static check => check.Passed)
                ? ScenarioStatus.Passed
                : ScenarioStatus.Failed;
        }
        catch (SmokeCheckException exception)
        {
            result.Status = ScenarioStatus.Failed;
            result.Failure = exception.Message;
            DumpUi(scenarioDirectory);
        }
        catch (UiTimeoutException exception)
        {
            result.Status = ScenarioStatus.Failed;
            result.Failure = "UI: " + exception.Message;
            DumpUi(scenarioDirectory);
        }
        catch (Exception exception)
        {
            result.Status = ScenarioStatus.Failed;
            result.Failure = exception.GetType().Name + ": " + exception.Message;
            DumpUi(scenarioDirectory);
        }
        finally
        {
            // The application is closed after the failure diagnostics, so the dump still sees its
            // windows and controls.
            if (running is not null)
            {
                try
                {
                    running.Close(TimeSpan.FromSeconds(25));
                }
                finally
                {
                    running.Dispose();
                }
            }

            result.Duration = stopwatch.Elapsed;
        }

        Console.WriteLine(result.Status == ScenarioStatus.Passed ? "PASS" : "FAIL");
        if (result.Status == ScenarioStatus.Failed && !string.IsNullOrWhiteSpace(result.Failure))
        {
            Console.WriteLine("    " + result.Failure.Replace(Environment.NewLine, Environment.NewLine + "    "));
        }
    }

    private SettingsModel BuildSettings(
        SmokeScenario scenario,
        IReadOnlyDictionary<DatasetKind, DatasetFixture> datasets)
    {
        var directories = scenario.Datasets.Select(kind => datasets[kind].DataDirectory).ToList();
        return new SettingsModel
        {
            DataDirectories = directories,
            ActiveDataDirectory = directories.Count == 0
                ? null
                : datasets[scenario.ActiveDataset].DataDirectory,
            ActiveLanguageCode = scenario.OverlayLanguage,
            UiLanguageCode = _options.UiLanguage,
            RecentDataDirectories = directories,
        };
    }

    private List<string> VerifyFileChanges(
        SmokeScenario scenario,
        IReadOnlyDictionary<DatasetKind, DatasetFixture> datasets,
        IReadOnlyDictionary<DatasetKind, Dictionary<string, string>> baseline,
        ScenarioContext context,
        ScenarioResult result)
    {
        var changedFiles = new List<string>();
        foreach (var pair in datasets.Where(static pair => pair.Value.IsCopy))
        {
            var kind = pair.Key;
            var after = FixtureBuilder.HashTree(pair.Value.DataDirectory);
            var changed = FixtureBuilder.Diff(baseline[kind], after);
            var expected = context.ExpectedChanges
                .Where(change => change.Kind == kind)
                .Select(static change => change.RelativePath)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var path in changed)
            {
                changedFiles.Add(path);
            }

            var unexpected = changed.Where(path => !expected.Contains(path)).ToList();
            var missing = expected.Where(path => !changed.Contains(path, StringComparer.OrdinalIgnoreCase)).ToList();
            _copyChangedFileCount += changed.Count;
            _copyExpectedChangeCount += expected.Count;
            if (unexpected.Count > 0 || missing.Count > 0)
            {
                _copyIntegrityProblems.Add(
                    $"{scenario.Id} ({FixtureBuilder.FolderName(kind)}): "
                    + $"лишние: {string.Join(", ", unexpected)}; "
                    + $"отсутствуют: {string.Join(", ", missing)}");
            }

            result.Checks.Add(new CheckResult(
                $"Изменения файлов {FixtureBuilder.FolderName(kind)} соответствуют ожидаемым",
                unexpected.Count == 0 && missing.Count == 0,
                unexpected.Count == 0 && missing.Count == 0
                    ? null
                    : $"лишние: {string.Join(", ", unexpected)}; отсутствуют: {string.Join(", ", missing)}"));
        }

        return changedFiles;
    }

    private void AddIntegrityResult(IReadOnlyList<string> originalsChanged)
    {
        var result = new ScenarioResult
        {
            Id = "S50",
            Title = _catalog.TitleOf("S50"),
        };
        result.Checks.Add(new CheckResult(
            "Оригиналы в References не изменились",
            originalsChanged.Count == 0,
            originalsChanged.Count == 0 ? null : string.Join(", ", originalsChanged)));
        result.Checks.Add(new CheckResult(
            "Все изменения копий заявлены сценариями",
            _copyIntegrityProblems.Count == 0,
            _copyIntegrityProblems.Count == 0
                ? null
                : string.Join(" | ", _copyIntegrityProblems.Take(10))));
        var artifacts = FindGitArtifacts();
        result.Checks.Add(new CheckResult(
            "В git нет артефактов сборки и пользовательских файлов",
            artifacts.Count == 0,
            artifacts.Count == 0 ? null : string.Join(", ", artifacts.Take(10))));
        result.Status = result.Checks.All(static check => check.Passed)
            ? ScenarioStatus.Passed
            : ScenarioStatus.Failed;
        result.Notes.Add(
            $"Проверены SHA-256 файлов TableData в References/JA 2 1.13 v4 и v5: {_originalFileCount}.");
        result.Notes.Add(
            $"Изменено файлов в копиях: {_copyChangedFileCount}; заявлено сценариями: {_copyExpectedChangeCount}.");
        _results.RemoveAll(static item => item.Id == "S50");
        _results.Add(result);
        _byId["S50"] = result;
    }

    /// <summary>
    /// Returns the paths reported by git that must never be committed: build output, IDE caches, and
    /// per-user files. Ignored paths do not appear in <c>git status</c>, so only real candidates remain.
    /// </summary>
    /// <returns>Offending repository-relative paths.</returns>
    private IReadOnlyList<string> FindGitArtifacts()
    {
        var output = RunGit("status --porcelain");
        if (string.IsNullOrWhiteSpace(output))
        {
            return [];
        }

        var artifacts = new List<string>();
        foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = line.TrimEnd('\r');
            if (trimmed.Length <= 3)
            {
                continue;
            }

            foreach (var part in trimmed[3..].Split(" -> ", StringSplitOptions.TrimEntries))
            {
                var path = part.Trim('"').Replace('\\', '/');
                if (IsBuildArtifact(path))
                {
                    artifacts.Add(path);
                }
            }
        }

        return artifacts;
    }

    private static bool IsBuildArtifact(string path)
    {
        var segments = path.Split('/');
        if (segments.Any(static segment =>
                segment is "bin" or "obj" or ".vs" or ".idea"))
        {
            return true;
        }

        return path.EndsWith(".user", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".suo", StringComparison.OrdinalIgnoreCase);
    }

    private Dictionary<string, string> HashOriginals()
    {
        var hashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kind in _originalDatasets)
        {
            var tableData = Path.Combine(_fixtures.ReferenceGameRoot(kind), "Data-1.13", "TableData");
            foreach (var pair in FixtureBuilder.HashTree(tableData))
            {
                hashes[$"{FixtureBuilder.FolderName(kind)}/{pair.Key}"] = pair.Value;
            }
        }

        return hashes;
    }

    private void DumpUi(string scenarioDirectory)
    {
        try
        {
            var app = _lastApp;
            if (app is null)
            {
                return;
            }

            var windows = new List<AutomationElement>();
            try
            {
                windows.Add(app.Window);
            }
            catch (ElementNotAvailableException)
            {
                // The main window is already gone.
            }

            try
            {
                var processWindows = AutomationElement.RootElement.FindAll(
                    TreeScope.Children,
                    new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId));
                windows.AddRange(processWindows.Cast<AutomationElement>());
            }
            catch (Exception exception) when (exception is ElementNotAvailableException or InvalidOperationException)
            {
                // Only the main window is dumped.
            }

            var index = 0;
            foreach (var window in windows.Distinct())
            {
                index++;
                var path = Path.Combine(scenarioDirectory, $"uia-{app.ProcessId}-{index}.txt");
                try
                {
                    window.DumpTree(path);
                    Console.WriteLine("    UI tree: " + path);
                }
                catch (Exception exception) when (
                    exception is ElementNotAvailableException or InvalidOperationException or IOException)
                {
                    // The window disappeared while it was being dumped.
                }
            }
        }
        catch (Exception exception) when (exception is ElementNotAvailableException or InvalidOperationException)
        {
            Console.WriteLine("    UI dump failed: " + exception.GetType().Name + ": " + exception.Message);
        }
    }

    private string? ReadGitCommit()
    {
        var output = RunGit("rev-parse --short HEAD");
        return output?.Trim();
    }

    private bool ReadGitDirty()
    {
        var output = RunGit("status --porcelain");
        return !string.IsNullOrWhiteSpace(output);
    }

    private string? RunGit(string arguments)
    {
        try
        {
            var startInfo = new ProcessStartInfo("git", arguments)
            {
                WorkingDirectory = _options.RepositoryRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return null;
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(10_000);
            return output;
        }
        catch (Exception exception) when (exception is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the default element timeout used by the scenarios.
    /// </summary>
    public static TimeSpan DefaultElementTimeout => ElementTimeout;
}
