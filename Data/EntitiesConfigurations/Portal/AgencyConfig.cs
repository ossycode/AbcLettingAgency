using AbcLettingAgency.EntityModel.Agencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations.Portal;

public class AgencyConfig : IEntityTypeConfiguration<Agency>
{
    public void Configure(EntityTypeBuilder<Agency> builder)
    {
        builder.ToTable("Agencies");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Slug).HasMaxLength(128).IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.LegalName).HasMaxLength(200);

        builder.Property(x => x.Status)
         .HasConversion<string>()
         .HasMaxLength(32)
         .IsRequired();

        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(x => x.Email);

        builder.Property(x => x.PhoneNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.PhoneNumber2).HasMaxLength(50);
        builder.Property(x => x.Website).HasMaxLength(256);

        builder.Property(x => x.TimeZone).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.CompanyNumber).HasMaxLength(50);
        builder.Property(x => x.VatNumber).HasMaxLength(50);

        builder.HasIndex(x => new { x.OrgId, x.Name }).IsUnique();

        // Self-reference
        builder.HasOne(x => x.ParentAgency)
         .WithMany(x => x.Children)
         .HasForeignKey(x => x.ParentAgencyId)
         .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(x => x.Address, o =>
        {
            o.Property(p => p.Line1).HasMaxLength(200).HasColumnName("Address_Line1").IsRequired();
            o.Property(p => p.Line2).HasMaxLength(200).HasColumnName("Address_Line2");
            o.Property(p => p.Line3).HasMaxLength(200).HasColumnName("Address_Line3");
            o.Property(p => p.City).HasMaxLength(100).HasColumnName("Address_City").IsRequired();
            o.Property(p => p.Region).HasMaxLength(100).HasColumnName("Address_Region");
            o.Property(p => p.PostCode).HasMaxLength(20).HasColumnName("Address_PostCode").IsRequired();
            o.Property(p => p.CountryCode).HasMaxLength(2).HasColumnName("Address_CountryCode").IsRequired();
        });
        builder.HasOne(a => a.Configuration)
            .WithOne(c => c.Agency)
            .HasForeignKey<AgencyConfiguration>(c => c.AgencyId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

    }
}

