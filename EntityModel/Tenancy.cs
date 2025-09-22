using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

public class Tenancy : EntityBase
{
    public string PropertyId { get; set; } = default!;
    public Property Property { get; set; } = default!;

    public string LandlordId { get; set; } = default!;
    public Landlord Landlord { get; set; } = default!;

    public string TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = default!;

    public TenancyStatus Status { get; set; } = TenancyStatus.ACTIVE;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? ManagementStart { get; set; }
    public DateTime? TaRenewalDate { get; set; }

    // Money
    [Column(TypeName = "decimal(12,2)")]
    public decimal RentAmount { get; set; }
    public int? RentDueDay { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? CommissionPercent { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? CommissionPercentTo15 { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal? DepositAmount { get; set; }

    [MaxLength(200)]
    public string? DepositLocation { get; set; }
    public string? Notes { get; set; }

    public List<RentCharge> Charges { get; set; } = new();
    public List<RentReceipt> Receipts { get; set; } = new();
    public List<ClientLedger> Ledger { get; set; } = new();
    public List<Update> Updates { get; set; } = new();
    public List<Document> Documents { get; set; } = new();
}
