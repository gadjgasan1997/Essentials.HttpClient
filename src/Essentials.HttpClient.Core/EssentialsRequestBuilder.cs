using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using System.Text;
using Essentials.HttpClient.Models;
using Essentials.HttpClient.Models.Implementations;
using LanguageExt;
using LanguageExt.Common;
using static Essentials.HttpClient.Dictionaries.KnownAuthenticationSchemes;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient;

/// <summary>
/// Билдер создания запроса
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class EssentialsRequestBuilder
{
    /// <summary>
    /// Таймаут запроса, необязательный
    /// </summary>
    private TimeSpan? _timeout;

    /// <summary>
    /// Кодировка содержимого запроса
    /// </summary>
    private Encoding? _encoding;
    
    /// <summary>
    /// Сообщение запроса
    /// </summary>
    public HttpRequestMessage? RequestMessage { get; private init; }
    
    private EssentialsRequestBuilder() { }
    
    /// <summary>
    /// Создает экземпляр билдера
    /// </summary>
    /// <param name="uri">Адрес запроса</param>
    public static EssentialsRequestBuilder CreateBuilder(Uri uri)
    {
        if (Uri.IsWellFormedUriString(uri.ToString(), UriKind.RelativeOrAbsolute))
        {
            return new EssentialsRequestBuilder
            {
                RequestMessage = new HttpRequestMessage
                {
                    RequestUri = uri
                }
            };
        }

        // TODO Log
        return new EssentialsRequestBuilder();
    }

    /// <summary>
    /// Создает экземпляр билдера
    /// </summary>
    /// <param name="validation">Адрес запроса</param>
    /// <returns></returns>
    public static EssentialsRequestBuilder CreateBuilder(Validation<Error, Uri> validation) =>
        validation.Match(CreateBuilder, _ => new EssentialsRequestBuilder());

    /// <summary>
    /// Добавляет заголовок к запросу
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="value">Значение заголовка</param>
    /// <returns>Билдер</returns>
    public EssentialsRequestBuilder WithHeader(string name, string value) => ModifyRequest(() => AddHeader(name, value));
    
    /// <summary>
    /// Добавляет заголовок к запросу, если его значение не пустое
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="value">Значение заголовка</param>
    /// <returns>Билдер</returns>
    public EssentialsRequestBuilder WithNotEmptyHeader(string name, string? value)
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
    public EssentialsRequestBuilder WithHeaders(IEnumerable<(string, string?)> headers)
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
    public EssentialsRequestBuilder WithNotEmptyHeaders(params (string, string?)[] headers)
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
    /// Устанавливает таймаут запроса
    /// </summary>
    /// <param name="timeout">Таймаут</param>
    /// <returns></returns>
    public EssentialsRequestBuilder SetTimeout(TimeSpan timeout) => ModifyRequest(() => _timeout = timeout);
    
    /// <summary>
    /// Устанавливает кодировку содержимого запроса
    /// </summary>
    /// <param name="encoding">Кодировка</param>
    /// <returns></returns>
    public EssentialsRequestBuilder SetEncoding(Encoding encoding) => ModifyRequest(() => _encoding = encoding);
    
    /// <summary>
    /// Настраивает Basic авторизацию
    /// </summary>
    /// <param name="userName">Логин</param>
    /// <param name="password">Пароль</param>
    /// <returns>Билдер</returns>
    public EssentialsRequestBuilder WithBasicAuthentication(string userName, string password)
    {
        var authenticationString = Try(() => Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}")))
            .Try()
            .Match<string>(
                Succ: @string => @string,
                Fail: exception =>
                {
                    // TODO Log
                    
                    return string.Empty;
                });

        return WithAuthentication(BASIC, authenticationString);
    }
    
    /// <summary>
    /// Настраивает авторизацию на Jwt токенах
    /// </summary>
    /// <param name="token">Токен</param>
    /// <returns>Билдер</returns>
    public EssentialsRequestBuilder WithJwtAuthentication(string token) => WithAuthentication(JWT, token);
    
    /// <summary>
    /// Настраивает авторизацию
    /// </summary>
    /// <param name="scheme">Схема авторизации</param>
    /// <param name="parameter">Параметр</param>
    /// <returns>Билдер</returns>
    public EssentialsRequestBuilder WithAuthentication(string scheme, string parameter)
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
    public Validation<Error, IEssentialsHttpRequest> Build(string? clientName = null)
    {
        // TODO Rename default client
        return Try(() => BuildPrivate(clientName ?? nameof(EssentialsRequestBuilder)))
            .Try()
            .Match(
                Succ: validation => validation,
                Fail: exception =>
                {
                    // TODO Log
                    return Error.New(exception);
                });
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns>Запрос</returns>
    public Task<Validation<Error, IEssentialsHttpRequest>> BuildAsync(string? clientName = null) =>
        Build(clientName).AsTask();

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <typeparam name="TClient">Тип клиента</typeparam>
    /// <returns>Запрос</returns>
    public Validation<Error, IEssentialsHttpRequest> Build<TClient>() => Build(typeof(TClient).Name);

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <typeparam name="TClient">Тип клиента</typeparam>
    /// <returns>Запрос</returns>
    public Task<Validation<Error, IEssentialsHttpRequest>> BuildAsync<TClient>() => Build<TClient>().AsTask();

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns>Запрос</returns>
    private Validation<Error, IEssentialsHttpRequest> BuildPrivate(string clientName)
    {
        if (RequestMessage is null)
        {
            const string errorMessage = "Не инициализированы обязательные параметры запроса. Запрос не будет создан";

            // TODO Log
            return Fail<Error, IEssentialsHttpRequest>(errorMessage);
        }

        // TODO Log Trace
        return new EssentialsHttpRequest(
            clientName,
            RequestMessage,
            _timeout,
            _encoding ?? Encoding.UTF8);
    }

    /// <summary>
    /// Меняет запрос
    /// </summary>
    /// <param name="modifyRequestAction">Действие по изменению запроса</param>
    /// <returns>Билдер</returns>
    private EssentialsRequestBuilder ModifyRequest(Action modifyRequestAction)
    {
        if (RequestMessage is null)
            return this;

        // TODO Log if fail
        _ = Try(() =>
            {
                modifyRequestAction();
                return this;
            })
            .Try()
            .IfFail(_ => { });
        
        return this;
    }
}