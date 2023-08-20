using Essentials.HttpClient.Events.Args;

namespace Essentials.HttpClient.Events;

/// <summary>
/// Класс с отдаваемыми событиями
/// </summary>
public static class EventsPublisher
{
    /// <summary>
    /// Событие успешно созданного адреса запроса
    /// </summary>
    public static event Handler<SuccessCreateUriEventArgs>? OnSuccessCreateUri;
    internal static void RaiseOnSuccessCreateUri(SuccessCreateUriEventArgs args) => OnSuccessCreateUri?.Invoke(args);
    
    /// <summary>
    /// Событие ошибки при создании адреса запроса
    /// </summary>
    public static event Handler<ErrorCreateUriEventArgs>? OnErrorCreateUri;
    internal static void RaiseOnErrorCreateUri(ErrorCreateUriEventArgs args) => OnErrorCreateUri?.Invoke(args);
    
    /// <summary>
    /// Событие успешно созданного запроса
    /// </summary>
    public static event Handler<SuccessCreateRequestEventArgs>? OnSuccessCreateRequest;
    internal static void RaiseOnSuccessCreateRequest(SuccessCreateRequestEventArgs args) =>
        OnSuccessCreateRequest?.Invoke(args);
    
    /// <summary>
    /// Событие ошибки при создании запроса
    /// </summary>
    public static event Handler<ErrorCreateRequestEventArgs>? OnErrorCreateRequest;
    internal static void RaiseOnErrorCreateRequest(ErrorCreateRequestEventArgs args) =>
        OnErrorCreateRequest?.Invoke(args);
    
    /// <summary>
    /// Событие ошибочных данных запроса
    /// </summary>
    public static event Handler<BadRequestEventArgs>? OnBadRequest;
    internal static void RaiseOnBadRequest(BadRequestEventArgs args) => OnBadRequest?.Invoke(args);
    
    /// <summary>
    /// Событие ошибки сериализации объекта
    /// </summary>
    public static event Handler<SerializeErrorEventArgs>? OnSerializeError;
    internal static void RaiseOnSerializeError(SerializeErrorEventArgs args) => OnSerializeError?.Invoke(args);
    
    /// <summary>
    /// Событие успеха отправки запроса
    /// </summary>
    public static event Handler<SuccessSendRequestEventArgs>? OnSuccessSend;
    internal static void RaiseOnSuccessSend(SuccessSendRequestEventArgs args) => OnSuccessSend?.Invoke(args);
    
    /// <summary>
    /// Событие ошибки отправки запроса
    /// </summary>
    public static event Handler<ErrorSendRequestEventArgs>? OnErrorSend;
    internal static void RaiseOnErrorSend(ErrorSendRequestEventArgs args) => OnErrorSend?.Invoke(args);
    
    /// <summary>
    /// Событие ошибочного Http кода ответа
    /// </summary>
    public static event Handler<BadStatusCodeEventArgs>? OnBadStatusCode;
    internal static void RaiseOnBadStatusCode(BadStatusCodeEventArgs args) => OnBadStatusCode?.Invoke(args);
    
    /// <summary>
    /// Событие ошибки при чтении содержимого из Http ответа
    /// </summary>
    public static event Handler<ErrorReadContentEventArgs>? OnErrorReadContent;
    internal static void RaiseOnErrorReadContent(ErrorReadContentEventArgs args) => OnErrorReadContent?.Invoke(args);
    
    /// <summary>
    /// Событие ошибки десериализации строки
    /// </summary>
    public static event Handler<DeserializeErrorEventArgs>? OnDeserializeError;
    internal static void RaiseOnDeserializeError(DeserializeErrorEventArgs args) => OnDeserializeError?.Invoke(args);
}