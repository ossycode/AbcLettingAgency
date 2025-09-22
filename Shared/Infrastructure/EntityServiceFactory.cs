using AbcLettingAgency.Shared.Abstractions;

namespace AbcLettingAgency.Shared.Infrastructure;

public sealed class EntityServiceFactory(IServiceProvider sp) : IEntityServiceFactory
{
    private readonly IServiceProvider _sp = sp;

    public IEntityService<TEntity> For<TEntity>() where TEntity : class, IEntityBase =>
        _sp.GetRequiredService<IEntityService<TEntity>>();
}


