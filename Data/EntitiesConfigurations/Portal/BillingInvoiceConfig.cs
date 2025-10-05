using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations.Portal;

public class BillingInvoiceConfig : IEntityTypeConfiguration<BillingInvoice>
{
    public void Configure(EntityTypeBuilder<BillingInvoice> b)
    {
        b.ToTable("BillingInvoices");
        b.HasKey(x => x.Id);

        b.Property(x => x.Provider).HasMaxLength(50).IsRequired();
        b.Property(x => x.ExternalId).HasMaxLength(100).IsRequired();
        b.Property(x => x.Number).HasMaxLength(50).IsRequired();

        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        b.Property(x => x.Currency).HasMaxLength(3).IsRequired();

        b.Property(x => x.HostedInvoiceUrl).HasMaxLength(500);
        b.Property(x => x.PdfUrl).HasMaxLength(500);

        b.HasIndex(x => new { x.Provider, x.ExternalId }).IsUnique();
        b.HasIndex(x => x.Number).IsUnique();

        b.HasIndex(x => new { x.AgencyId, x.IssuedAt });

        b.HasQueryFilter(x => !EF.Property<Agency>(x, "Agency").IsDeleted);
    }
}