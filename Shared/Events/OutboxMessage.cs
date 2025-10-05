using Microsoft.EntityFrameworkCore;

namespace AbcLettingAgency.Shared.Events;

public sealed class OutboxMessage
{
    public long Id { get; set; }
    public string Type { get; set; } = default!;
    public string? DedupKey { get; set; } 
    public string Payload { get; set; } = default!;
    public DateTime OccurredUtc { get; set; }

    public long? AgencyId { get; set; }
    public DateTime? LockedUntilUtc { get; set; }
    public DateTime? ProcessedUtc { get; set; }
    public DateTime? DeadLetteredUtc { get; set; }
    public int Attempts { get; set; }
    public string? Error { get; set; }
}

//[Keyless]
//public sealed class OutboxLeaseRow
//{
//    public long Id { get; set; }
//}