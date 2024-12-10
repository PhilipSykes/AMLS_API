using Common;
using MessageBroker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<Exchange>();

var app = builder.Build();
var exchange = app.Services.GetRequiredService<Exchange>();
await exchange.InitializeConnection();

app.Run();
