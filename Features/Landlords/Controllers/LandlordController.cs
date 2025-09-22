using AbcLettingAgency.Attributes;
using AbcLettingAgency.Authorization;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Features.Landlords.Interface;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Controllers;
using AbcLettingAgency.Shared.Paging;
using AbcLettingAgency.Shared.Query;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace AbcLettingAgency.Features.Landlords.Controllers;

[HasPermission(AppFeature.Entity, AppAction.Read)]
[Route("api/landlord")]
public class LandlordController(
    IEntityServiceFactory factory,
    ILandlordService landlordService)
  : ApiControllerBase<Landlord, LandlordDto>(factory)
{
    private readonly ILandlordService _service = landlordService;
    protected override Expression<Func<Landlord, LandlordDto>> Selector =>
        l => new LandlordDto
        {
            Id = l.Id,
            Name = l.Name,
            Email = l.Email,
            Phone = l.Phone,
            PropertyCount = l.Properties.Count,
            TenancyCount = l.Tenancies.Count,
            InvoiceCount = l.Invoices.Count,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
            Address = l.Address,
            Notes = l.Notes
            
        };

    [HttpGet]
    public Task<ActionResult<PagedList<LandlordDto>>> Get(
        [FromQuery] QueryOptions opts,
        [FromQuery] List<FilterRule>? filters,
        CancellationToken ct)
        => PagedAsync(opts, filters, _service.GetAll(), ct);

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken ct)
    {
        var result = await EntityService.GetByIdAsync(id, Selector, ct);
         return result is null ? NotFound() : Ok(result);
    }

    [HasPermission(AppFeature.Entity, AppAction.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLandlordRequest req, CancellationToken ct)
    {
        var result = await _service.CreateAsync(req, ct);
           return FromResult(result);
    }

    [HasPermission(AppFeature.Entity, AppAction.Update)]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateLandlordRequest req, CancellationToken ct)
    {
        return FromResult(await _service.UpdateAsync(id, req, ct));
    }

    [HasPermission(AppFeature.Entity, AppAction.Delete)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
       return FromResult(await _service.DeleteAsync(id, ct));
    }
         
}

