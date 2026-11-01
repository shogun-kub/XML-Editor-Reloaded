using System.Text;
using System.IO;

namespace XmlEditorReloaded.UiSmoke;

/// <summary>
/// Command-line options of the UI smoke runner.
/// </summary>
internal sealed class SmokeOptions
{
    /// <summary>
    /// Gets the absolute path of the application executable under test.
    /// </summary>
    public required string ApplicationPath { get; init; }

    /// <summary>
    /// Gets the repository root that contains <c>References</c> and <c>xml-editor-reloaded</c>.
    /// </summary>
    public required string RepositoryRoot { get; init; }

    /// <summary>
    /// Gets the directory that holds the read-only game data sets.
    /// </summary>
    public required string ReferencesRoot { get; init; }

    /// <summary>
    /// Gets the directory for fixtures, logs, and the report.
    /// </summary>
    public required string WorkRoot { get; init; }

    /// <summary>
    /// Gets the UI language code the application is started with.
    /// </summary>
    public string UiLanguage { get; init; } = "ru";

    /// <summary>
    /// Gets the comma-separated scenario identifiers to run, or <see langword="null"/> for every
    /// implemented scenario.
    /// </summary>
    public string? Only { get; init; }

    /// <summary>
    /// Gets a value indicating whether the work directory is kept after the run.
    /// </summary>
    public bool Keep { get; init; }

    /// <summary>
    /// Gets a value indicating whether the scenario catalog is printed without running anything.
    /// </summary>
    public bool List { get; init; }

    /// <summary>
    /// Gets the directory that receives reference screenshots, or <see langword="null"/> when the
    /// runner executes scenarios.
    /// </summary>
    public string? ShotsDirectory { get; init; }

    /// <summary>
    /// Gets the timeout applied to one application start.
    /// </summary>
    public TimeSpan StartupTimeout { get; init; } = TimeSpan.FromSeconds(90);

    /// <summary>
    /// Parses the command line.
    /// </summary>
    /// <param name="args">Raw command-line arguments.</param>
    /// <returns>Parsed options.</returns>
    /// <exception cref="ArgumentException">Thrown when an option value is missing or unknown.</exception>
    public static SmokeOptions Parse(string[] args)
    {
        var repositoryRoot = FindRepositoryRoot();
        string? app = null;
        string? data = null;
        string? work = null;
        var language = "ru";
        string? only = null;
        var keep = false;
        var list = false;
        string? shots = null;

        for (var index = 0; index < args.Length; index++)
        {
            var argument = args[index];
            string NextValue()
            {
                if (index + 1 >= args.Length)
                {
                    throw new ArgumentException($"Option '{argument}' requires a value.");
                }

                return args[++index];
            }

            switch (argument)
            {
                case "--app":
                    app = NextValue();
                    break;
                case "--data":
                    data = NextValue();
                    break;
                case "--work":
                    work = NextValue();
                    break;
                case "--lang":
                    language = NextValue();
                    break;
                case "--only":
                    only = NextValue();
                    break;
                case "--keep":
                    keep = true;
                    break;
                case "--list":
                    list = true;
                    break;
                case "--shots":
                    shots = NextValue();
                    break;
                case "--help":
                case "-h":
                    throw new ArgumentException(Usage);
                default:
                    throw new ArgumentException($"Unknown option '{argument}'.{Environment.NewLine}{Usage}");
            }
        }

        var applicationPath = app ?? Path.Combine(
            repositoryRoot,
            "out",
            "bin",
            "XmlEditorReloaded.App",
            "debug",
            "XmlEditorReloaded.App.exe");
        if (!File.Exists(applicationPath) && app is null)
        {
            var release = Path.Combine(
                repositoryRoot,
                "out",
                "bin",
                "XmlEditorReloaded.App",
                "release",
                "XmlEditorReloaded.App.exe");
            if (File.Exists(release))
            {
                applicationPath = release;
            }
        }

        return new SmokeOptions
        {
            ApplicationPath = Path.GetFullPath(applicationPath),
            RepositoryRoot = repositoryRoot,
            ReferencesRoot = Path.GetFullPath(data ?? Path.Combine(repositoryRoot, "References")),
            WorkRoot = Path.GetFullPath(
                work ?? Path.Combine(repositoryRoot, ".tmp", "ui-smoke", DateTime.Now.ToString("yyyyMMdd-HHmmss"))),
            UiLanguage = language,
            Only = only,
            Keep = keep,
            List = list,
            ShotsDirectory = shots is null ? null : Path.GetFullPath(shots),
        };
    }

    private const string Usage = """
        Usage: XmlEditorReloaded.UiSmoke [options]

          --app <path>    Application executable (default: out/bin/.../XmlEditorReloaded.App.exe)
          --data <path>   Folder that contains the JA 2 1.13 v4 / v5 reference data
          --work <path>   Fixture and report directory (default: .tmp/ui-smoke/<timestamp>)
          --lang <code>   UI language of the application, ru or en (default: ru)
          --only <ids>    Comma-separated scenario ids, for example S01,S06
          --keep          Keep the work directory after a successful run
          --list          Print the scenario catalog and exit
          --shots <path>  Capture reference screenshots into <path> and exit
        """;

    /// <summary>
    /// Walks up from the test binary to the repository root that contains the solution and reference
    /// projects.
    /// </summary>
    /// <returns>Absolute repository root.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no repository root can be found.</exception>
    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "xml-editor-reloaded"))
                && Directory.Exists(Path.Combine(directory.FullName, "References")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        var workingDirectory = Directory.GetCurrentDirectory();
        if (Directory.Exists(Path.Combine(workingDirectory, "xml-editor-reloaded")))
        {
            return workingDirectory;
        }

        throw new InvalidOperationException("The repository root could not be located from " + AppContext.BaseDirectory);
    }

    /// <summary>
    /// Returns the standard output encoding used by the runner.
    /// </summary>
    public static Encoding ConsoleEncoding => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
}
