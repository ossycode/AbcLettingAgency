using AbcLettingAgency.Attributes;
using AbcLettingAgency.Authorization;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Enums;
using AbcLettingAgency.Features.TenantManagement.RentCharges;
using AbcLettingAgency.Features.TenantManagement.RentCharges.Interfaces;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Controllers;
using AbcLettingAgency.Shared.Paging;
using AbcLettingAgency.Shared.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AbcLettingAgency.Features.TenantManagement.Controllers;

[HasPermission(AppFeature.Entity, AppAction.Read)]
[Route("api/rentcharge")]
public class RentChargeController(
    IEntityServiceFactory factory,
    IRentChargeService rentChargeService)
  : ApiControllerBase<RentCharge, RentChargeDto>(factory)
{
    private readonly IRentChargeService _service = rentChargeService;

    protected override Expression<Func<RentCharge, RentChargeDto>> Selector =>
        rc => new RentChargeDto
        {
            Id = rc.Id,
            TenancyId = rc.TenancyId,
            PeriodStart = rc.PeriodStart,
            PeriodEnd = rc.PeriodEnd,
            DueDate = rc.DueDate,
            Amount = rc.Amount,
            CommissionDue = rc.CommissionDue,
            AmountAfterCommission = rc.AmountAfterCommission,
            Status = rc.Status,
            Notes = rc.Notes,
            CreatedAt = rc.CreatedAt,
            UpdatedAt = rc.UpdatedAt,
            Paid = rc.Receipts.Sum(r => r.Amount),
            Outstanding = rc.Amount - rc.Receipts.Sum(r => r.Amount),
            IsOverdue = rc.Status == ChargeStatus.OPEN && rc.DueDate.Date < DateTime.UtcNow.Date
        };

    [HttpGet]
    public Task<ActionResult<PagedList<RentChargeDto>>> Get(
        [FromQuery] QueryOptions opts,
        [FromQuery] List<FilterRule>? filters,
        CancellationToken ct)
        => PagedAsync(opts, filters, EntityService.GetAll(), ct);

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
    {
        var dto = await EntityService.GetByIdAsync(id, Selector, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HasPermission(AppFeature.Entity, AppAction.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RentChargeCreateModel req, CancellationToken ct)
        => FromResult(await _service.CreateAsync(req, ct));
   
}
