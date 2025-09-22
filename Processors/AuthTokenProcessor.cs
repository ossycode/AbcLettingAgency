using AbcLettingAgency.Abstracts;
using AbcLettingAgency.Authorization;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace AbcLettingAgency.Processors;

public class AuthTokenProcessor(IOptions<JwtOptions> jwtOptions,
    IHttpContextAccessor httpContextAccessor,
    UserManager<AppUser> userManager, 
    RoleManager<IdentityRole<Guid>> roleManager, IHostEnvironment env,
    IOptions<AuthCookieOptions> authCookieOptions) : IAuthTokenProcessor

{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly UserManager<AppUser> _userManager = userManager;

    private readonly IHostEnvironment _env = env;
    private readonly AuthCookieOptions _options  = authCookieOptions.Value;




    public async Task<(string jwtToken, DateTime expiresAtUtc)> GenerateJwtToken(AppUser user)
    {
        var signinKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Secret));

        var credentials = new SigningCredentials(signinKey, algorithm: SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(type: JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(type: JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(type: JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(type: JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty),
            new(type: JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty),
            new("ss", user.SecurityStamp ?? string.Empty), // <-- add stamp


        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var perms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var userClaims = await userManager.GetClaimsAsync(user);
        foreach (var c in userClaims.Where(c => c.Type == AppClaim.Permission))
            perms.Add(c.Value);

        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null) continue;

            var roleClaims = await roleManager.GetClaimsAsync(role);
            foreach (var c in roleClaims.Where(c => c.Type == AppClaim.Permission))
                perms.Add(c.Value);
        }

        claims.AddRange(perms.Select(p => new Claim(AppClaim.Permission, p)));

        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims,
            expires: expires,
            signingCredentials: credentials
            );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        return (jwtToken, expires);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    public void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiresUtc)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No HttpContext available.");

        var sameSite = _options.SameSiteOverride
            ?? (_options.CrossSite ? SameSiteMode.None : SameSiteMode.Lax);

        var secure = _options.SecureOverride
            ?? (_env.IsProduction() || _options.CrossSite);

        // Idempotency flag so we don’t append multiple Set-Cookie headers for the same cookie
        var flagKey = $"cookie_written:{cookieName}";
        if (httpContext.Items.ContainsKey(flagKey))
            return;

        httpContext.Items[flagKey] = true;

        httpContext.Response.OnStarting(() =>
        {
            var opt = new CookieOptions
            {
                Expires = expiresUtc,    
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Secure = true,        
                Path = "/",
            };

            httpContext.Response.Cookies.Append(cookieName, token, opt);
            return Task.CompletedTask;
        });
    }

    public void ClearAuthCookie(string name)
    {
        var sameSite = _options.SameSiteOverride
            ?? (_options.CrossSite ? SameSiteMode.None : SameSiteMode.Lax);
        var secure = _options.SecureOverride
            ?? (_env.IsProduction() || _options.CrossSite);

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(name, "",
            new CookieOptions
            {
                Expires = DateTime.UnixEpoch,
                HttpOnly = true,
                IsEssential = true,
                SameSite = sameSite,
                Secure = secure,
                Path = _options.Path,
            });
    }
}
