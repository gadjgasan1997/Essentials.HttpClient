namespace Essentials.HttpClient.Events;

/// <summary>
/// Делегат-обработчик события
/// </summary>
public delegate void Handler();

/// <summary>
/// Класс с отдаваемыми событиями
/// </summary>
internal static class EventsStorage
{
    /// <summary>
    /// Событие ошибки сериализации объекта
    /// </summary>
    public static event Handler? OnSerializeError;
    public static void RaiseOnSerializeError() => OnSerializeError?.Invoke();
    
    /// <summary>
    /// Событие перед отправкой запроса
    /// </summary>
    public static event Handler? OnBeforeSend;
    public static void RaiseOnBeforeSend() => OnBeforeSend?.Invoke();
    
    /// <summary>
    /// Событие успеха отправки запроса
    /// </summary>
    public static event Handler? OnSuccessSend;
    public static void RaiseOnSuccessSend() => OnSuccessSend?.Invoke();
    
    /// <summary>
    /// Событие ошибки отправки запроса
    /// </summary>
    public static event Handler? OnErrorSend;
    public static void RaiseOnErrorSend() => OnErrorSend?.Invoke();
    
    /// <summary>
    /// Событие ошибочного Http кода ответа
    /// </summary>
    public static event Handler? OnBadStatusCode;
    public static void RaiseOnBadStatusCode() => OnBadStatusCode?.Invoke();
    
    /// <summary>
    /// Событие ошибки при чтении содержимого из Http ответа
    /// </summary>
    public static event Handler? OnErrorReadContent;
    public static void RaiseOnErrorReadContent() => OnErrorReadContent?.Invoke();
    
    /// <summary>
    /// Событие ошибки десериализации строки ответа
    /// </summary>
    public static event Handler? OnDeserializeError;
    public static void RaiseOnDeserializeError() => OnDeserializeError?.Invoke();
}