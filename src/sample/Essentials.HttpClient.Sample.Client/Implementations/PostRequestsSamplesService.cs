using System.Net.Mime;
using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.Sample.Client.Models;
using Essentials.HttpClient.Sample.Client.Models.Requests;
using static Essentials.HttpClient.Common.Helpers.SerializationHelpers;
using static Essentials.HttpClient.Sample.Client.Dictionaries.CommonConsts;

namespace Essentials.HttpClient.Sample.Client.Implementations;

[SuppressMessage("ReSharper", "UnusedVariable")]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public class PostRequestsSamplesService : IPostRequestsSamplesService
{
    private readonly ILogger<PostRequestsSamplesService> _logger;
    private readonly IEssentialsHttpClient _httpClient;

    public PostRequestsSamplesService(
        ILogger<PostRequestsSamplesService> logger,
        IEssentialsHttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
    public async Task RunSamples()
    {
        await RunSample_GetPersonsInJson();
        await RunSample_GetPersonsInXml();
        //await RunSample_GetPersonsInText();
    }
    
    private async Task RunSample_GetPersonsInJson()
    {
        var data = new GetPersonsInJsonRequest
        {
            Name = "as"
        };
        
        var uriValidation = await HttpUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("post", "GetPersonsInJson")
            .BuildAsync();

        var requestValidation = await HttpRequestBuilder
            .CreateBuilder(uriValidation)
            .WithRequestId("Test_Request_Id")
            .SetTypeId("GetPersonsInJson")
            
            /* Данным методом мы можем переопределить все существующие обработчики соответствующего события
                 Например, для события OnBeforeSend мы отменяем логирование по-умолчанию и производим свое
                 Но действовать это будет только на этом запросе */
            
            /*.OnBeforeSend(() =>
            {
                _logger.LogInformation(
                    $"Тест переопределения отправки запроса '{HttpRequestContext.Current!.Request.TypeId}'");
            })*/
            .BuildAsync<PostRequestsSamplesService>();

        var response = await _httpClient
            .PostJsonDataAsync(requestValidation, data)
            .ReceiveNativeJsonContentAsync<List<Person>>();
    }

    private async Task RunSample_GetPersonsInXml()
    {
        var data = new GetPersonsInXmlRequest
        {
            Name = "mi"
        };
        
        var uriValidation = await HttpUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("post", "GetPersonsInXml")
            .BuildAsync();

        var requestValidation = await HttpRequestBuilder
            .CreateBuilder(uriValidation)
            .SetTypeId("GetPersonsInXml")
            
            // Далее идет добавление интерсепторов
            // Интерсепторы запроса применяются строго после глобальных
            // Последовательность имеет значение
            
            // С помощью данного метода можно отключить глобальный интерсептор, в данном случае - интерсептор логов
            .DisableGlobalInterceptor<MetricsInterceptor>()
            
            // Затем можно добавить свой интерсептор, для простоты примера снова добавляется интерсептор логов
            .WithInterceptor<LoggingInterceptor>()
            
            // Интерсептор применяется единожды вне зависимости от того, сколько раз был добавлен
            .WithInterceptor<LoggingInterceptor>()
            
            /* Данным методом мы можем переопределить все существующие обработчики соответствующего события
                 Например, для события OnSuccessSend мы отменяем поведение по-умолчанию и производим свое
                 Но действовать это будет только на этом запросе */
            
            /*.OnSuccessSend(() =>
            {
                _logger.LogInformation(
                    $"Тест переопределения обработки ответ '{HttpRequestContext.Current!.Request.TypeId}'");
            })*/
            .BuildAsync<PostRequestsSamplesService>();

        var response = await _httpClient
            .PostXmlDataAsync(requestValidation, data)
            .ReceiveXmlContentAsync<List<Person>>();
    }

    private async Task RunSample_GetPersonsInText()
    {
        var data = new GetPersonsInXmlRequest
        {
            Name = "mi"
        };
        
        var requestString = SerializeInXml(data);
        
        var uriValidation = await HttpUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("post", "GetPersonsInPlainXmlText")
            .BuildAsync();

        var requestValidation = await HttpRequestBuilder
            .CreateBuilder(uriValidation)
            .SetTypeId("GetPersonsInPlainXmlText")
            .SetMediaTypeHeader(MediaTypeNames.Text.Xml)
            .BuildAsync<PostRequestsSamplesService>();

        var response = await _httpClient
            .PostStringAsync(requestValidation, requestString)
            .ReceiveXmlContentAsync<List<Person>>();
    }
}