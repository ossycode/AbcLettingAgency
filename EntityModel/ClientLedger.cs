using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

public class ClientLedger : EntityBase, IAgencyOwned
{
    public long? TenancyId { get; set; }
    public Tenancy? Tenancy { get; set; }

    public long? PropertyId { get; set; }
    public Property? Property { get; set; }

    public long? LandlordId { get; set; }
    public Landlord? Landlord { get; set; }

    public long? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public LedgerType EntryType { get; set; }
    public decimal Amount { get; set; } // +credit / -debit
    public string? Description { get; set; }
    public DateTime OccurredAt { get; set; }
    public long AgencyId { get; set; }
}
