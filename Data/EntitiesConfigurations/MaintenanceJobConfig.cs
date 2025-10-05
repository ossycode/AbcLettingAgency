using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public class MaintenanceJobConfig : IEntityTypeConfiguration<MaintenanceJob>
{
    public void Configure(EntityTypeBuilder<MaintenanceJob> b)
    {
        b.ToTable("MaintenanceJobs");
        b.HasKey(x => x.Id);

        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Cost).HasPrecision(18, 2);

        b.HasIndex(x => x.AgencyId);
        b.HasIndex(x => new { x.PropertyId, x.Status });

        b.HasOne<Agency>()
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Property)
         .WithMany(x => x.Maintenance)
         .HasForeignKey(x => x.PropertyId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Invoice)
         .WithMany()
         .HasForeignKey(x => x.InvoiceId)
         .OnDelete(DeleteBehavior.SetNull);
    }
}
