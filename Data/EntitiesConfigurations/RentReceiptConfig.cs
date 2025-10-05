using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public class RentReceiptConfig : IEntityTypeConfiguration<RentReceipt>
{
    public void Configure(EntityTypeBuilder<RentReceipt> b)
    {
        b.ToTable("RentReceipts");
        b.HasKey(x => x.Id);

        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.Method).HasMaxLength(60);
        b.Property(x => x.Reference).HasMaxLength(120);

        b.HasIndex(x => x.AgencyId);
        b.HasIndex(x => new { x.TenancyId, x.ReceivedAt });
        b.HasIndex(x => x.ChargeId);

        b.HasOne<Agency>()
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Tenancy)
         .WithMany(x => x.Receipts)
         .HasForeignKey(x => x.TenancyId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Charge)
         .WithMany(x => x.Receipts)
         .HasForeignKey(x => x.ChargeId)
         .OnDelete(DeleteBehavior.SetNull);

        b.HasQueryFilter(rr => !rr.Tenancy.IsDeleted);

    }
}
