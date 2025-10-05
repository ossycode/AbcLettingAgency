using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Features.TenantManagement.Tenancies.Errors;
using AbcLettingAgency.Features.TenantManagement.Tenancies.Interface;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Exceptions;
using AbcLettingAgency.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Features.TenantManagement.Tenancies.Services;

public class TenancyTenantService(IEntityServiceDependencies deps, IEntityServiceFactory entityService) : BaseEntityService<TenancyTenant>(deps), ITenancyTenantService
{
    private readonly IEntityServiceFactory _entityService = entityService;

    public async Task<Result> CreateAsync(long tenancyId, CreateOccupant req, CancellationToken token)
    {
        // Load tenancy with current occupants (for validations)
        var tenancy = await _entityService.For<Tenancy>()
            .GetAll() // tracked is fine; not modifying tenancy itself
            .Include(t => t.Occupants)
            .FirstOrDefaultAsync(t => t.Id == tenancyId, token);

        if (tenancy is null)
            return Result.Failure(TenancyTenantErrors.TenancyNotFound(tenancyId));

        // Tenant must exist
        var tenantExists = await _entityService.For<Tenant>()
            .GetAll()
            .AnyAsync(t => t.Id == req.TenantId, token);

        if (!tenantExists)
            return Result.Failure(TenancyTenantErrors.TenantNotFound(req.TenantId));

        // No duplicate (TenancyId, TenantId)
        if (tenancy.Occupants.Any(o => o.TenantId == req.TenantId))
            return Result.Failure(TenancyTenantErrors.DuplicateLink(tenancyId, req.TenantId));

        // Only one primary per tenancy
        if (req.IsPrimary && tenancy.Occupants.Any(o => o.IsPrimary))
            return Result.Failure(TenancyTenantErrors.PrimaryExists(tenancyId));

        // Validate share (0..100) and total <= 100
        if (req.ResponsibilitySharePercent is < 0 or > 100)
            return Result.Failure(TenancyTenantErrors.InvalidShare());

        var existingShareTotal = tenancy.Occupants
            .Where(o => o.ResponsibilitySharePercent.HasValue)
            .Sum(o => o.ResponsibilitySharePercent!.Value);

        var projectedTotal = existingShareTotal + (req.ResponsibilitySharePercent ?? 0m);
        if (projectedTotal > 100m + 0.0001m)
            return Result.Failure(TenancyTenantErrors.ShareWouldExceed100());

        // Dates
        var start = req.OccupancyStart ?? tenancy.StartDate ?? DateTime.UtcNow.Date;
        if (req.OccupancyEnd.HasValue && req.OccupancyEnd.Value < start)
            return Result.Failure(TenancyTenantErrors.InvalidDates());

        var link = new TenancyTenant
        {
            TenancyId = tenancyId,
            TenantId = req.TenantId,
            IsPrimary = req.IsPrimary,
            ResponsibilitySharePercent = req.ResponsibilitySharePercent,
            OccupancyStart = start,
            OccupancyEnd = req.OccupancyEnd
        };

        await base.CreateAsync(link, token);
        return Result.Success();
    }

    public async Task CreateTenancyTenantRangeAsync(IEnumerable<TenancyTenant> entities, CancellationToken token)
    {
        await base.CreateRangeAsync(entities, token);
    }
    public async Task<Result> DeleteTenancyTenantRangeAsync(IEnumerable<TenancyTenant> entities, CancellationToken token)
    {
        await base.DeleteRangeAsync(entities, token);

        return Result.Success();
    }

    public async Task<Result> ReplaceAsync(long tenancyId, IEnumerable<UpdateOccupant> occupants, CancellationToken ct)
    {
        // Load tenancy “context” (start date) + ensure tenancy exists
        var tenancyCtx = await _entityService.For<Tenancy>()
            .GetAll() 
            .Where(t => t.Id == tenancyId)
            .Select(t => new { t.Id, t.StartDate })
            .FirstOrDefaultAsync(ct);

        if (tenancyCtx is null)
            return Result.Failure(TenancyTenantErrors.TenancyNotFound(tenancyId));

        var incomingList = occupants.ToList();

        // 1) validations
        if (incomingList.Count(o => o.IsPrimary) != 1)
            return Result.Failure(TenancyTenantErrors.PrimaryExists(tenancyId));

        var ids = incomingList.Select(o => o.TenantId).ToArray();
        if (ids.Length != ids.Distinct().Count())
            return Result.Failure(TenancyErrors.DuplicateOccupants());

        if (incomingList.Exists(o => o.ResponsibilitySharePercent.HasValue))
        {
            var sum = incomingList.Sum(o => o.ResponsibilitySharePercent ?? 0m);
            if (Math.Abs(sum - 100m) > 0.001m)
                return Result.Failure(TenancyTenantErrors.InvalidShare());
        }

        // verify new tenant ids exist
        var newIds = ids.Distinct().ToArray();
        var exists = await _entityService.For<Tenant>().GetAll()
            .Where(t => newIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(ct);

        var missing = newIds.Except(exists).ToArray();
        if (missing.Length > 0)
            return Result.Failure(TenancyTenantErrors.TenantNotFound(missing[0]));

        // 2) load current occupants
        var current = await _entityService.For<TenancyTenant>()
            .GetAll(readOnly: false)
            .Where(o => o.TenancyId == tenancyId)
            .ToListAsync(ct);

        var incomingById = incomingList.ToDictionary(x => x.TenantId);

        var toRemove = current.Where(c => !incomingById.ContainsKey(c.TenantId)).ToList();
        var remaining = current.Where(c => incomingById.ContainsKey(c.TenantId)).ToList();
        var currentIds = remaining.Select(r => r.TenantId).ToHashSet();
        var toAdd = incomingList.Where(i => !currentIds.Contains(i.TenantId))
            .Select(i => new TenancyTenant
            {
                TenancyId = tenancyId,
                TenantId = i.TenantId,
                IsPrimary = i.IsPrimary,
                ResponsibilitySharePercent = i.ResponsibilitySharePercent,
                OccupancyStart = i.OccupancyStart ?? tenancyCtx.StartDate,
                OccupancyEnd = i.OccupancyEnd
            })
            .ToList();

        await ChangeScope.BatchAsync(BatchMode.UseTransaction, async () =>
        {
            foreach (var c in remaining)
            {
                var i = incomingById[c.TenantId];
                c.IsPrimary = i.IsPrimary;
                c.ResponsibilitySharePercent = i.ResponsibilitySharePercent;
                c.OccupancyStart = i.OccupancyStart ?? tenancyCtx.StartDate;
                c.OccupancyEnd = i.OccupancyEnd;
            }
            
            if (toRemove.Count > 0) await base.DeleteRangeAsync(toRemove, ct);
            if (toAdd.Count > 0) await base.CreateRangeAsync(toAdd, ct);
        }, ct);

        return Result.Success();
    }

}
