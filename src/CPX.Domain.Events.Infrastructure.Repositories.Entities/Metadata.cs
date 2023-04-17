namespace CPX.Domain.Events.Infrastructure.Repositories.Entities;

public sealed class Metadata : Entity
{
    public string ClassName { get; set; }

    public string NamespaceName { get; set; }

    public string AssemblyName {get; set;}

    public ICollection<EventStore> EventsStores { get; set; }

    public ICollection<Event> Events { get; set; }
}
