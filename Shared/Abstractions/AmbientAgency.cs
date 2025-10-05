namespace AbcLettingAgency.Shared.Abstractions;

public interface IAmbientAgency
{
    long? Current { get; set; }
}

public sealed class AmbientAgency : IAmbientAgency
{
    private static readonly AsyncLocal<long?> _holder = new();
    public long? Current {
        get => _holder.Value;
        set => _holder.Value = value;
    }
}
