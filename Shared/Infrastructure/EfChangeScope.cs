using AbcLettingAgency.Data;
using AbcLettingAgency.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AbcLettingAgency.Shared.Infrastructure;

public sealed class EfChangeScope<TDbContext>(TDbContext db) : IChangeScope
    where TDbContext : AppDbContext
{
    private readonly TDbContext _db = db;

    private  readonly AsyncLocal<int> Depth = new();
    private  readonly AsyncLocal<BatchMode?> AmbientMode = new();

    public bool InBatch => Depth.Value > 0;

    public Task BatchAsync(Func<Task> action, CancellationToken token)
        => BatchAsync(BatchMode.DeferSave, action, token);

    public async Task BatchAsync(BatchMode mode, Func<Task> action, CancellationToken ct)
    {
        var isOuter = Depth.Value == 0;
        IDbContextTransaction? tx = null;

        if (!isOuter)
        {
            // Enforce outer mode; don't allow upgrading mid-flight.
            var outer = AmbientMode.Value ?? BatchMode.DeferSave;
            if (mode == BatchMode.UseTransaction && outer == BatchMode.DeferSave)
                throw new InvalidOperationException(
                    "Cannot request a transactional batch inside a non-transactional batch. " +
                    "Start the outer batch with BatchMode.UseTransaction."
                );
        }

        Depth.Value++;

        try
        {
            if (isOuter)
            {
                AmbientMode.Value = mode;

                var strategy = _db.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    if (AmbientMode.Value == BatchMode.UseTransaction)
                        tx = await _db.Database.BeginTransactionAsync(ct);

                    try
                    {
                        await action();

                        // Save once at the outermost level
                        await _db.SaveChangesAsync(ct);

                        if (tx is not null)
                            await tx.CommitAsync(ct);
                    }
                    catch
                    {
                        if (tx is not null)
                            await tx.RollbackAsync(ct);
                        throw;
                    }
                });
            }
            else
            {
                await action();
            }
        }
        finally
        {
            Depth.Value = Math.Max(0, Depth.Value - 1);

            if (isOuter)
            {
                AmbientMode.Value = null;
                if (tx is not null)
                    await tx.DisposeAsync();
            }
        }
    }

    public async Task<T> BatchAsync<T>(BatchMode mode, Func<Task<T>> action, CancellationToken ct)
    {
        var isOuter = Depth.Value == 0;
        IDbContextTransaction? tx = null;

        if (!isOuter)
        {
            var outer = AmbientMode.Value ?? BatchMode.DeferSave;
            if (mode == BatchMode.UseTransaction && outer == BatchMode.DeferSave)
                throw new InvalidOperationException(
                    "Cannot request a transactional batch inside a non-transactional batch. " +
                    "Start the outer batch with BatchMode.UseTransaction."
                );
        }

        Depth.Value++;

        try
        {
            if (isOuter)
            {
                AmbientMode.Value = mode;
                var strategy = _db.Database.CreateExecutionStrategy();

                T result = default!;

                await strategy.ExecuteAsync(async () =>
                {
                    if (AmbientMode.Value == BatchMode.UseTransaction)
                        tx = await _db.Database.BeginTransactionAsync(ct);

                    try
                    {
                        result = await action();

                        // Single outermost SaveChanges
                        await _db.SaveChangesAsync(ct);

                        if (tx is not null)
                            await tx.CommitAsync(ct);
                    }
                    catch
                    {
                        if (tx is not null)
                            await tx.RollbackAsync(ct);
                        throw;
                    }
                });

                return result;
            }
            else
            {
                // Inner scopes: just run the work; outermost saves/commits
                return await action();
            }
        }
        finally
        {
            Depth.Value = Math.Max(0, Depth.Value - 1);

            if (isOuter)
            {
                AmbientMode.Value = null;
                if (tx is not null)
                    await tx.DisposeAsync();
            }
        }
    }
}
