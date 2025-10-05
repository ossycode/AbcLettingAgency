using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.TenantManagement.Tenancies.Errors;

public static class TenancyTenantErrors
{
    public static AppError TenancyNotFound(long id) =>
        new("TenancyTenant.TenancyNotFound", $"Tenancy '{id}' not found.", ErrorType.NotFound, id.ToString());

    public static AppError TenantNotFound(long id) =>
        new("TenancyTenant.TenantNotFound", $"Tenant '{id}' not found.", ErrorType.NotFound, id.ToString());

    public static AppError DuplicateLink(long tenancyId, long tenantId) =>
        new("TenancyTenant.Duplicate", $"Tenant '{tenantId}' is already an occupant of tenancy '{tenancyId}'.", ErrorType.Conflict);

    public static AppError PrimaryExists(long tenancyId) =>
        new("TenancyTenant.PrimaryExists", $"Tenancy '{tenancyId}' already has a primary occupant.", ErrorType.Validation, "IsPrimary");

    public static AppError InvalidShare() =>
        new("TenancyTenant.InvalidShare", "ResponsibilitySharePercent must be between 0 and 100.", ErrorType.Validation, "ResponsibilitySharePercent");

    public static AppError ShareWouldExceed100() =>
        new("TenancyTenant.ShareSumTooHigh", "Total responsibility share would exceed 100%.", ErrorType.Validation, "ResponsibilitySharePercent");

    public static AppError InvalidDates() =>
        new("TenancyTenant.InvalidDates", "OccupancyEnd cannot be before OccupancyStart.", ErrorType.Validation, "OccupancyEnd");
}