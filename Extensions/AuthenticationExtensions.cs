using AbcLettingAgency.Authorization;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace AbcLettingAgency.Extensions;

public static class AuthenticationExtensions
{
    /// <summary>
    /// Adds JWT Bearer authentication using settings from "AuthConfiguration".
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(JwtOptions.JwtOptionsKey);
        var opts = section.Get<JwtOptions>()
                   ?? throw new InvalidOperationException("Missing JwtOptions section.");
        if (string.IsNullOrWhiteSpace(opts.Secret))
            throw new InvalidOperationException("JwtOptions:Secret is missing.");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.Secret));

        services.Configure<JwtOptions>(section);

        services
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = opts.Issuer,
                    ValidAudience = opts.Audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier,
                    

                };

                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {

                        var token = ctx.Request.Cookies["ACCESS_TOKEN"];
                        if (!string.IsNullOrEmpty(token)) ctx.Token = token;
                        return Task.CompletedTask;
                    },


                    OnTokenValidated = async ctx =>
                    {
                        var userManager = ctx.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                        var userId = ctx.Principal!.FindFirstValue(ClaimTypes.NameIdentifier);
                        var jwtStamp = ctx.Principal!.FindFirst("ss")?.Value;

                        var user = await userManager.FindByIdAsync(userId);
                        if (user is null || string.IsNullOrEmpty(jwtStamp) || !string.Equals(jwtStamp, user.SecurityStamp, StringComparison.Ordinal))
                        {
                            ctx.Fail("Token revoked");
                        }
                    },

                    OnAuthenticationFailed = ctx =>
                    {
                        ctx.NoResult();        
                        return Task.CompletedTask;
                    },
                    OnChallenge = ctx =>
                    {
                        ctx.HandleResponse();
                        if (!ctx.Response.HasStarted)
                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;

                    },
                    OnForbidden = ctx =>
                    {
                        if (!ctx.Response.HasStarted)
                            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    },
                };
            });

        services.AddAuthorization(options =>
        {
            //foreach (var prop in typeof(AppPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            //{
            //    var propertyValue = prop.GetValue(null);
            //    if (propertyValue is not null)
            //    {
            //        options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(AppClaim.Permission, propertyValue.ToString()));
            //    }
            //}

            foreach (var p in AppPermissions.AllPermissions)
                options.AddPolicy(p.Name, policy => policy.RequireClaim(AppClaim.Permission, p.Name));
        });

        return services;
    }
}
