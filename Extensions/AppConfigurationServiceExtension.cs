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

    internal static AuthCookiesOptions GetAuthCookieOptionsSettings(this IServiceCollection services,
IConfiguration configuration)
    {
        var applicationSettingsConfiguration = configuration.GetSection(nameof(AuthCookiesOptions));
        services.Configure<AuthCookiesOptions>(configuration.GetSection(nameof(AuthCookiesOptions)));

        return applicationSettingsConfiguration.Get<AuthCookiesOptions>() ??
            throw new InvalidOperationException("Failed to bind configuration to AuthCookiesOptions.");

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
