using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Cache.Options;
using Essentials.HttpClient.Metrics.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Essentials.HttpClient.Options;

/// <summary>
/// Опции Http клиентов
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
internal class ClientsOptions
{
    /// <summary>
    /// Название секции в конфигурации
    /// </summary>
    public static string Section => "EssentialsHttpClients";
    
    /// <summary>
    /// Время жизни сервиса при регистрации
    /// </summary>
    public ServiceLifetime? ServiceLifetime { get; set; }
    
    /// <summary>
    /// Опции метрик
    /// </summary>
    public MetricsOptions? Metrics { get; set; }
    
    /// <summary>
    /// Опции кеша
    /// </summary>
    public CacheOptions? Cache { get; set; }
}