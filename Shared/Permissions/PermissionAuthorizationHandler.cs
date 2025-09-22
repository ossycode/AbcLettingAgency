using AbcLettingAgency.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace AbcLettingAgency.Shared.Permissions;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    public PermissionAuthorizationHandler() { }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
     PermissionRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated == true &&
            context.User.HasClaim(AppClaim.Permission, requirement.Permission))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}