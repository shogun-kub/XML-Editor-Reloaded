using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Diagnostics;
using XmlEditorReloaded.Data;
using XmlEditorReloaded.Data.Diagnostics;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Data.Settings;
using XmlEditorReloaded.Xml;
using XmlEditorReloaded.Xml.Comparison;

namespace XmlEditorReloaded.App;

/// <summary>
/// Application entry point and dependency-injection composition root.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    private string? _logPath;
    private StartupTrace? _startupTrace;

    /// <summary>
    /// Registers the application services, view models, and shell window.
    /// </summary>
    /// <param name="services">Service collection to configure.</param>
    /// <param name="databasePath">Absolute path of the metadata SQLite database.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="databasePath"/> is empty.</exception>
    public static void ConfigureServices(IServiceCollection services, string databasePath)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        services.AddXmlEditorCore();
        services.AddXmlEditorData(databasePath);
        services.AddXmlEditorXml();
        services.AddSingleton<IDialogService, WpfDialogService>();
        services.AddSingleton<IDataSetSessionFactory, DataSetSessionFactory>();
        services.AddSingleton<ITableEditorFactory, TableEditorFactory>();
        services.AddSingleton<IItemImageService, WpfItemImageService>();
        services.AddSingleton<IImageViewerService, WpfImageViewerService>();
        services.AddSingleton<ITableComparisonService>(provider => new TableDataComparer(
            provider.GetRequiredService<ITableCatalog>(),
            provider.GetRequiredService<ITableMetadataProvider>()));
        services.AddSingleton<CompareViewModel>();
        services.AddSingleton<IVersionCompareService, WpfVersionCompareService>();
        services.AddSingleton<ITableCopyDialogService, WpfTableCopyDialogService>();
        services.AddSingleton<ISettingsDialogService, WpfSettingsDialogService>();
        services.AddSingleton<ImageViewerViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
    }

    /// <inheritdoc />
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Themes.ThemeManager.Apply(EditorThemeMode.System);
        EventManager.RegisterClassHandler(
            typeof(Window),
            FrameworkElement.LoadedEvent,
            new RoutedEventHandler(static (sender, _) =>
                Themes.ThemeManager.ApplyWindowChrome((Window)sender)));

        var appDataDirectory = AppPaths.ApplicationDataDirectory;
        _logPath = AppPaths.LogFilePath;
        var databasePath = Path.Combine(appDataDirectory, "metadata.sqlite");

        // The bootstrap trace is written before the operation log and the service container exist, so
        // a failure in this window is still visible when the application-data folder is not writable.
        _startupTrace = new StartupTrace();
        _startupTrace.Write(
            $"Starting {typeof(App).Assembly.GetName().Version?.ToString() ?? "unknown"}; "
            + $"user {Environment.UserDomainName}\\{Environment.UserName}; "
            + $"elevated {Environment.IsPrivilegedProcess}; "
            + $"application data '{appDataDirectory}'; log '{_logPath}'; base '{AppContext.BaseDirectory}'.");

        // Probe write access and record the environment before any service is created. The probe and
        // the metadata below only read attributes; no attribute or ACL is ever changed.
        var probe = StartupDiagnostics.ProbeWriteAccess(appDataDirectory);
        _startupTrace.Write($"Write probe: {probe.Message}");

        OperationLog.Sink = new FileOperationLog(_logPath);
        RegisterGlobalExceptionHandlers();
        OperationLog.Info(
            "App",
            $"Starting XML Editor Reloaded {typeof(App).Assembly.GetName().Version?.ToString() ?? "unknown"}.");
        OperationLog.Info(
            "App",
            StartupDiagnostics.DescribeEnvironment(
                appDataDirectory,
                _logPath,
                JsonSettingsStore.DefaultSettingsPath,
                JsonLayoutStore.DefaultLayoutsPath,
                databasePath));
        if (!probe.IsWritable)
        {
            OperationLog.Warning("App", probe.Message, ErrorCodes.IoAccessDenied);
        }

        try
        {
            var services = new ServiceCollection();
            ConfigureServices(services, databasePath);
            _serviceProvider = services.BuildServiceProvider();

            var migration = await _serviceProvider
                .GetRequiredService<DatabaseMigrator>()
                .MigrateAsync(databasePath);
            if (!migration.IsSuccess)
            {
                var migrationMessage = migration.ErrorText ?? "The metadata database could not be migrated.";
                _startupTrace?.Write(
                    $"Fatal: metadata migration failed ({migration.ErrorCode ?? "no code"}) "
                    + $"for '{databasePath}': {migrationMessage}");
                OperationLog.Fatal("DatabaseMigrator", migrationMessage, migration.ErrorCode);
                ShowFatalError(migration.ErrorCode, migrationMessage, databasePath);
                Shutdown();
                return;
            }

            var settingsStore = _serviceProvider.GetRequiredService<ISettingsStore>();
            var settings = await settingsStore.LoadAsync();
            var uiLanguage = settings.IsSuccess
                ? settings.Data!.UiLanguageCode
                : "en";
            if (!string.IsNullOrWhiteSpace(uiLanguage))
            {
                try
                {
                    var culture = new CultureInfo(uiLanguage);
                    CultureInfo.CurrentUICulture = culture;
                    // The interface language is an application setting, so the strings are resolved from
                    // it explicitly instead of the ambient culture of whichever thread creates a window.
                    Strings.SetUiCulture(culture);
                }
                catch (CultureNotFoundException)
                {
                    // Fall back to the current UI culture when settings contain an unknown culture.
                }
            }

            var window = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow = window;
            if (settings.IsSuccess)
            {
                window.ApplySavedPlacement(settings.Data?.WindowPlacement);
            }

            window.Show();
            await _serviceProvider.GetRequiredService<MainViewModel>().InitializeAsync();
        }
        catch (Exception exception)
        {
            _startupTrace?.Write(
                $"Fatal: {exception.GetType().FullName} at '{AppContext.BaseDirectory}': {exception.Message}");
            OperationLog.Fatal("App", "The application failed to start.", exception: exception);
            ShowFatalError(null, exception.Message, AppContext.BaseDirectory, exception);
            Shutdown();
        }
    }

    /// <inheritdoc />
    protected override void OnExit(ExitEventArgs e)
    {
        OperationLog.Info("App", "Application exiting.");
        _serviceProvider?.Dispose();
        OperationLog.Sink = null;
        base.OnExit(e);
    }

    private void RegisterGlobalExceptionHandlers()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    /// <summary>
    /// Logs an unhandled UI exception, tells the user where the log is, and keeps the application
    /// running so the user can still save or close the open tables deliberately.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        OperationLog.Fatal("Dispatcher", "Unhandled UI exception.", exception: e.Exception);
        MessageBox.Show(
            BuildErrorMessage(e.Exception),
            Strings.Get("AppTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }

    private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception
            ?? new InvalidOperationException(e.ExceptionObject?.ToString() ?? "Unknown error.");
        OperationLog.Fatal(
            "AppDomain",
            e.IsTerminating
                ? "Unhandled exception; the process is terminating."
                : "Unhandled exception.",
            exception: exception);
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        OperationLog.Error(
            "TaskScheduler",
            "An unobserved task exception was reported.",
            exception: e.Exception);
        e.SetObserved();
    }

    /// <summary>
    /// Shows a fatal start-up error with the error code, the affected path, and the exception type, so
    /// the user and the log identify the failure without elevation or guesswork.
    /// </summary>
    /// <param name="errorCode">Stable error code, or <see langword="null"/> when the failure has none.</param>
    /// <param name="message">Human-readable failure description.</param>
    /// <param name="location">File or directory involved in the failure, or <see langword="null"/>.</param>
    /// <param name="exception">Exception that caused the failure, or <see langword="null"/>.</param>
    private void ShowFatalError(
        string? errorCode,
        string message,
        string? location = null,
        Exception? exception = null)
    {
        var text = new StringBuilder();
        text.AppendLine(Strings.Get("StartupFailedHeader"));
        if (!string.IsNullOrWhiteSpace(errorCode))
        {
            text.AppendLine(Strings.Format("StartupFailureCodeFormat", errorCode));
        }

        text.AppendLine(message);
        if (!string.IsNullOrWhiteSpace(location))
        {
            text.AppendLine(Strings.Format("StartupFailureLocationFormat", location));
        }

        if (exception is not null)
        {
            text.AppendLine(Strings.Format(
                "StartupFailureTypeFormat",
                exception.GetType().FullName ?? exception.GetType().Name));
        }

        if (!string.IsNullOrWhiteSpace(_logPath))
        {
            text.AppendLine();
            text.AppendLine(Strings.Format("LogFileHintFormat", _logPath));
            var tracePath = _startupTrace?.FilePath;
            if (!string.IsNullOrWhiteSpace(tracePath))
            {
                text.Append(Strings.Format("StartupTraceHintFormat", tracePath));
            }
        }

        MessageBox.Show(
            text.ToString().TrimEnd(),
            Strings.Get("AppTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private string BuildErrorMessage(Exception exception) => BuildErrorMessage(exception.Message);

    private string BuildErrorMessage(string message)
    {
        var text = Strings.Format("StatusError", message);
        return string.IsNullOrWhiteSpace(_logPath)
            ? text
            : text + Environment.NewLine + Environment.NewLine + Strings.Format("LogFileHintFormat", _logPath);
    }
}
