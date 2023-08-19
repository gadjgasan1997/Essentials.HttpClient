using System.Diagnostics.CodeAnalysis;
using Essentials.Func.Utils.Extensions;
using Essentials.HttpClient.Errors;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.Models;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using SystemHttpClient = System.Net.Http.HttpClient;
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

    /// <inheritdoc cref="IEssentialsHttpClient.PostAsync(IRequest, HttpContent, Token?)" />
    public Task<Validation<Error, IResponse>> PostAsync(IRequest request, HttpContent content, Token? token = null) =>
        SendRequestWithContentAsync(request, content, HttpMethod.Post, token);

    /// <inheritdoc cref="IEssentialsHttpClient.PutAsync(IRequest, HttpContent, Token?)" />
    public Task<Validation<Error, IResponse>> PutAsync(IRequest request, HttpContent content, Token? token = null) =>
        SendRequestWithContentAsync(request, content, HttpMethod.Put, token);

    /// <inheritdoc cref="IEssentialsHttpClient.PatchAsync(IRequest, HttpContent, Token?)" />
    public Task<Validation<Error, IResponse>> PatchAsync(IRequest request, HttpContent content, Token? token = null) =>
        SendRequestWithContentAsync(request, content, HttpMethod.Patch, token);

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
        
        return await SendRequestAsync(request, httpMethod, token: token).ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет запрос с содержимым
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="content">Содержимое</param>
    /// <param name="httpMethod">Http метод</param>
    /// <param name="token">Токен отмены</param>
    /// <returns></returns>
    private async Task<Validation<Error, IResponse>> SendRequestWithContentAsync(
        IRequest request,
        HttpContent content,
        HttpMethod httpMethod,
        Token? token = null)
    {
        // TODO Log
        if (request is null)
            return Error.New("Передан пустой запрос");
        
        if (content is null)
            return Error.New("Передано пустое содержимое запроса");

        return await SendRequestAsync(request, httpMethod, content, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет запрос
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="httpMethod">Http метод</param>
    /// <param name="httpContent">Содержимое запроса</param>
    /// <param name="token">Токен отмены</param>
    /// <returns></returns>
    private async Task<Validation<Error, IResponse>> SendRequestAsync(
        IRequest request,
        HttpMethod httpMethod,
        HttpContent? httpContent = null,
        Token? token = null)
    {
        return await CreateClient(request).BindAsync(SendFunc).ConfigureAwait(false);

        async Task<Validation<Error, IResponse>> SendFunc(SystemHttpClient client) =>
            await SendWithMetricsAsync(
                    request: request,
                    method: httpMethod,
                    async () => await SendAsync(request, client, httpMethod, httpContent, token).ConfigureAwait(false))
                .ConfigureAwait(false);
    }

    /// <summary>
    /// Отправляет запрос с добавлением метрик
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="method">Метод</param>
    /// <param name="sendFunc">Делегат отправки запроса</param>
    /// <returns>Http ответ</returns>
    protected virtual async Task<Validation<Error, IResponse>> SendWithMetricsAsync(
        IRequest request,
        HttpMethod method,
        Func<Task<Validation<Error, IResponse>>> sendFunc)
    {
        using var timer = _metrics.StartRequestTimer(request.ClientName);
        _metrics.HttpRequestSent(request.ClientName, method);
        
        var validation = await sendFunc().ConfigureAwait(false);
        
        validation.Match(
            Succ: _ => _metrics.HttpRequestSuccessSent(request.ClientName, method),
            Fail: _ => _metrics.HttpRequestErrorSent(request.ClientName, method));
        
        return validation;
    }

    /// <summary>
    /// Отправляет Http запрос
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <param name="httpClient">Http клиент</param>
    /// <param name="httpMethod">Http метод</param>
    /// <param name="httpContent">Содержимое запроса</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Http ответ</returns>
    protected virtual async Task<Validation<Error, IResponse>> SendAsync(
        IRequest request,
        SystemHttpClient httpClient,
        HttpMethod httpMethod,
        HttpContent? httpContent = null,
        Token? token = null)
    {
        // TODO Log
        HttpResponseMessage? responseMessage = null;
        HttpRequestMessage? requestMessage = null;
        try
        {
            requestMessage = new HttpRequestMessage
            {
                RequestUri = request.Uri,
                Method = httpMethod
            };

            if (request.AuthenticationHeader is not null)
                requestMessage.Headers.Authorization = request.AuthenticationHeader;

            if (request.Headers is not null)
                foreach (var (name, values) in request.Headers)
                    requestMessage.Headers.Add(name, values);

            if (request.ModifyRequestActions is not null)
                foreach (var action in request.ModifyRequestActions)
                    action(requestMessage);

            if (httpContent is not null)
            {
                httpContent.Headers.ContentType = request.MediaTypeHeader;
                requestMessage.Content = httpContent;
            }

            responseMessage = await httpClient.SendAsync(requestMessage, token ?? Token.None).ConfigureAwait(false);
        }
        catch (TimeoutException ex)
        {
            responseMessage?.Dispose();
            return Error.New(ex);
        }
        catch (Exception ex)
        {
            responseMessage?.Dispose();
            return Error.New(ex);
        }
        finally
        {
            requestMessage?.Dispose();
        }

        if (!responseMessage.IsSuccessStatusCode)
        {
            return BadStatusCodeError.New(
                responseMessage,
                $"Ошибочный Http код ответа: '{responseMessage.StatusCode}'.");
        }

        return new Response(responseMessage);
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