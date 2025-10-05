using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Enums;
using AbcLettingAgency.Features.Properties.Interface;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Exceptions;
using AbcLettingAgency.Shared.Services;
using AbcLettingAgency.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
using static System.ArgumentNullException;

namespace AbcLettingAgency.Features.Properties.Services;


public class PropertyService(IEntityServiceDependencies deps, FriendlyCodeGenerator codeGenerator, IEntityServiceFactory entityService)
        : BaseEntityService<Property>(deps), IPropertyService
{
    private readonly FriendlyCodeGenerator _codeGenerator = codeGenerator;
    private readonly IEntityServiceFactory _entityService = entityService;

    public async Task<Result> CreateAsync(CreatePropertyRequest req, CancellationToken token)
    {
      
        var landlordExisit = await _entityService.For<Landlord>().GetByIdAsync(req.LandlordId, token);

        if (landlordExisit is null) return Result.Failure(PropertyErrors.LandlordNotFound(req.LandlordId));


        var code = await _codeGenerator.GenerateForPropertyAsync("", req.AddressLine1, req.City, token);

        var entity = new Property
        {
            Code = code,
            AddressLine1 = req.AddressLine1,
            AddressLine2 = req.AddressLine2,
            City = req.City,
            Postcode = req.Postcode,
            Bedrooms = req.Bedrooms,
            Bathrooms = req.Bathrooms,
            Furnished = req.Furnished,
            AvailableFrom = req.AvailableFrom,
            LandlordId = req.LandlordId,
            Notes = req.Notes
        };

        await CreateAsync(entity, token);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(long id, CancellationToken token)
    {
        var entity = await _entityService.For<Property>().GetByIdAsync(id, token);

        if (entity is null) return Result.Failure(PropertyErrors.NotFound(id));

        var hasActiveTenancy = await _entityService.For<Tenancy>()
         .GetAll()
         .Where(t => t.PropertyId == id && t.Status == TenancyStatus.ACTIVE)
         .FirstOrDefaultAsync(token);

        if (hasActiveTenancy is not null) return Result.Failure(PropertyErrors.HasLinkedEntities());

        await DeleteAsync(entity, token);

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(long id, UpdatePropertyRequest req, CancellationToken token)
    {
       var entity =  await _entityService.For<Property>().GetByIdAsync(id, token);

        if (entity is null) return Result.Failure(PropertyErrors.NotFound(id));

        entity.AddressLine1 = req.AddressLine1.Trim();
        entity.AddressLine2 = req.AddressLine2?.Trim();
        entity.City = req.City?.Trim();
        entity.Postcode = req.Postcode?.Trim();
        entity.Bedrooms = req.Bedrooms;
        entity.Bathrooms = req.Bathrooms;
        entity.Furnished = req.Furnished;
        entity.AvailableFrom = req.AvailableFrom;
        entity.LandlordId = req.LandlordId;
        entity.Notes = req.Notes;

        await UpdateAsync(entity, token);

        return Result.Success();

    }
}