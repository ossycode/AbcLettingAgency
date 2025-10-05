using AbcLettingAgency.Shared.Attributes;
using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.TenantManagement.Tenants;

[AutoRegisterService]
public interface ITenantService
{
    Task<Result<TenantDto>> CreateAsync(CreateTenantRequest req, CancellationToken token);
    Task<Result<TenantDto>> UpdateAsync(long tenantId, UpdateTenantRequest req, CancellationToken token);
    Task<Result> DeleteAsync(long tenantId, CancellationToken token);
}
