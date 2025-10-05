using AbcLettingAgency.Attributes;
using AbcLettingAgency.Authorization;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Features.Properties.Interface;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Controllers;
using AbcLettingAgency.Shared.Paging;
using AbcLettingAgency.Shared.Query;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace AbcLettingAgency.Features.Properties.Controller;

[HasPermission(AppFeature.Entity, AppAction.Read)]
[Route("api/properties")]
public sealed class PropertiesController(IEntityServiceFactory factory,
    IPropertyService propertyService) : ApiControllerBase<Property, PropertyDto>(factory)
{
    private readonly IPropertyService _propertyService = propertyService;
    protected override Expression<Func<Property, PropertyDto>> Selector =>
    l => new PropertyDto
    {
        Id = l.Id,
        Code = l.Code,
        AddressLine1 = l.AddressLine1,
        AddressLine2 = l.AddressLine2,
        City = l.City,
        Postcode = l.Postcode,
        Bedrooms = l.Bedrooms,
        Bathrooms = l.Bathrooms,
        Furnished = l.Furnished,
        AvailableFrom = l.AvailableFrom,
        LandlordId = l.LandlordId,
        UpdatedAt = l.UpdatedAt,
        CreatedAt = l.CreatedAt,
        Notes = l.Notes,
        LandlordName = l.Landlord.Name
    };

    [HttpGet]
    public Task<ActionResult<PagedList<PropertyDto>>> Get(
    [FromQuery] QueryOptions opts,
    [FromQuery] List<FilterRule>? filters,
    CancellationToken ct)
    => PagedAsync(opts, filters, _propertyService.GetAll(), ct);

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
    {
        var result = await EntityService.GetByIdAsync(id, Selector, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HasPermission(AppFeature.Entity, AppAction.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest req, CancellationToken ct)
    {
        var result = await _propertyService.CreateAsync(req, ct);
        return FromResult(result);
    }

    [HasPermission(AppFeature.Entity, AppAction.Update)]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePropertyRequest req, CancellationToken ct)
    {
        return FromResult(await _propertyService.UpdateAsync(id, req, ct));
    }

    [HasPermission(AppFeature.Entity, AppAction.Delete)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        return FromResult(await _propertyService.DeleteAsync(id, ct));
    }

}