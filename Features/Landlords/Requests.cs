namespace AbcLettingAgency.Features.Landlords;

public sealed class CreateLandlordRequest
{
    public required string Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? BankIban { get; init; }
    public string? BankSort { get; init; }
    public string? Notes { get; init; }
}

public sealed class UpdateLandlordRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? BankIban { get; init; }
    public string? BankSort { get; init; }
    public string? Notes { get; init; }
}
