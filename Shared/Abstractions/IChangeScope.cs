namespace AbcLettingAgency.Shared.Abstractions;

public interface IChangeScope
{
    Task BatchAsync(Func<Task> action, CancellationToken ct = default);
}
