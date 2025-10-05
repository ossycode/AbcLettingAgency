using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Features.TenantManagement.RentCharges;
using AbcLettingAgency.Features.TenantManagement.Tenancies;
using AbcLettingAgency.Features.TenantManagement.Tenants;

namespace AbcLettingAgency.Features.TenantManagement;

public static class Mappers
{
    public static TenantDto TenantToDto(Tenant e) => new TenantDto
    {
        Id = e.Id, 
        FirstName = e.FirstName,
        LastName = e.LastName,
        Email = e.Email,
        SecondEmail = e.SecondEmail,
        Phone = e.Phone,
        SecondPhone = e.SecondPhone,
        Status = e.Status,
        Notes = e.Notes,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public static OccupantDto OccupantToDto(TenancyTenant link) => new OccupantDto
    {
        TenantId = link.TenantId,
        FullName = link.Tenant is null ? null : $"{link.Tenant.FirstName} {link.Tenant.LastName}".Trim(),
        Email = link.Tenant?.Email,
        Phone = link.Tenant?.Phone,
        IsPrimary = link.IsPrimary,
        ResponsibilitySharePercent = link.ResponsibilitySharePercent,
        OccupancyStart = link.OccupancyStart,
        OccupancyEnd = link.OccupancyEnd
    };

    /// <summary>
    /// Map Tenancy -> TenancyDto. Optionally pass a precomputed balance.
    /// Ensure the query includes Occupants.ThenInclude(o => o.Tenant) if you want FullName/Email/Phone populated.
    /// </summary>
    public static TenancyDto TenancyToDto(Tenancy e, decimal? balance = null) => new TenancyDto
    {
        Id = e.Id,
        PropertyId = e.PropertyId,
        LandlordId = e.LandlordId,

        Status = e.Status,
        StartDate = e.StartDate,
        EndDate = e.EndDate,

        RentDueDay = e.RentDueDay,
        Frequency = e.Frequency,

        RentAmount = e.RentAmount,
        CommissionPercent = e.CommissionPercent,

        DepositAmount = e.DepositAmount,
        DepositLocation = e.DepositLocation,

        NextChargeDate = e.NextChargeDate,
        Notes = e.Notes,

        Occupants = e.Occupants?.Select(OccupantToDto).ToList() ?? new List<OccupantDto>(),

        Balance = balance ?? 0m,

        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public static RentChargeDto RentChargeToDto(RentCharge rc)
    {
        var paid = rc.Receipts?.Sum(r => r.Amount) ?? 0m;
        var outstanding = Math.Max(0m, rc.Amount - paid);

        return new RentChargeDto
        {
            Id = rc.Id,
            TenancyId = rc.TenancyId,

            PeriodStart = rc.PeriodStart,
            PeriodEnd = rc.PeriodEnd,
            DueDate = rc.DueDate,

            Amount = rc.Amount,
            CommissionDue = rc.CommissionDue,
            AmountAfterCommission = rc.AmountAfterCommission,
            Status = rc.Status,

            Paid = paid,
            Outstanding = outstanding,

            Notes = rc.Notes,

            CreatedAt = rc.CreatedAt,
            UpdatedAt = rc.UpdatedAt
        };
    }
}
