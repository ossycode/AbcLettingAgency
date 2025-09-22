using AbcLettingAgency.Features.Properties.Interface;
using AbcLettingAgency.Features.Properties.Services;
using AbcLettingAgency.Shared;
using AbcLettingAgency.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AbcLettingAgency.Features.Properties.Controller;

[ApiController]
[Route("api/properties")]
//[Authorize(Roles = "Admin,Manager,Basic")]
public sealed class PropertiesController : ControllerBase
{
    private readonly IPropertyService _svc;
    public PropertiesController(IPropertyService svc) => _svc = svc;

    //[HttpGet]
    //public async Task<ActionResult<PagedResult<PropertyListItemDto>>> GetList(
    //    [FromQuery] string? q,
    //    [FromQuery] string? landlordId,
    //    [FromQuery] string? city,
    //    [FromQuery] bool? furnished,
    //    [FromQuery] int? bedroomsMin,
    //    [FromQuery] int? bedroomsMax,
    //    [FromQuery] bool? availableOnly,
    //    [FromQuery] int page = 1,
    //    [FromQuery] int pageSize = 20,
    //    CancellationToken ct = default)
    //{
    //    var filters = new PropertyFilters(q, landlordId, city, furnished, bedroomsMin, bedroomsMax, availableOnly);
    //    var result = await _svc.GetPagedAsync(filters, new PageRequest(page, pageSize), ct);
    //    return Ok(result);
    //}

    //// GET /api/properties/{id}
    //[HttpGet("{id}")]
    //public async Task<ActionResult<PropertyDetailDto>> GetOne(string id, CancellationToken ct = default)
    //{
    //    var dto = await _svc.GetDetailAsync(id, ct);
    //    return dto is null ? NotFound() : Ok(dto);
    //}

    //// POST /api/properties
    //[HttpPost]
    //[Authorize(Roles = "Admin,Manager")]
    //public async Task<ActionResult> Create([FromBody] CreatePropertyDto dto, CancellationToken ct = default)
    //{
    //    var id = await _svc.CreateAsync(dto, ct);
    //    return CreatedAtAction(nameof(GetOne), new { id }, new { id });
    //}

    //// PUT /api/properties/{id}
    //[HttpPut("{id}")]
    //[Authorize(Roles = "Admin,Manager")]
    //public async Task<ActionResult> Update(string id, [FromBody] UpdatePropertyDto dto, CancellationToken ct = default)
    //{
    //    await _svc.UpdateAsync(id, dto, ct);
    //    return NoContent();
    //}

    //// DELETE /api/properties/{id}
    //[HttpDelete("{id}")]
    //[Authorize(Roles = "Admin")]
    //public async Task<ActionResult> Delete(string id, CancellationToken ct = default)
    //{
    //    await _svc.DeleteAsync(id, ct);
    //    return NoContent();
    //}

    //// POST /api/properties/{id}/maintenance
    //[HttpPost("{id}/maintenance")]
    //[Authorize(Roles = "Admin,Manager")]
    //public async Task<ActionResult> AddMaintenance(string id, [FromBody] CreateMaintenanceDto dto, CancellationToken ct = default)
    //{
    //    var mid = await _svc.AddMaintenanceAsync(id, dto, ct);
    //    return Created($"/api/properties/{id}/maintenance/{mid}", new { id = mid });
    //}

    //// GET /api/properties/{id}/maintenance
    //[HttpGet("{id}/maintenance")]
    //public async Task<ActionResult<IEnumerable<PropertyMaintenanceDto>>> GetMaintenance(string id, CancellationToken ct = default)
    //{
    //    var rows = await _svc.GetMaintenanceAsync(id, ct);
    //    return Ok(rows);
    //}

    //// POST /api/properties/{id}/tenancies
    //[HttpPost("{id}/tenancies")]
    //[Authorize(Roles = "Admin,Manager")]
    //public async Task<ActionResult> AddTenancy(string id, [FromBody] CreateTenancyDto dto, CancellationToken ct = default)
    //{
    //    var tid = await _svc.AddTenancyAsync(id, dto, ct);
    //    return Created($"/api/tenancies/{tid}", new { id = tid });
    //}

    //// GET /api/properties/{id}/tenancies
    //[HttpGet("{id}/tenancies")]
    //public async Task<ActionResult<IEnumerable<PropertyTenancyDto>>> GetTenancies(string id, CancellationToken ct = default)
    //{
    //    var rows = await _svc.GetTenanciesAsync(id, ct);
    //    return Ok(rows);
    //}
}


 //var result = await _svc.GetDtoByIdAsync(id, ct); // Result<PropertyDto>
 //   return FromResult(result);