using System.Text;
using System.Threading.RateLimiting;
using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.Json;
using System.Collections.Concurrent;

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

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    // Global rate limiter for general API usage
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.Request.Headers["X-Forwarded-For"].ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Configure rate limit options
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        
        // Create error response
        var response = new
        {
            Status = 429,
            Title = "Too Many Requests",
            Detail = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                ? $"Too many requests. Please try again after {retryAfter.TotalSeconds} seconds."
                : "Too many requests. Please try again later.",
            RetryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue)
                ? (int)retryAfterValue.TotalSeconds
                : 60
        };
        
        // Set retry-after header
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var timeSpan))
        {
            context.HttpContext.Response.Headers.RetryAfter = ((int)timeSpan.TotalSeconds).ToString();
        }
        
        await context.HttpContext.Response.WriteAsJsonAsync(response, token);
    };
});
    
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
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddControllers();

// Add singleton for tracking login attempts
builder.Services.AddSingleton<LoginRateLimiter>();

var app = builder.Build();

app.UseHttpsRedirection();

// Apply CORS before authentication
app.UseCors("AllowBlazorClient");

app.UseAuthentication();  
app.UseAuthorization();

// Add custom middleware for login rate limiting
app.UseMiddleware<LoginRateLimitingMiddleware>();

// Apply global rate limiting to all requests
app.UseRateLimiter();

await app.UseOcelot();
app.MapControllers();
app.UseMiddleware<Gateway.Middleware.ResponseLoggingMiddleware>();

app.Run();

// Custom login rate limiter implementation
public class LoginRateLimiter
{
    private readonly ConcurrentDictionary<string, List<DateTime>> _loginAttempts = new();
    private readonly TimeSpan _windowDuration = TimeSpan.FromMinutes(5);
    private readonly int _maxAttempts = 5;

    public bool CheckAndRecordLoginAttempt(string ipAddress)
    {
        var now = DateTime.UtcNow;
        var attempts = _loginAttempts.GetOrAdd(ipAddress, _ => new List<DateTime>());

        // Clean up old attempts outside the window
        attempts.RemoveAll(time => now - time > _windowDuration);

        // Check if we're still under the limit
        if (attempts.Count < _maxAttempts)
        {
            attempts.Add(now);
            return true; // Allow the request
        }

        return false; // Reject the request
    }

    public TimeSpan GetRemainingTime(string ipAddress)
    {
        if (_loginAttempts.TryGetValue(ipAddress, out var attempts) && attempts.Count > 0)
        {
            var oldestAttempt = attempts.Min();
            var remainingTime = _windowDuration - (DateTime.UtcNow - oldestAttempt);
            return remainingTime > TimeSpan.Zero ? remainingTime : TimeSpan.Zero;
        }

        return TimeSpan.Zero;
    }
}

public class LoginRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly LoginRateLimiter _rateLimiter;

    public LoginRateLimitingMiddleware(RequestDelegate next, LoginRateLimiter rateLimiter)
    {
        _next = next;
        _rateLimiter = rateLimiter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if this is the login endpoint
        if (context.Request.Path.StartsWithSegments("/api/auth/login") && 
            context.Request.Method == "POST")
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? 
                         context.Request.Headers["X-Forwarded-For"].ToString();

            if (!_rateLimiter.CheckAndRecordLoginAttempt(ipAddress))
            {
                // Rate limit exceeded
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "application/json";
                
                var remainingTime = _rateLimiter.GetRemainingTime(ipAddress);
                var remainingSeconds = (int)Math.Ceiling(remainingTime.TotalSeconds);
                
                var response = new
                {
                    Status = 429,
                    Title = "Too Many Login Attempts",
                    Detail = $"You have exceeded the limit of 5 login attempts in 5 minutes. Please try again after {remainingSeconds} seconds.",
                    RetryAfter = remainingSeconds
                };
                
                // Set retry-after header
                context.Response.Headers.RetryAfter = remainingSeconds.ToString();
                
                await context.Response.WriteAsJsonAsync(response);
                return;
            }
        }
        
        await _next(context);
    }
}

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
            try
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
            catch
            {
                Console.WriteLine("Error logging responses");
            }
        }
    }
}
