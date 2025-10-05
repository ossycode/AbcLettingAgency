using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AbcLettingAgency.EntityModel;

public class Property : EntityBase, IAgencyOwned
{
    [MaxLength(50)]
    public string Code { get; set; }  = string.Empty;

    public long AgencyId { get; set; }

    [MaxLength(200)]
    public string AddressLine1 { get; set; } = default!;
    [MaxLength(200)]
    public string? AddressLine2 { get; set; }
    [MaxLength(120)]
    public string City { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Postcode { get; set; } = string.Empty;

    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public bool? Furnished { get; set; }
    public DateTime? AvailableFrom { get; set; }

    public string? Notes { get; set; }

    public long LandlordId { get; set; } = default!;
    public Landlord Landlord { get; set; } = default!;

    public List<Tenancy> Tenancies { get; set; } = new();
    public List<MaintenanceJob> Maintenance { get; set; } = new();
    public List<Invoice> Invoices { get; set; } = new();
    public List<Update> Updates { get; set; } = new();
    public List<Document> Documents { get; set; } = new();
}
