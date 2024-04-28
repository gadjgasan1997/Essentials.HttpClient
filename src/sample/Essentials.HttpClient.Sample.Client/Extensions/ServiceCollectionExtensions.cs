// ReSharper disable RedundantUsingDirective
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
    public static void ConfigureSampleClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTestsServices();
        AddCustomEventsHandlers();

        #region Конфигурация клиента через fluent api

        // Через fluent api можно переопределять опции логирования, метрик и сериализации
        // Для логирования, например, можно переопределить обработчики логирования
        // Или полностью отменить встроенные логи через метод DisableDefaultLogging на конфигураторе логирования
        // В делегатах, как и при обработке событий выше, будет доступ к контексту запроса
        
        #region Переопределение логов
        
        // ВНИМАНИЕ !!!
        // Данная возможность переопределяет стандартное поведение при сборе логов или метрик,
        // не заменяя собой ваши обработчики, добавленные через EventsPublisher
        // Ниже приведен пример переопределения некоторых событий
        Action<LoggingConfigurator> configureLogsAction = loggingConfigurator =>
            loggingConfigurator
                .SetupBeforeSendEventHandler(
                    () => MainSampleLogger.Info("Test Override BeforeSendHandler!"))
                .SetupSuccessSendEventHandler(
                    () => MainSampleLogger.Info("Test Override SuccessSendHandler!"))
            
                // Полностью отключает стандартное логирование. Имеет приоритет над всеми остальными операциями в рамках данного конфигуратора
                /*.DisableDefaultLogging()*/;
        
        #endregion

        #region Переопределение метрик

        Action<MetricsConfigurator> configureMetricsAction = metricsConfigurator =>
            { };

        #endregion

        #region Переопределение сериализации/десериализации

        // При старте сервиса можно переопределить существующие сериалайзеры/десериалайзеры через fluent api
        // Каждый встроенный сериалайзер имеет конструктор с необязательными параметрами,
        // через которые можно контролировать его поведение.
        // Также можно передать кастомный сериалайзер
        var customXmlSerializer = new XmlSerializer(new XmlWriterSettings());

        // Например, можно проставить свойство PropertyNameCaseInsensitive в false и убедиться,
        // что ответы от сервера не будут корректно десериализовываться
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
            configuration
            
            // Разинактивить блок, чтобы увидеть результат переопределения стандартных действий (логирования, метрик и сериализации)
            /*,
            configurator =>
                configurator
                    .ConfigureLogging(configureLogsAction)
                    .ConfigureMetrics(configureMetricsAction)
                    .ConfigureSerialization(configureSerializationAction)*/);
    }

    private static void AddCustomEventsHandlers()
    {
        // В процессе отправки запроса выстреливают события
        // На них можно подписываться чтобы как-нибудь обработать
        // Существуют и стандартные обработчики, собирающие логи и метрики
        // Данный функционал нужен для сценариев, когда стандартных обработчиков недостаточно
        EventsPublisher.OnBeforeSend += () =>
        {
            // Ваш обработчик
            // Здесь, как и во всех событиях, будет доступ к контексту запроса
            
            // ВНИМАНИЕ !!!
            // Не все свойства контекста доступны во всех событиях! Проверяйте на null, если не уверены
            
            // Разинактивить, чтобы увидеть результат
            //MainSampleLogger.Info($"Test from BeforeSend to '{HttpRequestContext.Current!.Request.Uri}'!");
        };
    }

    private static void AddTestsServices(this IServiceCollection services)
    {
        services.AddHttpClient<GetRequestsSamplesService>();
        services.AddHttpClient<HeadRequestsSamplesService>();
        services.AddHttpClient<PostRequestsSamplesService>();
        services.AddTransient<IGetRequestsSamplesService, GetRequestsSamplesService>();
        services.AddTransient<IHeadRequestsSamplesService, HeadRequestsSamplesService>();
        services.AddTransient<IPostRequestsSamplesService, PostRequestsSamplesService>();
    }
}