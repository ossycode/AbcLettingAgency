using AbcLettingAgency.Shared.Exceptions;

namespace AbcLettingAgency.Features.TenantManagement.RentCharges;

public static class RentChargeErrors
{
    public static AppError TenancyNotFound(long tenancyId) =>
        new("RentCharge.TenancyNotFound", $"Tenancy '{tenancyId}' not found.", ErrorType.NotFound, tenancyId.ToString());

    public static AppError TenancyNotActive(long tenancyId) =>
        new("RentCharge.TenancyNotActive", $"Tenancy '{tenancyId}' is not active.", ErrorType.Validation, tenancyId.ToString());

    public static AppError InvalidPeriod(DateTime start, DateTime end) =>
        new("RentCharge.InvalidPeriod", $"Period end ({end:yyyy-MM-dd}) is before start ({start:yyyy-MM-dd}).", ErrorType.Validation);

    public static AppError Overlap(DateTime start, DateTime end) =>
        new("RentCharge.Overlap", $"A charge already exists overlapping {start:yyyy-MM-dd}–{end:yyyy-MM-dd}.", ErrorType.Conflict);

    public static AppError InvalidAmount() =>
        new("RentCharge.InvalidAmount", "Amount must be greater than 0.", ErrorType.Validation);
}

