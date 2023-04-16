using CPX.Events.Infrastructure.Repositories.Abstract;
using CPX.Events.Infrastructure.Repositories.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CPX.Events.Infrastructure.Repositories;

public sealed class EventStoreContext : DbContext, IEventStoreContext
{
    public EventStoreContext(DbContextOptions options) : base(options)
    {
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MetadataTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EventStoreTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EventTypeConfiguration());
    }
}