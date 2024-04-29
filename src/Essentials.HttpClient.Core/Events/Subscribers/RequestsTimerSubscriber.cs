using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Collections.Concurrent;
using static Essentials.HttpClient.Events.EventsPublisher;
using Context = Essentials.HttpClient.HttpRequestContext;

namespace Essentials.HttpClient.Events.Subscribers;

/// <summary>
/// Подписчик на события для замерки времени выполнения запросов
/// </summary>
internal static class RequestsTimerSubscriber
{
    private static readonly ConcurrentDictionary<string, Stopwatch> _timers = new();

    public static void Subscribe()
    {
        OnBeforeSend += BeforeSendHandler;
        OnSuccessSend += SuccessSendHandler;
        OnErrorSend += ErrorSendHandler;
        OnBadStatusCode += BadStatusCodeHandler;
    }

    private static void BeforeSendHandler()
    {
        Contract.Assert(Context.Current is not null);
        
        var clock = new Stopwatch();
        clock.Start();
        
        _timers.AddOrUpdate(
            Context.Current.Request.Id,
            addValueFactory: _ => clock,
            updateValueFactory: (_, stopwatch) => stopwatch);
    }

    private static void SuccessSendHandler()
    {
        Contract.Assert(Context.Current is not null);

        if (!_timers.TryRemove(Context.Current.Request.Id, out var clock))
            return;
        
        clock.Stop();
        Context.Current.SetElapsedTime(clock.ElapsedMilliseconds);
    }

    private static void ErrorSendHandler()
    {
        Contract.Assert(Context.Current is not null);

        if (!_timers.TryRemove(Context.Current.Request.Id, out var clock))
            return;
        
        clock.Stop();
        Context.Current.SetElapsedTime(clock.ElapsedMilliseconds);
    }

    private static void BadStatusCodeHandler()
    {
        Contract.Assert(Context.Current is not null);

        if (!_timers.TryRemove(Context.Current.Request.Id, out var clock))
            return;
        
        clock.Stop();
        Context.Current.SetElapsedTime(clock.ElapsedMilliseconds);
    }
}