using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

// --- Client Money / Money Held ledger ---
public class ClientLedger : EntityBase
{
    public string? TenancyId { get; set; }
    public Tenancy? Tenancy { get; set; }

    public string? PropertyId { get; set; }
    public Property? Property { get; set; }

    public string? LandlordId { get; set; }
    public Landlord? Landlord { get; set; }

    public string? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public LedgerType EntryType { get; set; }
    [Column(TypeName = "decimal(12,2)")]
    public decimal Amount { get; set; } // +credit / -debit
    public string? Description { get; set; }
    public DateTime OccurredAt { get; set; }
}
