using AbcLettingAgency.Shared.Attributes;
using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.TenantManagement.Tenancies.Interface;

[AutoRegisterService]
public interface ITenancyOnboardingService
{
    Task<Result<TenancyDto>> StartTenancyAsync(StartTenancyRequest req, CancellationToken ct);
}
