using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AbcLettingAgency.EntityModel;

public class Tenant : EntityBase
{
    [MaxLength(120)]
    public string FirstName { get; set; } = default!;
    [MaxLength(120)]
    public string LastName { get; set; } = default!;
    [EmailAddress, MaxLength(256)]
    public string? Email { get; set; }
    [MaxLength(50)]
    public string? Phone { get; set; }
    public TenantStatus Status { get; set; } = TenantStatus.ACTIVE;

    public List<Document> Documents { get; set; } = new();
    public List<Tenancy> Tenancies { get; set; } = new();
    public List<Update> Updates { get; set; } = new();
}
