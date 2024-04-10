// ReSharper disable RedundantUsingDirective
using System.Text.Json;
using System.Xml;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Logging;
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
        // При старте сервиса можно переопределить существующие сериалайзеры/десериалайзеры, просто передав их в метод.
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
        
        // В процессе отправки запроса выстреливают события
        // На них можно подписываться чтобы как-нибудь обработать
        EventsPublisher.OnBeforeSend += () =>
        {
            // Ваш обработчик
            // Здесь, как и во всех событиях, будет доступ к контексту запроса
            // Внимание! Не все свойства контекста доступны во всех событиях! Проверяйте на null, если не уверены
            // Разинактивить, чтобы увидеть результат
            //MainSampleLogger.Info($"Test from BeforeSend to '{HttpRequestContext.Current!.Request.Uri}'!");
        };

        // Через объект loggingOptions можно переопределять опции логирования,
        // регистрируя свои обработчики событий, позволяющие по своему их логировать
        // Можно также полностью отменить встроенные логи через свойство DisableDefaultLogging
        // В делегатах, как и при обработке событий выше, будет доступ к контексту запроса
        var loggingOptions = new LoggingOptions
        {
            // Разинактивить, чтобы увидеть результат
            //SuccessSendHandler = () => MainSampleLogger.Info("Test Override SuccessSendHandler!")
        };

        services.AddHttpClient<GetRequestsSamplesService>();
        services.AddHttpClient<HeadRequestsSamplesService>();
        services.AddHttpClient<PostRequestsSamplesService>();
        
        services.ConfigureEssentialsHttpClient(configuration, loggingOptions);

        services.AddTransient<IGetRequestsSamplesService, GetRequestsSamplesService>();
        services.AddTransient<IHeadRequestsSamplesService, HeadRequestsSamplesService>();
        services.AddTransient<IPostRequestsSamplesService, PostRequestsSamplesService>();
    }
}