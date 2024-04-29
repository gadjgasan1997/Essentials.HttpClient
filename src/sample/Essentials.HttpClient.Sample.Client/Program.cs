using NLog;
using Essentials.Configuration.Helpers;
using Essentials.Configuration.Extensions;
using Essentials.HttpClient.Sample.Client;
using Essentials.HttpClient.Sample.Client.Extensions;

LogManager.Setup(setupBuilder => setupBuilder.LoadConfigurationFromFile(
    LoggingHelpers.GetNLogConfigPath("nlog.config")));

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureDefault(
    configureServicesAction: (host, services) =>
        services
            .ConfigureSampleClient(host.Configuration)
            .AddHostedService<HostedService>());

var app = builder.Build();

app.Run();