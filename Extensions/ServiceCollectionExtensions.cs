﻿using AbcLettingAgency.Data;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Shared.Permissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace AbcLettingAgency.Extensions;


public static class ServiceCollectionExtensions
{

    internal static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        var seeders = serviceScope.ServiceProvider.GetServices<ApplicationDbSeeder>();

        foreach (var seeder in seeders)
        {
            seeder.SeedDatabaseAsync().GetAwaiter().GetResult();
        }
        return app;
    }
    internal static IServiceCollection AddIdentitySettings(this IServiceCollection services)
    {
        services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
            .AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }

    internal static void RegisterSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "ABCLETTING AGENCY API",
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });
        });
    }

    //internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AppConfiguration config)
    //{
    //    var key = Encoding.ASCII.GetBytes(config.Secret);
    //    services
    //        .AddAuthentication(authentication =>
    //        {
    //            authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //            authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //        })
    //        .AddJwtBearer(bearer =>
    //        {
    //            bearer.RequireHttpsMetadata = false;
    //            bearer.SaveToken = true;
    //            bearer.TokenValidationParameters = new TokenValidationParameters
    //            {
    //                ValidateIssuerSigningKey = true,
    //                IssuerSigningKey = new SymmetricSecurityKey(key),
    //                ValidateIssuer = false,
    //                ValidateAudience = false,
    //                RoleClaimType = ClaimTypes.Role,
    //                ClockSkew = TimeSpan.Zero
    //            };

    //            bearer.Events = new JwtBearerEvents
    //            {
    //                OnAuthenticationFailed = c =>
    //                {
    //                    if (c.Exception is SecurityTokenExpiredException)
    //                    {
    //                        c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    //                        c.Response.ContentType = "application/json";
    //                        var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("The Token is expired."));
    //                        return c.Response.WriteAsync(result);
    //                    }
    //                    else
    //                    {
    //                        c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //                        c.Response.ContentType = "application/json";
    //                        var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("An unhandled error has occurred."));
    //                        return c.Response.WriteAsync(result);
    //                    }
    //                },
    //                OnChallenge = context =>
    //                {
    //                    context.HandleResponse();
    //                    if (!context.Response.HasStarted)
    //                    {
    //                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    //                        context.Response.ContentType = "application/json";
    //                        var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not Authorized."));
    //                        return context.Response.WriteAsync(result);
    //                    }

    //                    return Task.CompletedTask;
    //                },
    //                OnForbidden = context =>
    //                {
    //                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
    //                    context.Response.ContentType = "application/json";
    //                    var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resource."));
    //                    return context.Response.WriteAsync(result);
    //                },
    //            };
    //        });

    //    services.AddAuthorization(options =>
    //    {
    //        foreach (var prop in typeof(AppPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
    //        {
    //            var propertyValue = prop.GetValue(null);
    //            if (propertyValue is not null)
    //            {
    //                options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(AppClaim.Permission, propertyValue.ToString()));
    //            }
    //        }
    //    });
    //    return services;
    //}

}
