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
        return await CreateClient(request).DefaultBindAsync(client =>
            SendWithMetricsAsync(request, () => SendAsync(request, client, token)));
    }

    /// <inheritdoc cref="IEssentialsHttpClient.HeadAsync(Validation{Error, IEssentialsHttpRequest}, CancellationToken?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> HeadAsync(
        Validation<Error, IEssentialsHttpRequest> validation,
        CancellationToken? token = default)
    {
        return await validation.DefaultBindAsync(request => HeadAsync(request, token));
    }

    /// <inheritdoc cref="IEssentialsHttpClient.HeadAsync(IEssentialsHttpRequest, CancellationToken?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> HeadAsync(
        IEssentialsHttpRequest request,
        CancellationToken? token = default)
    {
        request.RequestMessage.Method = HttpMethod.Head;
        return await CreateClient(request).DefaultBindAsync(client =>
            SendWithMetricsAsync(request, () => SendAsync(request, client, token)));
    }

    /// <inheritdoc cref="IEssentialsHttpClient.PostStringAsync{TMediaType}(Validation{Error, IEssentialsHttpRequest}, string, Encoding?, CancellationToken?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TMediaType>(
        Validation<Error, IEssentialsHttpRequest> validation,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TMediaType : IMediaType, new()
    {
        return await validation.DefaultBindAsync(request =>
            PostStringAsync<TMediaType>(request, content, encoding, token));
    }

    /// <inheritdoc cref="IEssentialsHttpClient.PostStringAsync{TMediaType}(IEssentialsHttpRequest, string, Encoding?, CancellationToken?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> PostStringAsync<TMediaType>(
        IEssentialsHttpRequest request,
        string content,
        Encoding? encoding = null,
        CancellationToken? token = null)
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
                request.RequestMessage.Method = HttpMethod.Post;
                request.RequestMessage.Content = stringContent;
                return SendWithMetricsAsync(request, () => SendAsync(request, client, token));
            })
            .DefaultBindAsync(task => task);
    }

    /// <inheritdoc cref="IEssentialsHttpClient.PostDataAsync{TMediaType, TData, TSerializer}(Validation{Error, IEssentialsHttpRequest}, TData, Encoding?, CancellationToken?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TMediaType, TData, TSerializer>(
        Validation<Error, IEssentialsHttpRequest> validation,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TMediaType : IMediaType, new()
        where TSerializer : IEssentialsSerializer
    {
        return await validation.DefaultBindAsync(request =>
            PostDataAsync<TMediaType, TData, TSerializer>(request, data, encoding, token));
    }

    /// <inheritdoc cref="IEssentialsHttpClient.PostDataAsync{TMediaType, TData, TSerializer}(IEssentialsHttpRequest, TData, Encoding?, CancellationToken?)" />
    public async Task<Validation<Error, IEssentialsHttpResponse>> PostDataAsync<TMediaType, TData, TSerializer>(
        IEssentialsHttpRequest request,
        TData data,
        Encoding? encoding = null,
        CancellationToken? token = null)
        where TMediaType : IMediaType, new()
        where TSerializer : IEssentialsSerializer
    {
        // TODO Log
        if (data is null)
            return Error.New("Передано пустое содержимое запроса");

        return await SerializersCreator
            .GetSerializer<TSerializer>()
            .DefaultBindAsync(serializer => serializer.SerializeObject(data))
            .DefaultBindAsync(requestString => PostStringAsync<TMediaType>(request, requestString, encoding, token));
    }

    #region Additional Methods

    /// <summary>
    /// Отправляет запрос с добавлением метрик
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="sendFunc">Делегат отправки запроса</param>
    /// <returns>Http ответ</returns>
    protected virtual async Task<Validation<Error, IEssentialsHttpResponse>> SendWithMetricsAsync(
        IEssentialsHttpRequest request,
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
        IEssentialsHttpRequest request,
        SystemHttpClient httpClient,
        CancellationToken? token = default)
    {
        // TODO Log
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
    protected virtual Validation<Error, SystemHttpClient> CreateClient(IEssentialsHttpRequest request)
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