using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public class RentChargeConfig : IEntityTypeConfiguration<RentCharge>
{
    public void Configure(EntityTypeBuilder<RentCharge> b)
    {
        b.ToTable("RentCharges");
        b.HasKey(x => x.Id);

        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.CommissionDue).HasPrecision(18, 2);
        b.Property(x => x.AmountAfterCommission).HasPrecision(18, 2);
        b.Property(x => x.Notes).HasMaxLength(4000);

        b.HasIndex(x => x.AgencyId);
        b.HasIndex(x => new { x.TenancyId, x.DueDate });
        // Avoid duplicates for the same period (optional)
        b.HasIndex(x => new { x.TenancyId, x.PeriodStart, x.PeriodEnd }).IsUnique();

        b.HasOne<Agency>()
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Tenancy)
         .WithMany(x => x.Charges)
         .HasForeignKey(x => x.TenancyId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
