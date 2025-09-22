using AbcLettingAgency.Abstracts;
using AbcLettingAgency.Data;
using AbcLettingAgency.Extensions;
using AbcLettingAgency.Processors;
using AbcLettingAgency.Services;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Attributes;
using AbcLettingAgency.Shared.Exceptions;
using AbcLettingAgency.Shared.Infrastructure;
using AbcLettingAgency.Shared.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddIdentitySettings();
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
            npg => npg.MigrationsHistoryTable("__EFMigrationsHistory", "public")
));
builder.Services.AddTransient<ApplicationDbSeeder>();
builder.Services.AddScoped<IAuthTokenProcessor, AuthTokenProcessor>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IChangeScope, EfChangeScope<AppDbContext>>();
builder.Services.AddScoped<IEntityServiceFactory, EntityServiceFactory>();
builder.Services.AddScoped<IEntityServiceDependencies, EntityServiceDependencies>();

builder.Services.AddScoped(typeof(IEntityService<>), typeof(GenericEntityService<>));

builder.Services.Scan(s => s
    .FromApplicationDependencies(a =>
        a.FullName != null && a.FullName.StartsWith("AbcLettingAgency"))
    .AddClasses(c => c.Where(t =>
        t.GetInterfaces().Any(i =>
            i.GetCustomAttributes(typeof(AutoRegisterServiceAttribute), inherit: true).Any())))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

var allowedOrigins = builder.Configuration
    .GetSection("CORS:AllowedOrigins").Get<string[]>() ?? ["https://abc-letting-agency.vercel.app", "http://localhost:3000"];


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("LettingAgency", policy =>
//    {
//        policy
//              .AllowAnyMethod()
//              .AllowAnyHeader()
//              .AllowCredentials()
//              .SetIsOriginAllowed(host => true);

//    });
//});

builder.Services.AddCors(o =>
{
    o.AddPolicy("frontend", p =>
    {
        if (allowedOrigins.Length > 0)
        {
            // Credentials allowed ONLY with explicit origins
            p.WithOrigins(allowedOrigins)
             .AllowAnyHeader()
             .AllowAnyMethod()
             .AllowCredentials();
        }
        else
        {
            // Dev fallback: no credentials if you truly allow any origin
            p.AllowAnyOrigin()
             .AllowAnyHeader()
             .AllowAnyMethod();
            // NOTE: Do NOT call .AllowCredentials() here.
        }
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.RegisterSwagger();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddProblemDetails();

// Specific first
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<NoAccessExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();

// Fallback last
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


var app = builder.Build();

app.SeedDatabase();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors("frontend");

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,   // cross-site
    Secure = CookieSecurePolicy.Always
});

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
