using AbcLettingAgency.Enums;

namespace AbcLettingAgency.Features.TenantManagement.Tenants;


public sealed class TenantDto
{
    public long Id { get; set; } = default!; 
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? SecondEmail { get; set; }
    public string Phone { get; set; } = default!;
    public string? SecondPhone { get; set; }
    public TenantStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public int ActiveTenancyCount { get; set; }
    public int PastTenancyCount { get; set; }
}


public sealed class CreateTenantRequest
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? SecondEmail { get; set; }
    public string? SecondPhone { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateTenantRequest
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string? SecondEmail { get; set; }
    public string? SecondPhone { get; set; }
    public TenantStatus Status { get; set; }
    public string? Notes { get; set; }
}

