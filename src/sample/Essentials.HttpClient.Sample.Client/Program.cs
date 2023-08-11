using System.Text.Json;
using System.Xml;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Sample.Client;
using Essentials.HttpClient.Sample.Client.Implementations;
using Essentials.HttpClient.Serialization;
using Essentials.HttpClient.Serialization.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices(services =>
{
    // При старте сервиса можно переопределить существующие сериалайзеры/десериалайзеры, просто передав их в метод.
    // Каждый встроенный сериалайзер имеет конструктор с необязательными параметрами,
    // через которые можно контролировать его поведение.
    // Также можно передать кастомный сериалайзер
    var customXmlSerializer = new XmlSerializer(
        deserializeOptions: new XmlReaderSettings(), 
        serializeOptions: new XmlWriterSettings());

    // Например, можно проставить свойство PropertyNameCaseInsensitive в false и убедиться,
    // что ответы от сервера не будут корректно десериализовываться
    var customJsonSerializer = new NativeJsonSerializer(
        deserializeOptions: new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    services.ConfigureEssentialsHttpClient(
        builder.Configuration,
        new List<SerializerInfo> {new(customXmlSerializer)},
        new List<SerializerInfo> {new(customXmlSerializer), new(customJsonSerializer)});

    services.AddTransient<IGetRequestsSamplesService, GetRequestsSamplesService>();
    services.AddTransient<IHeadRequestsSamplesService, HeadRequestsSamplesService>();
    services.AddTransient<IPostRequestsSamplesService, PostRequestsSamplesService>();
});

var app = builder.Build();

var getRequestsSamplesService = app.Services.GetRequiredService<IGetRequestsSamplesService>();
var headRequestsSamplesService = app.Services.GetRequiredService<IHeadRequestsSamplesService>();
var postRequestsSamplesService = app.Services.GetRequiredService<IPostRequestsSamplesService>();

await getRequestsSamplesService.RunSamples();
await headRequestsSamplesService.RunSamples();
await postRequestsSamplesService.RunSamples();

app.MapGet("/", () => "Hello World!");

app.Run();