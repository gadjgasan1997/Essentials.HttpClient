using System.Diagnostics.CodeAnalysis;
using System.Text;
using Essentials.Func.Utils.Extensions;
using Essentials.HttpClient.ContentTypes.Interfaces;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.Models;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Serialization.Extensions;
using LanguageExt;
using LanguageExt.Common;
using SystemHttpClient = System.Net.Http.HttpClient;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient.Clients;

[SuppressMessage("ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract")]
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
    
    /// <inheritdoc cref="IEssentialsHttpClient.GetAsync(Validation{Error, IEssentialsHttpRequest}, CancellationToken?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> GetAsync(
        Validation<Error, IEssentialsHttpRequest> validation,
        CancellationToken? token = default)
    {
        return await validation.DefaultBindAsync(request => GetAsync(request, token));
    }

    /// <inheritdoc cref="IEssentialsHttpClient.GetAsync(IEssentialsHttpRequest, CancellationToken?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> GetAsync(
        IEssentialsHttpRequest request,
        CancellationToken? token = default)
    {
        request.RequestMessage.Method = HttpMethod.Get;
        return await CreateClient(request).DefaultBindAsync(client => SendAsync(request, client, token));
    }
    
    public async Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TContentType>(
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType, new()
    {
        return await validation.DefaultBindAsync(request =>
            PostStringAsync<TContentType>(request, content, encoding, token));
    }

    public async Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TContentType>(
        IEssentialsHttpRequest request,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType, new()
    {
        // TODO Log
        if (string.IsNullOrWhiteSpace(content))
            return Error.New("Передана пустая строка запроса");

        return await (
                CreateClient(request),
                BuildStringContent<TContentType>(content, encoding ?? Encoding.UTF8))
            .Apply((client, stringContent) =>
            {
                request.RequestMessage.Method = HttpMethod.Post;
                request.RequestMessage.Content = stringContent;
                return SendAsync(request, client, token);
            })
            .DefaultBindAsync(task => task);
    }

    public async Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TContentType, TContent>(
        Validation<Error, IEssentialsHttpRequest> validation,
        TContent data,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType, new()
    {
        return await validation.DefaultBindAsync(request =>
            PostDataAsync<TContentType, TContent>(request, data, encoding, token));
    }

    public async Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TContentType, TContent>(
        IEssentialsHttpRequest request,
        TContent data,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType, new()
    {
        // TODO Log
        if (data is null)
            return Error.New("Передано пустое содержимое запроса");

        return await SerializersCreator
            .GetSerializer(new TContentType())
            .DefaultBindAsync(serializer => serializer.SerializeObject(data))
            .DefaultBindAsync(requestString => PostStringAsync<TContentType>(request, requestString, encoding, token));
    }

    #region Protected and private
    
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
            responseMessage = await httpClient.SendAsync(request.RequestMessage, token ?? CancellationToken.None);
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

    protected Validation<Error, SystemHttpClient> CreateClient(IEssentialsHttpRequest request)
    {
        return Try(() => Factory.CreateClient(request.ClientName))
            .Match(
                Succ: client =>
                {
                    if (request.Timeout.HasValue)
                        client.Timeout = request.Timeout.Value;
                    return client;
                },
                Fail: exception => Fail<Error, SystemHttpClient>(exception));
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

    private static Validation<Error, StringContent> BuildStringContent<TContentType>(
        string requestSting,
        Encoding encoding)
        where TContentType : IContentType, new()
    {
        return Try(() =>
            {
                var contentType = new TContentType();
                return new StringContent(requestSting, encoding, contentType.ContentTypeName);
            })
            .Match(
                Succ: Success<Error, StringContent>,
                Fail: exception =>
                {
                    // TODO Log
                    return Error.New(exception);
                });
    }

    #endregion
}