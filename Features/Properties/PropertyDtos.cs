using AbcLettingAgency.Enums;

namespace AbcLettingAgency.Features.Properties;

public sealed class PropertyDto
{
    public string Id { get; init; } = default!;
    public string? Code { get; init; }
    public string AddressLine1 { get; init; } = default!;
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? Postcode { get; init; }
    public int? Bedrooms { get; init; }
    public int? Bathrooms { get; init; }
    public bool? Furnished { get; init; }
    public DateTime? AvailableFrom { get; init; }
    public string LandlordId { get; init; } = default!;
    public string? LandlordName { get; init; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public sealed class CreatePropertyRequest
{
    public string? Code { get; init; } = "PROP";
    public string AddressLine1 { get; init; } = default!;
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? Postcode { get; init; }
    public int? Bedrooms { get; init; }
    public int? Bathrooms { get; init; }
    public bool? Furnished { get; init; }
    public DateTime? AvailableFrom { get; init; }
    public string LandlordId { get; init; } = default!;
}

public sealed class UpdatePropertyRequest
{
    public string AddressLine1 { get; init; } = default!;
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? Postcode { get; init; }
    public int? Bedrooms { get; init; }
    public int? Bathrooms { get; init; }
    public bool? Furnished { get; init; }
    public DateTime? AvailableFrom { get; init; }
    public string LandlordId { get; init; } = default!;
}