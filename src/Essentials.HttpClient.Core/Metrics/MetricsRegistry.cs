using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Timer;

namespace Essentials.HttpClient.Metrics;

/// <summary>
/// Реестр метрик
/// </summary>
internal static class MetricsRegistry
{
    /// <summary>
    /// Счетчик для Http запросов
    /// </summary>
    public static CounterOptions HttpRequestCounter { get; } = new()
    {
        Name = "Http Request Counter"
    };

    /// <summary>
    /// Счетчик для успешных Http запросов
    /// </summary>
    public static CounterOptions ValidHttpRequestCounter { get; } = new()
    {
        Name = "Valid Http Request Counter"
    };

    /// <summary>
    /// Счетчик для ошибочных Http запросов
    /// </summary>
    public static CounterOptions InvalidHttpRequestCounter { get; } = new()
    {
        Name = "Invalid Http Request Counter"
    };

    /// <summary>
    /// Счетчик для Get Http запросов
    /// </summary>
    public static CounterOptions GetHttpRequestCounter { get; } = new()
    {
        Name = "Get Http Request Counter"
    };

    /// <summary>
    /// Счетчик для успешных Get Http запросов
    /// </summary>
    public static CounterOptions ValidGetHttpRequestCounter { get; } = new()
    {
        Name = "Valid Get Http Request Counter"
    };

    /// <summary>
    /// Счетчик для ошибочных Get Http запросов
    /// </summary>
    public static CounterOptions InvalidGetHttpRequestCounter { get; } = new()
    {
        Name = "Invalid Get Http Request Counter"
    };

    /// <summary>
    /// Счетчик для Post Http запросов
    /// </summary>
    public static CounterOptions PostHttpRequestCounter { get; } = new()
    {
        Name = "Post Http Request Counter"
    };

    /// <summary>
    /// Счетчик для успешных Post Http запросов
    /// </summary>
    public static CounterOptions ValidPostHttpRequestCounter { get; } = new()
    {
        Name = "Valid Post Http Request Counter"
    };

    /// <summary>
    /// Счетчик для ошибочных Post Http запросов
    /// </summary>
    public static CounterOptions InvalidPostHttpRequestCounter { get; } = new()
    {
        Name = "Invalid Post Http Request Counter"
    };

    /// <summary>
    /// Время выполнения запросов
    /// </summary>
    public static TimerOptions HttpRequestsTimer => new()
    {
        Name = "Http Requests Timer",
        MeasurementUnit = Unit.Requests,
        DurationUnit = TimeUnit.Milliseconds,
        RateUnit = TimeUnit.Milliseconds
    };
}