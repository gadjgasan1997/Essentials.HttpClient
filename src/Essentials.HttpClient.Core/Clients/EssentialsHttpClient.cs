using System.Diagnostics.CodeAnalysis;
using System.Text;
using Essentials.Func.Utils.Extensions;
using Essentials.HttpClient.MediaTypes.Interfaces;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.Models;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Serialization.Extensions;
using LanguageExt;
using LanguageExt.Common;
using SystemHttpClient = System.Net.Http.HttpClient;
using static LanguageExt.Prelude;
using IRequest = Essentials.HttpClient.IEssentialsHttpRequest;
using Token = System.Threading.CancellationToken;

namespace Essentials.HttpClient.Clients;

/// <inheritdoc cref="IEssentialsHttpClient" />
[SuppressMessage("ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract")]
[SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
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
    public async Task<Validation<Error, IEssentialsHttpResponse>> GetAsync(IRequest request, Token? token = null) =>
        await SendRequestAsync(request, HttpMethod.Get, token);

    /// <inheritdoc cref="IEssentialsHttpClient.HeadAsync(IRequest, Token?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> HeadAsync(IRequest request, Token? token = null) =>
        await SendRequestAsync(request, HttpMethod.Head, token);

    /// <inheritdoc cref="IEssentialsHttpClient.PostStringAsync{TMediaType}(IRequest, string, Encoding?, Token?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TMediaType>(
        IRequest request,
        string content,
        Encoding? encoding = null,
        Token? token = null)
        where TMediaType : IMediaType, new()
    {
        return await SendStringAsync<TMediaType>(
            request,
            content,
            HttpMethod.Post,
            encoding,
            token);
    }

    /// <inheritdoc cref="IEssentialsHttpClient.PostDataAsync{TMediaType, TData, TSerializer}(IRequest, TData, Encoding?, Token?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TMediaType, TData, TSerializer>(
        IRequest request,
        TData data,
        Encoding? encoding = null,
        Token? token = null)
        where TMediaType : IMediaType, new()
        where TSerializer : IEssentialsSerializer
    {
        return await SendDataAsync<TMediaType, TData, TSerializer>(
            request,
            data,
            HttpMethod.Post,
            encoding,
            token);
    }

    /// <inheritdoc cref="IEssentialsHttpClient.PutStringAsync{TMediaType}(IRequest, string, Encoding?, Token?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> PutStringAsync<TMediaType>(
        IRequest request,
        string content,
        Encoding? encoding = null,
        Token? token = null)
        where TMediaType : IMediaType, new()
    {
        return await SendStringAsync<TMediaType>(
            request,
            content,
            HttpMethod.Put,
            encoding,
            token);
    }

    /// <inheritdoc cref="IEssentialsHttpClient.PutDataAsync{TMediaType, TData, TSerializer}(IRequest, TData, Encoding?, Token?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> PutDataAsync<TMediaType, TData, TSerializer>(
        IRequest request,
        TData data,
        Encoding? encoding = null,
        Token? token = null)
        where TMediaType : IMediaType, new()
        where TSerializer : IEssentialsSerializer
    {
        return await SendDataAsync<TMediaType, TData, TSerializer>(
            request,
            data,
            HttpMethod.Put,
            encoding,
            token);
    }

    #region Additional Methods

    /// <summary>
    /// Отправляет запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="httpMethod">Http метод</param>
    /// <param name="token">Токен отмены</param>
    /// <returns></returns>
    private async Task<Validation<Error, IEssentialsHttpResponse>> SendRequestAsync(
        IRequest request,
        HttpMethod httpMethod,
        Token? token = default)
    {
        request.RequestMessage.Method = httpMethod;
        return await CreateClient(request).DefaultBindAsync(client =>
            SendWithMetricsAsync(request, () => SendAsync(request, client, token)));
    }
    
    /// <summary>
    /// Отправляет запрос с данными
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="data">Содержимое</param>
    /// <param name="httpMethod">Http метод</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TMediaType">Тип содержимого запроса (Json, Xml, ...)</typeparam>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns></returns>
    private async Task<Validation<Error, IEssentialsHttpResponse>> SendDataAsync<TMediaType, TData, TSerializer>(
        IRequest request,
        TData data,
        HttpMethod httpMethod,
        Encoding? encoding = null,
        Token? token = null)
        where TMediaType : IMediaType, new()
        where TSerializer : IEssentialsSerializer
    {
        // TODO Log
        if (data is null)
            return Error.New("Передано пустое содержимое запроса");

        return await SerializersCreator
            .GetSerializer<TSerializer>()
            .DefaultBindAsync(serializer => serializer.SerializeObject(data))
            .DefaultBindAsync(requestString =>
                SendStringAsync<TMediaType>(request, requestString, httpMethod, encoding, token));
    }
    
    /// <summary>
    /// Отправляет строку запроса
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Строка с содержимым</param>
    /// <param name="httpMethod">Http метод</param>
    /// <param name="encoding">Кодировка</param>
    /// <param name="token">Токен отмены</param>
    /// <typeparam name="TMediaType">Тип содержимого запроса (Json, Xml, ...)</typeparam>
    /// <returns></returns>
    private async Task<Validation<Error, IEssentialsHttpResponse>> SendStringAsync<TMediaType>(
        IRequest request,
        string content,
        HttpMethod httpMethod,
        Encoding? encoding = null,
        Token? token = null)
        where TMediaType : IMediaType, new()
    {
        // TODO Log
        if (string.IsNullOrWhiteSpace(content))
            return Error.New("Передана пустая строка запроса");

        return await (
                CreateClient(request),
                BuildStringContent<TMediaType>(content, encoding ?? Encoding.UTF8))
            .Apply((client, stringContent) =>
            {
                request.RequestMessage.Method = httpMethod;
                request.RequestMessage.Content = stringContent;
                return SendWithMetricsAsync(request, () => SendAsync(request, client, token));
            })
            .DefaultBindAsync(task => task);
    }

    /// <summary>
    /// Отправляет запрос с добавлением метрик
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="sendFunc">Делегат отправки запроса</param>
    /// <returns>Http ответ</returns>
    protected virtual async Task<Validation<Error, IEssentialsHttpResponse>> SendWithMetricsAsync(
        IRequest request,
        Func<Task<Validation<Error, IEssentialsHttpResponse>>> sendFunc)
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
    protected virtual async Task<Validation<Error, IEssentialsHttpResponse>> SendAsync(
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
            return Error.New($"Ошибочный Http код ответа: '{responseMessage.StatusCode}'.");

        return new EssentialsHttpResponse(responseMessage);
    }

    /// <summary>
    /// Создает контент для отправки
    /// </summary>
    /// <param name="requestSting">Строка запроса</param>
    /// <param name="encoding">Кодировка</param>
    /// <typeparam name="TMediaType">Тип содержимого запроса</typeparam>
    /// <returns>Контент</returns>
    protected virtual Validation<Error, StringContent> BuildStringContent<TMediaType>(
        string requestSting,
        Encoding encoding)
        where TMediaType : IMediaType, new()
    {
        return Try(() =>
            {
                var mediaType = new TMediaType();
                return new StringContent(requestSting, encoding, mediaType.TypeName);
            })
            .Match(
                Succ: Success<Error, StringContent>,
                Fail: exception =>
                {
                    // TODO Log
                    return Error.New(exception);
                });
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

    #endregion
}