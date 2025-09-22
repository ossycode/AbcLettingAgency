using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Features.Properties.Interface;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Services;
using static System.ArgumentNullException;

namespace AbcLettingAgency.Features.Properties.Services;


public class PropertyService(IEntityServiceDependencies deps)
        : BaseEntityService<Property>(deps), IPropertyService
{

    // availability = AvailableFrom <= today AND no active tenancy
    private static bool IsAvailableCore(DateTime? availableFrom, bool hasActiveTenancy)
        => (!availableFrom.HasValue || availableFrom.Value.Date <= DateTime.UtcNow.Date) && !hasActiveTenancy;

 
    public async Task<Property> CreateAsync(CreatePropertyDto dto, CancellationToken ct)
    {
        ThrowIfNull(dto);

        // Code uniqueness is already enforced by index
        var entity = new Property
        {
            Id = Guid.NewGuid().ToString(),
            Code = dto.Code?.Trim(),
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            City = dto.City,
            Postcode = dto.Postcode,
            Bedrooms = dto.Bedrooms,
            Bathrooms = dto.Bathrooms,
            Furnished = dto.Furnished,
            AvailableFrom = dto.AvailableFrom,
            LandlordId = dto.LandlordId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await CreateAsync(entity, ct);

        return entity;
    }

    public async Task UpdateAsync(string id, UpdatePropertyDto dto, CancellationToken ct)
    {
        ThrowIfNull(dto);

        var entity = await GetByIdAsync(id);

        if (entity is null) throw new KeyNotFoundException("Property not found");

        entity.Code = dto.Code ?? entity.Code;
        entity.AddressLine1 = dto.AddressLine1 ?? entity.AddressLine1;
        entity.AddressLine2 = dto.AddressLine2 ?? entity.AddressLine2;
        entity.City = dto.City ?? entity.City;
        entity.Postcode = dto.Postcode ?? entity.Postcode;
        entity.Bedrooms = dto.Bedrooms ?? entity.Bedrooms;
        entity.Bathrooms = dto.Bathrooms ?? entity.Bathrooms;
        entity.Furnished = dto.Furnished ?? entity.Furnished;
        entity.AvailableFrom = dto.AvailableFrom ?? entity.AvailableFrom;
        entity.LandlordId = dto.LandlordId ?? entity.LandlordId;
        entity.UpdatedAt = DateTime.UtcNow;

        await UpdateAsync(entity, ct);
    }

    public async Task DeleteAsync(string id, CancellationToken ct)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is null) return;

       await DeleteAsync(entity, ct);
    }

    public Task<string> AddMaintenanceAsync(string propertyId, CreateMaintenanceDto dto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<PropertyMaintenanceDto>> GetMaintenanceAsync(string propertyId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<string> AddTenancyAsync(string propertyId, CreateTenancyDto dto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<PropertyTenancyDto>> GetTenanciesAsync(string propertyId, CancellationToken ct)
    {
        throw new NotImplementedException();

        //await ChangeScope.BatchAsync(async () =>
        //{

        //});
    }

    //public async Task<string> AddMaintenanceAsync(string propertyId, CreateMaintenanceDto dto, CancellationToken ct)
    //{
    //    // verify property exists
    //    var exists = await _db.Properties.AnyAsync(p => p.Id == propertyId, ct);
    //    if (!exists) throw new KeyNotFoundException("Property not found");

    //    var m = new MaintenanceJob
    //    {
    //        Id = Guid.NewGuid().ToString(),
    //        PropertyId = propertyId,
    //        Title = dto.Title,
    //        Details = dto.Details,
    //        Status = dto.Status ?? 0,
    //        OpenedAt = dto.OpenedAt ?? DateTime.UtcNow,
    //        ClosedAt = dto.ClosedAt,
    //        Cost = dto.Cost,
    //        InvoiceId = dto.InvoiceId,
    //        CreatedAt = DateTime.UtcNow,
    //        UpdatedAt = DateTime.UtcNow
    //    };

    //    _db.MaintenanceJobs.Add(m);
    //    await _db.SaveChangesAsync(ct);
    //    return m.Id;
    //}

    //public async Task<IReadOnlyList<PropertyMaintenanceDto>> GetMaintenanceAsync(string propertyId, CancellationToken ct)
    //{
    //    return await _db.MaintenanceJobs.AsNoTracking()
    //        .Where(m => m.PropertyId == propertyId)
    //        .OrderByDescending(m => m.OpenedAt)
    //        .Select(m => new PropertyMaintenanceDto(
    //            m.Id, m.Title, m.Details, m.Status, m.OpenedAt, m.ClosedAt, m.Cost, m.InvoiceId))
    //        .ToListAsync(ct);
    //}

    //public async Task<string> AddTenancyAsync(string propertyId, CreateTenancyDto dto, CancellationToken ct)
    //{
    //    var exists = await _db.Properties.AnyAsync(p => p.Id == propertyId, ct);
    //    if (!exists) throw new KeyNotFoundException("Property not found");

    //    // find landlord from property
    //    var landlordId = await _db.Properties.Where(p => p.Id == propertyId)
    //        .Select(p => p.LandlordId)
    //        .FirstAsync(ct);

    //    var t = new Tenancy
    //    {
    //        Id = Guid.NewGuid().ToString(),
    //        PropertyId = propertyId,
    //        LandlordId = landlordId,
    //        TenantId = dto.TenantId,
    //        Status = dto.Status,
    //        StartDate = dto.StartDate,
    //        EndDate = dto.EndDate,
    //        RentAmount = dto.RentAmount,
    //        RentDueDay = dto.RentDueDay,
    //        CommissionPercent = dto.CommissionPercent,
    //        DepositAmount = dto.DepositAmount,
    //        Notes = dto.Notes,
    //        CreatedAt = DateTime.UtcNow,
    //        UpdatedAt = DateTime.UtcNow
    //    };

    //    _db.Tenancies.Add(t);
    //    await _db.SaveChangesAsync(ct);
    //    return t.Id;
    //}

    //public async Task<IReadOnlyList<PropertyTenancyDto>> GetTenanciesAsync(string propertyId, CancellationToken ct)
    //{
    //    return await _db.Tenancies.AsNoTracking()
    //        .Include(t => t.Tenant)
    //        .Where(t => t.PropertyId == propertyId)
    //        .OrderByDescending(t => t.StartDate ?? DateTime.MinValue)
    //        .Select(t => new PropertyTenancyDto(
    //            t.Id, t.TenantId, (t.Tenant.FirstName + " " + t.Tenant.LastName).Trim(),
    //            t.Status, t.StartDate, t.EndDate, t.RentAmount))
    //        .ToListAsync(ct);
    //}
}
