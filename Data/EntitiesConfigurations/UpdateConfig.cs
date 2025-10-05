using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public class UpdateConfig : IEntityTypeConfiguration<Update>
{
    public void Configure(EntityTypeBuilder<Update> b)
    {
        b.ToTable("Updates");
        b.HasKey(x => x.Id);

        b.Property(x => x.Body).IsRequired();
        b.Property(x => x.CreatedBy).HasMaxLength(200);

        b.HasIndex(x => x.AgencyId);
        b.HasIndex(x => x.PropertyId);
        b.HasIndex(x => x.TenancyId);
        b.HasIndex(x => x.LandlordId);
        b.HasIndex(x => x.TenantId);

        b.HasOne<Agency>()
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Property)
         .WithMany(x => x.Updates)
         .HasForeignKey(x => x.PropertyId)
         .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Tenancy)
         .WithMany(x => x.Updates)
         .HasForeignKey(x => x.TenancyId)
         .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Landlord)
         .WithMany(x => x.Updates)
         .HasForeignKey(x => x.LandlordId)
         .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Tenant)
         .WithMany(x => x.Updates)
         .HasForeignKey(x => x.TenantId)
         .OnDelete(DeleteBehavior.SetNull);
    }
}
