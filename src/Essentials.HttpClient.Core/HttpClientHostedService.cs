using Essentials.HttpClient.Clients;
using Essentials.HttpClient.Metrics;
using Microsoft.Extensions.Hosting;
using Essentials.Utils.Extensions;

namespace Essentials.HttpClient;

/// <summary>
/// Сервис для автоматического выполнения необходимых действий после старта
/// </summary>
internal class HttpClientHostedService : IHostedService
{
    private readonly IMetricsService _metricsService;
    private readonly IHttpClientFactory _httpClientFactory;
    
    public HttpClientHostedService(
        IMetricsService metricsService,
        IHttpClientFactory httpClientFactory)
    {
        _metricsService = metricsService.CheckNotNull();
        _httpClientFactory = httpClientFactory.CheckNotNull();
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var client = new EssentialsHttpClient(_metricsService, _httpClientFactory);
        HttpClientsHolder.Push(client);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}