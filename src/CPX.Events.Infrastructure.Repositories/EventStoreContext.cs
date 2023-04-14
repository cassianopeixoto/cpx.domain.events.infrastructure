using CPX.Events.Infrastructure.Repositories.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CPX.Events.Infrastructure.Repositories;

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