using System.Globalization;
using CPX.Domain.Abstract.Aggregates;
using CPX.Domain.Abstract.Events;
using CPX.Domain.Abstract.Identifiers;
using CPX.Events.Infrastructure.Repositories.Abstract;
using CPX.Events.Infrastructure.Repositories.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CPX.Events.Infrastructure.Repositories;

public sealed class EventStoreRepository<TAggregate, TIdentity> : IEventStoreRepository<TAggregate, TIdentity> where TAggregate : AggregateRoot<TIdentity> where TIdentity : Identifier
{
    private readonly IEventStoreContext eventStoreContext;

    public EventStoreRepository(IEventStoreContext eventStoreContext)
    {
        this.eventStoreContext = eventStoreContext;
    }

    public async Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken)
    {
        var eventStoreUuid = await CreateEventStoreAsync(aggregate, cancellationToken);

        var domainEvents = aggregate.GetUncommitedEvents();

        await CreateEventsAsync(eventStoreUuid, domainEvents, cancellationToken);

        await eventStoreContext.SaveChangesAsync(cancellationToken);

        aggregate.Commit();
    }

    public TAggregate GetAsync(TIdentity identifier, CancellationToken cancellationToken)
    {
        return null;
    }

    private async Task<Guid> CreateEventStoreAsync(TAggregate aggregate, CancellationToken cancellationToken)
    {
        if (aggregate is null) throw new ArgumentNullException(nameof(aggregate));

        var eventStoreUuid = Guid.NewGuid();

        if (aggregate.Id is null)
            eventStoreUuid = aggregate.Id;

        var setEventStore = eventStoreContext.Set<EventStore>();
        var eventStore = setEventStore.SingleOrDefault(o => o.Uuid == eventStoreUuid);

        if (eventStore is null)
        {
            var aggregateMetadata = await GetMetadataAsync(aggregate, cancellationToken);

            eventStore = new EventStore
            {
                Uuid = eventStoreUuid,
                CreatedAt = DateTimeOffset.UtcNow,
                MetadataUuid = aggregateMetadata.Uuid
            };
            await setEventStore.AddAsync(eventStore, cancellationToken);
        }

        return eventStoreUuid;
    }

    private async Task CreateEventsAsync(Guid eventStoreUuid, IReadOnlyCollection<DomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        var events = new List<Event>();

        foreach (var domainEvent in domainEvents)
        {
            var eventMetadata = await GetMetadataAsync(domainEvent, cancellationToken);

            var @event = new Event
            {
                Uuid = Guid.NewGuid(),
                CreatedAt = DateTimeOffset.UtcNow,
                MetadataUuid = eventMetadata.Uuid,
                EventStoreUuid = eventStoreUuid,
                Data = SerializeObject(domainEvent),
                Version = domainEvent.Version,
            };

            events.Add(@event);
        }

        var setEvent = eventStoreContext.Set<Event>();
        await setEvent.AddRangeAsync(events, cancellationToken);
    }

    private async Task<Metadata> GetMetadataAsync(object @object, CancellationToken cancellationToken)
    {
        var setMetadata = eventStoreContext.Set<Metadata>();
        var type = @object.GetType();
        var className = type.Name;
        var namespaceName = type.Namespace;
        var metadata = setMetadata.SingleOrDefault(o => o.ClassName == className && o.NamespaceName == namespaceName);

        if (metadata is null)
        {
            metadata = new Metadata
            {
                Uuid = Guid.NewGuid(),
                CreatedAt = DateTimeOffset.UtcNow,
                ClassName = className,
                NamespaceName = namespaceName
            };

            await setMetadata.AddAsync(metadata, cancellationToken);
        }

        return metadata;
    }

    private static string SerializeObject(DomainEvent @event)
    {
        return JsonConvert.SerializeObject(@event, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            Culture = CultureInfo.InvariantCulture
        });
    }
}