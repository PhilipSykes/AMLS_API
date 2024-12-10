using Common;
using Common.Database;
using Common.Models;
using static Common.Models.Entities;
using MediaService;
using MediaService.Configuration;
using MongoDB.Bson;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Add RabbitMQ configuration
builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));

//Add Mongo configuration
builder.Services.Configure<MongoDBConfig>(
    builder.Configuration.GetSection("MongoDB"));

//Add JWTToken config
builder.Services.Configure<JWTTokenConfig>(
    builder.Configuration.GetSection("JWTToken"));

builder.Services.AddScoped<IDatabaseConnection, DatabaseConnection>();
builder.Services.AddScoped<IFilterBuilder<BsonDocument>, BsonFilterBuilder>();
builder.Services.AddScoped<ISearchRepository<MediaInfo>, SearchRepository<MediaInfo>>();
builder.Services.AddScoped<IInventoryManager, InventoryManager>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthorization();
PolicyConfig.ApplyPolicies(builder.Services);

var app = builder.Build();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();