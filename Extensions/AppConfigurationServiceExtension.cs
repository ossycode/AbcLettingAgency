using AbcLettingAgency.Options;

namespace AbcLettingAgency.Extensions;

internal static class AppConfigurationServiceExtension
{
    internal static JwtOptions GetJwtOptionsSettings(this IServiceCollection services,
  IConfiguration configuration)
    {
        var applicationSettingsConfiguration = configuration.GetSection(nameof(JwtOptions));
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

        return applicationSettingsConfiguration.Get<JwtOptions>() ??
            throw new InvalidOperationException("Failed to bind configuration to JwtOptions.");

    }

    internal static AuthCookieOptions GetAuthCookieOptionsSettings(this IServiceCollection services,
IConfiguration configuration)
    {
        var applicationSettingsConfiguration = configuration.GetSection(nameof(AuthCookieOptions));
        services.Configure<AuthCookieOptions>(configuration.GetSection(nameof(AuthCookieOptions)));

        return applicationSettingsConfiguration.Get<AuthCookieOptions>() ??
            throw new InvalidOperationException("Failed to bind configuration to AuthCookieOptions.");

    }

    internal static OutboxOptions GetOutboxOptionsSettings(this IServiceCollection services,
IConfiguration configuration)
    {
        var applicationSettingsConfiguration = configuration.GetSection(nameof(OutboxOptions));
        services.Configure<OutboxOptions>(configuration.GetSection(nameof(OutboxOptions)));

        return applicationSettingsConfiguration.Get<OutboxOptions>() ??
            throw new InvalidOperationException("Failed to bind configuration to OutboxOptions.");

    }
}
