using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization;

public static class AuthorizationPoliciesExtensions
{
    private const string PermissionClaimName = "permissions";
    public static AuthorizationOptions AddPermissionPolicies(this AuthorizationOptions options)
    {
        var policyValues = AuthorizationPolicies.GetAll();
        foreach (var policyValue in policyValues)
        {
            options.AddPolicy(policyValue.PolicyName, policy => policy.RequireClaim(PermissionClaimName, policyValue.PermissionName));
        }

        return options;
    }
}