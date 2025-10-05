using AbcLettingAgency.Enums;

namespace AbcLettingAgency.Features.TenantManagement.RentCharges;


public sealed class RentChargeDto
{
    public long Id { get; set; } 
    public long TenancyId { get; set; } 
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }
    public decimal? CommissionDue { get; set; }
    public decimal? AmountAfterCommission { get; set; }
    public ChargeStatus Status { get; set; }

    public string? Notes { get; set; }

    // Aggregates for UI
    public decimal Paid { get; set; }
    public decimal Outstanding { get; set; }
    public bool IsOverdue { get; set; }

    // Audit-ish (optional)
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}