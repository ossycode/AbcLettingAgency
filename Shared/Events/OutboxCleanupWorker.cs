using AbcLettingAgency.Data;
using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Shared.Events;

public sealed class OutboxCleanupWorker(IServiceProvider sp, ILogger<OutboxCleanupWorker> log)
 : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);
    private const int RetainDaysProcessed = 14;
    private const int RetainDaysDead = 30;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(Interval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var now = DateTime.UtcNow;

                var cutProcessed = now.AddDays(-RetainDaysProcessed);
                var cutDead = now.AddDays(-RetainDaysDead);

                var removedProcessed = await db.OutboxMessages
                    .Where(x => x.ProcessedUtc != null && x.ProcessedUtc < cutProcessed)
                    .ExecuteDeleteAsync(stoppingToken);

                var removedDead = await db.OutboxMessages
                    .Where(x => x.DeadLetteredUtc != null && x.DeadLetteredUtc < cutDead)
                    .ExecuteDeleteAsync(stoppingToken);

                log.LogInformation("Outbox cleanup removed {Processed} processed, {Dead} dead-lettered", removedProcessed, removedDead);
            }
            catch (Exception ex)
            {
                // Don't crash the service because cleanup failed once
                log.LogError(ex, "Outbox cleanup failed.");
            }
        }
    }
}
