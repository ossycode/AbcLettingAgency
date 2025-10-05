using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations.Portal;

public class BillingSubscriptionItemConfig : IEntityTypeConfiguration<BillingSubscriptionItem>
{
    public void Configure(EntityTypeBuilder<BillingSubscriptionItem> b)
    {
        b.ToTable("BillingSubscriptionItems");
        b.HasKey(x => x.Id);

        b.Property(x => x.PriceId).HasMaxLength(100).IsRequired();
        b.Property(x => x.MetadataJson).HasColumnType("jsonb");

        b.HasOne(x => x.Subscription)
         .WithMany()
         .HasForeignKey(x => x.SubscriptionId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasQueryFilter(x => !x.Subscription.Agency.IsDeleted);
    }
}
