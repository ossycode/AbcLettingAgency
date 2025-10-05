
using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations.Portal;

public class AgencyGroupConfig : IEntityTypeConfiguration<AgencyGroup>
{
    public void Configure(EntityTypeBuilder<AgencyGroup> b)
    {
        b.ToTable("AgencyGroups");
        b.HasKey(x => x.Id);

        b.Property(x => x.Slug).HasMaxLength(128).IsRequired();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Description).HasMaxLength(500);

        b.HasIndex(x => x.Slug).IsUnique();
    }
}
