using Common.Constants;

namespace Api;

public static class Policies
{
    //Member permissions
    public const string CanReserveMedia = "CanReserveMedia";
    public const string CanCancelMedia = "CanCancelMedia";
    public const string CanBorrowMedia = "CanBorrowMedia";
    public const string CanExtendReservation = "CanExtendReservation";
    public const string CanReturnMedia = "CanReturnMedia";
    
    //Branch staff permissions
    public const string HasBranchAccess = "HasBranchAccess";
    public const string CanEditMedia = "CanEditMedia";
    public const string CanCreateMedia = "CanCreateMedia";
    public const string CanDeleteMedia = "CanDeleteMedia";
    public const string CanViewInventory = "CanViewInventory";
    
    //SystemAdmin permissions 
    public const string CanEditUserRoles = "RequireEditUsers";
    public const string CanEditUserPermissions = "CanEditUserPermissions";
    public const string CanViewMetricsReports = "CanViewMetricsReports";
        
    public static void ConfigurePolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            //Member 
            options.AddPolicy(CanReserveMedia, policy => 
                policy.RequireRole(PolicyRoles.Member));
            options.AddPolicy(CanCancelMedia, policy => 
                policy.RequireRole(PolicyRoles.Member));
            options.AddPolicy(CanReturnMedia, policy => 
                policy.RequireRole(PolicyRoles.Member));
            options.AddPolicy(CanBorrowMedia, policy =>
                policy.RequireRole(PolicyRoles.Member));
            options.AddPolicy(CanExtendReservation, policy => 
                policy.RequireRole(PolicyRoles.Member));

            //Branch staff
            //Review branch access logic for require policy 
            options.AddPolicy(HasBranchAccess, policy => 
                policy.RequireClaim(PolicyClaims.BranchAccess));
            
            options.AddPolicy(CanEditMedia, policy => 
                policy.RequireRole(PolicyRoles.BranchLibrarian,PolicyRoles.BranchManager));
            
            options.AddPolicy(CanCreateMedia, policy => 
                policy.RequireRole(PolicyRoles.BranchLibrarian,PolicyRoles.BranchManager));
            
            options.AddPolicy(CanDeleteMedia, policy => 
                policy.RequireRole(PolicyRoles.BranchLibrarian,PolicyRoles.BranchManager));
            
            options.AddPolicy(CanViewInventory, policy => 
                policy.RequireRole(PolicyRoles.BranchLibrarian,PolicyRoles.BranchManager));
            
            //System Admin
            options.AddPolicy(CanEditUserRoles, policy => 
                policy.RequireRole(PolicyRoles.SystemAdmin));
            
            options.AddPolicy(CanEditUserPermissions, policy => 
                policy.RequireRole(PolicyRoles.SystemAdmin));
            
            options.AddPolicy(CanViewMetricsReports, policy => 
                policy.RequireRole(PolicyRoles.SystemAdmin));
            
        });
    }
}