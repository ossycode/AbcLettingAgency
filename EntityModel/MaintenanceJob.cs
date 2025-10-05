using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

public class MaintenanceJob : EntityBase, IAgencyOwned
{
    public long PropertyId { get; set; } = default!;
    public Property Property { get; set; } = default!;

    public long AgencyId { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = default!;
    public string? Details { get; set; }
    public JobStatus Status { get; set; } = JobStatus.OPEN;
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    public decimal? Cost { get; set; }

    public long? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
}
