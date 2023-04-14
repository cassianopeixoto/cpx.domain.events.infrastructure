using System.Collections.Generic;

namespace CPX.Events.Infrastructure.Repository.Entities;

public sealed class Metadata : Entity
{
    public string ClassName { get; set; }
    
    public string NamespaceName { get; set; }

    public ICollection<Aggregate> Aggregates { get; set; }

    public ICollection<Event> Events { get; set; }
}
