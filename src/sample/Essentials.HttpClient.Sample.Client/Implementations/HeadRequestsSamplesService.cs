using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Events;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.Metrics;
using Essentials.HttpClient.Extensions;
using static Essentials.HttpClient.Sample.Client.Dictionaries.CommonConsts;

namespace Essentials.HttpClient.Sample.Client.Implementations;

[SuppressMessage("ReSharper", "UnusedVariable")]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public class HeadRequestsSamplesService : IHeadRequestsSamplesService
{
    private readonly ILogger<HeadRequestsSamplesService> _logger;
    private readonly IEssentialsHttpClient _httpClient;

    public HeadRequestsSamplesService(
        ILogger<HeadRequestsSamplesService> logger,
        IEssentialsHttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    
    public async Task RunSamples()
    {
        await RunSample_Head();
    }

    private async Task RunSample_Head()
    {
        var uriValidation = await HttpUriBuilder
            .CreateBuilder(SERVER_URL)
            .WithSegments("head", "Head")
            .BuildAsync();

        var requestValidation = await HttpRequestBuilder
            .CreateBuilder(uriValidation)
            .SetTypeId("Head")
            .WithHeader("User-Agent", "C# Program")
            .WithInterceptor<RequestsTimerInterceptor>()
            .WithInterceptor<LoggingInterceptor>()
            .WithInterceptor<MetricsInterceptor>()
            .BuildAsync<HeadRequestsSamplesService>();

        var response = await _httpClient
            .HeadAsync(requestValidation)
            .ReceiveMessageUnsafeAsync();
    }
}