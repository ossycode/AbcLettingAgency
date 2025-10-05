using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

public class RentReceipt : EntityBase, IAgencyOwned
{
    public long TenancyId { get; set; } = default!;
    public Tenancy Tenancy { get; set; } = default!;
    public long AgencyId { get; set; }

    public DateTime ReceivedAt { get; set; }
    public decimal Amount { get; set; }

    [MaxLength(60)]
    public string? Method { get; set; } // cash/bank/SO ref
    [MaxLength(120)]
    public string? Reference { get; set; }
    public string? Notes { get; set; }

    public long? ChargeId { get; set; }
    public RentCharge? Charge { get; set; }
}
