using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Features.TenantManagement.Tenancies.helpers;
using AbcLettingAgency.Features.TenantManagement.Tenancies.Interface;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Events;
using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Features.TenantManagement.EventsHandler;

public sealed class RentChargeCreatedHandler(
    IEntityServiceFactory entityService,
    ITenancyService tenancyService,
    ILogger<RentChargeCreatedHandler> log)
    : IIntegrationEventHandler<RentChargeCreated>
{
    public async Task HandleAsync(RentChargeCreated evt, CancellationToken ct)
    {
        // Load tenancy to compute next anchor from PeriodStart
        var tenancy = await entityService.For<Tenancy>().GetAll()
            .FirstOrDefaultAsync(t => t.Id == evt.TenancyId, ct);

        if (tenancy is null)
        {
            log.LogWarning("Tenancy {Id} not found for RentChargeCreated {EventId}", evt.TenancyId, evt.Id);
            return; // swallow (message considered processed)
        }

        // Compute suggested next
        var next = RentSchedule.NextPeriodStart(evt.PeriodStart, tenancy.Frequency, tenancy.RentDueDay);

        // Optional safety: only move forward
        if (tenancy.NextChargeDate.HasValue && next <= tenancy.NextChargeDate.Value.Date)
            return;

        // Let the domain service validate and persist
        var res = await tenancyService.SetNextChargeDateAsync(tenancy.Id, next, ct);
        if (!res.IsSuccess)
            throw new InvalidOperationException(string.Join("; ", res.Errors!.Select(e => e.Message)));
    }
}
