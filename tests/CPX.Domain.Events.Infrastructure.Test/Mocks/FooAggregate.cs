using CPX.Domain.Abstract.Aggregates;
using CPX.Domain.Abstract.Events;

namespace CPX.Domain.Events.Infrastructure.Test.Mocks;

public sealed class FooAggregate : AggregateRoot<FooId>, IApplyDomainEvent<FooCreatedEvent>, IApplyDomainEvent<FooNameChangedEvent>
{
    public FooAggregate() { }

    public FooAggregate(FooId fooId, DateTimeOffset createdAt, Guid createdBy)
    {
        Raise(new FooCreatedEvent(fooId, Version + 1, createdAt, createdBy));
    }

    public string? Name { get; private set; }

    public void Apply(FooCreatedEvent @event)
    {
        Id = @event.AggregateId;
        CreatedAt = @event.CreatedAt;
        base.Apply(@event);
    }

    public void Apply(FooNameChangedEvent @event)
    {
        Name = @event.Name;
        base.Apply(@event);
    }

    public void ChangeName(string name, DateTimeOffset updatedAt, Guid updatedBy)
    {
        Raise(new FooNameChangedEvent(Id, Version + 1, updatedAt, updatedBy, name));
    }
}