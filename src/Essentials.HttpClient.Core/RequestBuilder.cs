using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Essentials.HttpClient.Models;
using Essentials.HttpClient.Models.Implementations;
using LanguageExt;
using LanguageExt.Common;
using static Essentials.HttpClient.Dictionaries.KnownAuthenticationSchemes;
using static Essentials.HttpClient.Dictionaries.Loggers;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient;

/// <summary>
/// Билдер создания запроса
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class RequestBuilder
{
    private const string SEPARATOR = "/";
    
    /// <summary>
    /// Билдер
    /// </summary>
    public UriBuilder? UriBuilder { get; private set; }
    
    /// <summary>
    /// Параметры запроса
    /// </summary>
    public NameValueCollection? Query { get; private set; }
    
    /// <summary>
    /// Список сегментов запроса
    /// </summary>
    public List<string>? Segments { get; private set; }
    
    /// <summary>
    /// Сообщение запроса
    /// </summary>
    public HttpRequestMessage? RequestMessage { get; private set; }
    
    private RequestBuilder() { }
    
    /// <summary>
    /// Ининциализиурет параметры билдера
    /// </summary>
    /// <param name="uriGetter">Делегат получения адреса запроса</param>
    /// <returns></returns>
    private RequestBuilder Initialize(Func<Uri> uriGetter)
    {
        try
        {
            UriBuilder = new UriBuilder(uriGetter());
            Query = HttpUtility.ParseQueryString(UriBuilder.Query);
            Segments = UriBuilder.Path.Split(SEPARATOR).Where(@string => !string.IsNullOrWhiteSpace(@string)).ToList();
        }
        catch (Exception ex)
        {
            MainLogger.Error(ex, $"An exception occurred during request creation: '{ex.Message}'");
        }
        
        return this;
    }

    /// <summary>
    /// Создает билдер
    /// </summary>
    /// <param name="address">Адрес запрос</param>
    /// <returns>Билдер</returns>
    public static RequestBuilder CreateBuilder(Uri address) =>
        new RequestBuilder().Initialize(() => address);

    /// <summary>
    /// Создает билдер
    /// </summary>
    /// <param name="address">Адрес запрос</param>
    /// <returns>Билдер</returns>
    public static RequestBuilder CreateBuilder(string address) =>
        new RequestBuilder().Initialize(() => new Uri(address));

    /// <summary>
    /// Добавляет сегмент к запросу
    /// </summary>
    /// <param name="segment">Сегмент</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithSegment(string segment) => ModifyRequest(() => AddSegment(segment));

    /// <summary>
    /// Добавляет сегменты к запросу
    /// </summary>
    /// <param name="segments">Сегменты</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithSegments(IEnumerable<string> segments)
    {
        return ModifyRequest(() =>
        {
            foreach (var segment in segments)
                AddSegment(segment);
        });
    }

    /// <summary>
    /// Добавляет сегмент к запросу
    /// </summary>
    /// <param name="segment">Сегмент</param>
    private void AddSegment(string segment)
    {
        Contract.Assert(Segments is not null);
        
        if (string.IsNullOrWhiteSpace(segment))
            return;
        
        Segments.AddRange(segment.Split(SEPARATOR));
    }

    /// <summary>
    /// Добавляет параметр к Uri запроса
    /// </summary>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение параметра</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithUriParam(string name, string? value) => ModifyRequest(() => AddUriParam(name, value));

    /// <summary>
    /// Добавляет параметр к Uri запроса, если его значение не пустое
    /// </summary>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение параметра</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithNotEmptyUriParam(string name, string value)
    {
        return ModifyRequest(() =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            
            AddUriParam(name, value);
        });
    }
    
    /// <summary>
    /// Добавляет параметры к Uri запроса
    /// </summary>
    /// <param name="parameters">Параметры</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithUriParams(IEnumerable<(string, string?)> parameters)
    {
        return ModifyRequest(() =>
        {
            foreach (var (name, value) in parameters)
                AddUriParam(name, value);
        });
    }
    
    /// <summary>
    /// Добавляет параметры к Uri запроса, если их значения не пустые
    /// </summary>
    /// <param name="parameters">Параметры</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithNotEmptyUriParams(IEnumerable<(string, string)> parameters)
    {
        return ModifyRequest(() =>
        {
            foreach (var (name, value) in parameters)
            {
                if (string.IsNullOrWhiteSpace(value))
                    continue;
                
                AddUriParam(name, value);
            }
        });
    }
    
    /// <summary>
    /// Добавляет параметр к Uri запроса
    /// </summary>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение параметра</param>
    private void AddUriParam(string name, string? value)
    {
        Contract.Assert(Query is not null, "query must not null here!");

        if (string.IsNullOrWhiteSpace(name))
            return;

        Query[name] = value;
    }

    /// <summary>
    /// Добавляет заголовок к запросу
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="value">Значение заголовка</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithHeader(string name, string value) => ModifyRequest(() => AddHeader(name, value));
    
    /// <summary>
    /// Добавляет заголовок к запросу, если его значение не пустое
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="value">Значение заголовка</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithNotEmptyHeader(string name, string value)
    {
        return ModifyRequest(() =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            AddHeader(name, value);
        });
    }
    
    /// <summary>
    /// Добавляет заголовки к запросу
    /// </summary>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithHeaders(IEnumerable<(string, string?)> headers)
    {
        return ModifyRequest(() =>
        {
            foreach (var (name, value) in headers)
                AddHeader(name, value);
        });
    }
    
    /// <summary>
    /// Добавляет заголовки к запросу, если их значения не пустые
    /// </summary>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithNotEmptyHeaders(IEnumerable<(string, string)> headers)
    {
        return ModifyRequest(() =>
        {
            foreach (var (name, value) in headers)
            {
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                AddHeader(name, value);
            }
        });
    }
    
    /// <summary>
    /// Добавляет заголовок к запросу
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="value">Значение заголовка</param>
    private void AddHeader(string name, string? value)
    {
        Contract.Assert(RequestMessage is not null, "RequestMessage must not null here!");

        if (string.IsNullOrWhiteSpace(name))
            return;

        RequestMessage.Headers.Add(name, value);
    }
    
    /// <summary>
    /// Настраивает Basic авторизацию
    /// </summary>
    /// <param name="userName">Логин</param>
    /// <param name="password">Пароль</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithBasicAuthentication(string userName, string password)
    {
        var authenticationString = Prelude
            .Try(() => Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}")))
            .Try()
            .Match<string>(
                Succ: @string => @string,
                Fail: exception =>
                {
                    MainLogger.Error(exception,
                        "An exception occurred during the conversion of the authorization string to a base64 string. " +
                        " Login and password can be viewed in the service configuration.");
                    
                    return string.Empty;
                });

        return WithAuthentication(BASIC, authenticationString);
    }
    
    /// <summary>
    /// Настраивает авторизацию на Jwt токенах
    /// </summary>
    /// <param name="token">Токен</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithJwtAuthentication(string token) => WithAuthentication(JWT, token);
    
    /// <summary>
    /// Настраивает авторизацию
    /// </summary>
    /// <param name="scheme">Схема авторизации</param>
    /// <param name="parameter">Параметр</param>
    /// <returns>Билдер</returns>
    public RequestBuilder WithAuthentication(string scheme, string parameter)
    {
        return ModifyRequest(() =>
        {
            Contract.Assert(RequestMessage is not null, "RequestMessage must not null here!");

            if (string.IsNullOrWhiteSpace(scheme) || string.IsNullOrWhiteSpace(parameter))
                return;

            RequestMessage.Headers.Authorization = new AuthenticationHeaderValue(scheme, parameter);
        });
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns>Запрос</returns>
    public Validation<Error, IHttpRequest> Build(string? clientName = null)
    {
        // TODO Rename default client
        return Try(() => BuildPrivate(clientName ?? nameof(RequestBuilder))).Try()
            .Match(
                Succ: validation => validation,
                Fail: exception =>
                {
                    MainLogger.Error("An error occurred during the creation of the Http request.");
                    return Error.New(exception);
                });
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns>Запрос</returns>
    public Task<Validation<Error, IHttpRequest>> BuildAsync(string? clientName = null) =>
        Build(clientName).AsTask();

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <typeparam name="TClient">Тип клиента</typeparam>
    /// <returns>Запрос</returns>
    public Validation<Error, IHttpRequest> Build<TClient>() => Build(typeof(TClient).Name);

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <typeparam name="TClient">Тип клиента</typeparam>
    /// <returns>Запрос</returns>
    public Task<Validation<Error, IHttpRequest>> BuildAsync<TClient>() => Build<TClient>().AsTask();

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns>Запрос</returns>
    private Validation<Error, IHttpRequest> BuildPrivate(string clientName)
    {
        if (CheckRequestParams() && Segments is not null)
        {
            UriBuilder.Path = string.Join(SEPARATOR, Segments);
            UriBuilder.Query = Query.ToString();

            if (!Uri.TryCreate(UriBuilder.ToString(), UriKind.RelativeOrAbsolute, out var uri))
            {
                var invalidAddressError = "An error occurred during the construction of the request address. " +
                                          $"Invalid address: '{UriBuilder}'.";
                
                MainLogger.Error(invalidAddressError);
                return Error.New(invalidAddressError);
            }

            MainLogger.Trace(
                $"The request was successfully created. Http client used: '{clientName}'. " +
                $"Request address: '{uri}'.");

            RequestMessage.RequestUri = uri;
            return new HttpRequest(clientName, RequestMessage);
        }
        
        const string errorMessage =
            "An error occurred during the creation of the Http request. Required parameters are not initialized.";
                
        MainLogger.Error(errorMessage);
        return Error.New(errorMessage);
    }

    /// <summary>
    /// Меняет запрос
    /// </summary>
    /// <param name="modifyRequestAction">Действие по изменению запроса</param>
    /// <returns>Билдер</returns>
    private RequestBuilder ModifyRequest(Action modifyRequestAction)
    {
        if (CheckRequestParams())
            return this;

        _ = Prelude
            .Try(modifyRequestAction)
            .Try()
            .IfFail(() =>
                MainLogger.Error("An exception occurred during the request modification"));
        
        return this;
    }

    /// <summary>
    /// Проверяет обязательные параметры на пустоту
    /// </summary>
    /// <returns></returns>
    [MemberNotNullWhen(true, nameof(RequestMessage), nameof(UriBuilder), nameof(Query))]
    private bool CheckRequestParams() => RequestMessage is not null && UriBuilder is not null && Query is not null;
}