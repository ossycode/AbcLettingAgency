using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations.Portal;

public class BillingAccountConfig : IEntityTypeConfiguration<BillingAccount>
{
    public void Configure(EntityTypeBuilder<BillingAccount> b)
    {
        b.ToTable("BillingAccounts");
        b.HasKey(x => x.Id);

        b.Property(x => x.Provider).HasMaxLength(50).IsRequired();
        b.Property(x => x.StripeCustomerId).HasMaxLength(100).IsRequired();
        b.Property(x => x.BillingEmail).HasMaxLength(256);
        b.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        b.Property(x => x.TaxNumber).HasMaxLength(50);
        b.Property(x => x.TaxExempt).HasMaxLength(20);

        b.HasIndex(x => new { x.Provider, x.StripeCustomerId }).IsUnique();

        b.HasOne(x => x.Agency)
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasQueryFilter(x => !x.Agency.IsDeleted);
    }
}