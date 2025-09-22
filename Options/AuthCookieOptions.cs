namespace AbcLettingAgency.Options;

public sealed class AuthCookieOptions
{
    // If frontend and API are on different *sites* (not just subdomains), set true
    public bool CrossSite { get; init; } = true;

    // Optional overrides (otherwise inferred from env + CrossSite)
    public SameSiteMode? SameSiteOverride { get; init; }
    public bool? SecureOverride { get; init; }

    // Optional cookie scope tweaks
    public string Path { get; init; } = "/";
    public string? Domain { get; init; }   

    // Names / lifetimes (handy if you want to centralize)
    public string AccessCookieName { get; init; } = "ACCESS_TOKEN";
    public string RefreshCookieName { get; init; } = "REFRESH_TOKEN";
    public int AccessMinutes { get; init; } = 15;
    public int RefreshDays { get; init; } = 7;
}
