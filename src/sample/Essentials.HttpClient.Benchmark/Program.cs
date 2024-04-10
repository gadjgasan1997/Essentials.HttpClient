using NLog;
using BenchmarkDotNet.Running;
using Essentials.HttpClient.Benchmark;
using Essentials.HttpClient.Extensions;
using Microsoft.AspNetCore.Builder;

LogManager.Setup(setupBuilder => setupBuilder.LoadConfigurationFromFile("nlog.config"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureEssentialsHttpClient(builder.Configuration);

var app = builder.Build();

BenchmarkRunner.Run<CreateRequestsTests>();

app.Run();