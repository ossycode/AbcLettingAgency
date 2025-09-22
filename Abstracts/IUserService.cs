using AbcLettingAgency.Dtos.Request;
using AbcLettingAgency.Dtos.Response;
using AbcLettingAgency.Shared.Exceptions;
using System.Security.Claims;

namespace AbcLettingAgency.Abstracts;

public interface IUserService
{
    Task<Result<UserMeDto>> GetMeAsync(ClaimsPrincipal principal, CancellationToken ct);
    Task<Result> UpdateMeAsync(ClaimsPrincipal principal, UpdateMeRequest request, CancellationToken ct);
    Task<Result> ChangePasswordAsync(ClaimsPrincipal principal, ChangePasswordRequest request, CancellationToken ct);
    Task<Result> DeleteMeAsync(ClaimsPrincipal principal, DeleteMeRequest request, CancellationToken ct);
}