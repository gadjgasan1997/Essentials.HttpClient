using Essentials.HttpClient.Metrics.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using HttpClientsMetricsOptions = Essentials.HttpClient.Metrics.Options.MetricsOptions;

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
    /// <param name="metricsOptions">Опции метрик</param>
    public static IServiceCollection ConfigureMetrics(
        this IServiceCollection services,
        HttpClientsMetricsOptions? metricsOptions)
    {
        if (metricsOptions is null)
            return services.ConfigureMockMetrics();

        if (!metricsOptions.UseMetrics)
            return services.ConfigureMockMetrics();

        services.TryAddSingleton<IMetricsService, MetricsService>();
        return services;
    }

    /// <summary>
    /// Настраивает мок для метрик
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private static IServiceCollection ConfigureMockMetrics(this IServiceCollection services)
    {
        services.TryAddSingleton<IMetricsService, MockMetricsService>();
        return services;
    }
}