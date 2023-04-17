using CPX.Domain.Abstract.Events;

namespace CPX.Domain.Events.Infrastructure.Test.Mocks;
public sealed class FooNameChangedEvent : DomainEvent
{
    public FooNameChangedEvent(Guid aggregateId, int version, DateTimeOffset createdAt, Guid createdBy, string name) : base(aggregateId, version, createdAt, createdBy)
    {
        Name = name;
    }

    public string Name { get; }
}
