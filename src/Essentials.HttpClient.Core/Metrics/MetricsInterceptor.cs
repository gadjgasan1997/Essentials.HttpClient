using System.Diagnostics.Contracts;
using Essentials.Utils.Extensions;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.RequestsInterception;
using Context = Essentials.HttpClient.HttpRequestContext;

namespace Essentials.HttpClient.Metrics;

/// <summary>
/// Интерсептор сбора метрик
/// </summary>
public sealed class MetricsInterceptor : IRequestInterceptor
{
    private readonly IMetricsService _metricsService;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="metricsService">Сервис для отдачи метрик</param>
    public MetricsInterceptor(IMetricsService metricsService)
    {
        _metricsService = metricsService.CheckNotNull();
    }
    
    /// <inheritdoc cref="IRequestInterceptor.Intercept" />
    public async Task<HttpResponseMessage> Intercept(NextRequestDelegate next)
    {
        Contract.Assert(Context.Current is not null);
        
        var request = Context.Current.Request;
        
        request.IncrementMetric(
            Some: options => _metricsService.HttpRequestSent(options.Name, options.Tags),
            None: () => _metricsService.HttpRequestSent(request.ClientName, request.TypeId));
            
        using var timer = request.MetricsOptions.MatchUnsafe(
            Some: options => _metricsService.StartRequestTimer(options.Name, options.Tags),
            None: () => _metricsService.StartRequestTimer(request.ClientName, request.TypeId));

        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await next().ConfigureAwait(false);
        }
        catch
        {
            request.IncrementMetric(
                Some: options => _metricsService.HttpRequestErrorSent(options.Name, options.Tags),
                None: () => _metricsService.HttpRequestErrorSent(request.ClientName, request.TypeId));
            
            throw;
        }

        if (!responseMessage.IsSuccessStatusCode)
        {
            request.IncrementMetric(
                Some: options => _metricsService.HttpRequestErrorSent(options.Name, options.Tags),
                None: () => _metricsService.HttpRequestErrorSent(request.ClientName, request.TypeId));
        }
        else
        {
            request.IncrementMetric(
                Some: options => _metricsService.HttpRequestSuccessSent(options.Name, options.Tags),
                None: () => _metricsService.HttpRequestSuccessSent(request.ClientName, request.TypeId));
        }

        return responseMessage;
    }
}