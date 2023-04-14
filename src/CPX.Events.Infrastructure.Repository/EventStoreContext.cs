using CPX.Events.Infrastructure.Repository.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CPX.Events.Infrastructure.Repository;

public sealed class EventStoreContext : DbContext
{
    public EventStoreContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MetadataTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AggregateTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EventTypeConfiguration());
    }
}