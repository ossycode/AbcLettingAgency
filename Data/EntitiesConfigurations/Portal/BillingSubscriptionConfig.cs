using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations.Portal;

public class BillingSubscriptionConfig : IEntityTypeConfiguration<BillingSubscription>
{
    public void Configure(EntityTypeBuilder<BillingSubscription> b)
    {
        b.ToTable("BillingSubscriptions");
        b.HasKey(x => x.Id);

        b.Property(x => x.Provider).HasMaxLength(50).IsRequired();
        b.Property(x => x.ExternalId).HasMaxLength(100).IsRequired();
        b.HasIndex(x => new { x.Provider, x.ExternalId }).IsUnique();

        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        b.Property(x => x.ProductId).HasMaxLength(100).IsRequired();
        b.Property(x => x.PriceId).HasMaxLength(100).IsRequired();
        b.Property(x => x.Interval).HasConversion<string>().HasMaxLength(16).IsRequired();
        b.Property(x => x.Currency).HasMaxLength(3).IsRequired();

        b.HasOne(x => x.Agency)
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.BillingAccount)
         .WithMany()
         .HasForeignKey(x => x.BillingAccountId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasQueryFilter(x => !x.Agency.IsDeleted);
    }
}
