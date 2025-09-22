using AbcLettingAgency.Abstracts;
using AbcLettingAgency.Authorization;
using AbcLettingAgency.Dtos.Request;
using AbcLettingAgency.Dtos.Response;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AbcLettingAgency.Services;

public class UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager) : IUserService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;

    public async Task<Result<UserMeDto>> GetMeAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        var userResult = await GetUserAsync(principal);
        var user = userResult.Value;

        var roles = await _userManager.GetRolesAsync(user);
        var perms = new HashSet<string>(StringComparer.Ordinal);

        var userClaims = await _userManager.GetClaimsAsync(user);
        foreach (var c in userClaims.Where(c => c.Type == AppClaim.Permission))
            perms.Add(c.Value);

        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null) continue;
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var c in roleClaims.Where(c => c.Type == AppClaim.Permission))
                perms.Add(c.Value);
        }

        var dto =  new UserMeDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToArray(),
            Permissions = perms.OrderBy(x => x).ToArray()
        };

        return Result<UserMeDto>.Success(dto);

    }

    public async Task<Result> UpdateMeAsync(ClaimsPrincipal principal, UpdateMeRequest request, CancellationToken ct)
    {
        var userResult = await GetUserAsync(principal);

        var user = userResult.Value;

        if (request.FirstName is { Length: > 0 })
            user.FirstName = request.FirstName.Trim();

        if (request.LastName is { Length: > 0 })
            user.LastName = request.LastName.Trim();

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded
               ? Result.Success()
               : Result.Failure(Errors.Identity.FromIdentity(result));
    }

    public async Task<Result> ChangePasswordAsync(ClaimsPrincipal principal, ChangePasswordRequest request, CancellationToken ct)
    {
        var uRes = await GetUserAsync(principal);
        if (!uRes.IsSuccess) return Result.Failure(uRes.Errors);

        var user = uRes.Value!;
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
            return Result.Failure(Errors.Identity.FromIdentity(result));

        await _userManager.UpdateSecurityStampAsync(user);
        return Result.Success();
    }

    public async Task<Result> DeleteMeAsync(ClaimsPrincipal principal, DeleteMeRequest request, CancellationToken ct)
    {
        var uRes = await GetUserAsync(principal);
        if (!uRes.IsSuccess) return Result.Failure(uRes.Errors);

        var user = uRes.Value!;

        if (!string.IsNullOrWhiteSpace(request.CurrentPassword))
        {
            var ok = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!ok) return Result.Failure(Errors.User.InvalidPassword());
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(Errors.Identity.FromIdentity(result));
    }

    private async Task<Result<AppUser>> GetUserAsync(ClaimsPrincipal principal)
    {
        var user = await _userManager.GetUserAsync(principal);
        if (user is not null) return user;

        var userId = _userManager.GetUserId(principal)
                  ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Result<AppUser>.Failure(Errors.Auth.Unauthorized("User id not found in claims."));

        var byId = await _userManager.FindByIdAsync(userId);
        return byId is null
            ? Result<AppUser>.Failure(Errors.User.NotFound("id"))
            : Result<AppUser>.Success(byId);
    }
}
