using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Shared.Attributes;
using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.Properties.Interface;

[AutoRegisterService]
public interface IPropertyService : IEntityService<Property>
{
    Task<Result> CreateAsync(CreatePropertyRequest req, CancellationToken token);
    Task<Result> UpdateAsync(string id, UpdatePropertyRequest req, CancellationToken token);
    Task<Result> DeleteAsync(string id, CancellationToken token);
}