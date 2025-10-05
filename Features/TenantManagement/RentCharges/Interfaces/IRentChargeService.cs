using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Features.TenantManagement.RentCharges;
using AbcLettingAgency.Shared.Attributes;
using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.TenantManagement.RentCharges.Interfaces;

[AutoRegisterService]
public interface IRentChargeService
{
    Task<Result<RentChargeDto>> CreateAsync(RentChargeCreateModel model, CancellationToken token);
    Task DeleteRangeAsync(ICollection<RentCharge> rentCharges, CancellationToken token);
}
