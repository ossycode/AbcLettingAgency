using AbcLettingAgency.Data;
using AbcLettingAgency.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AbcLettingAgency.Shared.Infrastructure;

public sealed class EfChangeScope<TDbContext>(TDbContext db) : IChangeScope where TDbContext : AppDbContext
{
    private readonly TDbContext _db = db;
    private static readonly AsyncLocal<int> _depth = new();
    public static bool InBatch => _depth.Value > 0;

    public async Task BatchAsync(Func<Task> action, CancellationToken ct = default)
    {
        _depth.Value++;
        IDbContextTransaction? tx = null;
        try
        {
            if (_depth.Value == 1)
                tx = await _db.Database.BeginTransactionAsync(ct);

            await action();

            if (_depth.Value == 1)
            {
                await _db.SaveChangesAsync(ct);
                if (tx is not null) await tx.CommitAsync(ct);
            }
        }
        catch
        {
            if (_depth.Value == 1 && tx is not null)
                await tx.RollbackAsync(ct);
            throw;
        }
        finally
        {
            if (_depth.Value == 1)
                await (tx?.DisposeAsync() ?? ValueTask.CompletedTask);
            _depth.Value = Math.Max(0, _depth.Value - 1);
        }
    }
}

