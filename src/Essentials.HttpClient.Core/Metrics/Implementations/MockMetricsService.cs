namespace Essentials.HttpClient.Metrics.Implementations;

/// <inheritdoc cref="IMetricsService" />
internal class MockMetricsService : IMetricsService
{
    /// <inheritdoc cref="IMetricsService.StartRequestTimer" />
    public IDisposable? StartRequestTimer(string clientName) => null!;

    /// <inheritdoc cref="IMetricsService.HttpRequestSent" />
    public void HttpRequestSent(string clientName, HttpMethod httpMethod) { }

    /// <inheritdoc cref="IMetricsService.HttpRequestSuccessSent" />
    public void HttpRequestSuccessSent(string clientName, HttpMethod httpMethod) { }

    /// <inheritdoc cref="IMetricsService.HttpRequestErrorSent" />
    public void HttpRequestErrorSent(string clientName, HttpMethod httpMethod) { }
}