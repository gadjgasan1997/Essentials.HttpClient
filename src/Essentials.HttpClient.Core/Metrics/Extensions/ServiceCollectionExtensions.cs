using Essentials.HttpClient.Metrics.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Essentials.HttpClient.Metrics.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Настраивает метрики
    /// </summary>
    /// <param name="services"></param>
    public static IServiceCollection ConfigureMetrics(this IServiceCollection services)
    {
        services.TryAddSingleton<IMetricsService, MetricsService>();
        return services;
    }
}