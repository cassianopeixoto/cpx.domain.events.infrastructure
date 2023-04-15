using CPX.Events.Infrastructure.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPX.Events.Infrastructure.Repositories.Configurations;

public sealed class EventTypeConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");
        builder.HasKey(o => o.Uuid);
        builder.Property(o => o.Uuid).HasColumnName("uuid");
        builder.Property(o => o.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(o => o.MetadataUuid).HasColumnName("metadata_uuid").IsRequired();
        builder.HasOne(o => o.Metadata).WithMany(o => o.Events).HasForeignKey(o => o.MetadataUuid);
        builder.Property(o => o.EventStoreUuid).HasColumnName("event_store_uuid").IsRequired();
        builder.HasOne(o => o.EventStore).WithMany(o => o.Events).HasForeignKey(o => o.EventStoreUuid);
        builder.Property(o => o.Data).HasColumnName("data").IsRequired();
        builder.Property(o => o.Version).HasColumnName("version").IsRequired();

        builder.HasIndex(o => new { o.MetadataUuid, o.EventStoreUuid, o.Version }).IsUnique();
    }
}