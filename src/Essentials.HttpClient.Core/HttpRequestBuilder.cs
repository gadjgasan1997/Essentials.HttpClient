using LanguageExt;
using LanguageExt.Common;
using Essentials.HttpClient.Cache;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Extensions;
using Essentials.Utils.Extensions;
using System.Net.Http.Headers;
using System.Diagnostics.CodeAnalysis;

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
    public string? Id { get; internal set; }
    
    /// <summary>
    /// Id типа запроса
    /// </summary>
    public string? TypeId { get; internal set; }
    
    /// <summary>
    /// Адрес запроса
    /// </summary>
    public Uri Uri { get; }
    
    /// <summary>
    /// Заголовок с типом содержимого запроса
    /// </summary>
    public MediaTypeHeaderValue? MediaType { get; internal set; }

    /// <summary>
    /// Таймаут запроса
    /// </summary>
    public TimeSpan? Timeout { get; internal set; }

    /// <summary>
    /// Опции метрик
    /// </summary>
    public RequestMetricsOptions? MetricsOptions { get; internal set; }
    
    /// <summary>
    /// Действия, которые будут выполняться над запросом
    /// </summary>
    public List<Action<HttpRequestMessage>> Actions { get; } = new();

    /// <summary>
    /// Список перехватчиков запросов
    /// </summary>
    public List<Type> Interceptors { get; } = [];
    
    /// <summary>
    /// Список глобальных перехватчиков запросов, которые необходимо игнорировать
    /// </summary>
    public List<Type> IgnoredGlobalInterceptors { get; } = [];

    /// <summary>
    /// Обработчики событий запроса
    /// </summary>
    public Dictionary<string, Handler> EventsHandlers { get; } = new();
    
    private HttpRequestBuilder(Uri uri) => Uri = uri.CheckNotNull();
    
    /// <summary>
    /// Создает билдер
    /// </summary>
    /// <param name="uri">Адрес запроса</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> CreateBuilder(Uri uri)
    {
        try
        {
            uri.Validate();
            return new HttpRequestBuilder(uri);
        }
        catch (Exception ex)
        {
            return Error.New(ex);
        }
    }

    /// <summary>
    /// Создает билдер
    /// </summary>
    /// <param name="validation">Адрес запроса</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpRequestBuilder> CreateBuilder(Validation<Error, Uri> validation) =>
        validation.Bind(CreateBuilder);
    
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
        {
            return Prelude
                .Fail<Error, IRequest>("Id запроса для кеширования не должен быть пустым")
                .AsTask();
        }
        
        return RequestsCacheService.GetFromCacheOrCreate(id, () => creator().Result).AsTask();
    }

    /// <summary>
    /// Устанавливает обработчик события
    /// </summary>
    /// <param name="eventMame">Название события</param>
    /// <param name="handler">Обработчик</param>
    internal void SetHandler(string eventMame, Handler handler)
    {
        if (string.IsNullOrWhiteSpace(eventMame))
            return;

        if (!EventsHandlers.TryAdd(eventMame, handler))
            EventsHandlers[eventMame] = handler;
    }
}