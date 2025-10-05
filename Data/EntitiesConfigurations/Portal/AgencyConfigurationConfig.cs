using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations.Portal;

public class AgencyConfigurationConfig : IEntityTypeConfiguration<AgencyConfiguration>
{
    public void Configure(EntityTypeBuilder<AgencyConfiguration> b)
    {
        b.ToTable("AgencyConfigurations");

        b.HasKey(x => x.AgencyId);

        b.Property(x => x.LogoBlobId).HasMaxLength(200);
        b.Property(x => x.PrimaryColorHex).HasMaxLength(7);
        b.Property(x => x.SecondaryColorHex).HasMaxLength(7);

        b.Property(x => x.DefaultRentDueDay).IsRequired();
        b.Property(x => x.DefaultRentFrequency)
         .HasConversion<string>()
         .HasMaxLength(32)
         .IsRequired();

        b.Property(x => x.DefaultCommissionPercent).HasPrecision(18, 2);
        b.Property(x => x.EnableArrearsEmails).IsRequired();
        b.Property(x => x.ArrearsEmailDays).IsRequired();
    }
}