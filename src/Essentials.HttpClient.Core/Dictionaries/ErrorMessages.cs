namespace Essentials.HttpClient.Dictionaries;

/// <summary>
/// Сообщения об ошибках
/// </summary>
internal static class ErrorMessages
{
    public static string ErrorCreateUri => "Ошибка создания адреса запроса: '{0}'";
    public static string ErrorModifyUri => "Во время изменения адреса запроса произошло исключение: '{0}'";
    public static string ErrorCreateRequest => "Ошибка создания запроса: '{0}'";
    public static string ErrorModifyRequest => "Во время изменения запроса произошло исключение: '{0}'";
    public static string EmptyRequest => $"Передан пустой запрос ('{nameof(IRequest)}')";
    public static string EmptyContent => "Передано пустое содержимое запроса";
    public static string TimeoutError => "Таймаут отправки запроса: '{0}'";
    public static string ErrorCreateClient => "Во время создания Http клиента произошло исключение: '{0}'";
    public static string BadStatusCode => "Ошибочный Http код ответа: '{0}'";
    public static string ErrorGetContent => "Во время получения содержимого из Http ответа произошло исключение: '{0}'";
    public static string ContentIsNull => "Содержимое Http ответа равно null";
    public static string DeserializeError => "Во время десериализации потока в объект произошло исключение";
    public static string DeserializeNull => "Результат после серилизации равен null";
    public static string SerializeError => "Во время сериализации объекта в поток произошло исключение";
    public static string SerializeNull => "Результат после серилизации равен null";
}