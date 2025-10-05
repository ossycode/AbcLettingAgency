using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Enums;
using AbcLettingAgency.Features.TenantManagement.RentCharges;
using AbcLettingAgency.Features.TenantManagement.RentCharges.Interfaces;
using AbcLettingAgency.Features.TenantManagement.Tenancies;
using AbcLettingAgency.Features.TenantManagement.Tenancies.Errors;
using AbcLettingAgency.Features.TenantManagement.Tenancies.helpers;
using AbcLettingAgency.Features.TenantManagement.Tenancies.Interface;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Exceptions;
using AbcLettingAgency.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Features.TenantManagement.Tenancies.Services;

public class TenancyService(IEntityServiceDependencies deps,
    IEntityServiceFactory entityService, 
    ITenancyTenantService tenancyTenantService,
    IRentChargeService rentChargeService
    ) : BaseEntityService<Tenancy>(deps), ITenancyService
{
    private readonly IEntityServiceFactory _entityService = entityService;
    private readonly ITenancyTenantService _tenancyTenantService = tenancyTenantService;
    private readonly IRentChargeService _rentChargeService = rentChargeService;

    public async Task<Result<TenancyDto>> CreateAsync(CreateTenancyRequest req, CancellationToken token)
    {
        // Load & validate principals
        var property = await _entityService.For<Property>().GetAll()
            .Where(p => p.Id == req.PropertyId)
            .Select(p => new { p.Id, p.LandlordId })
            .FirstOrDefaultAsync(token);

        if (property == null)
            return TenancyErrors.PropertyNotFound(req.PropertyId);

        if (property.LandlordId != req.LandlordId) 
            return TenancyErrors.PropertyNotOwnedByLandlord(req.PropertyId, req.LandlordId);

        var tenantIds = req.Occupants.Select(o => o.TenantId).Distinct().ToArray();

        var existingTenantIds = await _entityService.For<Tenant>().GetAll()
            .Where(t => tenantIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(token);

        var missing = tenantIds.Except(existingTenantIds).ToArray();
        if (missing.Length > 0)
            return TenancyErrors.TenantNotFound(missing);

        if (req.Occupants.Count(o => o.IsPrimary) != 1) 
            return TenancyErrors.InvalidPrimaryOccupants();

        var occList = req.Occupants.ToList();
        if (occList.Exists(o => o.ResponsibilitySharePercent.HasValue))
        {
            var sum = occList.Sum(o => o.ResponsibilitySharePercent ?? 0m);
            if (Math.Abs(sum - 100m) > 0.001m)
                return TenancyErrors.InvalidShareDistribution();
        }

        if (req.RentDueDay is < 1 or > 31)
            return TenancyErrors.InvalidRentDueDay();

        var tenancy = new Tenancy
        {
            PropertyId = req.PropertyId,
            LandlordId = req.LandlordId,
            Status = req.Status,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            RentDueDay = req.RentDueDay,
            Frequency = req.Frequency,
            RentAmount = req.RentAmount,
            CommissionPercent = req.CommissionPercent,
            DepositAmount = req.DepositAmount,
            DepositLocation = req.DepositLocation,
            Notes = req.Notes,
            NextChargeDate = ComputeNextChargeDate(req.StartDate, req.RentDueDay, req.Frequency),
            Occupants = req.Occupants.Select(o => new TenancyTenant
            {
                TenantId = o.TenantId,
                IsPrimary = o.IsPrimary,
                ResponsibilitySharePercent = o.ResponsibilitySharePercent,
                OccupancyStart = o.OccupancyStart ?? req.StartDate,
                OccupancyEnd = o.OccupancyEnd
            }).ToList()
        };

        await ChangeScope.BatchAsync(BatchMode.UseTransaction, async () =>
        {
            await base.CreateAsync(tenancy, token);
        }, token);

        // Optional: immediately raise the first rent charge
        // await _rentScheduler.MaybeSeedFirstChargeAsync(tenancy.Id, ct);

        var dto = Mappers.TenancyToDto(tenancy);
        return dto;
    }

    public async Task<Result> UpdateAsync(long tenancyId, UpdateTenancyRequest req, CancellationToken token)
    {
        var tenancy = await _entityService.For<Tenancy>()
            .GetAll()
            .Where(p => p.Id == tenancyId)
            .FirstOrDefaultAsync(token);

        if (tenancy == null)
            return Result.Failure(TenancyErrors.NotFound(tenancyId));

        if (req.RentDueDay is < 1 or > 31)
            return Result.Failure(TenancyErrors.InvalidRentDueDay());

        await ChangeScope.BatchAsync(BatchMode.DeferSave, async () =>
        {
            tenancy.StartDate = req.StartDate;
            tenancy.EndDate = req.EndDate;
            tenancy.RentDueDay = req.RentDueDay;
            tenancy.Frequency = req.Frequency;
            tenancy.RentAmount = req.RentAmount;
            tenancy.CommissionPercent = req.CommissionPercent;
            tenancy.DepositAmount = req.DepositAmount;
            tenancy.DepositLocation = req.DepositLocation;
            tenancy.Status = req.Status;
            tenancy.Notes = req.Notes;

            tenancy.NextChargeDate = ComputeNextChargeDate(tenancy.StartDate, tenancy.RentDueDay, tenancy.Frequency);

            await base.UpdateAsync(tenancy, token);
        }, token);
        

        return Result.Success();

    }

    public async Task<Result> DeleteAsync(long tenancyId, CancellationToken token)
    {
        var tenancy = await _entityService.For<Tenancy>()
            .GetAll()
            .FirstOrDefaultAsync(t => t.Id == tenancyId, token);

        if (tenancy is null)
            return Result.Failure(TenancyErrors.NotFound(tenancyId));

        var hasReceipts = await _entityService.For<RentReceipt>()
            .GetAll()
            .AnyAsync(r => r.TenancyId == tenancyId, token);

        var hasCharges = await _entityService.For<RentCharge>()
            .GetAll()
            .AnyAsync(c => c.TenancyId == tenancyId, token);

        var hasLedger = await _entityService.For<ClientLedger>()
            .GetAll()
            .AnyAsync(l => l.TenancyId == tenancyId, token);

        var hasInvoices = await _entityService.For<Invoice>()
            .GetAll()
            .AnyAsync(i => i.TenancyId == tenancyId, token);

        var hasDocuments = await _entityService.For<Document>()
            .GetAll()
            .AnyAsync(d => d.TenancyId == tenancyId, token);

        if (hasReceipts || hasCharges || hasLedger || hasInvoices || hasDocuments)
            return Result.Failure(TenancyErrors.HasLinkedEntities());

        await ChangeScope.BatchAsync(BatchMode.UseTransaction, async () =>
        {
            await base.DeleteAsync(tenancy, token);
        }, token);

        return Result.Success();
    }

    public async Task<Result> UpdateTenancyStatusAsync(long tenancyId, TenancyStatus newStatus, string? note, CancellationToken token)
    {
        var tenancy = await _entityService.For<Tenancy>().GetAll()
            .Where(p => p.Id == tenancyId)
            .FirstOrDefaultAsync(token);

        if (tenancy is null)
            return Result.Failure(TenancyErrors.NotFound(tenancyId));

        // Optional: enforce transition rules
        // if (!AllowedTransition(tenancy.Status, newStatus)) return Result.Failure(TenancyErrors.InvalidStatusTransition(...));

        await ChangeScope.BatchAsync(BatchMode.UseTransaction, async () =>
        {
            tenancy.Status = newStatus;
            if (!string.IsNullOrWhiteSpace(note))
                tenancy.Notes = string.IsNullOrEmpty(tenancy.Notes) ? note : $"{tenancy.Notes}\n{note}";

            await base.UpdateAsync(tenancy, token);
        }, token);

        return Result.Success();
    }


    //public async Task<Result> UpdateOccupantsAsync(long tenancyId, IEnumerable<UpdateOccupant> occupants, CancellationToken token)
    //{
    //    var tenancy = await _entityService.For<Tenancy>().GetAll()
    //        .Where(p => p.Id == tenancyId).Include(t =>  t.Occupants)
    //        .FirstOrDefaultAsync(token);

    //    if (tenancy is null)
    //        return Result.Failure(TenancyErrors.NotFound(tenancyId));

    //    var occupantsList = occupants.ToList();
    //    if (occupantsList.Count(o => o.IsPrimary) != 1)
    //        return Result.Failure(TenancyErrors.InvalidPrimaryOccupants());

    //    var ids = occupantsList.Select(o => o.TenantId).ToArray();
    //    if (ids.Length != ids.Distinct().Count())
    //        return Result.Failure(TenancyErrors.DuplicateOccupants());

    //    if (occupantsList.Exists(o => o.ResponsibilitySharePercent.HasValue))
    //    {
    //        var sum = occupantsList.Sum(o => o.ResponsibilitySharePercent ?? 0m);
    //        if (Math.Abs(sum - 100m) > 0.001m)
    //            return Result.Failure(TenancyErrors.InvalidShareDistribution());
    //    }

    //    var existingIds = tenancy.Occupants.Select(o => o.TenantId).ToHashSet();
    //    var newIds = ids.Where(id => !existingIds.Contains(id)).Distinct().ToArray();
    //    if (newIds.Length > 0)
    //    {
    //        var found = await _entityService.For<Tenant>().GetAll()
    //            .Where(t => newIds.Contains(t.Id)).Select(t => t.Id).ToListAsync(token);
    //        var missing = newIds.Except(found).ToArray();
    //        if (missing.Length > 0)
    //            return Result.Failure(TenancyErrors.TenantNotFound(missing));
    //    }


    //    await ChangeScope.BatchAsync(BatchMode.UseTransaction, async () =>
    //    {
    //        var incoming = occupantsList.ToDictionary(x => x.TenantId);

    //        var toRemove = tenancy.Occupants.Where(o => !incoming.ContainsKey(o.TenantId)).ToList();
    //        if (toRemove.Count > 0)
    //        {
    //            await _tenancyTenantService.DeleteTenancyTenantRangeAsync(toRemove, token);
    //        }

    //        foreach (var o in tenancy.Occupants.Where(o => incoming.ContainsKey(o.TenantId)))
    //        {
    //            var i = incoming[o.TenantId];
    //            o.IsPrimary = i.IsPrimary;
    //            o.ResponsibilitySharePercent = i.ResponsibilitySharePercent;
    //            o.OccupancyStart = i.OccupancyStart ?? tenancy.StartDate;
    //            o.OccupancyEnd = i.OccupancyEnd;
    //        }

    //        var currentIds = tenancy.Occupants
    //            .Where(o => !toRemove.Contains(o))
    //            .Select(o => o.TenantId)
    //            .ToHashSet();

    //        var toAdd = occupantsList
    //           .Where(x => !currentIds.Contains(x.TenantId))
    //           .Select(i => new TenancyTenant
    //           {
    //               TenancyId = tenancy.Id,
    //               TenantId = i.TenantId,
    //               IsPrimary = i.IsPrimary,
    //               ResponsibilitySharePercent = i.ResponsibilitySharePercent,
    //               OccupancyStart = i.OccupancyStart ?? tenancy.StartDate,
    //               OccupancyEnd = i.OccupancyEnd
    //           })
    //           .ToList();
    //        if (toAdd.Count > 0)
    //        {
    //            await _tenancyTenantService.CreateTenancyTenantRangeAsync(toAdd, token);
    //        }
               
    //    }, token);

    //    return Result.Success();
    //}

    public async Task<Result> UpdateBillingScheduleAsync(long tenancyId, UpdateBillingScheduleRequest req, CancellationToken token)
    {
        var tenancy = await _entityService.For<Tenancy>()
            .GetAll()
            .FirstOrDefaultAsync(t => t.Id == tenancyId, token);

        if (tenancy is null)
            return Result.Failure(TenancyErrors.NotFound(tenancyId));

        if (req.RentDueDay is < 1 or > 31)
            return Result.Failure(TenancyErrors.InvalidRentDueDay());

        if (tenancy.Status == TenancyStatus.ENDED)
            return Result.Failure(TenancyErrors.InvalidStatusForScheduleChange(tenancy.Status));


        var startForSchedule = req.StartDate ?? tenancy.StartDate;

        await ChangeScope.BatchAsync(BatchMode.UseTransaction, async () =>
        {
            // 1) Update schedule fields
            tenancy.StartDate = startForSchedule;           
            tenancy.RentDueDay = req.RentDueDay;
            tenancy.Frequency = req.Frequency;

            // 2) Recompute NextChargeDate
            tenancy.NextChargeDate = ComputeNextChargeDate(tenancy.StartDate, tenancy.RentDueDay, tenancy.Frequency);

            // 3) Optional: adjust future charges, if requested
            if (req.RebuildFutureCharges)
            {
                var effective = req.EffectiveFrom ?? DateTime.UtcNow;

                // Load only pending/future charges you want to touch
                var futureCharges = await _entityService.For<RentCharge>().GetAll()
                    .Where(c => c.TenancyId == tenancyId && c.Status == ChargeStatus.OPEN && c.DueDate >= effective)
                    .ToListAsync(token);

                // Simple strategy: delete and regenerate future charges
                // (Alternative: shift dates / recompute amounts)
                if (futureCharges.Count > 0)
                    await _rentChargeService.DeleteRangeAsync(futureCharges, token);

                var seedFrom = req.EffectiveFrom ?? tenancy.NextChargeDate ?? tenancy.StartDate ?? DateTime.UtcNow.Date;
                var next = ComputeNextChargeDate(seedFrom, tenancy.RentDueDay, tenancy.Frequency);

                if (tenancy.EndDate is not null && next is not null && next > tenancy.EndDate.Value.Date)
                    next = null;

                if (next is not null)
                {
                    // Use the service, not the DbSet
                    var rcModel = new RentChargeCreateModel
                    {
                        TenancyId = tenancyId,
                        PeriodStart = next.Value,
                        Notes = "Reseeded after schedule change",
                        AdvanceNextChargeDate = false   
                    };
                         await _rentChargeService.CreateAsync(rcModel, token);
                }
            } 

            await base.UpdateAsync(tenancy, token);
        }, token);

        return Result.Success();
    }

    public async Task<Result> SetNextChargeDateAsync(long tenancyId, DateTime? nextChargeDate, CancellationToken token)
    {
        var tenancy = await _entityService.For<Tenancy>()
            .GetAll()
            .FirstOrDefaultAsync(t => t.Id == tenancyId, token);

        if (tenancy is null)
            return Result.Failure(TenancyErrors.NotFound(tenancyId));

        if (nextChargeDate is not null)
        {
            var d = nextChargeDate.Value.Date;
            if (tenancy.StartDate is not null && d < tenancy.StartDate.Value.Date)
                return Result.Failure(TenancyErrors.InvalidNextChargeDate("Before tenancy start"));
            if (tenancy.EndDate is not null && d > tenancy.EndDate.Value.Date)
                return Result.Failure(TenancyErrors.InvalidNextChargeDate("After tenancy end"));
        }

        await ChangeScope.BatchAsync(BatchMode.DeferSave, async () =>
        {
            tenancy.NextChargeDate = nextChargeDate?.Date;
            await UpdateAsync(tenancy);
        }, token);

        return Result.Success();
    }

    private static DateTime? ComputeNextChargeDate(DateTime? start, int dueDay, RentFrequency freq)
    {
        if (start is null) return null;
        var s = start.Value;

   
        return freq switch
        {
            RentFrequency.Monthly => RentSchedule.FirstDueOnOrAfter(s.Date, dueDay),
            RentFrequency.FourWeekly => DateTime.SpecifyKind(s.Date.AddDays(28), s.Kind),
            RentFrequency.Weekly => DateTime.SpecifyKind(
                                           s.Date.AddDays(((int)DayOfWeek.Monday - (int)s.DayOfWeek + 7) % 7),
                                           s.Kind),
            _ => RentSchedule.FirstDueOnOrAfter(s.Date, dueDay)
        };
    }
}
