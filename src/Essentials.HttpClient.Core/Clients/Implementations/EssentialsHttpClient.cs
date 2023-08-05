using System.Diagnostics.CodeAnalysis;
using Essentials.Func.Utils.Extensions;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.Models;
using Essentials.HttpClient.Models.Implementations;
using LanguageExt;
using LanguageExt.Common;
using SystemHttpClient = System.Net.Http.HttpClient;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient.Clients.Implementations;

/// <inheritdoc cref="IEssentialsHttpClient" />
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public class EssentialsHttpClient : IEssentialsHttpClient
{
    protected IMetricsService Metrics { get; }
    protected IHttpClientFactory Factory { get; }
    
    public EssentialsHttpClient(
        IMetricsService metricsService,
        IHttpClientFactory factory)
    {
        Metrics = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        Factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }
    
    /// <inheritdoc cref="IGetHttpClient.GetAsync" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> GetAsync(
        Validation<Error, IEssentialsHttpRequest> validation,
        CancellationToken? token = default)
    {
        return await (
                validation.Bind(Success<Error, IEssentialsHttpRequest>),
                validation.Bind(request => CreateClient(request.ClientName)))
            .Apply((request, client) => SendAsync(request, client, token))
            .DefaultBindAsync(task => task);
    }

    protected virtual async Task<Validation<Error, IEssentialsHttpResponse>> SendAsync(
        IEssentialsHttpRequest request,
        SystemHttpClient httpClient,
        CancellationToken? token = default)
    {
        return await SendWithMetricsAsync(request, () => SendAsyncCore(request, httpClient, token));
    }

    protected static async Task<Validation<Error, IEssentialsHttpResponse>> SendAsyncCore(
        IEssentialsHttpRequest request,
        SystemHttpClient httpClient,
        CancellationToken? token = default)
    {
        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = token is null
                ? await httpClient.SendAsync(request.RequestMessage)
                : await httpClient.SendAsync(request.RequestMessage, token.Value);
        }
        catch (TimeoutException ex)
        {
            return Error.New(ex);
        }
        catch (Exception ex)
        {
            return Error.New(ex);
        }

        if (!responseMessage.IsSuccessStatusCode)
            return Error.New("Ошибочный Http код ответа");

        return new EssentialsHttpResponse(responseMessage);
    }

    private async Task<Validation<Error, IEssentialsHttpResponse>> SendWithMetricsAsync(
        IEssentialsHttpRequest request,
        Func<Task<Validation<Error, IEssentialsHttpResponse>>> sendFunc)
    {
        using var timer = Metrics.StartRequestTimer(request.ClientName);
        Metrics.HttpRequestSent(request.ClientName, request.RequestMessage.Method);
        
        var result = await sendFunc();
        
        result.Match(
            Succ: _ => Metrics.HttpRequestSuccessSent(request.ClientName, request.RequestMessage.Method),
            Fail: _ => Metrics.HttpRequestErrorSent(request.ClientName, request.RequestMessage.Method));
        
        return result;
    }

    private Validation<Error, SystemHttpClient> CreateClient(string clientName)
    {
        return Try(() => Factory.CreateClient(clientName))
            .Try()
            .Match(
                Succ: client => client,
                Fail: exception => Fail<Error, SystemHttpClient>(exception));
    }
}