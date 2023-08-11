using System.Diagnostics.CodeAnalysis;

namespace Essentials.HttpClient.Metrics.Options;

/// <summary>
/// Опции метрик
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
internal class MetricsOptions
{
    /// <summary>
    /// Признак необходимости отдавать метрики
    /// </summary>
    public bool UseMetrics { get; set; }

    /// <summary>
    /// Признак необходимости отдавать метрики суммарно по всем типам запросов
    /// </summary>
    public bool UseAllRequestsMetrics { get; set; } = true;

    /// <summary>
    /// Признак необходимости отдавать метрики для Get запросов
    /// </summary>
    public bool UseGetRequestsMetrics { get; set; } = true;

    /// <summary>
    /// Признак необходимости отдавать метрики для Post запросов
    /// </summary>
    public bool UsePostRequestsMetrics { get; set; } = true;

    /// <summary>
    /// Мапа названий клиентов на признак игнорировать отдачу метрик для них
    /// </summary>
    public Dictionary<string, bool> ClientsMetricsIgnoreMap { get; set; } = new();
}