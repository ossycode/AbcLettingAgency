using AbcLettingAgency.Enums;
using AbcLettingAgency.Features.TenantManagement.Tenants;
using System.Text.Json.Serialization;

namespace AbcLettingAgency.Features.TenantManagement.Tenancies;


public class TenancyDto
{
    public long Id { get; init; }

    public long PropertyId { get; init; }

    public long LandlordId { get; init; }

    public TenancyStatus Status { get; init; }

    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int RentDueDay { get; init; }
    public RentFrequency Frequency { get; init; }
    public decimal RentAmount { get; init; }
    public decimal? CommissionPercent { get; init; }
    public decimal? DepositAmount { get; init; }
    public string? DepositLocation { get; init; }
    public DateTime? NextChargeDate { get; init; }
    public string? Notes { get; init; }

    public IEnumerable<OccupantDto> Occupants { get; init; } = [];

    public decimal Balance { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}


public  class OccupantDto
{
    public long TenantId { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string? Email { get; set; } 
    public string? Phone { get; set; }
    public bool IsPrimary { get; set; }
    public decimal? ResponsibilitySharePercent { get; set; }
    public DateTime? OccupancyStart { get; set; }
    public DateTime? OccupancyEnd { get; set; }
}

public class CreateTenancyRequest
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long PropertyId { get; init; }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public long LandlordId { get; init; }

    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int RentDueDay { get; init; }
    public RentFrequency Frequency { get; init; }
    public decimal RentAmount { get; init; }
    public decimal? CommissionPercent { get; init; }
    public decimal? DepositAmount { get; init; }
    public string? DepositLocation { get; init; }
    public string? Notes { get; init; }
    public TenancyStatus Status { get; init; }
    public List<CreateOccupant> Occupants { get; init; } = new();
}

public class UpdateTenancyRequest
{
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int RentDueDay { get; init; }
    public RentFrequency Frequency { get; init; }
    public decimal RentAmount { get; init; }
    public decimal? CommissionPercent { get; init; }
    public decimal? DepositAmount { get; init; }
    public string? DepositLocation { get; init; }
    public TenancyStatus Status { get; init; }
    public string? Notes { get; init; }

    public List<UpdateOccupant> Occupants { get; init; } = new();
}

public  class CreateOccupant

{
    public long TenantId { get; set; } = default!;
    public bool IsPrimary { get; set; } = false;
    public decimal? ResponsibilitySharePercent { get; set; }
    public DateTime? OccupancyStart { get; set; }
    public DateTime? OccupancyEnd { get; set; }
}

public class UpdateBillingScheduleRequest
{
    public int RentDueDay { get; init; }
    public RentFrequency Frequency { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EffectiveFrom {  get; init; }
    public bool RebuildFutureCharges { get; init; }
}

public class UpdateOccupant
{
    public long TenantId { get; set; } = default!;
    public bool IsPrimary { get; set; }
    public decimal? ResponsibilitySharePercent { get; set; }
    public DateTime? OccupancyStart { get; set; }
    public DateTime? OccupancyEnd { get; set; }
}


public class UpdateTenancyStatusRequest
{
    public TenancyStatus Status { get; set; }
    public string? Note { get; set; }
}


public class SetNextChargeDateRequest
{
    public DateTime? NextChargeDate { get; set; }
}


public class StartTenancyRequest
{
    public long PropertyId { get; set; }
    public long LandlordId { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int RentDueDay { get; set; } 
    public RentFrequency Frequency { get; set; }
    public decimal RentAmount { get; set; }
    public decimal? CommissionPercent { get; set; }
    public decimal? DepositAmount { get; set; }
    public string? DepositLocation { get; set; }
    public string? Notes { get; set; }

    public List<OccupantInput> Occupants { get; set; } = new();
    public bool SeedFirstCharge { get; set; } = true;
}

public sealed class OccupantInput
{
    public long? TenantId { get; set; }            
    public CreateTenantRequest? NewTenant { get; set; }
    public bool IsPrimary { get; set; }
    public decimal? ResponsibilitySharePercent { get; set; }
    public DateTime? OccupancyStart { get; set; }
    public DateTime? OccupancyEnd { get; set; }
}