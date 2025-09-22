using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

// --- Rents: charges and receipts ---
public class RentCharge : EntityBase
{
    public string TenancyId { get; set; } = default!;
    public Tenancy Tenancy { get; set; } = default!;

    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal? CommissionDue { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal? AmountAfterCommission { get; set; }

    public ChargeStatus Status { get; set; } = ChargeStatus.OPEN;

    public List<RentReceipt> Receipts { get; set; } = new();
}
