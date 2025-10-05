using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

public class TenancyTenant : EntityBase, IAgencyOwned
{
    public long TenancyId { get; set; }
    public Tenancy Tenancy { get; set; } = default!;

    public long AgencyId { get; set; }

    public long TenantId { get; set; }
    public Tenant Tenant { get; set; } = default!;

    public bool IsPrimary { get; set; }       
    public decimal? ResponsibilitySharePercent { get; set; } 

    public DateTime? OccupancyStart { get; set; }
    public DateTime? OccupancyEnd { get; set; }
}
