using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public sealed class TenancyConfig : IEntityTypeConfiguration<Tenancy>
{
    public void Configure(EntityTypeBuilder<Tenancy> b)
    {
        b.ToTable("Tenancies");
        b.HasKey(x => x.Id);

        b.Property(x => x.RentDueDay).IsRequired();
        b.Property(x => x.RentAmount).HasPrecision(12, 2);
        b.Property(x => x.CommissionPercent).HasPrecision(5, 2);
        b.Property(x => x.ManagementFeePercent).HasPrecision(5, 2);
        b.Property(x => x.DepositAmount).HasPrecision(12, 2);
        b.Property(x => x.DepositLocation).HasMaxLength(200);

        b.HasIndex(x => x.AgencyId);
        b.HasIndex(x => new { x.AgencyId, x.Status });

        b.HasOne<Agency>()
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Property)
         .WithMany(x => x.Tenancies)
         .HasForeignKey(x => x.PropertyId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Landlord)
         .WithMany(x => x.Tenancies)
         .HasForeignKey(x => x.LandlordId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
