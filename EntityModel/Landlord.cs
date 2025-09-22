using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AbcLettingAgency.EntityModel;

public class Landlord : EntityBase
{
    [MaxLength(200)]
    public string Name { get; set; } = default!;
    [EmailAddress, MaxLength(256)]
    public string? Email { get; set; }
    [MaxLength(50)]
    public string? Phone { get; set; }
    [MaxLength(400)]
    public string? Address { get; set; }
    [MaxLength(34)]
    public string? BankIban { get; set; }
    [MaxLength(20)]
    public string? BankSort { get; set; }
    public string? Notes { get; set; }

    public List<Property> Properties { get; set; } = new();
    public List<Tenancy> Tenancies { get; set; } = new();
    public List<Invoice> Invoices { get; set; } = new();
    public List<Update> Updates { get; set; } = new(); 
}
