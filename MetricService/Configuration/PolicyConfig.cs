using Common.Constants;


namespace MetricService.Configuration;

public static class PolicyConfig
{
    public static void ApplyPolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanViewMetricsReports, policy => 
                policy.RequireRole(PolicyRoles.SystemAdmin));
        });
    }
}