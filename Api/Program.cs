using System.Text.Json.Serialization;
using Api.MessageBroker;
using Common;
using Common.Database;
using Common.Database.Interfaces;
using Services.SearchService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Add RabbitMQ configuration
builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<Exchange>();

//Add Mongo configuration
builder.Services.Configure<MongoDBConfig>(
    builder.Configuration.GetSection("MongoDB"));

// Register Database Services
builder.Services.AddScoped<IDatabaseConnection, DatabaseConnection>();
builder.Services.AddScoped<ISearchRepository, SearchRepository>();

// Register Application Services
builder.Services.AddScoped<IMediaSearchService, MediaSearchService>();
builder.Services.AddScoped<IUserSearchService, UserSearchService>();

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

