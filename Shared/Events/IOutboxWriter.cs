namespace AbcLettingAgency.Shared.Events;

public interface IOutboxWriter
{
    Task AddAsync(IIntegrationEvent evt, string? dedupKey, CancellationToken ct);
}
