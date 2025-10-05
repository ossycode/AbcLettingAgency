using AbcLettingAgency.Attributes;
using AbcLettingAgency.Authorization;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Features.TenantManagement.Tenants;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Controllers;
using AbcLettingAgency.Shared.Paging;
using AbcLettingAgency.Shared.Query;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace AbcLettingAgency.Features.TenantManagement.Controllers;

[HasPermission(AppFeature.Entity, AppAction.Read)]
[Route("api/tenant")]
public class TenantController(
    IEntityServiceFactory factory,
    ITenantService tenantService)
  : ApiControllerBase<Tenant, TenantDto>(factory)
{
    private readonly ITenantService _service = tenantService;

    protected override Expression<Func<Tenant, TenantDto>> Selector =>
        t => new TenantDto
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            Email = t.Email,
            SecondEmail = t.SecondEmail,
            Phone = t.Phone,
            SecondPhone = t.SecondPhone,
            Status = t.Status,
            Notes = t.Notes,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        };

    [HttpGet]
    public Task<ActionResult<PagedList<TenantDto>>> Get(
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
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest req, CancellationToken ct)
        => FromResult(await _service.CreateAsync(req, ct));

    [HasPermission(AppFeature.Entity, AppAction.Update)]
    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateTenantRequest req, CancellationToken ct)
        => FromResult(await _service.UpdateAsync(id, req, ct));

    [HasPermission(AppFeature.Entity, AppAction.Delete)]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => FromResult(await _service.DeleteAsync(id, ct));
}

