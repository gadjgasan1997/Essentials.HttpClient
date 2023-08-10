var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices(services => services.AddControllers());

var app = builder.Build();

app.UseRouting().UseEndpoints(routeBuilder => routeBuilder.MapControllers());

app.Run();