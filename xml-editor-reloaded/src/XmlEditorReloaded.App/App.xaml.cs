using System.Globalization;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Data;
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
        services.AddSingleton<ImageViewerViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
    }

    /// <inheritdoc />
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var appDataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "XmlEditorReloaded");
            var databasePath = Path.Combine(appDataDirectory, "metadata.sqlite");

            var services = new ServiceCollection();
            ConfigureServices(services, databasePath);
            _serviceProvider = services.BuildServiceProvider();

            var migration = await _serviceProvider
                .GetRequiredService<DatabaseMigrator>()
                .MigrateAsync(databasePath);
            if (!migration.IsSuccess)
            {
                MessageBox.Show(
                    Strings.Format("StatusError", migration.ErrorText ?? string.Empty),
                    Strings.Get("AppTitle"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
            window.Show();
            await _serviceProvider.GetRequiredService<MainViewModel>().InitializeAsync();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                Strings.Format("StatusError", exception.Message),
                Strings.Get("AppTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }

    /// <inheritdoc />
    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
