using Essentials.HttpClient.Common.Models;
using Essentials.HttpClient.Common.Models.Requests;
using Essentials.HttpClient.Extensions;

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
    
    public async Task RunSamples()
    {
        await RunGetRequestsSamples();
        await RunPostRequestsSamples();
    }

    private async Task RunGetRequestsSamples()
    {
        await RunSample_GetJson();
    }

    private async Task RunPostRequestsSamples()
    {
        var request = new GetPersonsInJsonRequest
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
            .PostJsonDataAsync(requestValidation, request)
            .ReceiveJsonContentAsync<List<Person>>();
    }

    private async Task RunSample_GetJson()
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
            .ReceiveJsonContentUnsafeAsync<List<Person>>();
    }
}