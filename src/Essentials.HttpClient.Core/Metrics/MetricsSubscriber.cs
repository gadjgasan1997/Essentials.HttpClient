using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using Essentials.Utils.Extensions;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Extensions;
using static Essentials.HttpClient.Events.EventsPublisher;
using Context = Essentials.HttpClient.HttpRequestContext;

// ReSharper disable MemberCanBeMadeStatic.Global
#pragma warning disable CA1822

namespace Essentials.HttpClient.Metrics;

/// <summary>
/// Класс-подписчик на события для сбора метрик
/// </summary>
internal class MetricsSubscriber : BaseEvensSubscriber
{
    private readonly IMetricsService _metricsService;
    private static readonly ConcurrentDictionary<string, IDisposable> _timers = new();
    
    public MetricsSubscriber(IMetricsService metricsService)
    {
        _metricsService = metricsService.CheckNotNull();
    }
    
    public override void Subscribe()
    {
        OnBeforeSend += BeforeSendHandler;
        OnSuccessSend += SuccessSendHandler;
        OnErrorSend += ErrorSendHandler;
        OnBadStatusCode += BadStatusCodeHandler;
    }

    private void BeforeSendHandler()
    {
        TryHandle(() =>
        {
            Contract.Assert(Context.Current is not null);
            
            var request = Context.Current.Request;
            
            request.IncrementMetric(
                Some: options => _metricsService.HttpRequestSent(options.Name, options.Tags),
                None: () => _metricsService.HttpRequestSent(request.ClientName, request.TypeId));
            
            var timer = request.MetricsOptions.MatchUnsafe(
                Some: options => _metricsService.StartRequestTimer(options.Name, options.Tags),
                None: () => _metricsService.StartRequestTimer(request.ClientName, request.TypeId));

            if (timer is not null)
            {
                _timers.AddOrUpdate(
                    request.Id,
                    addValueFactory: _ => timer,
                    updateValueFactory: (_, disposable) => disposable);
            }

        }, nameof(OnBeforeSend));
    }

    private void SuccessSendHandler()
    {
        TryHandle(() =>
        {
            Contract.Assert(Context.Current is not null);
            
            var request = Context.Current.Request;
            
            request.IncrementMetric(
                Some: options => _metricsService.HttpRequestSuccessSent(options.Name, options.Tags),
                None: () => _metricsService.HttpRequestSuccessSent(request.ClientName, request.TypeId));

            if (_timers.TryRemove(request.Id, out var timer))
                timer.Dispose();
            
        }, nameof(OnSuccessSend));
    }

    private void ErrorSendHandler()
    {
        TryHandle(() =>
        {
            Contract.Assert(Context.Current is not null);
            
            var request = Context.Current.Request;

            request.IncrementMetric(
                Some: options => _metricsService.HttpRequestErrorSent(options.Name, options.Tags),
                None: () => _metricsService.HttpRequestErrorSent(request.ClientName, request.TypeId));

            if (_timers.TryRemove(request.Id, out var timer))
                timer.Dispose();

        }, nameof(OnErrorSend));
    }

    private void BadStatusCodeHandler()
    {
        TryHandle(() =>
        {
            Contract.Assert(Context.Current is not null);
            
            var request = Context.Current.Request;

            request.IncrementMetric(
                Some: options => _metricsService.HttpRequestErrorSent(options.Name, options.Tags),
                None: () => _metricsService.HttpRequestErrorSent(request.ClientName, request.TypeId));

            if (_timers.TryRemove(request.Id, out var timer))
                timer.Dispose();

        }, nameof(OnBadStatusCode));
    }
}