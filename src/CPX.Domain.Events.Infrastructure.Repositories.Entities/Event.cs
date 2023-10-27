namespace CPX.Domain.Events.Infrastructure.Repositories.Entities;

public sealed class Event : Entity
{
    public Guid MetadataUuid { get; set; }

    public Metadata? Metadata { get; set; }

    public Guid EventStoreUuid { get; set; }

    public EventStore? EventStore { get; set; }

    public string? Data { get; set; }

    public int Version { get; set; }
}