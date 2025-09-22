using AbcLettingAgency.Data;
using AbcLettingAgency.Shared.Abstractions;

namespace AbcLettingAgency.Shared.Services;

public sealed class EntityServiceDependencies(
 AppDbContext db,
 IChangeScope scope) : IEntityServiceDependencies
{
    public AppDbContext Db { get; } = db;
    public IChangeScope ChangeScope { get; } = scope;
}
