var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices(services => services.AddControllers().AddXmlSerializerFormatters());

var app = builder.Build();

app.UseRouting().UseEndpoints(routeBuilder => routeBuilder.MapControllers());

app.Run();