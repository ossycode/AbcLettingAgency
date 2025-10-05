using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AbcLettingAgency.EntityModel;

public class Document : EntityBase, IAgencyOwned
{
    [MaxLength(600)]
    public string Url { get; set; } = default!;
    [MaxLength(200)]
    public string Name { get; set; } = default!;
    [MaxLength(150)]
    public string? MimeType { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public long? TenancyId { get; set; }
    public Tenancy? Tenancy { get; set; }

    public long? TenantId { get; set; }
    public Tenant? Tenant { get; set; }

    public long? PropertyId { get; set; }
    public Property? Property { get; set; }

    public long? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public long AgencyId { get; set; }
}
