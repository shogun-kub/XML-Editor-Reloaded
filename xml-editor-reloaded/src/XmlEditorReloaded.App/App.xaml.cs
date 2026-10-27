using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Diagnostics;
using XmlEditorReloaded.Data;
using XmlEditorReloaded.Data.Diagnostics;
using XmlEditorReloaded.Data.Metadata;
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

        var appDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "XmlEditorReloaded");
        _logPath = Path.Combine(appDataDirectory, "XmlEditorReloaded.log");
        OperationLog.Sink = new FileOperationLog(_logPath);
        RegisterGlobalExceptionHandlers();
        OperationLog.Info(
            "App",
            $"Starting XML Editor Reloaded {typeof(App).Assembly.GetName().Version?.ToString() ?? "unknown"}.");

        try
        {
            var databasePath = Path.Combine(appDataDirectory, "metadata.sqlite");

            var services = new ServiceCollection();
            ConfigureServices(services, databasePath);
            _serviceProvider = services.BuildServiceProvider();

            var migration = await _serviceProvider
                .GetRequiredService<DatabaseMigrator>()
                .MigrateAsync(databasePath);
            if (!migration.IsSuccess)
            {
                OperationLog.Fatal(
                    "DatabaseMigrator",
                    migration.ErrorText ?? "The metadata database could not be migrated.",
                    migration.ErrorCode);
                ShowFatalError(migration.ErrorText ?? string.Empty);
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
                    CultureInfo.CurrentUICulture = new CultureInfo(uiLanguage);
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
            OperationLog.Fatal("App", "The application failed to start.", exception: exception);
            ShowFatalError(exception.Message);
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

    private void ShowFatalError(string message) =>
        MessageBox.Show(
            BuildErrorMessage(message),
            Strings.Get("AppTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);

    private string BuildErrorMessage(Exception exception) => BuildErrorMessage(exception.Message);

    private string BuildErrorMessage(string message)
    {
        var text = Strings.Format("StatusError", message);
        return string.IsNullOrWhiteSpace(_logPath)
            ? text
            : text + Environment.NewLine + Environment.NewLine + Strings.Format("LogFileHintFormat", _logPath);
    }
}
