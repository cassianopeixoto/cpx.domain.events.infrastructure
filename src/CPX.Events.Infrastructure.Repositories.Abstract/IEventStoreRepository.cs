using CPX.Domain.Abstract.Aggregates;
using CPX.Domain.Abstract.Identifiers;

namespace CPX.Events.Infrastructure.Repositories.Abstract;

public interface IEventStoreRepository<TAggregate, TIdentity> where TAggregate : AggregateRoot<TIdentity> where TIdentity : Identifier
{
    void SaveAsync(TAggregate aggregate, CancellationToken cancellationToken);

    TAggregate GetAsync(TIdentity identifier, CancellationToken cancellationToken);
}