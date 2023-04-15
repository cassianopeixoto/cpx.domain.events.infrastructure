namespace CPX.Events.Infrastructure.Repositories.Entities;

public sealed class EventStore : Entity
{
    public Guid MetadataUuid { get; set; }

    public Metadata? Metadata { get; set; }

    public ICollection<Event>? Events { get; set; }
}