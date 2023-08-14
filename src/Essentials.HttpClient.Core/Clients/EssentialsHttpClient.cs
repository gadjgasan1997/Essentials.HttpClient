using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using Essentials.Func.Utils.Extensions;
using Essentials.HttpClient.Errors;
using Essentials.HttpClient.MediaTypes.Interfaces;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.Models;
using LanguageExt;
using LanguageExt.Common;
using SystemHttpClient = System.Net.Http.HttpClient;
using static LanguageExt.Prelude;
using IRequest = Essentials.HttpClient.IEssentialsHttpRequest;
using IResponse = Essentials.HttpClient.IEssentialsHttpResponse;
using Token = System.Threading.CancellationToken;

namespace Essentials.HttpClient.Clients;

/// <inheritdoc cref="IEssentialsHttpClient" />
[SuppressMessage("ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract")]
[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
public class EssentialsHttpClient : IEssentialsHttpClient
{
    private readonly IMetricsService _metrics;
    private readonly IHttpClientFactory _factory;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="metricsService"></param>
    /// <param name="factory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public EssentialsHttpClient(
        IMetricsService metricsService,
        IHttpClientFactory factory)
    {
        _metrics = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <inheritdoc cref="IEssentialsHttpClient.GetAsync(IRequest, Token?)" />
    public async Task<Validation<Error, IResponse>> GetAsync(IRequest request, Token? token = null) =>
        await SendWithoutContentAsync(request, HttpMethod.Get, token);

    /// <inheritdoc cref="IEssentialsHttpClient.HeadAsync(IRequest, Token?)" />
    public async Task<Validation<Error, IResponse>> HeadAsync(IRequest request, Token? token = null) =>
        await SendWithoutContentAsync(request, HttpMethod.Head, token);

    /// <inheritdoc cref="IEssentialsHttpClient.PostAsync(IRequest, HttpContent, IMediaType, Encoding?, Token?)" />
    public async Task<Validation<Error, IResponse>> PostAsync(
        IRequest request,
        HttpContent content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await SendWithContentAsync(request, content, HttpMethod.Post, mediaType, encoding, token);
    }
    
    /// <inheritdoc cref="IEssentialsHttpClient.PutAsync(IRequest, HttpContent, IMediaType, Encoding?, Token?)" />
    public async Task<Validation<Error, IResponse>> PutAsync(
        IRequest request,
        HttpContent content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await SendWithContentAsync(request, content, HttpMethod.Put, mediaType, encoding, token);
    }

    /// <inheritdoc cref="IEssentialsHttpClient.PatchAsync(IRequest, HttpContent, IMediaType, Encoding?, Token?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> PatchAsync(
        IEssentialsHttpRequest request,
        HttpContent content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return await SendWithContentAsync(request, content, HttpMethod.Patch, mediaType, encoding, token);
    }

    /// <inheritdoc cref="IEssentialsHttpClient.DeleteAsync(IRequest, Token?)" />
    public async Task<Validation<Error, IResponse>> DeleteAsync(IRequest request, Token? token = null) =>
        await SendWithoutContentAsync(request, HttpMethod.Delete, token);

    #region Additional Methods

    /// <summary>
    /// Отправляет запрос без содержимого
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="httpMethod">Http метод</param>
    /// <param name="token">Токен отмены</param>
    /// <returns></returns>
    private async Task<Validation<Error, IResponse>> SendWithoutContentAsync(
        IRequest request,
        HttpMethod httpMethod,
        Token? token = default)
    {
        // TODO Log
        if (request is null)
            return Error.New("Передан пустой запрос");
        
        request.RequestMessage.Method = httpMethod;
        
        return await CreateClient(request)
            .DefaultBindAsync(client =>
                SendWithMetricsAsync(
                    request,
                    sendFunc: () => SendAsync(request, client, token)));
    }

    /// <summary>
    /// Отправляет запрос с содержимым
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Содержимое</param>
    /// <param name="httpMethod">Http метод</param>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <returns></returns>
    private async Task<Validation<Error, IResponse>> SendWithContentAsync(
        IRequest request,
        HttpContent content,
        HttpMethod httpMethod,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        Token? token = null)
    {
        // TODO Log
        if (request is null)
            return Error.New("Передан пустой запрос");
        
        if (content is null)
            return Error.New("Передано пустое содержимое запроса");
        
        if (mediaType is not null)
            content.Headers.ContentType = GetContentType(mediaType, encoding);
        
        request.RequestMessage.Method = httpMethod;
        request.RequestMessage.Content = content;

        return await CreateClient(request)
            .DefaultBindAsync(client =>
                SendWithMetricsAsync(
                    request,
                    sendFunc: () => SendAsync(request, client, token)));
    }

    /// <summary>
    /// Отправляет запрос с добавлением метрик
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="sendFunc">Делегат отправки запроса</param>
    /// <returns>Http ответ</returns>
    protected virtual async Task<Validation<Error, IResponse>> SendWithMetricsAsync(
        IRequest request,
        Func<Task<Validation<Error, IResponse>>> sendFunc)
    {
        using var timer = _metrics.StartRequestTimer(request.ClientName);
        _metrics.HttpRequestSent(request.ClientName, request.RequestMessage.Method);
        
        var result = await sendFunc();
        
        result.Match(
            Succ: _ => _metrics.HttpRequestSuccessSent(request.ClientName, request.RequestMessage.Method),
            Fail: _ => _metrics.HttpRequestErrorSent(request.ClientName, request.RequestMessage.Method));
        
        return result;
    }

    /// <summary>
    /// Отправляет Http запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    protected virtual async Task<Validation<Error, IResponse>> SendAsync(
        IRequest request,
        SystemHttpClient httpClient,
        Token? token = default)
    {
        // TODO Log
        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await httpClient.SendAsync(request.RequestMessage, token ?? Token.None);
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
        {
            return BadStatusCodeError.New(
                responseMessage,
                $"Ошибочный Http код ответа: '{responseMessage.StatusCode}'.");
        }

        return new EssentialsHttpResponse(responseMessage);
    }
    
    /// <summary>
    /// Создает Http клиент
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <returns>Http клиент</returns>
    protected virtual Validation<Error, SystemHttpClient> CreateClient(IRequest request)
    {
        return Try(() => _factory.CreateClient(request.ClientName))
            .Match(
                Succ: client =>
                {
                    if (request.Timeout.HasValue)
                        client.Timeout = request.Timeout.Value;
                    return client;
                },
                Fail: exception => Fail<Error, SystemHttpClient>(exception));
    }

    /// <summary>
    /// Возвращает заголовок с типом содержимого
    /// </summary>
    /// <param name="mediaType">Тип содержимого запроса (Json, Xml, ...)</param>
    /// <param name="encoding">Кодировка</param>
    /// <returns></returns>
    private static MediaTypeHeaderValue GetContentType(IMediaType mediaType, Encoding? encoding = null)
    {
        encoding ??= Encoding.Default;
        return new MediaTypeHeaderValue(mediaType.TypeName, encoding.WebName);
    }

    #endregion
}