using Api.MessageBroker;
using Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Add RabbitMQ configuration
builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<Exchange>();


builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();


app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();


// Initialize RabbitMQ Exchange as a background process
var exchange = app.Services.GetRequiredService<Exchange>();
await exchange.InitializeConnection();


app.Run();