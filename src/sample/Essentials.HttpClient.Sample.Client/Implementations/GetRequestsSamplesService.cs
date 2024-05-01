using LanguageExt;
using LanguageExt.Common;
using System.Diagnostics.CodeAnalysis;
using Essentials.Functional.Extensions;
using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Sample.Client.Models;
using static Essentials.HttpClient.Sample.Client.Dictionaries.CommonConsts;

namespace Essentials.HttpClient.Sample.Client.Implementations;

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
        await RunSample_GetPersonsInJson_WithCache();
        await RunSample_GetPersonsInXml();
        await RunSample_StaticClient();
    }

    private async Task RunSample_GetPersonsInJson()
    {
        var uriValidation = await HttpUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("get", "GetPersonsInJson")
            .WithUriParam("age", "26")
            .BuildAsync();

        var requestValidation = await HttpRequestBuilder
            .CreateBuilder(uriValidation)
            .SetTypeId("GetPersonsInJson")
            .SetTimeout(TimeSpan.FromSeconds(5))
            .WithHeader("personName", "as")
            .BuildAsync<GetRequestsSamplesService>();
    
        var persons = await _httpClient
            .GetAsync(requestValidation)
            .ReceiveNativeJsonContentAsync<List<Person>>();
    }

    private async Task RunSample_GetPersonsInJson_WithCache()
    {
        var uriValidation = await HttpUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("get", "GetPersonsInJson")
            .WithUriParam("age", "26")
            .BuildAsync();

        const string requestId = "persons_list";
        
        var request = await HttpRequestBuilder.GetFromCacheOrCreateAsync(requestId, Creator);
        var persons = await _httpClient
            .GetAsync(request)
            .ReceiveNativeJsonContentUnsafeAsync<List<Person>>();
        
        var request2 = await HttpRequestBuilder.GetFromCacheOrCreateAsync(requestId, Creator);
        var persons2 = await _httpClient
            .GetAsync(request2)
            .ReceiveNativeJsonContentUnsafeAsync<List<Person>>();
        
        var request3 = await HttpRequestBuilder.GetFromCacheOrCreateAsync(requestId, Creator);
        var persons3 = await _httpClient
            .GetAsync(request3)
            .ReceiveNativeJsonContentUnsafeAsync<List<Person>>();

        var request4 = await HttpRequestBuilder.GetFromCacheOrCreateAsync(requestId, Creator);
        var persons4 = await _httpClient
            .GetAsync(request4)
            .ReceiveNativeJsonContentUnsafeAsync<List<Person>>();
        
        return;

        Task<Validation<Error, IRequest>> Creator() =>
            HttpRequestBuilder
                .CreateBuilder(uriValidation)
                .WithRequestId("Cached_Request_Id")
                .SetTypeId("GetPersonsInJson_WithCache")
                .WithHeader("personName", "as")
                .BuildAsync<GetRequestsSamplesService>();
    }

    private async Task RunSample_GetPersonsInXml()
    {
        var uriValidation = await HttpUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("get", "GetPersonsInXml")
            .BuildAsync();

        var requestValidation = await HttpRequestBuilder
            .CreateBuilder(uriValidation)
            .SetTypeId("GetPersonsInXml")
            .BuildAsync<GetRequestsSamplesService>();

        var persons = await _httpClient
            .GetAsync(requestValidation)
            .ReceiveXmlContentAsync<List<Person>>();
    }

    private static async Task RunSample_StaticClient()
    {
        var uriValidation = await HttpUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("get", "GetPersonsInXml")
            .BuildAsync();

        var requestValidation = await HttpRequestBuilder
            .CreateBuilder(uriValidation)
            .SetTypeId("GetPersonsInXml")
            .BuildAsync<GetRequestsSamplesService>();
        
        var persons = await HttpClientsHolder
            .Peek()
            .BindAsync(client => client.GetAsync(requestValidation))
            .ReceiveXmlContentUnsafeAsync<List<Person>>();
    }
}