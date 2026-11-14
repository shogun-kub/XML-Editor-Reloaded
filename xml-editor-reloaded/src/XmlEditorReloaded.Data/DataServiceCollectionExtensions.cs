using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Layout;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Help;
using XmlEditorReloaded.Data.Localization;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Data.Settings;

namespace XmlEditorReloaded.Data;

/// <summary>
/// Dependency-injection registration for the SQLite metadata and catalog backend.
/// </summary>
public static class DataServiceCollectionExtensions
{
    /// <summary>
    /// Registers the metadata provider, table catalog, translation catalog, and settings store.
    /// </summary>
    /// <param name="services">Service collection to configure.</param>
    /// <param name="databasePath">Absolute path of the migrated metadata SQLite database.</param>
    /// <param name="tableDataDirectory">
    /// Optional absolute path of the active <c>TableData</c> directory. When supplied, a translation catalog
    /// bound to that directory is registered.
    /// </param>
    /// <returns>The same service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddXmlEditorData(
        this IServiceCollection services,
        string databasePath,
        string? tableDataDirectory = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        services.TryAddSingleton<DatabaseMigrator>();
        services.TryAddSingleton<ITableMetadataProvider>(_ => new SqliteMetadataProvider(databasePath));
        services.TryAddSingleton<ITableCatalog>(provider => new FileSystemTableCatalog(
            metadataProvider: provider.GetRequiredService<ITableMetadataProvider>()));
        if (!string.IsNullOrWhiteSpace(tableDataDirectory))
        {
            services.TryAddSingleton<ITranslationCatalog>(provider => new TranslationCatalog(
                provider.GetRequiredService<ITableMetadataProvider>(),
                provider.GetRequiredService<ITableCatalog>(),
                tableDataDirectory));
        }

        services.TryAddSingleton<ISettingsStore>(_ => new JsonSettingsStore());
        services.TryAddSingleton<ITableLayoutStore>(_ => new JsonLayoutStore());
        services.TryAddSingleton<ITableHelpProvider>(_ =>
            new JsonTableHelpProvider(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName));

        return services;
    }
}
