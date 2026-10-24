using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Xml.Copy;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Lookups;
using XmlEditorReloaded.Xml.Repository;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml;

/// <summary>
/// Dependency-injection registration for the XML backend.
/// </summary>
public static class XmlServiceCollectionExtensions
{
    /// <summary>
    /// Registers XML loading, saving, repository, and optional lookup services.
    /// </summary>
    /// <param name="services">Service collection to configure.</param>
    /// <param name="tableDataDirectory">
    /// Optional absolute <c>TableData</c> directory used to register <see cref="ILookupRepository"/>.
    /// </param>
    /// <returns>The same service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddXmlEditorXml(this IServiceCollection services, string? tableDataDirectory = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<TableFileLoader>();
        services.TryAddSingleton<DocumentSerializer>();
        services.TryAddSingleton<TableFileSaver>();
        services.TryAddSingleton<ITableRepository>(provider => new TableRepository(
            provider.GetService<ITableMetadataProvider>(),
            provider.GetRequiredService<TableFileLoader>(),
            provider.GetRequiredService<TableFileSaver>()));
        services.TryAddSingleton<ITableCopyService>(provider => new TableCopyService(
            provider.GetRequiredService<ITableCatalog>(),
            provider.GetRequiredService<ITableMetadataProvider>()));

        if (!string.IsNullOrWhiteSpace(tableDataDirectory))
        {
            services.TryAddSingleton<ILookupRepository>(provider => new XmlFileLookupRepository(
                tableDataDirectory,
                metadataProvider: provider.GetService<ITableMetadataProvider>()));
        }

        return services;
    }
}
