using static Essentials.HttpClient.Events.EventsPublisher;

namespace Essentials.HttpClient.Events;

/// <summary>
/// Опции событий
/// </summary>
internal static class EventsOptions
{
    /// <summary>
    /// Список обработчиков события <see cref="OnSerializeError" />
    /// </summary>
    public static readonly List<Handler> SerializeErrorHandlers = [];
    
    /// <summary>
    /// Список обработчиков события <see cref="OnBeforeSend" />
    /// </summary>
    public static readonly List<Handler> BeforeSendHandlers = [];
    
    /// <summary>
    /// Список обработчиков события <see cref="OnSuccessSend" />
    /// </summary>
    public static readonly List<Handler> SuccessSendHandlers = [];
    
    /// <summary>
    /// Список обработчиков события <see cref="OnErrorSend" />
    /// </summary>
    public static readonly List<Handler> ErrorSendHandlers = [];
    
    /// <summary>
    /// Список обработчиков события <see cref="OnBadStatusCode" />
    /// </summary>
    public static readonly List<Handler> BadStatusCodeHandlers = [];
    
    /// <summary>
    /// Список обработчиков события <see cref="OnErrorReadContent" />
    /// </summary>
    public static readonly List<Handler> ErrorReadContentHandlers = [];
    
    /// <summary>
    /// Список обработчиков события <see cref="OnDeserializeError" />
    /// </summary>
    public static readonly List<Handler> DeserializeErrorHandlers = [];
}