using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel;

public class Tenant : EntityBase, IAgencyOwned
{
    [MaxLength(120)]
    public string FirstName { get; set; } = default!;
    [MaxLength(120)]
    public string LastName { get; set; } = default!;
    public long AgencyId { get; set; }


    [EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [EmailAddress, MaxLength(256)]
    public string? SecondEmail { get; set; }

    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? SecondPhone { get; set; }
    public TenantStatus Status { get; set; } = TenantStatus.ACTIVE;

    [MaxLength(4000)]
    public string? Notes { get; set; }
    public ICollection<Document> Documents { get; set; } = [];
    public ICollection<Update> Updates { get; set; } = [];
    public bool IsDeleted { get; set; }
    [NotMapped] public string FullName => $"{FirstName} {LastName}".Trim();

    public ICollection<TenancyTenant> TenancyLinks { get; set; } = [];
}
