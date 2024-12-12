using Common;
using Common.Notification.Email;
using NotificationService;
using Common.MessageBroker;

var builder = WebApplication.CreateBuilder(args);

//Dictates which app settings doc to use (Docker/Local)
string appSettingsFileName = builder.Environment.EnvironmentName == "Docker" 
    ? "appsettings.Docker.json" 
    : "appsettings.json";

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(appSettingsFileName, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure RabbitMQ
builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));

// Add MessageBroker
builder.Services.AddMessageBroker();

// Add other services
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<NotificationManager>();

var app = builder.Build();

var exchange = app.Services.GetRequiredService<Exchange>();
await exchange.EnsureInitialized();

app.Run();