using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Sample.Client.Models;
using Essentials.HttpClient.Sample.Client.Models.Requests;
using TextPlain = Essentials.HttpClient.MediaTypes.Text.Plain;
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
        await RunSample_GetPersonsInText();
    }
    
    private async Task RunSample_GetPersonsInJson()
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
        
        var uriValidation = await EssentialsUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("post", "GetPersonsInXml")
            .BuildAsync();

        var requestValidation = await EssentialsRequestBuilder
            .CreateBuilder(uriValidation)
            .BuildAsync<PostRequestsSamplesService>();

        var response = await _httpClient
            .PostApplicationXmlDataAsync(requestValidation, data)
            .ReceiveXmlContentAsync<List<Person>>();
    }

    private async Task RunSample_GetPersonsInText()
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
            .BuildAsync<PostRequestsSamplesService>();

        var response = await _httpClient
            .PostStringAsync<TextPlain>(requestValidation, requestString)
            .ReceiveXmlContentAsync<List<Person>>();
    }
}