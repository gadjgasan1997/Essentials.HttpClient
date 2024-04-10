using System.Net.Http.Headers;
using App.Metrics;
using LanguageExt;
using Essentials.HttpClient.Events;
using Essentials.Utils.Extensions;

namespace Essentials.HttpClient;

/// <summary>
/// Http запрос
/// </summary>
public interface IRequest
{
    /// <summary>
    /// Id запроса
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// Название Http клиента, с помощью которого отправляется запрос
    /// </summary>
    string ClientName { get; }
    
    /// <summary>
    /// Id типа запроса
    /// </summary>
    Option<string> TypeId { get; }
    
    /// <summary>
    /// Uri запроса
    /// </summary>
    Uri Uri { get; }
    
    /// <summary>
    /// Заголовок с типом медиа
    /// </summary>
    Option<MediaTypeHeaderValue> MediaType { get; internal set; }
    
    /// <summary>
    /// Таймаут запроса
    /// </summary>
    Option<TimeSpan> Timeout { get; }
    
    /// <summary>
    /// Опции метрик
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    Option<RequestMetricsOptions> MetricsOptions { get; }
    
    /// <summary>
    /// Список действий по изменению запроса
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    IEnumerable<Action<HttpRequestMessage>> ModifyRequestActions { get; }
    
    /// <summary>
    /// Обработчики событий запроса
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    Dictionary<string, Handler> EventsHandlers { get; }
}

/// <summary>
/// Опции метрик для запроса
/// </summary>
/// <param name="Name">Название метрики</param>
/// <param name="Tags">Теги</param>
public readonly record struct RequestMetricsOptions(string Name, MetricTags Tags)
{
    /// <summary>
    /// Название метрики
    /// </summary>
    public string Name { get; } = Name.CheckNotNullOrEmpty();

    /// <summary>
    /// Список тегов
    /// </summary>
    public MetricTags Tags { get; } = Tags.CheckNotNull();
}