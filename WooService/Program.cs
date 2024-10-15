using WooService.Workers;
using WooService.Contexts;
using WooService.Models;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

AppSettings? appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.AddSingleton(appSettings!);
builder.Services.AddDbContext<AXContext>();
builder.Services.AddDbContextFactory<WooCommerceContext>();
builder.Services.AddHostedService<Worker>();




var host = builder.Build();

host.Run();
