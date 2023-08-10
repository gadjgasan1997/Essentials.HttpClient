using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Sample;
using Essentials.HttpClient.Sample.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices(services =>
{
    services.ConfigureEssentialsHttpClient(builder.Configuration);
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