namespace AbcLettingAgency.Dtos.Response;

public sealed class LoginResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = default!;
    public string DisplayName { get; init; } = default!;
    public bool IsPlatform { get; init; }
    public long? SelectedAgencyId { get; init; }
    public int AgencyCount { get; init; }
    public bool NeedsAgencySelection { get; init; }
    public AgencySummary[] Agencies { get; init; } = Array.Empty<AgencySummary>();
    public string[] Roles { get; init; } = Array.Empty<string>();
    public string[] Permissions { get; init; } = Array.Empty<string>();
}

public sealed class AgencySummary
{
    public long Id { get; init; }
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
}
