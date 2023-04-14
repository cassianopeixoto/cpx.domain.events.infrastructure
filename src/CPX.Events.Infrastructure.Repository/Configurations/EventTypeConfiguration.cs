using CPX.Events.Infrastructure.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPX.Events.Infrastructure.Repository.Configurations;

public sealed class EventTypeConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");
        builder.HasKey(o => o.Uuid);
        builder.Property(o => o.Uuid).HasColumnName("uuid");
        builder.Property(o => o.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(o => o.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(o => o.MetadataUuid).HasColumnName("metadata_uuid").IsRequired();
        builder.HasOne(o => o.Metadata).WithMany(o => o.Events).HasForeignKey(o => o.MetadataUuid);
        builder.Property(o => o.AggregateUuid).HasColumnName("aggregate_uuid").IsRequired();
        builder.HasOne(o => o.Aggregate).WithMany(o => o.Events).HasForeignKey(o => o.AggregateUuid);
        builder.Property(o => o.Data).HasColumnName("data").IsRequired();
        builder.Property(o => o.Version).HasColumnName("version").IsRequired();

        builder.HasIndex(o => new { o.MetadataUuid, o.AggregateUuid, o.Version }).IsUnique();
    }
}