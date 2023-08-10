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
    /// <returns></returns>
    IDisposable? StartRequestTimer(string clientName);

    /// <summary>
    /// Инкрементирует метрику отправки Http запроса
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <param name="httpMethod">Http метод</param>
    void HttpRequestSent(string clientName, HttpMethod httpMethod);

    /// <summary>
    /// Инкрементирует метрику успешной отправки Http запроса
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <param name="httpMethod">Http метод</param>
    void HttpRequestSuccessSent(string clientName, HttpMethod httpMethod);

    /// <summary>
    /// Инкрементирует метрику ошибочной отправки Http запроса
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <param name="httpMethod">Http метод</param>
    void HttpRequestErrorSent(string clientName, HttpMethod httpMethod);
}