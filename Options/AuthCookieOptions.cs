namespace AbcLettingAgency.Options;

public class AuthCookiesOptions
{
    // If frontend and API are on different *sites* (not just subdomains), set true
    public bool CrossSite { get; set; }

    // Optional overrides (otherwise inferred from env + CrossSite)
    public SameSiteMode? SameSiteOverride { get; set; }
    public bool? SecureOverride { get; set; }

    // Optional cookie scope tweaks
    public string Path { get; set; } = string.Empty;
    public string? Domain { get; init; }   

    // Names / lifetimes (handy if you want to centralize)
    public string AccessCookieName { get; init; } = "ACCESS_TOKEN";
    public string RefreshCookieName { get; init; } = "REFRESH_TOKEN";
    public int AccessMinutes { get; init; } = 120;
    public int RefreshDays { get; init; } = 7;
}
