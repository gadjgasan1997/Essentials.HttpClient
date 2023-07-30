using NLog;

namespace Essentials.HttpClient.Dictionaries;

/// <summary>
/// Известные логгеры
/// </summary>
public static class Loggers
{
    /// <summary>
    /// Основной логгер
    /// </summary>
    public static Logger MainLogger { get; } = LogManager.GetLogger("Essentials.HttpClient.Main");
}