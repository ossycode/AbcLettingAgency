using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public class TenantConfig : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> b)
    {
        b.ToTable("Tenants");
        b.HasKey(x => x.Id);

        b.Property(x => x.FirstName).IsRequired().HasMaxLength(120);
        b.Property(x => x.LastName).IsRequired().HasMaxLength(120);
        b.Property(x => x.Email).HasMaxLength(256);
        b.Property(x => x.SecondEmail).HasMaxLength(256);
        b.Property(x => x.Phone).HasMaxLength(50);
        b.Property(x => x.SecondPhone).HasMaxLength(50);

        b.HasIndex(x => x.AgencyId);
        b.HasIndex(x => new { x.AgencyId, x.Email }).IsUnique().HasFilter("\"Email\" <> ''");
        b.HasIndex(x => new { x.AgencyId, x.Phone }).HasFilter("\"Phone\" <> ''");

        b.HasOne<Agency>()
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
