using AbcLettingAgency.Enums;

namespace AbcLettingAgency.Features.Properties;

public sealed class PropertyDto
{
    public long Id { get; init; } = default!;
    public string Code { get; init; } = string.Empty;
    public string AddressLine1 { get; init; } = default!;
    public string? AddressLine2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string Postcode { get; init; } = string.Empty;
    public int Bedrooms { get; init; }
    public int Bathrooms { get; init; }
    public bool? Furnished { get; init; }
    public DateTime? AvailableFrom { get; init; }
    public long LandlordId { get; init; } = default!;
    public string? LandlordName { get; init; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Notes { get; set; }

}

public sealed class CreatePropertyRequest
{
    public string AddressLine1 { get; init; } = default!;
    public string? AddressLine2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string Postcode { get; init; } = string.Empty;
    public int Bedrooms { get; init; }
    public int Bathrooms { get; init; }
    public bool? Furnished { get; init; }
    public DateTime? AvailableFrom { get; init; }
    public long LandlordId { get; init; } = default!;
    public string? Notes { get; set; }

}

public sealed class UpdatePropertyRequest
{
    public string AddressLine1 { get; init; } = default!;
    public string? AddressLine2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string Postcode { get; init; } = string.Empty;
    public int Bedrooms { get; init; }
    public int Bathrooms { get; init; }
    public bool? Furnished { get; init; }
    public DateTime? AvailableFrom { get; init; }
    public long LandlordId { get; init; } = default!;
    public string? Notes { get; set; }

}