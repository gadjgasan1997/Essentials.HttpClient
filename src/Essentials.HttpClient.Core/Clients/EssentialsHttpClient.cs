using LanguageExt;
using LanguageExt.Common;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;
using Essentials.Utils.Extensions;
using Essentials.HttpClient.Errors;
using Essentials.HttpClient.Models;
using Essentials.HttpClient.Extensions;
using Essentials.Functional.Extensions;
using Essentials.HttpClient.RequestsInterception;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Events.EventsStorage;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;
using SystemHttpClient = System.Net.Http.HttpClient;
using Token = System.Threading.CancellationToken;

namespace Essentials.HttpClient.Clients;

/// <inheritdoc cref="IEssentialsHttpClient" />
[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
internal sealed class EssentialsHttpClient : IEssentialsHttpClient
{
    private readonly IHttpClientFactory _factory;
    private readonly IEnumerable<IRequestInterceptor> _interceptors;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="interceptors"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public EssentialsHttpClient(IHttpClientFactory factory, IEnumerable<IRequestInterceptor> interceptors)
    {
        _factory = factory.CheckNotNull();
        _interceptors = interceptors;
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

    #region Private Methods

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
        if (request is null)
            return Error.New(EmptyRequest);
        
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
        if (request is null)
            return Error.New(EmptyRequest);

        if (content is null)
            return Error.New(EmptyContent);

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
        using var scope = HttpRequestContext.CreateContext(request);
        
        return await CreateClient(request).BindAsync(SendFunc).ConfigureAwait(false);

        async Task<Validation<Error, IResponse>> SendFunc(SystemHttpClient client) =>
            await SendAsync(request, client, httpMethod, httpContent, token).ConfigureAwait(false);
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
    private async Task<Validation<Error, IResponse>> SendAsync(
        IRequest request,
        SystemHttpClient httpClient,
        HttpMethod httpMethod,
        HttpContent? httpContent = null,
        Token? token = null)
    {
        Contract.Assert(HttpRequestContext.Current is not null);
        
        HttpResponseMessage responseMessage;
        using var requestMessage = new HttpRequestMessage();

        try
        {
            requestMessage.RequestUri = request.Uri;
            requestMessage.Method = httpMethod;

            HttpRequestContext.Current.RequestMessage = requestMessage;

            foreach (var action in request.ModifyRequestActions)
                action(requestMessage);

            if (httpContent is not null)
            {
                request.MediaType.IfSome(mediaType => httpContent.Headers.ContentType = mediaType);
                requestMessage.Content = httpContent;
            }

            request.RaiseEvent(nameof(OnBeforeSend), RaiseOnBeforeSend);

            responseMessage = await request.Interceptors
                .Select(type => _interceptors.FirstOrDefault(interceptor => interceptor.GetType() == type))
                .OfType<IRequestInterceptor>()
                .Aggregate(
                    (NextRequestDelegate) SeedAsync,
                    (next, interceptor) => () => NextAsync(next, interceptor))
                .Invoke()
                .ConfigureAwait(false);

            // ReSharper disable once AccessToDisposedClosure
            async Task<HttpResponseMessage> SeedAsync() =>
                await httpClient.SendAsync(requestMessage, token ?? Token.None).ConfigureAwait(false);

            async Task<HttpResponseMessage> NextAsync(NextRequestDelegate next, IRequestInterceptor interceptor) =>
                await interceptor.Intercept(next).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            var errorMessage = exception.ToHttpRequestExceptionMessage(request);

            HttpRequestContext.Current.SetError(exception, errorMessage);
            
            request.RaiseEvent(nameof(OnErrorSend), RaiseOnErrorSend);
            return Error.New(exception);
        }

        HttpRequestContext.Current.SetResponse(responseMessage);
        
        if (!responseMessage.IsSuccessStatusCode)
        {
            request.RaiseEvent(nameof(OnBadStatusCode), RaiseOnBadStatusCode);
            
            return BadStatusCodeError.New(
                responseMessage,
                string.Format(BadStatusCode, responseMessage.StatusCode));
        }
        
        request.RaiseEvent(nameof(OnSuccessSend), RaiseOnSuccessSend);
        return new Response(request, responseMessage);
    }
    
    /// <summary>
    /// Создает Http клиент
    /// </summary>
    /// <param name="request">Http запрос</param>
    /// <returns>Http клиент</returns>
    private Validation<Error, SystemHttpClient> CreateClient(IRequest request)
    {
        return Try(() => _factory.CreateClient(request.ClientName))
            .Match(
                Succ: client =>
                {
                    request.Timeout.IfSome(timeout => client.Timeout = timeout);
                    return client;
                },
                Fail: exception =>
                {
                    var errorMessage = string.Format(ErrorCreateClient, exception.Message);
                    return Fail<Error, SystemHttpClient>(Error.New(errorMessage, exception));
                });
    }

    #endregion
}