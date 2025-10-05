using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Enums;
using AbcLettingAgency.Features.TenantManagement.RentCharges;
using AbcLettingAgency.Features.TenantManagement.RentCharges.Interfaces;
using AbcLettingAgency.Features.TenantManagement.Tenancies.Errors;
using AbcLettingAgency.Features.TenantManagement.Tenancies.Interface;
using AbcLettingAgency.Features.TenantManagement.Tenants;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Features.TenantManagement.Tenancies.Services;

public class TenancyOnboardingService(
    IChangeScope scope,
    ITenantService tenantService,
    ITenancyService tenancyService,
    IRentChargeService rentChargeService,
    IEntityServiceFactory entityService
) : ITenancyOnboardingService
{
    private readonly IChangeScope _scope = scope;
    private readonly ITenantService _tenantService = tenantService;
    private readonly ITenancyService _tenancyService = tenancyService;
    private readonly IRentChargeService _rentChargeService = rentChargeService;
    private readonly IEntityServiceFactory _entityService = entityService;

    public async Task<Result<TenancyDto>> StartTenancyAsync(StartTenancyRequest req, CancellationToken ct)
    {

        // 0) quick occupant sanity (exactly one primary, shares, duplicates)
        if (req.Occupants.Count(o => o.IsPrimary) != 1)
            return TenancyErrors.InvalidPrimaryOccupants();
        var idsProvided = req.Occupants.Where(o => o.TenantId.HasValue).Select(o => o.TenantId!.Value).ToArray();
        if (idsProvided.Length != idsProvided.Distinct().Count())
            return TenancyErrors.DuplicateOccupants();
        if (req.Occupants.Exists(o => o.ResponsibilitySharePercent.HasValue))
        {
            var sum = req.Occupants.Sum(o => o.ResponsibilitySharePercent ?? 0m);
            if (Math.Abs(sum - 100m) > 0.001m)
                return TenancyErrors.InvalidShareDistribution();
        }
        if (req.RentDueDay is < 1 or > 31)
            return TenancyErrors.InvalidRentDueDay();


        long newTenancyId = 0;

       var result =   await _scope.BatchAsync(BatchMode.UseTransaction, async () =>
        {

            // 1) Ensure tenants (existing or create)
            var occupantTenantIds = new List<(OccupantInput input, long tenantId)>(req.Occupants.Count);
            foreach (var occ in req.Occupants)
            {
                if (occ.TenantId is long existingId)
                {
                    var exists = await _entityService.For<Tenant>()
                    .GetAll()
                    .AnyAsync(t => t.Id == existingId, ct);

                    if (!exists)
                         throw new BatchAbortException(TenancyErrors.TenantNotFound([existingId]));

                    occupantTenantIds.Add((occ, existingId));
                }
                else if (occ.NewTenant is not null)
                {
                    // recommend: TenantService.CreateAsync returns Result<long>
                    var created = await _tenantService.CreateAsync(occ.NewTenant, ct);
                    if (!created.IsSuccess) throw new BatchAbortException(created.Errors![0]);

                    // If your current CreateAsync returns no id, add an overload that does,
                    // or resolve by unique email immediately after create.
                    var newId = created.Value.Id;
                    occupantTenantIds.Add((occ, newId));
                }
                else
                {
                    throw new BatchAbortException(TenancyErrors.InvalidTenants());
                }
            }

            // 2) Create the tenancy with occupants
            var createReq = new CreateTenancyRequest
            {
                PropertyId = req.PropertyId,
                LandlordId = req.LandlordId,
                Status = TenancyStatus.ACTIVE,
                StartDate = req.StartDate,
                EndDate = req.EndDate,
                RentDueDay = req.RentDueDay,
                Frequency = req.Frequency,
                RentAmount = req.RentAmount,
                CommissionPercent = req.CommissionPercent,
                DepositAmount = req.DepositAmount,
                DepositLocation = req.DepositLocation,
                Notes = req.Notes,
                Occupants = occupantTenantIds.Select(x => new CreateOccupant
                {
                    TenantId = x.tenantId,
                    IsPrimary = x.input.IsPrimary,
                    ResponsibilitySharePercent = x.input.ResponsibilitySharePercent,
                    OccupancyStart = x.input.OccupancyStart ?? req.StartDate,
                    OccupancyEnd = x.input.OccupancyEnd
                }).ToList()
            };

            var createdTenancy = await _tenancyService.CreateAsync(createReq, ct);
            if (!createdTenancy.IsSuccess) throw new BatchAbortException(createdTenancy.Errors[0]!);

            // If your CreateAsync returns void Result, adjust TenancyService to return the new id,
            // or query back by some natural key. Returning the id is best:
            //   public Task<Result<long>> CreateAsync(...)

            // Assume TenancyService.CreateAsync returns Result<long>:
            newTenancyId = createdTenancy.Value.Id;

            // 3) Optionally seed first charge
            if (req.SeedFirstCharge)
            {
                var rc = await _rentChargeService.CreateAsync(new RentChargeCreateModel
                {
                    TenancyId = newTenancyId,
                    AdvanceNextChargeDate = true
                }, ct);

                if (!rc.IsSuccess) throw new BatchAbortException(rc.Errors[0]!);
            }

            return createdTenancy.Value;

        }, ct);

        return result;

    }
}