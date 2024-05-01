using Microsoft.Extensions.Hosting;
using Essentials.Utils.Extensions;

namespace Essentials.HttpClient.HostedServices;

/// <summary>
/// Сервис для автоматической регистрации http клиентов
/// </summary>
internal class RegisterHttpClientsHostedService : IHostedService
{
    private readonly IEssentialsHttpClient _httpClient;
    
    public RegisterHttpClientsHostedService(IEssentialsHttpClient httpClient)
    {
        _httpClient = httpClient.CheckNotNull();
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        HttpClientsHolder.Push(_httpClient);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}