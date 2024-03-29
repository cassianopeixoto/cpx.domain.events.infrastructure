using CPX.Domain.Events.Infrastructure.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPX.Domain.Events.Infrastructure.Repositories.Configurations;

public sealed class MetadataTypeConfiguration : IEntityTypeConfiguration<Metadata>
{
    public void Configure(EntityTypeBuilder<Metadata> builder)
    {
        builder.ToTable("metadatas");
        builder.HasKey(o => o.Uuid);
        builder.Property(o => o.Uuid).HasColumnName("uuid");
        builder.Property(o => o.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(o => o.AssemblyName).HasColumnName("assembly_name").IsRequired();
        builder.Property(o => o.NamespaceName).HasColumnName("namespace_name").IsRequired();
        builder.Property(o => o.ClassName).HasColumnName("class_name").IsRequired();
        builder.HasMany(o => o.EventsStores).WithOne(o => o.Metadata).HasForeignKey(o => o.MetadataUuid);
        builder.HasMany(o => o.Events).WithOne(o => o.Metadata).HasForeignKey(o => o.MetadataUuid);

        builder.HasIndex(o => new { o.AssemblyName, o.NamespaceName, o.ClassName }).IsUnique();
    }
}