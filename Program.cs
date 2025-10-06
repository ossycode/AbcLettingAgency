using AbcLettingAgency.Abstracts;
using AbcLettingAgency.Data;
using AbcLettingAgency.Extensions;
using AbcLettingAgency.Features.TenantManagement.EventsHandler;
using AbcLettingAgency.Helpers;
using AbcLettingAgency.Options;
using AbcLettingAgency.Processors;
using AbcLettingAgency.Services;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Attributes;
using AbcLettingAgency.Shared.Events;
using AbcLettingAgency.Shared.Exceptions;
using AbcLettingAgency.Shared.Infrastructure;
using AbcLettingAgency.Shared.Services;
using AbcLettingAgency.Shared.Utilities;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddIdentitySettings();


builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), option =>
    {
        option.MigrationsHistoryTable("__EFMigrationsHistory", "public")
        .EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: new[] { "57P01", "08006", "40001" }   // admin shutdown, conn failure, serialization
        );
        option.CommandTimeout(30);
    }
));
builder.Services.AddNpgsqlDataSource(
    builder.Configuration.GetConnectionString("DefaultConnection"));

var jwtConfig = builder.Services.GetJwtOptionsSettings(builder.Configuration);
builder.Services.GetAuthCookieOptionsSettings(builder.Configuration);
builder.Services.GetOutboxOptionsSettings(builder.Configuration);
//builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection("Outbox"));
//builder.Services.Configure<AuthCookieOptions>(
//    builder.Configuration.GetSection("AuthCookies"));
//builder.Services.Configure<JwtOptions>(
//    builder.Configuration.GetSection("JwtOptions"));

builder.Services.AddTransient<ApplicationDbSeeder>();
builder.Services.AddScoped<IAuthTokenProcessor, AuthTokenProcessor>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IChangeScope, EfChangeScope<AppDbContext>>();
builder.Services.AddScoped<IEntityServiceFactory, EntityServiceFactory>();
builder.Services.AddScoped<IEntityServiceDependencies, EntityServiceDependencies>();

builder.Services.AddScoped(typeof(IEntityService<>), typeof(GenericEntityService<>));
builder.Services.AddScoped<FriendlyCodeGenerator>();
builder.Services.AddHybridCache();


//EVENTS
builder.Services.AddScoped<IOutboxWriter, EfOutboxWriter>();
builder.Services.AddHostedService<OutboxDispatcherWorker>();
builder.Services.AddHostedService<OutboxCleanupWorker>();
builder.Services.AddSingleton<IEventTypeRegistry, EventTypeRegistry>();
builder.Services.AddScoped<IIntegrationEventHandler<RentChargeCreated>, RentChargeCreatedHandler>();
builder.Services.AddSingleton<IAmbientAgency, AmbientAgency>();

//builder.Services.Scan(s => s
//    .FromCallingAssembly()
//    .AddClasses(c => c.AssignableTo(typeof(IIntegrationEventHandler<>)))
//    .AsImplementedInterfaces()
//    .WithScopedLifetime());

builder.Services.Scan(s => s
    .FromApplicationDependencies(a =>
        a.FullName != null && a.FullName.StartsWith("AbcLettingAgency"))
    .AddClasses(c => c.Where(t =>
        t.GetInterfaces().Any(i =>
            i.GetCustomAttributes(typeof(AutoRegisterServiceAttribute), inherit: true).Any())))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

var isDev = builder.Environment.IsDevelopment();

builder.Services.AddCors(o =>
{
    o.AddPolicy("LettingAgency", p =>
        p.SetIsOriginAllowed(origin =>
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var u)) return false;

            if (isDev)
            {
                // Dev: allow http/https from localhost, 127.0.0.1, and lvh.me (for subdomain testing)
                if (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps) return false;

                if (u.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)) return true;
                if (u.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)) return true;
                if (u.Host.Equals("lvh.me", StringComparison.OrdinalIgnoreCase)) return true;
                if (u.Host.EndsWith(".lvh.me", StringComparison.OrdinalIgnoreCase)) return true;

                // Add your LAN IP if you test from devices:
                // if (u.Host.StartsWith("192.168.", StringComparison.Ordinal)) return true;

                return false;
            }

            // Prod: only your real https hosts
            if (u.Scheme != Uri.UriSchemeHttps) return false;

            if (u.Host.Equals("cvedup.com", StringComparison.OrdinalIgnoreCase)) return true;
            if (u.Host.Equals("www.cvedup.com", StringComparison.OrdinalIgnoreCase)) return true;
            if (u.Host.EndsWith(".cvedup.com", StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});


//var allowedOrigins = builder.Environment.IsDevelopment()
//    ? new[] { "http://localhost:3000", "http://localhost:5173" }
//    : new[] { "https://cvedup.com", "https://www.cvedup.com", "https://*.cvedup.com" };

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("LettingAgency", policy =>
//    {
//        policy.WithOrigins(allowedOrigins)
//              .SetIsOriginAllowedToAllowWildcardSubdomains()
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();

builder.Services.RegisterSwagger();
builder.Services.AddJwtAuthentication(jwtConfig);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddProblemDetails();

// Specific first
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<NoAccessExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<ConcurrencyExceptionHandler>();
// Fallback last
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


builder.Services.PostConfigure<JwtOptions>(o =>
{
    Console.WriteLine($"[JwtOptions] Issuer={o.Issuer}, Audience={o.Audience}, ExpirationMinutes={o.ExpirationMinutes} {o.Secret}");
});

var app = builder.Build();

app.SeedDatabase();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors("LettingAgency");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/healthz", () => Results.Ok("ok"));

app.Run();
