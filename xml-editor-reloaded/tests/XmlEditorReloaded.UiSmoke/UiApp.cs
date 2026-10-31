using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke;

/// <summary>
/// A started application instance under test together with the isolated application-data directory it
/// uses.
/// </summary>
internal sealed class UiApp : IDisposable
{
    private static readonly JsonSerializerOptions SettingsJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    private readonly Process _process;

    private UiApp(Process process, AutomationElement window, string appDataDirectory)
    {
        _process = process;
        Window = window;
        AppDataDirectory = appDataDirectory;
    }

    /// <summary>
    /// Gets the main window of the application.
    /// </summary>
    public AutomationElement Window { get; private set; }

    /// <summary>
    /// Gets the absolute path of the isolated application-data directory.
    /// </summary>
    public string AppDataDirectory { get; }

    /// <summary>
    /// Gets the process identifier.
    /// </summary>
    public int ProcessId => _process.Id;

    /// <summary>
    /// Gets the native handle of the main window.
    /// </summary>
    public IntPtr WindowHandle => new(Window.Current.NativeWindowHandle);

    /// <summary>
    /// Starts the application with an isolated application-data directory.
    /// </summary>
    /// <param name="options">Runner options.</param>
    /// <param name="appDataDirectory">Directory that receives settings, layouts, the database, and the log.</param>
    /// <param name="settings">Settings to write before start, or <see langword="null"/> for a cold start.</param>
    /// <returns>The started application.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the application exits or shows no window.</exception>
    public static UiApp Start(
        SmokeOptions options,
        string appDataDirectory,
        SettingsModel? settings)
    {
        if (!File.Exists(options.ApplicationPath))
        {
            throw new InvalidOperationException(
                $"The application executable was not found at '{options.ApplicationPath}'. Build the solution first.");
        }

        Directory.CreateDirectory(appDataDirectory);
        var settingsPath = Path.Combine(appDataDirectory, "settings.json");
        if (settings is null)
        {
            if (File.Exists(settingsPath))
            {
                File.Delete(settingsPath);
            }
        }
        else
        {
            File.WriteAllText(settingsPath, JsonSerializer.Serialize(settings, SettingsJsonOptions));
        }

        var startInfo = new ProcessStartInfo(options.ApplicationPath)
        {
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(options.ApplicationPath)!,
        };
        startInfo.Environment[AppDataOverrideVariable] = appDataDirectory;

        var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("The application process could not be started.");
        process.EnableRaisingEvents = true;

        var deadline = DateTime.UtcNow + options.StartupTimeout;
        while (DateTime.UtcNow < deadline)
        {
            Thread.Sleep(200);
            if (process.HasExited)
            {
                throw new InvalidOperationException(
                    "The application exited during start-up with code "
                    + process.ExitCode
                    + "."
                    + Environment.NewLine
                    + ReadLogTail(appDataDirectory));
            }

            var condition = new AndCondition(
                new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
            var window = AutomationElement.RootElement.FindFirst(TreeScope.Children, condition);
            if (window is not null)
            {
                Thread.Sleep(250);
                if (process.HasExited)
                {
                    throw new InvalidOperationException(
                        "The application exited during start-up."
                        + Environment.NewLine
                        + ReadLogTail(appDataDirectory));
                }

                var started = new UiApp(process, window, appDataDirectory);
                started.Activate();
                // Synthetic clicks must reach the application even when another window is raised during
                // the run; the topmost flag is removed with the window when the application closes.
                NativeInput.KeepOnTop(started.WindowHandle);
                return started;
            }
        }

        TryKill(process);
        throw new InvalidOperationException(
            $"The main window did not appear within {options.StartupTimeout.TotalSeconds:N0} s."
            + Environment.NewLine
            + ReadLogTail(appDataDirectory));
    }

    /// <summary>
    /// Activates the main window so synthetic input reaches it.
    /// </summary>
    public void Activate() => NativeInput.Activate(WindowHandle);

    /// <summary>
    /// Re-reads the main window element, for example after a dialog was opened and closed.
    /// </summary>
    /// <returns>The current main window element.</returns>
    public AutomationElement RefreshWindow()
    {
        var condition = new AndCondition(
            new PropertyCondition(AutomationElement.ProcessIdProperty, _process.Id),
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
        var windows = AutomationElement.RootElement
            .FindAll(TreeScope.Children, condition)
            .Cast<AutomationElement>()
            .ToList();
        // Combo-box popups and dialogs are top-level windows too; the main window is the one that
        // contains the shell status line.
        var main = windows.FirstOrDefault(window =>
            window.FindFirst(TreeScope.Descendants, Ui.ById("StatusText")) is not null);
        var window = main ?? windows.FirstOrDefault();
        if (window is not null)
        {
            Window = window;
        }

        return Window;
    }

    /// <summary>
    /// Gets a value indicating whether the application process has exited.
    /// </summary>
    public bool HasExited => _process.HasExited;

    /// <summary>
    /// Requests a normal window close without waiting for the result. The close may show unsaved-change
    /// dialogs, which block the UI thread, so the request runs in the background.
    /// </summary>
    public void RequestClose()
    {
        var window = Window;
        _ = Task.Run(() =>
        {
            try
            {
                if (window.TryGetCurrentPattern(WindowPattern.Pattern, out var pattern)
                    && pattern is WindowPattern windowPattern)
                {
                    windowPattern.Close();
                    return;
                }
            }
            catch (Exception exception) when (
                exception is ElementNotAvailableException
                    or InvalidOperationException
                    or System.Runtime.InteropServices.COMException)
            {
                // The window already closed; the process exit is checked by the scenario.
            }

            try
            {
                _process.CloseMainWindow();
            }
            catch (InvalidOperationException)
            {
                // The process already exited.
            }
        });
        Thread.Sleep(400);
    }

    /// <summary>
    /// Waits until the application process exits.
    /// </summary>
    /// <param name="timeout">How long to wait.</param>
    /// <returns><see langword="true"/> when the process exited in time.</returns>
    public bool WaitForExit(TimeSpan timeout)
    {
        if (_process.HasExited)
        {
            return true;
        }

        return _process.WaitForExit((int)timeout.TotalMilliseconds);
    }

    /// <summary>
    /// Closes the application through the window-pattern close command and waits for the process to
    /// exit.
    /// </summary>
    /// <param name="timeout">How long to wait for a clean exit.</param>
    /// <returns><see langword="true"/> when the process exited; <see langword="false"/> when it had to be killed.</returns>
    public bool Close(TimeSpan? timeout = null)
    {
        if (_process.HasExited)
        {
            return true;
        }

        try
        {
            Activate();
            if (Window.TryGetCurrentPattern(WindowPattern.Pattern, out var pattern)
                && pattern is WindowPattern windowPattern)
            {
                windowPattern.Close();
            }
            else
            {
                _process.CloseMainWindow();
            }
        }
        catch (ElementNotAvailableException)
        {
            // The window already disappeared.
        }

        if (_process.WaitForExit((int)(timeout ?? TimeSpan.FromSeconds(20)).TotalMilliseconds))
        {
            return true;
        }

        TryKill(_process);
        return false;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_process.HasExited)
        {
            TryKill(_process);
        }

        _process.Dispose();
    }

    /// <summary>
    /// Returns the tail of the application log for failure messages.
    /// </summary>
    /// <param name="appDataDirectory">Isolated application-data directory.</param>
    /// <returns>Last log lines, or an empty string.</returns>
    public static string ReadLogTail(string appDataDirectory)
    {
        var logPath = Path.Combine(appDataDirectory, "XmlEditorReloaded.log");
        if (!File.Exists(logPath))
        {
            return string.Empty;
        }

        try
        {
            var lines = File.ReadAllLines(logPath);
            return string.Join(Environment.NewLine, lines.TakeLast(15));
        }
        catch (IOException)
        {
            return string.Empty;
        }
    }

    private static void TryKill(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                process.WaitForExit(10_000);
            }
        }
        catch (InvalidOperationException)
        {
            // The process already exited.
        }
    }

    private const string AppDataOverrideVariable = "XER_APP_DATA";
}

