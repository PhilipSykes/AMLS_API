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
            
            options.AddPolicy(Policies.CanViewStaff, policy => 
                policy.RequireRole(PolicyRoles.SystemAdmin, PolicyRoles.BranchManager));
            
            options.AddPolicy(Policies.CanViewMembers, policy => 
                policy.RequireRole(PolicyRoles.SystemAdmin, PolicyRoles.BranchLibrarian, PolicyRoles.BranchManager));
            
            options.AddPolicy(Policies.CanDeleteUserAccounts, policy => 
                policy.RequireRole(PolicyRoles.SystemAdmin));
            
        });
    }
}