namespace CPX.Events.Infrastructure.Repositories.Entities;

public sealed class Event : Entity
{
    public Guid MetadataUuid { get; set; }

    public Metadata? Metadata { get; set; }

    public Guid AggregateUuid { get; set; }

    public Aggregate? Aggregate { get; set; }

    public string? Data { get; set; }

    public int Version { get; set; }
}