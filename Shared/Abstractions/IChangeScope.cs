namespace AbcLettingAgency.Shared.Abstractions;

public interface IChangeScope
{
    bool InBatch { get; }
    Task BatchAsync(Func<Task> action, CancellationToken token);

    Task BatchAsync(BatchMode mode, Func<Task> action, CancellationToken token);

    Task BatchAsync(Func<Task> action) => BatchAsync(BatchMode.DeferSave, action,  CancellationToken.None);

    Task<T> BatchAsync<T>(BatchMode mode, Func<Task<T>> action, CancellationToken token);

}


public enum BatchMode
{
    DeferSave,
    UseTransaction
}
