using AbcLettingAgency.Abstracts;
using AbcLettingAgency.Authorization;
using AbcLettingAgency.Data;
using AbcLettingAgency.Dtos;
using AbcLettingAgency.Dtos.Response;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Exceptions;
using AbcLettingAgency.Processors;
using AbcLettingAgency.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AbcLettingAgency.Services;

public class AuthService(IAuthTokenProcessor authTokenProcessor,
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    AppDbContext db
   ) : IAuthService
{
    private readonly IAuthTokenProcessor _authTokenProcessor = authTokenProcessor;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
    private readonly AppDbContext _appDbContext = db;

    public async Task<Result> CreateUserAsync(RegisterRequest req)
    {
        var userExists = await _userManager.FindByEmailAsync(req.Email) != null;
        if (userExists) return Result.Failure(Errors.User.AlreadyExists(req.Email));

        var user = AppUser.Create(req.Email, req.FirstName, req.LastName);
        var created = await _userManager.CreateAsync(user, req.Password);
        if (!created.Succeeded)
            return Result.Failure(Errors.Identity.FromIdentity(created));

        var roleRes = await _userManager.AddToRoleAsync(user, AppRoles.Basic);
        if (!roleRes.Succeeded)
            return Result.Failure(Errors.Identity.FromIdentity(roleRes));

        return Result.Success();
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, req.Password))
            return Errors.Auth.InvalidCredentials();

        var roles = await _userManager.GetRolesAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);
        var isPlatform = userClaims.Any(c => c.Type == AppClaim.PlatformRole);

        // Memberships (IGNORE FILTERS – no agency claim yet)
        var agencies = await _appDbContext.AgencyUsers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.UserId == user.Id && x.IsActive)
            .OrderBy(x => x.JoinedAt)
            .Select(x => new AgencySummary
            {
                Id = x.AgencyId,
                Name = x.Agency.Name,
                Slug = x.Agency.Slug
            })
            .ToListAsync();

        long? selectedAgencyId = agencies.Count == 1 ? agencies[0].Id : null;

        var (jwtToken, expirationUtc) = await _authTokenProcessor.GenerateJwtToken(user, selectedAgencyId);

        var refreshToken = _authTokenProcessor.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
                    return Result<LoginResponse>.Failure(Errors.Identity.FromIdentity(update));

        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationUtc);
        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", refreshToken, user.RefreshTokenExpiresAtUtc);

        // Optional permissions payload (small sets only)
        var perms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var c in userClaims.Where(c => c.Type == AppClaim.Permission)) perms.Add(c.Value);
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null) continue;
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var c in roleClaims.Where(c => c.Type == AppClaim.Permission)) perms.Add(c.Value);
        }

        var dto = new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            DisplayName = $"{user.FirstName} {user.LastName}".Trim(),
            IsPlatform = isPlatform,
            SelectedAgencyId = selectedAgencyId,
            AgencyCount = agencies.Count,
            NeedsAgencySelection = !isPlatform && agencies.Count != 1,
            Agencies = agencies.ToArray(),
            Roles = roles.ToArray(),
            Permissions = perms.OrderBy(p => p).ToArray()
        };


        return dto;
    }

    public async Task<Result> RefreshTokenAsync(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Failure(Errors.Auth.RefreshTokenMissing());

        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user is null) return Result.Failure(Errors.Auth.RefreshTokenInvalid());

        if (user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
            return Result.Failure(Errors.Auth.RefreshTokenExpired());

        var (jwtToken, expirationUtc) = await _authTokenProcessor.GenerateJwtToken(user);
        var newRefresh = _authTokenProcessor.GenerateRefreshToken();

        user.RefreshToken = newRefresh;
        user.RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
            return Result.Failure(Errors.Identity.FromIdentity(update));

        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationUtc);
        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", newRefresh, user.RefreshTokenExpiresAtUtc);

        return Result.Success();
    }

    public async Task<Result> LogUserOut(ClaimsPrincipal principal)
    {
        var user = await _userManager.GetUserAsync(principal);
        if (user is not null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiresAtUtc = DateTime.UtcNow;
            await _userManager.UpdateSecurityStampAsync(user);
            var upd = await _userManager.UpdateAsync(user);
            if (!upd.Succeeded)
                return Result.Failure(Errors.Identity.FromIdentity(upd));
        }

        _authTokenProcessor.ClearAuthCookie("ACCESS_TOKEN");
        _authTokenProcessor.ClearAuthCookie("REFRESH_TOKEN");
        return Result.Success();
    }
}
