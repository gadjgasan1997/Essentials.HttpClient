namespace Essentials.HttpClient.Dictionaries;

/// <summary>
/// Сообщения об ошибках
/// </summary>
internal static class ErrorMessages
{
    public static string ErrorSendRequest => "Во время отправки запроса произошло исключение";
    public static string EmptyRequest => $"Передан пустой запрос ('{nameof(IRequest)}')";
    public static string EmptyContent => "Передано пустое содержимое запроса";
    public static string TimeoutError => "Таймаут отправки запроса: '{0}'";
    public static string SocketErrorMessage =>
        "Ошибка при попытке обратиться по адресу '{0}'. Вероятно отсутствует сетевой доступ или нет слушателя по порту. Ошибка: '{1}'";
    public static string ErrorCreateClient => "Во время создания Http клиента произошло исключение: '{0}'";
    public static string BadStatusCode => "Ошибочный Http код ответа: '{0}'";
    public static string ErrorGetContent => "Во время получения содержимого из Http ответа произошло исключение: '{0}'";
    public static string ContentIsNull => "Содержимое Http ответа равно null";
    public static string DeserializeError => "Во время десериализации потока в объект произошло исключение";
    public static string DeserializeNull => "Результат после десериализации равен null";
    public static string SerializeError => "Во время сериализации объекта в поток произошло исключение";
    public static string SerializeNull => "Результат после серилизации равен null";
}