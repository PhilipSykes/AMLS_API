using System.Text;
using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Dictates which app & ocelot settings docs to use (Docker/Local)
string appSettingsFileName = builder.Environment.EnvironmentName == "Docker" 
    ? "appsettings.Docker.json" 
    : "appsettings.json";

string ocelotFileName = builder.Environment.EnvironmentName == "Docker" 
    ? "ocelot.Docker.json" 
    : "ocelot.json";

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(appSettingsFileName, optional: false, reloadOnChange: true)
    .AddJsonFile(ocelotFileName, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy => policy.WithOrigins("https://localhost:7001")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowBlazorClient");
app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();  
app.UseAuthorization();

await app.UseOcelot();

app.Run();