namespace CPX.Events.Infrastructure.Repositories.Entities;

public sealed class Aggregate : Entity
{
    public Guid MetadataUuid { get; set; }

    public Metadata? Metadata { get; set; }

    public ICollection<Event>? Events { get; set; }
}