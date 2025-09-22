using AbcLettingAgency.Enums;

namespace AbcLettingAgency.Features.Properties;

public sealed record PropertyListItemDto(
    string Id,
    string? Code,
    string AddressLine1,
    string? City,
    string? Postcode,
    int? Bedrooms,
    int? Bathrooms,
    bool? Furnished,
    DateTime? AvailableFrom,
    string LandlordId,
    string LandlordName,
    bool IsAvailable,
    int ActiveTenancies,
    int OpenMaintenanceCount);

public sealed record PropertyDetailDto(
    string Id,
    string? Code,
    string AddressLine1,
    string? AddressLine2,
    string? City,
    string? Postcode,
    int? Bedrooms,
    int? Bathrooms,
    bool? Furnished,
    DateTime? AvailableFrom,
    string LandlordId,
    string LandlordName,
    bool IsAvailable,
    IReadOnlyList<PropertyTenancyDto> Tenancies,
    IReadOnlyList<PropertyMaintenanceDto> Maintenance);

public sealed record PropertyTenancyDto(
    string Id,
    string TenantId,
    string TenantName,
    TenancyStatus Status,
    DateTime? StartDate,
    DateTime? EndDate,
    decimal RentAmount);

public sealed record PropertyMaintenanceDto(
    string Id,
    string Title,
    string? Details,
    int Status,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    decimal? Cost,
    string? InvoiceId);

public sealed record CreatePropertyDto(
    string? Code,
    string AddressLine1,
    string? AddressLine2,
    string? City,
    string? Postcode,
    int? Bedrooms,
    int? Bathrooms,
    bool? Furnished,
    DateTime? AvailableFrom,
    string LandlordId);

public sealed record UpdatePropertyDto(
    string? Code,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Postcode,
    int? Bedrooms,
    int? Bathrooms,
    bool? Furnished,
    DateTime? AvailableFrom,
    string? LandlordId);

public sealed record CreateMaintenanceDto(
    string Title,
    string? Details,
    int? Status,
    DateTime? OpenedAt,
    DateTime? ClosedAt,
    decimal? Cost,
    string? InvoiceId);

public sealed record CreateTenancyDto(
    string TenantId,
    TenancyStatus Status,
    DateTime StartDate,
    DateTime? EndDate,
    decimal RentAmount,
    int? RentDueDay,
    decimal? CommissionPercent,
    decimal? DepositAmount,
    string? Notes);