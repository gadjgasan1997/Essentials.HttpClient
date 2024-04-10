using App.Metrics;
using LanguageExt;

namespace Essentials.HttpClient.Metrics.Implementations;

/// <inheritdoc cref="IMetricsService" />
internal class MockMetricsService : IMetricsService
{
    /// <inheritdoc cref="IMetricsService.StartRequestTimer(string, Option{string})" />
    public IDisposable? StartRequestTimer(string clientName, Option<string> requestTypeId) => null;

    /// <inheritdoc cref="IMetricsService.HttpRequestSent(string, Option{string})" />
    public void HttpRequestSent(string clientName, Option<string> requestTypeId) { }

    /// <inheritdoc cref="IMetricsService.HttpRequestSent(string, Option{string})" />
    public void HttpRequestSuccessSent(string clientName, Option<string> requestTypeId) { }

    /// <inheritdoc cref="IMetricsService.HttpRequestSent(string, Option{string})" />
    public void HttpRequestErrorSent(string clientName, Option<string> requestTypeId) { }

    /// <inheritdoc cref="IMetricsService.StartRequestTimer(string, MetricTags)" />
    public IDisposable? StartRequestTimer(string metricName, MetricTags tags) => null;

    /// <inheritdoc cref="IMetricsService.HttpRequestSent(string, MetricTags)" />
    public void HttpRequestSent(string metricName, MetricTags tags) { }

    /// <inheritdoc cref="IMetricsService.HttpRequestSent(string, MetricTags)" />
    public void HttpRequestSuccessSent(string metricName, MetricTags tags) { }

    /// <inheritdoc cref="IMetricsService.HttpRequestSent(string, MetricTags)" />
    public void HttpRequestErrorSent(string metricName, MetricTags tags) { }
}