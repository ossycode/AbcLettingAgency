using AbcLettingAgency.EntityModel;
using AbcLettingAgency.Shared.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AbcLettingAgency.Data.EntitiesConfigurations;

public class OutboxMessageConfig : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages", "public");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Payload).IsRequired();
        builder.Property(x => x.OccurredUtc).IsRequired();
        builder.Property(x => x.Attempts).HasDefaultValue(0);
        builder.HasIndex(x => x.ProcessedUtc);
        builder.HasIndex(x => x.DeadLetteredUtc);
        builder.Property(x => x.Payload)
                 .HasColumnType("jsonb")
                 .IsRequired();
        builder.HasIndex(x => new { x.Type, x.DedupKey })
         .IsUnique()
         .HasFilter("\"DedupKey\" IS NOT NULL");

        builder.HasIndex(x => x.AgencyId);
    }
}


