using Essentials.HttpClient.Extensions;
using Essentials.HttpClient.Sample;
using Essentials.HttpClient.Sample.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices(services =>
{
    services.ConfigureEssentialsHttpClient(builder.Configuration);
    services.AddTransient<ISamplesService, SamplesService>();
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();