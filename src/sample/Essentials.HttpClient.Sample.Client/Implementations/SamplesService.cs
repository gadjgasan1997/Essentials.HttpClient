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

        
    }

    private async Task RunSample_GetJson()
    {
        _logger.LogInformation(nameof(RunSample_GetJson));
        
        var uriValidation = await EssentialsUriBuilder
            .CreateBuilder(SERVER_URL)
            .BuildAsync();

        var requestValidation = await EssentialsRequestBuilder
            .CreateBuilder(uriValidation)
            .BuildAsync<SamplesService>();
    }
}