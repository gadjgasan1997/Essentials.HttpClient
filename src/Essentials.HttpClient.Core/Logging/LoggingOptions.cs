using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Events.Args;
using static Essentials.HttpClient.Events.EventsPublisher;

namespace Essentials.HttpClient.Logging;

/// <summary>
/// Опции логирования
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class LoggingOptions
{
    /// <summary>
    /// Обработчик события <see cref="OnSuccessCreateUri" />
    /// </summary>
    public Handler<SuccessCreateUriEventArgs>? SuccessCreateUriHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnErrorCreateUri" /> 
    /// </summary>
    public Handler<ErrorCreateUriEventArgs>? ErrorCreateUriHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnSuccessCreateRequest" /> 
    /// </summary>
    public Handler<SuccessCreateRequestEventArgs>? SuccessCreateRequestHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnErrorCreateRequest" /> 
    /// </summary>
    public Handler<ErrorCreateRequestEventArgs>? ErrorCreateRequestHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnBadRequest" /> 
    /// </summary>
    public Handler<BadRequestEventArgs>? BadRequestHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnSerializeError" /> 
    /// </summary>
    public Handler<SerializeErrorEventArgs>? SerializeErrorHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnSuccessSend" /> 
    /// </summary>
    public Handler<SuccessSendRequestEventArgs>? SuccessSendHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnErrorSend" /> 
    /// </summary>
    public Handler<ErrorSendRequestEventArgs>? ErrorSendHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnBadStatusCode" /> 
    /// </summary>
    public Handler<BadStatusCodeEventArgs>? BadStatusCodeHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnErrorReadContent" /> 
    /// </summary>
    public Handler<ErrorReadContentEventArgs>? ErrorReadContentHandler { get; init; }
    
    /// <summary>
    /// Обработчик события <see cref="OnDeserializeError" /> 
    /// </summary>
    public Handler<DeserializeErrorEventArgs>? DeserializeErrorHandler { get; init; }
}