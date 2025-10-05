using AbcLettingAgency.Data;
using System.Text.Json;

namespace AbcLettingAgency.Shared.Events;

public sealed class EfOutboxWriter(AppDbContext db) : IOutboxWriter
{
    public Task AddAsync(IIntegrationEvent evt, string? dedupKey, CancellationToken ct)
    {
        var msg = new OutboxMessage
        {
            Type = evt.GetType().Name,
            DedupKey = dedupKey,
            AgencyId = db.CurrentAgencyId,
            Payload = JsonSerializer.Serialize(evt, evt.GetType(), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
            OccurredUtc = DateTime.UtcNow
        };
        db.OutboxMessages.Add(msg);

        return Task.CompletedTask; 
    }


}