using CPX.Domain.Abstract.Events;

namespace CPX.Domain.Events.Infrastructure.Test.Mocks;
public sealed class FooCreatedEvent : DomainEvent
{
    public FooCreatedEvent(Guid aggregateId, int version, DateTimeOffset createdAt, Guid createdBy) : base(aggregateId, version, createdAt, createdBy)
    {
    }
}
