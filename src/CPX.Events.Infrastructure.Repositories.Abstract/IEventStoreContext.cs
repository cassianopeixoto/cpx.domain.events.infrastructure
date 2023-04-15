using Microsoft.EntityFrameworkCore;

namespace CPX.Events.Infrastructure.Repositories.Abstract;

public interface IEventStoreContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}