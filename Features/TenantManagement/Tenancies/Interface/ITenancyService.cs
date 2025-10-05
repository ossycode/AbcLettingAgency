using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Attributes;
using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.TenantManagement.Tenancies.Interface;

[AutoRegisterService]
public interface ITenancyService : IEntityService<Tenancy>
{
    Task<Result<TenancyDto>> CreateAsync(CreateTenancyRequest req, CancellationToken token);
    Task<Result> UpdateAsync(long tenancyId, UpdateTenancyRequest req, CancellationToken token);
    Task<Result> DeleteAsync(long tenancyId, CancellationToken token);
    Task<Result> UpdateTenancyStatusAsync(long tenancyId, TenancyStatus newStatus, string? note, CancellationToken token);
    //Task<Result> UpdateOccupantsAsync(long tenancyId, IEnumerable<UpdateOccupant> occupants, CancellationToken token);
    Task<Result> UpdateBillingScheduleAsync(long tenancyId, UpdateBillingScheduleRequest req, CancellationToken token);
    Task<Result> SetNextChargeDateAsync(long tenancyId, DateTime? nextChargeDate, CancellationToken token);
}
