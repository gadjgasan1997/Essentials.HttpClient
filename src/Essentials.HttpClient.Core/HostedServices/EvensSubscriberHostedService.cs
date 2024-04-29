using Microsoft.Extensions.Hosting;
using Essentials.HttpClient.Events;
using Essentials.Utils.Collections.Extensions;

namespace Essentials.HttpClient.HostedServices;

/// <summary>
/// Сервис для подписки на события
/// </summary>
internal class EvensSubscriberHostedService : IHostedService
{
    private readonly IEnumerable<BaseEvensSubscriber> _subscribers;
    
    public EvensSubscriberHostedService(IEnumerable<BaseEvensSubscriber> subscribers)
    {
        _subscribers = subscribers;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _subscribers.ForEach(subscriber => subscriber.Subscribe());

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}