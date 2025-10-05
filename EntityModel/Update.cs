using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AbcLettingAgency.EntityModel;

// --- Notes & Documents ---
public class Update : EntityBase, IAgencyOwned
{
    public string Body { get; set; } = default!;
    [MaxLength(200)]
    public string? CreatedBy { get; set; }
    public long AgencyId { get; set; }

    public long? PropertyId { get; set; }
    public Property? Property { get; set; }

    public long? TenancyId { get; set; }
    public Tenancy? Tenancy { get; set; }

    public long? LandlordId { get; set; }
    public Landlord? Landlord { get; set; }

    public long? TenantId { get; set; }
    public Tenant? Tenant { get; set; }
}
