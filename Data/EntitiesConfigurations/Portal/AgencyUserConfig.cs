using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations.Portal;

public sealed class AgencyUserConfig : IEntityTypeConfiguration<AgencyUser>
{
    public void Configure(EntityTypeBuilder<AgencyUser> b)
    {
        b.ToTable("AgencyUsers");

        b.HasKey(x => new { x.AgencyId, x.UserId });

        b.Property(x => x.Role)
         .HasConversion<string>()
         .HasMaxLength(50)
         .IsRequired();

        b.Property(x => x.JoinedAt).IsRequired();

        b.HasIndex(x => x.AgencyId);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => new { x.UserId, x.IsActive });

        b.HasOne(x => x.Agency)
         .WithMany(a => a.Users)             
         .HasForeignKey(x => x.AgencyId)
         .OnDelete(DeleteBehavior.Cascade); 

        b.HasOne(x => x.User)
         .WithMany()
         .HasForeignKey(x => x.UserId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasQueryFilter(x => !x.Agency.IsDeleted);
    }
}