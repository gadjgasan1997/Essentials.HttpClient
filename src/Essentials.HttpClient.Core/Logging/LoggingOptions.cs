using Essentials.HttpClient.Events;
using System.Diagnostics.CodeAnalysis;
using static Essentials.HttpClient.Events.EventsPublisher;

namespace Essentials.HttpClient.Logging;

/// <summary>
/// Опции логирования
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
internal static class LoggingOptions
{
    /// <summary>
    /// Признак необходимости отключить логирование по умолчанию
    /// </summary>
    public static bool DisableDefaultLogging { get; set; }
    
    /// <summary>
    /// Обработчик события <see cref="OnSerializeError" />
    /// </summary>
    public static Handler? SerializeErrorHandler { get; set; }
    
    /// <summary>
    /// Обработчик события <see cref="OnBeforeSend" />
    /// </summary>
    public static Handler? BeforeSendHandler { get; set; }
    
    /// <summary>
    /// Обработчик события <see cref="OnSuccessSend" />
    /// </summary>
    public static Handler? SuccessSendHandler { get; set; }
    
    /// <summary>
    /// Обработчик события <see cref="OnErrorSend" />
    /// </summary>
    public static Handler? ErrorSendHandler { get; set; }
    
    /// <summary>
    /// Обработчик события <see cref="OnBadStatusCode" />
    /// </summary>
    public static Handler? BadStatusCodeHandler { get; set; }
    
    /// <summary>
    /// Обработчик события <see cref="OnErrorReadContent" />
    /// </summary>
    public static Handler? ErrorReadContentHandler { get; set; }
    
    /// <summary>
    /// Обработчик события <see cref="OnDeserializeError" />
    /// </summary>
    public static Handler? DeserializeErrorHandler { get; set; }
}