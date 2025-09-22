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
                    NameClaimType = ClaimTypes.NameIdentifier

                };

                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var auth = context.Request.Headers["Authorization"].ToString();
                        if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            return Task.CompletedTask;
                        }

                        var token = context.Request.Cookies["ACCESS_TOKEN"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                            return Task.CompletedTask;
                        }

                        // 3) (Optional) Support query string for websockets/SignalR if needed:
                        // var qsToken = context.Request.Query["access_token"];
                        // if (!StringValues.IsNullOrEmpty(qsToken)) context.Token = qsToken;

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

                    OnAuthenticationFailed = c =>
                    {
                        if (c.Exception is SecurityTokenExpiredException)
                        {
                            c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            c.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject("The Token is expire");
                            return c.Response.WriteAsync(result);
                        }
                        else
                        {
                            c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            c.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject("An unhandled error has occurred.");
                            return c.Response.WriteAsync(result);
                        }
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject("You are not Authorized.");
                            return context.Response.WriteAsync(result);
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject("You are not authorized to access this resource.");
                        return context.Response.WriteAsync(result);
                    },
                };
            });

        services.AddAuthorization(options =>
        {
            foreach (var prop in typeof(AppPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(AppClaim.Permission, propertyValue.ToString()));
                }
            }
        });

        return services;
    }
}
