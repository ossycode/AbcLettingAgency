using AbcLettingAgency.Attributes;
using AbcLettingAgency.Authorization;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Enums;
using AbcLettingAgency.Features.TenantManagement.Tenancies;
using AbcLettingAgency.Features.TenantManagement.Tenancies.Interface;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Controllers;
using AbcLettingAgency.Shared.Paging;
using AbcLettingAgency.Shared.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace AbcLettingAgency.Features.TenantManagement.Controllers;

[HasPermission(AppFeature.Entity, AppAction.Read)]
[Route("api/tenancy")]
public class TenancyController(
    IEntityServiceFactory factory,
    ITenancyService tenancyService,
    ITenancyTenantService tenancyTenantService)
  : ApiControllerBase<Tenancy, TenancyDto>(factory)
{
    private readonly ITenancyService _service = tenancyService;
    private readonly ITenancyTenantService _tenancyTenantService = tenancyTenantService;

    protected override Expression<Func<Tenancy, TenancyDto>> Selector =>
        t => new TenancyDto
        {
            Id = t.Id,
            PropertyId = t.PropertyId,
            LandlordId = t.LandlordId,
            Status = t.Status,
            StartDate = t.StartDate,
            EndDate = t.EndDate,
            RentDueDay = t.RentDueDay,
            Frequency = t.Frequency,
            RentAmount = t.RentAmount,
            CommissionPercent = t.CommissionPercent,
            DepositAmount = t.DepositAmount,
            DepositLocation = t.DepositLocation,
            NextChargeDate = t.NextChargeDate,
            Notes = t.Notes,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            Occupants = t.Occupants.Select(o => new OccupantDto
            {
                TenantId = o.TenantId,
                FullName = o.Tenant.FirstName + " " + o.Tenant.LastName,
                Email = o.Tenant.Email,
                Phone = o.Tenant.Phone,
                IsPrimary = o.IsPrimary,
                ResponsibilitySharePercent = o.ResponsibilitySharePercent,
                OccupancyStart = o.OccupancyStart,
                OccupancyEnd = o.OccupancyEnd
            })
        };

    [HttpGet]
    public Task<ActionResult<PagedList<TenancyDto>>> Get(
        [FromQuery] QueryOptions opts,
        [FromQuery] List<FilterRule>? filters,
        CancellationToken ct)
        => PagedAsync(opts, filters, _service.GetAll(), ct);

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
    {
        var dto = await EntityService.GetByIdAsync(id, Selector, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HasPermission(AppFeature.Entity, AppAction.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenancyRequest req, CancellationToken ct)
        => FromResult(await _service.CreateAsync(req, ct));

    [HasPermission(AppFeature.Entity, AppAction.Update)]
    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateTenancyRequest req, CancellationToken ct)
        => FromResult(await _service.UpdateAsync(id, req, ct));

    [HasPermission(AppFeature.Entity, AppAction.Delete)]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => FromResult(await _service.DeleteAsync(id, ct));


    [HasPermission(AppFeature.Entity, AppAction.Update)]
    [HttpPatch("{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateTenancyStatusRequest req, CancellationToken ct)
        => FromResult(await _service.UpdateTenancyStatusAsync(id, req.Status, req.Note, ct));

    [HasPermission(AppFeature.Entity, AppAction.Update)]
    [HttpPut("{id:long}/occupants")]
    public async Task<IActionResult> UpdateOccupants(long id, [FromBody] IEnumerable<UpdateOccupant> occupants, CancellationToken ct)
        => FromResult(await _tenancyTenantService.ReplaceAsync(id, occupants, ct));

    [HasPermission(AppFeature.Entity, AppAction.Update)]
    [HttpPatch("{id:long}/billing-schedule")]
    public async Task<IActionResult> UpdateBillingSchedule(long id, [FromBody] UpdateBillingScheduleRequest req, CancellationToken ct)
        => FromResult(await _service.UpdateBillingScheduleAsync(id, req, ct));

    [HasPermission(AppFeature.Entity, AppAction.Update)]
    [HttpPatch("{id:long}/next-charge-date")]
    public async Task<IActionResult> SetNextChargeDate(long id, [FromBody] SetNextChargeDateRequest req, CancellationToken ct)
        => FromResult(await _service.SetNextChargeDateAsync(id, req.NextChargeDate, ct));

    [HttpPost("onboarding")]
    public async Task<IActionResult> Start([FromBody] StartTenancyRequest req,
      [FromServices] ITenancyOnboardingService tenancyOnboarding,
      CancellationToken ct)
    {
        var result = await tenancyOnboarding.StartTenancyAsync(req, ct);
        return FromResult(result);
    }
}



