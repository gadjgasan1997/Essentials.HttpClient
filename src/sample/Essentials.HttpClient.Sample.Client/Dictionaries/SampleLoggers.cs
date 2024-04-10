using NLog;

namespace Essentials.HttpClient.Sample.Client.Dictionaries;

/// <summary>
/// Логгеры
/// </summary>
public static class SampleLoggers
{
    /// <summary>
    /// Основной логгер
    /// </summary>
    public static Logger MainSampleLogger { get; } = LogManager.GetLogger("MainSampleLogger");
}