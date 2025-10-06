using AbcLettingAgency.Data;
using AbcLettingAgency.EntityModel.Agencies;
using AbcLettingAgency.Options;
using AbcLettingAgency.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AbcLettingAgency.Shared.Events;

public sealed class OutboxDispatcherWorker(IServiceProvider sp, 
    ILogger<OutboxDispatcherWorker> log,
    IOptions<OutboxOptions> opts,
     IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<OutboxDispatcherWorker> _log = log;
    private readonly OutboxOptions _opts = opts.Value;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(_opts.Timer));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sp = scope.ServiceProvider;
                var db = sp.GetRequiredService<AppDbContext>();
                var ambient = sp.GetRequiredService<IAmbientAgency>();

                var ids = await LeaseIdsAsync(db, _opts.LeaseSeconds, _opts.BatchSize, stoppingToken, agencyId: null);
                if (ids.Count == 0) continue;

                var batch = await db.OutboxMessages
                    .Where(o => ids.Contains(o.Id))
                    .OrderBy(o => o.Id)
                    .ToListAsync(stoppingToken);

                foreach (var msg in batch)
                {
                    try
                    {
                        ambient.Current = msg.AgencyId;

                        await DispatchAsync(scope.ServiceProvider, msg, stoppingToken); 
                        msg.ProcessedUtc = DateTime.UtcNow;
                        msg.Error = null;
                        msg.LockedUntilUtc = null;
                    }
                    catch (DbUpdateException ex) when (IsDedupViolation(ex))
                    {
                        msg.ProcessedUtc = DateTime.UtcNow;
                        msg.Error = "Dedup: duplicate";
                        msg.LockedUntilUtc = null;
                    }
                    catch (Exception ex)
                    {
                        msg.Attempts++;
                        msg.Error = ex.Message;
                        msg.LockedUntilUtc = null;

                        if (msg.Attempts >= _opts.MaxAttempts)
                            msg.DeadLetteredUtc = DateTime.UtcNow;
                    }
                    finally
                    {
                        ambient.Current = null;
                    }
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Outbox dispatch loop failed.");
            }
        }
    }

    private static async Task DispatchAsync(IServiceProvider sp, OutboxMessage msg, CancellationToken ct)
    {
        var types = sp.GetRequiredService<IEventTypeRegistry>();
        if (!types.TryGet(msg.Type, out var clrType))
            return;

        var evt = (IIntegrationEvent?)JsonSerializer.Deserialize(msg.Payload, clrType, JsonOpts);
        if (evt is null) throw new InvalidOperationException($"Failed to deserialize {msg.Type}");

        var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(clrType);
        var handler = sp.GetRequiredService(handlerType);

        var method = handlerType.GetMethod("HandleAsync")!;
        await (Task)method.Invoke(handler, [evt, ct])!;
    }

    private static async Task<List<long>> LeaseIdsAsync(
    AppDbContext db,
    int leaseOption, 
    int batchOption,
    CancellationToken ct,
    long? agencyId = null)
    {
        const string sql = @"
            WITH cte AS (
              SELECT o.""Id""
              FROM public.outbox_messages o
              WHERE o.""ProcessedUtc"" IS NULL
                AND o.""DeadLetteredUtc"" IS NULL
                AND (o.""LockedUntilUtc"" IS NULL OR o.""LockedUntilUtc"" < now())
                AND (@aid IS NULL OR o.""AgencyId"" = @aid)
              ORDER BY o.""Id""
              LIMIT @batch
              FOR UPDATE SKIP LOCKED
            )
            UPDATE public.outbox_messages o
            SET ""LockedUntilUtc"" = now() + make_interval(secs => @leaseSecs)
            FROM cte
            WHERE o.""Id"" = cte.""Id""
            RETURNING o.""Id"" AS ""Value"";  -- IMPORTANT for scalar mapping
            ";

        var ids = await db.Database
            .SqlQueryRaw<long>(sql,
                new Npgsql.NpgsqlParameter<int>("leaseSecs", leaseOption),
                new Npgsql.NpgsqlParameter<int>("batch", batchOption),
                new Npgsql.NpgsqlParameter<long?>("aid", agencyId))
            .ToListAsync(ct);

        return ids;
    }


    private static bool IsDedupViolation(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("outbox_messages", StringComparison.OrdinalIgnoreCase) == true
           && ex.InnerException.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase);
}

