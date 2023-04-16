using CPX.Domain.Abstract.Events;

namespace CPX.Events.Infrastructure.Test.Mocks;
public sealed class FooCreatedEvent : DomainEvent
{
    public FooCreatedEvent(Guid aggregateId, int version, DateTimeOffset createdAt) : base(aggregateId, version, createdAt)
    {
    }
}
