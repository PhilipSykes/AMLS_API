using Common.Constants;

namespace UserService.Configuration;

public static class PolicyConfig
{
    public static void ApplyPolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanEditUserRoles, policy => 
                policy.RequireRole(PolicyRoles.SystemAdmin));
            
            options.AddPolicy(Policies.CanEditUserPermissions, policy => 
                policy.RequireRole(PolicyRoles.SystemAdmin));
        });
    }
}