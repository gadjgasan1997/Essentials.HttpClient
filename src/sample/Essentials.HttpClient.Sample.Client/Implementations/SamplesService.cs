using Essentials.HttpClient.Common.Models;
using Essentials.HttpClient.Common.Models.Requests;
using Essentials.HttpClient.Extensions;
using TextPlain = Essentials.HttpClient.MediaTypes.Text.Plain;
using static Essentials.HttpClient.Common.Helpers.SerializationHelpers;

namespace Essentials.HttpClient.Sample.Implementations;

public class SamplesService : ISamplesService
{
    private const string SERVER_URL = "http://localhost:5001";
    
    private readonly ILogger<ISamplesService> _logger;
    private readonly IEssentialsHttpClient _httpClient;

    public SamplesService(
        ILogger<ISamplesService> logger,
        IEssentialsHttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
    public async Task RunGetSamples()
    {
        await RunGetSample_GetPersonsInJson();
        await RunGetSample_GetPersonsInXml();
    }

    public async Task RunPostSamples()
    {
        await RunPostSample_GetPersonsInJson();
        await RunPostSample_GetPersonsInXml();
        await RunPostSample_GetPersonsInText();
    }

    private async Task RunGetSample_GetPersonsInJson()
    {
        var uriValidation = await EssentialsUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("get", "GetPersonsInJson")
            .WithUriParam("age", "26")
            .BuildAsync();

        var requestValidation = await EssentialsRequestBuilder
            .CreateBuilder(uriValidation)
            .WithHeader("personName", "as")
            .BuildAsync<SamplesService>();

        var persons = await _httpClient
            .GetAsync(requestValidation)
            .ReceiveNativeJsonContentAsync<List<Person>>();
    }

    private async Task RunGetSample_GetPersonsInXml()
    {
        var uriValidation = await EssentialsUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("get", "GetPersonsInXml")
            .BuildAsync();

        var requestValidation = await EssentialsRequestBuilder
            .CreateBuilder(uriValidation)
            .BuildAsync<SamplesService>();

        var persons = await _httpClient
            .GetAsync(requestValidation)
            .ReceiveXmlContentAsync<List<Person>>();
    }

    private async Task RunPostSample_GetPersonsInJson()
    {
        var data = new GetPersonsInJsonRequest
        {
            Name = "as"
        };
        
        var uriValidation = await EssentialsUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("post", "GetPersonsInJson")
            .BuildAsync();

        var requestValidation = await EssentialsRequestBuilder
            .CreateBuilder(uriValidation)
            .BuildAsync<SamplesService>();

        var response = await _httpClient
            .PostJsonDataAsync(requestValidation, data)
            .ReceiveJsonContentAsync<List<Person>>();
    }

    private async Task RunPostSample_GetPersonsInXml()
    {
        var data = new GetPersonsInXmlRequest
        {
            Name = "mi"
        };
        
        var uriValidation = await EssentialsUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("post", "GetPersonsInXml")
            .BuildAsync();

        var requestValidation = await EssentialsRequestBuilder
            .CreateBuilder(uriValidation)
            .BuildAsync<SamplesService>();

        var response = await _httpClient
            .PostApplicationXmlDataAsync(requestValidation, data)
            .ReceiveXmlContentAsync<List<Person>>();
    }

    private async Task RunPostSample_GetPersonsInText()
    {
        var data = new GetPersonsInXmlRequest
        {
            Name = "mi"
        };
        
        var requestString = SerializeInXml(data);
        
        var uriValidation = await EssentialsUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("post", "GetPersonsInPlainXmlText")
            .BuildAsync();

        var requestValidation = await EssentialsRequestBuilder
            .CreateBuilder(uriValidation)
            .BuildAsync<SamplesService>();

        var response = await _httpClient
            .PostStringAsync<TextPlain>(requestValidation, requestString)
            .ReceiveXmlContentAsync<List<Person>>();
    }
}