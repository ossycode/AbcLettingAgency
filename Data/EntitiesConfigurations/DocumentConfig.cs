using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations
{
    public class DocumentConfig : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> b)
        {
            b.ToTable("Documents");
            b.HasKey(x => x.Id);

            b.Property(x => x.Url).IsRequired().HasMaxLength(600);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.MimeType).HasMaxLength(150);

            b.HasIndex(x => x.AgencyId);
            b.HasIndex(x => x.UploadedAt);
            b.HasIndex(x => x.TenancyId);
            b.HasIndex(x => x.TenantId);
            b.HasIndex(x => x.PropertyId);
            b.HasIndex(x => x.InvoiceId);

            b.HasOne<Agency>()
             .WithMany()
             .HasForeignKey(x => x.AgencyId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Tenancy)
             .WithMany(x => x.Documents)
             .HasForeignKey(x => x.TenancyId)
             .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.Tenant)
             .WithMany(x => x.Documents)
             .HasForeignKey(x => x.TenantId)
             .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.Property)
             .WithMany(x => x.Documents)
             .HasForeignKey(x => x.PropertyId)
             .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.Invoice)
             .WithMany(x => x.Documents)
             .HasForeignKey(x => x.InvoiceId)
             .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
