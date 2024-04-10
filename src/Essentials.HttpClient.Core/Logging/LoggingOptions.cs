using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Events;
using static Essentials.HttpClient.Events.EventsPublisher;

namespace Essentials.HttpClient.Logging;

/// <summary>
/// Опции логирования
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class LoggingOptions
{
    /// <summary>
    /// Признак необходимости отключить логирование по умолчанию
    /// </summary>
    public bool DisableDefaultLogging { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnSerializeError" /> 
    /// </summary>
    public Handler? SerializeErrorHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnBeforeSend" /> 
    /// </summary>
    public Handler? BeforeSendHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnSuccessSend" /> 
    /// </summary>
    public Handler? SuccessSendHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnErrorSend" /> 
    /// </summary>
    public Handler? ErrorSendHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnBadStatusCode" /> 
    /// </summary>
    public Handler? BadStatusCodeHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnErrorReadContent" /> 
    /// </summary>
    public Handler? ErrorReadContentHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnDeserializeError" /> 
    /// </summary>
    public Handler? DeserializeErrorHandler { get; init; }
}