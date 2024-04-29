using Essentials.Utils.Extensions;
using static Essentials.HttpClient.Events.EventsOptions;
using static Essentials.HttpClient.Events.EventsPublisher;

namespace Essentials.HttpClient.Events;

/// <summary>
/// Конфигуратор событий
/// </summary>
public class EventsConfigurator
{
    internal EventsConfigurator()
    { }

    /// <summary>
    /// Добавляет обработчик события <see cref="OnSerializeError" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор событий</returns>
    public EventsConfigurator AttachSerializeErrorHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnSerializeError)}' не может быть null");
        
        SerializeErrorHandlers.Add(handler);
        return this;
    }

    /// <summary>
    /// Добавляет обработчик события <see cref="OnBeforeSend" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор событий</returns>
    public EventsConfigurator AttachBeforeSendHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnBeforeSend)}' не может быть null");
        
        BeforeSendHandlers.Add(handler);
        return this;
    }

    /// <summary>
    /// Добавляет обработчик события <see cref="OnSuccessSend" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор событий</returns>
    public EventsConfigurator AttachSuccessSendHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnSuccessSend)}' не может быть null");
        
        SuccessSendHandlers.Add(handler);
        return this;
    }

    /// <summary>
    /// Добавляет обработчик события <see cref="OnErrorSend" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор событий</returns>
    public EventsConfigurator AttachErrorSendHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnErrorSend)}' не может быть null");
        
        ErrorSendHandlers.Add(handler);
        return this;
    }

    /// <summary>
    /// Добавляет обработчик события <see cref="OnBadStatusCode" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор событий</returns>
    public EventsConfigurator AttachBadStatusCodeHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnBadStatusCode)}' не может быть null");
        
        BadStatusCodeHandlers.Add(handler);
        return this;
    }

    /// <summary>
    /// Добавляет обработчик события <see cref="OnErrorReadContent" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор событий</returns>
    public EventsConfigurator AttachErrorReadContentHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnErrorReadContent)}' не может быть null");
        
        ErrorReadContentHandlers.Add(handler);
        return this;
    }

    /// <summary>
    /// Добавляет обработчик события <see cref="OnDeserializeError" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор событий</returns>
    public EventsConfigurator AttachDeserializeErrorHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnDeserializeError)}' не может быть null");
        
        DeserializeErrorHandlers.Add(handler);
        return this;
    }
}