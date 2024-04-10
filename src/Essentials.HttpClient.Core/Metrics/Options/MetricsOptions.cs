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
    /// Мапа названий метрик на признак игнорировать их отдачу
    /// </summary>
    public Dictionary<string, bool> MetricsIgnoreMap { get; set; } = new();

    /// <summary>
    /// Возвращает признак, надо ли собирать метрику
    /// </summary>
    /// <param name="metricName">Название метрики</param>
    /// <returns></returns>
    public bool NeedMetrics(string metricName) =>
        !MetricsIgnoreMap.TryGetValue(metricName, out var ignoreMetrics) || !ignoreMetrics;
}