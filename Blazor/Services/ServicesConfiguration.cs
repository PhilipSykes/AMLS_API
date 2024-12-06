using Blazored.SessionStorage;
using Common.Notification.Email;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor.Services;

public static class ServiceConfiguration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddBlazoredSessionStorage();
        services.AddAuthorizationCore();
        services.AddScoped<EmailService>();
        services.AddScoped<AuthStateService>(); 
        services.AddScoped<AuthenticationStateProvider>(sp => 
            sp.GetRequiredService<AuthStateService>());
    }
}