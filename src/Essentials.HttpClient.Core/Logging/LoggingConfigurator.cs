using Essentials.HttpClient.Events;
using Essentials.Utils.Extensions;
using static Essentials.HttpClient.Logging.LoggingOptions;
using static Essentials.HttpClient.Events.EventsPublisher;
#pragma warning disable CA1822

namespace Essentials.HttpClient.Logging;

/// <summary>
/// Конфигуратор логирования
/// </summary>
public class LoggingConfigurator
{
    internal LoggingConfigurator()
    { }

    /// <summary>
    /// Устанавливает обработчик события <see cref="OnSerializeError" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор логирования</returns>
    public LoggingConfigurator SetupSerializeErrorEventHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnSerializeError)}' не может быть null");
        
        SerializeErrorHandler = handler;
        return this;
    }

    /// <summary>
    /// Устанавливает обработчик события <see cref="OnBeforeSend" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор логирования</returns>
    public LoggingConfigurator SetupBeforeSendEventHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnBeforeSend)}' не может быть null");
        
        BeforeSendHandler = handler;
        return this;
    }

    /// <summary>
    /// Устанавливает обработчик события <see cref="OnSuccessSend" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор логирования</returns>
    public LoggingConfigurator SetupSuccessSendEventHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnSuccessSend)}' не может быть null");
        
        SuccessSendHandler = handler;
        return this;
    }

    /// <summary>
    /// Устанавливает обработчик события <see cref="OnErrorSend" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор логирования</returns>
    public LoggingConfigurator SetupErrorSendEventHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnErrorSend)}' не может быть null");
        
        ErrorSendHandler = handler;
        return this;
    }

    /// <summary>
    /// Устанавливает обработчик события <see cref="OnBadStatusCode" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор логирования</returns>
    public LoggingConfigurator SetupBadStatusCodeEventHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnBadStatusCode)}' не может быть null");
        
        BadStatusCodeHandler = handler;
        return this;
    }

    /// <summary>
    /// Устанавливает обработчик события <see cref="OnErrorReadContent" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор логирования</returns>
    public LoggingConfigurator SetupErrorReadContentEventHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnErrorReadContent)}' не может быть null");
        
        ErrorReadContentHandler = handler;
        return this;
    }

    /// <summary>
    /// Устанавливает обработчик события <see cref="OnDeserializeError" />
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Конфигуратор логирования</returns>
    public LoggingConfigurator SetupDeserializeErrorEventHandler(Handler handler)
    {
        handler.CheckNotNull($"Обработчик события '{nameof(OnDeserializeError)}' не может быть null");
        
        DeserializeErrorHandler = handler;
        return this;
    }

    /// <summary>
    /// Отключает логирование по-умолчанию
    /// </summary>
    public void DisableDefaultLogging() => LoggingOptions.DisableDefaultLogging = true;
}