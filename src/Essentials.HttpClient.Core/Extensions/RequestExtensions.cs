using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="IRequest" />
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class RequestExtensions
{
    /// <summary>
    /// Проставляет тип медиа в случае если он не зааполнен
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="mediaTypeHeader">Заголовок с типом медиа</param>
    public static void IfNone(this IRequest request, MediaTypeHeaderValue mediaTypeHeader) =>
        request.MediaType.IfNone(() => request.MediaType = mediaTypeHeader);
    
    /// <summary>
    /// Инкрементирует метрику количества запросов
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="Some">Делегат, выполняющийся если проставлены опции метрик на запросе</param>
    /// <param name="None">Делегат, выполняющийся если не проставлены опции метрик на запросе</param>
    public static void IncrementMetric(
        this IRequest request,
        Action<RequestMetricsOptions> Some,
        Action None)
    {
        request.MetricsOptions.IfSome(Some);
        request.MetricsOptions.IfNone(None);
    }

    /// <summary>
    /// Обрабатывает событие специфичным для запроса обработчиком, если такой был найден.
    /// Иначе выполняет переданный делегат
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="eventName">Название события</param>
    /// <param name="ifNoneAction">Действие, выполняемое в случае не найденного обработчика события для данного запроса</param>
    public static void RaiseEvent(this IRequest request, string eventName, Action ifNoneAction)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            return;

        if (request.EventsHandlers.TryGetValue(eventName, out var handler))
        {
            handler();
            return;
        }

        ifNoneAction();
    }
}