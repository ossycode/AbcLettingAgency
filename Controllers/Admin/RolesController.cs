using AbcLettingAgency.Attributes;
using AbcLettingAgency.Authorization;
using AbcLettingAgency.Dtos;
using AbcLettingAgency.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AbcLettingAgency.Controllers.Admin;


[ApiController]
[Route("api/[controller]")]
public sealed class AdminRolesController(RoleManager<IdentityRole<Guid>> roles) : ControllerBase
{
    private readonly RoleManager<IdentityRole<Guid>> _roles = roles;

    [HttpGet]
    [HasPermission(AppFeature.Roles, AppAction.Read)]
    public async Task<ActionResult<IEnumerable<RoleDto>>> List()
    {
        var result = new List<RoleDto>();
        foreach (var role in _roles.Roles.OrderBy(r => r.Name))
        {
            var claims = await _roles.GetClaimsAsync(role);
            var perms = claims.Where(c => c.Type == AppClaim.Permission).Select(c => c.Value).OrderBy(x => x).ToArray();
            result.Add(new RoleDto(role.Name!, perms));
        }
        return Ok(result);
    }

    [HttpGet("{roleName}")]
    [HasPermission(AppFeature.Roles, AppAction.Read)]
    public async Task<ActionResult<RoleDto>> Get(string roleName)
    {
        var role = await _roles.FindByNameAsync(roleName);
        if (role is null) return NotFound();

        var claims = await _roles.GetClaimsAsync(role);
        var perms = claims.Where(c => c.Type == AppClaim.Permission).Select(c => c.Value).OrderBy(x => x).ToArray();
        return Ok(new RoleDto(role.Name!, perms));
    }

    [HttpPost]
    [HasPermission(AppFeature.Roles, AppAction.Create)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name)) return BadRequest("Role name required.");
        if (await _roles.FindByNameAsync(req.Name) is not null) return Conflict($"Role '{req.Name}' already exists.");

        var unknown = PermissionHelpers.UnknownPermissions(req.Permissions ?? Array.Empty<string>()).ToArray();
        if (unknown.Length > 0) return BadRequest($"Unknown permissions: {string.Join(", ", unknown)}");

        var role = new IdentityRole<Guid>(req.Name);
        var create = await _roles.CreateAsync(role);
        if (!create.Succeeded) return Problem(string.Join("; ", create.Errors.Select(e => e.Description)));

        if (req.Permissions is { Length: > 0 })
        {
            foreach (var p in req.Permissions.Distinct())
                await _roles.AddClaimAsync(role, new Claim(AppClaim.Permission, p));
        }

        return CreatedAtAction(nameof(Get), new { roleName = req.Name }, null);
    }

    [HttpPatch("{roleName}/rename")]
    [HasPermission(AppFeature.Roles, AppAction.Update)]
    public async Task<IActionResult> Rename(string roleName, [FromBody] RenameRoleRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.NewName)) return BadRequest("NewName required.");
        var role = await _roles.FindByNameAsync(roleName);
        if (role is null) return NotFound();

        if (!role.Name!.Equals(req.NewName, StringComparison.Ordinal))
        {
            if (await _roles.FindByNameAsync(req.NewName) is not null)
                return Conflict($"Role '{req.NewName}' already exists.");

            role.Name = req.NewName;
            var updated = await _roles.UpdateAsync(role);
            if (!updated.Succeeded) return Problem(string.Join("; ", updated.Errors.Select(e => e.Description)));
        }
        return NoContent();
    }

    [HttpDelete("{roleName}")]
    [HasPermission(AppFeature.Roles, AppAction.Delete)]
    public async Task<IActionResult> Delete(string roleName)
    {
        if (AppRoles.IsDefault(roleName)) return BadRequest("Default roles cannot be deleted.");
        var role = await _roles.FindByNameAsync(roleName);
        if (role is null) return NotFound();

        var result = await _roles.DeleteAsync(role);
        if (!result.Succeeded) return Problem(string.Join("; ", result.Errors.Select(e => e.Description)));
        return NoContent();
    }

    // ----- Role Permissions -----

    [HttpGet("{roleName}/permissions")]
    [HasPermission(AppFeature.RoleClaims, AppAction.Read)]
    public async Task<ActionResult<object>> GetPermissions(string roleName)
    {
        var role = await _roles.FindByNameAsync(roleName);
        if (role is null) return NotFound();

        var claims = await _roles.GetClaimsAsync(role);
        var assigned = claims.Where(c => c.Type == AppClaim.Permission).Select(c => c.Value).OrderBy(x => x).ToArray();
        var available = AppPermissions.AllPermissions.Select(p => p.Name).Except(assigned).OrderBy(x => x).ToArray();

        return Ok(new { role = role.Name, assigned, available });
    }

    [HttpPut("{roleName}/permissions")]
    [HasPermission(AppFeature.RoleClaims, AppAction.Update)]
    public async Task<IActionResult> SetPermissions(string roleName, [FromBody] SetPermissionsRequest req)
    {
        var role = await _roles.FindByNameAsync(roleName);
        if (role is null) return NotFound();

        var desired = (req.Permissions ?? Array.Empty<string>()).Distinct().ToArray();
        var unknown = PermissionHelpers.UnknownPermissions(desired).ToArray();
        if (unknown.Length > 0) return BadRequest($"Unknown permissions: {string.Join(", ", unknown)}");

        var current = (await _roles.GetClaimsAsync(role))
            .Where(c => c.Type == AppClaim.Permission)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.Ordinal);

        // remove missing
        foreach (var toRemove in current.Except(desired).ToArray())
            await _roles.RemoveClaimAsync(role, new Claim(AppClaim.Permission, toRemove));

        // add new
        foreach (var toAdd in desired.Except(current))
            await _roles.AddClaimAsync(role, new Claim(AppClaim.Permission, toAdd));

        return NoContent();
    }

    [HttpPost("{roleName}/permissions")]
    [HasPermission(AppFeature.RoleClaims, AppAction.Update)]
    public async Task<IActionResult> AddPermissions(string roleName, [FromBody] AddPermissionsRequest req)
    {
        var role = await _roles.FindByNameAsync(roleName);
        if (role is null) return NotFound();

        var toAdd = (req.Permissions ?? Array.Empty<string>()).Distinct().ToArray();
        var unknown = PermissionHelpers.UnknownPermissions(toAdd).ToArray();
        if (unknown.Length > 0) return BadRequest($"Unknown permissions: {string.Join(", ", unknown)}");

        var existing = (await _roles.GetClaimsAsync(role))
            .Where(c => c.Type == AppClaim.Permission)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var p in toAdd.Except(existing))
            await _roles.AddClaimAsync(role, new Claim(AppClaim.Permission, p));

        return NoContent();
    }

    [HttpDelete("{roleName}/permissions/{permission}")]
    [HasPermission(AppFeature.RoleClaims, AppAction.Update)]
    public async Task<IActionResult> RemovePermission(string roleName, string permission)
    {
        if (!PermissionHelpers.IsKnownPermission(permission))
            return BadRequest("Unknown permission.");

        var role = await _roles.FindByNameAsync(roleName);
        if (role is null) return NotFound();

        await _roles.RemoveClaimAsync(role, new Claim(AppClaim.Permission, permission));
        return NoContent();
    }

  }