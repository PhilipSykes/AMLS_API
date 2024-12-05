using Common;
using Common.Database;
using Common.Database.Interfaces;
using Common.Notification.Email;
using MongoDB.Bson;
using Services.MediaService;
using Services.NotificationService;
using Services.UserService;

var builder = Host.CreateApplicationBuilder(args);

// Add RabbitMQ config
builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));

// Register services
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddScoped<IDatabaseConnection, DatabaseConnection>();
builder.Services.AddScoped<IFilterBuilder<BsonDocument>, BsonFilterBuilder>();
builder.Services.AddScoped<ISearchRepository, SearchRepository>();
builder.Services.AddScoped<IMediaSearch, MediaSearch>();
builder.Services.AddScoped<IUserSearch, UserSearch>();

//RabbitMQ receivers 
builder.Services.AddHostedService<NotificationManager>();


var host = builder.Build();
host.Run();