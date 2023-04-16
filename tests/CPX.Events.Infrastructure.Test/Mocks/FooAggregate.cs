using CPX.Domain.Abstract.Aggregates;
using CPX.Domain.Abstract.Events;

namespace CPX.Events.Infrastructure.Test.Mocks;

public sealed class FooAggregate : AggregateRoot<FooId>, IApplyDomainEvent<FooCreatedEvent>, IApplyDomainEvent<FooNameChangedEvent>
{
    public FooAggregate() : base()
    {
    }

    public FooAggregate(FooId fooId, DateTimeOffset createdAt) : base(fooId, createdAt)
    {
        Raise(new FooCreatedEvent(fooId, Version + 1, createdAt));
    }

    public string Name { get; private set; }

    public void Apply(FooCreatedEvent @event)
    {
        Id = @event.AggregateId;
        CreatedAt = @event.CreatedAt;
        UpdatedAt = @event.CreatedAt;
        Version = @event.Version;
    }

    public void Apply(FooNameChangedEvent @event)
    {
        Name = @event.Name;
        UpdatedAt = @event.CreatedAt;
        Version = @event.Version;
    }

    public void ChangeName(string name, DateTimeOffset updatedAt)
    {
        Raise(new FooNameChangedEvent(Id, Version + 1, updatedAt, name));
    }
}