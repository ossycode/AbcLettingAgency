using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Attributes;
using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.Landlords.Interface;

[AutoRegisterService] 
public interface ILandlordService : IEntityService<Landlord>
{
    Task<Result> CreateAsync(CreateLandlordRequest req, CancellationToken ct);
    Task<Result> UpdateAsync(string id, UpdateLandlordRequest req, CancellationToken ct);
    Task<Result> DeleteAsync(string id, CancellationToken ct);
}
