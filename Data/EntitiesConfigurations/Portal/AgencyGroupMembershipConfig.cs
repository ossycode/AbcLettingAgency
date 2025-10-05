using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations.Portal;

public class AgencyGroupMembershipConfig : IEntityTypeConfiguration<AgencyGroupMembership>
{
    public void Configure(EntityTypeBuilder<AgencyGroupMembership> b)
    {
        b.ToTable("AgencyGroupMemberships");
        b.HasKey(x => new { x.AgencyId, x.GroupId });

        b.HasOne(x => x.Agency)
         .WithMany(a => a.GroupMemberships)
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Group)
         .WithMany(g => g.Members)
         .HasForeignKey(x => x.GroupId)
         .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.AddedAt).IsRequired();

        b.HasQueryFilter(x => !x.Agency.IsDeleted);
    }
}
