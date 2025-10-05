using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

public class RentCharge : EntityBase, IAgencyOwned
{
    public long TenancyId { get; set; } = default!;
    public Tenancy Tenancy { get; set; } = default!;
    public long AgencyId { get; set; }

    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }

    public decimal? CommissionDue { get; set; }

    public decimal? AmountAfterCommission { get; set; }

    public ChargeStatus Status { get; set; } = ChargeStatus.OPEN;

    public List<RentReceipt> Receipts { get; set; } = new();

    [MaxLength(4000)] 
    public string? Notes { get; set; }
}
