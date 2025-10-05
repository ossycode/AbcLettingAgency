using System.Linq.Expressions;

namespace AbcLettingAgency.Shared.Abstractions;

public interface IEntityReadService<TEntity> where TEntity : IEntityBase
{
    IQueryable<TEntity> GetAll(bool readOnly = true);

    Task<TEntity?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<TResult?> GetByIdAsync<TResult>(long id,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default);
}

public interface IEntityWriteService<TEntity> where TEntity : IEntityBase
{
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);
    Task CreateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
}

public interface IEntityService<TEntity> :
    IEntityReadService<TEntity>,
    IEntityWriteService<TEntity>
    where TEntity : class, IEntityBase
{ }
