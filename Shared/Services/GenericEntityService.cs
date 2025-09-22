using AbcLettingAgency.Data;
using AbcLettingAgency.Shared.Abstractions;

namespace AbcLettingAgency.Shared.Services;

public sealed class GenericEntityService<TEntity>(IEntityServiceDependencies deps)
    : BaseEntityService<TEntity>(deps), IEntityService<TEntity>
    where TEntity : class, IEntityBase
{
}
