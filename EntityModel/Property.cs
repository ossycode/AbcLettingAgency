using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AbcLettingAgency.EntityModel;

public class Property : EntityBase
{
    [MaxLength(50)]
    public string? Code { get; set; } // unique friendly code

    [MaxLength(200)]
    public string AddressLine1 { get; set; } = default!;
    [MaxLength(200)]
    public string? AddressLine2 { get; set; }
    [MaxLength(120)]
    public string? City { get; set; }
    [MaxLength(20)]
    public string? Postcode { get; set; }

    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public bool? Furnished { get; set; }
    public DateTime? AvailableFrom { get; set; }

    // FK
    public string LandlordId { get; set; } = default!;
    public Landlord Landlord { get; set; } = default!;

    public List<Tenancy> Tenancies { get; set; } = new();
    public List<MaintenanceJob> Maintenance { get; set; } = new();
    public List<Invoice> Invoices { get; set; } = new();
    public List<Update> Updates { get; set; } = new();
    public List<Document> Documents { get; set; } = new();
}
