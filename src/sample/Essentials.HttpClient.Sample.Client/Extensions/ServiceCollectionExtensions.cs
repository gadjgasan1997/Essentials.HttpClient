﻿// ReSharper disable RedundantUsingDirective
using System.Xml;
using System.Text.Json;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.Sample.Client.Implementations;
using Essentials.HttpClient.Serialization;
using Essentials.Serialization;
using Essentials.Serialization.Serializers;
using Essentials.Serialization.Deserializers;
using static Essentials.HttpClient.Sample.Client.Dictionaries.SampleLoggers;

namespace Essentials.HttpClient.Sample.Client.Extensions;

/// <summary>
/// Методы расширения для <see cref="IServiceCollection" />
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Настраивает сервисы
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static IServiceCollection ConfigureSampleClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTestsServices();
        
        #region Конфигурация клиента через fluent api
        
        // Через fluent api можно переопределять разные опции отправки запросов
        
        #region Добавление кастомных обработчиков событий
        
        // В процессе отправки запроса выстреливают события
        // На них можно подписываться чтобы как-нибудь обработать

        Action<EventsConfigurator> configureEventsAction = eventsConfigurator =>
            eventsConfigurator
                .AttachBeforeSendHandler(() =>
                {
                    // Ваш обработчик
                    // Здесь, как и во всех событиях, будет доступ к контексту запроса

                    // ВНИМАНИЕ !!!
                    // Не все свойства контекста доступны во всех событиях! Проверяйте на null, если не уверены
                    MainSampleLogger.Info($"My custom handler for BeforeSendEvent: '{HttpRequestContext.Current!.Request.Uri}'!");
                })
                .AttachSuccessSendHandler(() =>
                {
                    // Ваш обработчик
                    // Здесь, как и во всех событиях, будет доступ к контексту запроса

                    // ВНИМАНИЕ !!!
                    // Не все свойства контекста доступны во всех событиях! Проверяйте на null, если не уверены
                    MainSampleLogger.Info(
                        $"My custom handler for SuccessSendEvent: '{HttpRequestContext.Current!.ElapsedMilliseconds}'!");
                });
        
        #endregion
        
        #region Переопределение сериализации/десериализации

        // При старте сервиса можно переопределить существующие сериалайзеры/десериалайзеры через fluent api
        // Каждый встроенный сериалайзер имеет конструктор с необязательными параметрами,
        // через которые можно контролировать его поведение.
        // Также можно передать кастомный сериалайзер
        var customXmlSerializer = new XmlSerializer(new XmlWriterSettings());
        var customJsonDeserializer = new NativeJsonDeserializer(
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        Action<SerializationConfigurator> configureSerializationAction = serializationConfigurator =>
            serializationConfigurator
                .SetupDefaultXmlSerializer(customXmlSerializer)
                .SetupDefaultNativeJsonDeserializer(customJsonDeserializer);

        #endregion
        
        #endregion
        
        services.ConfigureEssentialsHttpClient(
            configuration,
            configurator =>
                configurator
                    // Интерсепторы применяются в той последовательности, в которой были добавлены,
                    // а также единожды вне зависимости от того, сколько раз вызван метод с одним и тем же типом
                    .AttachGlobalInterceptor<RequestsTimerInterceptor>()
                    .AttachGlobalInterceptor<MetricsInterceptor>()
                    .AttachGlobalInterceptor<LoggingInterceptor>()
                    
                    // Разинактивить блок, чтобы увидеть результат переопределения стандартных действий и подписки на события
                    //.ConfigureSerialization(configureSerializationAction)
                    //.SubscribeToEvents(configureEventsAction)
                    );

        return services;
    }

    private static void AddTestsServices(this IServiceCollection services)
    {
        services.AddTransient<IGetRequestsSamplesService, GetRequestsSamplesService>();
        services.AddTransient<IHeadRequestsSamplesService, HeadRequestsSamplesService>();
        services.AddTransient<IPostRequestsSamplesService, PostRequestsSamplesService>();
        
        // Клиенты можно не добавлять, тогда будет использоваться клиент по-умолчанию
        services.AddHttpClient<GetRequestsSamplesService>();
        services.AddHttpClient<HeadRequestsSamplesService>();
        services.AddHttpClient<PostRequestsSamplesService>();
    }
}