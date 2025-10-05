using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.TenantManagement.Tenancies.Errors;

public static class TenancyErrors
{
    public static AppError NotFound(long? id = null)
        => new(
            code: "Tenancy.NotFound",
            message: "Tenancy not found.",
            type: ErrorType.NotFound,
            key: id?.ToString() ?? string.Empty
        );

    public static AppError PropertyNotFound(long? id = null)
    => new(
        code: "Tenancy:Property.NotFound",
        message: "Property not found.",
        type: ErrorType.NotFound,
        key: id?.ToString() ?? string.Empty
    );

    public static AppError PropertyNotOwnedByLandlord(long propertyId, long landlordId)
        => new(
            code: "Tenancy.PropertyNotOwnedByLandlord",
            message: $"Property '{propertyId}' is not owned by selected landlord '{landlordId}'.",
            type: ErrorType.Validation,
            key: propertyId.ToString()
        );

    public static AppError TenantNotFound(long[]? tenantId)
        => new(
            code: "Tenancy.TenantNotFound",
            message: $"Tenants '{tenantId}' was not found.",
            type: ErrorType.NotFound,
            key: tenantId?.ToString()
        );
    public static AppError InvalidTenants()
        => new(
            code: "Tenancy.InvalidTenants",
            message: $"\"Occupants\", \"Each occupant must have TenantId or NewTenant.\"",
            type: ErrorType.NotFound,
            key: "InvalidTenants"
        );
    public static AppError DuplicateOccupants()
        => new(
            code: "Tenancy.DuplicateOccupants",
            message: $"Duplicate Occupants",
            type: ErrorType.Conflict,
            key: "DuplicateOccupants"
        );

    public static AppError InvalidRentDueDay()
    => new(
        code: "Tenancy.InvalidRentDueDay",
        message: $"Invalid Rent Due Day",
        type: ErrorType.Conflict,
        key: "InvalidRentDueDay"
    );
    public static AppError InvalidNextChargeDate(string? msg)
    => new(
        code: "Tenancy.InvalidNextChargeDate",
        message: $"InvalidNextChargeDate ${msg}",
        type: ErrorType.Conflict,
        key: "InvalidNextChargeDate"
    );
    public static AppError InvalidStatusForScheduleChange(TenancyStatus status)
    => new(
        code: "Tenancy.InvalidStatusForScheduleChange",
        message: $" InvalidStatusForScheduleChange ${status}",
        type: ErrorType.Conflict,
        key: status.ToString()
    );

    public static AppError InvalidPrimaryOccupants()
        => new(
            code: "Tenancy.InvalidPrimaryOccupants",
            message: "Exactly one primary occupant is required.",
            type: ErrorType.Validation,
            key: "occupants"
        );

    public static AppError InvalidShareDistribution()
        => new(
            code: "Tenancy.InvalidShareDistribution",
            message: "Responsibility shares must total 100%.",
            type: ErrorType.Validation,
            key: "shares"
        );

    public static AppError HasLinkedEntities()
        => new(
            code: "Tenancy.HasLinkedEntities",
            message: "Cannot delete a tenancy with linked rent receipts, ledger entries, invoices, or documents.",
            type: ErrorType.Conflict,
            key: "tenancy"
        );

    public static AppError Validation(IEnumerable<KeyValuePair<string, string>> errors)
        => new(
            code: "Tenancy.Validation",
            message: "One or more validation errors occurred.",
            type: ErrorType.Validation
        )
        {
            Details = errors.ToList()
        };
}
