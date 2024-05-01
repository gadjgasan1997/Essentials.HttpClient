// ReSharper disable RedundantUsingDirective
using Polly;
using Polly.Retry;
using System.Xml;
using System.Text.Json;
using Polly.Extensions.Http;
using Essentials.Utils.Extensions;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Sample.Client.Implementations;
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
                })
                // Имеются и другие события
                ;
        
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

        #region Вызов метода конфигурации клиента
        
        services.ConfigureEssentialsHttpClient(
            configuration,
            configurator =>
                configurator
                    
                    // Интерсепторы применяются в той последовательности, в которой были добавлены,
                    // а также единожды вне зависимости от того, сколько раз вызван метод с одним и тем же типом
                    
                    // Данный интерсептор можно не использовать, интерсептор логов автоматически засекает таймер для отображения в логах
                    // Однако если необходимо в контексте запроса видеть свойство ElapsedMilliseconds, тогда можно добавить
                    //.AttachGlobalInterceptor<RequestsTimerInterceptor>()
                    
                    // Метрики
                    .AttachGlobalInterceptor<MetricsInterceptor>()
                    
                    // Логи
                    .AttachGlobalInterceptor<LoggingInterceptor>()
                    
                    // Разинактивить блок, чтобы увидеть результат переопределения стандартных действий и подписки на события
                    //.ConfigureSerialization(configureSerializationAction)
                    //.SubscribeToEvents(configureEventsAction)
                    );

        #endregion

        return services;
    }

    private static void AddTestsServices(this IServiceCollection services)
    {
        services.AddTransient<IGetRequestsSamplesService, GetRequestsSamplesService>();
        services.AddTransient<IHeadRequestsSamplesService, HeadRequestsSamplesService>();
        services.AddTransient<IPostRequestsSamplesService, PostRequestsSamplesService>();
        
        // Клиенты можно не добавлять, тогда будет использоваться клиент по-умолчанию
        services
            .AddHttpClient<GetRequestsSamplesService>()
            .AddPolicyHandler(GetRetryPolicy());
        
        services
            .AddHttpClient<HeadRequestsSamplesService>()
            .AddPolicyHandler(GetRetryPolicy());
        
        services
            .AddHttpClient<PostRequestsSamplesService>()
            .AddPolicyHandler(GetRetryPolicy());
    }
    
    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (_, span) =>
                {
                    // Внутри данного делегата также доступен контекст запроса
                    var context = HttpRequestContext.Current.CheckNotNull();
                    var request = context.Request;
                    
                    MainSampleLogger.Error(
                        $"Ошибка отправки запроса по происшествию '{span}' по адресу '{request.Uri}'");
                });
    }
}