using Microsoft.Extensions.DependencyInjection;

namespace XmlEditorReloaded.Core;

/// <summary>
/// Dependency-injection registration for the Core contracts.
/// </summary>
public static class CoreServiceCollectionExtensions
{
    /// <summary>
    /// Registers Core services. Core currently exposes contracts and value types only,
    /// so this method intentionally adds no implementations.
    /// </summary>
    /// <param name="services">Service collection to configure.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddXmlEditorCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
