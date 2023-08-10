using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Common.Models;
using Essentials.HttpClient.Extensions;
using TextPlain = Essentials.HttpClient.MediaTypes.Text.Plain;
using static Essentials.HttpClient.Sample.Dictionaries.CommonConsts;

namespace Essentials.HttpClient.Sample.Implementations;

[SuppressMessage("ReSharper", "UnusedVariable")]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public class GetRequestsSamplesService : IGetRequestsSamplesService
{
    private readonly ILogger<GetRequestsSamplesService> _logger;
    private readonly IEssentialsHttpClient _httpClient;

    public GetRequestsSamplesService(
        ILogger<GetRequestsSamplesService> logger,
        IEssentialsHttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
    public async Task RunSamples()
    {
        await RunSample_GetPersonsInJson();
        await RunSample_GetPersonsInXml();
    }

    private async Task RunSample_GetPersonsInJson()
    {
        var uriValidation = await EssentialsUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("get", "GetPersonsInJson")
            .WithUriParam("age", "26")
            .BuildAsync();

        var requestValidation = await EssentialsRequestBuilder
            .CreateBuilder(uriValidation)
            .WithHeader("personName", "as")
            .BuildAsync<GetRequestsSamplesService>();

        var persons = await _httpClient
            .GetAsync(requestValidation)
            .ReceiveNativeJsonContentAsync<List<Person>>();
    }

    private async Task RunSample_GetPersonsInXml()
    {
        var uriValidation = await EssentialsUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("get", "GetPersonsInXml")
            .BuildAsync();

        var requestValidation = await EssentialsRequestBuilder
            .CreateBuilder(uriValidation)
            .BuildAsync<GetRequestsSamplesService>();

        var persons = await _httpClient
            .GetAsync(requestValidation)
            .ReceiveXmlContentAsync<List<Person>>();
    }
}