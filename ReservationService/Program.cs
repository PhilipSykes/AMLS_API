using Common;
using Common.Database;
using Common.MessageBroker;
using MongoDB.Bson;
using ReservationService;
using ReservationService.Configuration;
using static Common.Models.Entities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Add RabbitMQ configuration
builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddMessageBroker();

//Add Mongo configuration
builder.Services.Configure<MongoDBConfig>(
    builder.Configuration.GetSection("MongoDB"));

//Add JWTToken config
builder.Services.Configure<JWTTokenConfig>(
    builder.Configuration.GetSection("JWTToken"));

builder.Services.AddScoped<IDatabaseConnection, DatabaseConnection>();
builder.Services.AddScoped<IFilterBuilder<BsonDocument>, BsonFilterBuilder>();
builder.Services.AddScoped<ISearchRepository<Reservation>, SearchRepository<Reservation>>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationCreator, ReservationCreator>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthorization();
PolicyConfig.ApplyPolicies(builder.Services);



var app = builder.Build();
var exchange = app.Services.GetRequiredService<Exchange>();
await exchange.EnsureInitialized();

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();