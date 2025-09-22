using System.Linq.Expressions;

namespace AbcLettingAgency.Shared.Abstractions;

public interface IEntityReadService<TEntity> where TEntity : IEntityBase
{
    IQueryable<TEntity> GetAll(bool readOnly = true);   
    Task<TEntity?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<TResult?> GetByIdAsync<TResult>(string id,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default);
}

public interface IEntityWriteService<TEntity> where TEntity : IEntityBase
{
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);
}

public interface IEntityService<TEntity> :
    IEntityReadService<TEntity>,
    IEntityWriteService<TEntity>
    where TEntity : class, IEntityBase
{ }
