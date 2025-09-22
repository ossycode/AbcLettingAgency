using AbcLettingAgency.Dtos;
using AbcLettingAgency.Shared.Exceptions;
using System.Security.Claims;

namespace AbcLettingAgency.Abstracts;

public interface IAuthService
{
    Task<Result> CreateUserAsync(RegisterRequest req);
    Task<Result> LoginAsync(LoginRequest req);
    Task<Result> RefreshTokenAsync(string? refreshToken);
    Task<Result> LogUserOut(ClaimsPrincipal principal);
}