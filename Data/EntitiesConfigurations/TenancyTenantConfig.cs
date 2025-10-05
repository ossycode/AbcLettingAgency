using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public class TenancyTenantConfig : IEntityTypeConfiguration<TenancyTenant>
{
    public void Configure(EntityTypeBuilder<TenancyTenant> b)
    {
        b.ToTable("TenancyTenants");
        b.HasKey(x => x.Id);

        b.Property(x => x.ResponsibilitySharePercent).HasPrecision(5, 2);
        b.HasQueryFilter(rr => !rr.Tenancy.IsDeleted);

        b.HasIndex(x => x.AgencyId);
        // One row per (Tenancy, Tenant)
        b.HasIndex(x => new { x.TenancyId, x.TenantId }).IsUnique();
        // Only one primary occupant per tenancy (filtered unique)
        b.HasIndex(x => x.TenancyId)
         .IsUnique()
         .HasFilter("\"IsPrimary\" = TRUE");

        b.HasOne<Agency>()
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Tenancy)
         .WithMany(x => x.Occupants)
         .HasForeignKey(x => x.TenancyId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Tenant)
         .WithMany(x => x.TenancyLinks)
         .HasForeignKey(x => x.TenantId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}