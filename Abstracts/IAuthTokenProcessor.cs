using AbcLettingAgency.EntityModel;

namespace AbcLettingAgency.Abstracts;

public interface IAuthTokenProcessor
{
    Task<(string jwtToken, DateTime expiresAtUtc)> GenerateJwtToken(AppUser user, long? agencyId = null);
    string GenerateRefreshToken();
    void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
    void ClearAuthCookie(string name);
}
