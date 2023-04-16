using CPX.Events.Infrastructure.Repositories;
using CPX.Events.Infrastructure.Test.Mocks;

namespace CPX.Events.Infrastructure.Test
{
    public class EventStoryRepositoryTest
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
            var name = "foo";
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            // Act
            var fooAggregate = new FooAggregate(fooId, createdAt);
            fooAggregate.ChangeName(name, updatedAt);
            var domainEvents = fooAggregate.GetUncommitedEvents();
            await eventStoreRepository.SaveAsync(fooAggregate.Id, domainEvents, cancellationToken);
            var aggregate = await eventStoreRepository.GetAsync(fooId, "Apply", cancellationToken);
            // Assert
            Assert.Equal(fooId, aggregate.Id);
            Assert.Equal(createdAt, aggregate.CreatedAt);
            Assert.Equal(updatedAt, aggregate.UpdatedAt);
            Assert.Equal(domainEvents.Count, aggregate.Version);
            Assert.Equal(name, aggregate.Name);
        }
    }
}
