using Common;
using Services.NotificationService;
using Services.SearchService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddHostedService<NotificationMessageReceiver>();
builder.Services.AddHostedService<SearchServiceMessageReceiver>();


var host = builder.Build();
host.Run();

