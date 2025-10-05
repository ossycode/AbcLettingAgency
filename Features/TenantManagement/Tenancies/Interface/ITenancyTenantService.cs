using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Shared.Attributes;
using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.TenantManagement.Tenancies.Interface;

[AutoRegisterService]
public interface ITenancyTenantService
{
    Task<Result> CreateAsync(long tenancyId, CreateOccupant req, CancellationToken token);
    //Task<Result> DeleteAsync(long tenancyId, long tenantId, CancellationToken token);
    Task<Result> ReplaceAsync(long tenancyId, IEnumerable<UpdateOccupant> occupants, CancellationToken ct);

    Task CreateTenancyTenantRangeAsync(IEnumerable<TenancyTenant> entities, CancellationToken token);
    Task<Result> DeleteTenancyTenantRangeAsync(IEnumerable<TenancyTenant> entities, CancellationToken token);
}

