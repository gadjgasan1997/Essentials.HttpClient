using App.Metrics;
using LanguageExt;

namespace Essentials.HttpClient.Metrics;

/// <summary>
/// Сервис для отдачи метрик
/// </summary>
public interface IMetricsService
{
    /// <summary>
    /// Запускает таймер
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <param name="requestTypeId">Id типа запроса</param>
    /// <returns></returns>
    IDisposable? StartRequestTimer(string clientName, Option<string> requestTypeId);

    /// <summary>
    /// Инкрементирует метрику отправки Http запроса
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <param name="requestTypeId">Id типа запроса</param>
    void HttpRequestSent(string clientName, Option<string> requestTypeId);

    /// <summary>
    /// Инкрементирует метрику успешной отправки Http запроса
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <param name="requestTypeId">Id типа запроса</param>
    void HttpRequestSuccessSent(string clientName, Option<string> requestTypeId);

    /// <summary>
    /// Инкрементирует метрику ошибочной отправки Http запроса
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <param name="requestTypeId">Id типа запроса</param>
    void HttpRequestErrorSent(string clientName, Option<string> requestTypeId);
    
    /// <summary>
    /// Запускает таймер
    /// </summary>
    /// <param name="metricName">Название отдаваемой метрики</param>
    /// <param name="tags">Теги</param>
    /// <returns></returns>
    IDisposable? StartRequestTimer(string metricName, MetricTags tags);
    
    /// <summary>
    /// Инкрементирует метрику отправки Http запроса
    /// </summary>
    /// <param name="metricName">Название отдаваемой метрики</param>
    /// <param name="tags">Теги</param>
    void HttpRequestSent(string metricName, MetricTags tags);

    /// <summary>
    /// Инкрементирует метрику успешной отправки Http запроса
    /// </summary>
    /// <param name="metricName">Название отдаваемой метрики</param>
    /// <param name="tags">Теги</param>
    void HttpRequestSuccessSent(string metricName, MetricTags tags);

    /// <summary>
    /// Инкрементирует метрику ошибочной отправки Http запроса
    /// </summary>
    /// <param name="metricName">Название отдаваемой метрики</param>
    /// <param name="tags">Теги</param>
    void HttpRequestErrorSent(string metricName, MetricTags tags);
}