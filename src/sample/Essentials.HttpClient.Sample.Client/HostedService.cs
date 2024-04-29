namespace Essentials.HttpClient.Sample.Client;

public class HostedService : IHostedService
{
    private readonly IGetRequestsSamplesService _getRequestsSamplesService;
    private readonly IHeadRequestsSamplesService _headRequestsSamplesService;
    private readonly IPostRequestsSamplesService _postRequestsSamplesService;
    
    public HostedService(
        IGetRequestsSamplesService getRequestsSamplesService,
        IHeadRequestsSamplesService headRequestsSamplesService,
        IPostRequestsSamplesService postRequestsSamplesService)
    {
        _getRequestsSamplesService = getRequestsSamplesService;
        _headRequestsSamplesService = headRequestsSamplesService;
        _postRequestsSamplesService = postRequestsSamplesService;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _getRequestsSamplesService.RunSamples();
        await _headRequestsSamplesService.RunSamples();
        await _postRequestsSamplesService.RunSamples();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}