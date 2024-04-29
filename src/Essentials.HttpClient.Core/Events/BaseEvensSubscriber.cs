using static Essentials.HttpClient.Logging.LogManager;

namespace Essentials.HttpClient.Events;

/// <summary>
/// Базовый класс для подписчиков на события
/// </summary>
internal abstract class BaseEvensSubscriber
{
    /// <summary>
    /// Подписывается на события
    /// </summary>
    public abstract void Subscribe();

    /// <summary>
    /// Пытается применить обработчик события
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <param name="eventName">Название события</param>
    protected static void TryHandle(Handler handler, string eventName)
    {
        try
        {
            handler();
        }
        catch (Exception ex)
        {
            MainLogger.Error(ex, $"Во время обработки события '{eventName}' произошло исключение");
        }
    }
}