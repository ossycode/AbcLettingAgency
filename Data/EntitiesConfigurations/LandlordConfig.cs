using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public sealed class LandlordConfig : IEntityTypeConfiguration<Landlord>
{
    public void Configure(EntityTypeBuilder<Landlord> b)
    {
        b.ToTable("Landlords");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Email).HasMaxLength(256);
        b.Property(x => x.Phone).HasMaxLength(50);
        b.Property(x => x.Address).HasMaxLength(400);
        b.Property(x => x.BankIban).HasMaxLength(34);
        b.Property(x => x.BankSort).HasMaxLength(20);

        b.HasIndex(x => new { x.AgencyId, x.Name });
        b.HasIndex(x => new { x.AgencyId, x.Email }).IsUnique().HasFilter("\"Email\" IS NOT NULL");

        b.HasOne<Agency>()
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
