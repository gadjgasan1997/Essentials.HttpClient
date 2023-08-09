using System.Diagnostics.CodeAnalysis;
using System.Text;
using Essentials.Func.Utils.Extensions;
using Essentials.HttpClient.ContentTypes.Interfaces;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.Models;
using Essentials.HttpClient.Serialization;
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

    protected EssentialsHttpClient(
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
        return await CreateClient(request.ClientName).DefaultBindAsync(client => SendAsync(request, client, token));
    }
    
    public async Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TContentType>(
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        TContentType contentType,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType
    {
        return await validation.DefaultBindAsync(request =>
            PostStringAsync(request, content, contentType, encoding, token));
    }

    public async Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TContentType>(
        IEssentialsHttpRequest request,
        string content,
        TContentType contentType,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType
    {
        // TODO Log
        if (string.IsNullOrWhiteSpace(content))
            return Error.New("Передана пустая строка запроса");
        
        if (string.IsNullOrWhiteSpace(contentType?.ContentTypeName))
            return Error.New("Передан пустой тип содержимого запроса");
        
        return await (
                CreateClient(request.ClientName),
                BuildStringContent(content, encoding ?? Encoding.UTF8, contentType.ContentTypeName))
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
        TContentType contentType,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType
    {
        return await validation.DefaultBindAsync(request => PostDataAsync(request, data, contentType, encoding, token));
    }

    public async Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TContentType, TContent>(
        IEssentialsHttpRequest request,
        TContent data,
        TContentType contentType,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TContentType : IContentType
    {
        // TODO Log
        if (data is null)
            return Error.New("Передано пустое содержимое запроса");
        
        if (string.IsNullOrWhiteSpace(contentType?.ContentTypeName))
            return Error.New("Передан пустой тип содержимого запроса");

        return await SerializersCreator
            .GetSerializer(contentType.ContentTypeName)
            .DefaultBindAsync(serializer => Serialize(serializer, data))
            .DefaultBindAsync(requestString => PostStringAsync(request, requestString, contentType, encoding, token));
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

    protected Validation<Error, SystemHttpClient> CreateClient(string clientName)
    {
        return Try(() => Factory.CreateClient(clientName))
            .Try()
            .Match(
                Succ: client => client,
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
    
    private static Validation<Error, string> Serialize<T>(IEssentialsSerializer serializer, T content)
    {
        return Try(() => serializer.Serialize(content)).Try()
            .Match(
                Succ: serializedString => serializedString,
                Fail: exception =>
                {
                    // TODO Log
                    return Error.New(exception);
                });
    }

    private static Validation<Error, StringContent> BuildStringContent(
        string requestSting,
        Encoding encoding,
        string contentType)
    {
        return Try(() => new StringContent(requestSting, encoding, contentType)).Try()
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