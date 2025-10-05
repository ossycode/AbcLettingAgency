namespace AbcLettingAgency.Shared.Events;

public class RentChargeCreated : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredUtc { get; } = DateTime.UtcNow;
    public long RentChargeId { get; init; }
    public long TenancyId { get; init; }
    public DateTime PeriodStart { get; init; }
}
