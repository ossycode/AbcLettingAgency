using AbcLettingAgency.Shared;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Shared.Attributes;

namespace AbcLettingAgency.Features.Properties.Interface;

[AutoRegisterService]
public interface IPropertyService 
{
    Task<Property> CreateAsync(CreatePropertyDto dto, CancellationToken ct);
    Task UpdateAsync(string id, UpdatePropertyDto dto, CancellationToken ct);

    Task<string> AddMaintenanceAsync(string propertyId, CreateMaintenanceDto dto, CancellationToken ct);
    Task<IReadOnlyList<PropertyMaintenanceDto>> GetMaintenanceAsync(string propertyId, CancellationToken ct);

    Task<string> AddTenancyAsync(string propertyId, CreateTenancyDto dto, CancellationToken ct);
    Task<IReadOnlyList<PropertyTenancyDto>> GetTenanciesAsync(string propertyId, CancellationToken ct);
}