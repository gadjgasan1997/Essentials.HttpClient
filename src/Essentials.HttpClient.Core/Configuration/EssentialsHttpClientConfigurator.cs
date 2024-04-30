using Essentials.Utils.Extensions;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.RequestsInterception;

namespace Essentials.HttpClient.Configuration;

/// <summary>
/// Конфигуратор http клиента
/// </summary>
public class EssentialsHttpClientConfigurator
{
    internal EssentialsHttpClientConfigurator()
    { }
    
    /// <summary>
    /// Настраивает логирование на клиенте
    /// </summary>
    /// <param name="configureAction">Действие конфигурации логирования</param>
    /// <returns>Конфигуратор http клиента</returns>
    public EssentialsHttpClientConfigurator ConfigureLogging(Action<LoggingConfigurator> configureAction)
    {
        configureAction.CheckNotNull("Действие конфигурации логирования не может быть null");
        
        configureAction(new LoggingConfigurator());
        return this;
    }
    
    /// <summary>
    /// Настраивает метрики на клиенте
    /// </summary>
    /// <param name="configureAction">Действие конфигурации метрик</param>
    /// <returns>Конфигуратор http клиента</returns>
    public EssentialsHttpClientConfigurator ConfigureMetrics(Action<MetricsConfigurator> configureAction)
    {
        configureAction.CheckNotNull("Действие конфигурации метрик не может быть null");

        configureAction(new MetricsConfigurator());
        return this;
    }
    
    /// <summary>
    /// Настраивает сериализацию/десериализацию на клиенте
    /// </summary>
    /// <param name="configureAction">Действие конфигурации метрик</param>
    /// <returns>Конфигуратор http клиента</returns>
    public EssentialsHttpClientConfigurator ConfigureSerialization(Action<SerializationConfigurator> configureAction)
    {
        configureAction.CheckNotNull("Действие конфигурации сериализации не может быть null");

        configureAction(new SerializationConfigurator());
        return this;
    }

    /// <summary>
    /// Подписывается на события
    /// </summary>
    /// <param name="configureAction">Действие подписки на события</param>
    /// <returns>Конфигуратор http клиента</returns>
    public EssentialsHttpClientConfigurator SubscribeToEvents(Action<EventsConfigurator> configureAction)
    {
        configureAction.CheckNotNull("Действие конфигурации событий не может быть null");
        
        configureAction(new EventsConfigurator());
        return this;
    }

    /// <summary>
    /// Регистрирует интерсептор
    /// </summary>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    /// <returns>Конфигуратор http клиента</returns>
    public EssentialsHttpClientConfigurator RegisterInterceptor<TInterceptor>()
        where TInterceptor : IRequestInterceptor
    {
        InterceptorsStorage.TryAddInterceptor<TInterceptor>();
        return this;
    }
}