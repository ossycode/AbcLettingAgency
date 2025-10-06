using AbcLettingAgency.Abstracts;
using AbcLettingAgency.Authorization;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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




    public async Task<(string jwtToken, DateTime expiresAtUtc)> GenerateJwtToken(AppUser user, long? agencyId = null)
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
            new("ss", user.SecurityStamp ?? string.Empty)
        };

        if (agencyId is not null)
            claims.Add(new(AppClaim.AgencyId, agencyId.Value.ToString()));

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

        foreach (var c in userClaims.Where(c => c.Type == AppClaim.PlatformRole))
            claims.Add(c);

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

        ResolveCookiePolicy(httpContext, out var sameSite, out var secure, out var domain, out var path);

        if (sameSite == SameSiteMode.None && !secure)
        {
            if (_env.IsDevelopment())
            {
                secure = true; 
            }
            else
            {
                throw new InvalidOperationException("SameSite=None requires Secure=true.");
            }
        }

        if (httpContext.Response.HasStarted)
        {
            Console.WriteLine($"[Auth] Response already started; cannot set cookie '{cookieName}'.");
            return;
        }

        var opt = new CookieOptions
        {
            Expires = new DateTimeOffset(expiresUtc, TimeSpan.Zero),
            HttpOnly = true,
            IsEssential = true,
            SameSite = sameSite,
            Secure = secure,
            Path = path,
            Domain = domain
        };

        httpContext.Response.Cookies.Append(cookieName, token, opt);
    }

    public void ClearAuthCookie(string cookieName)
    {
        var ctx = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No HttpContext available.");

        // Use the *same policy values* as when setting the cookie
        ResolveCookiePolicy(ctx, out var sameSite, out var secure, out var domain, out var path);

        var opt = new CookieOptions
        {
            Expires = DateTimeOffset.UnixEpoch,
            HttpOnly = true,
            IsEssential = true,
            SameSite = sameSite,
            Secure = secure,
            Path = path,
            Domain = domain
        };

        if (!ctx.Response.HasStarted)
        {
            ctx.Response.Cookies.Append(cookieName, string.Empty, opt);
            ctx.Response.Cookies.Delete(cookieName, opt);
        }
    }

    private void ResolveCookiePolicy(
        HttpContext ctx,
        out SameSiteMode sameSite,
        out bool secure,
        out string? domain,
        out string path)
    {
        sameSite = _options.SameSiteOverride
                   ?? (_options.CrossSite ? SameSiteMode.None : SameSiteMode.Lax);

        secure = _options.SecureOverride
                 ?? (_env.IsProduction() || _options.CrossSite);

        domain = _options.Domain; // e.g. ".abclettingagency.com" for subdomain sharing
        path = string.IsNullOrWhiteSpace(_options.Path) ? "/" : _options.Path;

        if (_env.IsDevelopment() && _options.CrossSite && !ctx.Request.IsHttps)
        {
            // log warn: Cross-site auth cookies require HTTPS (Chrome)
            Console.WriteLine(" Cross-site auth cookies require HTTPS (Chrome)");
        }
    }
}
