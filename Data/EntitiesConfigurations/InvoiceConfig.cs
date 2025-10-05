using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public class InvoiceConfig : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> b)
    {
        b.ToTable("Invoices");
        b.HasKey(x => x.Id);

        b.Property(x => x.VendorName).IsRequired().HasMaxLength(200);
        b.Property(x => x.Reference).HasMaxLength(120);
        b.Property(x => x.NetAmount).HasPrecision(18, 2);
        b.Property(x => x.VatAmount).HasPrecision(18, 2);
        b.Property(x => x.GrossAmount).HasPrecision(18, 2);

        b.HasIndex(x => x.AgencyId);
        b.HasIndex(x => new { x.IssueDate, x.Status });
        b.HasIndex(x => x.PropertyId);
        b.HasIndex(x => x.TenancyId);

        b.HasOne<Agency>()
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Property)
         .WithMany(x => x.Invoices)
         .HasForeignKey(x => x.PropertyId)
         .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Tenancy)
         .WithMany()
         .HasForeignKey(x => x.TenancyId)
         .OnDelete(DeleteBehavior.SetNull);
    }
}