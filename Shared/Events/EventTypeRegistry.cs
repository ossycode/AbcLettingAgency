namespace AbcLettingAgency.Shared.Events;

public interface IEventTypeRegistry
{
    bool TryGet(string typeName, out Type eventType);
}

public sealed class EventTypeRegistry : IEventTypeRegistry
{
    private readonly Dictionary<string, Type> _map = new(StringComparer.Ordinal)
    {
        { nameof(RentChargeCreated), typeof(RentChargeCreated) },
        // add more here
    };

    public bool TryGet(string typeName, out Type eventType) => _map.TryGetValue(typeName, out eventType!);
}