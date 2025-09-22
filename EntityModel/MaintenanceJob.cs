using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

// --- Maintenance ---
public class MaintenanceJob : EntityBase
{
    public string PropertyId { get; set; } = default!;
    public Property Property { get; set; } = default!;

    [MaxLength(200)]
    public string Title { get; set; } = default!;
    public string? Details { get; set; }
    public JobStatus Status { get; set; } = JobStatus.OPEN;
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    [Column(TypeName = "decimal(12,2)")]
    public decimal? Cost { get; set; }

    public string? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
}
