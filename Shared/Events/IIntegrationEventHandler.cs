namespace AbcLettingAgency.Shared.Events;

public interface IIntegrationEvent
{
    Guid Id { get; }   
    DateTime OccurredUtc { get; }
}

public interface IIntegrationEventHandler<in T> where T : IIntegrationEvent
{
    Task HandleAsync(T message, CancellationToken ct);
}

