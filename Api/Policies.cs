using Common.Constants;

namespace Api;

public static class Policies
{
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireLibrarian = "RequireLibrarian";
    public const string RequireManager = "RequireManager";
    public const string RequireAccountant = "RequireAccountant";
    public const string RequireMember = "RequireMember";
    public const string RequireBranchAccess = "RequireBranchAccess";
    public static void ConfigurePolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(RequireAdmin, policy => 
                policy.RequireClaim(PolicyClaims.AdminClaim));
            
            options.AddPolicy(RequireLibrarian, policy => 
                policy.RequireClaim(PolicyClaims.LibrarianClaim));
                
            options.AddPolicy(RequireManager, policy => 
                policy.RequireClaim(PolicyClaims.ManagerClaim));
                
            options.AddPolicy(RequireAccountant, policy => 
                policy.RequireClaim(PolicyClaims.AccountantClaim));
                
            options.AddPolicy(RequireMember, policy => 
                policy.RequireClaim(PolicyClaims.MemberClaim));
            
            options.AddPolicy(RequireBranchAccess, policy => 
                policy.RequireClaim(PolicyClaims.BranchAccess));
        });
    }
}