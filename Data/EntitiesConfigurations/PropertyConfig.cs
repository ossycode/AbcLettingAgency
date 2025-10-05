using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public class PropertyConfig : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> b)
    {
        b.ToTable("Properties");
        b.HasKey(x => x.Id);

        b.Property(x => x.Code).HasMaxLength(50);
        b.Property(x => x.AddressLine1).IsRequired().HasMaxLength(200);
        b.Property(x => x.AddressLine2).HasMaxLength(200);
        b.Property(x => x.City).IsRequired().HasMaxLength(120);
        b.Property(x => x.Postcode).IsRequired().HasMaxLength(20);

        b.HasIndex(x => x.AgencyId);
        b.HasIndex(x => new { x.AgencyId, x.Code }).IsUnique().HasFilter("\"Code\" <> ''");

        b.HasOne<Agency>()
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Landlord)
         .WithMany(x => x.Properties)
         .HasForeignKey(x => x.LandlordId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
