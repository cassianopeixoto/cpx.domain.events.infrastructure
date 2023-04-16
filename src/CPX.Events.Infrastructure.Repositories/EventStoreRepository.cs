using System.Globalization;
using System.Reflection;
using CPX.Domain.Abstract.Aggregates;
using CPX.Domain.Abstract.Events;
using CPX.Domain.Abstract.Identifiers;
using CPX.Events.Infrastructure.Repositories.Abstract;
using CPX.Events.Infrastructure.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CPX.Events.Infrastructure.Repositories;

public sealed class EventStoreRepository<TAggregate, TIdentity> : IEventStoreRepository<TAggregate, TIdentity> where TIdentity : Identifier where TAggregate : AggregateRoot<TIdentity>
{
    private readonly IEventStoreContext eventStoreContext;

    public EventStoreRepository(IEventStoreContext eventStoreContext)
    {
        this.eventStoreContext = eventStoreContext;
    }

    public async Task SaveAsync(Guid eventStoreUuid, IReadOnlyCollection<DomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        await CreateEventStoreAsync(eventStoreUuid, cancellationToken);
        await CreateEventsAsync(eventStoreUuid, domainEvents, cancellationToken);
        await eventStoreContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TAggregate?> GetAsync(TIdentity identifier, string methodName, CancellationToken cancellationToken)
    {
        var eventStore = await GetEventStoreAsync(identifier, cancellationToken);

        if (eventStore is null)
            return null;

        var aggregateType = GetType(eventStore.Metadata);

        if (aggregateType is null)
            return null;

        var instance = Activator.CreateInstance(aggregateType);

        var methodInfos = aggregateType.GetMethods().Where(o => o.Name == methodName).ToArray();

        if (methodInfos.Any().Equals(false))
            return null;

        var events = eventStore.Events;

        foreach (var @event in events)
        {
            var eventType = GetType(@event.Metadata);

            if (eventType is not null)
            {
                var domainEvent = JsonConvert.DeserializeObject(@event.Data, eventType);

                if (domainEvent is not null)
                {
                    foreach (var methodInfo in methodInfos)
                    {
                        var parameter = methodInfo.GetParameters().SingleOrDefault(o => o.ParameterType == eventType);
                        if (parameter is not null)
                        {
                            methodInfo.Invoke(instance, new object[] { domainEvent });
                        }
                    }
                }
            }
        }

        return instance as TAggregate;
    }

    private static Type? GetType(Metadata metadata)
    {
        try
        {
            var assembly = Assembly.Load(metadata.AssemblyName);
            var fullTypeName = $"{metadata.NamespaceName}.{metadata.ClassName}";
            var type = assembly.GetType(fullTypeName);
            return type;
        }
        catch
        {
            return null;
        }
    }

    private async Task CreateEventStoreAsync(Guid eventStoreUuid, CancellationToken cancellationToken)
    {
        var eventStore = await GetEventStoreAsync(eventStoreUuid, cancellationToken);

        if (eventStore is null)
        {
            var metadataUuid = await GetMetadataAsync<TAggregate>(cancellationToken);

            eventStore = new EventStore
            {
                Uuid = eventStoreUuid,
                CreatedAt = DateTimeOffset.UtcNow,
                MetadataUuid = metadataUuid
            };
            await eventStoreContext.Set<EventStore>().AddAsync(eventStore, cancellationToken);
        }
    }

    private async Task<EventStore?> GetEventStoreAsync(Guid identifier, CancellationToken cancellationToken)
    {
        var eventStore = await eventStoreContext.Set<EventStore>()
            .Include(es => es.Metadata)
            .Include(es => es.Events).ThenInclude(o => o.Metadata)
            .Where(es => es.Uuid == identifier)
            .Select(es => new EventStore
            {
                Uuid = es.Uuid,
                CreatedAt = es.CreatedAt,
                MetadataUuid = es.MetadataUuid,
                Metadata = new Metadata
                {
                    Uuid = es.Metadata.Uuid,
                    CreatedAt = es.Metadata.CreatedAt,
                    AssemblyName = es.Metadata.AssemblyName,
                    NamespaceName = es.Metadata.NamespaceName,
                    ClassName = es.Metadata.ClassName,
                },
                Events = es.Events.Select(e => new Event
                {
                    Uuid = e.Uuid,
                    CreatedAt = e.CreatedAt,
                    MetadataUuid = e.MetadataUuid,
                    Metadata = new Metadata
                    {
                        Uuid = e.Metadata.Uuid,
                        CreatedAt = e.Metadata.CreatedAt,
                        AssemblyName = e.Metadata.AssemblyName,
                        NamespaceName = e.Metadata.NamespaceName,
                        ClassName = e.Metadata.ClassName,
                    },
                    Version = e.Version,
                    Data = e.Data
                }).OrderBy(o => o.Version).ToList()
            }).SingleOrDefaultAsync(cancellationToken);

        return eventStore;
    }

    private async Task CreateEventsAsync(Guid eventStoreUuid, IReadOnlyCollection<DomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        var events = new List<Event>();

        foreach (var domainEvent in domainEvents)
        {
            var metadataUuid = await GetMetadataAsync(domainEvent, cancellationToken);

            var @event = new Event
            {
                Uuid = Guid.NewGuid(),
                CreatedAt = DateTimeOffset.UtcNow,
                MetadataUuid = metadataUuid,
                EventStoreUuid = eventStoreUuid,
                Data = SerializeObject(domainEvent),
                Version = domainEvent.Version,
            };

            events.Add(@event);
        }

        var setEvent = eventStoreContext.Set<Event>();
        await setEvent.AddRangeAsync(events, cancellationToken);
    }

    private async Task<Guid> GetMetadataAsync<T>(CancellationToken cancellationToken)
    {
        var type = typeof(T);
        return await GetMetadataAsync(type, cancellationToken);
    }

    private async Task<Guid> GetMetadataAsync(object @object, CancellationToken cancellationToken)
    {
        var type = @object.GetType();
        return await GetMetadataAsync(type, cancellationToken);
    }

    private async Task<Guid> GetMetadataAsync(Type type, CancellationToken cancellationToken)
    {
        var assemblyName = type.Assembly.FullName;
        var namespaceName = type.Namespace;
        var className = type.Name;

        var setMetadata = eventStoreContext.Set<Metadata>();
        var metadata = setMetadata.SingleOrDefault(o => o.AssemblyName == assemblyName && o.NamespaceName == namespaceName && o.ClassName == className);

        if (metadata is null)
        {
            metadata = new Metadata
            {
                Uuid = Guid.NewGuid(),
                CreatedAt = DateTimeOffset.UtcNow,
                AssemblyName = assemblyName,
                NamespaceName = namespaceName,
                ClassName = className,
            };

            await setMetadata.AddAsync(metadata, cancellationToken);
        }

        return metadata.Uuid;
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