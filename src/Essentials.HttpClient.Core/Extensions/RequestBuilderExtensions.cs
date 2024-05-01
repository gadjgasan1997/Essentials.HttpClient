using System.Text;
using LanguageExt;
using LanguageExt.Common;
using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.RequestsInterception;

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
    /// <param name="try"></param>
    /// <param name="requestId">Id запроса</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> WithRequestId(
        this Try<HttpRequestBuilder> @try,
        string requestId)
    {
        return @try.Bind(builder => builder.WithRequestId(requestId));
    }

    /// <summary>
    /// Устанавливает Id типа запроса
    /// </summary>
    /// <param name="try"></param>
    /// <param name="typeId">Id типа</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> SetTypeId(
        this Try<HttpRequestBuilder> @try,
        string typeId)
    {
        return @try.Bind(builder => builder.SetTypeId(typeId));
    }

    /// <summary>
    /// Устанавливает таймаут запроса
    /// </summary>
    /// <param name="try"></param>
    /// <param name="timeout">Таймаут</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> SetTimeout(
        this Try<HttpRequestBuilder> @try,
        TimeSpan timeout)
    {
        return @try.Bind(builder => builder.SetTimeout(timeout));
    }

    /// <summary>
    /// Устанавливает метрики для запроса
    /// </summary>
    /// <param name="try"></param>
    /// <param name="metricsOptions">Опции метрик</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> WithRequestMetrics(
        this Try<HttpRequestBuilder> @try,
        RequestMetricsOptions metricsOptions)
    {
        return @try.Bind(builder => builder.WithRequestMetrics(metricsOptions));
    }
   
    /// <summary>
    /// Добавляет заголовок к запросу
    /// </summary>
    /// <param name="try"></param>
    /// <param name="name">Название заголовка</param>
    /// <param name="values">Значения заголовка</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> WithHeader(
        this Try<HttpRequestBuilder> @try,
        string name,
        params string?[] values)
    {
        return @try.Bind(builder => builder.WithHeader(name, values));
    }
    
    /// <summary>
    /// Добавляет заголовок к запросу, если его значение не пустое
    /// </summary>
    /// <param name="try"></param>
    /// <param name="name">Название заголовка</param>
    /// <param name="values">Значения заголовка</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> WithNotEmptyHeader(
        this Try<HttpRequestBuilder> @try,
        string name,
        params string?[] values)
    {
        return @try.Bind(builder => builder.WithNotEmptyHeader(name, values));
    }
    
    /// <summary>
    /// Добавляет заголовки к запросу
    /// </summary>
    /// <param name="try"></param>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> WithHeaders(
        this Try<HttpRequestBuilder> @try,
        params (string Name, IEnumerable<string?> Value)[] headers)
    {
        return @try.Bind(builder => builder.WithHeaders(headers));
    }
    
    /// <summary>
    /// Добавляет заголовки к запросу, если их значения не пустые
    /// </summary>
    /// <param name="try"></param>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> WithNotEmptyHeaders(
        this Try<HttpRequestBuilder> @try,
        params (string Name, IEnumerable<string?> Value)[] headers)
    {
        return @try.Bind(builder => builder.WithNotEmptyHeaders(headers));
    }

    /// <summary>
    /// Устанавливает заголовок с типом содержимого запроса
    /// </summary>
    /// <param name="try"></param>
    /// <param name="mediaType">Тип содержимого</param>
    /// <param name="encoding">Кодировка</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> SetMediaTypeHeader(
        this Try<HttpRequestBuilder> @try,
        string mediaType,
        Encoding? encoding = null)
    {
        return @try.Bind(builder => builder.SetMediaTypeHeader(mediaType, encoding));
    }

    /// <summary>
    /// Настраивает авторизацию
    /// </summary>
    /// <param name="try"></param>
    /// <param name="scheme">Схема авторизации</param>
    /// <param name="parameter">Параметр</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> WithAuthentication(
        this Try<HttpRequestBuilder> @try,
        string scheme,
        string parameter)
    {
        return @try.Bind(builder => builder.WithAuthentication(scheme, parameter));
    }

    /// <summary>
    /// Настраивает Basic авторизацию
    /// </summary>
    /// <param name="try"></param>
    /// <param name="userName">Логин</param>
    /// <param name="password">Пароль</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> WithBasicAuthentication(
        this Try<HttpRequestBuilder> @try,
        string userName,
        string password)
    {
        return @try.Bind(builder => builder.WithBasicAuthentication(userName, password));
    }

    /// <summary>
    /// Настраивает авторизацию на Jwt токенах
    /// </summary>
    /// <param name="try"></param>
    /// <param name="token">Токен</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> WithJwtAuthentication(
        this Try<HttpRequestBuilder> @try,
        string token)
    {
        return @try.Bind(builder => builder.WithJwtAuthentication(token));
    }

    /// <summary>
    /// Добавляет интерсептор к запросу
    /// </summary>
    /// <param name="try"></param>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> WithInterceptor<TInterceptor>(
        this Try<HttpRequestBuilder> @try)
        where TInterceptor : IRequestInterceptor
    {
        return @try.Bind(builder => builder.WithInterceptor<TInterceptor>());
    }

    /// <summary>
    /// Отключает глобальный интерсептор для запроса
    /// </summary>
    /// <param name="try"></param>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> DisableGlobalInterceptor<TInterceptor>(
        this Try<HttpRequestBuilder> @try)
        where TInterceptor : IRequestInterceptor
    {
        return @try.Bind(builder => builder.DisableGlobalInterceptor<TInterceptor>());
    }

    /// <summary>
    /// Устанавливает обработчик события ошибки сериализации объекта
    /// </summary>
    /// <param name="try"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> OnSerializeError(
        this Try<HttpRequestBuilder> @try,
        Handler handler)
    {
        return @try.Bind(builder => builder.OnSerializeError(handler));
    }

    /// <summary>
    /// Устанавливает обработчик события перед отправкой запроса
    /// </summary>
    /// <param name="try"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> OnBeforeSend(
        this Try<HttpRequestBuilder> @try,
        Handler handler)
    {
        return @try.Bind(builder => builder.OnBeforeSend(handler));
    }

    /// <summary>
    /// Устанавливает обработчик события успеха отправки запроса
    /// </summary>
    /// <param name="try"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> OnSuccessSend(
        this Try<HttpRequestBuilder> @try,
        Handler handler)
    {
        return @try.Bind(builder => builder.OnSuccessSend(handler));
    }

    /// <summary>
    /// Устанавливает обработчик события ошибки отправки запроса
    /// </summary>
    /// <param name="try"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> OnErrorSend(
        this Try<HttpRequestBuilder> @try,
        Handler handler)
    {
        return @try.Bind(builder => builder.OnErrorSend(handler));
    }

    /// <summary>
    /// Устанавливает обработчик события ошибочного Http кода ответа
    /// </summary>
    /// <param name="try"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> OnBadStatusCode(
        this Try<HttpRequestBuilder> @try,
        Handler handler)
    {
        return @try.Bind(builder => builder.OnBadStatusCode(handler));
    }

    /// <summary>
    /// Устанавливает обработчик события ошибки при чтении содержимого из Http ответа
    /// </summary>
    /// <param name="try"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> OnErrorReadContent(
        this Try<HttpRequestBuilder> @try,
        Handler handler)
    {
        return @try.Bind(builder => builder.OnErrorReadContent(handler));
    }

    /// <summary>
    /// Устанавливает обработчик события ошибки десериализации строки ответа
    /// </summary>
    /// <param name="try"></param>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> OnDeserializeError(
        this Try<HttpRequestBuilder> @try,
        Handler handler)
    {
        return @try.Bind(builder => builder.OnDeserializeError(handler));
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="try"></param>
    /// <param name="clientName">Название http клиента</param>
    /// <returns>Запрос</returns>
    public static Validation<Error, IRequest> Build(
        this Try<HttpRequestBuilder> @try,
        string? clientName = null)
    {
        return @try
            .Map(builder =>
                builder.Build(
                    string.IsNullOrWhiteSpace(clientName)
                        ? LogManager.MainLogger.Name
                        : clientName))
            .ToValidation(Error.New)
            .Bind(data => data);
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="try"></param>
    /// <param name="clientName">Название http клиента</param>
    /// <returns>Запрос</returns>
    public static Task<Validation<Error, IRequest>> BuildAsync(
        this Try<HttpRequestBuilder> @try,
        string? clientName = null)
    {
        return @try.Build(clientName).AsTask();
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="try"></param>
    /// <typeparam name="TClient">Тип клиента</typeparam>
    /// <returns>Запрос</returns>
    public static Validation<Error, IRequest> Build<TClient>(
        this Try<HttpRequestBuilder> @try)
    {
        return @try.Build(typeof(TClient).Name);
    }

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="try"></param>
    /// <typeparam name="TClient">Тип клиента</typeparam>
    /// <returns>Запрос</returns>
    public static Task<Validation<Error, IRequest>> BuildAsync<TClient>(
        this Try<HttpRequestBuilder> @try)
    {
        return @try.Build<TClient>().AsTask();
    }
}