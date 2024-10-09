using WooService.Workers;
using WooService.Contexts;
using WooService.Models;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
AppSettings? appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.AddSingleton(appSettings!);
builder.Services.AddDbContext<WooCommerceContext>();
builder.Services.AddDbContext<AXContext>();

host.Run();
