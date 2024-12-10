using Common;
using Common.Notification.Email;
using NotificationService;
using Common.MessageBroker;

var builder = WebApplication.CreateBuilder(args);

// Configure RabbitMQ
builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));

// Add MessageBroker
builder.Services.AddMessageBroker();

// Add other services
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<NotificationManager>();

var app = builder.Build();

// Initialize the Exchange
var exchange = app.Services.GetRequiredService<Exchange>();
await exchange.EnsureInitialized();

app.Run();