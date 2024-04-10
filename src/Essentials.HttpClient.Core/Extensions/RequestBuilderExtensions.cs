using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Net.Http.Headers;
using LanguageExt;
using LanguageExt.Common;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.Models;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Dictionaries.KnownAuthenticationSchemes;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="HttpUriBuilder" />
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class RequestBuilderExtensions
{
    /// <summary>
    /// Устанавливает Id для запроса
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="requestId">Id запроса</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> WithRequestId(
        this Validation<Error, HttpRequestBuilder> validation,
        string requestId)
    {
        return string.IsNullOrWhiteSpace(requestId)
            ? validation
            : validation.ModifyRequest(builder => () => builder.Id = requestId);
    }
    
    /// <summary>
    /// Добавляет заголовок к запросу
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="name">Название заголовка</param>
    /// <param name="values">Значения заголовка</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> WithHeader(
        this Validation<Error, HttpRequestBuilder> validation,
        string name,
        params string?[] values)
    {
        if (string.IsNullOrWhiteSpace(name))
            return validation;

        return validation.ModifyRequest(builder =>
            () =>
            {
                builder.Actions.Add(message => message.Headers.Add(name, values));
            });
    }
    
    /// <summary>
    /// Добавляет заголовок к запросу, если его значение не пустое
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="name">Название заголовка</param>
    /// <param name="values">Значения заголовка</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> WithNotEmptyHeader(
        this Validation<Error, HttpRequestBuilder> validation,
        string name,
        params string?[] values)
    {
        if (string.IsNullOrWhiteSpace(name))
            return validation;

        return validation.ModifyRequest(builder =>
            () =>
            {
                builder.Actions.Add(message =>
                {
                    message.Headers.Add(name, values.Where(@string => !string.IsNullOrWhiteSpace(@string)));
                });
            });
    }
    
    /// <summary>
    /// Добавляет заголовки к запросу
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> WithHeaders(
        this Validation<Error, HttpRequestBuilder> validation,
        params (string Name, IEnumerable<string?> Value)[] headers)
    {
        return validation.ModifyRequest(builder =>
            () =>
            {
                foreach (var tuple in headers)
                {
                    if (string.IsNullOrWhiteSpace(tuple.Name))
                        continue;
                    
                    builder.Actions.Add(message => message.Headers.Add(tuple.Name, tuple.Value));
                }
            });
    }
    
    /// <summary>
    /// Добавляет заголовки к запросу, если их значения не пустые
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> WithNotEmptyHeaders(
        this Validation<Error, HttpRequestBuilder> validation,
        params (string Name, IEnumerable<string?> Value)[] headers)
    {
        return validation.ModifyRequest(builder =>
            () =>
            {
                foreach (var tuple in headers)
                {
                    if (string.IsNullOrWhiteSpace(tuple.Name))
                        continue;
                    
                    builder.Actions.Add(message =>
                    {
                        message.Headers.Add(
                            tuple.Name,
                            tuple.Value.Where(@string => !string.IsNullOrWhiteSpace(@string)));
                    });
                }
            });
    }

    /// <summary>
    /// Устанавливает заголовок с типом содержимого запроса
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="mediaType">Тип содержимого</param>
    /// <param name="encoding">Кодировка</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> SetMediaTypeHeader(
        this Validation<Error, HttpRequestBuilder> validation,
        string mediaType,
        Encoding? encoding = null)
    {
        return validation.ModifyRequest(builder =>
            () =>
            {
                if (string.IsNullOrWhiteSpace(mediaType))
                    return;
            
                encoding ??= Encoding.Default;
                builder.MediaType = new MediaTypeHeaderValue(mediaType)
                {
                    CharSet = encoding.WebName
                };
            });
    }

    /// <summary>
    /// Настраивает Basic авторизацию
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="userName">Логин</param>
    /// <param name="password">Пароль</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> WithBasicAuthentication(
        this Validation<Error, HttpRequestBuilder> validation,
        string userName,
        string password)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            return validation;
        
        string authenticationString;
        try
        {
            var bytes = Encoding.ASCII.GetBytes($"{userName}:{password}");
            authenticationString = Convert.ToBase64String(bytes);
        }
        catch (Exception ex)
        {
            return Error.New(ex);
        }
        
        return validation.WithAuthentication(BASIC, authenticationString);
    }

    /// <summary>
    /// Настраивает авторизацию на Jwt токенах
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="token">Токен</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> WithJwtAuthentication(
        this Validation<Error, HttpRequestBuilder> validation,
        string token)
    {
        return validation.WithAuthentication(JWT, token);
    }

    /// <summary>
    /// Настраивает авторизацию
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="scheme">Схема авторизации</param>
    /// <param name="parameter">Параметр</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> WithAuthentication(
        this Validation<Error, HttpRequestBuilder> validation,
        string scheme,
        string parameter)
    {
        if (string.IsNullOrWhiteSpace(scheme) || string.IsNullOrWhiteSpace(parameter))
            return validation;
        
        return validation.ModifyRequest(builder =>
            () =>
            {
                builder.Actions.Add(message =>
                    message.Headers.Authorization = new AuthenticationHeaderValue(scheme, parameter));
            });
    }

    /// <summary>
    /// Устанавливает таймаут запроса
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="timeout">Таймаут</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> SetTimeout(
        this Validation<Error, HttpRequestBuilder> validation,
        TimeSpan timeout)
    {
        return validation.ModifyRequest(builder => () => builder.Timeout = timeout);
    }

    /// <summary>
    /// Устанавливает метрики для запроса
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="metricsOptions">Опции метрик</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> WithRequestMetrics(
        this Validation<Error, HttpRequestBuilder> validation,
        RequestMetricsOptions metricsOptions)
    {
        return validation.ModifyRequest(builder => () => builder.MetricsOptions = metricsOptions);
    }

    /// <summary>
    /// Устанавливает Id типа запроса
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="typeId">Id типа</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> SetTypeId(
        this Validation<Error, HttpRequestBuilder> validation,
        string typeId)
    {
        return string.IsNullOrWhiteSpace(typeId)
            ? validation
            : validation.ModifyRequest(builder => () => builder.TypeId = typeId);
    }

    /// <summary>
    /// Устанавливает обработчик события ошибки сериализации объекта
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> OnSerializeError(
        this Validation<Error, HttpRequestBuilder> validation,
        Handler handler)
    {
        return validation.ModifyRequest(builder =>
            () =>
            {
                builder.SetHandler(nameof(EventsPublisher.OnSerializeError), handler);
            });
    }

    /// <summary>
    /// Устанавливает обработчик события перед отправкой запроса
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> OnBeforeSend(
        this Validation<Error, HttpRequestBuilder> validation,
        Handler handler)
    {
        return validation.ModifyRequest(builder =>
            () =>
            {
                builder.SetHandler(nameof(EventsPublisher.OnBeforeSend), handler);
            });
    }

    /// <summary>
    /// Устанавливает обработчик события успеха отправки запроса
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> OnSuccessSend(
        this Validation<Error, HttpRequestBuilder> validation,
        Handler handler)
    {
        return validation.ModifyRequest(builder =>
            () =>
            {
                builder.SetHandler(nameof(EventsPublisher.OnSuccessSend), handler);
            });
    }

    /// <summary>
    /// Устанавливает обработчик события ошибки отправки запроса
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> OnErrorSend(
        this Validation<Error, HttpRequestBuilder> validation,
        Handler handler)
    {
        return validation.ModifyRequest(builder =>
            () =>
            {
                builder.SetHandler(nameof(EventsPublisher.OnErrorSend), handler);
            });
    }

    /// <summary>
    /// Устанавливает обработчик события ошибочного Http кода ответа
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> OnBadStatusCode(
        this Validation<Error, HttpRequestBuilder> validation,
        Handler handler)
    {
        return validation.ModifyRequest(builder =>
            () =>
            {
                builder.SetHandler(nameof(EventsPublisher.OnBadStatusCode), handler);
            });
    }

    /// <summary>
    /// Устанавливает обработчик события ошибки при чтении содержимого из Http ответа
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> OnErrorReadContent(
        this Validation<Error, HttpRequestBuilder> validation,
        Handler handler)
    {
        return validation.ModifyRequest(builder =>
            () =>
            {
                builder.SetHandler(nameof(EventsPublisher.OnErrorReadContent), handler);
            });
    }

    /// <summary>
    /// Устанавливает обработчик события ошибки десериализации строки ответа
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> OnDeserializeError(
        this Validation<Error, HttpRequestBuilder> validation,
        Handler handler)
    {
        return validation.ModifyRequest(builder =>
            () =>
            {
                builder.SetHandler(nameof(EventsPublisher.OnDeserializeError), handler);
            });
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="clientName">Название http клиента</param>
    /// <returns>Запрос</returns>
    public static Validation<Error, IRequest> Build(
        this Validation<Error, HttpRequestBuilder> validation,
        string? clientName = null)
    {
        return validation.Bind(builder =>
            BuildPrivate(
                builder,
                string.IsNullOrWhiteSpace(clientName) ? LogManager.MainLogger.Name : clientName));
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="clientName">Название http клиента</param>
    /// <returns>Запрос</returns>
    public static Task<Validation<Error, IRequest>> BuildAsync(
        this Validation<Error, HttpRequestBuilder> validation,
        string? clientName = null)
    {
        return validation.Build(clientName).AsTask();
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="validation"></param>
    /// <typeparam name="TClient">Тип клиента</typeparam>
    /// <returns>Запрос</returns>
    public static Validation<Error, IRequest> Build<TClient>(
        this Validation<Error, HttpRequestBuilder> validation)
    {
        return validation.Build(typeof(TClient).Name);
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="validation"></param>
    /// <typeparam name="TClient">Тип клиента</typeparam>
    /// <returns>Запрос</returns>
    public static Task<Validation<Error, IRequest>> BuildAsync<TClient>(
        this Validation<Error, HttpRequestBuilder> validation)
    {
        return validation.Build<TClient>().AsTask();
    }

    /// <summary>
    /// Изменяет запрос
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="func">Действие по изменению запроса</param>
    /// <returns>Билдер</returns>
    private static Validation<Error, HttpRequestBuilder> ModifyRequest(
        this Validation<Error, HttpRequestBuilder> validation,
        Func<HttpRequestBuilder, Action> func)
    {
        return validation.Bind(builder =>
        {
            return Try(() =>
                {
                    var action = func(builder);
                    action();
                    return builder;
                })
                .Try()
                .Match(
                    Succ: uriBuilder => uriBuilder,
                    Fail: exception => Fail<Error, HttpRequestBuilder>(
                        Error.New("Во время изменения запроса произошла ошибка", exception)));
        });
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="builder">Билдер</param>
    /// <param name="clientName">Название http клиента</param>
    /// <returns>Билдер</returns>
    private static Validation<Error, IRequest> BuildPrivate(
        HttpRequestBuilder builder,
        string clientName)
    {
        return Try(() =>
                new Request(
                    builder.Id ?? Guid.NewGuid().ToString(),
                    clientName,
                    builder.TypeId,
                    builder.Uri,
                    builder.MediaType,
                    Optional(builder.Timeout),
                    Optional(builder.MetricsOptions),
                    builder.Actions,
                    builder.EventsHandlers))
            .Try()
            .Match(
                Succ: request => request,
                Fail: exception => Fail<Error, IRequest>(exception));
    }
}