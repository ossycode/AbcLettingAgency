using System.Text.Json.Serialization;

namespace AbcLettingAgency.Features.Landlords;

public sealed class LandlordListDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public int PropertyCount { get; init; }
    public int TenancyCount { get; init; }
    public int InvoiceCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string? Address { get; init; }
}

public sealed class LandlordDto
{
    //[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required long Id { get; init; }
    public required string Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? BankIban { get; init; }
    public string? BankSort { get; init; }
    public string? Notes { get; init; }
    public int PropertyCount { get; init; }
    public int TenancyCount { get; init; }
    public int InvoiceCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}


