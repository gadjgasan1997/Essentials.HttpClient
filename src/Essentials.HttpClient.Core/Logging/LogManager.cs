using NLog;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using static System.Environment;

namespace Essentials.HttpClient.Logging;

/// <summary>
/// Менеджер для получения логгеров
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal static class LogManager
{
    /// <summary>
    /// Мапа названий Http клиентов на их логгеры
    /// </summary>
    private static readonly ConcurrentDictionary<string, Logger> _loggers = new();
    
    /// <summary>
    /// Основной логгер
    /// </summary>
    public static Logger MainLogger { get; } = NLog.LogManager.GetLogger("Essentials.HttpClient.MainLogger");

    /// <summary>
    /// Возвращает логгер по названию Http клиента
    /// </summary>
    /// <param name="clientName"></param>
    /// <returns></returns>
    public static Logger GetLogger(string clientName)
    {
        try
        {
            if (_loggers.TryGetValue(clientName, out var logger))
                return logger;

            return _loggers.AddOrUpdate(
                clientName,
                _ => NLog.LogManager.GetLogger(clientName),
                (_, existingLogger) => existingLogger);
        }
        catch (Exception exception)
        {
            MainLogger.Error(
                exception,
                $"Во время получения логгера для http клиента с названием '{clientName}' произошло исключение." +
                $"{NewLine}Будет использован логгер по-умолчанию ('{MainLogger.Name}')");
            
            return MainLogger;
        }
    }

    /// <summary>
    /// Возвращает логгер для текущего запроса
    /// </summary>
    /// <returns></returns>
    public static Logger GetCurrentRequestLogger() =>
        HttpRequestContext.Current is null
            ? MainLogger
            : GetLogger(HttpRequestContext.Current.Request.ClientName);
}