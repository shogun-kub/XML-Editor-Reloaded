using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XmlEditorReloaded.Imaging.Cache;
using XmlEditorReloaded.Imaging.Catalog;
using XmlEditorReloaded.Imaging.Etrle;
using XmlEditorReloaded.Imaging.Sti;

namespace XmlEditorReloaded.Imaging;

/// <summary>
/// Dependency-injection registration for the imaging backend.
/// </summary>
public static class ImagingServiceCollectionExtensions
{
    /// <summary>
    /// Registers the STI parser, ETRLE decoder, item image catalog, and decoded-frame cache.
    /// </summary>
    /// <param name="services">Service collection to configure.</param>
    /// <param name="dataDirectory">Absolute path of the <c>Data-1.13</c> directory.</param>
    /// <param name="cacheCapacityBytes">Decoded-frame cache limit in bytes.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="dataDirectory"/> is empty.</exception>
    public static IServiceCollection AddXmlEditorImaging(
        this IServiceCollection services,
        string dataDirectory,
        long cacheCapacityBytes = 128L * 1024 * 1024)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cacheCapacityBytes);

        services.TryAddSingleton<StciParser>();
        services.TryAddSingleton<EtrleDecoder>();
        services.TryAddSingleton<StiDecoder>();
        services.TryAddSingleton<DecodedFrameCache>(_ => new DecodedFrameCache(cacheCapacityBytes));
        services.TryAddSingleton<ItemImageCatalog>(provider => new ItemImageCatalog(
            dataDirectory,
            provider.GetRequiredService<StiDecoder>(),
            provider.GetRequiredService<DecodedFrameCache>()));

        return services;
    }
}
