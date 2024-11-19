using Common.Configuration;
using Common.Notification.Email;
using Services.NotificationService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<NotificationMessageReceiver>();


var host = builder.Build();
host.Run();

