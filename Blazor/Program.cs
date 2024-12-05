using Blazor;
using Blazor.Services;
using Blazored.SessionStorage;
using Common.Notification.Email;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7500") });
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AuthStateService>();

builder.Services.AddBlazoredSessionStorage();

await builder.Build().RunAsync();