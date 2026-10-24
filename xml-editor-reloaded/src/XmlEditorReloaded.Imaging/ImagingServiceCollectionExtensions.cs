using Microsoft.Extensions.DependencyInjection;

namespace XmlEditorReloaded.Imaging;

/// <summary>
/// Dependency-injection registration for the imaging backend.
/// </summary>
public static class ImagingServiceCollectionExtensions
{
    /// <summary>
    /// Registers imaging services. The STI decoder and image catalog are added in milestone M7.
    /// </summary>
    /// <param name="services">Service collection to configure.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddXmlEditorImaging(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
