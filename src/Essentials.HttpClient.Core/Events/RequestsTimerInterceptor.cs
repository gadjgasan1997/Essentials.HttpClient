using System.Diagnostics;
using System.Diagnostics.Contracts;
using Essentials.HttpClient.RequestsInterception;
using Context = Essentials.HttpClient.HttpRequestContext;

namespace Essentials.HttpClient.Events;

/// <summary>
/// Интерсептор замера времени выполнения запроса
/// </summary>
public sealed class RequestsTimerInterceptor : IRequestInterceptor
{
    /// <inheritdoc cref="IRequestInterceptor.Intercept" />
    public async Task<HttpResponseMessage> Intercept(NextRequestDelegate next)
    {
        Contract.Assert(Context.Current is not null);
        
        var clock = new Stopwatch();
        clock.Start();

        try
        {
            var responseMessage = await next();
            
            clock.Stop();
            Context.Current.SetElapsedTime(clock.ElapsedMilliseconds);
            
            return responseMessage;
        }
        catch
        {
            clock.Stop();
            Context.Current.SetElapsedTime(clock.ElapsedMilliseconds);
            
            throw;
        }
    }
}