namespace AbcLettingAgency.Features.Properties;

public sealed record PropertyFilters(
    string? Q = null,
    string? LandlordId = null,
    string? City = null,
    bool? Furnished = null,
    int? BedroomsMin = null,
    int? BedroomsMax = null,
    bool? AvailableOnly = null);
