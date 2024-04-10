namespace Essentials.HttpClient.Metrics.Dictionaries;

/// <summary>
/// Изветсные теги для метрик
/// </summary>
internal static class KnownMetricsTags
{
    /// <summary>
    /// Тег с названием http клиента
    /// </summary>
    public const string CLIENT_NAME = "client_name";

    /// <summary>
    /// Тег с Id типа запроса
    /// </summary>
    public const string REQUEST_TYPE_ID = "request_type_id";
}