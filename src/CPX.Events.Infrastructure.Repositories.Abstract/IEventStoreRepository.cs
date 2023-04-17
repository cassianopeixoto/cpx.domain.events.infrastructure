using CPX.Domain.Abstract.Aggregates;
using CPX.Domain.Abstract.Events;
using CPX.Domain.Abstract.Identifiers;

namespace CPX.Events.Infrastructure.Repositories.Abstract;

public interface IEventStoreRepository<TAggregate, TIdentity> where TAggregate : AggregateRoot<TIdentity> where TIdentity : Identifier
{
    Task SaveAsync(Guid eventStoreUuid, IReadOnlyCollection<DomainEvent> domainEvents, CancellationToken cancellationToken);

    Task<TAggregate?> GetAsync(TIdentity identifier, CancellationToken cancellationToken);

    Task<TAggregate?> GetAsync(TIdentity identifier, string methodName, CancellationToken cancellationToken);
}