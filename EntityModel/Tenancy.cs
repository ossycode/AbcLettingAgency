using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

public class Tenancy : EntityBase, IAgencyOwned
{
    public long PropertyId { get; set; }
    public Property Property { get; set; } = default!;
    public long AgencyId { get; set; }

    public long LandlordId { get; set; }
    public Landlord Landlord { get; set; } = default!;

    public TenancyStatus Status { get; set; } = TenancyStatus.ACTIVE;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int RentDueDay { get; set; }
    public RentFrequency Frequency { get; set; } = RentFrequency.Monthly;

    public DateTime? CheckInDate { get; set; }
    public DateTime? ManagementStart { get; set; }
    public DateTime? RenewalDueOn { get; set; }
    public DateTime? NextChargeDate { get; set; }
    public decimal RentAmount { get; set; }

    public decimal? CommissionPercent { get; set; }
    public decimal? ManagementFeePercent { get; set; }

    public decimal? DepositAmount { get; set; }

    [MaxLength(200)]
    public string? DepositLocation { get; set; }
    public string? Notes { get; set; }

    public ICollection<RentCharge> Charges { get; set; } = [];
    public ICollection<RentReceipt> Receipts { get; set; } = [];
    public ICollection<ClientLedger> Ledger { get; set; } = [];
    public ICollection<Update> Updates { get; set; } = [];
    public ICollection<Document> Documents { get; set; } = [];

    public ICollection<TenancyTenant> Occupants { get; set; } = [];
    public bool IsDeleted { get; set; }
}
