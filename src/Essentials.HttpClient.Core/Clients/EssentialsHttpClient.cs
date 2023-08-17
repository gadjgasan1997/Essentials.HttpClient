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
    public Task<Validation<Error, IResponse>> GetAsync(IRequest request, Token? token = null) =>
        SendRequestWithoutContentAsync(request, HttpMethod.Get, token);

    /// <inheritdoc cref="IEssentialsHttpClient.HeadAsync(IRequest, Token?)" />
    public Task<Validation<Error, IResponse>> HeadAsync(IRequest request, Token? token = null) =>
        SendRequestWithoutContentAsync(request, HttpMethod.Head, token);

    /// <inheritdoc cref="IEssentialsHttpClient.PostAsync(IRequest, HttpContent, IMediaType, Encoding?, Token?)" />
    public Task<Validation<Error, IResponse>> PostAsync(
        IRequest request,
        HttpContent content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return SendRequestWithContentAsync(request, content, HttpMethod.Post, mediaType, encoding, token);
    }
    
    /// <inheritdoc cref="IEssentialsHttpClient.PutAsync(IRequest, HttpContent, IMediaType, Encoding?, Token?)" />
    public Task<Validation<Error, IResponse>> PutAsync(
        IRequest request,
        HttpContent content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return SendRequestWithContentAsync(request, content, HttpMethod.Put, mediaType, encoding, token);
    }

    /// <inheritdoc cref="IEssentialsHttpClient.PatchAsync(IRequest, HttpContent, IMediaType, Encoding?, Token?)" />
    public Task<Validation<Error, IEssentialsHttpResponse>> PatchAsync(
        IEssentialsHttpRequest request,
        HttpContent content,
        IMediaType? mediaType = null,
        Encoding? encoding = null,
        CancellationToken? token = null)
    {
        return SendRequestWithContentAsync(request, content, HttpMethod.Patch, mediaType, encoding, token);
    }

    /// <inheritdoc cref="IEssentialsHttpClient.DeleteAsync(IRequest, Token?)" />
    public Task<Validation<Error, IResponse>> DeleteAsync(IRequest request, Token? token = null) =>
        SendRequestWithoutContentAsync(request, HttpMethod.Delete, token);

    #region Additional Methods

    /// <summary>
    /// Отправляет запрос без содержимого
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="httpMethod">Http метод</param>
    /// <param name="token">Токен отмены</param>
    /// <returns></returns>
    private async Task<Validation<Error, IResponse>> SendRequestWithoutContentAsync(
        IRequest request,
        HttpMethod httpMethod,
        Token? token = null)
    {
        // TODO Log
        if (request is null)
            return Error.New("Передан пустой запрос");
        
        if (!TryModifyRequest(Action, out var exception))
            return Error.New(exception);
        
        return await SendRequestAsync(request, token).ConfigureAwait(false);

        void Action() => request.RequestMessage.Method = httpMethod;
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
    private async Task<Validation<Error, IResponse>> SendRequestWithContentAsync(
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
        
        if (!TryModifyRequest(Action, out var exception))
            return Error.New(exception);

        return await SendRequestAsync(request, token).ConfigureAwait(false);

        void Action()
        {
            if (mediaType is not null)
                content.Headers.ContentType = GetContentType(mediaType, encoding);

            request.RequestMessage.Method = httpMethod;
            request.RequestMessage.Content = content;
        }
    }

    /// <summary>
    /// Отправляет запрос
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="token">Токен отмены</param>
    /// <returns></returns>
    private async Task<Validation<Error, IEssentialsHttpResponse>> SendRequestAsync(
        IEssentialsHttpRequest request,
        Token? token = null)
    {
        return await CreateClient(request).BindAsync(SendFunc).ConfigureAwait(false);

        async Task<Validation<Error, IEssentialsHttpResponse>> SendFunc(SystemHttpClient client) =>
            await SendWithMetricsAsync(request,
                    async () => await SendAsync(request, client, token).ConfigureAwait(false))
                .ConfigureAwait(false);
    }

    /// <summary>
    /// Пытается изменить запрос
    /// </summary>
    /// <param name="modifyRequestAction">Действие изменения запроса</param>
    /// <param name="exception">Возникшее исключение</param>
    /// <returns></returns>
    private static bool TryModifyRequest(
        Action modifyRequestAction,
        [NotNullWhen(false)] out Exception? exception)
    {
        try
        {
            modifyRequestAction();
            exception = null;
            return true;
        }
        catch (Exception innerException)
        {
            // TODO Log and ex message
            exception = new InvalidOperationException(
                message: "Во время изменения запроса произошло исключение. Запрос не будет отправлен.",
                innerException);
            
            return false;
        }
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
        
        var validation = await sendFunc().ConfigureAwait(false);
        
        validation.Match(
            Succ: _ => _metrics.HttpRequestSuccessSent(request.ClientName, request.RequestMessage.Method),
            Fail: _ => _metrics.HttpRequestErrorSent(request.ClientName, request.RequestMessage.Method));
        
        return validation;
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
        Token? token = null)
    {
        // TODO Log
        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await httpClient
                .SendAsync(request.RequestMessage, token ?? Token.None)
                .ConfigureAwait(false);
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