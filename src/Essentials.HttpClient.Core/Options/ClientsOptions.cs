using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Metrics.Options;

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
    /// Опции метрик
    /// </summary>
    public MetricsOptions? Metrics { get; set; }
}