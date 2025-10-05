using AbcLettingAgency.Enums;

namespace AbcLettingAgency.Features.TenantManagement.RentCharges;

public class RentChargeCreateModel
{
    public long TenancyId { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? Amount { get; set; }
    public string? Notes { get; set; }
    public bool AdvanceNextChargeDate { get; set; }
}


public sealed class UpdateRentChargeRequest
{
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? Amount { get; set; }
    public ChargeStatus? Status { get; set; }
    public string? Notes { get; set; }
}