using Common.Constants;

namespace ReservationService.Configuration;

public static class PolicyConfig
{
    public static void ApplyPolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
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
        });
    }
}