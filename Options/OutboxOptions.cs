namespace AbcLettingAgency.Options;

public class OutboxOptions
{
    public int PollIntervalSeconds { get; set; } = 120;
    public int BatchSize { get; set; } = 100;
    public int LeaseSeconds { get; set; } = 60;
    public int MaxAttempts { get; set; } = 10;
    public int Timer { get; set; } = 120;
}
