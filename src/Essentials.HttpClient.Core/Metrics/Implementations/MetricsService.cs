using LanguageExt;
using App.Metrics;
using App.Metrics.Timer;
using App.Metrics.Counter;
using Essentials.Utils.Extensions;
using System.Collections.Concurrent;
using static Essentials.HttpClient.Metrics.Dictionaries.KnownMetricsTags;

namespace Essentials.HttpClient.Metrics.Implementations;

/// <inheritdoc cref="IMetricsService" />
internal class MetricsService : IMetricsService
{
    private readonly IMetrics _metrics;
    private static readonly ConcurrentDictionary<(string, string?), MetricTags> _defaultTags = new();
    private static readonly ConcurrentDictionary<string, CounterOptions> _counters = new();
    private static readonly ConcurrentDictionary<string, TimerOptions> _timers = new();

    public MetricsService(IMetrics metrics)
    {
        _metrics = metrics.CheckNotNull();
    }

    /// <inheritdoc cref="IMetricsService.StartRequestTimer(string, Option{string})" />
    public IDisposable? StartRequestTimer(string clientName, Option<string> requestTypeId)
    {
        if (string.IsNullOrWhiteSpace(clientName))
            return null;
        
        var timer = GetTimer(clientName);
        var defaultTag = GetMetricDefaultTag(clientName, requestTypeId);
        return _metrics.Measure.Timer.Time(timer, defaultTag);
    }

    /// <inheritdoc cref="IMetricsService.HttpRequestSent(string, Option{string})" />
    public void HttpRequestSent(string clientName, Option<string> requestTypeId)
    {
        if (string.IsNullOrWhiteSpace(clientName))
            return;
        
        IncrementDefaultCounter($"{clientName} Request Counter", clientName, requestTypeId);
    }

    /// <inheritdoc cref="IMetricsService.HttpRequestSuccessSent(string, Option{string})" />
    public void HttpRequestSuccessSent(string clientName, Option<string> requestTypeId)
    {
        if (string.IsNullOrWhiteSpace(clientName))
            return;

        IncrementDefaultCounter($"{clientName} Valid Request Counter", clientName, requestTypeId);
    }

    /// <inheritdoc cref="IMetricsService.HttpRequestErrorSent(string, Option{string})" />
    public void HttpRequestErrorSent(string clientName, Option<string> requestTypeId)
    {
        if (string.IsNullOrWhiteSpace(clientName))
            return;

        IncrementDefaultCounter($"{clientName} Invalid Request Counter", clientName, requestTypeId);
    }

    /// <inheritdoc cref="IMetricsService.StartRequestTimer(string, MetricTags)" />
    public IDisposable? StartRequestTimer(string metricName, MetricTags tags)
    {
        if (string.IsNullOrWhiteSpace(metricName))
            return null;

        var timer = GetTimer(metricName);
        return _metrics.Measure.Timer.Time(timer, tags);
    }

    /// <inheritdoc cref="IMetricsService.HttpRequestSent(string, MetricTags)" />
    public void HttpRequestSent(string metricName, MetricTags tags)
    {
        if (string.IsNullOrWhiteSpace(metricName))
            return;

        IncrementCounter($"{metricName} Request Counter", ref tags);
    }

    /// <inheritdoc cref="IMetricsService.HttpRequestSuccessSent(string, MetricTags)" />
    public void HttpRequestSuccessSent(string metricName, MetricTags tags)
    {
        if (string.IsNullOrWhiteSpace(metricName))
            return;

        IncrementCounter($"{metricName} Valid Request Counter", ref tags);
    }

    /// <inheritdoc cref="IMetricsService.HttpRequestErrorSent(string, MetricTags)" />
    public void HttpRequestErrorSent(string metricName, MetricTags tags)
    {
        if (string.IsNullOrWhiteSpace(metricName))
            return;

        IncrementCounter($"{metricName} Invalid Request Counter", ref tags);
    }

    /// <summary>
    /// Инкрементирует дефолтовый счетчик
    /// </summary>
    /// <param name="metricName">Название метрики для счетчика</param>
    /// <param name="clientName">Название Http клиента</param>
    /// <param name="requestTypeId">Id типа запроса</param>
    private void IncrementDefaultCounter(
        string metricName,
        string clientName,
        Option<string> requestTypeId)
    {
        var defaultTag = GetMetricDefaultTag(clientName, requestTypeId);
        IncrementCounter(metricName, ref defaultTag);
    }

    /// <summary>
    /// Инкрементирует счетчик
    /// </summary>
    /// <param name="metricName">Название метрики для счетчика</param>
    /// <param name="tags">Теги</param>
    private void IncrementCounter(string metricName, ref MetricTags tags)
    {
        var counter = GetCounter(metricName);
        _metrics.Measure.Counter.Increment(counter, tags);
    }

    /// <summary>
    /// Возвращает тег метрик по-умолчанию
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <param name="requestTypeId">Id типа запроса</param>
    /// <returns>Тег</returns>
    private static MetricTags GetMetricDefaultTag(string clientName, Option<string> requestTypeId)
    {
        return requestTypeId.Match(
            Some: typeId =>
            {
                return _defaultTags.GetOrAdd(
                    (clientName, typeId),
                    valueFactory: _ => new MetricTags(
                        keys: new[] { CLIENT_NAME, REQUEST_TYPE_ID },
                        values: new[] { clientName, typeId }));

            },
            None: () =>
            {
                return _defaultTags.GetOrAdd(
                    (clientName, string.Empty),
                    valueFactory: _ => new MetricTags(CLIENT_NAME, clientName));
            });
    }

    /// <summary>
    /// Возвращает счетчик
    /// </summary>
    /// <param name="metricName">Название метрики</param>
    /// <returns>Счетчик</returns>
    private static CounterOptions GetCounter(string metricName) =>
        _counters.GetOrAdd(metricName, _ => new CounterOptions { Name = metricName });

    /// <summary>
    /// Возвращает таймер
    /// </summary>
    /// <param name="metricName">Название метрики</param>
    /// <returns>Таймер</returns>
    private static TimerOptions GetTimer(string metricName) =>
        _timers.GetOrAdd(metricName, _ => new TimerOptions
        {
            Name = $"{metricName} Requests Timer",
            MeasurementUnit = App.Metrics.Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        });
}