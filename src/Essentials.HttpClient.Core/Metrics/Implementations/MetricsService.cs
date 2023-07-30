using App.Metrics;
using App.Metrics.Counter;
using Essentials.HttpClient.Metrics.Dictionaries;
using Microsoft.Extensions.Options;
using static Essentials.HttpClient.Metrics.MetricsRegistry;
using HttpRequestsMetricsOptions = Essentials.HttpClient.Metrics.Options.MetricsOptions;

namespace Essentials.HttpClient.Metrics.Implementations;

/// <inheritdoc cref="IMetricsService" />
internal class MetricsService : IMetricsService
{
    private readonly IMetrics _metrics;
    private readonly HttpRequestsMetricsOptions _options;

    public MetricsService(
        IMetrics metrics,
        IOptions<HttpRequestsMetricsOptions> options)
    {
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc cref="IMetricsService.StartRequestTimer" />
    public IDisposable? StartRequestTimer(string clientName)
    {
        if (_options.ClientsMetricsIgnoreMap.TryGetValue(clientName, out var ignoreMetrics) && ignoreMetrics)
            return null;

        return _metrics.Measure.Timer.Time(HttpRequestsTimer);
    }

    /// <inheritdoc cref="IMetricsService.HttpRequestSent" />
    public void HttpRequestSent(string clientName, HttpMethod httpMethod)
    {
        if (_options.ClientsMetricsIgnoreMap.TryGetValue(clientName, out var ignoreMetrics) && ignoreMetrics)
            return;

        IncrementRequestMetric(HttpRequestCounter, clientName);
        IncrementGetRequestMetric(GetHttpRequestCounter, httpMethod, clientName);
        IncrementPostRequestMetric(PostHttpRequestCounter, httpMethod, clientName);
    }

    /// <inheritdoc cref="IMetricsService.HttpRequestSuccessSent" />
    public void HttpRequestSuccessSent(string clientName, HttpMethod httpMethod)
    {
        if (_options.ClientsMetricsIgnoreMap.TryGetValue(clientName, out var ignoreMetrics) && ignoreMetrics)
            return;

        IncrementRequestMetric(ValidHttpRequestCounter, clientName);
        IncrementGetRequestMetric(ValidGetHttpRequestCounter, httpMethod, clientName);
        IncrementPostRequestMetric(ValidPostHttpRequestCounter, httpMethod, clientName);
    }

    /// <inheritdoc cref="IMetricsService.HttpRequestErrorSent" />
    public void HttpRequestErrorSent(string clientName, HttpMethod httpMethod)
    {
        if (_options.ClientsMetricsIgnoreMap.TryGetValue(clientName, out var ignoreMetrics) && ignoreMetrics)
            return;

        IncrementRequestMetric(InvalidHttpRequestCounter, clientName);
        IncrementGetRequestMetric(InvalidGetHttpRequestCounter, httpMethod, clientName);
        IncrementPostRequestMetric(InvalidPostHttpRequestCounter, httpMethod, clientName);
    }

    /// <summary>
    /// Инкрементирует счетчик Http запроса
    /// </summary>
    /// <param name="counter">Счетчик</param>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns></returns>
    private void IncrementRequestMetric(CounterOptions counter, string clientName)
    {
        if (_options.UseAllRequestsMetrics)
            HttpRequestIncrement(counter, clientName);
    }

    /// <summary>
    /// Инкрементирует счетчик Get Http запроса
    /// </summary>
    /// <param name="counter">Счетчик</param>
    /// <param name="httpMethod">Http метод</param>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns></returns>
    private void IncrementGetRequestMetric(
        CounterOptions counter,
        HttpMethod httpMethod,
        string clientName)
    {
        if (httpMethod == HttpMethod.Get && _options.UseGetRequestsMetrics)
            HttpRequestIncrement(counter, clientName);
    }

    /// <summary>
    /// Инкрементирует счетчик Post Http запроса
    /// </summary>
    /// <param name="counter">Счетчик</param>
    /// <param name="httpMethod">Http метод</param>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns></returns>
    private void IncrementPostRequestMetric(
        CounterOptions counter,
        HttpMethod httpMethod,
        string clientName)
    {
        if (httpMethod == HttpMethod.Post && _options.UsePostRequestsMetrics)
            HttpRequestIncrement(counter, clientName);
    }

    /// <summary>
    /// Инкрементирует счетчик Http запроса
    /// </summary>
    /// <param name="counter">Счетчик</param>
    /// <param name="clientName">Название Http клиента</param>
    private void HttpRequestIncrement(CounterOptions counter, string clientName)
    {
        _metrics.Measure.Counter.Increment(
            counter,
            new MetricTags(KnownMetricsTags.CLIENT_NAME, clientName));
    }
}