using static Essentials.HttpClient.Events.EventsOptions;
using static Essentials.HttpClient.Events.EventsPublisher;

namespace Essentials.HttpClient.Events;

/// <summary>
/// Подписчик на события, обработчики которых контролируются клиентом
/// </summary>
internal static class EventsSubscriber
{
    /// <summary>
    /// Подписывается на события
    /// </summary>
    public static void Subscribe()
    {
        SerializeErrorHandlers.ForEach(handler => OnSerializeError += handler);
        BeforeSendHandlers.ForEach(handler => OnBeforeSend += handler);
        SuccessSendHandlers.ForEach(handler => OnSuccessSend += handler);
        ErrorSendHandlers.ForEach(handler => OnErrorSend += handler);
        BadStatusCodeHandlers.ForEach(handler => OnBadStatusCode += handler);
        ErrorReadContentHandlers.ForEach(handler => OnErrorReadContent += handler);
        DeserializeErrorHandlers.ForEach(handler => OnDeserializeError += handler);
    }
}