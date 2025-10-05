using AbcLettingAgency.Enums;
using AbcLettingAgency.Shared.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcLettingAgency.EntityModel.Agencies;

public class Agency : EntityBase, ISoftDelete
{
    public long? OrgId { get; set; }             
    public long? ParentAgencyId { get; set; }   

    public string Slug { get; set; } = default!; 
    public string Name { get; set; } = default!;
    public string? LegalName { get; set; }
    public AgencyStatus Status { get; set; } 

    // Contact
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string? PhoneNumber2 { get; set; }
    public string? Website { get; set; }

    // Owned value object
    public Address Address { get; set; } = default!;

    // Locale / finance basics
    public string TimeZone { get; set; } = "Europe/London";
    public string Currency { get; set; } = "GBP";
    public string? CompanyNumber { get; set; }
    public string? VatNumber { get; set; }

    // Billing (optional)
    public string? BillingPlan { get; set; }

    // Ownership / soft delete
    public Guid? OwnerUserId { get; set; }

    // Navigation
    public Agency? ParentAgency { get; set; }
    public List<Agency> Children { get; set; } = new();
    public List<AgencyGroupMembership> GroupMemberships { get; set; } = new();
    public bool IsDeleted { get; set; }
    public List<AgencyUser> Users { get; set; } = new();
    public AgencyConfiguration? Configuration { get; set; }
}

public class Address 
{
    public string Line1 { get; private set; } = default!;
    public string? Line2 { get; private set; }
    public string? Line3 { get; private set; }
    public string City { get; private set; } = default!;
    public string? Region { get; private set; }       // County/State/Region
    public string PostCode { get; private set; } = default!;
    public string CountryCode { get; private set; } = "GB"; // ISO-3166-1 alpha-2

    private Address() { } 

    public Address(string line1, string city, string postCode,
                   string? line2 = null, string? line3 = null, string? region = null,
                   string countryCode = "GB")
    {
        if (string.IsNullOrWhiteSpace(line1)) throw new ArgumentException("Line1 is required", nameof(line1));
        if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City is required", nameof(city));
        if (string.IsNullOrWhiteSpace(postCode)) throw new ArgumentException("PostalCode is required", nameof(postCode));
        if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2) throw new ArgumentException("CountryCode must be ISO-2", nameof(countryCode));

        Line1 = line1.Trim();
        Line2 = string.IsNullOrWhiteSpace(line2) ? null : line2.Trim();
        Line3 = string.IsNullOrWhiteSpace(line3) ? null : line3.Trim();
        City = city.Trim();
        Region = string.IsNullOrWhiteSpace(region) ? null : region.Trim();
        PostCode = postCode.Trim();
        CountryCode = countryCode.ToUpperInvariant();
    }

    public override string ToString() =>
        string.Join(", ", new[] { Line1, Line2, Line3, City, Region, PostCode, CountryCode }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
}

public class AgencyConfiguration
{
    public long AgencyId { get; set; }

    public string? LogoBlobId { get; set; }
    public string? PrimaryColorHex { get; set; }
    public string? SecondaryColorHex { get; set; }

    public int DefaultRentDueDay { get; set; } = 1;
    public RentFrequency DefaultRentFrequency { get; set; }
    public decimal? DefaultCommissionPercent { get; set; }
    public bool EnableArrearsEmails { get; set; } = true;
    public int ArrearsEmailDays { get; set; } = 3;

    public Agency Agency { get; set; } = default!;
}


public class AgencyGroup : EntityBase
{
    public string Slug { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public List<AgencyGroupMembership> Members { get; set; } = new();
}

public  class AgencyGroupMembership 
{
    public long AgencyId { get; set; }
    public long GroupId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public Agency Agency { get; set; } = default!;
    public AgencyGroup Group { get; set; } = default!;
}


public sealed class BillingAccount : EntityBase
{
    public long AgencyId { get; set; }
    public string Provider { get; set; } = default!;
    public string StripeCustomerId { get; set; } = default!;

    public string? BillingEmail { get; set; }
    public string Currency { get; set; } = default!;
    public string? TaxNumber { get; set; }  
    public string? TaxExempt { get; set; } 
    public Agency Agency { get; set; } = default!;
}

public sealed class BillingSubscription : EntityBase
{
    public long AgencyId { get; set; }
    public long BillingAccountId { get; set; }

    public string Provider { get; set; } = default!;
    public string ExternalId { get; set; } = default!; 

    public SubscriptionStatus Status { get; set; }

    public string ProductId { get; set; } = default!;
    public string PriceId { get; set; } = default!;
    public BillingInterval Interval { get; set; }
    public long UnitAmount { get; set; }  
    public string Currency { get; set; } = "GBP";

    public int Seats { get; set; } 

    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public DateTime? TrialEnd { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public DateTime? CanceledAt { get; set; }

    public DateTime LastSyncedAt { get; set; } = DateTime.UtcNow;

    public BillingAccount BillingAccount { get; set; } = default!;
    public Agency Agency { get; set; } = default!;
}

public sealed class BillingSubscriptionItem : EntityBase
{
    public long SubscriptionId { get; set; }
    public string PriceId { get; set; } = default!;
    public int Quantity { get; set; }
    public string? MetadataJson { get; set; } 

    public BillingSubscription Subscription { get; set; } = default!;
}

public sealed class BillingInvoice : EntityBase
{
    public long AgencyId { get; set; }
    public string Provider { get; set; } = default!;
    public string ExternalId { get; set; } = default!;
    public string Number { get; set; } = default!;
    public BillingInvoiceStatus Status { get; set; } 
    public long AmountDue { get; set; }
    public long AmountPaid { get; set; }
    public string Currency { get; set; } = "GBP";
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string? HostedInvoiceUrl { get; set; }
    public string? PdfUrl { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime LastSyncedAt { get; set; }
}

public sealed class AgencyUser : IAgencyOwned
{
    public long AgencyId { get; set; }
    public Guid UserId { get; set; }
    public Role Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeactivatedAt { get; set; }
    public Agency Agency { get; set; } = default!;
    public AppUser User { get; set; } = default!;
}

public enum BillingInterval { Month, Year }