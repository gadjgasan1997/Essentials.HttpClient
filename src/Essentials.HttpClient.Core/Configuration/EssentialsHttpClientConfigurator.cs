using Essentials.Utils.Extensions;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.RequestsInterception;
using Microsoft.Extensions.DependencyInjection;

namespace Essentials.HttpClient.Configuration;

/// <summary>
/// Конфигуратор http клиента
/// </summary>
public class EssentialsHttpClientConfigurator
{
    internal EssentialsHttpClientConfigurator()
    { }
    
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
    /// Добавляет глобальный интерсептор, который будет автоматически добавляться ко всем запросам
    /// </summary>
    /// <param name="serviceLifetime">Требуемое время жизни интерсептора, по-умолчанию <see cref="ServiceLifetime.Singleton" /></param>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    /// <returns>Конфигуратор http клиента</returns>
    public EssentialsHttpClientConfigurator AttachGlobalInterceptor<TInterceptor>(ServiceLifetime? serviceLifetime = null)
        where TInterceptor : IRequestInterceptor
    {
        InterceptorsStorage.TryAttachGlobalInterceptor<TInterceptor>(serviceLifetime);
        return this;
    }

    /// <summary>
    /// Регистрирует интерсептор
    /// </summary>
    /// <param name="serviceLifetime">Требуемое время жизни интерсептора, по-умолчанию <see cref="ServiceLifetime.Singleton" /></param>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    /// <returns>Конфигуратор http клиента</returns>
    public EssentialsHttpClientConfigurator RegisterInterceptor<TInterceptor>(ServiceLifetime? serviceLifetime = null)
        where TInterceptor : IRequestInterceptor
    {
        InterceptorsStorage.TryAddInterceptorToRegister<TInterceptor>(serviceLifetime);
        return this;
    }
}