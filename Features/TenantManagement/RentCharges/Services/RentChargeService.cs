using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Enums;
using AbcLettingAgency.Features.TenantManagement.RentCharges.Interfaces;
using AbcLettingAgency.Features.TenantManagement.Tenancies.helpers;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Events;
using AbcLettingAgency.Shared.Exceptions;
using AbcLettingAgency.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Features.TenantManagement.RentCharges.Services;

public class RentChargeService(IEntityServiceDependencies deps,
    IEntityServiceFactory entityService,
    IOutboxWriter outboxWriter) : BaseEntityService<RentCharge>(deps), IRentChargeService
{
    private readonly IEntityServiceFactory _entityService = entityService;
    private readonly IOutboxWriter _outbox = outboxWriter;

    public async Task<Result<RentChargeDto>> CreateAsync(RentChargeCreateModel model, CancellationToken token)
    {
        // Load tenancy TRACKED (we may advance NextChargeDate)
        var tenancy = await _entityService.For<Tenancy>().GetAll()
            .FirstOrDefaultAsync(t => t.Id == model.TenancyId, token);

        if (tenancy is null)
            return RentChargeErrors.TenancyNotFound(model.TenancyId);

        if (tenancy.IsDeleted || tenancy.Status != TenancyStatus.ACTIVE)
            return RentChargeErrors.TenancyNotActive(model.TenancyId    );

        // Determine period
        var periodStart = (model.PeriodStart ?? tenancy.NextChargeDate ?? tenancy.StartDate)?.Date
                          ?? DateTime.UtcNow.Date;

        var periodEnd = (model.PeriodEnd ??
                         RentSchedule.DerivePeriodEnd(periodStart, tenancy.Frequency, tenancy.RentDueDay)).Date;

        if (periodEnd < periodStart)
            return RentChargeErrors.InvalidPeriod(periodStart, periodEnd);

        // Due date
        var dueDate = (model.DueDate ?? RentSchedule.DeriveDueDate(periodStart, tenancy.Frequency, tenancy.RentDueDay)).Date;

        // Amount (assume tenancy.RentAmount is “per-period” for its Frequency)
        var amount = model.Amount ?? tenancy.RentAmount;
        if (amount <= 0m)
            return RentChargeErrors.InvalidAmount();

        // Commission
        decimal? commissionDue = tenancy.CommissionPercent.HasValue
            ? Math.Round(amount * tenancy.CommissionPercent.Value / 100m, 2, MidpointRounding.AwayFromZero)
            : null;

        decimal? amountAfterCommission = commissionDue.HasValue ? amount - commissionDue.Value : null;

        // Prevent overlap with existing charges
        var overlaps = await _entityService.For<RentCharge>().GetAll()
            .AnyAsync(rc =>
                rc.TenancyId == tenancy.Id &&
                rc.PeriodStart <= periodEnd &&
                rc.PeriodEnd >= periodStart, token);

        if (overlaps)
            return RentChargeErrors.Overlap(periodStart, periodEnd);

        var entity = new RentCharge
        {
            TenancyId = tenancy.Id,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            DueDate = dueDate,
            Amount = amount,
            CommissionDue = commissionDue,
            AmountAfterCommission = amountAfterCommission,
            Status = ChargeStatus.OPEN,
            Notes = model.Notes
        };

        await ChangeScope.BatchAsync(BatchMode.UseTransaction, async () =>
        {
            await base.CreateAsync(entity, token);

            await _outbox.AddAsync(new RentChargeCreated
            {
                RentChargeId = entity.Id,
                TenancyId = tenancy.Id,
                PeriodStart = entity.PeriodStart
            }, dedupKey: $"RentChargeCreated:{tenancy.Id}", token);

        }, token);

        var dto = Mappers.RentChargeToDto(entity);
        return dto;
    }

    public async Task DeleteRangeAsync(ICollection<RentCharge> rentCharges, CancellationToken token)
    {
        await base.DeleteRangeAsync(rentCharges, token);
    }
}
