using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using Essentials.HttpClient.Events.Args;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Models;
using LanguageExt;
using LanguageExt.Common;
using static Essentials.HttpClient.Dictionaries.KnownAuthenticationSchemes;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;
using static Essentials.HttpClient.Events.EventsPublisher;
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved

namespace Essentials.HttpClient.Builders;

/// <summary>
/// Билдер создания запроса
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal class EssentialsRequestBuilder : IRequestBuilder
{
    private readonly Uri _uri;
    
    private MediaTypeHeaderValue? _mediaTypeHeader;
    
    private AuthenticationHeaderValue? _authenticationHeader;

    private readonly List<(string, IEnumerable<string?>)> _headers = new();

    private readonly List<Action<HttpRequestMessage>> _modifyRequestActions = new();
    
    private TimeSpan? _timeout;
    
    public EssentialsRequestBuilder(Uri uri)
    {
        _uri = uri ?? throw new ArgumentNullException(nameof(uri));
    }
    
    /// <inheritdoc cref="IRequestBuilder.WithHeader"/>
    public IRequestBuilder WithHeader(string name, params string?[] values) =>
        ModifyRequest(() => AddHeader(name, values));

    /// <inheritdoc cref="IRequestBuilder.WithNotEmptyHeader"/>
    public IRequestBuilder WithNotEmptyHeader(string name, params string?[] values) =>
        ModifyRequest(() => AddHeader(name, values.Where(value => !string.IsNullOrWhiteSpace(value))));
    
    /// <inheritdoc cref="IRequestBuilder.WithHeaders"/>
    public IRequestBuilder WithHeaders(params (string Name, IEnumerable<string?> Value)[] headers) =>
        ModifyRequest(() => headers.Map(header => AddHeader(header.Name, header.Value)));

    /// <inheritdoc cref="IRequestBuilder.WithNotEmptyHeaders"/>
    public IRequestBuilder WithNotEmptyHeaders(params (string Name, IEnumerable<string?> Value)[] headers) =>
        ModifyRequest(() => headers.Map(header =>
            AddHeader(header.Name, header.Value.Where(value => !string.IsNullOrWhiteSpace(value)))));
    
    /// <summary>
    /// Добавляет заголовок к запросу
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="values">Значения заголовка</param>
    private void AddHeader(string name, IEnumerable<string?> values)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        _headers.Add((name, values));
    }

    /// <inheritdoc cref="IRequestBuilder.SetMediaType"/>
    public IRequestBuilder SetMediaType(string mediaType, Encoding? encoding = null)
    {
        return ModifyRequest(() =>
        {
            if (string.IsNullOrWhiteSpace(mediaType))
                return;
            
            encoding ??= Encoding.Default;
            _mediaTypeHeader = new MediaTypeHeaderValue(mediaType, encoding.WebName);
        });
    }

    /// <inheritdoc cref="IRequestBuilder.WithBasicAuthentication"/>
    public IRequestBuilder WithBasicAuthentication(string userName, string password)
    {
        string authenticationString;
        try
        {
            var bytes = Encoding.ASCII.GetBytes($"{userName}:{password}");
            authenticationString = Convert.ToBase64String(bytes);
        }
        catch (Exception exception)
        {
            RaiseOnErrorCreateRequest(
                new ErrorCreateRequestEventArgs(
                    _uri,
                    exception,
                    "Во время получения авторизационной строки из логина и пароля произошло исключение"));

            return this;
        }

        return WithAuthentication(BASIC, authenticationString);
    }
    
    /// <inheritdoc cref="IRequestBuilder.WithJwtAuthentication"/>
    public IRequestBuilder WithJwtAuthentication(string token) => WithAuthentication(JWT, token);
    
    /// <inheritdoc cref="IRequestBuilder.WithAuthentication"/>
    public IRequestBuilder WithAuthentication(string scheme, string parameter)
    {
        return ModifyRequest(() =>
        {
            if (string.IsNullOrWhiteSpace(scheme) || string.IsNullOrWhiteSpace(parameter))
                return;

            _authenticationHeader = new AuthenticationHeaderValue(scheme, parameter);
        });
    }

    /// <inheritdoc cref="IRequestBuilder.ModifyRequest"/>
    public IRequestBuilder ModifyRequest(Action<HttpRequestMessage?> action)
    {
        _modifyRequestActions.Add(action);
        return this;
    }
    
    /// <inheritdoc cref="IRequestBuilder.SetTimeout"/>
    public IRequestBuilder SetTimeout(TimeSpan timeout) => ModifyRequest(() => _timeout = timeout);

    /// <inheritdoc cref="IRequestBuilder.Build"/>
    public Validation<Error, IRequest> Build(string? clientName = null)
    {
        return Try(() => BuildPrivate(clientName ?? nameof(IEssentialsHttpClient)))
            .Match(
                Succ: validation => validation,
                Fail: exception =>
                {
                    RaiseOnErrorCreateRequest(new ErrorCreateRequestEventArgs(_uri, exception));
                    return Error.New(exception);
                });
    }

    /// <inheritdoc cref="IRequestBuilder.BuildAsync"/>
    public Task<Validation<Error, IRequest>> BuildAsync(string? clientName = null) =>
        Build(clientName).AsTask();

    /// <inheritdoc cref="IRequestBuilder.Build"/>
    public Validation<Error, IRequest> Build<TClient>() => Build(typeof(TClient).Name);

    /// <inheritdoc cref="IRequestBuilder.BuildAsync"/>
    public Task<Validation<Error, IRequest>> BuildAsync<TClient>() => Build<TClient>().AsTask();

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns>Запрос</returns>
    private Validation<Error, IRequest> BuildPrivate(string clientName)
    {
        var request = new Request(
            clientName,
            _uri,
            _mediaTypeHeader,
            _authenticationHeader,
            _headers,
            _modifyRequestActions,
            _timeout);
        
        RaiseOnSuccessCreateRequest(new SuccessCreateRequestEventArgs(request));
        return request;
    }

    /// <summary>
    /// Меняет запрос
    /// </summary>
    /// <param name="modifyRequestAction">Действие по изменению запроса</param>
    /// <returns>Билдер</returns>
    private EssentialsRequestBuilder ModifyRequest(Action modifyRequestAction)
    {
        try
        {
            modifyRequestAction();
        }
        catch (Exception ex)
        {
            var errorMessage = string.Format(ErrorModifyRequest, ex.Message);
            RaiseOnErrorCreateRequest(new ErrorCreateRequestEventArgs(_uri, ex, errorMessage));
        }
        
        return this;
    }
}