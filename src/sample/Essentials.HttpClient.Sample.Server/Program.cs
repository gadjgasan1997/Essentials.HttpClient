var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddXmlSerializerFormatters();

var app = builder.Build();

app.UseRouting().UseEndpoints(routeBuilder => routeBuilder.MapControllers());

app.Run();