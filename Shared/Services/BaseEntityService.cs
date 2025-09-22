using AbcLettingAgency.Data;
using AbcLettingAgency.Shared.Abstractions;
using AbcLettingAgency.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AbcLettingAgency.Shared.Services;

public abstract class BaseEntityService<TEntity>(IEntityServiceDependencies deps) : IEntityService<TEntity>
    where TEntity : class, IEntityBase
{
    protected readonly AppDbContext Db = deps.Db;
    protected readonly IChangeScope ChangeScope = deps.ChangeScope;

    // ---------- READ ----------
    public virtual IQueryable<TEntity> GetAll(bool readOnly = true)
        => readOnly ? Db.Set<TEntity>().AsNoTracking() : Db.Set<TEntity>();

    public virtual Task<TEntity?> GetByIdAsync(string id, CancellationToken ct = default) =>
        Db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);

    public virtual Task<TResult?> GetByIdAsync<TResult>(
        string id,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken ct = default)
        => Db.Set<TEntity>().AsNoTracking()
            .Where(e => e.Id == id)
            .Select(selector)
            .FirstOrDefaultAsync(ct);

    // ---------- WRITE ----------
    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        await Db.Set<TEntity>().AddAsync(entity, ct);
        if (!EfChangeScope<AppDbContext>.InBatch) await Db.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        Db.Set<TEntity>().Update(entity);
        if (!EfChangeScope<AppDbContext>.InBatch) await Db.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
    
        Db.Set<TEntity>().Remove(entity);
        if (!EfChangeScope<AppDbContext>.InBatch) await Db.SaveChangesAsync(ct);
    }

    protected Task BatchAsync(Func<Task> work, CancellationToken ct = default) =>
      ChangeScope.BatchAsync(work, ct);
}