using Blazor;
using Blazor.Services;
using Blazored.SessionStorage;
using Common.Constants;
using Common.Notification.Email;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBlazoredSessionStorage();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7500") });

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy(Policies.CanReserveMedia, policy => 
        policy.RequireRole(PolicyRoles.Member));
    
    options.AddPolicy(Policies.CanCancelMedia, policy => 
        policy.RequireRole(PolicyRoles.Member));
    
    options.AddPolicy(Policies.CanReturnMedia, policy => 
        policy.RequireRole(PolicyRoles.Member));
    
    options.AddPolicy(Policies.CanBorrowMedia, policy =>
        policy.RequireRole(PolicyRoles.Member));
    
    options.AddPolicy(Policies.CanExtendReservation, policy => 
        policy.RequireRole(PolicyRoles.Member));
    
    options.AddPolicy(Policies.HasBranchAccess, policy => 
        policy.RequireRole(PolicyRoles.BranchLibrarian, PolicyRoles.BranchManager));
    
    options.AddPolicy("EditBranchMedia", policy => policy
        .RequireRole(PolicyRoles.BranchLibrarian, PolicyRoles.BranchManager)
        .RequireClaim(PolicyClaims.BranchAccess));
    
    options.AddPolicy(Policies.CanEditMedia, policy => 
        policy.RequireRole(PolicyRoles.BranchLibrarian, PolicyRoles.BranchManager));
    
    options.AddPolicy(Policies.CanDeleteMedia, policy => 
        policy.RequireRole(PolicyRoles.BranchLibrarian, PolicyRoles.BranchManager));
    
    options.AddPolicy(Policies.CanViewInventory, policy => 
        policy.RequireRole(PolicyRoles.BranchLibrarian, PolicyRoles.BranchManager));
    
    options.AddPolicy(Policies.CanViewStaff, policy => 
        policy.RequireRole(PolicyRoles.SystemAdmin, PolicyRoles.BranchManager));
            
    options.AddPolicy(Policies.CanViewMembers, policy => 
        policy.RequireRole(PolicyRoles.SystemAdmin, PolicyRoles.BranchLibrarian, PolicyRoles.BranchManager));

    
    options.AddPolicy(Policies.CanEditUserRoles, policy => 
        policy.RequireRole(PolicyRoles.SystemAdmin));
            
    options.AddPolicy(Policies.CanEditUserPermissions, policy => 
        policy.RequireRole(PolicyRoles.SystemAdmin));
    
    options.AddPolicy(Policies.CanViewMetricsReports, policy => 
        policy.RequireRole(PolicyRoles.SystemAdmin));
    
    
});


builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AuthenticationStateProvider, ClientAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();