/// <summary>
/// Shape of <c>settings.json</c> written before the application starts.
/// </summary>
internal sealed class SettingsModel
{
    /// <summary>
    /// Gets or sets the configured data directories.
    /// </summary>
    public List<string> DataDirectories { get; set; } = [];

    /// <summary>
    /// Gets or sets the active data directory.
    /// </summary>
    public string? ActiveDataDirectory { get; set; }

    /// <summary>
    /// Gets or sets the language code used for overlay editing.
    /// </summary>
    public string? ActiveLanguageCode { get; set; }

    /// <summary>
    /// Gets or sets the UI language code.
    /// </summary>
    public string UiLanguageCode { get; set; } = "ru";

    /// <summary>
    /// Gets or sets the recent data directories.
    /// </summary>
    public List<string> RecentDataDirectories { get; set; } = [];

    /// <summary>
    /// Gets or sets the saved window placement.
    /// </summary>
    public WindowPlacementModel? WindowPlacement { get; set; }
}

/// <summary>
/// Shape of the saved window placement in <c>settings.json</c>.
/// </summary>
internal sealed class WindowPlacementModel
{
    /// <summary>
    /// Gets or sets the left edge.
    /// </summary>
    public double Left { get; set; }

    /// <summary>
    /// Gets or sets the top edge.
    /// </summary>
    public double Top { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the window was maximized.
    /// </summary>
    public bool IsMaximized { get; set; }
}

