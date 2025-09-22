using System.ComponentModel.DataAnnotations;

namespace AbcLettingAgency.Options;

public class JwtOptions
{
    public const string JwtOptionsKey = nameof(JwtOptions);

    public string Secret { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int ExpirationMinutes { get; set; }
}
