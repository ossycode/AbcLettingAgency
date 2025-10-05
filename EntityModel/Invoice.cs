using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

public class Invoice : EntityBase, IAgencyOwned
{
    public long? PropertyId { get; set; }
    public Property? Property { get; set; }

    public long AgencyId { get; set; }
    public long? TenancyId { get; set; }
    public Tenancy? Tenancy { get; set; }

    [MaxLength(200)]
    public string VendorName { get; set; } = default!;
    [MaxLength(120)]
    public string? Reference { get; set; }
    public string? Description { get; set; }

    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }

    public decimal NetAmount { get; set; }
    public decimal VatAmount { get; set; }
    public decimal GrossAmount { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.OPEN;
    public DateTime? PaidAt { get; set; }

    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
