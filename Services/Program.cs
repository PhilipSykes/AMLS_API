using Common;
using Common.Database;
using Common.Database.Interfaces;
using MongoDB.Bson;
using Services.NotificationService;
using Services.SearchService;

var builder = Host.CreateApplicationBuilder(args);

// Add RabbitMQ config
builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));

// Add MongoDB config
builder.Services.Configure<MongoDBConfig>(
    builder.Configuration.GetSection("MongoDB"));

// Register services
builder.Services.AddScoped<IDatabaseConnection, DatabaseConnection>();
builder.Services.AddScoped<IFilterBuilder<BsonDocument>, BsonFilterBuilder>();
builder.Services.AddScoped<ISearchRepository, SearchRepository>();
builder.Services.AddScoped<IMediaSearchService, MediaSearchService>();

builder.Services.AddHostedService<NotificationMessageReceiver>();

var host = builder.Build();
host.Run();