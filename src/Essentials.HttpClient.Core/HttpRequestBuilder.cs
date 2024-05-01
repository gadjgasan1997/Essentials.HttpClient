using System.Text;
using LanguageExt;
using LanguageExt.Common;
using System.Net.Http.Headers;
using System.Diagnostics.CodeAnalysis;
using Essentials.Utils.Extensions;
using Essentials.Functional.Extensions;
using Essentials.HttpClient.Cache;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Models;
using Essentials.HttpClient.RequestsInterception;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Dictionaries.KnownAuthenticationSchemes;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable ConvertToLambdaExpression
// ReSharper disable ConvertIfStatementToReturnStatement

namespace Essentials.HttpClient;

/// <summary>
/// Билдер создания запроса
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class HttpRequestBuilder
{
    /// <summary>
    /// Id запроса
    /// </summary>
    internal string? Id { get; set; }
    
    /// <summary>
    /// Id типа запроса
    /// </summary>
    internal string? TypeId { get; set; }
    
    /// <summary>
    /// Адрес запроса
    /// </summary>
    internal Uri Uri { get; }
    
    /// <summary>
    /// Заголовок с типом содержимого запроса
    /// </summary>
    internal MediaTypeHeaderValue? MediaType { get; set; }

    /// <summary>
    /// Таймаут запроса
    /// </summary>
    internal TimeSpan? Timeout { get; set; }

    /// <summary>
    /// Опции метрик
    /// </summary>
    internal RequestMetricsOptions? MetricsOptions { get; set; }
    
    /// <summary>
    /// Действия, которые будут выполняться над запросом
    /// </summary>
    internal List<Action<HttpRequestMessage>> Actions { get; } = new();

    /// <summary>
    /// Список перехватчиков запросов
    /// </summary>
    internal List<Type> Interceptors { get; } = [];
    
    /// <summary>
    /// Список глобальных перехватчиков запросов, которые необходимо игнорировать
    /// </summary>
    internal List<Type> IgnoredGlobalInterceptors { get; } = [];

    /// <summary>
    /// Обработчики событий запроса
    /// </summary>
    internal Dictionary<string, Handler> EventsHandlers { get; } = new();
    
    private HttpRequestBuilder(Uri uri) => Uri = uri.CheckNotNull();
    
    /// <summary>
    /// Создает билдер
    /// </summary>
    /// <param name="uri">Адрес запроса</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> CreateBuilder(Uri uri)
    {
        return () =>
        {
            uri.Validate();
            return new HttpRequestBuilder(uri);
        };
    }

    /// <summary>
    /// Создает билдер
    /// </summary>
    /// <param name="validation">Адрес запроса</param>
    /// <returns>Билдер</returns>
    public static Try<HttpRequestBuilder> CreateBuilder(Validation<Error, Uri> validation)
    {
        return () =>
        {
            return validation
                .Match(
                    CreateBuilder,
                    Fail: errors => throw errors.ToAggregateException())
                .Try();
        };
    }
    
    /// <summary>
    /// Возвращает запрос из кеша по Id или создает новый
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="creator">Делегат создания запроса</param>
    /// <returns></returns>
    public static Validation<Error, IRequest> GetFromCacheOrCreate(
        string id,
        Func<Validation<Error, IRequest>> creator)
    {
        return string.IsNullOrWhiteSpace(id)
            ? Error.New("Id запроса для кеширования не должен быть пустым")
            : RequestsCacheService.GetFromCacheOrCreate(id, creator);
    }
    
    /// <summary>
    /// Возвращает запрос из кеша по Id или создает новый
    /// </summary>
    /// <param name="id">Id</param>
    /// <param name="creator">Делегат создания запроса</param>
    /// <returns></returns>
    public static Task<Validation<Error, IRequest>> GetFromCacheOrCreateAsync(
        string id,
        Func<Task<Validation<Error, IRequest>>> creator)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Fail<Error, IRequest>("Id запроса для кеширования не должен быть пустым").AsTask();
        
        return RequestsCacheService.GetFromCacheOrCreate(id, () => creator().Result).AsTask();
    }
    
    /// <summary>
    /// Устанавливает Id для запроса
    /// </summary>
    /// <param name="requestId">Id запроса</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> WithRequestId(string requestId)
    {
        return () =>
        {
            if (string.IsNullOrWhiteSpace(requestId))
                return this;

            Id = requestId;
            return this;
        };
    }

    /// <summary>
    /// Устанавливает Id типа запроса
    /// </summary>
    /// <param name="typeId">Id типа</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> SetTypeId(string typeId)
    {
        return () =>
        {
            if (string.IsNullOrWhiteSpace(typeId))
                return this;

            TypeId = typeId;
            return this;
        };
    }

    /// <summary>
    /// Устанавливает таймаут запроса
    /// </summary>
    /// <param name="timeout">Таймаут</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> SetTimeout(TimeSpan timeout)
    {
        return () =>
        {
            Timeout = timeout;
            return this;
        };
    }

    /// <summary>
    /// Устанавливает метрики для запроса
    /// </summary>
    /// <param name="metricsOptions">Опции метрик</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> WithRequestMetrics(RequestMetricsOptions metricsOptions)
    {
        return () =>
        {
            MetricsOptions = metricsOptions;
            return this;
        };
    }

    /// <summary>
    /// Добавляет заголовок к запросу
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="values">Значения заголовка</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> WithHeader(string name, params string?[] values)
    {
        return () =>
        {
            if (string.IsNullOrWhiteSpace(name))
                return this;

            Actions.Add(message => message.Headers.Add(name, values));
            return this;
        };
    }

    /// <summary>
    /// Добавляет заголовок к запросу
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="values">Значения заголовка</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> WithNotEmptyHeader(string name, params string?[] values)
    {
        return () =>
        {
            if (string.IsNullOrWhiteSpace(name))
                return this;

            Actions.Add(message =>
            {
                message.Headers.Add(
                    name,
                    values.Where(@string => !string.IsNullOrWhiteSpace(@string)));
            });
            
            return this;
        };
    }

    /// <summary>
    /// Добавляет заголовки к запросу
    /// </summary>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> WithHeaders(params (string Name, IEnumerable<string?> Value)[] headers)
    {
        return () =>
        {
            foreach (var tuple in headers)
            {
                if (string.IsNullOrWhiteSpace(tuple.Name))
                    continue;
                
                Actions.Add(message => message.Headers.Add(tuple.Name, tuple.Value));
            }
            
            return this;
        };
    }

    /// <summary>
    /// Добавляет заголовки к запросу, если их значения не пустые
    /// </summary>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> WithNotEmptyHeaders(params (string Name, IEnumerable<string?> Value)[] headers)
    {
        return () =>
        {
            foreach (var tuple in headers)
            {
                if (string.IsNullOrWhiteSpace(tuple.Name))
                    continue;
                    
                Actions.Add(message =>
                {
                    message.Headers.Add(
                        tuple.Name,
                        tuple.Value.Where(@string => !string.IsNullOrWhiteSpace(@string)));
                });
            }

            return this;
        };
    }

    /// <summary>
    /// Устанавливает заголовок с типом содержимого запроса
    /// </summary>
    /// <param name="mediaType">Тип содержимого</param>
    /// <param name="encoding">Кодировка</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> SetMediaTypeHeader(string mediaType, Encoding? encoding = null)
    {
        return () =>
        {
            if (string.IsNullOrWhiteSpace(mediaType))
                return this;
            
            encoding ??= Encoding.Default;
            MediaType = new MediaTypeHeaderValue(mediaType)
            {
                CharSet = encoding.WebName
            };

            return this;
        };
    }

    /// <summary>
    /// Настраивает авторизацию
    /// </summary>
    /// <param name="scheme">Схема авторизации</param>
    /// <param name="parameter">Параметр</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> WithAuthentication(string scheme, string parameter)
    {
        return () =>
        {
            if (string.IsNullOrWhiteSpace(scheme) || string.IsNullOrWhiteSpace(parameter))
                return this;
            
            Actions.Add(message =>
            {
                message.Headers.Authorization = new AuthenticationHeaderValue(scheme, parameter);
            });

            return this;
        };
    }

    /// <summary>
    /// Настраивает Basic авторизацию
    /// </summary>
    /// <param name="userName">Логин</param>
    /// <param name="password">Пароль</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> WithBasicAuthentication(string userName, string password)
    {
        return () =>
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                return this;

            var bytes = Encoding.ASCII.GetBytes($"{userName}:{password}");
            var authenticationString = Convert.ToBase64String(bytes);

            return WithAuthentication(BASIC, authenticationString).Try();
        };
    }

    /// <summary>
    /// Настраивает авторизацию на Jwt токенах
    /// </summary>
    /// <param name="token">Токен</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> WithJwtAuthentication(string token) =>
        WithAuthentication(JWT, token);

    /// <summary>
    /// Добавляет интерсептор к запросу
    /// </summary>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> WithInterceptor<TInterceptor>()
        where TInterceptor : IRequestInterceptor
    {
        return () =>
        {
            InterceptorsStorage.CheckInterceptorIsRegistered<TInterceptor>();
            Interceptors.Add(typeof(TInterceptor));
            
            return this;
        };
    }
    
    /// <summary>
    /// Отключает глобальный интерсептор для запроса
    /// </summary>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> DisableGlobalInterceptor<TInterceptor>()
        where TInterceptor : IRequestInterceptor
    {
        return () =>
        {
            IgnoredGlobalInterceptors.Add(typeof(TInterceptor));
            
            return this;
        };
    }

    /// <summary>
    /// Устанавливает обработчик события ошибки сериализации объекта
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> OnSerializeError(Handler handler) =>
        SetHandler(nameof(EventsStorage.OnSerializeError), handler);

    /// <summary>
    /// Устанавливает обработчик события перед отправкой запроса
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> OnBeforeSend(Handler handler) =>
        SetHandler(nameof(EventsStorage.OnBeforeSend), handler);

    /// <summary>
    /// Устанавливает обработчик события успеха отправки запроса
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> OnSuccessSend(Handler handler) =>
        SetHandler(nameof(EventsStorage.OnSuccessSend), handler);

    /// <summary>
    /// Устанавливает обработчик события ошибки отправки запроса
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> OnErrorSend(Handler handler) =>
        SetHandler(nameof(EventsStorage.OnErrorSend), handler);

    /// <summary>
    /// Устанавливает обработчик события ошибочного Http кода ответа
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> OnBadStatusCode(Handler handler) =>
        SetHandler(nameof(EventsStorage.OnBadStatusCode), handler);

    /// <summary>
    /// Устанавливает обработчик события ошибки при чтении содержимого из Http ответа
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> OnErrorReadContent(Handler handler) =>
        SetHandler(nameof(EventsStorage.OnErrorReadContent), handler);

    /// <summary>
    /// Устанавливает обработчик события ошибки десериализации строки ответа
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Билдер</returns>
    public Try<HttpRequestBuilder> OnDeserializeError(Handler handler) =>
        SetHandler(nameof(EventsStorage.OnDeserializeError), handler);
    
    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="clientName">Название http клиента</param>
    /// <returns>Билдер</returns>
    internal Validation<Error, IRequest> Build(string clientName)
    {
        var interceptors = InterceptorsStorage.GetInterceptorsToAttach(
            Interceptors,
            IgnoredGlobalInterceptors);

        var requestId = string.IsNullOrWhiteSpace(Id)
            ? RequestId.CreateDefault()
            : RequestId.CreateManual(Id);

        return Try(() =>
            {
                return new Request(
                    requestId,
                    clientName,
                    TypeId,
                    Uri,
                    MediaType,
                    Optional(Timeout),
                    Optional(MetricsOptions),
                    Actions,
                    interceptors,
                    EventsHandlers);
            })
            .Match(
                Succ: request => request,
                Fail: exception => Fail<Error, IRequest>(exception));
    }

    /// <summary>
    /// Устанавливает обработчик события
    /// </summary>
    /// <param name="eventMame">Название события</param>
    /// <param name="handler">Обработчик</param>
    private Try<HttpRequestBuilder> SetHandler(string eventMame, Handler handler)
    {
        return () =>
        {
            if (string.IsNullOrWhiteSpace(eventMame))
                return this;

            if (handler is null)
                return this;
            
            if (!EventsHandlers.TryAdd(eventMame, handler))
                EventsHandlers[eventMame] = handler;

            return this;
        };
    }
}