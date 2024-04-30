using Microsoft.Extensions.Hosting;
using Essentials.Utils.Extensions;
using Essentials.HttpClient.Clients;
using Essentials.HttpClient.RequestsInterception;

namespace Essentials.HttpClient.HostedServices;

/// <summary>
/// Сервис для автоматической регистрации http клиентов
/// </summary>
internal class RegisterHttpClientsHostedService : IHostedService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEnumerable<IRequestInterceptor> _interceptors;
    
    public RegisterHttpClientsHostedService(IHttpClientFactory factory, IEnumerable<IRequestInterceptor> interceptors)
    {
        _httpClientFactory = factory.CheckNotNull();
        _interceptors = interceptors;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var client = new EssentialsHttpClient(_httpClientFactory, _interceptors);
        HttpClientsHolder.Push(client);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}