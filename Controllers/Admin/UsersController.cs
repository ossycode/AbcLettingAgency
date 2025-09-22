using AbcLettingAgency.Attributes;
using AbcLettingAgency.Authorization;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AbcLettingAgency.Controllers.Admin;

[Route("api/[controller]")]
[ApiController]
public class UsersController(UserManager<AppUser> users, RoleManager<IdentityRole<Guid>> roles) : ControllerBase
{
    private readonly UserManager<AppUser> _users = users;
    private readonly RoleManager<IdentityRole<Guid>> _roles = roles;

    public sealed record UpdateUserRolesRequest(string[] Roles);
    public sealed record AddUserPermissionsRequest(string[] Permissions);

    [HttpGet("{userId}/roles")]
    [HasPermission(AppFeature.UserRoles, AppAction.Read)]
    public async Task<ActionResult<string[]>> GetUserRoles(string userId)
    {
        var u = await _users.FindByIdAsync(userId);
        if (u is null) return NotFound();
        var roles = await _users.GetRolesAsync(u);
        return Ok(roles.OrderBy(r => r).ToArray());
    }

    [HttpPost("{userId}/roles")]
    [HasPermission(AppFeature.UserRoles, AppAction.Update)]
    public async Task<IActionResult> AddUserRoles(string userId, [FromBody] UpdateUserRolesRequest req)
    {
        var u = await _users.FindByIdAsync(userId);
        if (u is null) return NotFound();

        var requested = (req.Roles ?? Array.Empty<string>()).Distinct().ToArray();
        var unknown = requested.Where(r => _roles.FindByNameAsync(r).Result is null).ToArray();
        if (unknown.Length > 0) return BadRequest($"Unknown roles: {string.Join(", ", unknown)}");

        var current = await _users.GetRolesAsync(u);
        var toAdd = requested.Except(current, StringComparer.Ordinal).ToArray();
        if (toAdd.Length > 0)
        {
            var res = await _users.AddToRolesAsync(u, toAdd);
            if (!res.Succeeded) return Problem(string.Join("; ", res.Errors.Select(e => e.Description)));
        }
        return NoContent();
    }

    [HttpDelete("{userId}/roles/{roleName}")]
    [HasPermission(AppFeature.UserRoles, AppAction.Update)]
    public async Task<IActionResult> RemoveUserRole(string userId, string roleName)
    {
        var u = await _users.FindByIdAsync(userId);
        if (u is null) return NotFound();

        if (AppRoles.IsDefault(roleName))
            return BadRequest("Default role membership changes are allowed, but ensure you keep at least one access path.");

        var res = await _users.RemoveFromRoleAsync(u, roleName);
        if (!res.Succeeded) return Problem(string.Join("; ", res.Errors.Select(e => e.Description)));
        return NoContent();
    }

    // ----- Optional: direct user permission claims (overrides/exceptions) -----

    [HttpGet("{userId}/permissions")]
    [HasPermission(AppFeature.RoleClaims, AppAction.Read)]
    public async Task<ActionResult<object>> GetUserPermissions(string userId)
    {
        var u = await _users.FindByIdAsync(userId);
        if (u is null) return NotFound();

        var userClaims = await _users.GetClaimsAsync(u);
        var direct = userClaims.Where(c => c.Type == AppClaim.Permission).Select(c => c.Value).OrderBy(x => x).ToArray();

        // effective = direct + role claims
        var roles = await _users.GetRolesAsync(u);
        var effective = new HashSet<string>(direct);
        foreach (var r in roles)
        {
            var role = await _roles.FindByNameAsync(r);
            if (role is null) continue;
            var rc = await _roles.GetClaimsAsync(role);
            foreach (var c in rc.Where(x => x.Type == AppClaim.Permission))
                effective.Add(c.Value);
        }

        return Ok(new { direct, effective = effective.OrderBy(x => x).ToArray() });
    }

    [HttpPost("{userId}/permissions")]
    [HasPermission(AppFeature.RoleClaims, AppAction.Update)]
    public async Task<IActionResult> AddUserPermissions(string userId, [FromBody] AddUserPermissionsRequest req)
    {
        var u = await _users.FindByIdAsync(userId);
        if (u is null) return NotFound();

        var toAdd = (req.Permissions ?? Array.Empty<string>()).Distinct().ToArray();
        var unknown = PermissionHelpers.UnknownPermissions(toAdd).ToArray();
        if (unknown.Length > 0) return BadRequest($"Unknown permissions: {string.Join(", ", unknown)}");

        var existing = (await _users.GetClaimsAsync(u))
            .Where(c => c.Type == AppClaim.Permission)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var p in toAdd.Except(existing))
            await _users.AddClaimAsync(u, new Claim(AppClaim.Permission, p));

        return NoContent();
    }

    [HttpDelete("{userId}/permissions/{permission}")]
    [HasPermission(AppFeature.RoleClaims, AppAction.Update)]
    public async Task<IActionResult> RemoveUserPermission(string userId, string permission)
    {
        if (!PermissionHelpers.IsKnownPermission(permission))
            return BadRequest("Unknown permission.");

        var u = await _users.FindByIdAsync(userId);
        if (u is null) return NotFound();

        await _users.RemoveClaimAsync(u, new Claim(AppClaim.Permission, permission));
        return NoContent();
    }
}
