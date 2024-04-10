using Essentials.HttpClient.Cache.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Essentials.HttpClient.Cache.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Настраивает кеширование
    /// </summary>
    /// <param name="services"></param>
    /// <param name="cacheOptions">Опции кеша</param>
    public static IServiceCollection ConfigureCache(
        this IServiceCollection services,
        CacheOptions? cacheOptions)
    {
        if (cacheOptions is null)
            return services;

        if (cacheOptions.Requests is null)
            return services;

        RequestsCacheService.IgnoredIdList = cacheOptions.Requests.IgnoredIdList;
        return services;
    }
}