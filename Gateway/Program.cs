using System.Text;
using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


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
app.UseMiddleware<Gateway.Middleware.ResponseLoggingMiddleware>();

await app.UseOcelot();

app.Run();

namespace Gateway.Middleware
{
    public class ResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Save the original response body stream
            var originalResponseBody = context.Response.Body;

            try
            {
                // Create a memory stream to capture the response
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                // Continue processing the request
                await _next(context);

                // Capture and log the response
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                LogResponse(context, responseText);

                // Copy the response back to the original stream
                await responseBody.CopyToAsync(originalResponseBody);
            }
            finally
            {
                context.Response.Body = originalResponseBody;
            }
        }

        private void LogResponse(HttpContext context, string responseText)
        {
            var logPath = "Logs/ResponseLogs.json"; // Path to save logs
            var logEntry = new
            {
                Path = context.Request.Path,
                Method = context.Request.Method,
                Query = context.Request.QueryString.ToString(),
                Response = JsonSerializer.Deserialize<object>(responseText),
                Timestamp = System.DateTime.UtcNow
            };

            Directory.CreateDirectory("Logs"); // Ensure Logs folder exists
            File.AppendAllText(logPath, JsonSerializer.Serialize(logEntry) + "\n");
        }
    }
}
