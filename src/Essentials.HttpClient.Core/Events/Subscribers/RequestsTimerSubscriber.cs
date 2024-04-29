using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Collections.Concurrent;
using static Essentials.HttpClient.Events.EventsPublisher;
using Context = Essentials.HttpClient.HttpRequestContext;

namespace Essentials.HttpClient.Events.Subscribers;

/// <summary>
/// Подписчик на события для замерки времени выполнения запросов
/// </summary>
internal class RequestsTimerSubscriber : BaseEvensSubscriber
{
    private static readonly ConcurrentDictionary<string, Stopwatch> _timers = new();

    public override void Subscribe()
    {
        OnBeforeSend += BeforeSendHandler;
        OnSuccessSend += SuccessSendHandler;
        OnErrorSend += ErrorSendHandler;
        OnBadStatusCode += BadStatusCodeHandler;
    }

    private static void BeforeSendHandler()
    {
        TryHandle(() =>
        {
            Contract.Assert(Context.Current is not null);
        
            var clock = new Stopwatch();
            clock.Start();
        
            _timers.AddOrUpdate(
                Context.Current.Request.Id,
                addValueFactory: _ => clock,
                updateValueFactory: (_, stopwatch) => stopwatch);
        }, nameof(OnBeforeSend));
    }

    private static void SuccessSendHandler()
    {
        TryHandle(() =>
        {
            Contract.Assert(Context.Current is not null);

            if (!_timers.TryRemove(Context.Current.Request.Id, out var clock))
                return;
        
            clock.Stop();
            Context.Current.SetElapsedTime(clock.ElapsedMilliseconds);
        }, nameof(OnSuccessSend));
    }

    private static void ErrorSendHandler()
    {
        TryHandle(() =>
        {
            Contract.Assert(Context.Current is not null);

            if (!_timers.TryRemove(Context.Current.Request.Id, out var clock))
                return;
        
            clock.Stop();
            Context.Current.SetElapsedTime(clock.ElapsedMilliseconds);
        }, nameof(OnErrorSend));
    }

    private static void BadStatusCodeHandler()
    {
        TryHandle(() =>
        {
            Contract.Assert(Context.Current is not null);

            if (!_timers.TryRemove(Context.Current.Request.Id, out var clock))
                return;
        
            clock.Stop();
            Context.Current.SetElapsedTime(clock.ElapsedMilliseconds);
        }, nameof(OnBadStatusCode));
    }
}