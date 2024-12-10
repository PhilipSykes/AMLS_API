using System.Text;
using Api;
using Api.MessageBroker;
using Common;
using Common.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Services.MediaService;
using Services.ReservationService;
using Services.TokenAuthService;
using Services.UserService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// Add RabbitMQ configuration
builder.Services.Configure<RabbitMQConfig>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<Exchange>();

//Add Mongo configuration
builder.Services.Configure<MongoDBConfig>(
    builder.Configuration.GetSection("MongoDB"));

//Add JWTToken config
builder.Services.Configure<JWTTokenConfig>(
    builder.Configuration.GetSection("JWTToken"));

builder.Services.AddHttpContextAccessor();

// Register Database Services
builder.Services.AddScoped<IDatabaseConnection, DatabaseConnection>();
builder.Services.AddScoped<ISearchRepository, SearchRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

// Register Application Services
builder.Services.AddScoped<IMediaSearch, MediaSearch>();
builder.Services.AddScoped<IReservationCreator, ReservationCreator>();
builder.Services.AddScoped<IUserSearch, UserSearch>();
builder.Services.AddScoped<IInventoryManager, InventoryManager>();
builder.Services.AddScoped<TokenAuthService>();

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
builder.Services.AddAuthorization();
Policies.ConfigurePolicies(builder.Services);

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


// Initialize RabbitMQ Exchange as a background process
var exchange = app.Services.GetRequiredService<Exchange>();
await exchange.InitializeConnection();


app.Run();