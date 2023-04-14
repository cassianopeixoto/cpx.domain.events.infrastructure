using CPX.Domain.Abstract.Aggregates;
using CPX.Domain.Abstract.Identifiers;
using CPX.Events.Infrastructure.Repositories.Abstract;

namespace CPX.Events.Infrastructure.Repositories;

public sealed class EventStoreRepository<TAggregate, TIdentity> : IEventStoreRepository<TAggregate, TIdentity> where TAggregate : AggregateRoot<TIdentity> where TIdentity : Identifier
{
    void SaveAsync(TAggregate aggregate, CancellationToken cancellationToken) { }

    TAggregate GetAsync(TIdentity identifier, CancellationToken cancellationToken) { }
}