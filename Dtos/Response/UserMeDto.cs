namespace AbcLettingAgency.Dtos.Response;

public sealed class UserMeDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = default!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string[] Roles { get; init; } = Array.Empty<string>();
    public string[] Permissions { get; init; } = Array.Empty<string>();
}

