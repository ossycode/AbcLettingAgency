namespace AbcLettingAgency.Shared.Abstractions;

public interface IEntityServiceFactory
{
    IEntityService<TEntity> For<TEntity>() where TEntity : class, IEntityBase;
}