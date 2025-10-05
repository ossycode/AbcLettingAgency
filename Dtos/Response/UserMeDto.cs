namespace AbcLettingAgency.Dtos.Response;

public sealed class UserMeDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = default!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string[] Roles { get; init; } = Array.Empty<string>();
    public string[] Permissions { get; init; } = Array.Empty<string>();

    public bool IsPlatform { get; init; }
    public long? ActiveAgencyId { get; init; }
    public AgencyMembershipDto[] Memberships { get; init; } = Array.Empty<AgencyMembershipDto>();
}


public sealed class AgencyMembershipDto
{
    public long AgencyId { get; init; }
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string Role { get; init; } = default!;
}
