using AbcLettingAgency.EntityModel;
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public class ClientLedgerConfig : IEntityTypeConfiguration<ClientLedger>
{
    public void Configure(EntityTypeBuilder<ClientLedger> b)
    {
        b.ToTable("ClientLedgers");
        b.HasKey(x => x.Id);

        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.Description);

        b.HasIndex(x => x.AgencyId);
        b.HasIndex(x => new { x.EntryType, x.OccurredAt });

        b.HasOne<Agency>()
         .WithMany()
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Tenancy)
         .WithMany(x => x.Ledger)
         .HasForeignKey(x => x.TenancyId)
         .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Property)
         .WithMany()
         .HasForeignKey(x => x.PropertyId)
         .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Landlord)
         .WithMany()
         .HasForeignKey(x => x.LandlordId)
         .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Tenant)
         .WithMany()
         .HasForeignKey(x => x.TenantId)
         .OnDelete(DeleteBehavior.SetNull);
    }
}
