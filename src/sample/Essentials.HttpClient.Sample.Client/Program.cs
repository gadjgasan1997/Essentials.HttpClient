using NLog;
using NLog.Web;
using NLog.Extensions.Logging;
using Essentials.HttpClient.Sample.Client;
using Essentials.HttpClient.Sample.Client.Extensions;

LogManager.Setup(setupBuilder => setupBuilder.LoadConfigurationFromFile("nlog.config"));

var builder = new HostBuilder();

builder
    .ConfigureAppConfiguration(config =>
        config.AddJsonFile("appsettings.json").AddEnvironmentVariables())
    .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders().AddNLog())
    .UseNLog(NLogAspNetCoreOptions.Default)
    .ConfigureWebHostDefaults(host => host.Configure(applicationBuilder => applicationBuilder.UseRouting().Build()))
    .ConfigureServices((host, services) => services.ConfigureSampleClient(host.Configuration));

var app = builder.Build();

var getRequestsSamplesService = app.Services.GetRequiredService<IGetRequestsSamplesService>();
var headRequestsSamplesService = app.Services.GetRequiredService<IHeadRequestsSamplesService>();
var postRequestsSamplesService = app.Services.GetRequiredService<IPostRequestsSamplesService>();

await getRequestsSamplesService.RunSamples();
await headRequestsSamplesService.RunSamples();
await postRequestsSamplesService.RunSamples();

app.Run();