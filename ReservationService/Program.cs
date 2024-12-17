using System.Text;
using Common;
using Common.Database;
using Common.MessageBroker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using ReservationService;
using ReservationService.Configuration;
using static Common.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

//Dictates which app settings doc to use (Docker/Local)
string appSettingsFileName = builder.Environment.EnvironmentName == "Docker" 
    ? "appsettings.Docker.json" 
    : "appsettings.json";

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(appSettingsFileName, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

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
var jwtConfig = builder.Configuration.GetSection("JWTToken").Get<JWTTokenConfig>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey))

        };
    });

builder.Services.AddScoped<IDatabaseConnection, DatabaseConnection>();
builder.Services.AddScoped<IFilterBuilder<BsonDocument>, BsonFilterBuilder>();
builder.Services.AddScoped<ISearchRepository<Reservation>, SearchRepository<Reservation>>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
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