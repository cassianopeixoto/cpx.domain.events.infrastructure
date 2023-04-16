using CPX.Domain.Abstract.Events;

namespace CPX.Events.Infrastructure.Test.Mocks;
public sealed class FooNameChangedEvent : DomainEvent
{
    public FooNameChangedEvent(Guid aggregateId, int version, DateTimeOffset createdAt, string name) : base(aggregateId, version, createdAt)
    {
        Name = name;
    }

    public string Name { get; }
}
