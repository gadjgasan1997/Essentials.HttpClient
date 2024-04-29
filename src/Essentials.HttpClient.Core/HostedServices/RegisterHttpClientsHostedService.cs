using Essentials.HttpClient.Clients;
using Microsoft.Extensions.Hosting;
using Essentials.Utils.Extensions;

namespace Essentials.HttpClient.HostedServices;

/// <summary>
/// Сервис для автоматической регистрации http клиентов
/// </summary>
internal class RegisterHttpClientsHostedService : IHostedService
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public RegisterHttpClientsHostedService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory.CheckNotNull();
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var client = new EssentialsHttpClient(_httpClientFactory);
        HttpClientsHolder.Push(client);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}