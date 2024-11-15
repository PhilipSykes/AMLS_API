using Common.Configuration;
using Services.NotificationService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddHostedService<NotificationMessageReceiver>();

var host = builder.Build();
host.Run();

