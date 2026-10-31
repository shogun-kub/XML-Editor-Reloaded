using System.Windows.Automation;
using System.IO;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Base class of one automated smoke scenario.
/// </summary>
internal abstract class SmokeScenario
{
    /// <summary>
    /// Gets the checklist identifier, for example <c>S06</c>.
    /// </summary>
    public abstract string Id { get; }

    /// <summary>
    /// Gets the data sets the scenario needs.
    /// </summary>
    public virtual IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <summary>
    /// Gets the data set that is active when the application starts.
    /// </summary>
    public virtual DatasetKind ActiveDataset => Datasets.Count > 0 ? Datasets[0] : DatasetKind.V4;

    /// <summary>
    /// Gets the overlay language code selected in the settings, or <see langword="null"/> for the base
    /// table.
    /// </summary>
    public virtual string? OverlayLanguage => null;

    /// <summary>
    /// Determines whether the scenario writes into the given data set and therefore needs a copy.
    /// </summary>
    /// <param name="kind">Data set kind.</param>
    /// <returns><see langword="true"/> for a writable copy.</returns>
    public virtual bool WritesTo(DatasetKind kind) => false;

    /// <summary>
    /// Gets a value indicating whether the scenario starts with a settings file. A cold start runs
    /// without any settings file, so the application uses its built-in defaults.
    /// </summary>
    public virtual bool WritesSettings => true;

    /// <summary>
    /// Runs the scenario.
    /// </summary>
    /// <param name="context">Scenario context with the running application and fixtures.</param>
    public abstract void Run(ScenarioContext context);
}

/// <summary>
/// One assertion of a scenario.
/// </summary>
/// <param name="Description">What was checked.</param>
/// <param name="Passed">Whether the check passed.</param>
/// <param name="Details">Actual value or failure description.</param>
internal sealed record CheckResult(string Description, bool Passed, string? Details);

/// <summary>
/// Collects checks, notes, and expected file changes of one scenario.
/// </summary>
internal sealed class ScenarioRecorder
{
    private readonly List<CheckResult> _checks = [];
    private readonly List<string> _notes = [];

    /// <summary>
    /// Gets the checks recorded so far.
    /// </summary>
    public IReadOnlyList<CheckResult> Checks => _checks;

    /// <summary>
    /// Gets the notes recorded so far.
    /// </summary>
    public IReadOnlyList<string> Notes => _notes;

    /// <summary>
    /// Records a check and aborts the scenario when it fails.
    /// </summary>
    /// <param name="description">What is being checked.</param>
    /// <param name="passed">Whether the check passed.</param>
    /// <param name="details">Actual value or failure description.</param>
    /// <exception cref="SmokeCheckException">Thrown when the check fails.</exception>
    public void Check(string description, bool passed, string? details = null)
    {
        _checks.Add(new CheckResult(description, passed, details));
        if (!passed)
        {
            throw new SmokeCheckException(description, details);
        }
    }

    /// <summary>
    /// Records a check without aborting the scenario when it fails.
    /// </summary>
    /// <param name="description">What is being checked.</param>
    /// <param name="passed">Whether the check passed.</param>
    /// <param name="details">Actual value or failure description.</param>
    public void SoftCheck(string description, bool passed, string? details = null) =>
        _checks.Add(new CheckResult(description, passed, details));

    /// <summary>
    /// Adds a free-form note to the scenario result.
    /// </summary>
    /// <param name="message">Note text.</param>
    public void Note(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            _notes.Add(message);
        }
    }
}

/// <summary>
/// Thrown when a scenario check fails.
/// </summary>
internal sealed class SmokeCheckException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SmokeCheckException"/> class.
    /// </summary>
    /// <param name="description">Failed check description.</param>
    /// <param name="details">Actual value or failure description.</param>
    public SmokeCheckException(string description, string? details)
        : base(string.IsNullOrWhiteSpace(details) ? description : $"{description}: {details}")
    {
        Description = description;
        Details = details;
    }

    /// <summary>
    /// Gets the failed check description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the actual value or failure description.
    /// </summary>
    public string? Details { get; }
}

/// <summary>
/// Everything a scenario needs: the running application, resolved data sets, and the recorder.
/// </summary>
internal sealed class ScenarioContext
{
    private readonly Func<SettingsModel?, UiApp> _restartApp;
    private readonly Dictionary<DatasetKind, DatasetFixture> _datasets;
    private readonly List<(DatasetKind Kind, string RelativePath)> _expectedChanges = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ScenarioContext"/> class.
    /// </summary>
    /// <param name="options">Runner options.</param>
    /// <param name="strings">Localized application strings.</param>
    /// <param name="recorder">Check recorder.</param>
    /// <param name="directory">Scenario work directory.</param>
    /// <param name="datasets">Resolved data sets.</param>
    /// <param name="app">Started application.</param>
    /// <param name="restartApp">Restarts the application with the given or the current settings.</param>
    public ScenarioContext(
        SmokeOptions options,
        UiStrings strings,
        ScenarioRecorder recorder,
        string directory,
        Dictionary<DatasetKind, DatasetFixture> datasets,
        UiApp app,
        Func<SettingsModel?, UiApp> restartApp)
    {
        Options = options;
        Strings = strings;
        Recorder = recorder;
        Directory = directory;
        _datasets = datasets;
        _restartApp = restartApp;
        App = app;
    }

    /// <summary>
    /// Gets the runner options.
    /// </summary>
    public SmokeOptions Options { get; }

    /// <summary>
    /// Gets the localized application strings.
    /// </summary>
    public UiStrings Strings { get; }

    /// <summary>
    /// Gets the check recorder.
    /// </summary>
    public ScenarioRecorder Recorder { get; }

    /// <summary>
    /// Gets the scenario work directory.
    /// </summary>
    public string Directory { get; }

    /// <summary>
    /// Gets the running application.
    /// </summary>
    public UiApp App { get; private set; }

    /// <summary>
    /// Gets the main window of the running application.
    /// </summary>
    public AutomationElement Window => App.RefreshWindow();

    /// <summary>
    /// Gets the file changes the scenario intends to make.
    /// </summary>
    public IReadOnlyList<(DatasetKind Kind, string RelativePath)> ExpectedChanges => _expectedChanges;

    /// <summary>
    /// Returns a resolved data set.
    /// </summary>
    /// <param name="kind">Data set kind.</param>
    /// <returns>The data-set fixture.</returns>
    public DatasetFixture Data(DatasetKind kind) => _datasets[kind];

    /// <summary>
    /// Declares a file that the scenario is expected to modify, relative to the data directory.
    /// </summary>
    /// <param name="kind">Data set kind.</param>
    /// <param name="relativePath">Path such as <c>TableData/Lookup.Cursor.xml</c>.</param>
    public void ExpectChange(DatasetKind kind, string relativePath) =>
        _expectedChanges.Add((kind, relativePath.Replace('\\', '/')));

    /// <summary>
    /// Closes the application and starts it again with the same settings or the given ones.
    /// </summary>
    /// <param name="settings">Settings to write before the restart, or <see langword="null"/> to keep
    /// the current settings file.</param>
    public void RestartApp(SettingsModel? settings = null) => App = _restartApp(settings);
}
