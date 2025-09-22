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


builder.Services.AddCors(options =>
{
    options.AddPolicy("LettingAgency", policy =>
    {
        policy
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowed(host => true);

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
