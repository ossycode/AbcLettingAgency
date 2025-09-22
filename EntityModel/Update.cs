using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AbcLettingAgency.EntityModel;

// --- Notes & Documents ---
public class Update : EntityBase
{
    public string Body { get; set; } = default!;
    [MaxLength(200)]
    public string? CreatedBy { get; set; }

    // Polymorphic via nullable FKs
    public string? PropertyId { get; set; }
    public Property? Property { get; set; }

    public string? TenancyId { get; set; }
    public Tenancy? Tenancy { get; set; }

    public string? LandlordId { get; set; }
    public Landlord? Landlord { get; set; }

    public string? TenantId { get; set; }
    public Tenant? Tenant { get; set; }
}
