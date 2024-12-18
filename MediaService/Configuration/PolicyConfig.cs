using Common.Constants;

namespace MediaService.Configuration;

public static class PolicyConfig
{
    public static void ApplyPolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanEditMedia, policy => 
                policy.RequireRole(PolicyRoles.BranchLibrarian, PolicyRoles.BranchManager));

            options.AddPolicy(Policies.CanCreateMedia, policy => 
                policy.RequireRole(PolicyRoles.BranchLibrarian, PolicyRoles.BranchManager));
            
            options.AddPolicy(Policies.CanDeleteMedia, policy => 
                policy.RequireRole(PolicyRoles.BranchLibrarian, PolicyRoles.BranchManager));
            
            options.AddPolicy(Policies.CanViewInventory, policy => 
                policy.RequireRole(PolicyRoles.BranchLibrarian, PolicyRoles.BranchManager));
            
            options.AddPolicy(Policies.HasBranchAccess, policy => 
                policy.RequireClaim(PolicyClaims.BranchAccess));
        });
    }
}