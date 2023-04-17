using CPX.Domain.Events.Infrastructure.Repositories;
using CPX.Domain.Events.Infrastructure.Test.Mocks;

namespace CPX.Domain.Events.Infrastructure.Test
{
    public sealed class EventStoryRepositoryTest
    {
        [Fact]
        public async Task Should_be_able_persist_events()
        {
            // Arrange
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<EventStoreContext>();
            var options = dbContextOptionsBuilder.UseInMemoryDatabase("event_store").Options;
            using var eventStoreContext = new EventStoreContext(options);
            var eventStoreRepository = new EventStoreRepository<FooAggregate, FooId>(eventStoreContext);
            var fooId = new FooId(Guid.NewGuid());
            var createdAt = DateTimeOffset.UtcNow;
            var updatedAt = DateTimeOffset.UtcNow.AddMinutes(1);
            var createdBy = Guid.NewGuid();
            var updatedBy = Guid.NewGuid();
            var name = "foo";
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            // Act
            var fooAggregate = new FooAggregate(fooId, createdAt, createdBy);
            fooAggregate.ChangeName(name, updatedAt, updatedBy);
            var domainEvents = fooAggregate.GetUncommitedEvents();
            await eventStoreRepository.SaveAsync(fooAggregate.Id, domainEvents, cancellationToken);
            var aggregate = await eventStoreRepository.GetAsync(fooId, cancellationToken);
            // Assert
            Assert.NotNull(aggregate);
            if (aggregate is not null)
            {
                Assert.Equal(fooId, aggregate.Id);
                Assert.Equal(createdAt, aggregate.CreatedAt);
                Assert.Equal(updatedAt, aggregate.UpdatedAt);
                Assert.Equal(updatedBy, aggregate.UpdatedBy);
                Assert.Equal(domainEvents.Count, aggregate.Version);
                Assert.Equal(name, aggregate.Name);
            }
        }

        [Fact]
        public void Should_not_be_able_create_instance_with_event_store_context_null()
        {
            // Arrange
            EventStoreContext? eventStoreContext = null;
            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                new EventStoreRepository<FooAggregate, FooId>(eventStoreContext);
            });
        }
    }
}
