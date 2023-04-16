using CPX.Events.Infrastructure.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPX.Events.Infrastructure.Repositories.Configurations;

public sealed class EventStoreTypeConfiguration : IEntityTypeConfiguration<EventStore>
{
    public void Configure(EntityTypeBuilder<EventStore> builder)
    {
        builder.ToTable("events_stores");
        builder.HasKey(o => o.Uuid);
        builder.Property(o => o.Uuid).HasColumnName("uuid");
        builder.Property(o => o.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(o => o.MetadataUuid).HasColumnName("metadata_uuid").IsRequired();
        builder.HasOne(o => o.Metadata).WithMany(o => o.EventsStores).HasForeignKey(o => o.MetadataUuid);
        builder.HasMany(o => o.Events).WithOne(o => o.EventStore).HasForeignKey(o => o.EventStoreUuid);

        builder.HasIndex(o => new { o.Uuid, o.MetadataUuid }).IsUnique();
    }
}