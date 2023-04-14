using CPX.Events.Infrastructure.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPX.Events.Infrastructure.Repositories.Configurations;

public sealed class AggregateTypeConfiguration : IEntityTypeConfiguration<Aggregate>
{
    public void Configure(EntityTypeBuilder<Aggregate> builder)
    {
        builder.ToTable("aggregates");
        builder.HasKey(o => o.Uuid);
        builder.Property(o => o.Uuid).HasColumnName("uuid");
        builder.Property(o => o.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(o => o.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(o => o.MetadataUuid).HasColumnName("metadata_uuid").IsRequired();
        builder.HasOne(o => o.Metadata).WithMany(o => o.Aggregates).HasForeignKey(o => o.MetadataUuid);
        builder.HasMany(o => o.Events).WithOne(o => o.Aggregate).HasForeignKey(o => o.AggregateUuid);

        builder.HasIndex(o => new { o.Uuid, o.MetadataUuid }).IsUnique();
    }
}