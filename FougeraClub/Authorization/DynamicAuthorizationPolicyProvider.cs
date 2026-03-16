using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace KhaledTeamRecycling.Authorization
{
    public class DynamicAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public DynamicAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // Build a policy dynamically based on the name
            var policy = new AuthorizationPolicyBuilder();
            policy.RequireAuthenticatedUser(); // optional
            policy.RequireClaim("permission", policyName); // dynamic requirement based on name
            return Task.FromResult(policy.Build());
        }
    }

}
